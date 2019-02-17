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
    public class Dot9Bitmap
    {
        Bitmap bitmap;
        Graphics orgGraphics;
        int[] widthStretchPts;
        int[] heightStretchPts;

        SectionInfo[] widthSectionInfos;
        SectionInfo[] heightSectionInfos;

        BitmapStretchInfo[,] bitmapStretchInfos;

        public Dot9Bitmap(Bitmap bitmap, int[] widthStretchPts, int[] heightStretchPts)
        {
            this.bitmap = bitmap;
            orgGraphics = Graphics.FromImage(this.bitmap);
            this.widthStretchPts = widthStretchPts;
            this.heightStretchPts = heightStretchPts;

            Init();
        }


        void Init()
        {
            int[] combineStretchPts = CombineStretchPts(widthStretchPts);
            widthSectionInfos = CreateSectionInfo(combineStretchPts, 0, bitmap.Width - 1);

            combineStretchPts = CombineStretchPts(heightStretchPts);
            heightSectionInfos = CreateSectionInfo(combineStretchPts, 0, bitmap.Height - 1);

            bitmapStretchInfos = new BitmapStretchInfo[widthSectionInfos.Count(), heightSectionInfos.Count()];

            SectionInfo wSectionInfo;
            SectionInfo hSectionInfo;
            int w, h;
            Rectangle rc;
            BitmapStretchInfo bmpStretchInfo;

            for (int i = 0; i < widthSectionInfos.Count(); i++)
            {
                wSectionInfo = widthSectionInfos[i];

                for (int j = 0; j < heightSectionInfos.Count(); j++)
                {
                    hSectionInfo = heightSectionInfos[j];
                    w = wSectionInfo.rightVal - wSectionInfo.leftVal + 1;
                    h = hSectionInfo.rightVal - hSectionInfo.leftVal + 1;
                    rc = new Rectangle(wSectionInfo.leftVal, hSectionInfo.leftVal, w, h);
                    bmpStretchInfo = new BitmapStretchInfo();
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

        SectionInfo[] CreateSectionInfo(int[] combineStretchPts, int leftLimitVal, int rightLimitVal)
        {
            SectionInfo sectionInfo;
            List<SectionInfo> sectionInfoList = new List<SectionInfo>();
            int endIdx = combineStretchPts.Count() - 1;

            if (combineStretchPts[0] != leftLimitVal)
            {
                sectionInfo = new SectionInfo();
                sectionInfo.type = 0;
                sectionInfo.leftVal = 0;
                sectionInfo.rightVal = combineStretchPts[0] - 1;
                sectionInfoList.Add(sectionInfo);
            }


            for (int i = 0; i < combineStretchPts.Count(); i += 2)
            {
                sectionInfo = new SectionInfo();
                sectionInfo.type = 1;
                sectionInfo.leftVal = combineStretchPts[i];
                sectionInfo.rightVal = combineStretchPts[i + 1];
                sectionInfoList.Add(sectionInfo);

                if (i + 2 < combineStretchPts.Count())
                {
                    sectionInfo = new SectionInfo();
                    sectionInfo.type = 0;
                    sectionInfo.leftVal = combineStretchPts[i + 1] + 1;
                    sectionInfo.rightVal = combineStretchPts[i + 2] - 1;
                    sectionInfoList.Add(sectionInfo);
                }
            }

            if (combineStretchPts[endIdx] != rightLimitVal)
            {
                sectionInfo = new SectionInfo();
                sectionInfo.type = 0;
                sectionInfo.leftVal = combineStretchPts[endIdx] + 1;
                sectionInfo.rightVal = rightLimitVal;
                sectionInfoList.Add(sectionInfo);
            }

            return sectionInfoList.ToArray();
        }

        public void ComputeBitmapStretch(int newWidth, int newHeight)
        {
            SectionInfo wSectionInfo;
            SectionInfo hSectionInfo;

            int[] widthStretchValue = ComputeStretchValue(newWidth, widthSectionInfos);
            int[] heightStretchValue = ComputeStretchValue(newHeight, heightSectionInfos);

            BitmapStretchInfo bmpStretchInfo;
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
                   
                    bitmapStretchInfos[i, j].stretchRect = new Rectangle(x, y, widthStretchValue[i], heightStretchValue[j]);
                }
            }
        }

        public void DrawTo(Bitmap stretchBitmap)
        {
            Rectangle orgRect;
            Rectangle stretchRect;

            for (int i=0; i< widthSectionInfos.Count(); i++)
            {
                for (int j = 0; j < heightSectionInfos.Count();j++)
                {
                    orgRect = bitmapStretchInfos[i, j].orgRect;
                    stretchRect = bitmapStretchInfos[i, j].stretchRect;    
                    BitmapProcess.TileParseImage(bitmap, orgRect, stretchBitmap, stretchRect); 
                }
            }
        }

        int[] ComputeStretchValue(int newLen, SectionInfo[] sectionInfos)
        {
            SectionInfo sectionInfo;
      
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
    }





    public class SectionInfo
    {
        public int type;
        public int leftVal;
        public int rightVal;
    }


    public class BitmapStretchInfo
    {
        public Rectangle orgRect;
        public Rectangle stretchRect;
    }
}
