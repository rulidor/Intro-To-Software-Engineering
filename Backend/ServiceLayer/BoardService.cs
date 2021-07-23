using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IntroSE.Kanban.Backend.BusinessLayer.Board_Package;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class BoardService
    {
        private BoardController boards;

        private static readonly log4net.ILog logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Simple public constructor.
        /// </summary>
        public BoardService()
        {
            boards = new BoardController();
        }

        /// <summary>
        /// adds a new board to a new user who just registered to the kanban system
        /// </summary>
        /// <param name="email">Email of the user.
        public void addNewBoard(string email)
        {
            this.boards.addNewBoard(email);
        }
        /// <summary>
        /// adds a new user- to a  user  subscribers list who just registered to the kanban system
        /// </summary>
        /// <param name="creator">Email of the host user.</param>
        /// <param name="newUser">Email of the subscriber </param>
        public void addUserToBoard(string creator, string newUser)
        {
            boards.addUserToBoard(creator, newUser);
        }

        /// <summary>        
        /// Loads the data. Intended be invoked only when the program starts
        /// </summary>
        public void loadData()
        {
            try
            {
                boards.loadData();
                logger.Info("load board data from dataBase successfully");
            }
            catch (Exception e)
            {
                logger.Error("error while loading data");
                throw e;
            }
        }

        public bool checkUser(string email)
        {
            return boards.checkUser(email);
        }
        /// <summary>
        /// Returns the board of a user. The user must be logged in
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>A response object with a value set to the board, instead the response should contain a error message in case of an error</returns>
        public Response<Board> GetBoard(string email)
        {
            Board newBoard;
            IReadOnlyCollection<string> ColumnsNames;
            Collection<string> cols = new Collection<string>();
            try
            {
                BusinessLayer.Board_Package.Board board = boards.getBoard(email);
                foreach (var c in board.getColumns().Values)
                {
                    cols.Add(c.getName());
                }

                ColumnsNames = cols;
                newBoard = new Board(ColumnsNames, email);
            }
            catch (Exception e)
            {
                logger.Error("GET BOARD- " + e.Message);

                return new Response<Board>(e.Message);
            }

            return new Response<Board>(newBoard, null);
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response LimitColumnTasks(string email, int columnOrdinal, int limit)
        {
            try
            {
                boards.LimitColumnTasks(email, columnOrdinal, limit);
                logger.Info(email + " limited the column " + boards.getColumn(email, columnOrdinal).getName() + "to " +
                            limit);
            }
            catch (Exception e)
            {
                logger.Error("LIMIT COLUMN- " + e.Message);
                return new Response<Exception>(e, e.Message);
            }

            return new Response();
        }

        /// <summary>
        /// Add a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="DueDate">The due date if the new task</param>
        /// <returns>A response object with a value set to the Task, instead the response should contain a error message in case of an error</returns>
        public Response<Task> AddTask(string email, string title, string description, DateTime DueDate)
        {
            Task newTask;
            try
            {
                BusinessLayer.Board_Package.Task task = boards.addTask(email, title, description, DueDate);
                newTask = new Task(task.getId(), DateTime.Now, DueDate, title, description, email);
                logger.Info(email + " added task  " + title);
            }
            catch (Exception e)
            {
                logger.Error("ADD TASK- " + e.Message);
                return new Response<Task>(e.Message);
            }

            return new Response<Task>(newTask, null);
        }

        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="DueDate">The new due date of the column</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDueDate(string email, int columnOrdinal, int taskId, DateTime DueDate)
        {
            try
            {

                boards.UpdateTaskDueDate(email, columnOrdinal, taskId, DueDate);
                
                logger.Info(email + " updated the due date of task # " + taskId);
            }
            catch (Exception e)
            {
                logger.Error("UPDATE DueDate- " + e.Message);
                return new Response<Exception>(e, e.Message);
            }

            return new Response();
        }

        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskTitle(string email, int columnOrdinal, int taskId, string title)
        {
            try
            {
                boards.UpdateTaskTitle(email, columnOrdinal, taskId, title);

                logger.Info(email + " updated the title of task # " + taskId);
            }
            catch (Exception e)
            {
                logger.Error("UPDATE TITLE- " + e.Message);
                return new Response<Exception>(e, e.Message);
            }

            return new Response();
        }

        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDescription(string email, int columnOrdinal, int taskId, string description)
        {
            try
            {
                boards.UpdateTaskDescription(email, columnOrdinal, taskId, description);

                logger.Info(email + " updated the description of task # " + taskId);
            }
            catch (Exception e)
            {
                logger.Error("UPDATE DESCRIPTION- " + e.Message);
                return new Response(e.Message);
            }

            return new Response();
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AdvanceTask(string email, int columnOrdinal, int taskId)
        {
            try
            {
                if (!boards.getBoard(email).getColumns().ContainsKey(columnOrdinal))
                    return new Response<Column>("illegal column number");
                boards.advanceTask(email, columnOrdinal, taskId);
                logger.Info(email + " advanced task # " + taskId);
            }
            catch (Exception e)
            {
                logger.Error("ADVANCE TASK- " + e.Message);
                return new Response<Exception>(e, e.Message);
            }

            return new Response();
        }


        /// <summary>
        /// Returns a column given it's name
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnName">Column name</param>
        /// <returns>A response object with a value set to the Column, The response should contain a error message in case of an error</returns>
        public Response<Column> GetColumn(string email, string columnName)
        {
            Column newColumn;
            IReadOnlyCollection<Task> tasks;
            List<Task> tasksToAdd = new List<Task>();
            try
            {
                BusinessLayer.Board_Package.Column column = boards.getColumn(email, columnName);

                foreach (BusinessLayer.Board_Package.Task task in column.getTasks().Values)
                {
                    Task temp = new Task(task.getId(), task.getCreationTime(), task.getDueDate(), task.getTitle(),
                        task.getDescription(), email);
                    tasksToAdd.Add(temp);
                }

                tasks = new ReadOnlyCollection<Task>(tasksToAdd);
                newColumn = new Column(tasks, columnName, column.getLimit());
            }
            catch (Exception e)
            {
                logger.Error("GET COLUMN- " + e.Message);
                return new Response<Column>(e.Message);
            }

            return new Response<Column>(newColumn, null);
        }

        /// <summary>
        /// Returns a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Column ID</param>
        /// <returns>A response object with a value set to the Column, The response should contain a error message in case of an error</returns>
        public Response<Column> GetColumn(string email, int columnOrdinal)
        {
            Column newColumn;
            IReadOnlyCollection<Task> tasks;
            List<Task> tasksToAdd = new List<Task>();

            try
            {
                BusinessLayer.Board_Package.Column column = boards.getColumn(email, columnOrdinal);

                foreach (BusinessLayer.Board_Package.Task task in column.getTasks().Values)
                {
                    Task temp = new Task(task.getId(), task.getCreationTime(), task.getDueDate(), task.getTitle(),
                        task.getDescription(), email);
                    tasksToAdd.Add(temp);
                }

                tasks = new ReadOnlyCollection<Task>(tasksToAdd);
                newColumn = new Column(tasks, column.getName(), column.getLimit());
            }
            catch (Exception e)
            {
                logger.Error("GET COLUMN- " + e.Message);

                return new Response<Column>(e.Message);
            }

            return new Response<Column>(newColumn, null);
        }

        /// <summary>
        /// Removes a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Column ID</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveColumn(string email, int columnOrdinal)
        {
            try
            {
                if (!boards.getBoard(email).getColumns().ContainsKey(columnOrdinal))
                    return new Response<Column>("illegal column number");
                boards.RemoveColumn(email, columnOrdinal);
                logger.Info(email + " remove column # " + columnOrdinal);
            }
            catch (Exception e)
            {
                logger.Error("REMOVE COLUMN- " + e.Message);
                return new Response<Exception>(e, e.Message);
            }

            return new Response();
        }

        /// <summary>
        /// Adds a new column, given it's name and a location to place it.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Location to place to column</param>
        /// <param name="Name">new Column name</param>
        /// <returns>A response object with a value set to the new Column, the response should contain a error message in case of an error</returns>
        public Response<Column> AddColumn(string email, int columnOrdinal, string Name)
        {
            Column newColumn;

            try
            {
                BusinessLayer.Board_Package.Column col = boards.AddColumn(email, columnOrdinal, Name);

                newColumn = new Column(null, Name, col.getLimit());
                logger.Info(email + " add column # " + columnOrdinal + " with the name: " + Name);
            }
            catch (Exception e)
            {
                logger.Error("ADD COLUMN- " + e.Message);
                return new Response<Column>(e.Message);
            }

            return new Response<Column>(newColumn, null);
        }

        /// <summary>
        /// Moves a column to the right, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the column</param>
        /// <returns>A response object with a value set to the moved Column, the response should contain a error message in case of an error</returns>
        public Response<Column> MoveColumnRight(string email, int columnOrdinal)
        {
            Column newColumn;
            IReadOnlyCollection<Task> tasks;
            List<Task> tasksToAdd = new List<Task>();
            try
            {
                BusinessLayer.Board_Package.Column col = boards.MoveColumnRight(email, columnOrdinal);
                foreach (BusinessLayer.Board_Package.Task task in col.getTasks().Values)
                {
                    Task temp = new Task(task.getId(), task.getCreationTime(), task.getDueDate(), task.getTitle(),
                        task.getDescription(), email);
                    tasksToAdd.Add(temp);
                }

                tasks = new ReadOnlyCollection<Task>(tasksToAdd);
                newColumn = new Column(tasks, col.getName(), col.getLimit());
                logger.Info(email + " move column # " + columnOrdinal + " to right");
            }

            catch (Exception e)
            {
                logger.Error("MOVE COLUMN TO RIGHT- " + e.Message);
                return new Response<Column>(e.Message);
            }

            return new Response<Column>(newColumn, null);
        }

        /// <summary>
        /// Moves a column to the left, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the column</param>
        /// <returns>A response object with a value set to the moved Column, the response should contain a error message in case of an error</returns>
        public Response<Column> MoveColumnLeft(string email, int columnOrdinal)
        {
            Column newColumn;
            IReadOnlyCollection<Task> tasks;
            List<Task> tasksToAdd = new List<Task>();
            try
            {
                BusinessLayer.Board_Package.Column col = boards.MoveColumnLeft(email, columnOrdinal);
                foreach (BusinessLayer.Board_Package.Task task in col.getTasks().Values)
                {
                    Task temp = new Task(task.getId(), task.getCreationTime(), task.getDueDate(), task.getTitle(),
                        task.getDescription(), email);
                    tasksToAdd.Add(temp);
                }

                tasks = new ReadOnlyCollection<Task>(tasksToAdd);
                newColumn = new Column(tasks, col.getName(), col.getLimit());
                logger.Info(email + " move column # " + columnOrdinal + " to left");
            }
            catch (Exception e)
            {
                logger.Error("MOVE COLUMN TO LEFT- " + e.Message);
                return new Response<Column>(e.Message);
            }

            return new Response<Column>(newColumn, null);
        }

        /// <summary>        
        /// getter for the boards of all the kanban users
        /// </summary>
        /// <returns>A Dictionary object. The Dictionary should contain a map of <email, Board> of all the users
        public Dictionary<string, BusinessLayer.Board_Package.Board> getBoards()
        {
            return boards.getBoardsByEmail();
        }

        ///<summary>Remove all Board persistent data.</summary>
        public void deleteData()
        {
            try
            {
                boards.deleteData();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AssignTask(string email, int columnOrdinal, int taskId, string emailAssignee)
        {
            try
            {
                boards.AssignTask(email, columnOrdinal, taskId, emailAssignee);
                logger.Info(email + " assigned task # " + taskId + " to "+emailAssignee);
            }
            catch (Exception e)
            {
                logger.Error("ASSIGN TASK- " + e.Message);
                return new Response<Exception>(e, e.Message);
            }

            return new Response();
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        		
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response DeleteTask(string email, int columnOrdinal, int taskId)
        {
            try
            {
                boards.deleteTask(columnOrdinal, taskId, email);
            }
            catch (Exception e)
            {
                return new Response<Exception>(e, e.Message);
            }

            return new Response();
        }

        /// <summary>
        /// Change the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="newName">The new name.</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response ChangeColumnName(string email, int columnOrdinal, string newName)
        {
            try
            {
                boards.ChangeColumnName(email, columnOrdinal, newName);
                logger.Info(email+" changed column # "+columnOrdinal+" name to "+newName);
            }
            catch (Exception e)
            {
                logger.Error("CHANGE COLUMN NAME- "+e.Message);
                return new Response(e.Message);
            }
            return new Response();
        }
    }
}