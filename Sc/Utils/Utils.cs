using System.Collections.Generic;
using System.IO;


namespace PrintUtils
{
    public class Utils
    {
        public static byte[] CreateRegDataToBytes(string regEditPath)
        {
            string path = regEditPath;

            int startIdx = path.IndexOf("\\");
            string domain = path.Substring(0, startIdx);

            int endIdx = path.LastIndexOf("\\");
            string name = path.Substring(endIdx + 1);
            string subkey = path.Substring(startIdx + 1, endIdx - startIdx - 1);

            Register register = new Register();
            register.SubKey = subkey;
            register.Domain = register.GetRegDomainFromName(domain);
            register.RegeditKey = name;


            byte[] a = (byte[])(register.ReadRegeditKey());
            return a;
        }

        public static bool CreateRegDataToFile(string regEditPath, string saveFilePath)
        {

            byte[] a = CreateRegDataToBytes(regEditPath);
            if (a == null)
                return false;

            CreateDir(saveFilePath);

            BinaryWriter bw = new BinaryWriter(new FileStream(saveFilePath, FileMode.Create));
            bw.Write(a);
            bw.Close();

            return true;

        }

        public static void CreateRegDataToRegEdit(string regFilePath, string regEditPath)
        {
            Register register = new Register();

            string path = regEditPath;

            int startIdx = regEditPath.IndexOf("\\");
            string domain = regEditPath.Substring(0, startIdx);

            int endIdx = regEditPath.LastIndexOf("\\");
            string key = regEditPath.Substring(endIdx + 1);
            string subkey = regEditPath.Substring(startIdx + 1, endIdx - startIdx - 1);


            string setFilePath = regFilePath;
            FileStream fs = new FileStream(setFilePath, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            byte[] a = br.ReadBytes((int)fs.Length);

            register.SubKey = subkey;
            register.Domain = register.GetRegDomainFromName(domain);
            register.RegeditKey = key;

            register.WriteRegeditKey(a);

            br.Close();
        }



        public static void CreateDir(string path)
        {
            List<string> dirList = new List<string>();
            string dirPath = path;
            int idx;

            for (;;)
            {
                idx = dirPath.LastIndexOf("\\");
                if (idx == -1)
                    break;

                dirPath = dirPath.Substring(0, idx);

                if (dirPath == null)
                    break;

                dirList.Add(dirPath);
            }

            for (int i = dirList.Count - 1; i >= 0; i--)
            {
                dirPath = dirList[i];
                if (!Directory.Exists(dirPath))//如果路径不存在
                    Directory.CreateDirectory(dirPath);//创建一个路径的文件夹
            }
        }
    }
}
