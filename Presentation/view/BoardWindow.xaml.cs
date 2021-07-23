using System.Windows;
using Presentation.model;
using Presentation.viewModel;

namespace Presentation.view
{
    public partial class BoardWindow : Window
    {
        private BoardViewModel boardViewModel;
        internal BoardViewModel ViewModel { get => boardViewModel; set => boardViewModel = value; }

        public BoardWindow(UserModel u)
        {
            InitializeComponent();
            this.DataContext = new BoardViewModel(u);
            this.boardViewModel = (BoardViewModel)DataContext;
        }
        
        /// <summary> a button click to- Log out an logged in user </summary>
       private void Logout_click(object sender, RoutedEventArgs e)
        {
            boardViewModel.logout();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        /// <summary> a button click to- add a column to the board </summary>
        private void AddColumn(object sender, RoutedEventArgs e)
        {
            
            boardViewModel.AddColumn(ColumnTitle.Text,ColumnOrdinal.Text);
        }
        
        /// <summary> a button click to- delete a column from the board </summary>
        private void DeleteColumn(object sender, RoutedEventArgs e)
        {
            boardViewModel.removeColumn();
        }
        
        /// <summary> a button click to- limit a column </summary>
        private void LimitColumn(object sender, RoutedEventArgs e)
        {
            boardViewModel.limitColumnsTask(ColumnLimit.Text);
        }
        
        /// <summary> a button click to- move a column to the right </summary>
        private void MoveRight(object sender, RoutedEventArgs e)
        {
            boardViewModel.MoveColRight();
        }
        
        /// <summary> a button click to- move a column to the left </summary>
        private void MoveLeft(object sender, RoutedEventArgs e)
        {
            boardViewModel.MoveLeft();
        }

        /// <summary> a button click to- advance a task to the next column </summary>
        private void AdvanceTask(object sender, RoutedEventArgs e)
        {
            boardViewModel.AdvanceTask();
        }

        /// <summary> a button click to- delete a task form the board </summary>

        private void deleteTask(object sender, RoutedEventArgs e)
        {
            boardViewModel.deleteTask();
        }
        
        /// <summary> a button click to- add a task to the board </summary>
        private void AddTask(object sender, RoutedEventArgs e)
        {
            /*AddTaskWindow adw=new AddTaskWindow(ViewModel.Controller,this);
            adw.Show();*/
            boardViewModel.addTask(taskTitle.Text,datePickerBoard.Text,description.Text);
        }
        
        /// <summary> a button click to- update title of a task </summary>
        private void UpdateTitle(object sender, RoutedEventArgs e)
        {
            boardViewModel.UpdateTaskTitle(NewTitle.Text);
        }
        
        /// <summary> a button click to- update task description </summary>
        private void UpdateDescription(object sender, RoutedEventArgs e)
        {
            boardViewModel.UpdateTaskDescription(NewDesc.Text);
        }
        
        /// <summary> a button click to- update task duedate </summary>
        private void UpdateDueDate(object sender, RoutedEventArgs e)
        {
            boardViewModel.UpdateTaskDueDate(pickNewDate.Text);
        }

        /// <summary> a button click to- assign a task to a user </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            boardViewModel.assignTask(emailAssignee.Text);
        }
    }
}