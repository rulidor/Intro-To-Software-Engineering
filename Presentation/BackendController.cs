using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Reflection;
using System.Windows;
using Presentation.model;
using Presentation.viewModel;

namespace Presentation
{
    public class BackendController
    {
        private IService Service { get; set; }
        public BackendController(IService service)
        {
            this.Service = service;
        }

        public BackendController()
        {
            this.Service = new Service();
            Service.LoadData();
        }
        /// <summary>
        /// Registers a new user and joins the user to an existing board.
        /// </summary>
        /// <param name="email">The email address of the user to register</param>
        /// <param name="password">The password of the user to register</param>
        /// <param name="nickname">The nickname of the user to register</param>
        /// <param name="emailHost">The email address of the host user which owns the board</param>
        public void register(string email, string password, string nickName, string hostEmail)
        {
            Response u;
            if (string.IsNullOrEmpty(hostEmail)||string.IsNullOrWhiteSpace(hostEmail))
                u=Service.Register(email, password, nickName);
            else
                u = Service.Register(email, password, nickName, hostEmail);

            if (u.ErrorOccured)
            {
                throw new Exception(u.ErrorMessage);
            }
        }
        /// <summary>
        /// Log in an existing user
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        public UserModel login(string email, string password)
        {
            Response<User> user = Service.Login(email, password);
            if(user.ErrorOccured)
                throw new Exception(user.ErrorMessage);
            return new UserModel(this, email);
        }
        
        /// <summary>        
        /// Log out an logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        public void logout(string email)
        {
            Response logout = Service.Logout(email);
            if(logout.ErrorOccured)
                throw new Exception(logout.ErrorMessage);
            
        }

        ///<summary>Remove all persistent data.</summary>
        public void deleteData()
        {
            Response delete = Service.DeleteData();
            if(delete.ErrorOccured)
                throw new Exception(delete.ErrorMessage);
        }

        /// <summary>
        /// Returns the board of a user. The user must be logged in
        /// </summary>
        /// <param name="email">The email of the user</param>
        public BoardModel getBoard(string email)
        {
            Response<Board> board = Service.GetBoard(email);
            if(board.ErrorOccured)
                throw new Exception(board.ErrorMessage);
            Dictionary<string,int> list=new Dictionary<string, int>();
            foreach (string colName in board.Value.ColumnsNames)
            {
                Response<Column> col = Service.GetColumn(email, colName);
                list.Add(colName,col.Value.Limit);
            }
            return new BoardModel(this,board.Value.emailCreator,list);
        }

        /// <summary>
        /// Removes a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Column ID</param>
        public BoardModel removeColumn(string email, int columnOridinal)
        {
            Response remCol = Service.RemoveColumn(email, columnOridinal);
            if(remCol.ErrorOccured)
                throw new Exception(remCol.ErrorMessage);
            return getBoard(email);
        }

        /// <summary>
        /// Adds a new column, given it's name and a location to place it.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="colOrdinal">Location to place to column</param>
        /// <param name="colName">new Column name</param>
        public BoardModel addColumn(string email, int colOrdinal, string colName)
        {
            Response<Column> addCol = Service.AddColumn(email, colOrdinal, colName);
            if(addCol.ErrorOccured)
                throw new Exception(addCol.ErrorMessage);

            return getBoard(email);
        }
        
        /// <summary>
        /// getters for the task in a column    
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnName">column name</param>
        public List<TaskModel> getColumnTasks(string email,string columnName)
        {
            List<TaskModel> mTasks = new List<TaskModel>();
            Response<Column> col = Service.GetColumn(email, columnName);
            if (col.Value.Tasks!=null)
            {
                BoardModel b = getBoard(email);
                int colid=1;
                foreach (var column in b.ColumnNames)
                {
                    if(column.Name.Equals(columnName))
                        break;
                    colid++;
                }
                foreach (IntroSE.Kanban.Backend.ServiceLayer.Task task in col.Value.Tasks)
                {
                    mTasks.Add(new TaskModel(this,task.Id,task.CreationTime,task.DueDate,task.Title,task.Description,task.emailAssignee,email,colid));


                }
            }
            return mTasks;
        }
        
        /// <summary>
        /// getters for the task in a column    
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="colId">column ID</param>
        public List<TaskModel> getColumnTasks(string email,int colId)
        {
            List<TaskModel> mTasks = new List<TaskModel>();
            Response<Column> col = Service.GetColumn(email, colId);
            if (col.Value.Tasks!=null)
            {
                foreach (IntroSE.Kanban.Backend.ServiceLayer.Task task in col.Value.Tasks)
                {
                    mTasks.Add(new TaskModel(this,task.Id,task.CreationTime,task.DueDate,task.Title,task.Description,task.emailAssignee,email,colId));


                }
            }
            return mTasks;
        }

        /// <summary>
        /// Moves a column to the right, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the column</param>
        public BoardModel moveColRight(string email, int columnOrdinal)
        {
            Response<Column> movCol = Service.MoveColumnRight(email, columnOrdinal);
            if(movCol.ErrorOccured)
                throw new Exception(movCol.ErrorMessage);
            return getBoard(email);
            
        }

        /// <summary>
        /// Moves a column to the left, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the column</param>
        public BoardModel moveColLeft(string email, int columnOrdinal)
        {
            Response<Column> movCol = Service.MoveColumnLeft(email, columnOrdinal);
            if(movCol.ErrorOccured)
                throw new Exception(movCol.ErrorMessage);
            return getBoard(email);
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        public void advnacedTask(string email, int colId, int taskId)
        {
            Response adTask = Service.AdvanceTask(email, colId, taskId);
            if(adTask.ErrorOccured)
                throw new Exception(adTask.ErrorMessage);
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="colId">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="lim">The new limit value. A value of -1 indicates no limit.</param>
        public void limitCol(string email, int colId, int lim)
        {
            Response limRes = Service.LimitColumnTasks(email, colId, lim);
            if(limRes.ErrorOccured)
                throw new Exception(limRes.ErrorMessage);
            
        }
        
        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>  
        internal void deleteTask(string email, int columnOrdinal, int taskId)
        {
            Response res = Service.DeleteTask(email, columnOrdinal, taskId);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }
        
        /// <summary>
        /// Add a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        public List<TaskModel> addTask(string email,string title,string dueDate,string description)
        {
            try
            {
                DateTime m = DateTime.Parse(dueDate);
                Service.AddTask(email, title, description, m);
                return this.getColumnTasks(email, 0);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="colId">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        public void updateDueDate(string email, int colId, int taskId, DateTime dueDate)
        {
            Response r = Service.UpdateTaskDueDate(email, colId, taskId, dueDate);
            if(r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="colId">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        public void updateTitle(string email, int colId, int taskId, string title)
        {
            Response r = Service.UpdateTaskTitle(email, colId, taskId, title);
            if(r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="colId">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        public void updateDescription(string email, int colId, int taskId, string description)
        {
            Response r = Service.UpdateTaskDescription(email, colId, taskId, description);
            if(r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="hostemail">Email of the user. Must be logged in</param>
        /// <param name="colId">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        public void taskAssignee(string hostemail, int colId, int taskId, string emailAssignee)
        {
            Response r = Service.AssignTask(hostemail, colId, taskId, emailAssignee);
            if(r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

    }
}