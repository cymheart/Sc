using System.Collections.Generic;
using System.IO;


namespace PrintUtils
{
    public class PrinterExSettingInfoOp
    {
        public string exSettingInfoPath;

        public string[] GetAllPrinterExSettingFileNameList()
        {
            List<string> exSettingFileList = new List<string>();
            DirectoryInfo folder = new DirectoryInfo(exSettingInfoPath);
            string name;

            foreach (FileInfo file in folder.GetFiles("*.bin"))
            {
                name = file.Name.Replace(".bin", "");
                exSettingFileList.Add(name);
            }

            return exSettingFileList.ToArray(); 
        }

        public string GetPrinterExSettingFilePath(string name)
        {
            List<string> exSettingFileList = new List<string>();
            DirectoryInfo folder = new DirectoryInfo(exSettingInfoPath);

            foreach (FileInfo file in folder.GetFiles("*.bin"))
            {
                if(file.Name == name + ".bin")
                {
                    return exSettingInfoPath + file.Name;
                }
            }

            return null;
        }


        public void ExportPrinterExSettingMode1(string strPrinter, string name)
        {
            string regEditPath = @"HKEY_CURRENT_USER\Printers\DevModePerUser\" + strPrinter;
            string path = exSettingInfoPath + name + ".bin";
            Utils.CreateRegDataToFile(regEditPath, path);
        }

        public void ExportPrinterExSettingMode2(string strPrinter, string setName, string name)
        {
            string rootDir = "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Print\\Printers\\";
            string regEditPath = rootDir + strPrinter + "\\PrinterDriverData\\Presets\\" + setName;
            string path = exSettingInfoPath + name + ".bin";

            Utils.CreateRegDataToFile(regEditPath, path);
        }

        public void ImportPrinterExSetting(string strPrinter, string name)
        {
            string regpath = "HKEY_CURRENT_USER\\Printers\\DevModePerUser\\" + strPrinter;
            string filepath = GetPrinterExSettingFilePath(name);

            byte[] b = Utils.CreateRegDataToBytes(regpath);
            if (b == null)
            {
                return; 
            }
            else
            {
                Utils.CreateRegDataToRegEdit(filepath, regpath);
            }
        }

    }
}
