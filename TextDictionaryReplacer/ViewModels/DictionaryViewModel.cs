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
        private ObservableCollection<DictionaryPairViewModel> _dictionaryItems;
        public ObservableCollection<DictionaryPairViewModel> DictionaryItems
        {
            get => _dictionaryItems;
            set => RaisePropertyChanged(ref _dictionaryItems, value);
        }

        public string DictionaryPreview
        {
            get => _dictionaryPreview;
            set => RaisePropertyChanged(ref _dictionaryPreview, value);
        }

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

        private bool _matchWholeWords;
        public bool MatchWholeWords
        {
            get => _matchWholeWords;
            set => RaisePropertyChanged(ref _matchWholeWords, value);
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

        private bool _isNotLoadingDictionaryFromFile;
        private string _dictionaryPreview;

        public bool IsNotLoadingDictionaryFromFile
        {
            get => _isNotLoadingDictionaryFromFile;
            set => RaisePropertyChanged(ref _isNotLoadingDictionaryFromFile, value);
        }

        public ICommand AddKeyPairCommand { get; }
        public ICommand ClearAllPairsCommand { get; }
        public ICommand ShowSearchFilePathCommand { get; }
        public ICommand LoadDictionaryFromPathCommand { get; }

        public DictionaryViewModel()
        {
            DictionaryItems = new ObservableCollection<DictionaryPairViewModel>(new List<DictionaryPairViewModel>(1000));
            DictionaryPreview = "";
            AddKeyPairCommand = new Command(AddKeyPair);
            ClearAllPairsCommand = new Command(ClearKeyPairs);
            ShowSearchFilePathCommand = new Command(ShowSearchForFilePathDialog);
            LoadDictionaryFromPathCommand= new Command(LoadDictionaryFromPath);

            IsNotLoadingDictionaryFromFile = true;
        }

        public static string FormatPair(string replace, string with)
        {
            return $"'{replace}' --> '{with}'";
        }

        public void AddKeyPair()
        {
            if (!Replace.IsEmpty() && !With.IsEmpty())
            {
                DictionaryPairViewModel pair = new DictionaryPairViewModel();
                pair.Replace = Replace;
                pair.With = With;
                DictionaryPreview += FormatPair(pair.Replace, pair.With) + '\n';

                SetupDictionaryCallbacks(pair);
                AddKeyPair(pair);
            }
        }

        public void AddKeyPair(string replace, string with)
        {
            if (!replace.IsEmpty() && !with.IsEmpty())
            {
                DictionaryPairViewModel pair = new DictionaryPairViewModel(replace, with);
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
                    IsNotLoadingDictionaryFromFile = false;
                    string[] dictionary = File.ReadAllLines(LoadFilePath);
                    // extra 10 just incase
                    List<DictionaryPairViewModel> pairs = new List<DictionaryPairViewModel>(dictionary.Length + 10);
                    StringBuilder preview = new StringBuilder(dictionary.Length * 50);
                    // possibly some thead safe issues here maybe but idk i had no problems
                    foreach(DictionaryPairViewModel pair in DictionaryItems)
                    {
                        pairs.Add(pair);
                        preview.AppendLine(FormatPair(pair.Replace, pair.With));
                    }

                    for (int dictLnIndex = 0; dictLnIndex < dictionary.Length; dictLnIndex++)
                    {
                        string line = dictionary[dictLnIndex];
                        string formatted = line.CollapseSpaces().TrimStart().TrimEnd();
                        char separator = KeyValueSeparator?.Length > 0 ? ' ' : KeyValueSeparator[0];
                        string[] pair = formatted.Split(separator);
                        if (pair.Length >= 2)
                        {
                            string replace = pair[0];
                            string with = pair[1];
                            if (!replace.IsEmpty() && !with.IsEmpty())
                            {
                                DictionaryPairViewModel newPair = new DictionaryPairViewModel(replace, with);
                                SetupDictionaryCallbacks(newPair);
                                pairs.Add(newPair);
                                preview.AppendLine(FormatPair(newPair.Replace, newPair.With));
                            }

                            await Task.Delay(TimeSpan.FromMilliseconds(0.05));
                        }
                    }
                    IsNotLoadingDictionaryFromFile = true;

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        DictionaryItems = new ObservableCollection<DictionaryPairViewModel>(pairs);
                        DictionaryPreview += preview.ToString();
                    });
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
