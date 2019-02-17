using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class BitmapProcess
    {
        static public void TileParseImage(Bitmap srcBitmap, Rectangle srcRect, Bitmap dstBitmap, Rectangle dstRect)
        {
            unsafe
            {
                Rectangle srcBitmapRect = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);
                Rectangle dstBitmapRect = new Rectangle(0, 0, dstBitmap.Width, dstBitmap.Height);

                //将Bitmap锁定到系统内存中,获得BitmapData
                BitmapData srcBmData = srcBitmap.LockBits(srcBitmapRect,
                    ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                BitmapData dstBmData = dstBitmap.LockBits(dstBitmapRect,
                   ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                byte* srcScan = (byte*)(void*)(srcBmData.Scan0);
                byte* srcPtr = srcScan + srcRect.Top * srcBmData.Stride + srcRect.Left * 4;

                byte* dstScan = (byte*)(void*)(dstBmData.Scan0);
                byte* dstPtr = dstScan + dstRect.Top * dstBmData.Stride + dstRect.Left * 4;
                byte* tileDstPtr;
                int pos;
                int srcpos;


                Rectangle rc = new Rectangle(0,0, srcRect.Width, srcRect.Height);
                int wn = dstRect.Width / srcRect.Width;
                int hn = dstRect.Height / srcRect.Height;

                int wCount = wn;
                int hCount = hn;

                int richWidth = dstRect.Width - wn * srcRect.Width;
                int richHeight = dstRect.Height - hn * srcRect.Height;

                if (richWidth > 0)
                    wCount++;

                if (richHeight > 0)
                    hCount++;

                int[] wlist = new int[wCount];
                int[] hlist = new int[hCount];

                for (int i = 0; i < wn; i++)
                    wlist[i] = srcRect.Width;

                if (richWidth > 0)
                    wlist[wCount - 1] = richWidth;

                for (int i = 0; i < hn; i++)
                    hlist[i] = srcRect.Height;

                if (richHeight > 0)
                    hlist[hCount - 1] = richHeight;


                for (int i = 0; i < wlist.Count(); i++)
                {
                    for (int j = 0; j < hlist.Count(); j++)
                    {
                        rc.X = i * srcRect.Width;
                        rc.Y = j * srcRect.Height;
                        rc.Width = wlist[i];
                        rc.Height = hlist[j];

                        tileDstPtr = dstPtr + rc.Top * dstBmData.Stride + rc.Left * 4;

                        for (int m = 0; m < rc.Height; m++)
                        {
                            pos = m * dstBmData.Stride;
                            srcpos = m * srcBmData.Stride;

                            for (int n = 0; n < rc.Width; n++)
                            {
                                *((int*)(tileDstPtr + pos)) = *((int*)(srcPtr + srcpos));
                                pos += 4;
                                srcpos += 4;
                            }
                        }
                    }
                }

                //解锁位图
                srcBitmap.UnlockBits(srcBmData);
                dstBitmap.UnlockBits(dstBmData);
            }
        }


        static public void BoxShadow(Bitmap processBitmap, int shadowRadius, int[] cornersType, int[] cornersRadius)
        {
            int r = shadowRadius;
            int sideWidth = shadowRadius;
            int offset = 2;

            Graphics g;
            Bitmap bitmap;
            Rectangle rc;
            Rectangle rc1;
            Bitmap effectBitmap;
            GraphicsPath path;
            GaussianBlur gs = new GaussianBlur(r);

            int width = processBitmap.Width;
            int height = processBitmap.Height;
            Graphics baseg = Graphics.FromImage(processBitmap);

            //
            int leftTopWidth = sideWidth + r;
            int leftTopHeight = sideWidth + r;
            bitmap = new Bitmap(leftTopWidth, leftTopHeight);
            g = Graphics.FromImage(bitmap);
            rc = new Rectangle(sideWidth, sideWidth, r, r);

            if (cornersType[0] == 0)
            {
                g.FillRectangle(Brushes.Black, rc);
                effectBitmap = gs.ProcessImage(bitmap);
                baseg.DrawImage(effectBitmap, 0, 0, new Rectangle(0, 0, sideWidth, sideWidth), GraphicsUnit.Pixel);
            }
            else
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                path = DrawUtils.CreateRoundedRectanglePath(rc, cornersRadius[0]);
                g.FillPath(Brushes.Black, path);
                path.Dispose();
                effectBitmap = gs.ProcessImage(bitmap);
                baseg.DrawImage(effectBitmap, 0, 0, new Rectangle(0, 0, sideWidth, sideWidth), GraphicsUnit.Pixel);
                baseg.DrawImage(effectBitmap, sideWidth, sideWidth, new Rectangle(sideWidth, sideWidth, r, r), GraphicsUnit.Pixel);
            }

            g.Dispose();
            bitmap.Dispose();


            ////
            int rightTopWidth = sideWidth + r;
            int rightTopHeight = sideWidth + r;
            bitmap = new Bitmap(rightTopWidth, rightTopHeight);
            g = Graphics.FromImage(bitmap);
            rc = new Rectangle(0, sideWidth, r, r);

            if (cornersType[0] == 0)
            {
                g.FillRectangle(Brushes.Black, rc);
                effectBitmap = gs.ProcessImage(bitmap);
                baseg.DrawImage(effectBitmap, width - sideWidth, 0, new Rectangle(rightTopWidth - sideWidth, 0, sideWidth, sideWidth), GraphicsUnit.Pixel);
            }
            else
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                path = DrawUtils.CreateRoundedRectanglePath(rc, cornersRadius[0]);
                g.FillPath(Brushes.Black, path);
                path.Dispose();
                effectBitmap = gs.ProcessImage(bitmap);
                baseg.DrawImage(effectBitmap, width - sideWidth, 0, new Rectangle(rightTopWidth - sideWidth, 0, sideWidth, sideWidth), GraphicsUnit.Pixel);
                baseg.DrawImage(effectBitmap, width - sideWidth - r, sideWidth, new Rectangle(0, sideWidth, r, r), GraphicsUnit.Pixel);
            }
            g.Dispose();
            bitmap.Dispose();

            ////
            int leftBottomWidth = sideWidth + r;
            int leftBottomHeight = sideWidth + r;
            bitmap = new Bitmap(leftBottomWidth, leftBottomHeight);
            g = Graphics.FromImage(bitmap);
            rc = new Rectangle(sideWidth, 0, r, r);

            if (cornersType[2] == 0)
            {
                g.FillRectangle(Brushes.Black, rc);
                effectBitmap = gs.ProcessImage(bitmap);
                baseg.DrawImage(effectBitmap, 0, height - sideWidth, new Rectangle(0, leftBottomHeight - sideWidth, sideWidth, sideWidth), GraphicsUnit.Pixel);
            }
            else
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                path = DrawUtils.CreateRoundedRectanglePath(rc, cornersRadius[0]);
                g.FillPath(Brushes.Black, path);
                path.Dispose();
                effectBitmap = gs.ProcessImage(bitmap);
                baseg.DrawImage(effectBitmap, 0, height - sideWidth, new Rectangle(0, leftBottomHeight - sideWidth, sideWidth, sideWidth), GraphicsUnit.Pixel);
                baseg.DrawImage(effectBitmap, sideWidth, height - sideWidth - r, new Rectangle(sideWidth, 0, r, r), GraphicsUnit.Pixel);
            }

            g.Dispose();
            bitmap.Dispose();

            ////
            int rightBottomWidth = sideWidth + r;
            int rightBottomHeight = sideWidth + r;
            bitmap = new Bitmap(rightBottomWidth, rightBottomHeight);
            g = Graphics.FromImage(bitmap);
            rc = new Rectangle(0, 0, r, r);

            if (cornersType[3] == 0)
            {
                g.FillRectangle(Brushes.Black, rc);
                effectBitmap = gs.ProcessImage(bitmap);
                baseg.DrawImage(effectBitmap, width - sideWidth, height - sideWidth, new Rectangle(rightBottomHeight - sideWidth, rightBottomHeight - sideWidth, sideWidth, sideWidth), GraphicsUnit.Pixel);
            }
            else
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                path = DrawUtils.CreateRoundedRectanglePath(rc, cornersRadius[0]);
                g.FillPath(Brushes.Black, path);
                path.Dispose();
                effectBitmap = gs.ProcessImage(bitmap);
                baseg.DrawImage(effectBitmap, width - sideWidth, height - sideWidth, new Rectangle(rightBottomHeight - sideWidth, rightBottomHeight - sideWidth, sideWidth, sideWidth), GraphicsUnit.Pixel);
                baseg.DrawImage(effectBitmap, width - sideWidth - r, height - sideWidth - r, new Rectangle(0, 0, r, r), GraphicsUnit.Pixel);
            }

            g.Dispose();
            bitmap.Dispose();


            ////
            int topWidth = width;
            int topHeight = r + sideWidth;
            bitmap = new Bitmap(topWidth, topHeight);
            g = Graphics.FromImage(bitmap);
            rc = new Rectangle(sideWidth, sideWidth, width - sideWidth * 2, r);

            if (cornersType[0] == 1)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                rc1 = new Rectangle(rc.X, rc.Y, rc.Width / 2 + offset, rc.Height );
                path = DrawUtils.CreateRoundedRectanglePath(rc1, cornersRadius[0]);
                g.FillPath(Brushes.Black, path);
            }
            else
            {
                rc1 = new Rectangle(rc.X, rc.Y, rc.Width / 2 + offset, rc.Height);
                g.FillRectangle(Brushes.Black, rc1);
            }


            if (cornersType[1] == 1)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                rc1 = new Rectangle(rc.Right - rc.Width /2 - offset, rc.Y, rc.Width / 2 + offset, rc.Height);
                path = DrawUtils.CreateRoundedRectanglePath(rc1, cornersRadius[1]);
                g.FillPath(Brushes.Black, path);
            }
            else
            {
                rc1 = new Rectangle(rc.Right - rc.Width / 2 - offset, rc.Y, rc.Width / 2 + offset, rc.Height);
                g.FillRectangle(Brushes.Black, rc1);
            }

            effectBitmap = gs.ProcessImage(bitmap);
            baseg.DrawImage(effectBitmap, sideWidth, 0, new Rectangle(sideWidth, 0, topWidth - sideWidth * 2, sideWidth), GraphicsUnit.Pixel);
            g.Dispose();
            bitmap.Dispose();

            ////
            int bottomWidth = width;
            int bottomHeight = r + sideWidth;
            bitmap = new Bitmap(bottomWidth, bottomHeight);
            g = Graphics.FromImage(bitmap);
            rc = new Rectangle(sideWidth, 0, bottomWidth - 2 * sideWidth, r);

            if (cornersType[2] == 1)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                rc1 = new Rectangle(rc.X, rc.Y, rc.Width / 2 + offset, rc.Height);
                path = DrawUtils.CreateRoundedRectanglePath(rc1, cornersRadius[0]);
                g.FillPath(Brushes.Black, path);
            }
            else
            {
                rc1 = new Rectangle(rc.X, rc.Y, rc.Width / 2 + offset, rc.Height);
                g.FillRectangle(Brushes.Black, rc1);
            }


            if (cornersType[3] == 1)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                rc1 = new Rectangle(rc.Right - rc.Width / 2 - offset, rc.Y, rc.Width / 2 + offset, rc.Height);
                path = DrawUtils.CreateRoundedRectanglePath(rc1, cornersRadius[1]);
                g.FillPath(Brushes.Black, path);
            }
            else
            {
                rc1 = new Rectangle(rc.Right - rc.Width / 2 - offset, rc.Y, rc.Width / 2 + offset, rc.Height);
                g.FillRectangle(Brushes.Black, rc1);
            }

            effectBitmap = gs.ProcessImage(bitmap);
            baseg.DrawImage(effectBitmap, sideWidth, height - sideWidth, new Rectangle(sideWidth, bottomHeight - sideWidth, bottomWidth - sideWidth * 2, sideWidth), GraphicsUnit.Pixel);
            g.Dispose();
            bitmap.Dispose();

            ////
            int leftWidth = sideWidth + r;
            int leftHeight = height;
            bitmap = new Bitmap(leftWidth, leftHeight);
            g = Graphics.FromImage(bitmap);
            rc = new Rectangle(sideWidth, sideWidth, r, leftHeight - sideWidth * 2);

            if (cornersType[0] == 1)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                rc1 = new Rectangle(rc.X, rc.Y, rc.Width, rc.Height / 2 + offset);
                path = DrawUtils.CreateRoundedRectanglePath(rc1, cornersRadius[0]);
                g.FillPath(Brushes.Black, path);
            }
            else
            {
                rc1 = new Rectangle(rc.X, rc.Y, rc.Width, rc.Height / 2 + offset);
                g.FillRectangle(Brushes.Black, rc1);
            }


            if (cornersType[2] == 1)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                rc1 = new Rectangle(rc.X, rc.Bottom - rc.Height / 2 - offset, rc.Width , rc.Height /2 + offset);
                path = DrawUtils.CreateRoundedRectanglePath(rc1, cornersRadius[2]);
                g.FillPath(Brushes.Black, path);
            }
            else
            {
                rc1 = new Rectangle(rc.X, rc.Bottom - rc.Height / 2 - offset, rc.Width, rc.Height / 2 + offset);
                g.FillRectangle(Brushes.Black, rc1);
            }

            effectBitmap = gs.ProcessImage(bitmap);
            baseg.DrawImage(effectBitmap, 0, sideWidth, new Rectangle(0, sideWidth, sideWidth, leftHeight - sideWidth * 2), GraphicsUnit.Pixel);
            g.Dispose();
            bitmap.Dispose();

            ////
            int rightWidth = sideWidth + r;
            int rightHeight = height;
            bitmap = new Bitmap(leftWidth, leftHeight);
            g = Graphics.FromImage(bitmap);
            rc = new Rectangle(0, sideWidth, r, rightHeight - sideWidth * 2);


            if (cornersType[1] == 1)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                rc1 = new Rectangle(rc.X, rc.Y, rc.Width, rc.Height / 2 + offset);
                path = DrawUtils.CreateRoundedRectanglePath(rc1, cornersRadius[1]);
                g.FillPath(Brushes.Black, path);
            }
            else
            {
                rc1 = new Rectangle(rc.X, rc.Y, rc.Width, rc.Height / 2 + offset);
                g.FillRectangle(Brushes.Black, rc1);
            }


            if (cornersType[3] == 1)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                rc1 = new Rectangle(rc.X, rc.Bottom - rc.Height / 2 - offset, rc.Width, rc.Height / 2 + offset);
                path = DrawUtils.CreateRoundedRectanglePath(rc1, cornersRadius[3]);
                g.FillPath(Brushes.Black, path);
            }
            else
            {
                rc1 = new Rectangle(rc.X, rc.Bottom - rc.Height / 2 - offset, rc.Width, rc.Height / 2 + offset);
                g.FillRectangle(Brushes.Black, rc1);
            }

            effectBitmap = gs.ProcessImage(bitmap);
            baseg.DrawImage(effectBitmap, width - sideWidth, sideWidth, new Rectangle(leftWidth - sideWidth, sideWidth, sideWidth, leftHeight - sideWidth * 2), GraphicsUnit.Pixel);
            g.Dispose();
            bitmap.Dispose();

            baseg.Dispose();
        }

    }
}
