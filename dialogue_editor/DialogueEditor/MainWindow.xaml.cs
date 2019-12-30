using floofy;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;

namespace DialogueEditor
{
    #region Commands

    public class UndoCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private CommandExecutor _cmdExec;

        public UndoCommand(CommandExecutor cmdExec)
        {
            _cmdExec = cmdExec;
        }

        public bool CanExecute(object parameter)
        {
            return _cmdExec != null && _cmdExec.UndoCount > 0;
        }

        public void Execute(object parameter)
        {
            _cmdExec.UndoLatest();
        }
    }

    public class RedoCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private CommandExecutor _cmdExec;

        public RedoCommand(CommandExecutor cmdExec)
        {
            _cmdExec = cmdExec;
        }

        public bool CanExecute(object parameter)
        {
            return _cmdExec != null && _cmdExec.RedoCount > 0;
        }

        public void Execute(object parameter)
        {
            _cmdExec.RedoLatest();
        }
    }

    public class SaveCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public SaveCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
        }
    }

    public class SaveAsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public SaveAsCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
        }
    }

    #endregion Commands

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DialogueModel DlgModel { get; set; }
        public ICommand UndoCommand { get { return _undoCommand; } }
        public ICommand RedoCommand { get { return _redoCommand; } }
        public ICommand SaveCommand { get { return _saveCommand; } }
        public ICommand SaveAsCommand { get { return _saveAsCommand; } }

        private CommandExecutor _cmdExec;
        private ICommand _undoCommand;
        private ICommand _redoCommand;
        private ICommand _saveCommand;
        private ICommand _saveAsCommand;
        private string _currentFile = null;
        private bool Dirty { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _cmdExec = new CommandExecutor();

            DlgModel = new DialogueModel(_cmdExec);

            _undoCommand = new UndoCommand(_cmdExec);
            _redoCommand = new RedoCommand(_cmdExec);
            _saveCommand = new SaveCommand();
            _saveAsCommand = new SaveAsCommand();
            Dirty = false;
            DataContext = this;
        }

        private bool Save()
        {
            if (_currentFile == null)
            {
                return SaveAs();
            }
            else
            {
                if (!DlgModel.Save(_currentFile))
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

            DlgModel.Add("Hello World");
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
                var mgr = DialogueManager.Load(openFileDlg.FileName);
                if (mgr == null)
                {
                    MessageBox.Show(string.Format("Failed to load {0}", openFileDlg.FileName), "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    _currentFile = openFileDlg.FileName;
                    DlgModel = new DialogueModel(mgr, _cmdExec);
                }
            }
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

        private void OpenDialogue(string dialogueName)
        {
        }

        private void DialogueList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as DialogueItem;
            if (item != null)
            {
                OpenDialogue(item.DialogueName);
            }
        }

        private void Rename_Dialogue_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as DialogueItem;
            if (item != null)
            {
            }
        }

        private void Delete_Dialogue_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as DialogueItem;
            if (item != null)
            {
                DlgModel.Remove(item.DialogueName);
            }
        }
    }
}