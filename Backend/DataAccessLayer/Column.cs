using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class Column : DalObject<Column>
    {
        
        public  string Name { get; set; }
        public  int Limit { get; set; }
        public List<Task> Tasks { get; set; }


        private Repo rep=new Repo();
        public Column(string name, int limit, List<Task> tasks)
        {
            Name = name;
            Limit = limit;
            Tasks = tasks;
        }

        public Column()
        {
            
        }
        
        public override void save()
        {
           
        }
        /// <summary>
        /// Saves column to the database 
        /// </summary>
        /// <param name="userEmail">The email address of the user- owner of the board</param>
        /// <param name="id">The id of the column to change</param>
        public void saveColumns(string userEmail,int id)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql = "INSERT OR REPLACE INTO BoardsAndColumns (EMAIL, COLUMN_ID, COLUMN_NAME,LIMIT_TASKS) VALUES (@email, @colID, @name, @limit)";
                SQLiteCommand command=new SQLiteCommand(sql,conn );
                command.Parameters.Add(new SQLiteParameter("@email", userEmail));
                command.Parameters.Add(new SQLiteParameter("@colID", id ));
                command.Parameters.Add(new SQLiteParameter("@name", Name));
                command.Parameters.Add(new SQLiteParameter("@limit", Limit));  
                command.ExecuteNonQuery();
                conn.Close();

                foreach (var t in Tasks)
                {
                    t.SaveTask(userEmail,id);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Set limit to column in the database
        /// </summary>
        /// <param name="userEmail">The email address of the user- owner of the board</param>
        /// <param name="id">The id of the column to change</param>
        /// <param name="limit">The new limit if the column</param>
        public void setLimit(String userEmail, int id, int limit)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql = "UPDATE BoardsAndColumns SET LIMIT_TASKS=@newLim WHERE EMAIL=@email AND COLUMN_ID=@colId ";
                SQLiteCommand command=new SQLiteCommand(sql,conn );
                command.Parameters.Add(new SQLiteParameter("@newLim", limit));
                command.Parameters.Add(new SQLiteParameter("@email", userEmail));
                command.Parameters.Add(new SQLiteParameter("@colId", id ));
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Set name of a column in the database
        /// </summary>
        /// <param name="userEmail">The email address of the user- owner of the board</param>
        /// <param name="id">The id of the column to change</param>
        /// <param name="name">The new name of the column</param>
        public void setName(String userEmail, int id, string name)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql = "UPDATE BoardsAndColumns SET COLUMN_NAME=@name WHERE EMAIL=@email AND COLUMN_ID=@colId ";
                SQLiteCommand command=new SQLiteCommand(sql,conn );
                command.Parameters.Add(new SQLiteParameter("@name", name));
                command.Parameters.Add(new SQLiteParameter("@email", userEmail));
                command.Parameters.Add(new SQLiteParameter("@colId", id ));
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

       
    }
}