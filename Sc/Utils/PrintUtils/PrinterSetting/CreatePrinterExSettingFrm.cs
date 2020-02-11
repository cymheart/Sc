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
    public partial class CreatePrinterExSettingFrm : Form
    {
        string exSettingFileDir;
        public string printerName = null;

        public CreatePrinterExSettingFrm(string exSettingFileDir)
        {
            if (!string.IsNullOrEmpty(exSettingFileDir))
                this.exSettingFileDir = exSettingFileDir;
            InitializeComponent();
            this.Opacity = 1;
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

        private void btnCreate_Click(object sender, EventArgs e)
        {
            //
            int ret = RegEditSettingExportMethod3();
            if (ret == 0)
            {
                MessageBox.Show("导出成功!");
                return;
            }
            if (ret != 2)
                return;

            //
            ret = RegEditSettingExportMethod1();
            if (ret == 0)
            {
                MessageBox.Show("导出成功!");
                return;
            }

            MessageBox.Show("此打印机不支持导出扩展配置文件!");
        }

        int RegEditSettingExportMethod1()
        {
            string path;
            string strPrinter = printerList.SelectedItem.ToString();

            if (string.IsNullOrEmpty(textBoxExportName.Text))
            {
                MessageBox.Show("输入导出的文件名称!");
                return 1;
            }

            string regEditPath = @"HKEY_CURRENT_USER\Printers\DevModePerUser\" + strPrinter;
            path = exSettingFileDir + textBoxExportName.Text + ".bin";

            bool ret = Utils.CreateRegDataToFile(regEditPath, path);
            if (ret == false)
                return 2;

            return 0;
        }

        int RegEditSettingExportMethod2()
        {
            string path;
            string strPrinter = printerList.SelectedItem.ToString();

            if (string.IsNullOrEmpty(textBoxExportName.Text))
            {
                MessageBox.Show("输入导出的文件名称!");
                return 1;
            }

            string regEditPath = @"HKEY_CURRENT_USER\Printers\DevModes2\" + strPrinter;
            path = exSettingFileDir + textBoxExportName.Text + ".bin";

            bool ret = Utils.CreateRegDataToFile(regEditPath, path);
            if (ret == false)
                return 2;

            return 0;
        }

        int RegEditSettingExportMethod3()
        {
            string strPrinter = printerList.SelectedItem.ToString();

            if (string.IsNullOrEmpty(textBoxExportName.Text))
            {
                MessageBox.Show("输入导出的文件名称!");
                return 1;
            }

            if (string.IsNullOrEmpty(textBoxPreSetName.Text))
            {
                MessageBox.Show("输入预设的打印配置的文件名称!");
                return 3;
            }

            string path = exSettingFileDir + textBoxExportName.Text + ".bin";
            Register register = new Register();
            register.SubKey = "SYSTEM\\CurrentControlSet\\Control\\Print\\Printers\\" + strPrinter + "\\PrinterDriverData\\Presets\\";
            register.Domain = register.GetRegDomainFromName("HKEY_LOCAL_MACHINE");

            string key = register.GetValueRegEditKey(textBoxPreSetName.Text);
            if (key == null)
                return 2;

            register.RegeditKey = key.Replace("Name", "DEVMODE");
     
            byte[] a = (byte[])(register.ReadRegeditKey());
            if (a == null)
                return 2;

            Utils.CreateDir(path);
            BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create));
            bw.Write(a);
            bw.Close();   

            return 0;
        }

        private void PopupBtn_Close_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
