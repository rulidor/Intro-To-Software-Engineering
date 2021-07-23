using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Presentation.model;

namespace Presentation.viewModel
{
    public class BoardViewModel : NotifiableObject
    {
        public BackendController Controller;
        public UserModel User;
        
        public BoardModel _board;
        public BoardModel Board
        {
            get => _board;
            private set
            {
                _board = value;
                RaisePropertyChanged("Board");
            }
        }
        public string Title { get; private set; }
        private ColumnModel _selectedColumn;
        public ColumnModel SelectedColumn
        {
            get
            {
                return _selectedColumn;
            }
            set
            {
                _selectedColumn = value;
                EnableForwardColumn = value != null;
                if(value!=null)
                    Tasks =   new ObservableCollection<TaskModel>(Controller.getColumnTasks(User.Email,value.Name));

                RaisePropertyChanged("SelectedTask");
            }
        }
        private bool _enableForwardColumn = false;
        public bool EnableForwardColumn
        {
            get => _enableForwardColumn;
            private set
            {
                _enableForwardColumn = value;
                RaisePropertyChanged("EnableForwardColumn");
            }
        }
        private bool _enableForwardTask = false;
        public bool EnableForwardTask
        {
            get => _enableForwardTask;
            private set
            {
                _enableForwardTask = value;
                RaisePropertyChanged("EnableForwardTask");
            }
        }
        private ObservableCollection<TaskModel> _tasks;
        public ObservableCollection<TaskModel> Tasks
        {
            get => _tasks;
            set
            {
                if (value != null)
                {
                    foreach (TaskModel task in value)
                    {
                        Color c = task.getColor(User.Email);
                        task.BgColor = new SolidColorBrush(c); 
                    }
                    _tasks = value;
                    RaisePropertyChanged("Tasks");
                }
            }
        }
        private TaskModel _selectedTask;
        public TaskModel SelectedTask
        {
            get
            {
                return _selectedTask;
            }
            set
            {
                _selectedTask = value;
                EnableForwardTask = value != null;


                RaisePropertyChanged("SelectedTask");

            }
        }
        public BoardViewModel(UserModel u)
        {
            User = u;
            Controller=u.Controller;
            Title = "Kanban board for " + User.Email;
            Board = Controller.getBoard(User.Email);
        }

        public BoardViewModel(BackendController c)
        {
            Controller = c;
        }
        
        /// <summary>
        /// Add a new task.
        /// </summary>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        public void addTask(string title, string dueDate, string description)
        {
            try
            {
                this.Tasks = new ObservableCollection<TaskModel>(Controller.addTask(User.Email, title, dueDate, description));
                MessageBox.Show("Add task was successful!");

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        
        /// <summary>        
        /// Log out an logged in user. 
        /// </summary>
        public void logout()
        {
            try
            {
                Controller.logout(User.Email);
                MessageBox.Show( "Logout was Successful") ;
            }
            catch (Exception e)
            {
                MessageBox.Show("Logout error: "+e.Message);
            }
        }
        
        /// <summary>
        /// Removes a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        public void removeColumn()
        {

            try
            {
                Board = Controller.removeColumn(User.Email, getColumnOrdinal(SelectedColumn.Name));
                Tasks = null;
                SelectedColumn = null;
                EnableForwardColumn = false;
                MessageBox.Show("Remove column was successful");
            }
            catch (Exception e )
            {
                MessageBox.Show("Cannot remove message. " + e.Message);
            }
            
        }
        
        /// <summary>
        /// Adds a new column, given it's name and a location to place it.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="colOrdinal">Location to place to column</param>
        /// <param name="colName">new Column name</param>
        public void AddColumn(string columnTitle,string columnOrdinal)
        {
            try
            {
                int ord = Int32.Parse(columnOrdinal);
                Board = Controller.addColumn(User.Email,ord, columnTitle);
                MessageBox.Show("Add column was successful!");

            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot add new Column message. " + e.Message);
            }
        }
        
        /// <summary>
        /// return a column id
        /// </summary>
        /// <param name="columnName">the name of the column we want to find the id </param>
        /// <returns>return an int for the column id</returns>
        public int getColumnOrdinal(string columnName)
        {
            int i = 0;
            foreach (ColumnModel col in Board.ColumnNames)
            {
                if (col.Name == columnName)
                    return i;
                i++;
            }
            return i;
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        public void limitColumnsTask(string limit)
        {
            try
            {
                Controller.limitCol(User.Email,getColumnOrdinal(SelectedColumn.Name),Int32.Parse(limit));
                SelectedColumn.Limit = Int32.Parse(limit);
                MessageBox.Show("Limit column was successful!");

            }
            catch (Exception e)
            {
                MessageBox.Show("Limit Column task: " + e.Message);
            }
        }
        
        /// <summary>
        /// Moves a column to the right, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        public void MoveColRight()
        {

            try
            {
                Board = Controller.moveColRight(User.Email, getColumnOrdinal(SelectedColumn.Name));
                Tasks = new ObservableCollection<TaskModel>(Controller.getColumnTasks(User.Email, 0));
                MessageBox.Show("Move column right was successful!");
    
            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot move column: " + e.Message);
            }
        }
        
        /// <summary>
        /// Moves a column to the left, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column.
        /// </summary>
        public void MoveLeft()
        {

            try
            {
                Board = Controller.moveColLeft(User.Email, getColumnOrdinal(SelectedColumn.Name));
                Tasks = new ObservableCollection<TaskModel>(Controller.getColumnTasks(User.Email, 0));
                MessageBox.Show("Move column left was successful!");

            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot move column: " + e.Message);
            }
        }
        
        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        public void AdvanceTask()
        {

            try
            {
                Controller.advnacedTask(User.Email, getColumnOrdinal(SelectedColumn.Name),SelectedTask.Id);
                Tasks = new ObservableCollection<TaskModel>(Controller.getColumnTasks(User.Email, 0));
                MessageBox.Show("AdvanceTask was successful!");
            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot advance task: " + e.Message);
            }
        }
        
        /// <summary>
        /// Delete a task
        /// </summary>
        public void deleteTask()

        {
            try
            {
                Controller.deleteTask(User.Email, getColumnOrdinal(SelectedColumn.Name), SelectedTask.Id);
                MessageBox.Show("Delete task was successful!");

            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot delete column " + e.Message);
            }
        }
        
        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="title">New title for the task</param>
        public void UpdateTaskTitle(string title)
        {
            try
            {
                Controller.updateTitle(User.Email,getColumnOrdinal(SelectedColumn.Name),SelectedTask.Id,title);
                SelectedTask.Title=title;
                MessageBox.Show("Update task title was successful!");

            }
            catch (Exception e)
            {
                MessageBox.Show("Update task title: " + e.Message);
            }
        }
        
        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="desc">New description for the task</param>
        public void UpdateTaskDescription(string desc)
        {
            try
            {
                Controller.updateDescription(User.Email,getColumnOrdinal(SelectedColumn.Name),SelectedTask.Id,desc);
                SelectedTask.Description = desc;
                MessageBox.Show("Update task description was successful!");
            }
            catch (Exception e)
            {
                MessageBox.Show("Update description task: " + e.Message);
            }
        }
        
        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="dueDate">The new due date of the column</param>
        public void UpdateTaskDueDate(string dueDate)
        {
            try
            {
                DateTime date= DateTime.Parse(dueDate);
                Controller.updateDueDate(User.Email,getColumnOrdinal(SelectedColumn.Name),SelectedTask.Id,date);
                SelectedTask.DueDate = date;
                MessageBox.Show("Update task dueDate was successful!");

            }
            catch (Exception e)
            {
                MessageBox.Show("Update task dueDate: " + e.Message);
            }
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        public void assignTask( string emailAssignee)
        {
            try
            {
                Controller.taskAssignee(User.Email,getColumnOrdinal(SelectedColumn.Name),SelectedTask.Id,emailAssignee);
                MessageBox.Show("Assign task was successful!");
            }
            catch (Exception e)
            {
                MessageBox.Show("Assign task: " + e.Message);
            }
        }
        
       
    }
}