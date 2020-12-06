using Microsoft.Win32;
using NetworkUI;
using System;
using System.Windows;
using System.Windows.Input;

namespace DialogueEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            networkControl.IsEnabled = false;
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

        private void CanCreateNode(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.DlgModel != null && ViewModel.DlgModel.Network != null && ViewModel.DlgModel.Network.Participants.Count != 0;
        }

        private void AddNewDialogue_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewDialogue();
        }

        private void AddNewParticipant_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.DlgModel.Network != null)
            {
                ViewModel.DlgModel.Network.AddNewParticipant();
            }
        }

        private void AddNewChoice_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ChoiceModel != null)
            {
                ViewModel.ChoiceModel.Add("New Choice" + ViewModel.ChoiceModel.ChoiceItems.Count);
            }
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
                ViewModel.OpenFile(openFileDlg.FileName);
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var item = (System.Windows.Controls.MenuItem)(e.OriginalSource);
            ViewModel.OpenFile(item.DataContext.ToString());
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Save();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveAs();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Dirty)
            {
                var res = MessageBox.Show("All unsaved changes will be lost. Do you want your changes to be saved?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        if (ViewModel.Save())
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

        private void DialogueList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as DialogueViewModel;
            if (item != null)
            {
                ViewModel.OpenDialogue(item.Name);
                networkControl.IsEnabled = true;
            }
        }

        private void Rename_Dialogue_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as DialogueViewModel;
            if (item != null)
            {
                // Instantiate the dialog box
                var dlg = new TextInputDialog
                {
                    // Configure the dialog box
                    Owner = this,
                    Text = item.Name
                };

                // Open the dialog box modally
                dlg.ShowDialog();

                if (dlg.DialogResult == true)
                {
                    item.Name = dlg.Text;
                }
            }
        }

        private void Delete_Dialogue_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as DialogueViewModel;
            if (item != null)
            {
                ViewModel.RemoveItem(item.Name);
            }
        }

        private void Rename_Participant_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as ParticipantViewModel;
            if (item != null)
            {
                // Instantiate the dialog box
                var dlg = new TextInputDialog
                {
                    // Configure the dialog box
                    Owner = this,
                    Text = item.Name
                };

                // Open the dialog box modally
                dlg.ShowDialog();

                if (dlg.DialogResult == true)
                {
                    item.Name = dlg.Text;
                }
            }
        }

        private void Delete_Participant_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as ParticipantViewModel;
            if (item != null)
            {
                ViewModel.DlgModel.Network.Participants.Remove(item);
            }
        }

        private void Rename_Choice_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as ChoiceViewModel;
            if (item != null)
            {
                // Instantiate the dialog box
                var dlg = new TextInputDialog
                {
                    // Configure the dialog box
                    Owner = this,
                    Text = item.Name
                };

                // Open the dialog box modally
                dlg.ShowDialog();

                if (dlg.DialogResult == true)
                {
                    item.Name = dlg.Text;
                }
            }
        }

        private void Delete_Choice_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as ChoiceViewModel;
            if (item != null)
            {
                ViewModel.ChoiceModel.Remove(item.Name);
            }
        }
    }
}