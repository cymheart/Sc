using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace PrintUtils
{
    public class PrinterDocumentPreSettingOp
    {
        public PrintDocument printDocument;

        public void LoadPreSettingToPrintDocument(string printModuleKey)
        {
            string rootdir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            PrinterSettingInfoOp printerSettingInfoOp = new PrinterSettingInfoOp();
            printerSettingInfoOp.printerSettingPath = rootdir + "PrintersPreSetting\\PrinterSetting.xml";
            PrinterSettingInfo settingInfo = printerSettingInfoOp.GetPrinterSettingInfo(printModuleKey);

            if (settingInfo == null)
                return;

           
            PrinterExSettingInfoOp printerExSettingInfoOp = new PrinterExSettingInfoOp();
            printerExSettingInfoOp.exSettingInfoPath = rootdir + "PrintersPreSetting\\";
            printerExSettingInfoOp.ImportPrinterExSetting(settingInfo.printerName, settingInfo.exSettingFileName);

            printDocument.PrinterSettings.PrinterName = settingInfo.printerName;

            if (settingInfo.pageName == "Custom" &&
                settingInfo.pageSize.Width > 0 &&
                settingInfo.pageSize.Height > 0)
            {
                int w = (int)Math.Round(settingInfo.pageSize.Width / 0.254f);
                int h = (int)Math.Round(settingInfo.pageSize.Height / 0.254f);
                printDocument.DefaultPageSettings.PaperSize = new PaperSize("Custom", w, h);
            }
            else
            {
                PaperSize pkSize;
                for (int i = 0; i < printDocument.PrinterSettings.PaperSizes.Count; i++)
                {
                    pkSize = printDocument.PrinterSettings.PaperSizes[i];

                    if (pkSize.PaperName == settingInfo.pageName)
                    {
                        printDocument.DefaultPageSettings.PaperSize = pkSize;
                        break;
                    }
                }
            }
        }



        public void LoadPreSettingToPrintDocument(PrinterSettingInfo settingInfo, string exSettingInfoPath)
        {
            PrinterExSettingInfoOp printerExSettingInfoOp = new PrinterExSettingInfoOp();
            printerExSettingInfoOp.exSettingInfoPath = exSettingInfoPath;

            printerExSettingInfoOp.ImportPrinterExSetting(settingInfo.printerName, settingInfo.exSettingFileName);

     
            if (settingInfo.pageName == "Custom" &&
                settingInfo.pageSize.Width > 0 &&
                settingInfo.pageSize.Height > 0)
            {
                int w = (int)Math.Round(settingInfo.pageSize.Width / 0.254f);
                int h = (int)Math.Round(settingInfo.pageSize.Height / 0.254f);
                printDocument.DefaultPageSettings.PaperSize = new PaperSize("Custom", w, h);
            }
            else
            {
                PaperSize pkSize;
                for (int i = 0; i < printDocument.PrinterSettings.PaperSizes.Count; i++)
                {
                    pkSize = printDocument.PrinterSettings.PaperSizes[i];

                    if (pkSize.PaperName == settingInfo.pageName)
                    {
                        printDocument.DefaultPageSettings.PaperSize = pkSize;
                        break;
                    }
                }
            }
        }
    }
}
