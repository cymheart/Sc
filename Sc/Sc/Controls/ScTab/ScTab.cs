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
    public class ScTab : ScLayer
    {
        ScLayer tabHeadBox;
        ScLayer tabBodyBox;

        ScTabHeader scTabHead;
        Dictionary<ScTabHeaderItem, ScLayer> tabPageDict = new Dictionary<ScTabHeaderItem, ScLayer>();

        public bool hideTabHeight = false;
        public int upOrBottom = 0;
        public float itemWidth = 112;
        int state = 0;

        public delegate void SelectedItemEventHandler(ScTabHeaderItem selectedItem);
        public event SelectedItemEventHandler SelectedItemEvent;

        public int HeaderHeight = 40;
        public int HeaderWidth = 400;
        public SizeF HeaderItemSize = new SizeF(112, 40);
        public float HeaderOffset = 0;

        public ScTab(ScMgr scMgr)
            : base(scMgr)
        {
            IsUseDebugPanitCode = true;
            scTabHead = new ScTabHeader(ScMgr);
            scTabHead.SelectedItemEvent += ScTabHead_SelectedItemEvent;

            scTabHead.IsUseDebugPanitCode = true;

            tabHeadBox = new ScLayer(scMgr);
            tabHeadBox.Add(scTabHead);
            Add(tabHeadBox);

  

            tabBodyBox = new ScLayer(scMgr);
            Add(tabBodyBox);

            tabBodyBox.D2DPaint += TabBodyBox_D2DPaint;

            tabBodyBox.MouseEnter += TabBodyBox_MouseEnter;
            tabBodyBox.MouseLeave += TabBodyBox_MouseLeave;

            SizeChanged += ScTabEx_SizeChanged;         
        }



        private void ScTabEx_SizeChanged(object sender, SizeF oldSize)
        {
            tabHeadBox.Width = Width;
            tabHeadBox.Height = HeaderHeight;

            scTabHead.Size = new SizeF(Width, HeaderItemSize.Height);
            scTabHead.Location = new PointF(HeaderOffset, tabHeadBox.Height - scTabHead.Height);
         
            float y = tabHeadBox.Location.Y + tabHeadBox.Height;
            tabBodyBox.Location = new PointF(0, y);
            tabBodyBox.Width = Width;
            tabBodyBox.Height = Height - tabHeadBox.Height;
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

        private void ScTabHead_SelectedItemEvent(ScTabHeaderItem selectedItem)
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
    

        public ScTabHeaderItem AddTabPageContentLayer(string name, ScLayer layer)
        {
            ScTabHeaderItem item = scTabHead.AddItem(name);

            ScLayer tabBodyViewBox = new ScLayer();
            tabBodyViewBox.Location = new Point(5, 5);
            tabBodyViewBox.Width = tabBodyBox.Width - 10;
            tabBodyViewBox.Height = tabBodyBox.Height - 10;

            if (layer != null)
                tabBodyViewBox.Add(layer);

            tabPageDict.Add(item, tabBodyViewBox);
            return item;
        }

        public void SetSelectedTabPage(ScTabHeaderItem item)
        {
            scTabHead.SetSelectedItem(item);
        }

        public void SetSelectedTabPage(int selectedIndex)
        {
            scTabHead.SetSelectedIndex(selectedIndex);
        }

        private void TabBodyBox_D2DPaint(D2DGraphics g)
        {

            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
            //RawRectangleF rect = new RawRectangleF(0, 0, Width - 1, Height - 1);
            //RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 255, 255, 255));
            //SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            //g.RenderTarget.FillRectangle(rect, brush);

            RawRectangleF rect = new RawRectangleF(1, 1, tabBodyBox.Width - 1, tabBodyBox.Height - 1);
            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 222, 226, 230));
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.DrawRectangle(rect, brush, 2);




            //RawRectangleF rect = new RawRectangleF(0, 0, tabBodyBox.Width, tabBodyBox.Height);
            //PathGeometry pathGeo = DrawUtils.CreateOutlineRoundRectGeometry(g, rect, 6);

            //RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 191, 152, 90));
            //SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            //g.RenderTarget.FillGeometry(pathGeo, brush);
        }

      
    }
}
