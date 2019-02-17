using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PrintUtils
{
    public class PrintModuleInfo
    {
        public string key;
        public string name;
    }

    public class PrintModuleInfoOp
    {
        public string printModuleSetPath;

        public PrintModuleInfo[] GetAllPrintModuleInfo()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(printModuleSetPath);//加载xml

            if (xmlDoc.ChildNodes.Count == 0)
                return null;

            List<PrintModuleInfo> printModuleInfoList = new List<PrintModuleInfo>();

            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Root"); //取得节点名为Root的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合
                PrintModuleInfo newInfo;

                foreach (XmlNode cxmlNode in childList)
                {
                    newInfo = new PrintModuleInfo();
                    newInfo.key = cxmlNode.Attributes["key"].Value.ToString();
                    newInfo.name = cxmlNode.Attributes["name"].Value.ToString();
                    printModuleInfoList.Add(newInfo);
                }
            }

            return printModuleInfoList.ToArray();
        }


        public void AddPrintModuleToFile(PrintModuleInfo printModuleInfo)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root;

            if (File.Exists(printModuleSetPath))
            {
                xmlDoc.Load(printModuleSetPath);//加载xml
            }
            else
            {
                XmlDeclaration xmldecl;
                xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);
                xmlDoc.AppendChild(xmldecl);

                //加入一个根元素     
                root = xmlDoc.CreateElement("Root");//创建一个<Root>节点 
                root.SetAttribute("name", "打印模块信息设置");//设置该节点genre属性 
                root.SetAttribute("ISBN", "2-3631-4");//设置该节点ISBN属性
                xmlDoc.AppendChild(root);

                XmlElement xesub1 = xmlDoc.CreateElement("Row");
                xesub1.SetAttribute("key", printModuleInfo.key);//设置该节点key属性 
                xesub1.SetAttribute("name", printModuleInfo.name);//设置该节点name属性 
                root.AppendChild(xesub1);

                //保存创建好的XML文档
                Utils.CreateDir(printModuleSetPath);
                xmlDoc.Save(printModuleSetPath);
                return;
            }


            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Root"); //取得节点名为Root的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合

                foreach (XmlNode cxmlNode in childList)
                {
                    if (cxmlNode.Attributes["key"].Value == printModuleInfo.key)
                    {
                        cxmlNode.Attributes["name"].Value = printModuleInfo.name;
                        xmlDoc.Save(printModuleSetPath);
                        return;
                    }
                }
            }

            XmlNodeList list = xmlDoc.GetElementsByTagName("Root");//创建一个<Node>节点 
            XmlNode rootx = list[0];
            XmlElement xesub = xmlDoc.CreateElement("Row");

            xesub.SetAttribute("key", printModuleInfo.key);//设置该节点key属性 
            xesub.SetAttribute("name", printModuleInfo.name);//设置该节点name属性 
            rootx.AppendChild(xesub);

            //保存创建好的XML文档
            xmlDoc.Save(printModuleSetPath);
        }

        public void RemovePrintModuleFromFile(string key)
        {
            XmlDocument xmlDoc = new XmlDocument();

            if (!File.Exists(printModuleSetPath))
                return;

            xmlDoc.Load(printModuleSetPath);//加载xml

            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Root"); //取得节点名为Root的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合

                foreach (XmlNode cxmlNode in childList)
                {
                    if (cxmlNode.Attributes["key"].Value == key)
                    {
                        xmlNode.RemoveChild(cxmlNode);
                        xmlDoc.Save(printModuleSetPath);
                        return;
                    }
                }
            }
        }
    }
}
