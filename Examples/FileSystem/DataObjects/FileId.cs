using System;

namespace Examples.FileSystem.DataObjects
{
    public class FileId
    {
        public FileId(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}