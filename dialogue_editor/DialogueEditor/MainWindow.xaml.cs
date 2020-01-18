using floofy;
using Microsoft.Win32;
using NetworkUI;
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

        //private Point currentPoint = new Point();

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
        }

        public MainWindowViewModel ViewModel
        {
            get
            {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        /// <summary>
        /// Event raised when the user has started to drag out a connection.
        /// </summary>
        private void networkControl_ConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e)
        {
            var draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            var curDragPoint = Mouse.GetPosition(networkControl);

            //
            // Delegate the real work to the view model.
            //
            var connection = this.ViewModel.ConnectionDragStarted(draggedOutConnector, curDragPoint);

            //
            // Must return the view-model object that represents the connection via the event args.
            // This is so that NetworkView can keep track of the object while it is being dragged.
            //
            e.Connection = connection;
        }

        /// <summary>
        /// Event raised while the user is dragging a connection.
        /// </summary>
        private void networkControl_ConnectionDragging(object sender, ConnectionDraggingEventArgs e)
        {
            var curDragPoint = Mouse.GetPosition(networkControl);
            var connection = (ConnectionViewModel)e.Connection;
            this.ViewModel.ConnectionDragging(connection, curDragPoint);
        }

        /// <summary>
        /// Event raised when the user has finished dragging out a connection.
        /// </summary>
        private void networkControl_ConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
        {
            var connectorDraggedOut = (ConnectorViewModel)e.ConnectorDraggedOut;
            var connectorDraggedOver = (ConnectorViewModel)e.ConnectorDraggedOver;
            var newConnection = (ConnectionViewModel)e.Connection;
            this.ViewModel.ConnectionDragCompleted(newConnection, connectorDraggedOut, connectorDraggedOver);
        }

        /// <summary>
        /// Event raised to delete the selected node.
        /// </summary>
        private void DeleteSelectedNodes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.DeleteSelectedNodes();
        }

        /// <summary>
        /// Event raised to create a new node.
        /// </summary>
        private void CreateNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Point newNodeLocation = Mouse.GetPosition(networkControl);
            this.ViewModel.CreateNode("New Node!", newNodeLocation);
        }

        //private void Canvas_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    if (e.ButtonState == MouseButtonState.Pressed)
        //    {
        //        currentPoint = e.GetPosition(graphCanvas);
        //    }
        //}

        //private void Canvas_MouseMove_1(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        Line line = new Line
        //        {
        //            Stroke = SystemColors.WindowFrameBrush,
        //            X1 = currentPoint.X,
        //            Y1 = currentPoint.Y,
        //            X2 = e.GetPosition(graphCanvas).X,
        //            Y2 = e.GetPosition(graphCanvas).Y
        //        };

        //        currentPoint = e.GetPosition(graphCanvas);

        //        graphCanvas.Children.Add(line);
        //    }
        //}

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