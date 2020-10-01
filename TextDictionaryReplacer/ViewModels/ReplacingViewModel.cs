using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

        private FileViewModel _selectedFile;
        public FileViewModel SelectedFile
        {
            get => _selectedFile;
            set => RaisePropertyChanged(ref _selectedFile, value);
        }

        public ICommand OpenFileCommand { get; }
        public ICommand OpenFolderCommand { get; }
        public ICommand ClearFilesCommand { get; }
        public ICommand AutoStartSearchCommand { get; }

        public DictionaryViewModel Dictionary { get; set; }

        public ReplacingViewModel(DictionaryViewModel dictionary)
        {
            Dictionary = dictionary;

            Files = new ObservableCollection<FileViewModel>();

            OpenFileCommand = new Command(OpenAndAddFileFromExplorer);
            OpenFolderCommand = new Command(OpenAndAddFolderFilesFromExplorer);
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
            IsSearching = true;
        }

        public void StopSearching()
        {
            IsSearching = false;
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
                foreach (string file in ofd.FileNames)
                {
                    AddFile(file);
                }
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
                                LocalOpenFilesAndFoldersInFolder(folder);
                            }

                            foreach (string file in Directory.GetFiles(path))
                            {
                                Application.Current?.Dispatcher?.Invoke(() =>
                                {
                                    AddFile(file);
                                });

                                // need to add a delay between adding files
                                // otherwise the UI will freeze due to 1000s
                                // of items being added per second. this should
                                // takes about 10ms to create 1 usercontrol so
                                // this limits it to around 100 elements per second
                                await Task.Delay(1);
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

                            await Task.Delay(1);
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
