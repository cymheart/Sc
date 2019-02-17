using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace PrintUtils
{
    public enum PrintDrawMode
    {
        PRINTER_MODE,
        SCREEN_MODE,
        IMGEXPROT_MODE
    }
    public class PrintPageBase
    {
        protected PrintDrawMode printMode;
        protected float permmWidth;
        protected float permmHeight;
        protected float widthScale = 0;
        protected float heightScale = 0;
        protected float offsetX = 0;
        protected float offsetY = 0;
        protected Control control;

        protected float orgPageWidth;
        protected float orgPageHeight;
        protected float pageWidth;
        protected float pageHeight;

        protected int pageCount = 1;
        protected int printPage = 1;

        protected Graphics g;
        protected float scale = 1.0f;

        public PrintPageBase()
        {

        }
        public PrintPageBase(float pageWidth, float pageHeight, float scale, Control control, MonitorInfo monitorInfo)
        {
            Init(pageWidth, pageHeight, scale, control, monitorInfo);
        }

        public void Init(float pageWidth, float pageHeight, float scale, Control control, MonitorInfo monitorInfo)
        {
            permmWidth = monitorInfo.permmWidth;
            permmHeight = monitorInfo.permmHeight;

            this.control = control;
            orgPageWidth = pageWidth;
            orgPageHeight = pageHeight;

            this.pageHeight = orgPageHeight * scale;
            this.pageWidth = orgPageWidth * scale;
            this.scale = scale;

            printMode = PrintDrawMode.SCREEN_MODE;
        }

        public void ResetScale(float scale)
        {
            if (scale < 0)
                return;

            pageHeight = orgPageHeight * scale;
            pageWidth = orgPageWidth * scale;
            this.scale = scale;
        }

        public void ResetSize(SizeF size)
        {
            if (size == SizeF.Empty)
                return;

            orgPageWidth = pageWidth;
            orgPageHeight = pageHeight;

            pageHeight = orgPageHeight * scale;
            pageWidth = orgPageWidth * scale;
        }

        public float GetScale()
        {
            return scale;
        }

        public int GetPageCount()
        {
            return pageCount;
        }

        public void SetPrintPage(int page)
        {
            printPage = page;
        }

        public void SetPrintMode(PrintDrawMode mode, RectangleF realMarginBounds)
        {
            printMode = mode;

            switch (printMode)
            {
                case PrintDrawMode.SCREEN_MODE:
                    pageWidth = orgPageWidth * scale;
                    pageHeight = orgPageHeight * scale;

                    widthScale = permmWidth;
                    heightScale = permmHeight;


                    if (realMarginBounds != RectangleF.Empty)
                    {
                        offsetX = realMarginBounds.X;
                        offsetY = realMarginBounds.Y;
                    }
                    else
                    {
                        offsetX = ((control.Width / permmWidth) / 2 - pageWidth / 2) * permmWidth;
                        offsetY = ((control.Height / permmHeight) / 2 - pageHeight / 2) * permmHeight;
                    }
                    break;

                case PrintDrawMode.IMGEXPROT_MODE:
                    pageWidth = orgPageWidth * scale;
                    pageHeight = orgPageHeight * scale;

                    widthScale = permmWidth;
                    heightScale = permmHeight;

                    if (realMarginBounds != RectangleF.Empty)
                    {
                        offsetX = realMarginBounds.X;
                        offsetY = realMarginBounds.Y;
                    }
                    else
                    {
                        offsetX = 0;
                        offsetY = 0;
                    }
                    break;

                case PrintDrawMode.PRINTER_MODE:
                    scale = 1.0f;
                    pageWidth = orgPageWidth * scale;
                    pageHeight = orgPageHeight * scale;

                    widthScale = 1.0f;
                    heightScale = 1.0f;

                    if (realMarginBounds != RectangleF.Empty)
                    {
                        offsetX = realMarginBounds.X;
                        offsetY = realMarginBounds.Y;
                    }
                    else
                    {
                        offsetX = 0;
                        offsetY = 0;
                    }
                    break;
            }
        }

        public virtual void Draw(Graphics g)
        {
            this.g = g;
            
            switch (printMode)
            {
                case PrintDrawMode.SCREEN_MODE:
                case PrintDrawMode.IMGEXPROT_MODE:
                    g.PageUnit = GraphicsUnit.Pixel;
                    g.ResetTransform();
                    break;

                case PrintDrawMode.PRINTER_MODE:
                    g.PageUnit = GraphicsUnit.Millimeter;
                    g.ResetTransform();
                    break;
            }
        }

        public virtual void Draw(EventArgs e)
        {
            switch (printMode)
            {
                case PrintDrawMode.SCREEN_MODE:
                case PrintDrawMode.IMGEXPROT_MODE:
                    g = ((PaintEventArgs)e).Graphics;
                    g.PageUnit = GraphicsUnit.Pixel;
                    g.ResetTransform();
                    break;

                case PrintDrawMode.PRINTER_MODE:
                    g = ((PrintPageEventArgs)e).Graphics;
                    g.PageUnit = GraphicsUnit.Millimeter;
                    g.ResetTransform();
                    break;
            }
        }
    }
}
