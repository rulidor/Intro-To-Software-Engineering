using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class DalController
    {
        private Repo rep=new Repo();
        private static readonly log4net.ILog logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DalController()
        {
        }
        
        
      
        /// <summary>
        /// Load users from the database
        /// </summary>
        /// <returns>A Dictionary: key- string: user email, value- a bussiness layer User object<returns>
        public Dictionary<string,BusinessLayer.User_Package.User> readUsers()
        {
            try
            {
                Dictionary<string,BusinessLayer.User_Package.User> users=new Dictionary<string, BusinessLayer.User_Package.User>();
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "SELECT * FROM Users";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string email = reader.GetString(reader.GetOrdinal("EMAIL"));
                    string nickname = reader.GetString((reader.GetOrdinal("NICKNAME")));
                    string password = reader.GetString(reader.GetOrdinal("PASSWORD"));
                    bool isLogged = reader.GetBoolean(reader.GetOrdinal("ISLOGGEDIN"));
                    users.Add(email,new BusinessLayer.User_Package.User(email,nickname,password, isLogged));
                }
                conn.Close();
                logger.Info("read users from database");
                return users;
            }
            catch (Exception e)
            {
                logger.Error("READ USERS- "+e.Message);
                throw e;
            }
        }
        /// <summary>
        /// Load boards and columns from the database
        /// </summary>
        /// <returns>A Dictionary: key- string: user email, value- a bussiness layer Board object<returns>
        public Dictionary<string, BusinessLayer.Board_Package.Board> readBoard()
        {
            int i = 0;
            try
            {
                Dictionary<int,BusinessLayer.Board_Package.Column> columns=new Dictionary<int, BusinessLayer.Board_Package.Column>();
                Dictionary<string, BusinessLayer.Board_Package.Board> boards=new Dictionary<string, BusinessLayer.Board_Package.Board>();
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "SELECT * FROM BoardsAndColumns ORDER BY EMAIL,COLUMN_ID";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                string email1="";
                if (reader.Read())
                {
                    email1 = reader.GetString(reader.GetOrdinal("EMAIL"));
                    int colId1 = reader.GetInt32((reader.GetOrdinal("COLUMN_ID")));
                    string name1 = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
                    int lim1 = reader.GetInt32(reader.GetOrdinal("LIMIT_TASKS"));
                    columns.Add(colId1,new BusinessLayer.Board_Package.Column(name1,lim1));
                    columns[colId1].setTasks(readTask(email1,colId1,conn));
                    boards.Add(email1,new BusinessLayer.Board_Package.Board(email1,columns));
                    i += columns[colId1].getTasks().Count;
                    boards[email1].setIndex(i);
                }
               
                while (reader.Read())
                {
                    string email = reader.GetString(reader.GetOrdinal("EMAIL"));
                    int colId = reader.GetInt32((reader.GetOrdinal("COLUMN_ID")));
                    string name = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
                    int lim = reader.GetInt32(reader.GetOrdinal("LIMIT_TASKS"));
                    if (email.Equals(email1))
                    {
                        columns.Add(colId,new BusinessLayer.Board_Package.Column(name,lim));
                        columns[colId].setTasks(readTask(email,colId,conn));
                        i += columns[colId].getTasks().Count;

                    }
                    else
                    {
                        boards[email1]=new BusinessLayer.Board_Package.Board(email1,columns);
                        boards[email1].setIndex(i+1);
                        columns=new Dictionary<int, BusinessLayer.Board_Package.Column>();
                        columns.Add(colId,new BusinessLayer.Board_Package.Column(name,lim));
                        columns[colId].setTasks(readTask(email,colId,conn));
                        i = 0;
                        boards.Add(email,new BusinessLayer.Board_Package.Board(email,columns));
                        i += columns[colId].getTasks().Count;
                        email1 = email;
                    }
                }

                if (email1 != null && !email1.Equals(""))
                {
                    boards[email1] = new BusinessLayer.Board_Package.Board(email1, columns);
                    boards[email1].setIndex(i+1);
                }

                conn.Close();
                logger.Info("read boards from data base");
                return boards;
            }
            catch (Exception e)
            {
                logger.Error("READ BOARD-"+e.Message);
                throw e;
            }
        }
        /// <summary>
        /// Load tasks from the database
        /// </summary>
        /// <param name="userEmail">The email address of the user-owner of the board the task belongs to</param>
        /// <param name="colId">The column id that the task belongs to</param>
        /// <param name="conn">A sql connection to the database</param>
        /// <returns>A Dictionary: key- int: task id, value- a bussiness layer Task object<returns>
        public Dictionary<int, BusinessLayer.Board_Package.Task> readTask(string userEmail,int colId,SQLiteConnection conn)
        {
            try
            {
                Dictionary<int, BusinessLayer.Board_Package.Task> tasks =
                    new Dictionary<int, BusinessLayer.Board_Package.Task>();
                //conn.Open();
                string sql =
                    "SELECT * FROM Tasks WHERE EMAIL=@email AND COLUMN_ID=@columnId";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.Add(new SQLiteParameter("@email", userEmail));
                command.Parameters.Add(new SQLiteParameter("@columnId", colId));
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    int tId = reader.GetInt32(reader.GetOrdinal("TASK_ID"));
                    DateTime cDate = reader.GetDateTime(reader.GetOrdinal("CREATION_TIME"));
                    DateTime due = reader.GetDateTime(reader.GetOrdinal("DUE_DATE"));
                    string title = reader.GetString(reader.GetOrdinal("TITLE"));
                    string desc = reader.GetString(reader.GetOrdinal("DESCRIPTION"));
                    if (desc.Equals("null"))
                        desc = null;
                    string emailAssi = reader.GetString((reader.GetOrdinal("EMAIL_ASSIGNEE")));
                    tasks.Add(tId,new BusinessLayer.Board_Package.Task(tId,cDate,due,title,desc,emailAssi));

                    //conn.Close();
                }
                logger.Info("read tasks  from data base");
                return tasks;
            }
            catch (Exception e)
            {
                logger.Error("READ TASKS- "+e.Message);
                throw e;
            }
        }
        /// <summary>
        /// Load host and subscribers from the database
        /// </summary>
     /// <returns>A Dictionary: key- int: host email, value- a list of strings- the subscribers email of the key<returns>
        public Dictionary<string,List<string>> readHostSubs()
        {
            try
            {
                Dictionary<string,List<string>> users=new Dictionary<string, List<string>>();
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "SELECT * FROM HostAndSubscribers";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string Hostemail = reader.GetString(reader.GetOrdinal("HOST_EMAIL"));
                    string SubEmail = reader.GetString((reader.GetOrdinal("SUBSCRIBER_EMAIL")));

                    if (users.ContainsKey(Hostemail))
                    {
                        users[Hostemail].Add(SubEmail);
                    }
                    else
                    {
                        users.Add(Hostemail,new List<string>());
                        users[Hostemail].Add(SubEmail);
                    }
                }
                conn.Close();
                logger.Info("read host and users from data base");
                return users;
            }
            catch (Exception e)
            {
                logger.Error("READ HOST AND SUBSCRIBERS - "+e.Message);
                throw e;
            }
        }
    }
}