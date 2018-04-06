using System;
using Examples.FileSystem.DataObjects;
using Functions;

namespace Examples.FileSystem
{
    public interface IFileSystemService
    {
        FileContent GetFileContent(FileId fileId);

        FolderContents GetFolderContents(FolderId folderId);

        void CopyFileToFolder(FileId fileId, FolderId folderId);
    }

    public static class FileSystemServiceMapExtensions
    {
        public static IFileSystemService Map(this IFileSystemService instance, Func<(IFunction<FileId, FileContent> getFileContent, IFunction<FolderId, FolderContents> getFolderContents, IFunction<(FileId fileId, FolderId folderId), Unit> copyFileToFolder), (IFunction<FileId, FileContent> getFileContent, IFunction<FolderId, FolderContents> getFolderContents, IFunction<(FileId fileId, FolderId folderId), Unit> copyFileToFolder)> decorationFunction)
        {
            var functions = decorationFunction((new GetFileContentToFunctionClass(instance), new GetFolderContentsToFunctionClass(instance), new CopyFileToFolderToFunctionClass(instance)));
            return new FromFunctionClass(functions.Item1, functions.Item2, functions.Item3);
        }

        public static (IFunction<FileId, FileContent> getFileContent, IFunction<FolderId, FolderContents> getFolderContents, IFunction<(FileId fileId, FolderId folderId), Unit> copyFileToFolder) ToFunctions(this IFileSystemService instance)
        {
            return (new GetFileContentToFunctionClass(instance), new GetFolderContentsToFunctionClass(instance), new CopyFileToFolderToFunctionClass(instance));
        }

        public static IFileSystemService ToFileSystemService(this (IFunction<FileId, FileContent> getFileContent, IFunction<FolderId, FolderContents> getFolderContents, IFunction<(FileId fileId, FolderId folderId), Unit> copyFileToFolder) functions)
        {
            return new FromFunctionClass(functions.getFileContent, functions.getFolderContents, functions.copyFileToFolder);
        }

        private class FromFunctionClass : IFileSystemService
        {
            private readonly IFunction<FileId, FileContent> function1;
            private readonly IFunction<FolderId, FolderContents> function2;
            private readonly IFunction<(FileId fileId, FolderId folderId), Unit> function3;
            public FromFunctionClass(IFunction<FileId, FileContent> function1, IFunction<FolderId, FolderContents> function2, IFunction<(FileId fileId, FolderId folderId), Unit> function3)
            {
                this.function1 = function1;
                this.function2 = function2;
                this.function3 = function3;
            }

            public FileContent GetFileContent(FileId fileId)
            {
                return this.function1.Invoke(fileId);
            }

            public FolderContents GetFolderContents(FolderId folderId)
            {
                return this.function2.Invoke(folderId);
            }

            public void CopyFileToFolder(FileId fileId, FolderId folderId)
            {
                this.function3.Invoke((fileId, folderId));
            }
        }

        private class GetFileContentToFunctionClass : IFunction<FileId, FileContent>
        {
            private readonly IFileSystemService instance;
            public GetFileContentToFunctionClass(IFileSystemService instance)
            {
                this.instance = instance;
            }

            public FileContent Invoke(FileId input)
            {
                return this.instance.GetFileContent(input);
            }
        }

        private class GetFolderContentsToFunctionClass : IFunction<FolderId, FolderContents>
        {
            private readonly IFileSystemService instance;
            public GetFolderContentsToFunctionClass(IFileSystemService instance)
            {
                this.instance = instance;
            }

            public FolderContents Invoke(FolderId input)
            {
                return this.instance.GetFolderContents(input);
            }
        }

        private class CopyFileToFolderToFunctionClass : IFunction<(FileId fileId, FolderId folderId), Unit>
        {
            private readonly IFileSystemService instance;
            public CopyFileToFolderToFunctionClass(IFileSystemService instance)
            {
                this.instance = instance;
            }

            public Unit Invoke((FileId fileId, FolderId folderId) input)
            {
                this.instance.CopyFileToFolder(input.fileId, input.folderId);
                return Unit.Default;
            }
        }
    }
}