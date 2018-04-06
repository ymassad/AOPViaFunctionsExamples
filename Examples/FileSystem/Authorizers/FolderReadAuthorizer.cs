using Examples.FileSystem.DataObjects;

namespace Examples.FileSystem.Authorizers
{
    public class FolderReadAuthorizer : IResourceAuthorize<FolderId>
    {
        public bool HasAccess(FolderId resourceId)
        {
            return true;
        }
    }
}