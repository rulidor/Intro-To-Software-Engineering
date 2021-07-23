using System;
using System.Collections.Generic;
using System.Linq;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board_Package
{
    public class BoardController
    {
        private readonly int minTitle = 0;
        private readonly int maxTitle = 50;
        private readonly int descMax = 300;
        //save dalController
        private DalController dalController;
        private Dictionary<string, Board> boardsByEmail { get; set; }
        private Dictionary<string, List<string>> sharedUsers { get; set; } 
        
        /// <summary>
        /// constructor for a BoardController
        ///  </summary>
        public BoardController()
        {
            boardsByEmail=new Dictionary<string, Board>();
            dalController=new DalController();
            sharedUsers = new Dictionary<string, List<string>>();
        }
        
        /// <summary>
        /// getter for Boards, mapped by user email
        /// </summary>
        /// <returns>A Dictionart<string, Board> object, containing Boards mapped by users emails</returns>
        public Dictionary<string, Board> getBoardsByEmail()
        {
            return boardsByEmail;
        }

        /// <summary>
        /// adds a new Board
        /// </summary>
        /// <param name="email">user email which the added Board is belonged to</param>
        public void addNewBoard(string email)
        {
            boardsByEmail.Add(email, new Board(email));
            boardsByEmail[email].toDalObject().save();
        }

        public bool checkUser(string email)
        {
            if (boardsByEmail.ContainsKey(email))
                return true;
            return false;
        }
        /// <summary>
        /// adds a user to a Board
        /// </summary>
        /// <param name="creator">Host email which the Board is belonged to</param>
        /// <param name="newUser"> a new use email to add to an existing board</param>
        public void addUserToBoard(string creator, string newUser)
        {
            if(!boardsByEmail.ContainsKey(creator))
                throw new Exception("host email doesn't exist");
            if(boardsByEmail.ContainsKey(newUser))
                throw new Exception("the user already own a board");
            bool flag = false;
            foreach (var usersValue in sharedUsers.Values)
            {
                foreach (var user in usersValue)
                {
                    if (user.Equals(newUser))
                        flag = true;

                }   
            }
            if(flag)
                throw new Exception("user is already associated with a board");
            if (!sharedUsers.ContainsKey(creator))
            {
                sharedUsers.Add(creator, new List<string>());
                sharedUsers[creator].Add(newUser);
            }
            else
            {
                sharedUsers[creator].Add(newUser);
                
            }
            boardsByEmail[creator].toDalObject().saveHostUsers(creator,newUser);
        }

        /// <summary>
        /// loads data to this controller from database
        /// </summary>
        public void loadData()
        {
            try
            {
                boardsByEmail=dalController.readBoard();
                sharedUsers = dalController.readHostSubs();

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        /// <summary>
        /// adds a task to a board
        /// </summary>
        /// <param name="email">user email the board is belonged to</param>
        /// <param name="title">task's title</param>
        /// <param name="description">task's description</param>
        /// <param name="DueDate">task's DueDate</param>
        /// <returns>The added Task object</returns>
        public Task addTask(string email, string title, string description, DateTime DueDate)
        {
            try
            {
                bool flag = false;
                string host = "";
                if (!boardsByEmail.ContainsKey(email))
                {
                    foreach (var usersList in sharedUsers)
                    {
                        foreach (var user in usersList.Value)
                        {
                            if (email.Equals(user))
                            {
                                host = usersList.Key;
                                flag = true;
                            }
                        }
                    }
                    if(!flag)
                        throw new Exception("email not found");
                }
                else
                {
                    host = email;
                }
                if (title==null || title.Length == minTitle || title.Length > maxTitle)
                {
                    throw new Exception("title max. 50 characters, not empty");
                }

                if (description != null && description.Length > descMax ) 
                {
                    throw new Exception("description max. 300 characters");
                }
                if (DueDate==null || DueDate <= DateTime.Now)
                {
                    throw new Exception("due date not valid");
                }
                Task t = boardsByEmail[host].addTask(new Task(0, DateTime.Now, DueDate, title, description, email));
                t.toDalObject().SaveTask(host,0);
                return t;
            }
            catch (Exception e)
            { 
                throw e;
            }
            
        }

        /// <summary>
        /// advances a task
        /// </summary>
        /// <param name="email">user email the board is belonged to</param>
        /// <param name="column">column the task is currently at</param>
        /// <param name="taskId">task's id</param>
        public void advanceTask(string email, int column, int taskId)
        {
            try
            {
                bool flag = false;
                string host = "";
                if (!boardsByEmail.ContainsKey(email))
                {
                    foreach (var usersList in sharedUsers)
                    {
                        foreach (var user in usersList.Value)
                        {
                            if (email.Equals(user))
                            {
                                host = usersList.Key;
                                flag = true;
                            }
                        }
                    }

                    if (!flag)
                        throw new Exception("email not found");
                }
                else
                {
                    host = email;
                }
               
            
                Task t=boardsByEmail[host].advanceTask(email,column, taskId);
                t.toDalObject().removeTask(host,column,taskId);
                t.toDalObject().SaveTask(host,column+1);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        
        /// <summary>
        /// limits number of tasks at a board
        /// </summary>
        /// <param name="email">user email the board is belonged to</param>
        /// <param name="column">column's ordinal number</param>
        /// <param name="limit">new limit</param>
        public void LimitColumnTasks(string email, int column, int limit)
        {
            try
            {
                if (!boardsByEmail.ContainsKey(email))
                    throw new Exception("only the creator of the board can change the limit ");
                boardsByEmail[email].setLimit(column, limit, email);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// updates a task DueDate
        /// </summary>
        /// <param name="email">user email assigned to the task</param>
        /// <param name="column">column's ordinal number</param>
        /// <param name="taskId">task's id</param>
        /// <param name="DueDate">new DueDate</param>
        public void UpdateTaskDueDate(string email, int column, int taskId, DateTime DueDate)
        {
            bool flag = false;
            string hostEmail = "";
            if (!boardsByEmail.ContainsKey(email))
            {
                foreach (var usersList in sharedUsers)
                {
                    foreach (var user in usersList.Value)
                    {
                        if (email.Equals(user))
                        {
                            hostEmail = usersList.Key;
                            flag = true;

                        }
                    }
                }

                if (!flag)
                    throw new Exception("email not found");
            }
            else
            {
                hostEmail = email;
            }
            
            boardsByEmail[hostEmail].UpdateTaskDueDate(email,column,taskId,DueDate);
            boardsByEmail[hostEmail].getColumn(column).getTasks()[taskId].toDalObject().updateDueTask(hostEmail,column,taskId, DueDate);
        }

        /// <summary>
        /// updates a task title
        /// </summary>
        /// <param name="email">user email the board is belonged to</param>
        /// <param name="column">column's ordinal number</param>
        /// <param name="taskId">task's id</param>
        /// <param name="title">new title</param>
        public void UpdateTaskTitle(string email, int column, int taskId, string title)
        {
            bool flag = false;
            string host = "";
            if (!boardsByEmail.ContainsKey(email))
            {
                foreach (var usersList in sharedUsers)
                {
                    foreach (var user in usersList.Value)
                    {
                        if (email.Equals(user))
                        {
                            host = usersList.Key;
                            flag = true;
                        }
                    }
                }

                if (!flag)
                    throw new Exception("email not found");
            }
            else
            {
                host = email;
            }
            if (!boardsByEmail[host].getColumns().ContainsKey(column))
                throw new Exception("Column not found");
            if (!boardsByEmail[host].getColumn(column).getTasks().ContainsKey(taskId))
            {
                throw new Exception("task not found");
            }
            if (title==null || title.Length == minTitle || title.Length > maxTitle)
            {
                throw new Exception("title max. 50 characters, not empty");
            }
            if(column==boardsByEmail[host].getColumns().Keys.Last())
                throw new Exception("cant change task because its in the done column");
            if(!boardsByEmail[host].getColumn(column).getTasks()[taskId].getEmailAssignee().Equals(email))
                throw new Exception("only email assignee can change tasks");
            boardsByEmail[host].getColumn(column).getTasks()[taskId].setTitle(title);
            boardsByEmail[host].getColumn(column).getTasks()[taskId].toDalObject().updateTitle(host,column,taskId,title);
        }

        /// <summary>
        /// updates a task description
        /// </summary>
        /// <param name="email">user email the board is belonged to</param>
        /// <param name="column">column's ordinal number</param>
        /// <param name="taskId">task's id</param>
        /// <param name="description">new description</param>
        public void UpdateTaskDescription(string email, int column, int taskId, string description)
        {
            bool flag = false;
            string host = "";
            if (!boardsByEmail.ContainsKey(email))
            {
                foreach (var usersList in sharedUsers)
                {
                    foreach (var user in usersList.Value)
                    {
                        if (email.Equals(user))
                        {
                            host = usersList.Key;
                            flag = true;
                        }
                    }
                }

                if (!flag)
                    throw new Exception("email not found");
            }
            else
            {
                host = email;
            }
            if (!boardsByEmail[host].getColumns().ContainsKey(column))
                throw new Exception("Column not found");
            if (!boardsByEmail[host].getColumn(column).getTasks().ContainsKey(taskId))
            {
                throw new Exception("task not found");
            }
            if (description!=null && description.Length > descMax)
            {
                throw new Exception("description max. 300 characters");
            }
            if(column == boardsByEmail[host].getColumns().Keys.Last())
                throw new Exception("cant change task because its in the done column");
            if(!boardsByEmail[host].getColumn(column).getTasks()[taskId].getEmailAssignee().Equals(email))
                throw new Exception("only email assignee can change tasks");
            boardsByEmail[host].getColumn(column).getTasks()[taskId].setDescription(description);
            boardsByEmail[host].getColumn(column).getTasks()[taskId].toDalObject().updateDesc(host,column,taskId,description);
        }
        
        /// <summary>
        /// removes a column
        /// </summary>
        /// <param name="email">user email the board is belonged to</param>
        /// <param name="column">column's ordinal number</param>
        public void RemoveColumn(string email, int columnOrdinal)
        {
            if (!boardsByEmail.ContainsKey(email))
                throw new Exception("only creator can remove column");
            if(!boardsByEmail[email].getColumns().ContainsKey(columnOrdinal))
                throw new Exception("cannot get column");
            boardsByEmail[email].removeColumn(columnOrdinal);
        }
        
        /// <summary>
        /// adds a column to a Board
        /// </summary>
        /// <param name="email">user email the board is belonged to</param>
        /// <param name="columnOrdinal">column's ordinal number</param>
        /// <param name="Name">column's name</param>
        /// <returns>The added Column object</returns>
        public Column AddColumn(string email, int columnOrdinal, string Name)
        {
            if (!boardsByEmail.ContainsKey(email))
                throw new Exception("only creator can add column");
            Column c= boardsByEmail[email].addColumn(columnOrdinal, Name);
            //c.toDalObject().saveColumns(email,columnOrdinal);
            return c;
        }
        
        /// <summary>
        /// moves a column to the right and swaps its righthand column
        /// </summary>
        /// <param name="email">user email the board is belonged to</param>
        /// <param name="columnOrdinal">column ordinal num. to be shifted to the right</param>
        /// <returns>A Column object, representing the shifted to the right's column</returns>
        public Column MoveColumnRight(string email, int columnOrdinal)
        {
            try
            {
                if(!boardsByEmail.ContainsKey(email))
                    throw new Exception("only the board creator can order the columns");
                return boardsByEmail[email].MoveColumnRight(columnOrdinal, email);
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
        
        /// <summary>
        /// moves a column to the left and swaps its lefthand column
        /// </summary>
        /// <param name="email">user email the board is belonged to</param>
        /// <param name="columnOrdinal">column ordinal num. to be shifted to the left</param>
        /// <returns>A Column object, representing the shifted to the left's column</returns>
        public Column MoveColumnLeft(string email, int columnOrdinal)
        {
            try
            {
                if(!boardsByEmail.ContainsKey(email))
                    throw new Exception("only the board creator can order the columns");
                return boardsByEmail[email].MoveColumnLeft(columnOrdinal, email);
            }
            catch (Exception e)
            {
                throw e;
            }
           
        }
        
        /// <summary>
        /// getter for a column by its name
        /// </summary>
        /// <param name="email">user email the board is belonged to</param>
        /// <param name="column">column's name</param>
        /// <returns>A Column object, representing the desired Column</returns>
        public Column getColumn(string email, string column)
        {
            bool flag = false;
            string host = "";
            if (!boardsByEmail.ContainsKey(email))
            {
                foreach (var usersList in sharedUsers)
                {
                    foreach (var user in usersList.Value)
                    {
                        if (email.Equals(user))
                        {
                            host = usersList.Key;
                            flag = true;
                        }
                    }
                }

                if (!flag)
                    throw new Exception("email not found");
            }
            else
            {
                host = email;
            }
            return boardsByEmail[host].getColumn(column);
        }
        
        /// <summary>
        /// getter for a column by its ordinal number
        /// </summary>
        /// <param name="email">user email the board is belonged to</param>
        /// <param name="column">column's ordinal number</param>
        /// <returns>A Column object, representing the desired Column</returns>
        public Column getColumn(string email, int column)
        {
            bool flag = false;
            string host = "";
            if (!boardsByEmail.ContainsKey(email))
            {
                foreach (var usersList in sharedUsers)
                {
                    foreach (var user in usersList.Value)
                    {
                        if (email.Equals(user))
                        {
                            host = usersList.Key;
                            flag = true;
                        }
                    }
                }

                if (!flag)
                    throw new Exception("email not found");
            }
            else
            {
                host = email;
            }
            return boardsByEmail[host].getColumn(column);
        }
        
        
        /// <summary>
        /// getter for a board by user email
        /// </summary>
        /// <param name="email">user email the board is belonged to</param>
        /// <returns>A Board object, representing the desired Board</returns> 
        public Board getBoard(string email)
        {
            bool flag = false;
            string host = "";
            if (!boardsByEmail.ContainsKey(email))
            {
                foreach (var usersList in sharedUsers)
                {
                    foreach (var user in usersList.Value)
                    {
                        if (email.Equals(user))
                        {
                            host = usersList.Key;
                            flag = true;
                        }
                    }
                }

                if (!flag)
                    throw new Exception("email not found");
            }
            else
            {
                host = email;
            }
            return boardsByEmail[host];
        }

        /// <summary>
        /// deletes data from database
        /// </summary>
        public void deleteData()
        {
            boardsByEmail=new Dictionary<string, Board>();
            sharedUsers=new Dictionary<string, List<string>>();
        }
        /// <summary>
        /// change column name
        /// </summary>
        /// <param name="email">email of the owner of the board/param>
        /// <param name="columnOrdinal">column ordinal of the column we wish to change name</param>
        /// <param name="newName">the new column name</param>
        public void ChangeColumnName(string email, int columnOrdinal, string newName)
        {
            if(!boardsByEmail.ContainsKey(email))
                throw new Exception("only the creator can change column name");
            boardsByEmail[email].setName(email,columnOrdinal, newName);
        }
        /// <summary>
        /// assign a task to a user
        /// </summary>
        /// <param name="columnOrdinal">column ordinal of the task</param>
        /// <param name="taskId">task Id to assign</param>
        /// <param name="emailAssignee"> the email assignee of the task</param>
        public void AssignTask(string email, int columnOrdinal, int taskId, string emailAssignee)
        {
            bool flag = false, flag2 = false;
            string userBoard = "", userBoard2 = "";
            if (boardsByEmail.ContainsKey(email))
            {
                foreach (var user in sharedUsers[email])
                {
                    if (emailAssignee.Equals(user))
                        flag = true;
                }
                if (!flag)
                    throw new Exception("emailAssignee not found");
                boardsByEmail[email].AssignTask(columnOrdinal, taskId, emailAssignee);
            }
            else if (boardsByEmail.ContainsKey(emailAssignee))
            {
                foreach (var user in sharedUsers[emailAssignee])
                {
                    if (email.Equals(user))
                        flag2 = true;
                }
                if (!flag2)
                    throw new Exception("email not found");
                boardsByEmail[emailAssignee].AssignTask(columnOrdinal, taskId, emailAssignee);
            }
            else
            {
                foreach (var usersList in sharedUsers)
                {
                    foreach (var user in usersList.Value)
                    {
                        if (email.Equals(user))
                            userBoard = usersList.Key;
                        if (emailAssignee.Equals(user))
                            userBoard2 = usersList.Key;
                    }
                }
                if (!userBoard.Equals(userBoard2))
                    throw new Exception("email and assignee email don't belong to the same board");
                boardsByEmail[userBoard].AssignTask(columnOrdinal, taskId, emailAssignee);
            }
        }
        /// <summary>
        /// deletes a task from a column in this board
        /// </summary>
        /// <param name="columnOrdinal">column ordinal of the task</param>
        /// <param name="taskId">task Id to delete</param>
        /// <param name="emailAssignee">email assignee of the task to delete</param>
        public void deleteTask(int columnOrdinal, int taskId, string emailAssignee)
        {
            string userBoard = "";
            if(boardsByEmail.ContainsKey(emailAssignee))
                boardsByEmail[emailAssignee].deleteTask(columnOrdinal, taskId, emailAssignee);
            else
            {
                foreach (var usersList in sharedUsers)
                {
                    foreach (var user in usersList.Value)
                    {
                        if (emailAssignee.Equals(user))
                            userBoard = usersList.Key;
                    }
                }
                if(userBoard.Equals(""))
                    throw new Exception("no board is shared with emailAssignee");    
            }
            boardsByEmail[userBoard].deleteTask(columnOrdinal, taskId, emailAssignee);
            
        }
    }
}