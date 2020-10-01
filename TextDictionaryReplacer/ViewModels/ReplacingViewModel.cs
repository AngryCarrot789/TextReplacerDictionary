using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TextDictionaryReplacer.Dictionaries;
using TextDictionaryReplacer.Replacing;
using TextDictionaryReplacer.Utilities;

namespace TextDictionaryReplacer.ViewModels
{
    public class ReplacingViewModel : BaseViewModel
    {
        public ObservableCollection<FileViewModel> Files { get; set; }

        private bool _searchRecursively;
        public bool SearchRecursively
        {
            get => _searchRecursively;
            set => RaisePropertyChanged(ref _searchRecursively, value);
        }

        private bool _isSearching;
        public bool IsSearching
        {
            get => _isSearching;
            set => RaisePropertyChanged(ref _isSearching, value);
        }

        private int _filesLeftToSearch;
        public int FilesLeftToSearch
        {
            get => _filesLeftToSearch;
            set => RaisePropertyChanged(ref _filesLeftToSearch, value);
        }

        private FileViewModel _selectedFile;
        public FileViewModel SelectedFile
        {
            get => _selectedFile;
            set => RaisePropertyChanged(ref _selectedFile, value);
        }

        public ICommand OpenFileCommand { get; }
        public ICommand OpenFolderCommand { get; }
        public ICommand SaveAllFilesCommand { get; }
        public ICommand ClearFilesCommand { get; }
        public ICommand AutoStartSearchCommand { get; }

        public DictionaryViewModel Dictionary { get; set; }

        public ReplacingViewModel(DictionaryViewModel dictionary)
        {
            Dictionary = dictionary;

            Files = new ObservableCollection<FileViewModel>(new List<FileViewModel>(1000));

            OpenFileCommand = new Command(OpenAndAddFileFromExplorer);
            OpenFolderCommand = new Command(OpenAndAddFolderFilesFromExplorer);
            SaveAllFilesCommand = new Command(SaveAllFiles);
            ClearFilesCommand = new Command(ClearFiles);
            AutoStartSearchCommand = new Command(AutoSearch);
        }

        public void AutoSearch()
        {
            if (IsSearching)
                StopSearching();
            else
                StartSearching();
        }

        public void StartSearching()
        {
            FilesLeftToSearch = Files.Count;
            IsSearching = true;

            bool matchWord = Dictionary.MatchWholeWords;
            bool caseSensitive = Dictionary.CaseSensitive;

            Task.Run(async() =>
            {
                for (int fIndex = 0; fIndex < Files.Count; fIndex++)
                {
                    FileViewModel file = Files[fIndex];
                    if (!IsSearching) return;
                    if (!file.Text.IsEmpty())
                    {
                        for (int pairIndex = 0; pairIndex < Dictionary.DictionaryItems.Count; pairIndex++)
                        {
                            DictionaryPairViewModel pair = Dictionary.DictionaryItems[pairIndex];
                            string oldText = caseSensitive ? file.Text : file.Text.ToLower();
                            string replace = caseSensitive ? pair.Replace : pair.Replace.ToLower();
                            string with = caseSensitive ? pair.With : pair.With.ToLower();

                            if (matchWord)
                                file.Text = oldText.ReplaceFullWords(replace, with);
                            else
                                file.Text = oldText.Replace(replace, with);
                            if (!IsSearching) return;
                        }

                        FilesLeftToSearch -= 1;

                        await Task.Delay(TimeSpan.FromMilliseconds(0.2));
                    }
                }

                StopSearching();
            });
        }

        public void StopSearching()
        {
            IsSearching = false;
        }

        private void SaveAllFiles()
        {
            foreach(FileViewModel file in Files)
            {
                file.SaveText();
            }
        }

        public void AddFile(string path)
        {
            FileViewModel file = new FileViewModel();
            file.FilePath = path;
            file.LoadText();

            SetupFileCallbacks(file);
            AddFile(file);
        }

        public void AddFile(FileViewModel file)
        {
            Files.Add(file);
        }

        public void RemoveFile(FileViewModel file)
        {
            Files.Remove(file);
        }

        public void OpenAndAddFileFromExplorer()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Search for the dictionary file";
            ofd.Filter = "All Files (*)|*.*";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == true)
            {
                Task.Run(async() =>
                {
                    foreach (string file in ofd.FileNames)
                    {
                        if (File.Exists(file))
                        {
                            Application.Current?.Dispatcher?.Invoke(() =>
                            {
                                AddFile(file);
                            });

                            await Task.Delay(1);
                        }
                    }
                });
            }
        }

        public void OpenAndAddFolderFilesFromExplorer()
        {
            VistaFolderBrowserDialog vfbd = new VistaFolderBrowserDialog();
            vfbd.UseDescriptionForTitle = true;
            vfbd.Description = "Open all files in this folder and sub-folders";

            if (vfbd.ShowDialog() == true)
            {
                Task.Run(async () =>
                {
                    // Recursively search for all folders
                    if (SearchRecursively)
                    {

                        async void LocalOpenFilesAndFoldersInFolder(string path)
                        {
                            foreach (string folder in Directory.GetDirectories(path))
                            {
                                if (Directory.Exists(folder))
                                {
                                    LocalOpenFilesAndFoldersInFolder(folder);
                                }
                            }

                            foreach (string file in Directory.GetFiles(path))
                            {
                                if (File.Exists(file))
                                {
                                    Application.Current?.Dispatcher?.Invoke(() =>
                                    {
                                        AddFile(file);
                                    });

                                    // need to add a delay between adding files
                                    // otherwise the UI will freeze due to 1000s
                                    // of items being added per second
                                    await Task.Delay(2);
                                }
                            }
                        }

                        LocalOpenFilesAndFoldersInFolder(vfbd.SelectedPath);
                    }

                    // Search only this folder for files (no sub folders)
                    else
                    {
                        foreach (string file in Directory.GetFiles(vfbd.SelectedPath))
                        {
                            Application.Current?.Dispatcher?.Invoke(() =>
                            {
                                AddFile(file);
                            });

                            await Task.Delay(2);
                        }
                    }
                });
            }
        }

        public void ClearFiles()
        {
            Files.Clear();
        }

        public void SetupFileCallbacks(FileViewModel file)
        {
            file.RemoveFileCallback = RemoveFile;
        }
    }
}
