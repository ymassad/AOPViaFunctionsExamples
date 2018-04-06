namespace Examples.FileSystem.DataObjects
{
    public class IdAndName<TId>
    {
        public IdAndName(TId id, string name)
        {
            Id = id;
            Name = name;
        }

        public TId Id { get; }

        public string Name { get; }
    }
}