namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// An interface, which all Business Layer objects that should be stored in the database
    /// need to implement, in order to convert themselves into DataAccessLayer objects.
    /// </summary>
    public interface IPersistedObject <T> where T:  DataAccessLayer.DalObject<T>
    {
        // T toDalObject();
    }
}