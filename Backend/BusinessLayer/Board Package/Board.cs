using System;
using System.Collections.Generic;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board_Package
{
    public class Board: IPersistedObject<DataAccessLayer.Board>
    {
        readonly int colNameSize = 15;
        private readonly int minCol = 2;
        private string userEmail { get; set; }
        private Dictionary<int, Column> columns { get; set; }
        private int index { get; set; }
        
        /// <summary>
        /// constructor for Board
        /// </summary>
        /// <param name="userEmail">board's user email</param>
        public Board(string userEmail)
        {
            this.userEmail = userEmail;
            columns = new Dictionary<int, Column>();
            columns.Add(0, new Column("backlog"));
            columns.Add(1, new Column("in progress"));
            columns.Add(2, new Column("done"));
            index = 1;
        }

        /// <summary>
        /// constructor for Board, with given columns
        /// </summary>
        /// <param name="userEmail">task to be added</param>
        /// <param name="columns">columns to be contained at this Board, mapped by their column ordinary number</param>
        public Board(string userEmail, Dictionary<int, Column> columns)
        {
            this.userEmail = userEmail;
            this.columns = columns;
        }
        
        /// <summary>
        /// adds a Task for this Board
        /// </summary>
        /// <param name="task">task to be added</param>
        /// <returns>the task added</returns>
        public Task addTask(Task task)
        {
            try
            {
                task.setId(index);
                Task t = columns[0].addTask(task);
                index++;
                return t;
            }
            catch (Exception e)
            {
                
                throw e;
            }
            
        }

        
        /// <summary>
        /// getter for columns at this Board
        /// </summary>
        /// <returns>A Dictionary<int, Column> object, representing this Board columns, mapped by their ordinary number</returns>
        public Dictionary<int, Column> getColumns()
        {
            return columns;
        }

      
        /// <summary>
        /// advances a task at this Board. If tasks at the last column - will not be advanced
        /// </summary>
        /// <param name="email">user email</param>
        /// <param name="columnOrdinal">column num., which the task is currently at</param>
        /// <param name="taskId">task to be advanced</param>
        /// <returns>the task advanced</returns>
        public Task advanceTask(string email,int columnOrdinal, int taskId)
        {
            try
            {
                if(!columns.ContainsKey(columnOrdinal))
                    throw new Exception("cannot get column");
                if(!columns[columnOrdinal].getTasks().ContainsKey(taskId))
                    throw new Exception("task is not in column");
                if(columnOrdinal==columns.Count-1)
                    throw new Exception("can't change tasks in last column");
                if(!columns.ContainsKey(columnOrdinal + 1))
                    throw new Exception("cannot get next column");
                if (columns[columnOrdinal + 1].getTasks().Count >= columns[columnOrdinal + 1].getLimit())
                    throw new Exception("can't advanced task due to limit");

                Task t = columns[columnOrdinal].getTask(taskId);
                if (!email.Equals(t.getEmailAssignee())) 
                    throw new Exception("email is not te assignee email");
                columns[columnOrdinal].getTasks().Remove(taskId);
                columns[columnOrdinal + 1].addTask(t);
                return t;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// setter for a column limit at this Board
        /// </summary>
        /// <param name="columnOrdinal">column ordinal num. to be limited</param>
        /// <param name="limit">new limit to set</param>
        public void setLimit(int columnOrdinal, int limit, string creator)
        {
            try
            {
                if(!userEmail.Equals(creator))
                    throw new Exception("only the creator of the board can change the limit");
                if(!columns.ContainsKey(columnOrdinal))
                    throw new Exception("cannot get column");
                columns[columnOrdinal].setLimit(limit);
                columns[columnOrdinal].toDalObject().setLimit(userEmail,columnOrdinal,limit);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        /// <summary>
        /// getter for a column at this Board, by its columnOrdinal
        /// </summary>
        /// <param name="columnOrdinal">column ordinal num.</param>
        /// <returns>A Column object, representing the desired column</returns>
        public Column getColumn(int columnOrdinal)
        {
            if(!columns.ContainsKey(columnOrdinal))
                throw new Exception("cannot get column");
            return columns[columnOrdinal];
        }
        
        /// <summary>
        /// getter for a column at this Board, by its columnName
        /// </summary>
        /// <param name="columnName">column ordinal name</param>
        /// <returns>A Column object, representing the desired column</returns>
        public Column getColumn(string column)
        {
            Column c = null;
            foreach (var col in columns.Values)
            {
                if (col.getName().Equals(column))
                    c = col;
            }
            if(c == null)
                throw new Exception("column doesn't exist");
            return c;
        }

        /// <summary>
        /// getter for this Board's index, representing task's index
        /// </summary>
        /// <returns>index of this board</returns>
        public int getIndex()
        {
            return this.index;
        }
        /// <summary>
        /// update the dueDate of a task in this board
        /// </summary>
        /// <param name="email">email of the owner of the board</param>
        /// <param name="column">column ordinal num.</param>
        /// <param name="taskId">task id that we want to change</param>
        /// <param name="DueDate">the new dueDate of the task</param>
        public void UpdateTaskDueDate(string email, int column, int taskId, DateTime DueDate)
        {
            if (!columns.ContainsKey(column))
                throw new Exception("cannot get column");
            if (!columns[column].getTasks().ContainsKey(taskId))
            {
                throw new Exception("task not found");
            }
            if (DueDate==null || DueDate <= DateTime.Now)
            {
                throw new Exception("due date not valid");
            }
            if(!columns[column].getTasks()[taskId].getEmailAssignee().Equals(email))
                throw new Exception("email is not the assignee email of the task");
            if (column == columns.Count-1)
                throw new Exception("cant change task because its in the done column");
            columns[column].getTasks()[taskId].setDueDate(DueDate);
        }
        /// <summary>
        /// setter for this Board's index
        /// </summary>
        /// <param name="i">new index</param>
        public void setIndex(int i)
        {
            this.index = i;
        }

        /// <summary>
        /// removes a column from this Board, by its columnOrdinal
        /// </summary>
        /// <param name="columnOrdinal">column ordinal num.</param>
        public void removeColumn(int columnOrdinal)
        {
            Dictionary<int, Column> newColumns = new Dictionary<int, Column>();
            if (!columns.ContainsKey(columnOrdinal))
                throw new Exception("cannot get column");
            if(columns.Count<=minCol)
                throw new Exception("a board must have at least 2 columns");
            if (columnOrdinal == 0)
            {
                if (!columns.ContainsKey(columnOrdinal + 1))
                    throw new Exception("cannot get next column");
                if (columns[columnOrdinal + 1].getTasks().Count + columns[columnOrdinal].getTasks().Count >=
                    columns[columnOrdinal + 1].getLimit())
                    throw new Exception("can't advance task due to limit");
                foreach (var t in columns[columnOrdinal].getTasks().Values)
                {
                    columns[columnOrdinal + 1].addTask(t);
                    
                }
            }
            else
            {
                if (columns[columnOrdinal - 1].getTasks().Count + columns[columnOrdinal].getTasks().Count >=
                    columns[columnOrdinal - 1].getLimit())
                    throw new Exception("can't advance task due to limit");
                
                foreach (var t in columns[columnOrdinal].getTasks().Values)
                {
                    columns[columnOrdinal - 1].addTask(t);
                    
                }
            }
            foreach (var col in columns)
            {
                if(col.Key < columnOrdinal)
                    newColumns.Add(col.Key, col.Value);
                else if(col.Key > columnOrdinal)
                {
                    newColumns.Add(col.Key - 1, col.Value);
                }
            }
            columns = newColumns;
            this.toDalObject().deleteCol(userEmail,columnOrdinal);
            foreach (var col in columns)
            {
                foreach (var task in col.Value.getTasks().Values)
                {
                    task.toDalObject().setColID(userEmail,task.getId(),col.Key);
                }
                toDalObject().updateColeId(userEmail,col.Value.getName(),col.Key);
                
            }
            
        }

        /// <summary>
        /// adds a column at this Board
        /// </summary>
        /// <param name="columnOrdinal">column ordinal num.</param>
        /// <param name="columnName">column name</param>
        /// <returns>A Column object, representing the added column</returns>
        public Column addColumn(int columnOrdinal, string columnName)
        {
            if( string.IsNullOrWhiteSpace(columnName) || columnName.Length > colNameSize )
                throw new Exception("column name is not valid");
            foreach (var col in columns.Values)
            {
                if(col.getName().Equals(columnName))
                    throw new Exception("column name already exists in board");
            }
            
            Dictionary<int, Column> newColumns = new Dictionary<int, Column>();
            if(columnOrdinal >= columns.Count + 1)
                throw new Exception("cannot add a column with empty column between them");
            if (columnOrdinal == columns.Count)
            {
                columns.Add(columnOrdinal, new Column(columnName));
                columns[columnOrdinal].toDalObject().saveColumns(userEmail,columnOrdinal);
                return columns[columnOrdinal];
            }

            int index = 1;
            foreach (var col in columns)
            {
                if(col.Key < columnOrdinal)
                    newColumns.Add(col.Key, col.Value);
                else if(col.Key == columnOrdinal)
                {
                    newColumns.Add(columnOrdinal, new Column(columnName));
                    newColumns.Add(columnOrdinal + index, col.Value);
                    index++;
                }
                else
                {
                    newColumns.Add(columnOrdinal + index, col.Value);
                    index++;
                }
            }
            columns = newColumns;
            foreach (var col in columns)
            {
                toDalObject().updateColeId(userEmail,col.Value.getName(),col.Key);
                foreach (var task in col.Value.getTasks())
                {
                    task.Value.toDalObject().setColID(userEmail,task.Key,col.Key);
                }
            }
            columns[columnOrdinal].toDalObject().saveColumns(userEmail,columnOrdinal);

            return columns[columnOrdinal];
        }

        /// <summary>
        /// moves a column to the right and swaps its righthand column, at this Board
        /// </summary>
        /// <param name="columnOrdinal">column ordinal num. to be shifted to the right</param>
        /// <param name="creator"> the creator of this board</param>
        /// <returns>A Column object, representing the shifted to the right's column</returns>
        public Column MoveColumnRight(int columnOrdinal, string creator)
        {
            
            if(!columns.ContainsKey(columnOrdinal))
                throw new Exception("cannot get column");
            if(!columns.ContainsKey(columnOrdinal + 1))
                throw new Exception("cannot swap columns locations");
            Column c1 = columns[columnOrdinal];
            Column c2 = columns[columnOrdinal + 1];
            columns.Remove(columnOrdinal);
            columns.Remove(columnOrdinal + 1);
            columns.Add(columnOrdinal + 1, c1);
            foreach (var task in columns[columnOrdinal+1].getTasks())
            {
                task.Value.toDalObject().setColID(userEmail,task.Key,columnOrdinal+1);
            }
            columns.Add(columnOrdinal, c2);
            foreach (var task in columns[columnOrdinal].getTasks())
            {
                task.Value.toDalObject().setColID(userEmail,task.Key,columnOrdinal);
            }
            
            toDalObject().moveCOlRight(userEmail,c1.getName(),-1,columnOrdinal);
            toDalObject().moveCOlRight(userEmail,c2.getName(),columnOrdinal,columnOrdinal+1);
            toDalObject().moveCOlRight(userEmail,c1.getName(),columnOrdinal+1,-1);
            
            return columns[columnOrdinal + 1];
        }

        /// <summary>
        /// moves a column to the left and swaps its lefthand column, at this Board
        /// </summary>
        /// <param name="columnOrdinal">column ordinal num. to be shifted to the left</param>
        /// <param name="creator"> the creator of this board</param>
        /// <returns>A Column object, representing the shifted to the left's column</returns>
        public Column MoveColumnLeft(int columnOrdinal, string creator)
        {
            if(!columns.ContainsKey(columnOrdinal))
                throw new Exception("cannot get column");
            if(!userEmail.Equals(creator))
                throw new Exception("only the board creator can order the columns");
            if(columnOrdinal == 0)
                throw new Exception("cannot swap columns locations");
            Column c1 = columns[columnOrdinal];
            Column c2 = columns[columnOrdinal - 1];
            columns.Remove(columnOrdinal);
            columns.Remove(columnOrdinal - 1);
            columns.Add(columnOrdinal - 1, c1);
            foreach (var task in columns[columnOrdinal-1].getTasks().Values)
            {
                task.toDalObject().setColID(userEmail,task.getId(),columnOrdinal-1);
            }
            columns.Add(columnOrdinal, c2);
            foreach (var task in columns[columnOrdinal].getTasks().Values)
            {
                task.toDalObject().setColID(userEmail,task.getId(),columnOrdinal);
            }
            
            toDalObject().moveCOlRight(userEmail,c1.getName(),-1,columnOrdinal);
            toDalObject().moveCOlRight(userEmail,c2.getName(),columnOrdinal,columnOrdinal-1);
            toDalObject().moveCOlRight(userEmail,c1.getName(),columnOrdinal-1,-1);
            return columns[columnOrdinal - 1];
        }

        /// <summary>
        /// converts this Board to a DataAccessLayer.Board object
        /// </summary>
        /// <returns>A DataAccessLayer.Board object, representing this Board</returns>
         public DataAccessLayer.Board toDalObject()
         {
             Dictionary<int,DataAccessLayer.Column> cols=new Dictionary<int, DataAccessLayer.Column>();
             foreach (var entry in columns)
             {
                 cols.Add(entry.Key,entry.Value.toDalObject());
             }
            return new DataAccessLayer.Board(this.userEmail,cols);
         }
        /// <summary>
        /// change column name
        /// </summary>
        /// <param name="email">email of the owner of the board/param>
        /// <param name="columnOrdinal">column ordinal of the column we wish to change name</param>
        /// <param name="newName">the new column name</param>
        public void setName( string email,int columnOrdinal, string newName)
        {
            foreach (var col in columns.Values)
            {
                if(col.getName().Equals(newName))
                    throw new Exception("column name already exists in board");
            }
            if(columnOrdinal < 0 || columnOrdinal >= columns.Count)
                throw new Exception("illegal column index");
            if(!email.Equals(userEmail))
                throw new Exception("only creator can change columns");
            columns[columnOrdinal].setName(newName);
            columns[columnOrdinal].toDalObject().setName(userEmail,columnOrdinal,newName);
            
            
        }

        public Board sortBoard()
        {
            Dictionary<int, Column> newColumns = columns;
            foreach (var col in newColumns.Values)
            {
                col.sortByDueDate();
            }
            return new Board(userEmail, newColumns);
        }
        /// <summary>
        /// assign a task to a user
        /// </summary>
        /// <param name="columnOrdinal">column ordinal of the task</param>
        /// <param name="taskId">task Id to assign</param>
        /// <param name="emailAssignee"> the email assignee of the task</param>
        public void AssignTask(int columnOrdinal, int taskId, string emailAssignee)
        {
            if(!columns.ContainsKey(columnOrdinal))
                throw new Exception("column doesn't exist");
            if(columns.Count-1==columnOrdinal)
                throw new Exception("can't change tasks in the last column"); 
            if(!columns[columnOrdinal].getTasks().ContainsKey(taskId))
                throw new Exception("task doesn't exist");
            columns[columnOrdinal].getTasks()[taskId].setEmailAssignee(emailAssignee);
            columns[columnOrdinal].getTasks()[taskId].toDalObject().updateAssignee(userEmail,columnOrdinal,taskId,emailAssignee);
            
        }
        /// <summary>
        /// deletes a task from a column in this board
        /// </summary>
        /// <param name="columnOrdinal">column ordinal of the task</param>
        /// <param name="taskId">task Id to delete</param>
        /// <param name="emailAssignee">email assignee of the task to delete</param>
        public void deleteTask(int columnOrdinal, int taskId, string emailAssignee)
        {
            if(!columns.ContainsKey(columnOrdinal))
                throw new Exception("column doesn't exist");
            if(columns.Count-1==columnOrdinal)
                throw new Exception("can't change tasks in the last column"); 
            if(!columns[columnOrdinal].getTasks().ContainsKey(taskId))
                throw new Exception("task doesn't exist");
            if(!emailAssignee.Equals(columns[columnOrdinal].getTasks()[taskId].getEmailAssignee()))
                throw new Exception("only the assignee can delete a task");
            columns[columnOrdinal].getTasks()[taskId].toDalObject().removeTask(userEmail,columnOrdinal,taskId);
            columns[columnOrdinal].deleteTask(taskId);
            
        }
    }
}