using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TextDictionaryReplacer.Dictionaries;
using TextDictionaryReplacer.Utilities;

namespace TextDictionaryReplacer.ViewModels
{
    public class DictionaryViewModel : BaseViewModel
    {
        public ObservableCollection<DictionaryPairViewModel> DictionaryItems { get; set; }

        private string _replace;
        public string Replace
        {
            get => _replace;
            set => RaisePropertyChanged(ref _replace, value);
        }

        private string _with;
        public string With
        {
            get => _with;
            set => RaisePropertyChanged(ref _with, value);
        }

        private bool _caseSensitive;
        public bool CaseSensitive
        {
            get => _caseSensitive;
            set => RaisePropertyChanged(ref _caseSensitive, value);
        }

        private string _loadFilePath;
        public string LoadFilePath
        {
            get => _loadFilePath;
            set => RaisePropertyChanged(ref _loadFilePath, value);
        }

        private string _keyValueSeparator;
        public string KeyValueSeparator
        {
            get => _keyValueSeparator;
            set => RaisePropertyChanged(ref _keyValueSeparator, value);
        }

        public ICommand AddKeyPairCommand { get; }
        public ICommand ClearAllPairsCommand { get; }
        public ICommand ShowSearchFilePathCommand { get; }
        public ICommand LoadDictionaryFromPathCommand { get; }

        public DictionaryViewModel()
        {
            DictionaryItems = new ObservableCollection<DictionaryPairViewModel>();

            AddKeyPairCommand = new Command(AddKeyPair);
            ClearAllPairsCommand = new Command(ClearKeyPairs);
            ShowSearchFilePathCommand = new Command(ShowSearchForFilePathDialog);
            LoadDictionaryFromPathCommand= new Command(LoadDictionaryFromPath);
        }

        public void AddKeyPair()
        {
            if (!Replace.IsEmpty() && !With.IsEmpty())
            {
                DictionaryPairViewModel pair = new DictionaryPairViewModel();
                pair.Replace = Replace;
                pair.With = With;

                SetupDictionaryCallbacks(pair);
                AddKeyPair(pair);
            }
        }

        public void AddKeyPair(string replace, string with)
        {
            if (!replace.IsEmpty() && !with.IsEmpty())
            {
                DictionaryPairViewModel pair = new DictionaryPairViewModel();
                pair.Replace = replace;
                pair.With = with;

                SetupDictionaryCallbacks(pair);
                AddKeyPair(pair);
            }
        }

        public void AddKeyPair(DictionaryPairViewModel pair)
        {
            DictionaryItems.Add(pair);
        }

        public void RemoveKeyPair(DictionaryPairViewModel pair)
        {
            DictionaryItems.Remove(pair);
        }

        public void ClearKeyPairs()
        {
            DictionaryItems.Clear();
        }

        public void SetupDictionaryCallbacks(DictionaryPairViewModel pair)
        {
            pair.RemoveItemCallback = RemoveKeyPair;
        }

        public void ShowSearchForFilePathDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Search for the dictionary file";
            ofd.Filter = "Text (.txt)|*.txt*| All Files (*)|*.*";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == true)
            {
                LoadFilePath = ofd.FileName;
            }
        }

        public void LoadDictionaryFromPath()
        {
            if (File.Exists(LoadFilePath))
            {
                Task.Run(async() =>
                {
                    foreach (string line in File.ReadAllLines(LoadFilePath))
                    {
                        string formatted = line.CollapseSpaces().TrimStart().TrimEnd();
                        string[] pair = formatted.Split(' ');
                        if (pair.Length >= 2)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                AddKeyPair(pair[0], pair[1]);
                            });
                        }
                        await Task.Delay(1);
                    }
                });

                // Will use this eventually so that the entire file isn't loaded into memory
                //const int chunkSize = 1024; // read the file by chunks of 1KB
                //using (var file = File.OpenRead("foo.dat"))
                //{
                //    int bytesRead;
                //    var buffer = new byte[chunkSize];
                //    while ((bytesRead = file.Read(buffer, 0, buffer.Length)) > 0)
                //    {
                //        
                //    }
                //}
            }
        }
    }
}
