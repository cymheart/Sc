using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class ColorDisplace
    {
        static public byte[,] GetAlphaMask(Bitmap srcBitmap)
        {
            unsafe
            {
                int width = srcBitmap.Width;
                int height = srcBitmap.Height;
                byte[,] alphaMask = new byte[width, height];
                Rectangle bitmapRect = new Rectangle(0, 0, width, height);

                //将Bitmap锁定到系统内存中,获得BitmapData
                BitmapData srcBmData = srcBitmap.LockBits(bitmapRect,
                    ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


                byte* srcScan = (byte*)(void*)(srcBmData.Scan0);

                //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
                byte* srcPtr = srcScan;
                int pos;

                for (int i = 0; i < height; i++)
                {
                    pos = i * srcBmData.Stride;

                    for (int j = 0; j < width; j++)
                    {
                        alphaMask[j, i] = *(srcPtr + pos + 3);
                        pos += 4;
                    }
                }

                //解锁位图
                srcBitmap.UnlockBits(srcBmData);
  
                return alphaMask;
            }
        }

     
        static public void DisplaceAlpha(Bitmap srcBitmap, float alphaScale, byte[,] alphaMask)
        {
            unsafe
            {
                int width = srcBitmap.Width;
                int height = srcBitmap.Height;

                Rectangle bitmapRect = new Rectangle(0, 0, width, height);

                //将Bitmap锁定到系统内存中,获得BitmapData
                BitmapData srcBmData = srcBitmap.LockBits(bitmapRect,
                    ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


                byte* srcScan = (byte*)(void*)(srcBmData.Scan0);

                //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
                byte* srcPtr = srcScan;
                int pos;

                for (int i = 0; i < height; i++)
                {
                    pos = i * srcBmData.Stride;

                    for (int j = 0; j < width; j++)
                    {
                        *(srcPtr + pos + 3) = (byte)(alphaMask[j, i] * alphaScale);
                        pos += 4;
                    }
                }

                //解锁位图
                srcBitmap.UnlockBits(srcBmData);
            }
        }


        static public void Displace(Bitmap srcBitmap, Color color, Rectangle rc, bool usedAlpha = false)
        {
            unsafe
            {
                Rectangle bitmapRect = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);

                //将Bitmap锁定到系统内存中,获得BitmapData
                BitmapData srcBmData = srcBitmap.LockBits(bitmapRect,
                    ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


                byte* srcScan = (byte*)(void*)(srcBmData.Scan0);

                //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
                byte * srcPtr = srcScan + rc.Top * srcBmData.Stride + rc.Left * 4;
                int pos;

                for (int i = 0; i < rc.Height; i++)
                {
                    pos = i * srcBmData.Stride;

                    for (int j = 0; j < rc.Width; j++)
                    {
                        *(srcPtr + pos) = color.B;
                        *(srcPtr + pos + 1) = color.G;
                        *(srcPtr + pos + 2) = color.R;

                        if (usedAlpha)
                            *(srcPtr + pos + 3) = color.A;
  
                        pos += 4;
                    }
                }

                //解锁位图
                srcBitmap.UnlockBits(srcBmData);
            }
        }


        static public void Displace(Bitmap srcBitmap, Color color, bool usedAlpha = false)
        {
            Rectangle rc = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);
            Displace(srcBitmap, color, rc, usedAlpha);
        }

        #region a
        //static public void Displace(Bitmap srcBitmap, Color color, bool usedAlpha = false)
        //{
        //    unsafe
        //    {
        //        int width = srcBitmap.Width;
        //        int height = srcBitmap.Height;
        //        Rectangle rect = new Rectangle(0, 0, width, height);

        //        //将Bitmap锁定到系统内存中,获得BitmapData
        //        BitmapData srcBmData = srcBitmap.LockBits(rect,
        //            ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


        //        //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
        //        System.IntPtr srcPtr = srcBmData.Scan0;

        //        //将Bitmap对象的信息存放到byte数组中
        //        int src_bytes = srcBmData.Stride * height;
        //        byte[] srcValues = new byte[src_bytes];

        //        //复制GRB信息到byte数组
        //        System.Runtime.InteropServices.Marshal.Copy(srcPtr, srcValues, 0, src_bytes);

        //        int pos;
        //        int k = 0;

        //        //根据Y=0.299*R+0.114*G+0.587B,Y为亮度
        //        for (int i = 0; i < height; i++)
        //        {
        //            pos = i * srcBmData.Stride;

        //            for (int j = 0; j < width; j++)
        //            {
        //                srcValues[pos + k] = color.B;
        //                srcValues[pos + k + 1] = color.G;
        //                srcValues[pos + k + 2] = color.R;

        //                if (usedAlpha)
        //                    srcValues[pos + k + 3] = color.A;

        //                pos += 4;
        //            }
        //        }


        //        System.Runtime.InteropServices.Marshal.Copy(srcValues, 0, srcPtr, src_bytes);

        //        //解锁位图
        //        srcBitmap.UnlockBits(srcBmData);
        //    }
        //}

        #endregion

    }
}
