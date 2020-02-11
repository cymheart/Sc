using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;

namespace PrintUtils
{
    public class PrinterSettingInfo
    {
        public string printModuleKey;
        public string printerName;
        public string pageName;
        public SizeF pageSize;
        public string exSettingFileName;
    }


    public class PrinterSettingInfoOp
    {
        public string printerSettingPath = null;

        public PrinterSettingInfo GetPrinterSettingInfo(string printModuleKey)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(printerSettingPath);//加载xml

            if (xmlDoc.ChildNodes.Count == 0)
                return null;

            List<PrintModuleInfo> printModuleInfoList = new List<PrintModuleInfo>();

            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Root"); //取得节点名为Root的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合
                PrinterSettingInfo newInfo;

                foreach (XmlNode cxmlNode in childList)
                {
                    if (cxmlNode.Attributes["PrintModuleKey"].Value == printModuleKey)
                    {
                        newInfo = new PrinterSettingInfo();
                        newInfo.printModuleKey = cxmlNode.Attributes["PrintModuleKey"].Value;
                        newInfo.printerName = cxmlNode.Attributes["PrinterName"].Value;
                        newInfo.pageSize.Width = float.Parse(cxmlNode.Attributes["PageSizeWidth"].Value);
                        newInfo.pageSize.Height = float.Parse(cxmlNode.Attributes["PageSizeHeight"].Value);
                        newInfo.pageName = cxmlNode.Attributes["PageName"].Value;
                        newInfo.exSettingFileName = cxmlNode.Attributes["ExSettingFileName"].Value;

                        return newInfo;
                    }
                }
            }

            return null;
        }


        public void AddPrinterSettingToFile(PrinterSettingInfo printerSettingInfo)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root;

            if (File.Exists(printerSettingPath))
            {
                xmlDoc.Load(printerSettingPath);//加载xml
            }
            else
            {
                XmlDeclaration xmldecl;
                xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);
                xmlDoc.AppendChild(xmldecl);

                //加入一个根元素     
                root = xmlDoc.CreateElement("Root");//创建一个<Root>节点 
                root.SetAttribute("name", "打印模块对应设置信息");//设置该节点genre属性 
                root.SetAttribute("ISBN", "2-3631-4");//设置该节点ISBN属性
                xmlDoc.AppendChild(root);

                XmlElement xesub1 = xmlDoc.CreateElement("Row");
                xesub1.SetAttribute("PrintModuleKey", printerSettingInfo.printModuleKey);//设置该节点打印模块关键字属性 
                xesub1.SetAttribute("PrinterName", printerSettingInfo.printerName);//设置该节点打印机名称属性 
                xesub1.SetAttribute("PageName", printerSettingInfo.pageName);
                xesub1.SetAttribute("PageSizeWidth", printerSettingInfo.pageSize.Width.ToString());//设置该节点打印机页面尺寸宽度属性 
                xesub1.SetAttribute("PageSizeHeight", printerSettingInfo.pageSize.Height.ToString());//设置该节点打印机页面尺寸高度属性        
                xesub1.SetAttribute("ExSettingFileName", printerSettingInfo.exSettingFileName);//设置该节点打印机扩展属性文件路径 
                root.AppendChild(xesub1);

                //保存创建好的XML文档
                Utils.CreateDir(printerSettingPath);
                xmlDoc.Save(printerSettingPath);
                return;
            }


            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Root"); //取得节点名为Root的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合

                foreach (XmlNode cxmlNode in childList)
                {
                    if (cxmlNode.Attributes["PrintModuleKey"].Value == printerSettingInfo.printModuleKey)
                    {
                        cxmlNode.Attributes["PrinterName"].Value = printerSettingInfo.printerName;//设置该节点打印机名称属性 

                        cxmlNode.Attributes["PageName"].Value = printerSettingInfo.pageName;
                        cxmlNode.Attributes["PageSizeWidth"].Value = printerSettingInfo.pageSize.Width.ToString();//设置该节点打印机页面尺寸宽度属性 
                        cxmlNode.Attributes["PageSizeHeight"].Value = printerSettingInfo.pageSize.Height.ToString();//设置该节点打印机页面尺寸高度属性        

                        cxmlNode.Attributes["ExSettingFileName"].Value = printerSettingInfo.exSettingFileName;//设置该节点打印机扩展属性文件路径 
                        xmlDoc.Save(printerSettingPath);
                        return;
                    }
                }
            }

            XmlNodeList list = xmlDoc.GetElementsByTagName("Root");//创建一个<Node>节点 
            XmlNode rootx = list[0];
            XmlElement xesub = xmlDoc.CreateElement("Row");

            xesub.SetAttribute("PrintModuleKey", printerSettingInfo.printModuleKey);//设置该节点打印模块关键字属性 
            xesub.SetAttribute("PrinterName", printerSettingInfo.printerName);//设置该节点打印机名称属性 
            xesub.SetAttribute("PageName", printerSettingInfo.pageName);
            xesub.SetAttribute("PageSizeWidth", printerSettingInfo.pageSize.Width.ToString());//设置该节点打印机页面尺寸宽度属性 
            xesub.SetAttribute("PageSizeHeight", printerSettingInfo.pageSize.Height.ToString());//设置该节点打印机页面尺寸高度属性        
            xesub.SetAttribute("ExSettingFileName", printerSettingInfo.exSettingFileName);//设置该节点打印机扩展属性文件路径 
            rootx.AppendChild(xesub);

            //保存创建好的XML文档
            xmlDoc.Save(printerSettingPath);
        }

        public void RemovePrinterSettingFromFile(string printModuleKey)
        {
            XmlDocument xmlDoc = new XmlDocument();

            if (!File.Exists(printerSettingPath))
                return;

            xmlDoc.Load(printerSettingPath);//加载xml

            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Root"); //取得节点名为Root的XmlNode集合
            foreach (XmlNode xmlNode in xmlList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得row节点集合

                foreach (XmlNode cxmlNode in childList)
                {
                    if (cxmlNode.Attributes["PrintModuleKey"].Value == printModuleKey)
                    {
                        xmlNode.RemoveChild(cxmlNode);
                        xmlDoc.Save(printerSettingPath);
                        return;
                    }
                }
            }
        }
    }
}
