using System;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class Task :  DalObject<Task>
    {
        public int Id  { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime DueDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string emailAssignee { get; set; }
        private Repo rep=new Repo();

        public Task(int id, DateTime creationTime, DateTime DueDate, string title, string description, string emailAssignee)
        {
            Id = id;
            CreationTime = creationTime;
            this.DueDate = DueDate;
            Title = title;
            Description = description;
            this.emailAssignee = emailAssignee;
        }

        public Task()
        {
            
        }
        
        public override void save()
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// Save task to the database
        /// </summary>
        /// <param name="email">The email address of the user- the owner of the board</param>
        /// <param name="colID">The column id that the task belongs to</param>
        
        public void SaveTask(string email, int colID)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "INSERT OR REPLACE INTO Tasks (EMAIL, COLUMN_ID, TASK_ID,CREATION_TIME,DUE_DATE,TITLE,DESCRIPTION,EMAIL_ASSIGNEE) VALUES (@email, @colID, @taskid, @creTime,@dueDate,@title,@desc,@emailAssi)";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.Add(new SQLiteParameter("@email", email));
                command.Parameters.Add(new SQLiteParameter("@colID", colID));
                command.Parameters.Add(new SQLiteParameter("@taskid", Id));
                command.Parameters.Add(new SQLiteParameter("@creTime", CreationTime));
                command.Parameters.Add(new SQLiteParameter("@dueDate", DueDate));
                command.Parameters.Add(new SQLiteParameter("@title", Title));
                if (Description==null)
                    command.Parameters.Add(new SQLiteParameter("@desc", "null"));
                else
                {
                    command.Parameters.Add(new SQLiteParameter("@desc", Description));

                }
                command.Parameters.Add(new SQLiteParameter("@emailAssi", emailAssignee));

                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Update the due date of a task in the database
        /// </summary>
        /// <param name="email">The email address of the user-owner of the board</param>
        /// <param name="colId">The column id of the column that the task belongs to</param>
        /// <param name="taskId">The id of the task to change</param>
        /// <param name="date">The new due date of the task</param>
        public void updateDueTask(string email, int colId,int taskId,DateTime date)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "UPDATE Tasks SET DUE_DATE=@newDate WHERE EMAIL=@email AND COLUMN_ID=@colid AND TASK_ID=@tId";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.Add(new SQLiteParameter("@newDate", date));
                command.Parameters.Add(new SQLiteParameter("@email", email));
                command.Parameters.Add(new SQLiteParameter("@colid", colId));
                command.Parameters.Add(new SQLiteParameter("@tId", taskId));

          
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Update the title of a task in the database
        /// </summary>
        /// <param name="email">The email address of the user-owner of the board</param>
        /// <param name="colId">The column id of the column that the task belongs to</param>
        /// <param name="taskId">The id of the task to change</param>
        /// <param name="title">The new title of the task</param>
        public void updateTitle(string email, int colId, int taskId,string title)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "UPDATE Tasks SET TITLE=@newTitle WHERE EMAIL=@email AND COLUMN_ID=@colid AND TASK_ID=@tID";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.Add(new SQLiteParameter("@newTitle", title));
                command.Parameters.Add(new SQLiteParameter("@email", email));
                command.Parameters.Add(new SQLiteParameter("@colid", colId));
                command.Parameters.Add(new SQLiteParameter("@tId", taskId));

                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        /// <summary>
        /// Update the description of a task in the database
        /// </summary>
        /// <param name="email">The email address of the user-owner of the board</param>
        /// <param name="colId">The column id of the column that the task belongs to</param>
        /// <param name="taskId">The id of the task to change</param>
        /// <param name="desc">The new description of the task</param>
        public void updateDesc(string email, int colId,int taskId,string desc)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "UPDATE Tasks SET DESCRIPTION=@newDesc WHERE EMAIL=@email AND COLUMN_ID=@colid AND TASK_ID=@tId";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.Add(new SQLiteParameter("@newDesc", desc));
                command.Parameters.Add(new SQLiteParameter("@email", email));
                command.Parameters.Add(new SQLiteParameter("@colid", colId));
                command.Parameters.Add(new SQLiteParameter("@tId", taskId));

          
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Delete a task from the database
        /// </summary>
        /// <param name="email">The email address of the user-owner of the board</param>
        /// <param name="colId">The column id of the column that the task belongs to</param>
        /// <param name="taskId">The id of the task to delete</param>
        public void removeTask(string email, int colId,int taskId)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "DELETE FROM Tasks WHERE EMAIL=@email AND COLUMN_ID=@colid AND TASK_ID=@tId";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.Add(new SQLiteParameter("@email", email));
                command.Parameters.Add(new SQLiteParameter("@colid", colId));
                command.Parameters.Add(new SQLiteParameter("@tId", taskId));

          
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Update the column if of a task in the database
        /// </summary>
        /// <param name="email">The email address of the user-owner of the board</param>
        /// <param name="taskId">The id of the task to change</param>
        /// <param name="newId">The new column id to change</param>
        public void setColID(string email,int taskId,int newID)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "UPDATE Tasks SET COLUMN_ID=@newcol WHERE EMAIL=@email AND TASK_ID=@tId";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.Add(new SQLiteParameter("@newcol", newID));
                command.Parameters.Add(new SQLiteParameter("@email", email));
                command.Parameters.Add(new SQLiteParameter("@tId", taskId));

          
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Update the email assignee of a task in the database
        /// </summary>
        /// <param name="email">The email address of the user-owner of the board</param>
        /// <param name="colId">the column id that the task belongs to</param>
        /// <param name="taskId">The id of the task to change</param>
        /// <param name="emailAssi">The new email assignee to change</param>
        public void updateAssignee(string email, int colId,int taskId,string emailAssi)
        {
            try
            {
                SQLiteConnection conn = rep.openConnection();
                conn.Open();
                string sql =
                    "UPDATE Tasks SET EMAIL_ASSIGNEE=@newAssignee WHERE EMAIL=@email AND COLUMN_ID=@colid AND TASK_ID=@tId";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.Add(new SQLiteParameter("@newAssignee", emailAssi));
                command.Parameters.Add(new SQLiteParameter("@email", email));
                command.Parameters.Add(new SQLiteParameter("@colid", colId));
                command.Parameters.Add(new SQLiteParameter("@tId", taskId));

          
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