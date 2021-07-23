
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board_Package
{
    public class Column : IPersistedObject<DataAccessLayer.Column>
    {
        private Dictionary<int, Task> Tasks { get; set; }
        private  string Name { get; set; }
        private  int Limit { get; set; }
        
        /// <summary>
        /// constructor for Column with limited tasks number
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="limit">number of max. tasks for the column. if -1 then unlimited.</param>
        public Column(string name, int limit)
        {
            Tasks = new Dictionary<int, Task>();
            Name = name;
            Limit = limit;
        }
        
        /// <summary>
        /// constructor for Column, constructs it to be unlimited bounded for tasks
        /// </summary>
        /// <param name="name">column name</param>
        public Column(string name)
        {
            Tasks = new Dictionary<int, Task>();
            Name = name;
            Limit = 100;
        }

        /// <summary>
        /// adds a Task to this Column
        /// </summary>
        /// <param name="task">task to be added</param>
        /// <returns>the task added</returns>
        public Task addTask(Task task)
        {
            if(Tasks.Count >= Limit)
                throw new Exception("cannot add task due to limit");
            Tasks.Add(task.getId(), task);
            return task;
        }
        
        /// <summary>
        /// deleted a Task from this Column
        /// </summary>
        /// <param name="task">task to be deleted</param>
        public void deleteTask(int taskId)
        {
            Tasks.Remove(taskId);
        }

        /// <summary>
        /// getter for a Task in this Column
        /// </summary>
        /// <param name="id">task id</param>
        /// <returns>A Task object, which has the id received</returns>
        public Task getTask(int id)
        {
            return Tasks[id];
        }

        /// <summary>
        /// limits number of Tasks at this Column
        /// </summary>
        /// <param name="limit">number of max. Tasks to be defined at this column</param>
        public void setLimit(int limit)
        {
            if(limit < 0)
                throw new Exception("limit can't be lower than 0");
            if(Tasks.Keys.Count > limit)
                throw new Exception($"limit is smaller than number of tasks");
            this.Limit = limit;
        }

        /// <summary>
        /// edits a Task at this Column
        /// </summary>
        /// <param name="task">task to be edited</param>
        public void editTask(Task task)
        {
            Tasks[task.getId()] = task;
        }

        /// <summary>
        /// getter for all of this Column's Tasks
        /// </summary>
        /// <returns>A Dictionary<int, Task> object, containing all the tasks in this column, mapped by their task id.</returns>
        public Dictionary<int, Task> getTasks()
        {
            return Tasks;
        }

        /// <summary>
        /// getter for this Column number of max. Tasks
        /// </summary>
        /// <returns>the limit of this column</returns>
        public int getLimit()
        {
            return Limit;
        }

        /// <summary>
        /// getter for this Column's name
        /// </summary>
        /// <returns>column's name</returns>
        public string getName()
        {
            return Name;
        }
        
        /// <summary>
        /// converts this Column to a DataAccessLayer.Column object
        /// </summary>
        /// <returns>A DataAccessLayer.Column object, representing this Column</returns>
        public DataAccessLayer.Column toDalObject()
        {
            List<DataAccessLayer.Task> tasks=new List<DataAccessLayer.Task>();
            foreach (var t in this.Tasks)
            {
                tasks.Add(t.Value.toDalObject());
            }
            return new DataAccessLayer.Column(this.Name,this.Limit,tasks);
        }

        /// <summary>
        /// setter for this Column's Tasks
        /// </summary>
        /// <param name="tasks">tasks to be added, mapped by their task id</param>
        public void setTasks(Dictionary<int, Task> tasks)
        {
            this.Tasks = tasks;
        }
        /// <summary>
        /// setter for this Column's name
        /// </summary>
        /// <param name="newName">the new name of the column</param>
        public void setName(string newName)
        {
            this.Name = newName;
        }
        
        public Dictionary<int, Task> sortByDueDate()
        {
            Dictionary<int, Task> tasks = Tasks;
            tasks.OrderBy(key => key.Value.getDueDate());
            return tasks;
        }
    }
}