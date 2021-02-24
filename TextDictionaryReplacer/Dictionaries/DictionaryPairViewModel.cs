using System;
using System.Windows.Input;
using TextDictionaryReplacer.Utilities;

namespace TextDictionaryReplacer.Dictionaries
{
    public class DictionaryPairViewModel
    {
        /// <summary>
        /// Replaces this
        /// </summary>
        public string Replace { get; set; }
        /// <summary>
        /// Replaces <see cref="Replace"/> with this
        /// </summary>
        public string With { get; set; }

        public ICommand RemoveItemCommand { get; }

        public Action<DictionaryPairViewModel> RemoveItemCallback { get; set; }

        public DictionaryPairViewModel()
        {
            RemoveItemCommand = new Command(RemoveItem);
        }

        public DictionaryPairViewModel(string replace, string with) : this()
        {
            Replace = replace;
            With = with;
        }

        public void RemoveItem()
        {
            RemoveItemCallback?.Invoke(this);
        }
    }
}
