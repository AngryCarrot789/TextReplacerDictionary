using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextDictionaryReplacer.Utilities;

namespace TextDictionaryReplacer.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private DictionaryViewModel _dictionary;
        public DictionaryViewModel Dictionary
        {
            get => _dictionary;
            set => RaisePropertyChanged(ref _dictionary, value);
        }

        private ReplacingViewModel _replacing;
        public ReplacingViewModel Replacing
        {
            get => _replacing;
            set => RaisePropertyChanged(ref _replacing, value);
        }

        public MainViewModel()
        {
            Dictionary = new DictionaryViewModel();
            Replacing = new ReplacingViewModel(Dictionary);
        }
    }
}
