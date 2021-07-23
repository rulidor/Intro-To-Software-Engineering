using System.Collections.ObjectModel;

namespace Presentation.model
{
    public class ColumnModel : NotifiableModelObject
    {
        public ObservableCollection<TaskModel> tasks { get; set; }
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private int _limit;
        public int Limit
        {
            get => _limit;
            set
            {
                _limit = value;
                RaisePropertyChanged("Limit");
            }
        }


        public ColumnModel(BackendController controller,string name,int limit) : base(controller)
        {
            this.Name = name;
            this.Limit = limit;
            
        }
        
        
    }
}