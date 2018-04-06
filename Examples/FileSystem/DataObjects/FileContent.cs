namespace Examples.FileSystem.DataObjects
{
    public class FileContent
    {
        public FileContent(byte[] content)
        {
            Content = content;
        }

        public byte[] Content { get; }
    }
}