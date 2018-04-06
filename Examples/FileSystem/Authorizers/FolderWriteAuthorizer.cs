using Examples.FileSystem.DataObjects;

namespace Examples.FileSystem.Authorizers
{
    public class FolderWriteAuthorizer : IResourceAuthorize<FolderId>
    {
        public bool HasAccess(FolderId resourceId)
        {
            return true;
        }
    }
}