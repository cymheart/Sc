using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScVxTipsProgressBar:ScLayer
    {
        ScVxProgressBar progressBar;
        RectangleF[] progressNumRects;
        ScVxPageTips[] tipsSet;

        int curtProgressNodeIdx;

        public float barHeight = 10;
        public float tipsRowHeightRatio = 0.4f;
        public float barRowHeightRatio = 0.4f;

        public float tipsWidth = 50;

        public Color numColor = Color.White;
        public Font numFont = new Font("微软雅黑", 9, FontStyle.Bold);

        Table mainTable;


        public delegate void ProgressEventHandler(float progressPos);
        public event ProgressEventHandler ProgressEvent;


        public int CurtProgressNodeIdx
        {
            get { return curtProgressNodeIdx; }
            set
            {
                curtProgressNodeIdx = value;
                progressBar.curtProgressNodeIdx = value;
            }
        }


        public ScVxTipsProgressBar()
        {
            progressBar = new ScVxProgressBar();
            progressBar.MouseMoveEvent += ProgressBar_MouseMoveEvent;
            progressBar.MouseLeave += ProgressBar_MouseLeave;

            Add(progressBar);

            SizeChanged += ScVxTipsProgressBar_SizeChanged;
            progressBar.ProgressEvent += ProgressBar_ProgressEvent;

            GDIPaint += ScVxTipsProgressBar_GDIPaint;
        }

        public void AnimToNode(int nodeIdx)
        {
            progressBar.AnimToNode(nodeIdx);
        }

        private void ProgressBar_MouseLeave(object sender)
        {
            for (int i = 0; i < tipsSet.Count(); i++)
            {
                if (progressBar.curtProgressNodeIdx == i)
                    continue;

                tipsSet[i].alpha = 0;
            }

            Refresh();
        }

        private void ProgressBar_MouseMoveEvent(int nodeIdx)
        {
            tipsSet[nodeIdx].alpha = 255;

            for(int i=0; i< tipsSet.Count(); i++)
            {
                if(i != nodeIdx && progressBar.curtProgressNodeIdx != i)
                    tipsSet[i].alpha = 0;
            }

            Refresh();
        }

        private void ScVxTipsProgressBar_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;
            graphis.TextRenderingHint = TextRenderingHint.AntiAlias;


            ScProgressNodeAnim nodeAnim;
            ScProgressNodeAnim[] progressNodeAnims = progressBar.progressNodeAnims;
            RectangleF r;
            Brush brush;
            float n = 100f / progressNodeAnims.Count();
            float val;
            string strVal;

            for (int i = 0; i < progressNodeAnims.Count(); i++)
            {
                nodeAnim = progressNodeAnims[i];

                if(nodeAnim.animScaleValue >= 1.0f)
                {
                    r = progressNumRects[i];

                    val = (i + 1) * n;
                    strVal = Math.Round(val, 0) + "%";

                    brush = new SolidBrush(numColor);
                    DrawUtils.LimitBoxDraw(graphis, strVal, numFont, brush, r, true, 0);
                    brush.Dispose();
                }
            }
        }

        private void ProgressBar_ProgressEvent(float progressPos)
        {
            RectangleF[] progressNodeRects = progressBar.progressNodeRects;
            RectangleF r;

            float len = progressNodeRects[1].Right - progressNodeRects[0].Right;
            float d;

            for (int i = 0; i < progressNodeRects.Count(); i++)
            {
                r = progressNodeRects[i];
                d = progressPos - r.Right;

                if (progressPos > r.Right && i == progressNodeRects.Count() - 1)
                    continue;

                if (d > len || d < -len)
                    tipsSet[i].alpha = 0;
                else
                {
                    tipsSet[i].alpha = (int)Math.Abs(((len - Math.Abs(d)) / len) * 255);
                }    
            }

            if (ProgressEvent != null)
                ProgressEvent(progressPos);

            Refresh();
        }

        private void ScVxTipsProgressBar_SizeChanged(object sender, SizeF oldSize)
        {
            //
            float tipsRowHeight = Height * tipsRowHeightRatio;
            float barRowHeight = Height * barRowHeightRatio;

            RectangleF rect = new RectangleF(0, 0, Width, Height);
            mainTable = new Table(rect, 3, 1);

            Margin margin = new Margin(0, 5, 0, 5);

            mainTable.SetCellMargin(0, 0, margin);
            mainTable.SetCellMargin(2, 0, margin);

            TableLine tableLine = new TableLine(LineDir.HORIZONTAL);
            tableLine.lineComputeMode = LineComputeMode.ABSOLUTE;

            tableLine.computeParam = tipsRowHeight;
            mainTable.SetLineArea(0, tableLine);

            tableLine.computeParam = barRowHeight;
            mainTable.SetLineArea(1, tableLine);

            tableLine.computeParam = Height - tipsRowHeight - barRowHeight;
            mainTable.SetLineArea(2, tableLine);

            mainTable.ComputeLinesArea(LineDir.HORIZONTAL);


            RectangleF tipsRowRect = mainTable.GetCellContentRect(0, 0);
            RectangleF barRowRect = mainTable.GetCellContentRect(1, 0);
            RectangleF numRowRect = mainTable.GetCellContentRect(2, 0);

            progressBar.BarHeight = barHeight;
            progressBar.Height = barRowHeight;
            progressBar.Location = new PointF(barRowRect.X, barRowRect.Y);


            //
            RectangleF[] progressNodeRects = progressBar.progressNodeRects;
            List<ScVxPageTips> tipsList = new List<ScVxPageTips>();
            List<RectangleF> numRectList = new List<RectangleF>();
            ScVxPageTips tips;
            RectangleF r;
            float x;

            string[] nodeNames = progressBar.progressNodeNames;

          

            for (int i=0; i < progressNodeRects.Count(); i++)
            {
                r = progressNodeRects[i];
                x = r.X + r.Width / 2 - tipsWidth / 2;

                tips = new ScVxPageTips();
                tips.txt = nodeNames[i];
                tips.triWidthScale = 4f;
                tips.triHeightScale = 5f;
                tips.bgColor = Color.Black;
                tips.txtColor = Color.White;
                tips.isDisplaySide = true;
                tips.alpha = 0;

                tips.Width = tipsWidth;
                tips.Height = tipsRowRect.Height;
                tips.Location = new PointF(x, tipsRowRect.Y);
                

                Add(tips);
                tipsList.Add(tips);

                //
                x = r.X + r.Width / 2 - r.Width;
                RectangleF r2 = new RectangleF(x, numRowRect.Y, r.Width * 2, numRowRect.Height);
                numRectList.Add(r2);
            }

            tipsSet = tipsList.ToArray();
            progressNumRects = numRectList.ToArray();

            //
        }

        public void SetProgressNodesInfo(string[] progressNodeNames, float nodesSpacing)
        {
            progressBar.SetProgressNodesInfo(progressNodeNames, nodesSpacing);
            Width = progressBar.Width;
        }

        public RectangleF GetBarRowRect()
        {
            RectangleF barRowRect = mainTable.GetCellContentRect(1, 0);
            return barRowRect;
        }
    }
}
