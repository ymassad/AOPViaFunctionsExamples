using System;

namespace Examples.FileSystem.DataObjects
{
    public class FolderId
    {
        public FolderId(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}