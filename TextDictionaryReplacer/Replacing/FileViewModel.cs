using System.IO;
using System.Windows.Forms;
using TextDictionaryReplacer.Utilities;

namespace TextDictionaryReplacer.Replacing
{
    public class FileViewModel : BaseViewModel
    {
        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set => RaisePropertyChanged(ref _fileName, value);
        }

        private string _fileSize;
        public string FileSize
        {
            get => _fileSize;
            set => RaisePropertyChanged(ref _fileSize, value);
        }

        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set => RaisePropertyChanged(ref _filePath, value);
        }

        private string _text;
        public string Text
        {
            get => _text;
            set => RaisePropertyChanged(ref _text, value);
        }

        public void LoadText()
        {
            if (File.Exists(FilePath))
            {
                Text = File.ReadAllText(FilePath);
            }
        }

        public void SaveText()
        {
            if (File.Exists(FilePath))
            {
                if (Text.IsEmpty())
                {
                    DialogResult result = MessageBox.Show("Text is empty. Save Anyway?", "Text Empty", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        File.WriteAllText(FilePath, Text);
                    }
                }
                else
                {
                    File.WriteAllText(FilePath, Text);
                }
            }
        }
    }
}
