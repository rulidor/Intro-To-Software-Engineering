using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    
    public abstract class DalObject<T> where T : DalObject<T>
    {
    
        /// <summary>
        /// An abstruct dave methode
        /// </summary>
        public abstract void save();
    }
}