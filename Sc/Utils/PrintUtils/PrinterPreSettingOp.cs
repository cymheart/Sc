using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PrintUtils
{
    public class PrinterPreSettingOp
    {
        string dir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        string fileName = "printerSet";
        string printerSetPath;
        public Print print;

        public PrinterPreSettingOp(string dir, string fileName)
        {
            SetPath(dir, fileName);
        }

        public void SetPath(string dir, string fileName)
        {
            if (!string.IsNullOrEmpty(dir))
                this.dir = dir;

            if (!string.IsNullOrEmpty(fileName))
                this.fileName = fileName;

            printerSetPath = dir + fileName;
        }

        public void AddPrinterPreSettingFile(string fileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root;

            if (File.Exists(printerSetPath))
            {
                xmlDoc.Load(printerSetPath);//加载xml
            }
            else
            {
                XmlDeclaration xmldecl;
                xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);
                xmlDoc.AppendChild(xmldecl);

                //加入一个根元素     
                root = xmlDoc.CreateElement("Node");//创建一个<Node>节点 
                root.SetAttribute("name", "默认设置");//设置该节点genre属性 
                root.SetAttribute("ISBN", "2-3631-4");//设置该节点ISBN属性
                xmlDoc.AppendChild(root);

                XmlElement xesub1 = xmlDoc.CreateElement("Row");
                xesub1.SetAttribute("RegeditKey", "");//设置该节点name属性 
                xesub1.SetAttribute("Domain", "");//设置该节点name属性 
                xesub1.SetAttribute("SubKey", "");//设置该节点name属性 
                xesub1.InnerText = fileName;
                root.AppendChild(xesub1);

                //保存创建好的XML文档
                Utils.CreateDir(printerSetPath);
                xmlDoc.Save(printerSetPath);
                return;
            }


            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Node"); //取得节点名为Node的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合

                foreach (XmlNode cxmlNode in childList)
                {
                    if (cxmlNode.InnerText.Equals(fileName))
                        return;
                }
            }

            XmlNodeList list = xmlDoc.GetElementsByTagName("Node");//创建一个<Node>节点 
            XmlNode rootx = list[0];
            XmlElement xesub = xmlDoc.CreateElement("Row");

            xesub.SetAttribute("RegeditKey", "");//设置该节点name属性 
            xesub.SetAttribute("Domain", "");//设置该节点name属性 
            xesub.SetAttribute("SubKey", "");//设置该节点name属性 
            xesub.InnerText = fileName;
            rootx.AppendChild(xesub);

            //保存创建好的XML文档
            xmlDoc.Save(printerSetPath);

        }

        public PrinterPreSettingInfo[] ParseFile(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);//加载xml

            if (xmlDoc.ChildNodes.Count == 0)
                return null;

            List<PrinterPreSettingInfo> printerPreSettingInfoList = new List<PrinterPreSettingInfo>();

            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Node"); //取得节点名为Node的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合
                PrinterPreSettingInfo newInfo;

                foreach (XmlNode cxmlNode in childList)
                {
                    newInfo = new PrinterPreSettingInfo();

                    newInfo.RegeditKey = cxmlNode.Attributes["RegeditKey"].Value.ToString();
                    newInfo.DomainName = cxmlNode.Attributes["Domain"].Value.ToString();
                    newInfo.SubKey = cxmlNode.Attributes["SubKey"].Value.ToString();
                    newInfo.FilePath = cxmlNode.InnerText;

                    printerPreSettingInfoList.Add(newInfo);
                }
            }

            return printerPreSettingInfoList.ToArray();
        }

        public void ImportPrintersPreSettingFile()
        {
            PrinterPreSettingInfo[] printerPreSettingInfos = ParseFile(printerSetPath);

            Register register = new Register();

            foreach (PrinterPreSettingInfo info in printerPreSettingInfos)
            {
                string setFilePath = print.printerSettingDir + info.FilePath;
                FileStream fs = new FileStream(setFilePath, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                byte[] a = br.ReadBytes((int)fs.Length);

                register.SubKey = info.SubKey;
                register.Domain = register.GetRegDomainFromName(info.DomainName);
                register.RegeditKey = info.RegeditKey;

                int startIdx = info.FilePath.LastIndexOf('\\');
                int endIdx = info.FilePath.LastIndexOf('.');
                string printerName = info.FilePath.Substring(startIdx + 1, endIdx - startIdx - 1);

                if (string.IsNullOrEmpty(info.SubKey))
                {
                    register.SubKey = "Printers\\DevModePerUser";
                }

                if (string.IsNullOrEmpty(info.DomainName))
                {
                    register.Domain = RegDomain.CurrentUser;
                }

                if (string.IsNullOrEmpty(info.RegeditKey))
                {
                    register.RegeditKey = printerName;
                }


                register.WriteRegeditKey(a);

                br.Close();
            }
        }
    }
}
