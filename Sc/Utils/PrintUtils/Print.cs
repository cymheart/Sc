using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace PrintUtils
{
    public class Print
    {
        public string defaultSetDir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        public string defaultSetFile = "defaultSet";

        public string printerSetDir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        public string printerSetFile = "printerSet";

        public string printerSettingDir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "PrintersPreSetting\\";

        public PrintDocument printDocument;
        public PrinterPreSettingOp printerPreSettingOp;
        public PrinterDefaultSettingOp printerDefaultSettingOp;

        public Print()
        {
            printDocument = new PrintDocument();

            printerPreSettingOp = new PrinterPreSettingOp(printerSetDir, printerSetFile);
            printerPreSettingOp.print = this;

            printerDefaultSettingOp = new PrinterDefaultSettingOp(defaultSetDir, defaultSetFile);

            printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            printDocument.OriginAtMargins = true;
        }


        public void SetPrintPageEvenHandler(PrintPageEventHandler handler)
        {
            printDocument.PrintPage += handler;
        }

        public void SetPrinterPreSetPath(string dir, string name)
        {
            printerSetDir = dir;
            printerSetFile = name;
            printerPreSettingOp.SetPath(printerSetDir, printerSetFile);
        }

        public void SetDefualtSetPath(string dir, string name)
        {
            defaultSetDir = dir;
            defaultSetFile = name;
            printerDefaultSettingOp.SetPath(defaultSetDir, defaultSetFile);
        }

        public void SetPrinterSettingDir(string dir)
        {
            printerSettingDir = dir;
        }

        public void SetPageSize(SizeF size)
        {
            int w = (int)Math.Round(size.Width / 0.254f);
            int h = (int)Math.Round(size.Height / 0.254f);
            printDocument.DefaultPageSettings.PaperSize = new PaperSize("Custum", w, h);
        }

        public void ShowPrintSetDialog()
        {
            PrinterSetFrm frm = new PrinterSetFrm(this);
            frm.ShowDialog();
        }

        public void ShowPageSetDialog()
        {
            PageSetupDialog pageSetupDialog = new PageSetupDialog();
            pageSetupDialog.Document = printDocument;
            pageSetupDialog.ShowDialog();
        }

        public void ShowPreviewDialog()
        {
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.ShowDialog();
        }

        public void ShowPrintPreSetDialog()
        {
            CreatePrinterPreSettingFrm frm = new CreatePrinterPreSettingFrm(printerSettingDir, this);
            frm.ShowDialog();
        }

        public string ResetDefaultPrinterBySettingFile()
        {
            string printerName = printerDefaultSettingOp.GetDefaultXmlPrinter();
            printDocument.PrinterSettings.PrinterName = printerName;
            return printerName;
        }


        public void PrintDocument()
        {
            try
            {
                printDocument.Print();
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message, "打印出错");
                printDocument.PrintController.OnEndPrint(printDocument, new PrintEventArgs());
            }
        }
    }
}
