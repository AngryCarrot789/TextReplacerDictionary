using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TextDictionaryReplacer.Replacing;
using TextDictionaryReplacer.Utilities;

namespace TextDictionaryReplacer.ViewModels
{
    public class ReplacingViewModel : BaseViewModel
    {
        public ObservableCollection<FileViewModel> Files { get; set; }

        public ICommand OpenFileCommand { get; }
        public ICommand OpenFolderCommand { get; }
        public ICommand ClearFilesCommand { get; }

        private DictionaryViewModel Dictionary { get; set; }

        public ReplacingViewModel(DictionaryViewModel dictionary)
        {
            Dictionary = dictionary;

            Files = new ObservableCollection<FileViewModel>();

            OpenFileCommand = new Command(OpenAndAddFileFromExplorer);
            OpenFolderCommand = new Command(OpenAndAddFolderFilesFromExplorer);
            ClearFilesCommand = new Command(ClearFiles);
        }

        public void OpenAndAddFileFromExplorer()
        {

        }

        public void OpenAndAddFolderFilesFromExplorer()
        {

        }

        public void ClearFiles()
        {
            Files.Clear();
        }
    }
}
