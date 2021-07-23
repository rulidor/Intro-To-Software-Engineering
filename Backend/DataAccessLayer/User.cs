using System;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class User :  DalObject<User>
    {
        public string email { get; set; }
        public string nickname { get; set; }
        public string password { get; set; }
        public Boolean isLoggedIn { get; set; }
        private Repo rep=new Repo();
        public User(string email, string nickname, string password,Boolean isLoged)
        {
            this.email = email;
            this.nickname = nickname;
            this.password = password;
            this.isLoggedIn = isLoged;

        }

        public User()
        {
            
        }
        /// <summary>
        /// Save a user to the database
        /// </summary>
      public override void save()
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql = "INSERT OR REPLACE INTO Users (EMAIL, NICKNAME, PASSWORD,ISLOGGEDIN) VALUES (@email, @nickName, @password, @isLogged)";
                SQLiteCommand command=new SQLiteCommand(sql,conn );
                command.Parameters.Add(new SQLiteParameter("@email", email));
                command.Parameters.Add(new SQLiteParameter("@nickName", nickname));
                command.Parameters.Add(new SQLiteParameter("@password", password));
                command.Parameters.Add(new SQLiteParameter("@isLogged", isLoggedIn));
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