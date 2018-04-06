using System.Collections.Immutable;
using Examples.FileSystem.DataObjects;

namespace Examples.FileSystem
{
    public class FileSystemService : IFileSystemService
    {
        public FileContent GetFileContent(FileId fileId)
        {
            return new FileContent(new byte[0]);
        }

        public FolderContents GetFolderContents(FolderId folderId)
        {
            return new FolderContents(ImmutableArray<IdAndName<FileId>>.Empty, ImmutableArray<IdAndName<FolderId>>.Empty);
        }

        public void CopyFileToFolder(FileId fileId, FolderId folderId)
        {
            
        }
    }
}