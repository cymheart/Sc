using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Utils;

namespace Sc
{
    public partial class ScTabSharp : UserControl
    {
        ScMgr scMgr;
        ScLayer root;
        ScTab scTab;
        ScLayer titleInfoLayer;
        Font font = new Font("微软雅黑", 13);

        public string titleInfo = "";

        public delegate void SelectedItemEventHandler(ScTabHeadItem selectedItem);
        public event SelectedItemEventHandler SelectedItemEvent;

        public ScTabSharp(int w, int h, int itemWidth = 112, int upOrBottom = 0, bool hideTabHeight = false)
        {
            InitializeComponent();

            Width = w;
            Height = h;

            #region 防止打开闪烁
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            #endregion

            scMgr = new ScMgr(Width, Height);
            scMgr.BackgroundColor =  Color.FromArgb(255, 233, 233, 233);
            Controls.Add(scMgr.control);

            root = scMgr.GetRootLayer();

            //
            scTab = new ScTab(scMgr, root.Width, root.Height, itemWidth, upOrBottom, hideTabHeight);
            scTab.Location = new Point(0, 0);
            scTab.SelectedItemEvent += ScTab_SelectedItemEvent;

            root.Add(scTab);



            //
            titleInfoLayer = new ScLayer();
            titleInfoLayer.Width = 60;
            titleInfoLayer.Height = 25;
            titleInfoLayer.Location = new PointF(Width - 60, 0);
            root.Add(titleInfoLayer);

            titleInfoLayer.GDIPaint += TitleInfoLayer_GDIPaint;

        }

        private void ScTab_SelectedItemEvent(ScTabHeadItem selectedItem)
        {
            if (SelectedItemEvent != null)
                SelectedItemEvent(selectedItem);
        }

        public void SetBGColor(Color color)
        {
            scMgr.BackgroundColor = color;
        }

        private void TitleInfoLayer_GDIPaint(GDIGraphics g)
        {
            Graphics gdiGraph = g.GdiGraph;
            gdiGraph.SmoothingMode = SmoothingMode.HighQuality;// 指定高质量、低速度呈现。
            gdiGraph.TextRenderingHint = TextRenderingHint.AntiAlias;

            RectangleF rect = new RectangleF(0, 0, titleInfoLayer.Width, titleInfoLayer.Height);
            Brush brush = new SolidBrush(Color.FromArgb(153, 114, 49));
            DrawUtils.LimitBoxDraw(gdiGraph, titleInfo, font, brush, rect, true, 0);

        }

        public SizeF GetTabPageSize()
        {
            return scTab.GetTabPageSize();
        }
        public ScTabHeadItem AddTabPage(string name, ScLayer scControl)
        {
            ScTabHeadItem item = scTab.AddTabPageContentControl(name, scControl);
            return item;
        }

        public ScTabHeadItem AddTabPage(string name)
        {
            /*
            Size size = scTab.GetTabPageSize();
            ScContainer scScrollContainer = new ScContainer(size.Width, size.Height);
            scScrollContainer.HitRegion = new Region(scScrollContainer.rect);
            */

            ScTabHeadItem item = scTab.AddTabPageContentControl(name, null);
            return item;
        }


        public void SetSelectedTabPage(ScTabHeadItem item)
        {
            scTab.SetSelectedTabPage(item);
        }

     
        private void SimpleTabHead_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
