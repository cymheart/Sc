using System.Drawing;
using System.IO;
using System.Xml;

namespace PrintUtils
{
    public class PrinterPreSettingInfo
    {
        public string SubKey = null;
        public string DomainName = null;
        public string RegeditKey = null;
        public string FilePath = null;
    }
    public class PrinterDefaultSettingOp
    {
        string dir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        string fileName = "defaultSet";
        string defaultSetPath;

        public PrinterDefaultSettingOp(string dir, string fileName)
        {
            SetPath(dir, fileName);
        }

        public void SetPath(string dir, string fileName)
        {
            if (!string.IsNullOrEmpty(dir))
                this.dir = dir;

            if (!string.IsNullOrEmpty(fileName))
                this.fileName = fileName;

            defaultSetPath = dir + fileName;
        }


        public void SetDefaultXmlSize(SizeF defaultSize)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root;

            if (File.Exists(defaultSetPath))
            {
                xmlDoc.Load(defaultSetPath);//加载xml
            }
            else
            {
                XmlDeclaration xmldecl;
                xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);
                xmlDoc.AppendChild(xmldecl);

                //加入一个根元素     
                root = xmlDoc.CreateElement("Node");//创建一个<Node>节点 
                root.SetAttribute("name", "默认设置文件路径");//设置该节点genre属性 
                root.SetAttribute("ISBN", "2-3631-4");//设置该节点ISBN属性
                xmlDoc.AppendChild(root);

                XmlElement xesub1 = xmlDoc.CreateElement("Row");
                xesub1.SetAttribute("name", "尺寸");//设置该节点name属性 
                xesub1.SetAttribute("width", defaultSize.Width.ToString());//设置该节点name属性 
                xesub1.SetAttribute("height", defaultSize.Height.ToString());//设置该节点name属性 
                root.AppendChild(xesub1);

                //保存创建好的XML文档
                Utils.CreateDir(defaultSetPath);
                xmlDoc.Save(defaultSetPath);
                return;
            }


            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Node"); //取得节点名为Node的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合

                foreach (XmlNode cxmlNode in childList)
                {
                    if (cxmlNode.Attributes["name"].Value == "尺寸")
                    {
                                      cxmlNode.Attributes["width"].Value = defaultSize.Width.ToString();
                        cxmlNode.Attributes["height"].Value = defaultSize.Height.ToString();

                        //保存创建好的XML文档
                        xmlDoc.Save(defaultSetPath);
                        return;
                    }
                }
            }

            XmlNodeList list = xmlDoc.GetElementsByTagName("Node");//创建一个<Node>节点 
            XmlNode rootx = list[0];
            XmlElement xesub = xmlDoc.CreateElement("Row");
            xesub.SetAttribute("name", "尺寸");//设置该节点name属性 
            xesub.SetAttribute("width", defaultSize.Width.ToString());//设置该节点name属性 
            xesub.SetAttribute("height", defaultSize.Height.ToString());//设置该节点name属性 
            rootx.AppendChild(xesub);

            //保存创建好的XML文档
            xmlDoc.Save(defaultSetPath);

        }

        public void SetDefaultXmlFile(string defaultSetFilePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
          
            if (File.Exists(defaultSetPath))
            {
                xmlDoc.Load(defaultSetPath);//加载xml
            }
            else
            {
                XmlDeclaration xmldecl;
                xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);
                xmlDoc.AppendChild(xmldecl);

                //加入一个根元素     
                XmlElement root = xmlDoc.CreateElement("Node");//创建一个<Node>节点 
                root.SetAttribute("name", "默认设置");//设置该节点genre属性 
                root.SetAttribute("ISBN", "2-3631-4");//设置该节点ISBN属性
                xmlDoc.AppendChild(root);

                XmlElement xesub1 = xmlDoc.CreateElement("Row");
                xesub1.SetAttribute("name", "默认内容文件路径");//设置该节点genre属性 
                xesub1.InnerText = defaultSetFilePath;
                root.AppendChild(xesub1);

                //保存创建好的XML文档
                Utils.CreateDir(defaultSetPath);
                xmlDoc.Save(defaultSetPath);
                return;
            }


            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Node"); //取得节点名为Node的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合

                foreach (XmlNode cxmlNode in childList)
                {
                    if (cxmlNode.Attributes["name"].Value == "默认内容文件路径")
                    {
                        cxmlNode.InnerText = defaultSetFilePath;

                        //保存创建好的XML文档
                        xmlDoc.Save(defaultSetPath);

                        return;
                    }
                }
            }


            XmlNodeList list = xmlDoc.GetElementsByTagName("Node");//创建一个<Node>节点 
            XmlNode rootx = list[0];
            XmlElement xesub = xmlDoc.CreateElement("Row");
            xesub.SetAttribute("name", "默认内容文件路径");//设置该节点name属性 
            xesub.InnerText = defaultSetFilePath;
            rootx.AppendChild(xesub);

            //保存创建好的XML文档
            xmlDoc.Save(defaultSetPath);
        }


        public void SetDefaultXmlPrinter(string defaultPrinter)
        {
            XmlDocument xmlDoc = new XmlDocument();
 
            if (File.Exists(defaultSetPath))
            {
                xmlDoc.Load(defaultSetPath);//加载xml
            }
            else
            {
                XmlDeclaration xmldecl;
                xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);
                xmlDoc.AppendChild(xmldecl);

                //加入一个根元素     
                XmlElement root = xmlDoc.CreateElement("Node");//创建一个<Node>节点 
                root.SetAttribute("name", "默认设置");//设置该节点genre属性 
                root.SetAttribute("ISBN", "2-3631-4");//设置该节点ISBN属性
                xmlDoc.AppendChild(root);

                XmlElement xesub1 = xmlDoc.CreateElement("Row");
                xesub1.SetAttribute("name", "默认打印机");//设置该节点genre属性 
                xesub1.InnerText = defaultPrinter;
                root.AppendChild(xesub1);

                //保存创建好的XML文档
                Utils.CreateDir(defaultSetPath);
                xmlDoc.Save(defaultSetPath);
                return;
            }


            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Node"); //取得节点名为Node的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合

                foreach (XmlNode cxmlNode in childList)
                {
                    if (cxmlNode.Attributes["name"].Value == "默认打印机")
                    {
                        cxmlNode.InnerText = defaultPrinter;

                        //保存创建好的XML文档
                        xmlDoc.Save(defaultSetPath);

                        return;
                    }
                }
            }


            XmlNodeList list = xmlDoc.GetElementsByTagName("Node");//创建一个<Node>节点 
            XmlNode rootx = list[0];
            XmlElement xesub = xmlDoc.CreateElement("Row");
            xesub.SetAttribute("name", "默认打印机");//设置该节点name属性 
            xesub.InnerText = defaultPrinter;
            rootx.AppendChild(xesub);

            //保存创建好的XML文档
            xmlDoc.Save(defaultSetPath);
        }

        public string GetDefaultXmlPrinter()
        {
            XmlDocument xmlDoc = new XmlDocument();
  
            if (!File.Exists(defaultSetPath))
                return null;

            xmlDoc.Load(defaultSetPath);//加载xml

            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Node"); //取得节点名为Node的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合

                foreach (XmlNode cxmlNode in childList)
                {
                    if (cxmlNode.Attributes["name"].Value == "默认打印机")
                    {
                        string printerName = cxmlNode.InnerText;
                        return printerName;
                    }
                }
            }

            return null;
        }


        public string GetDefaultXmlFile()
        {
            XmlDocument xmlDoc = new XmlDocument();

            if (!File.Exists(defaultSetPath))
                return null;

            xmlDoc.Load(defaultSetPath);//加载xml

            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Node"); //取得节点名为Node的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合

                foreach (XmlNode cxmlNode in childList)
                {
                    if (cxmlNode.Attributes["name"].Value == "默认内容文件路径")
                    {
                        return cxmlNode.InnerText;
                    }
                }
            }

            return null;
        }

        public SizeF GetDefaultXmlSize()
        {
            XmlDocument xmlDoc = new XmlDocument();
            SizeF size = new SizeF(60f, 190f);

            if (!File.Exists(defaultSetPath))
                return size;

            xmlDoc.Load(defaultSetPath);//加载xml

            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Node"); //取得节点名为Node的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合

                foreach (XmlNode cxmlNode in childList)
                {
                    if (cxmlNode.Attributes["name"].Value == "尺寸")
                    {
                        size.Width = float.Parse(cxmlNode.Attributes["width"].Value);
                        size.Height = float.Parse(cxmlNode.Attributes["height"].Value);
                    }
                }
            }

            return size;
        } 
    }
}
