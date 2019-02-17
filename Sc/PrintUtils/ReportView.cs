using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Utils;
using MouseKeyboardLibrary;

namespace PrintUtils
{
    public partial class ReportView : UserControl
    {
        public Print print;
        PrintPageBase printPage;
        MonitorInfo monitorInfo = new MonitorInfo();
        SizeF pageSize;
        MouseHook mouseHook = new MouseHook();
        KeyboardHook keyboardHook = new KeyboardHook();
        bool isKeyDownCtrl = false;

        Image reportImage;
        float scale = 1.0f;


        string printModuleKey;
        PrinterDocumentPreSettingOp printerDocPreSettingOp;

        public ReportView(string printModuleKey, Color viewBgColor)
        {
            InitializeComponent();

            #region 防止打开闪烁
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true);         // 双缓冲
            #endregion

            SetControlsDoubleBuffer(this);

            if(viewBgColor != null)
                panelView.BackColor = viewBgColor;

            this.printModuleKey = printModuleKey;
            print = new Print();
            print.SetPrintPageEvenHandler(printDocument_PrintPage);

            printerDocPreSettingOp = new PrinterDocumentPreSettingOp();
            printerDocPreSettingOp.printDocument = print.printDocument;

           // printerDocPreSettingOp.LoadPreSettingToPrintDocument(printModuleKey);


            mouseHook.MouseWheel += new MouseEventHandler(mouseHook_MouseWheel);
            keyboardHook.KeyDown += new KeyEventHandler(keyboardHook_KeyDown);
            keyboardHook.KeyUp += new KeyEventHandler(keyboardHook_KeyUp);

            mouseHook.Start();
            keyboardHook.Start();

        }

        void mouseHook_MouseWheel(object sender, MouseEventArgs e)
        {
            Rectangle rectangle = panelScroll.RectangleToScreen(panelScroll.ClientRectangle);
            if (rectangle.Contains(Control.MousePosition))
            {
                if (!isKeyDownCtrl)
                {
                    if (e.Delta > 0)
                    {
                        if (panelScroll.VerticalScroll.Value == 0)
                            return;

                        if (panelScroll.VerticalScroll.Value <= 100)
                        {
                            panelScroll.VerticalScroll.Value = 0;
                            panelScroll.VerticalScroll.Value = 0;
                            return;
                        }

                        panelScroll.VerticalScroll.Value -= 100;
                        panelScroll.VerticalScroll.Value -= 100;
                    }
                    else
                    {

                        if (panelScroll.VerticalScroll.Value == panelScroll.VerticalScroll.Maximum)
                            return;

                        if (panelScroll.VerticalScroll.Value + 100 >= panelScroll.VerticalScroll.Maximum)
                        {
                            panelScroll.VerticalScroll.Value = panelScroll.VerticalScroll.Maximum;
                            panelScroll.VerticalScroll.Value = panelScroll.VerticalScroll.Maximum;
                            return;
                        }

                        panelScroll.VerticalScroll.Value += 100;
                        panelScroll.VerticalScroll.Value += 100;

                    }
                }
                else
                {
                    scale = printPage.GetScale();

                    if (e.Delta > 0)
                    {  
                        if (scale - 0.1f < 0.3f)
                            scale = 0.3f;
                        else
                            scale -= 0.1f;
                    }
                    else
                    {
                        scale += 0.1f;
                    }

                    ResetPageSize(SizeF.Empty, scale);   
                }
            }
        }

        void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LControlKey ||
                e.KeyCode == Keys.RControlKey)
            {
                isKeyDownCtrl = true;
            }
        }

        void keyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LControlKey ||
               e.KeyCode == Keys.RControlKey)
            {
                isKeyDownCtrl = false;
            }
        }


        public void ResetPageSize(SizeF newPageSize, float newScale)
        {
            if(newPageSize != SizeF.Empty)
            {
                pageSize = newPageSize;
                printPage.ResetSize(newPageSize);
                print.SetPageSize(newPageSize);
            }

            if(newScale < 0)
            {
                newScale = scale;
            }

            printPage.ResetScale(newScale);
            ResetPanelViewSize(pageSize, newScale);
            ResetViewLocation();

            DrawToImage(panelView.Width, panelView.Height);
            panelView.BackgroundImage = reportImage;
        }


        public void SetPrintPage(PrintPageBase _printPage, SizeF pageSize)
        {
            this.printPage = _printPage;
            this.pageSize = pageSize;

            ResetPanelViewSize(pageSize, 1.0f);
            ResetViewLocation();
            printPage.Init(pageSize.Width, pageSize.Height, 1.0f, panelView, monitorInfo);

            print.SetPageSize(pageSize);
            DrawToImage(panelView.Width, panelView.Height);

            panelView.BackgroundImage = reportImage;
        }


        void ResetPanelViewSize(SizeF pageSize, float scale)
        {
            panelView.Width = (int)(pageSize.Width * monitorInfo.permmWidth * scale);
            panelView.Height = (int)(pageSize.Height * monitorInfo.permmHeight * scale);
        }

        void ResetViewLocation()
        {          
            float x = 0;
            float y = 0;

            if(panelView.Width < panelScroll.Width)
               x = (panelScroll.Width - panelView.Width) / 2f;

            if (panelView.Height < panelScroll.Height)
                y = (panelScroll.Height - panelView.Height) / 2f;

            panelView.Location = new Point((int)x, (int)y);
        }

        Rectangle GetRealPageBounds(PrintPageEventArgs e, bool preview)
        {
            if (preview)
                return e.MarginBounds;

            float cx = e.PageSettings.HardMarginX;
            float cy = e.PageSettings.HardMarginY;
            float dpix = e.Graphics.DpiX;
            float dpiy = e.Graphics.DpiY;
            Rectangle marginBounds = e.MarginBounds;
            marginBounds.Offset((int)Math.Round(-cx * 100 / dpix), (int)Math.Round(-cy * 100 / dpiy));
            return marginBounds;
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;// 指定高质量、低速度呈现。
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            RectangleF realMarginBounds = GetRealPageBounds(e, false);
       
            printPage.SetPrintMode(PrintDrawMode.PRINTER_MODE, realMarginBounds);
            printPage.Draw(g);
        }

        private void panelView_Paint(object sender, PaintEventArgs e)
        {  
            /*
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;// 指定高质量、低速度呈现。
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            printPage.SetPrintMode(PrintDrawMode.SCREEN_MODE, RectangleF.Empty);
            printPage.Draw(e);
            */
        }

        private void panelScroll_SizeChanged(object sender, EventArgs e)
        {
            panelScroll.VerticalScroll.Value = 0;
            panelScroll.HorizontalScroll.Value = 0;
            ResetViewLocation();
        }


        void DrawToImage(int w, int h)
        {
            if(reportImage != null)
                reportImage.Dispose();

            reportImage = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(reportImage);
            g.SmoothingMode = SmoothingMode.HighQuality;// 指定高质量、低速度呈现。
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            printPage.SetPrintMode(PrintDrawMode.IMGEXPROT_MODE, RectangleF.Empty);
            printPage.Draw(g);
        }

        public void RePaint()
        {
            panelView.Refresh();
        }


        public void Reset()
        {
            scale = printPage.GetScale();
            ResetPageSize(SizeF.Empty, scale);
        }

        public void PrintDocument()
        {
            print.PrintDocument();
        }

        public void SetControlsDoubleBuffer(Control parentCtl)
        {
            foreach (Control c in parentCtl.Controls)
            {
                c.GetType().GetProperty(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic).SetValue(c, true, null);

                if (c.Controls.Count > 0)
                    SetControlsDoubleBuffer(c);//窗体内其余控件还可能嵌套控件(比如panel),要单独抽出,因为要递归调用
            }
        }
    }
}
