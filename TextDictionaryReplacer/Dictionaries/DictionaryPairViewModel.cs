using System;
using System.Windows.Input;
using TextDictionaryReplacer.Utilities;

namespace TextDictionaryReplacer.Dictionaries
{
    public class DictionaryPairViewModel : BaseViewModel
    {
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

        public ICommand RemoveItemCommand { get; }

        public Action<DictionaryPairViewModel> RemoveItemCallback { get; set; }

        public DictionaryPairViewModel()
        {
            RemoveItemCommand = new Command(RemoveItem);
        }

        public void RemoveItem()
        {
            RemoveItemCallback?.Invoke(this);
        }
    }
}
