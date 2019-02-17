using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
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
    public class ScTab: ScLayer
    {
        ScLayer tabHeadBox;
        ScLayer tabBodyBox;
  
        ScTabHead scTabHead;
        Dictionary<ScTabHeadItem, ScLayer> tabPageDict = new Dictionary<ScTabHeadItem, ScLayer>();

        public bool hideTabHeight = false;
        public int upOrBottom = 0;
        public float itemWidth = 112;
        int state = 0;

        public delegate void SelectedItemEventHandler(ScTabHeadItem selectedItem);
        public event SelectedItemEventHandler SelectedItemEvent;


        public ScTab(ScMgr scMgr, float w, float h, int itemWidth, int upOrBottom, bool hideTabHeight)
            :base(scMgr)
        {
            this.upOrBottom = upOrBottom;
            this.hideTabHeight = hideTabHeight;
            this.itemWidth = itemWidth;
            Init(w, h);
        }
        public ScTab(ScMgr scMgr, float w, float h)
             : base(scMgr)
        {
            Init(w, h);
        }

        void Init(float w, float h)
        {
            Width = w;
            Height = h;

            float headHeight = 24;
            if (hideTabHeight)
                headHeight = 0;

            scTabHead = new ScTabHead(ScMgr, Width - 12, headHeight);
            scTabHead.SetItemSize(new SizeF(itemWidth, headHeight));
            scTabHead.upOrBottom = upOrBottom;
            scTabHead.Location = new Point(6, 0);
            scTabHead.SelectedItemEvent += ScTabHead_SelectedItemEvent;

            //
            tabHeadBox = new ScLayer();
            tabHeadBox.Width = Width;
            tabHeadBox.Height = headHeight;

            tabHeadBox.Add(scTabHead);
            Add(tabHeadBox);

            tabBodyBox = new ScLayer();

            if (upOrBottom == 0)
            {       
                float y = tabHeadBox.Location.Y + tabHeadBox.Height;
                tabBodyBox.Location = new PointF(0, y);
                tabBodyBox.Width = Width;
                tabBodyBox.Height = Height - tabHeadBox.Height;
            }
            else
            {
                tabBodyBox.Location = new PointF(0, 0);
                tabBodyBox.Width = Width;
                tabBodyBox.Height = Height - tabHeadBox.Height;

                float y = tabBodyBox.Location.Y + tabBodyBox.Height - 1;
                tabHeadBox.Location = new PointF(0, y);
            }

            tabBodyBox.D2DPaint += TabBodyBox_D2DPaint;
            tabBodyBox.GDIPaint += TabBodyBox_GDIPaint;

            tabBodyBox.MouseEnter += TabBodyBox_MouseEnter;
            tabBodyBox.MouseLeave += TabBodyBox_MouseLeave;

            Add(tabBodyBox);
        }


        private void TabBodyBox_MouseLeave(object sender)
        {
            state = 0;
            Refresh();
        }

        private void TabBodyBox_MouseEnter(object sender, ScMouseEventArgs e)
        {
            state = 1;
            Refresh();
        }

        public SizeF GetTabPageSize()
        {
            SizeF size = new SizeF(tabBodyBox.Width - 10, tabBodyBox.Height - 10);
            return size;
        }

        private void ScTabHead_SelectedItemEvent(ScTabHeadItem selectedItem)
        {
            if (tabPageDict.ContainsKey(selectedItem))
            {
                ScLayer tabBodyViewBox = tabPageDict[selectedItem];
                tabBodyBox.Clear();
      
                tabBodyBox.Add(tabBodyViewBox);

                if (SelectedItemEvent != null)
                    SelectedItemEvent(selectedItem);

                tabBodyViewBox.Refresh();
            }
        }

        public ScTabHeadItem AddTabPageContentControl(string name, ScLayer control)
        {
            ScTabHeadItem item = scTabHead.AddItem(name);

            ScLayer tabBodyViewBox = new ScLayer();
            tabBodyViewBox.Location = new Point(5, 5);
            tabBodyViewBox.Width = tabBodyBox.Width - 10;
            tabBodyViewBox.Height = tabBodyBox.Height - 10;

            if (control != null)
                tabBodyViewBox.Add(control);

            tabPageDict.Add(item, tabBodyViewBox);
            return item;
        }

        public void SetSelectedTabPage(ScTabHeadItem item)
        {
            if (item == null)
                return;

            scTabHead.SetSelectedItem(item);
        }

        public void SetSelectedTabPage(int selectedIndex)
        {
            scTabHead.SetSelectedIndex(selectedIndex);
        }

        public void Layout()
        {
            scTabHead.ReAddAllItems();
            scTabHead.SetSelectedIndex(0);
        }                                                                                                                                   


        private void TabBodyBox_D2DPaint(D2DGraphics g)
        {
            RawRectangleF rect = new RawRectangleF(0, 0, tabBodyBox.Width, tabBodyBox.Height);
            PathGeometry pathGeo = DrawUtils.CreateOutlineRoundRectGeometry(g, rect, 6);

            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 191, 152, 90));
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.FillGeometry(pathGeo, brush);
        }

        private void TabBodyBox_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;

            graphis.SmoothingMode = SmoothingMode.HighQuality;// 指定高质量、低速度呈现。
            graphis.TextRenderingHint = TextRenderingHint.AntiAlias;

            RectangleF rect = new RectangleF(0, 0, tabBodyBox.Width - 1, tabBodyBox.Height - 1);
            Pen pen;

            if (state == 1)
                pen = new Pen(Color.FromArgb(255, 255, 0, 0), 1f);
            else
                pen = new Pen(Color.FromArgb(255, 191, 152, 90), 1f);


            DrawUtils.DrawRoundRectangle(graphis, pen, rect, 4);
           
        }
    }
}
