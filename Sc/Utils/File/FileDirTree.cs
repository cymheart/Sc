
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Utils
{
    public class FilePath
    {
       public string name;
       public string path;
    }

    public class FileDir
    {
        public List<FileDir> childFileDirs = new List<FileDir>();       
        public List<FilePath> filePathList = new List<FilePath>();
        public string name;


        public FileDir GetChildDir(string name)
        {
            string[] dirs = name.Split('/','\\');
            int i = 0;
            int j = 0;
            List<FileDir> childDirs = childFileDirs;
            FileDir fileDir = null;

            do
            {
                for (j = 0; j < childDirs.Count; j++)
                {
                    fileDir = childDirs[j];

                    if (fileDir.name.Equals(dirs[i]))      
                        break;
                }

                if (j == childDirs.Count)
                    return null;

                if (i + 1 == dirs.Length )
                    return fileDir;

                i++;
                childDirs = fileDir.childFileDirs;

            } while (true);

        }


        public string GetFile(string fileName)
        {
            foreach (FilePath filePath in filePathList)
            {
                if (filePath.name.Equals(fileName))
                    return filePath.path;
            }

            return null;
        }


        public Dictionary<string, FilePath> FlatFileDirFile(string rootDir, string excludeDir)
        {
            return FlatFileDirFile(rootDir, new string[] { excludeDir });
        }


        public Dictionary<string, FilePath> FlatFileDirFile(string rootDir, string[] excludeDirs)
        {
            Dictionary<string, FilePath> pathDict = new Dictionary<string, FilePath>();
            FileDir imageDir = GetChildDir(rootDir);
            FlatFileDirFile(pathDict, imageDir, excludeDirs);
            return pathDict;
        }


        void FlatFileDirFile(Dictionary<string, FilePath> pathDict, FileDir fileDir, string[] excludeDirs)
        {
            foreach (FilePath path in fileDir.filePathList)
            {
                if(!pathDict.ContainsKey(path.name))
                    pathDict.Add(path.name, path);
            }

            foreach (FileDir nodedir in fileDir.childFileDirs)
            {
                bool isExclude = false;

                for (int i = 0; i < excludeDirs.Length; i++)
                {
                    if (nodedir.name == excludeDirs[i])
                    {
                        isExclude = true;
                        break;
                    }
                }

                if (!isExclude)
                    FlatFileDirFile(pathDict, nodedir, excludeDirs);
            }
        }


        static public FileDir CreateChildFileDir(FileDir parentDir, DirectoryInfo directoryFolder, string searchPattern, string replacePattern)
        {
            FileDir childFileDir;

            DirectoryInfo[] dictTypeFolderList = directoryFolder.GetDirectories();
            FileInfo[] fileInfoList = directoryFolder.GetFiles(searchPattern);

            FileDir fileDir = new FileDir();
            fileDir.name = directoryFolder.Name;
            FilePath filePath;

            for (int i = 0; i < fileInfoList.Length; i++)
            {
                filePath = new FilePath();
                filePath.path = fileInfoList[i].FullName;
                filePath.name = fileInfoList[i].Name.Replace(replacePattern, "");

                fileDir.filePathList.Add(filePath);
            }

            for (int i = 0; i < dictTypeFolderList.Length; i++)
            {
                childFileDir = CreateChildFileDir(fileDir, dictTypeFolderList[i], searchPattern, replacePattern);
                fileDir.childFileDirs.Add(childFileDir);
            }

            return fileDir;
        }

        static public FileDir CreateFileDirTree(string str, string searchPattern, string replacePattern)
        {
            DirectoryInfo directoryFolder = new DirectoryInfo(str);
            FileDir fileDirRoot = CreateChildFileDir(null, directoryFolder, searchPattern, replacePattern);
            return fileDirRoot;
        }
    }
}
