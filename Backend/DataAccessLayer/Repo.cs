using System;
using System.Configuration;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class Repo
    {
        private static readonly log4net.ILog logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// open a connection to the data base
        /// </summary>
        public SQLiteConnection openConnection()
        {

            return new SQLiteConnection("Data Source=kanban.db;Version=3;");
        }
        
        /// <summary>
        /// create the tables in the database
        /// </summary>
       public void createDatabase()
        {
            using (SQLiteConnection conn = openConnection())
            {
                try
                {
                    createTableUser(conn);
                    createTableBoard(conn);
                    createTableBoardTask(conn);
                    HostAndSubscribersBoard(conn);
                    logger.Info("created dataBase");
                }
                catch (Exception e)
                {
                    logger.Error("CREATE DATABASE- "+e.Message);
                    throw e;
                }
            }
        }
        /// <summary>
        /// Create the user table in the database
        /// </summary>
        /// <param name="conn">An sql connection to the data base</param>
        public void createTableUser(SQLiteConnection conn)
        {
            try
            {
                conn.Open();
                
                string sql = "CREATE TABLE IF NOT EXISTS Users" +
                                      "(EMAIL VARCHAR(255) PRIMARY KEY NOT NULL," +
                                      "NICKNAME VARCHAR(255) NOT NULL," +
                                      "PASSWORD VARCHAR(20) NOT NULL," +
                                      "ISLOGGEDIN BOOLEAN NOT NULL)";
                SQLiteCommand command=new SQLiteCommand(sql,conn );

                command.ExecuteNonQuery();
                conn.Close();


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        /// <summary>
        /// Create the boards and columns table in the database
        /// </summary>
        /// <param name="conn">An sql connection to the data base</param>
        public void createTableBoard(SQLiteConnection conn)
        {
            try
            {
               
                conn.Open();
                
                string sql = "CREATE TABLE IF NOT EXISTS BoardsAndColumns" +
                             "(EMAIL VARCHAR(255)  NOT NULL," +
                             "COLUMN_ID INT NOT NULL," +
                             "COLUMN_NAME VARCHAR(15) NOT NULL," +
                             "LIMIT_TASKS INT NOT NULL,"+
                             "PRIMARY KEY (EMAIL,COLUMN_NAME),"+
                             "FOREIGN KEY (EMAIL) REFERENCES Users(EMAIL)) ";
                SQLiteCommand command=new SQLiteCommand(sql,conn );

                command.ExecuteNonQuery();
                conn.Close();


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        /// <summary>
        /// Create the task table in the database
        /// </summary>
        /// <param name="conn">An sql connection to the data base</param>
        public void createTableBoardTask(SQLiteConnection conn)
        {
            try
            {
                
                conn.Open();

                string sql = "CREATE TABLE IF NOT EXISTS Tasks" +
                             "(EMAIL VARCHAR(255)  NOT NULL," +
                             "COLUMN_ID INT NOT NULL," +
                             "TASK_ID INT NOT NULL," +
                             "CREATION_TIME DATETIME NOT NULL," +
                             "DUE_DATE DATETIME NOT NULL," +
                             "TITLE VARCHAR(50) NOT NULL," +
                             "DESCRIPTION VARCHAR(300)," +
                             "EMAIL_ASSIGNEE VARCHAR(255) NOT NULL, " +
                             "PRIMARY KEY(EMAIL,TASK_ID)," +
                             "FOREIGN KEY (EMAIL) REFERENCES Users(EMAIL)," +
                             "FOREIGN KEY (COLUMN_ID) REFERENCES BoardsAndColumns(COLUMN_ID) ON UPDATE CASCADE," +
                             "FOREIGN KEY (EMAIL_ASSIGNEE) REFERENCES Users(EMAIL))";
                    
        

                SQLiteCommand command=new SQLiteCommand(sql,conn );

                command.ExecuteNonQuery();
                conn.Close();


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Create the host and subscribers table in the database
        /// </summary>
        /// <param name="conn">An sql connection to the data base</param>
        public void HostAndSubscribersBoard(SQLiteConnection conn)
        {
            try
            {
                
                conn.Open();

                string sql = "CREATE TABLE IF NOT EXISTS HostAndSubscribers" +
                             "(HOST_EMAIL VARCHAR(255)  NOT NULL," +
                             "SUBSCRIBER_EMAIL VARCHAR(255) NOT NULL, " +
                             "PRIMARY KEY(HOST_EMAIL,SUBSCRIBER_EMAIL)," +
                             "FOREIGN KEY (HOST_EMAIL) REFERENCES Users(EMAIL) ," +
                             "FOREIGN KEY (SUBSCRIBER_EMAIL) REFERENCES Users(EMAIL))";
                    
        

                SQLiteCommand command=new SQLiteCommand(sql,conn );

                command.ExecuteNonQuery();
                conn.Close();


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Delete the database
        /// </summary>
        public void deleteDataBase()
        {
            try
            {
                dropTasks();
                dropBoards();
                dropHostUsers();
                dropUsers();
                logger.Info("deleted dataBase");
            }
            catch (Exception e)
            {
                logger.Error("DELETE DATABASE-"+e.Message);
                throw e;
            }
        }
        /// <summary>
        /// Drop the users table from the data database
        /// </summary>
        public void dropUsers()
        {
            try
            {
                SQLiteConnection conn = openConnection();
                conn.Open();
                string sql = "DROP TABLE IF EXISTS Users";
                SQLiteCommand command=new SQLiteCommand(sql,conn );

                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        /// <summary>
        /// Drop the boards and column table from the data database
        /// </summary>
        public void dropBoards()
        {
            try
            {
                SQLiteConnection conn = openConnection();
                conn.Open();
                
                string sql = "DROP TABLE IF EXISTS BoardsAndColumns";
                SQLiteCommand command=new SQLiteCommand(sql,conn );

                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        /// <summary>
        /// Drop the tasks table from the data database
        /// </summary>
        public void dropTasks()
        {
            try
            {
                SQLiteConnection conn = openConnection();
                conn.Open();
                /*command.Connection = conn;
                command.CommandTimeout = 0;*/
                string sql = "DROP TABLE IF EXISTS Tasks";
                SQLiteCommand command=new SQLiteCommand(sql,conn );

                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        
        /// <summary>
        /// Drop the host and subscribers table from the data database
        /// </summary>
        public void dropHostUsers()
        {
            try
            {
                SQLiteConnection conn = openConnection();
                conn.Open();
                
                string sql = "DROP TABLE IF EXISTS HostAndSubscribers";
                SQLiteCommand command=new SQLiteCommand(sql,conn );

                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        
    }
}