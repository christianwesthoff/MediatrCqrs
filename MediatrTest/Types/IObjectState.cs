namespace MediatrTest.Types
{
    public enum EObjectState
    {
        Unchanged,
        Deleted,
        Added,
        Modified,
        Detached
    }
    
    public interface IEntity
    {
        EObjectState ObjectState { get; set; }
    }
}