using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using IntroSE.Kanban.Backend.ServiceLayer;
using Board = IntroSE.Kanban.Backend.BusinessLayer.Board_Package.Board;

namespace Presentation.model
{
    public class TaskModel : NotifiableModelObject
    {
        private  int _id;
        public int Id
        {
            get => _id;
            set
            {
                this._id = value;
                RaisePropertyChanged("Id");
            }
        } 
        private  DateTime _creationTime;
        public DateTime CreationTime
        {
            get => _creationTime;
            set
            {
                _creationTime = value;
                RaisePropertyChanged("CreationTime");
                
            } 
        }
        
        private  DateTime _dueDate;
        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                DateTime temp = _dueDate;
                _dueDate = value;
                RaisePropertyChanged("DueDate");
                /*try
                {
                    Controller.updateDueDate(boardEmail,ColId,Id,value);
                }
                catch (Exception e)
                {
                    _dueDate = temp;
                    MessageBox.Show("update due date failed: " + e.Message);
                }*/
                
            } 
        }
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                string temp = _title;
                _title = value;
                RaisePropertyChanged("Title");
                /*try
                {
                    Controller.updateTitle(boardEmail,ColId,Id,value);
                }
                catch (Exception e)
                {
                    _title = temp;
                    MessageBox.Show("update task title failed: " + e.Message);
                }*/
            } 
        }

        private string _description;

        public string Description
        {
            get => _description;
            set
            {
                string temp = _description;
                _description = value;
                RaisePropertyChanged("Description");
                /*try
                {
                    Controller.updateDescription(boardEmail,ColId,Id,Description);
                }
                catch (Exception e)
                {
                    _description = temp;
                    MessageBox.Show("update task Description failed: " + e.Message);
                }*/
                
            } 
        }

        private string _emailAssignee;

        public string EmailAssignee
        {
            get => _emailAssignee;
            set
            {
                _emailAssignee = value;
                RaisePropertyChanged("EmailAssignee");
                
            } 
        }
        private SolidColorBrush _bgColor;

        public SolidColorBrush BgColor
        {
            get => _bgColor; 
            set => _bgColor = value;
        }

        private int _colId;

        public int ColId
        {
            get => _colId;
            set
            {
                _colId= value;
                RaisePropertyChanged("EmailAssignee");
                
            } 
        }

        private string boardEmail; //storing this user here is an hack becuase static & singletone are not allowed.
        public TaskModel(BackendController controller, int id, DateTime creationTime, DateTime dueDate ,string title, string description, string emailAssi,string user_email,int colID) :
            base(controller)
        {
            Id = id;
            CreationTime = creationTime;
            DueDate = dueDate;
            Title = title;
            Description = description;
            EmailAssignee = emailAssi;
            boardEmail = user_email;
            BgColor = new SolidColorBrush(Colors.Transparent);
            ColId = colID;

        }
        
        public override bool Equals(object obj)
        {
            return obj is TaskModel task &&
                   BgColor.Equals(task.BgColor);
        }
        
        public override int GetHashCode()
        {
            return 1998019921 + BgColor.GetHashCode();
        }
        
        /// <summary>
        /// a functions that changes the background color of the task 
        /// </summary>
        /// <param name="email"> assignee eamil of a task</param>
        /// <returns></returns>
        public Color getColor(string email)
        {
            if (DueDate.CompareTo(DateTime.Now) < 0)
            {
                return Colors.Red;
            }
            
            int t1 = (int)(this.DueDate - this.CreationTime).TotalSeconds;
            int t2 = (int)(DateTime.Now - this.CreationTime).TotalSeconds;

            double precsent = ((double)t2) / t1;
            if (precsent >= 0.75)
            {
                return Colors.Orange;
            }

            if (this.EmailAssignee.Equals(email))
                return Colors.Blue;
            return  Colors.Transparent;
                
            
        }
    }
}