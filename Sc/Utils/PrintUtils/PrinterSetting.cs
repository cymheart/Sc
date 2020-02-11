using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace PrintUtils
{
    //written by fujie  
    public class PrinterSetting
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PRINTER_DEFAULTS
        {
            public int pDatatype;
            public int pDevMode;
            public int DesiredAccess;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct PRINTER_INFO_2
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pServerName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pPrinterName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pShareName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pPortName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDriverName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pComment;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pLocation;
            public IntPtr pDevMode;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pSepFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pPrintProcessor;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDatatype;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pParameters;
            public IntPtr pSecurityDescriptor;
            public Int32 Attributes;
            public Int32 Priority;
            public Int32 DefaultPriority;
            public Int32 StartTime;
            public Int32 UntilTime;
            public Int32 Status;
            public Int32 cJobs;
            public Int32 AveragePPM;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public short dmOrientation;
            public short dmPaperSize;
            public short dmPaperLength;
            public short dmPaperWidth;
            public short dmScale;
            public short dmCopies;
            public short dmDefaultSource;
            public short dmPrintQuality;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmUnusedPadding;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
        }
        #region ■变量_____________________________________________________________  
        private IntPtr hPrinter = new System.IntPtr();
        private PRINTER_DEFAULTS PrinterValues = new PRINTER_DEFAULTS();
        private PRINTER_INFO_2 pinfo = new PRINTER_INFO_2();
        private DEVMODE dm;
        private IntPtr ptrDM;
        private IntPtr ptrPrinterInfo;
        private int sizeOfDevMode = 0;
        private int lastError;
        private int nBytesNeeded;
        private long nRet;
        private int intError;
        private System.Int32 nJunk;
        private IntPtr yDevModeData;
        #endregion
        #region ■ＡＰＩ___________________________________________________________  
        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true,
          ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool ClosePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "DocumentPropertiesA", SetLastError = true,
          ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter,
         [MarshalAs(UnmanagedType.LPStr)] string pDeviceNameg,
         IntPtr pDevModeOutput, ref IntPtr pDevModeInput, int fMode);
        [DllImport("winspool.Drv", EntryPoint = "GetPrinterA", SetLastError = true, CharSet = CharSet.Ansi,
          ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool GetPrinter(IntPtr hPrinter, Int32 dwLevel,
         IntPtr pPrinter, Int32 dwBuf, out Int32 dwNeeded);
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi,
          ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter,
         out IntPtr hPrinter, ref PRINTER_DEFAULTS pd);
        [DllImport("winspool.drv", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool SetPrinter(IntPtr hPrinter, int Level, IntPtr
         pPrinter, int Command);
        #endregion
        #region ■常数_____________________________________________________________  
        private const int DM_OUT_BUFFER = 2;
        private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        private const int PRINTER_ACCESS_ADMINISTER = 0x4;
        private const int PRINTER_ACCESS_USE = 0x8;
        private const int PRINTER_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | PRINTER_ACCESS_ADMINISTER | PRINTER_ACCESS_USE);
        #endregion
        #region ■方法_____________________________________________________________  
        public void ChangePrinterSetting(string i_printerName)
        {
            DEVMODE dm;
            IntPtr pPrinter = IntPtr.Zero;
            IntPtr pDevModeOutput = IntPtr.Zero;
            IntPtr pDevModeInput = IntPtr.Zero;
            PrinterValues.pDatatype = 0;
            PrinterValues.pDevMode = 0;
            PrinterValues.DesiredAccess = PRINTER_ALL_ACCESS;
            OpenPrinter(i_printerName, out pPrinter, ref PrinterValues);
            int iNeeded = DocumentProperties(IntPtr.Zero, pPrinter, i_printerName, pDevModeOutput, ref pDevModeInput, 0);
            pDevModeOutput = Marshal.AllocHGlobal(iNeeded);
            int mode = 2 | 4;
            int nRet = DocumentProperties(IntPtr.Zero, pPrinter, i_printerName, pDevModeOutput, ref pDevModeInput, mode);
            if (nRet == 1)
            {
                dm = (DEVMODE)Marshal.PtrToStructure(pDevModeOutput, typeof(DEVMODE));
                ChangePrinterSetting(i_printerName, ref dm);
            }
            ClosePrinter(pPrinter);
        }
        private void ChangePrinterSetting(string i_printerName, ref DEVMODE i_fujie)
        {
            {
                dm = this.GetPrinterSettings(i_printerName);
                dm = i_fujie;
                Marshal.StructureToPtr(dm, yDevModeData, true);
                pinfo.pDevMode = yDevModeData;
                pinfo.pSecurityDescriptor = IntPtr.Zero;
                Marshal.StructureToPtr(pinfo, ptrPrinterInfo, false);
                lastError = Marshal.GetLastWin32Error();
                nRet = Convert.ToInt16(SetPrinter(hPrinter, 2, ptrPrinterInfo, 0));
                if (nRet == 0)
                {
                    lastError = Marshal.GetLastWin32Error();
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                if (hPrinter != IntPtr.Zero)
                    ClosePrinter(hPrinter);
            }
        }
        private DEVMODE GetPrinterSettings(string i_printerName)
        {
            DEVMODE dm;
            nRet = Convert.ToInt32(OpenPrinter(i_printerName, out hPrinter, ref PrinterValues));
            if (nRet == 0)
            {
                lastError = Marshal.GetLastWin32Error();
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            GetPrinter(hPrinter, 2, IntPtr.Zero, 0, out nBytesNeeded);
            if (nBytesNeeded <= 0)
            {
                throw new System.Exception("Unable to allocate memory by fujie");
            }
            else
            {
                ptrPrinterInfo = Marshal.AllocHGlobal(nBytesNeeded);
                nRet = Convert.ToInt32(GetPrinter(hPrinter, 2, ptrPrinterInfo, nBytesNeeded, out nJunk));
                if (nRet == 0)
                {
                    lastError = Marshal.GetLastWin32Error();
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                pinfo = (PRINTER_INFO_2)Marshal.PtrToStructure(ptrPrinterInfo, typeof(PRINTER_INFO_2));
                IntPtr Temp = new IntPtr();
                if (pinfo.pDevMode == IntPtr.Zero)
                {
                    IntPtr ptrZero = IntPtr.Zero;
                    sizeOfDevMode = DocumentProperties(IntPtr.Zero, hPrinter, i_printerName, ptrZero, ref ptrZero, 0);
                    ptrDM = Marshal.AllocCoTaskMem(sizeOfDevMode);
                    int i;
                    i = DocumentProperties(IntPtr.Zero, hPrinter, i_printerName, ptrDM, ref ptrZero, DM_OUT_BUFFER);
                    if ((i < 0) || (ptrDM == IntPtr.Zero))
                    {
                        throw new System.Exception("fujie:Cannot get DEVMODE data");
                    }
                    pinfo.pDevMode = ptrDM;
                }
                intError = DocumentProperties(IntPtr.Zero, hPrinter, i_printerName, IntPtr.Zero, ref Temp, 0);
                yDevModeData = Marshal.AllocHGlobal(intError);
                intError = DocumentProperties(IntPtr.Zero, hPrinter, i_printerName, yDevModeData, ref Temp, 2);
                dm = (DEVMODE)Marshal.PtrToStructure(yDevModeData, typeof(DEVMODE));
                if ((nRet == 0) || (hPrinter == IntPtr.Zero))
                {
                    lastError = Marshal.GetLastWin32Error();
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                return dm;
            }
        }

        #endregion
    }
}

