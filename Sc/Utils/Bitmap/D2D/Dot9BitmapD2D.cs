using Sc;
using SharpDX;
using SharpDX.Direct2D1;
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
    public class Dot9BitmapD2D :IDisposable
    {
        SharpDX.Direct2D1.Bitmap bitmap;
        int[] widthStretchPts;
        int[] heightStretchPts;

        SectionInfoD2D[] widthSectionInfos;
        SectionInfoD2D[] heightSectionInfos;

        BitmapStretchInfoD2D[,] bitmapStretchInfos;

        public Dot9BitmapD2D(SharpDX.Direct2D1.Bitmap bitmap, int[] widthStretchPts, int[] heightStretchPts)
        {
            this.bitmap = bitmap;
            this.widthStretchPts = widthStretchPts;
            this.heightStretchPts = heightStretchPts;

            Init();
        }


        void Init()
        {
            int width = (int)bitmap.Size.Width;
            int height = (int)bitmap.Size.Height;


            int[] combineStretchPts = CombineStretchPts(widthStretchPts);
            widthSectionInfos = CreateSectionInfo(combineStretchPts, 0, width - 1);

            combineStretchPts = CombineStretchPts(heightStretchPts);
            heightSectionInfos = CreateSectionInfo(combineStretchPts, 0, height - 1);

            bitmapStretchInfos = new BitmapStretchInfoD2D[widthSectionInfos.Count(), heightSectionInfos.Count()];

            SectionInfoD2D wSectionInfo;
            SectionInfoD2D hSectionInfo;
            int w, h;
            SharpDX.Rectangle rc;
            BitmapStretchInfoD2D bmpStretchInfo;

            for (int i = 0; i < widthSectionInfos.Count(); i++)
            {
                wSectionInfo = widthSectionInfos[i];

                for (int j = 0; j < heightSectionInfos.Count(); j++)
                {
                    hSectionInfo = heightSectionInfos[j];
                    w = wSectionInfo.rightVal - wSectionInfo.leftVal + 1;
                    h = hSectionInfo.rightVal - hSectionInfo.leftVal + 1;
                    rc = new SharpDX.Rectangle(wSectionInfo.leftVal, hSectionInfo.leftVal, w, h);
                    bmpStretchInfo = new BitmapStretchInfoD2D();
                    bmpStretchInfo.orgRect = rc;
                    bitmapStretchInfos[i, j] = bmpStretchInfo;
                }
            }
        }

        int[] CombineStretchPts(int[] stretchPts)
        {
            List<int> strecthPtList = new List<int>();

            int idx;
            strecthPtList.Add(stretchPts[0]);
            strecthPtList.Add(stretchPts[1]);
            idx = strecthPtList.Count() - 2;

            for (int i = 2; i < stretchPts.Count(); i += 2)
            {
                if (strecthPtList[idx + 1] + 1 >= stretchPts[i])
                {
                    strecthPtList[idx + 1] = stretchPts[i + 1];
                }
                else
                {
                    strecthPtList.Add(stretchPts[i]);
                    strecthPtList.Add(stretchPts[i + 1]);
                    idx = strecthPtList.Count() - 2;
                }
            }

            return strecthPtList.ToArray();
        }

        SectionInfoD2D[] CreateSectionInfo(int[] combineStretchPts, int leftLimitVal, int rightLimitVal)
        {
            SectionInfoD2D sectionInfo;
            List<SectionInfoD2D> sectionInfoList = new List<SectionInfoD2D>();
            int endIdx = combineStretchPts.Count() - 1;

            if (combineStretchPts[0] != leftLimitVal)
            {
                sectionInfo = new SectionInfoD2D();
                sectionInfo.type = 0;
                sectionInfo.leftVal = 0;
                sectionInfo.rightVal = combineStretchPts[0] - 1;
                sectionInfoList.Add(sectionInfo);
            }


            for (int i = 0; i < combineStretchPts.Count(); i += 2)
            {
                sectionInfo = new SectionInfoD2D();
                sectionInfo.type = 1;
                sectionInfo.leftVal = combineStretchPts[i];
                sectionInfo.rightVal = combineStretchPts[i + 1];
                sectionInfoList.Add(sectionInfo);

                if (i + 2 < combineStretchPts.Count())
                {
                    sectionInfo = new SectionInfoD2D();
                    sectionInfo.type = 0;
                    sectionInfo.leftVal = combineStretchPts[i + 1] + 1;
                    sectionInfo.rightVal = combineStretchPts[i + 2] - 1;
                    sectionInfoList.Add(sectionInfo);
                }
            }

            if (combineStretchPts[endIdx] != rightLimitVal)
            {
                sectionInfo = new SectionInfoD2D();
                sectionInfo.type = 0;
                sectionInfo.leftVal = combineStretchPts[endIdx] + 1;
                sectionInfo.rightVal = rightLimitVal;
                sectionInfoList.Add(sectionInfo);
            }

            return sectionInfoList.ToArray();
        }

        public void ComputeBitmapStretch(int newWidth, int newHeight)
        {
            SectionInfoD2D wSectionInfo;
            SectionInfoD2D hSectionInfo;

            int[] widthStretchValue = ComputeStretchValue(newWidth, widthSectionInfos);
            int[] heightStretchValue = ComputeStretchValue(newHeight, heightSectionInfos);

            BitmapStretchInfoD2D bmpStretchInfo;
            int x, y;

            for (int i = 0; i < widthSectionInfos.Count(); i++)
            {
                wSectionInfo = widthSectionInfos[i];

                for (int j = 0; j < heightSectionInfos.Count(); j++)
                {
                    hSectionInfo = heightSectionInfos[j];
                    x = 0;
                    y = 0;

                    if (i != 0)
                    {
                        bmpStretchInfo = bitmapStretchInfos[i - 1, j];
                        x = bmpStretchInfo.stretchRect.Right;
                    }

                    if (j != 0)
                    {
                        bmpStretchInfo = bitmapStretchInfos[i, j - 1];
                        y = bmpStretchInfo.stretchRect.Bottom;
                    }

                    bitmapStretchInfos[i, j].stretchRect = new SharpDX.Rectangle(x, y, widthStretchValue[i], heightStretchValue[j]);
                }
            }
        }

        int[] ComputeStretchValue(int newLen, SectionInfoD2D[] sectionInfos)
        {
            SectionInfoD2D sectionInfo;

            int[] stretchValue = new int[sectionInfos.Count()];
            int fixTotalLen = 0;
            int stretchTotalVal = 0;
            int val;

            for (int i = 0; i < sectionInfos.Count(); i++)
            {
                sectionInfo = sectionInfos[i];

                if (sectionInfo.type == 0)
                {
                    stretchValue[i] = sectionInfo.rightVal - sectionInfo.leftVal + 1;
                    fixTotalLen += stretchValue[i];
                }
                else
                {
                    stretchTotalVal += sectionInfo.rightVal - sectionInfo.leftVal + 1;
                    stretchValue[i] = 0;
                }
            }

            int richLen = newLen - fixTotalLen;
            int usedRichLen = 0;
            int lastIdx = 0;

            for (int i = sectionInfos.Count() - 1; i >= 0; i--)
            {
                if (sectionInfos[i].type == 1)
                {
                    lastIdx = i;
                    break;
                }
            }

            for (int i = 0; i < sectionInfos.Count(); i++)
            {
                sectionInfo = sectionInfos[i];

                if (sectionInfo.type == 1)
                {
                    if (i != lastIdx)
                    {
                        val = sectionInfo.rightVal - sectionInfo.leftVal + 1;
                        stretchValue[i] = (int)((float)val / (float)stretchTotalVal * (float)richLen);
                        usedRichLen += stretchValue[i];
                    }
                    else
                    {
                        stretchValue[i] = richLen - usedRichLen;
                        break;
                    }
                }
            }

            return stretchValue;
        }


        public void DrawTo(D2DGraphics g)
        {
            SharpDX.Rectangle orgRect, stretchRect;
            SharpDX.RectangleF orgRectF, stretchRectF;

            BitmapBrush bitmapBrush = new BitmapBrush(g.RenderTarget, bitmap);

            for (int i = 0; i < widthSectionInfos.Count(); i++)
            {
                for (int j = 0; j < heightSectionInfos.Count(); j++)
                {
                    orgRect = bitmapStretchInfos[i, j].orgRect;
                    stretchRect = bitmapStretchInfos[i, j].stretchRect;

                    orgRectF.Left = orgRect.Left;
                    orgRectF.Top = orgRect.Top;
                    orgRectF.Right = orgRect.Right;
                    orgRectF.Bottom = orgRect.Bottom;

                    stretchRectF.Left = stretchRect.Left;
                    stretchRectF.Top = stretchRect.Top;
                    stretchRectF.Right = stretchRect.Right;
                    stretchRectF.Bottom = stretchRect.Bottom;

                    //Matrix3x2 mat = Matrix3x2.Identity;
                    //Matrix3x2.TransformPoint(mat, new Vector2(orgRectF.Left, orgRectF.Top));
                    //bitmapBrush.Transform = mat;

                    //g.RenderTarget.FillRectangle(stretchRectF, bitmapBrush);

                    g.RenderTarget.DrawBitmap(bitmap, stretchRectF, 1.0f, BitmapInterpolationMode.NearestNeighbor, orgRectF);

                   
                }
            }

            bitmapBrush.Dispose();
        }

        static public Dot9BitmapD2D CreateDot9BoxShadowBitmap(D2DGraphics g, float cornersRadius, float shadowRadius, System.Drawing.Color shadowColor)
        {
            int width = (int)(cornersRadius * 2 + shadowRadius * 4 + 2);
            int height = (int)(cornersRadius * 2 + shadowRadius * 4 + 2);

            SharpDX.Direct2D1.PixelFormat format = new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied);
            BitmapProperties prop = new BitmapProperties(format);
            SharpDX.Direct2D1.Bitmap bitmap = new SharpDX.Direct2D1.Bitmap(g.RenderTarget, new Size2(width, height), prop);
  
            unsafe
            {
                System.Drawing.Bitmap gdibmp = new System.Drawing.Bitmap(width, height);
                System.Drawing.RectangleF rect = new System.Drawing.RectangleF(shadowRadius, shadowRadius, width - shadowRadius * 2, height - shadowRadius * 2);
                Graphics gdiGraph = Graphics.FromImage(gdibmp);
                GraphicsPath myPath = DrawUtils.CreateRoundedRectanglePath(rect, cornersRadius);
                System.Drawing.Brush brush = new SolidBrush(shadowColor);
                gdiGraph.FillPath(brush, myPath);
                brush.Dispose();

                GaussianBlur gs = new GaussianBlur((int)shadowRadius);
                gdibmp = gs.ProcessImage(gdibmp);

                System.Drawing.Rectangle bitmapRect = new System.Drawing.Rectangle(0, 0, gdibmp.Width, gdibmp.Height);
                BitmapData srcBmData = gdibmp.LockBits(bitmapRect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                byte* ptr = (byte*)srcBmData.Scan0;
                bitmap.CopyFromMemory((IntPtr)ptr, srcBmData.Stride);
                gdibmp.UnlockBits(srcBmData);

                //  gdibmp.Save("f:\\shadow.png");
                gdibmp.Dispose();
            }

   

            int[] widthPts = new int[]
            {
                (int)(cornersRadius + shadowRadius*2 + 1 ), (int)(width - cornersRadius - shadowRadius*2 - 1)
            };

            int[] heightPts = new int[]
            {
                 (int)(cornersRadius + shadowRadius*2 + 1), (int)(width - cornersRadius - shadowRadius*2 - 1)
            };

         
            Dot9BitmapD2D dot9Bitmap = new Dot9BitmapD2D(bitmap, widthPts, heightPts);
            return dot9Bitmap;
        }

        public void Dispose()
        {
            bitmap.Dispose();
        }
    }


    public class SectionInfoD2D
    {
        public int type;
        public int leftVal;
        public int rightVal;
    }


    public class BitmapStretchInfoD2D
    {
        public SharpDX.Rectangle orgRect;
        public SharpDX.Rectangle stretchRect;
    }
}
