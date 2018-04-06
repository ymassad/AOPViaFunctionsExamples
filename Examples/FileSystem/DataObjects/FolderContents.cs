using System.Collections.Immutable;

namespace Examples.FileSystem.DataObjects
{
    public class FolderContents
    {
        public FolderContents(ImmutableArray<IdAndName<FileId>> files, ImmutableArray<IdAndName<FolderId>> subFolders)
        {
            Files = files;
            SubFolders = subFolders;
        }

        public ImmutableArray<IdAndName<FileId>> Files { get; }

        public ImmutableArray<IdAndName<FolderId>> SubFolders { get; }

    }
}