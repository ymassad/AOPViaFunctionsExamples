using Examples.FileSystem.DataObjects;

namespace Examples.FileSystem.Authorizers
{
    public class FileReadAuthorizer : IResourceAuthorize<FileId>
    {
        public bool HasAccess(FileId resourceId)
        {
            return true;
        }
    }
}