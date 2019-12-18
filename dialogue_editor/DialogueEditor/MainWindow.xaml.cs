using floofy;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace DialogueEditor
{
    public class DialogueEntry
    {
        public string DialogueName { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DialogueManager _mgr = null;
        private string _currentFile = null;

        public ObservableCollection<DialogueEntry> DlgEntries { get; set; }

        private bool Dirty { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _mgr = new DialogueManager();
            DlgEntries = new ObservableCollection<DialogueEntry>();
            Dirty = false;
            DataContext = this;
        }

        private void PopulateDialogueList()
        {
            DlgEntries.Clear();
            if (_mgr == null)
            {
                Debug.Assert(false);
                return;
            }

            for (int i = 0; i < _mgr.NumDialogues; ++i)
            {
                DlgEntries.Add(new DialogueEntry { DialogueName = _mgr.Dialogue(i).Name });
            }
        }

        private bool Save()
        {
            if (_currentFile == null)
            {
                return SaveAs();
            }
            else
            {
                if (!_mgr.Write(_currentFile))
                {
                    MessageBox.Show("An error occurred while saving the file.", "Error Saving");
                    return false;
                }
                Dirty = false;
                return true;
            }
        }

        private bool SaveAs()
        {
            var saveFileDlg = new SaveFileDialog()
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (saveFileDlg.ShowDialog() == true)
            {
                _currentFile = saveFileDlg.FileName;
                return Save();
            }

            return false;
        }

        private void AddNewDialogue_Click(object sender, RoutedEventArgs e)
        {
            Dirty = true;

            DlgEntries.Add(new DialogueEntry { DialogueName = "Hello World" });
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var openFileDlg = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDlg.ShowDialog() == true)
            {
                _mgr = DialogueManager.Load(openFileDlg.FileName);
                if (_mgr == null)
                {
                    _currentFile = openFileDlg.FileName;
                }
                else
                {
                    _currentFile = null;
                    MessageBox.Show(string.Format("Failed to load {0}", openFileDlg.FileName), "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _mgr = new DialogueManager();
                }
            }

            PopulateDialogueList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (Dirty)
            {
                var res = MessageBox.Show("All unsaved changes will be lost. Do you want your changes to be saved?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        if (Save())
                        {
                            Close();
                        }
                        break;

                    case MessageBoxResult.No:
                        Close();
                        break;

                    case MessageBoxResult.Cancel:
                        break;
                }
            }
            else
            {
                Close();
            }
        }
    }
}