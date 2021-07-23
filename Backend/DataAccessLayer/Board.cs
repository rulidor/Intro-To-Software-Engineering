using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class Board : DalObject<Board>
    {
        public string userEmail { get; set; }
        public Dictionary<int, Column> columns { get; set; }
        
        private Repo rep;

        public Board(string userEmail, Dictionary<int,Column>columns)
        {
            this.userEmail = userEmail;
            this.columns = columns;
            rep =new Repo();
        }

        
       
        /// <summary>
        /// Saves a board to th data base
        /// </summary>
        public override void save()
        {
            try
            {
                
                SQLiteConnection conn = rep.openConnection();
                
                foreach (var entry in columns)
                {
                    entry.Value.saveColumns(userEmail,entry.Key);

                }
                
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
        /// <summary>
        /// Delete column from the data base
        /// </summary>
        /// <param name="email">The email address of the user </param>
        /// <param name="col">The column id to delete from the database</param>
        public void deleteCol(string email, int col)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "DELETE FROM BoardsAndColumns WHERE EMAIL=@email AND COLUMN_ID=@colid";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.Add(new SQLiteParameter("@email", email));
                command.Parameters.Add(new SQLiteParameter("@colid", col));

          
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Update column id in the database
        /// </summary>
        /// <param name="email">The email address of the user- owner of the board</param>
        /// <param name="name">The name of the column to change</param>
        /// <param name="newColId">The new id of the column to set</param>
        public void updateColeId(string email, string name, int newColId)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "UPDATE BoardsAndColumns SET COLUMN_ID=@newcolid  WHERE EMAIL=@email AND COLUMN_NAME=@name";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.Add(new SQLiteParameter("@newcolid", newColId));
                command.Parameters.Add(new SQLiteParameter("@email", email));
                command.Parameters.Add(new SQLiteParameter("@name", name));

          
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Update column id to a new id in the database 
        /// </summary>
        /// <param name="email">The email address of the user- owner of the board</param>
        /// <param name="name">The name of the column to change</param>
        /// <param name="newColId">The new id of the column</param>
        /// <param name="coldColId">The old id of the column</param>
        public void moveCOlRight(string email, string name, int newColId,int oldColId)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "UPDATE BoardsAndColumns SET COLUMN_ID=@newcolid  WHERE EMAIL=@email AND COLUMN_NAME=@name AND COLUMN_ID=@oldId";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.Add(new SQLiteParameter("@newcolid", newColId));
                command.Parameters.Add(new SQLiteParameter("@email", email));
                command.Parameters.Add(new SQLiteParameter("@name", name));
                command.Parameters.Add(new SQLiteParameter("@oldId", oldColId));


          
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// boards host and the subscribers of the board 
        /// </summary>
        /// <param name="host">email host and the owner of the board</param>
        /// <param name="subscriber">The subscriber's email to this board owner</param>
        
        public void saveHostUsers(string host,string subscriber)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql = "INSERT OR REPLACE INTO HostAndSubscribers (HOST_EMAIL, SUBSCRIBER_EMAIL) VALUES (@host, @sub)";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.Add(new SQLiteParameter("@host", host));
                command.Parameters.Add(new SQLiteParameter("@sub", subscriber));
                


          
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