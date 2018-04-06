namespace Examples.FileSystem.Authorizers
{
    public interface IResourceAuthorize<in TResourceId>
    {
        bool HasAccess(TResourceId resourceId);
    }
}