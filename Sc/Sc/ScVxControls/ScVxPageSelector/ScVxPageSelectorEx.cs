using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace Sc
{
    public class ScVxPageSelectorEx:ScLayer
    {
        public ScVxPageSelector ps;

        RectangleF leftShowRect;
        RectangleF rightShowRect;

        int leftPageNum;
        int rightPageNum;

        public int numAlpha = 80;
        public Color numColor = Color.FromArgb(133, 215, 223);
        public Font font = new Font("微软雅黑", 12);
        bool isShowNum = false;
        public ScVxPageSelectorEx()
        {
            ps = new ScVxPageSelector();
            ps.PageMoveEvent += PageSelector_PageMoveEvent;
            Add(ps);

            SizeChanged += ScVxPageSelectorEx_SizeChanged;
            GDIPaint += ScVxPageSelectorEx_GDIPaint;
            
        }

        private void PageSelector_PageMoveEvent()
        {
        
            leftPageNum = ps.GetCurtLeftPageNum() + 1;
            rightPageNum = ps.GetCurtRightPageNum();

            if (ps.pageAmount < ps.showPageAmount)
            {
                isShowNum = false;
                return;
            }
            else
                isShowNum = true;

            Refresh();
        }
 
        private void ScVxPageSelectorEx_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;
            graphis.TextRenderingHint = TextRenderingHint.AntiAlias;

            if (!isShowNum)
                return;

            Color c = Color.FromArgb(numAlpha, numColor);
            Brush brush = new SolidBrush(c);
            DrawUtils.LimitBoxDraw(graphis, leftPageNum.ToString(), font, brush, leftShowRect, true, 0);
            DrawUtils.LimitBoxDraw(graphis, rightPageNum.ToString(), font, brush, rightShowRect, true, 0);
            brush.Dispose();
        }

        public void SetTips(ScVxPageTips tips)
        {
            ps.SetTips(tips);
        }


        public float GetFixWidth()
        {
            float w = ps.GetFixWidth();
            w += 80;
            return w;
        }

        private void ScVxPageSelectorEx_SizeChanged(object sender, SizeF oldSize)
        {
            ps.Width = (int)(ps.GetFixWidth());
            ps.Height = (int)(Height - 10);

            leftShowRect = new RectangleF(0, 0, 40, Height);
            rightShowRect = new RectangleF(Width - 40, 0,40, Height);

            RectangleF middleShowRect = new RectangleF(leftShowRect.Right, 0, Width - 80, Height);

            float x = middleShowRect.X + middleShowRect.Width / 2 - ps.Width / 2;
            float y = middleShowRect.Y + middleShowRect.Height / 2 - ps.Height / 2;
            ps.Location = new PointF((int)x, (int)y);

            leftPageNum = ps.GetCurtLeftPageNum() + 1;
            rightPageNum = ps.GetCurtRightPageNum();
        }
    }
}
