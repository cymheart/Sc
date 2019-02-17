using API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace PrintUtils
{
    public partial class PrinterSetFrm : Form
    {
        Print print;
        PrintDocument printDocument;

        public delegate void PrintDelegate();
        public event PrintDelegate printEvent = null;

        public PrinterSetFrm(Print _print)
        {
            InitializeComponent();

            print = _print;
            printDocument = print.printDocument;
            GetPrinterNameList();
            numericUpDownCopies.Value = printDocument.PrinterSettings.Copies;
        }


        void GetPrinterNameList()
        {
            string strDefaultPrinter = printDocument.PrinterSettings.PrinterName;//获取默认的打印机名 

            //在列表框中列出所有的打印机
            foreach (String strPrinter in PrinterSettings.InstalledPrinters)
            {
                printerList.Items.Add(strPrinter);
                if (strPrinter == strDefaultPrinter)//把默认打印机设为缺省值 
                {
                    printerList.SelectedIndex = printerList.Items.IndexOf(strPrinter);
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string printerName = printerList.SelectedItem.ToString();
            printDocument.PrinterSettings.PrinterName = printerName;
            printDocument.PrinterSettings.Copies = short.Parse(numericUpDownCopies.Value.ToString());

            if (printEvent == null)
                printDocument.Print();

            else
                printEvent();
        }



        private void exportPrinterSetting_Click(object sender, EventArgs e)
        {
            CreatePrinterPreSettingFrm frm = new CreatePrinterPreSettingFrm(print.printerSettingDir, print);
            frm.ShowDialog();
        }


        //调用DocumentProperties
        private void documentPropButton_Click(object sender, EventArgs e)
        {
            string printerName = printerList.SelectedItem.ToString();
            PrinterSetting printerSetting = new PrinterSetting();
            printerSetting.ChangePrinterSetting(printerName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string printerName = printerList.SelectedItem.ToString();
            printDocument.PrinterSettings.PrinterName = printerName;
            printDocument.PrinterSettings.Copies = short.Parse(numericUpDownCopies.Value.ToString());
            print.printerDefaultSettingOp.SetDefaultXmlPrinter(printerName);
            Close();
        }
    }
}
