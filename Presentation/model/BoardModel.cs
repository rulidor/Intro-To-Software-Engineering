using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Presentation.model
{
    public class BoardModel : NotifiableModelObject
    {
        public ObservableCollection<ColumnModel> _columns;
        public ObservableCollection<ColumnModel> ColumnNames
        {
            get => _columns;
            set
            {
                _columns = value;
                RaisePropertyChanged("ColumnNames");
            }
        }
        private string _userEmail;

        public string UserEmail
        {
            get => _userEmail;
            set
            {
                _userEmail = value;
                RaisePropertyChanged("UserEmail");
            }
        }
        
        

        public BoardModel(BackendController controller,string userEmail,Dictionary<string,int> columns) : base(controller)
        {
            UserEmail = userEmail;
            LinkedList<ColumnModel> colMod = new LinkedList<ColumnModel>();
            foreach (var m in columns)
            {
                colMod.AddLast(new ColumnModel(controller,m.Key,m.Value));
            }
            
            ColumnNames = new ObservableCollection<ColumnModel>(colMod);
            ColumnNames.CollectionChanged += HandleChange;

        }

       
        public void HandleChange(object sender, NotifyCollectionChangedEventArgs e)

        {
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ColumnModel m in e.OldItems)
                    Controller.removeColumn(UserEmail, 8);

            }
            


        }
    }
}