using System;
using System.Text;
using API;
using System.Windows.Forms;

namespace Utils
{
    public class MonitorInfo
    {
        public float mmWidth { get; set; }
        public float mmHeight { get; set; }
        public float permmWidth { get; set; }
        public float permmHeight { get; set; }

        public MonitorInfo()
        {
            float _mmWidth = 0;
            float _mmHeight = 0;
            XDD_GetActiveMonitorPhysicalSize(ref _mmWidth, ref _mmHeight);

            mmWidth = _mmWidth;
            mmHeight = _mmHeight;

            int pxHeight = Screen.PrimaryScreen.Bounds.Height;
            int pxWidth = Screen.PrimaryScreen.Bounds.Width;


            permmWidth = pxWidth / mmWidth;
            permmHeight = pxHeight / mmHeight;
        }

       public bool  XDD_GetActiveAttachedMonitor(  
            ref DISPLAY_DEVICE ddMonitor                // 输出ddMonitor信息    
            )   
       {

           // 枚举Adapter下Monitor用变量  
           int dwMonitorIndex = 0;
           DISPLAY_DEVICE ddMonEmpty = new DISPLAY_DEVICE();
           DISPLAY_DEVICE ddMonTmp = new DISPLAY_DEVICE();

           // 枚举Adapter  
           int dwAdapterIndex = 0;
           DISPLAY_DEVICE ddAdapter = new DISPLAY_DEVICE();
           ddAdapter.cb = System.Runtime.InteropServices.Marshal.SizeOf(typeof(DISPLAY_DEVICE));

           while (WindowsAPI.EnumDisplayDevices(null, dwAdapterIndex, ref ddAdapter, 0) != false)
           {
               // 枚举该Adapter下的Monitor    
               dwMonitorIndex = 0;
               ddMonTmp = ddMonEmpty;
               ddMonTmp.cb = System.Runtime.InteropServices.Marshal.SizeOf(typeof(DISPLAY_DEVICE));

               while (WindowsAPI.EnumDisplayDevices(ddAdapter.DeviceName, dwMonitorIndex, ref ddMonTmp, 0) != false)
               {
                   // 判断状态是否正确  
                   if ((ddMonTmp.StateFlags & CommonConst.DISPLAY_DEVICE_ACTIVE) == CommonConst.DISPLAY_DEVICE_ACTIVE
                       && (ddMonTmp.StateFlags & CommonConst.DISPLAY_DEVICE_ATTACHED) == CommonConst.DISPLAY_DEVICE_ATTACHED
                       )
                   {
                       ddMonitor = ddMonTmp;
                       return true;
                   }

                   // 下一个Monitor  
                   dwMonitorIndex += 1;
                   ddMonTmp = ddMonEmpty;
                   ddMonTmp.cb = System.Runtime.InteropServices.Marshal.SizeOf(typeof(DISPLAY_DEVICE));
               }

               // 下一个Adapter  
               dwAdapterIndex += 1;
               ddAdapter = ddMonEmpty;
               ddAdapter.cb = System.Runtime.InteropServices.Marshal.SizeOf(typeof(DISPLAY_DEVICE));
           }  

           // 未枚举到满足条件的Monitor  
           return false;   
       }  


        // 解析DeviceID得到LEN0028以及{4d36e96e-e325-11ce-bfc1-08002be10318}\0001  
        // DeviceID:MONITOR\LEN0028\{4d36e96e-e325-11ce-bfc1-08002be10318}\0001  
        public bool XDD_GetModelDriverFromDeviceID(  
            string lpDeviceID,                      // DeviceID  
            ref StringBuilder strModel,                 // 输出型号，比如LEN0028  
            ref StringBuilder strDriver                 // 输出驱动信息，比如{4d36e96e-e325-11ce-bfc1-08002be10318}\0001  
            )  
        {  
            // 初始化输出参数  
            strModel = new StringBuilder("");
            strDriver = new StringBuilder("");
  
 
            // 参数有效性  
            if (lpDeviceID == null)  
            {  
                return false;  
            }

            // 查找第一个斜杠后的开始位置  
            int iBegin = lpDeviceID.IndexOf("\\");
            if (iBegin < -1)
            {
                return false;
            }

            iBegin += 1;

            // 查找开始后的第一个斜杠  
            int iSlash = lpDeviceID.IndexOf("\\", iBegin);
            if (iSlash < -1)
            {
                return false;
            }

            strModel.Append(lpDeviceID.Substring(iBegin, iSlash - iBegin));
            strDriver.Append(lpDeviceID.Substring(iSlash + 1));
   
            // 解析成功  
            return true;  
        }
  
        // 根据Model判断EDID数据是否正确  
        bool  XDD_IsCorrectEDID(  
            byte [] pEDIDBuf,                    // EDID数据缓冲区   
            int dwcbBufSize,                    // 数据字节大小  
            string lpModel                         // 型号   
            )  
        {  
   
            // 参数有效性  
            if (pEDIDBuf == null || dwcbBufSize<24 || lpModel == null)  
            {  
                return false;  
            }  

            // 判断EDID头  
            if ( pEDIDBuf[0] != 0x00 ||
                pEDIDBuf[1] != 0xFF ||
                pEDIDBuf[2] != 0xFF || 
                pEDIDBuf[3] != 0xFF ||
                pEDIDBuf[4] != 0xFF || 
                pEDIDBuf[5] != 0xFF ||
                pEDIDBuf[6] != 0xFF || 
                pEDIDBuf[7] != 0x00)  
            {  
                return false;   
            }  
  
            // 厂商名称 2个字节 可表三个大写英文字母  
            // 每个字母有5位 共15位不足一位 在第一个字母代码最高位补 0” 字母 A”至 Z”对应的代码为00001至11010  
            // 例如 MAG”三个字母 M代码为01101 A代码为00001 G代码为00111 在M代码前补0为001101   
            // 自左向右排列得2字节 001101 00001 00111 转化为十六进制数即为34 27   
            int dwPos = 8;      
            byte byte1 = pEDIDBuf[dwPos];  
            byte byte2 = pEDIDBuf[dwPos+1];  
            StringBuilder wcModelBuf = new StringBuilder();

            wcModelBuf.Append((char)(((byte1 & 0x7C) >> 2) + 64));
            wcModelBuf.Append((char)(((byte1 & 3) << 3) + ((byte2 & 0xE0) >> 5) + 64));
            wcModelBuf.Append((char)((byte2 & 0x1F) + 64));

            byte byte3 = pEDIDBuf[dwPos + 3];

            int v = (byte3 & 0xf0) >> 4;
            wcModelBuf.Append(v.ToString("X"));

            v = byte3 & 0xf;
            wcModelBuf.Append(v.ToString("X"));

            byte byte4 = pEDIDBuf[dwPos + 2];  

            v = (byte4 & 0xf0) >> 4;
            wcModelBuf.Append(v.ToString("X"));

            v = (byte4 & 0x0f);
            wcModelBuf.Append(v.ToString("X"));


            // 比较MODEL是否匹配  
            if (wcModelBuf.ToString() == lpModel)
                return true;

            return false;
        }
  

        // 根据Model及Driver信息取得EDID数据  
        public bool XDD_GetDeviceEDID(  
            string lpModel,                         // 型号  
            string lpDriver,                        // Driver  
            ref byte []pDataBuf,                    // 输出EDID数据缓冲区  
            int dwcbBufSize,                        // 输出缓冲区字节大小，不可小于256  
            ref int pdwGetBytes                     // 实际获得字节数  
            )  
        {  
   
            // 初始化输出参数    
            pdwGetBytes = 0;  
          
            // 参数有效性  
            if (   lpModel == null ||
                lpDriver == null || 
                pDataBuf == null || 
                dwcbBufSize == 0  
                )  
            {  
                return false;  
            }  
  
            // 打开设备注册表子键  
            StringBuilder wcSubKey = new StringBuilder("SYSTEM\\CurrentControlSet\\Enum\\DISPLAY\\");
            wcSubKey.Append(lpModel);

            UIntPtr hSubKey;  
   
            if(WindowsAPI.RegOpenKeyEx(CommonConst.HKEY_LOCAL_MACHINE, wcSubKey.ToString(), 0, CommonConst.KEY_READ,out hSubKey) != CommonConst.ERROR_SUCCESS)
            {  
                return false;  
            }  
  
            // 存放EDID数据  
            bool bGetEDIDSuccess = false;  
            byte [] EDIDBuf = new byte [256];  
            int dwEDIDSize = sizeof(byte) * 256;

            byte[] lpData = new byte[2048];
 
            // 枚举该子键下的键  
            int dwIndex = 0;  
            int dwSubKeyLen = wcSubKey.Length;

            IntPtr Null = (IntPtr)null;
            FILETIME ft = new FILETIME();

     
            while(bGetEDIDSuccess == false  &&  
                 WindowsAPI.RegEnumKeyEx(hSubKey, dwIndex, wcSubKey, ref dwSubKeyLen, Null, Null, Null, ref ft) == CommonConst.ERROR_SUCCESS )               
           {  
              
                // 打开枚举到的键  
                UIntPtr hEnumKey;  
       
                if (WindowsAPI.RegOpenKeyEx(hSubKey, wcSubKey.ToString(), 0, CommonConst.KEY_READ, out hEnumKey) == CommonConst.ERROR_SUCCESS)  
                {  
                    // 打开的键下查询Driver键的值  
                    dwSubKeyLen = 2048;  
                    uint type = 0;

                    if (WindowsAPI.RegQueryValueEx(hEnumKey, "Driver", 0, ref type, lpData, ref dwSubKeyLen) == CommonConst.ERROR_SUCCESS ) 
                    {  
                        string s = System.Text.Encoding.Default.GetString ( lpData,0,dwSubKeyLen - 1);

                        if (s == lpDriver) // Driver匹配 
                        {
                            // 打开键Device Parameters  
                            UIntPtr hDevParaKey;

                            if (WindowsAPI.RegOpenKeyEx(hEnumKey, "Device Parameters", 0, CommonConst.KEY_READ, out hDevParaKey) == CommonConst.ERROR_SUCCESS)
                            {

                                // 读取EDID  
                                Array.Clear(EDIDBuf, 0, EDIDBuf.Length);
                                dwEDIDSize = sizeof(byte) * 256;

                                if (WindowsAPI.RegQueryValueEx(hDevParaKey, "EDID", 0, ref type, EDIDBuf, ref dwEDIDSize) == CommonConst.ERROR_SUCCESS &&
                                    XDD_IsCorrectEDID(EDIDBuf, dwEDIDSize, lpModel) == true // 正确的EDID数据  
                                    )
                                {
                                    // 得到输出参数  
                                    pdwGetBytes = Math.Min(dwEDIDSize, dwcbBufSize);
                                    pDataBuf = EDIDBuf;
  
                                    // 成功获取EDID数据  
                                    bGetEDIDSuccess = true;
                                }


                                // 关闭键Device Parameters  
                                WindowsAPI.RegCloseKey(hDevParaKey);
                            }
                        }
                    }  
  
                    // 关闭枚举到的键  
                    WindowsAPI.RegCloseKey(hEnumKey);  
                }         
  
                // 下一个子键  
                dwIndex += 1; 
            }  
  
            // 关闭设备注册表子键  
            WindowsAPI.RegCloseKey(hSubKey);  
  
            // 返回获取EDID数据结果  
            return bGetEDIDSuccess; 
        }  


        // 获取当前Monitor的物理尺寸，单位CM  
        public bool  XDD_GetActiveMonitorPhysicalSize(  
            ref float dwWidth,                          // 输出宽度，单位CM  
            ref float dwHeight                          // 输出高度，单位CM  
            )  
        {  
    
            // 初始化输出参数  
            dwWidth = 0;  
            dwHeight = 0;  
  
            // 取得当前Monitor的DISPLAY_DEVICE数据  
            DISPLAY_DEVICE ddMonitor = new DISPLAY_DEVICE();  
   
            if (XDD_GetActiveAttachedMonitor(ref ddMonitor) == false)  
            {  
                return false;  
            }  
  
            // 解析DeviceID得到Model和Driver  

            StringBuilder strModel = new StringBuilder();
            StringBuilder strDriver = new StringBuilder();
   
            if (XDD_GetModelDriverFromDeviceID(ddMonitor.DeviceID, ref strModel, ref strDriver) == false)  
            {  
                return false;  
            }  
  
  
            // 取得设备EDID数据  
            byte [] EDIDBuf = new byte[256];  
            int dwRealGetBytes = 0;  

            if (   XDD_GetDeviceEDID(strModel.ToString(), strDriver.ToString(), ref EDIDBuf, sizeof(byte) * 256, ref dwRealGetBytes) == false  
                || dwRealGetBytes < 23 )  
            {  
                return false;  
            }  
  
   
            // EDID结构中第22和23个字节为宽度和高度  
            dwWidth = EDIDBuf[21] * 10.0f;  
            dwHeight = EDIDBuf[22] * 10.0f;  
  
            // 成功获取显示器物理尺寸  
            return true;  
        }  
    }
}
