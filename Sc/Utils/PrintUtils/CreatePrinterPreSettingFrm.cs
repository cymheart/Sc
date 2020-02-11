using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using API;
using System.Drawing.Printing;
using System.IO;

namespace PrintUtils
{
    public partial class CreatePrinterPreSettingFrm : Form
    {
        string rootDir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "PrintersPreSetting\\";
        string preSettingFileDir;
        Print print = null;
        public string printerName = null;

        public CreatePrinterPreSettingFrm(string preSettingFileDir, Print print)
        {
            if (!string.IsNullOrEmpty(preSettingFileDir))
                this.preSettingFileDir = preSettingFileDir;       

            this.print = print;

            InitializeComponent();
            GetPrinterNameList();
        }


        void GetPrinterNameList()
        {
            PrintDocument prtdoc = new PrintDocument();
            string strDefaultPrinter = prtdoc.PrinterSettings.PrinterName;//获取默认的打印机名 

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


        //调用DocumentProperties
        private void documentPropButton_Click(object sender, EventArgs e)
        {
            string printerName = printerList.SelectedItem.ToString();
            if (printerName != null && printerName.Length > 0)
            {
                IntPtr pPrinter = IntPtr.Zero;
                IntPtr pDevModeOutput = IntPtr.Zero;
                IntPtr pDevModeInput = IntPtr.Zero;
                IntPtr nullPointer = IntPtr.Zero;
                WindowsAPI.OpenPrinter(printerName, ref pPrinter, ref nullPointer);
                int iNeeded = WindowsAPI.DocumentProperties(this.Handle, pPrinter, printerName, ref pDevModeOutput, ref pDevModeInput, 0);
                pDevModeOutput = System.Runtime.InteropServices.Marshal.AllocHGlobal(iNeeded);
                WindowsAPI.DocumentProperties(this.Handle, pPrinter, printerName, ref pDevModeOutput, ref pDevModeInput, 4);
                WindowsAPI.ClosePrinter(pPrinter);
            }
        }


        // 调用PrinterProperties
        private void printPropButton_Click(object sender, EventArgs e)
        {
            string printerName = printerList.SelectedItem.ToString();
            if (printerName != null && printerName.Length > 0)
            {
                IntPtr pPrinter = IntPtr.Zero;
                IntPtr pDevModeOutput = IntPtr.Zero;
                IntPtr pDevModeInput = IntPtr.Zero;
                IntPtr nullPointer = IntPtr.Zero;
                WindowsAPI.OpenPrinter(printerName, ref pPrinter, ref nullPointer);
                int iNeeded = WindowsAPI.PrinterProperties(this.Handle, pPrinter);
                WindowsAPI.ClosePrinter(pPrinter);
            }
        }

        private void btnGetRegPath_Click(object sender, EventArgs e)
        {
            string printerName = printerList.SelectedItem.ToString();
            string dir = "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Print\\Printers\\";
            string text = dir + printerName + "\\PrinterDriverData\\Presets\\[改写为已设定的配置名称]";
            txtRegPath.Text = text;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string path = txtRegPath.Text;

            int startIdx = path.IndexOf("\\");
            string domain = path.Substring(0, startIdx);

            int endIdx = path.LastIndexOf("\\");
            string name = path.Substring(endIdx + 1);
            string subkey = path.Substring(startIdx + 1, endIdx - startIdx - 1);

            Register register = new Register();
            register.SubKey = subkey;
            register.Domain = register.GetRegDomainFromName(domain);
            string key = register.GetValueRegEditKey(name);

            if (key == null)
            {
                MessageBox.Show("导出配置文件失败!");
                return;
            }

            printerName = printerList.SelectedItem.ToString() + ".bin";
            string printerSettingFilePath = preSettingFileDir + printerName;
            Utils.CreateDir(printerSettingFilePath);

            register.RegeditKey = key.Replace("Name", "DEVMODE");
            byte[] a = (byte[])(register.ReadRegeditKey());
            BinaryWriter bw = new BinaryWriter(new FileStream(printerSettingFilePath, FileMode.Create));
            bw.Write(a);
            bw.Close();

            if(print!=null && print.printerPreSettingOp != null)
                print.printerPreSettingOp.AddPrinterPreSettingFile(printerName);

   
            MessageBox.Show("导出成功!");      
        }


        bool CreateRegDataToFile(string regDataPath, string saveFilePath)
        {
            string path = regDataPath;

            int startIdx = path.IndexOf("\\");
            string domain = path.Substring(0, startIdx);

            int endIdx = path.LastIndexOf("\\");
            string name = path.Substring(endIdx + 1);
            string subkey = path.Substring(startIdx + 1, endIdx - startIdx - 1);

            Register register = new Register();
            register.SubKey = subkey;
            register.Domain = register.GetRegDomainFromName(domain);
            register.RegeditKey = name;
  
            Utils.CreateDir(saveFilePath);

            byte[] a = (byte[])(register.ReadRegeditKey());
            BinaryWriter bw = new BinaryWriter(new FileStream(saveFilePath, FileMode.Create));
            bw.Write(a);
            bw.Close();

            return true;
        }

    }
}
