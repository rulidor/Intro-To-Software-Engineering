using System;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board_Package
{
    public class Task : IPersistedObject<DataAccessLayer.Task>
    {
        private int Id  { get; set; }
        private DateTime CreationTime { get; set; }
        private DateTime DueDate { get; set; }
        private string Title { get; set; }
        private string Description { get; set; }
        private string emailAssignee { get; set; }

        /// <summary>
        /// Constructor for Task
        /// </summary>
        /// <param name="id">task id</param>
        /// <param name="creationTime">time the task was created</param>
        /// <param name="DueDate">dut date for task</param>
        /// <param name="title">the task's title</param>
        /// <param name="description">the task's description</param>
        public Task(int id, DateTime creationTime, DateTime DueDate, string title, string description, string emailAssignee)
        {
            Id = id;
            CreationTime = creationTime;
            this.DueDate = DueDate;
            Title = title;
            Description = description;
            this.emailAssignee = emailAssignee;
        }
        
        /// <summary>
        /// Constructor for Task
        /// </summary>
        /// <param name="id">task id</param>
        /// <param name="creationTime">time the task was created</param>
        /// <param name="DueDate">dut date for task</param>
        /// <param name="title">the task's title</param>
        public Task(int id, DateTime creationTime, DateTime DueDate, string title, string emailAssignee)
        {
            Id = id;
            CreationTime = creationTime;
            this.DueDate = DueDate;
            Title = title;
            Description = "";
            this.emailAssignee = emailAssignee;
        }

        /// <summary>
        /// Constructor for Task
        /// </summary>
        /// <param name="task">Data layer Task object</param>
        public Task(DataAccessLayer.Task task)
        {
            Id = task.Id;
            CreationTime = task.CreationTime;
            this.DueDate = task.DueDate;
            this.Title = task.Title;
            this.Description = task.Description;
            this.emailAssignee = task.emailAssignee;
        }

        /// <summary>
        /// getter for Task id
        /// </summary>
        /// <returns>the task id</returns>
        public int getId()
        {
            return Id;
        }

        /// <summary>
        /// setter for Task id
        /// </summary>
        /// <param name="id">new task id</param>
        public void setId(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// setter for Task DueDate
        /// </summary>
        /// <param name="DueDate">new DueDate</param>
        public void setDueDate(DateTime DueDate)
        {
            this.DueDate = DueDate;
        }

        /// <summary>
        /// setter for Task title
        /// </summary>
        /// <param name="title">new title</param>
        public void setTitle(string title)
        {
            this.Title = title;
        }

        /// <summary>
        /// setter for Task description
        /// </summary>
        /// <param name="description">new description</param>
        public void setDescription(string description)
        {
            this.Description = description;
        }

        /// <summary>
        /// getter for Task CreationTime
        /// </summary>
        /// <returns>A DateTime object, representing the task creation time</returns>
        public DateTime getCreationTime()
        {
            return CreationTime;
        }

        /// <summary>
        /// getter for Task DueDate
        /// </summary>
        /// <returns>A DateTime object, representing the task due date</returns>
        public DateTime getDueDate()
        {
            return DueDate;
        }

        /// <summary>
        /// getter for Task title
        /// </summary>
        /// <returns>A string representing the Task title</returns>
        public string getTitle()
        {
            return Title;
        }

        /// <summary>
        /// getter for Task description
        /// </summary>
        /// <returns>A string, representing the Task description</returns>
        public string getDescription()
        {
            return Description;
        }

        /// <summary>
        /// convert the Task object to a DataAccessLayer.Task object
        /// </summary>
        /// <returns>A DataAccessLayer.Task object, representing this Task object.</returns>
        public DataAccessLayer.Task toDalObject()
        {
            return new DataAccessLayer.Task(this.Id,this.CreationTime,this.DueDate,this.Title,this.Description, this.emailAssignee);
        }
        
        /// <summary>
        /// setter for Task emailAssignee
        /// </summary>
        /// <param name="newUser">new email assignee</param>
        public void setEmailAssignee(string newUser)
        {
            emailAssignee = newUser;
        }
        /// <summary>
        /// getter for Task emailAssignee
        /// </summary>
        /// <returns>A string, representing the Task emailAssignee</returns>
        public string getEmailAssignee()
        {
            return emailAssignee;
        }
    }
}