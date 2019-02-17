using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScWarningInfoBox: ScLayer
    {
        Table mainTable;
        Table contentTable;
        public Margin margin = new Margin(10,10,10,10);

        ScLayer titleBtn;
        ScLayer closedBtn;
        ScLayer infoLayer;
        ScLayer okBtn;

        Color closedBtnColor = Color.White;
        Color okBtnColor = Color.SeaGreen;

        public string titleInfo = "警告";
        public Font titleFont = new Font("微软雅黑", 10);

        public string info = "";
        public Font infoFont = new Font("微软雅黑", 12);

        public string okBtnText = "确定";
        public Font okBtnFont = new Font("微软雅黑", 12);

        public delegate void ClosedEventHandler();
        public event ClosedEventHandler ClosedEvent;

        public ScWarningInfoBox()
        {
            titleBtn = new ScLayer();
            closedBtn = new ScLayer();
            infoLayer = new ScLayer();
            okBtn = new ScLayer();

            Add(titleBtn);
            Add(closedBtn);
            Add(infoLayer);
            Add(okBtn);

            titleBtn.GDIPaint += TitleBtn_GDIPaint;

            closedBtn.MouseEnter += ClosedBtn_MouseEnter;
            closedBtn.MouseLeave += ClosedBtn_MouseLeave;
            closedBtn.GDIPaint += ClosedBtn_GDIPaint;
            closedBtn.MouseDown += ClosedBtn_MouseDown;

            infoLayer.GDIPaint += InfoLayer_GDIPaint;

            okBtn.MouseEnter += OkBtn_MouseEnter;
            okBtn.MouseLeave += OkBtn_MouseLeave;
            okBtn.GDIPaint += OkBtn_GDIPaint;
            okBtn.MouseDown += OkBtn_MouseDown;

            SizeChanged += ScWarningInfoBox_SizeChanged;

        }

        private void OkBtn_MouseDown(object sender, ScMouseEventArgs e)
        {
            if(ClosedEvent != null)
                ClosedEvent();
        }

        private void ClosedBtn_MouseDown(object sender, ScMouseEventArgs e)
        {
            if (ClosedEvent != null)
                ClosedEvent();
        }

        private void TitleBtn_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;
            RectangleF rect = new RectangleF(0, 0, titleBtn.Width, titleBtn.Height);
            DrawUtils.LimitBoxDraw(graphis, titleInfo, titleFont, Brushes.WhiteSmoke, rect, DrawPositionType.Left, true, false, 0);
        }

        private void OkBtn_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            RectangleF rect = new RectangleF(0, 0, okBtn.Width, okBtn.Height);

            Brush brush = new SolidBrush(okBtnColor);
            graphis.FillRectangle(brush, rect);

            DrawUtils.LimitBoxDraw(graphis, okBtnText, okBtnFont,  Brushes.WhiteSmoke, rect, DrawPositionType.Center, true, false, 0);
        }

        private void OkBtn_MouseLeave(object sender)
        {
            okBtnColor = Color.SeaGreen;
            okBtn.Refresh();
        }

        private void OkBtn_MouseEnter(object sender, ScMouseEventArgs e)
        {
            okBtnColor = Color.DarkRed;
            okBtn.Refresh();
        }

        private void InfoLayer_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            RectangleF rect = new RectangleF(0, 0, infoLayer.Width, infoLayer.Height);
            SolidBrush brush = new SolidBrush(Color.WhiteSmoke);
            DrawUtils.LimitBoxDraw(graphis, info, infoFont, brush, rect, DrawPositionType.Left, true, true, 0);
        }

        private void ClosedBtn_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            RectangleF rect = new RectangleF(0, 0, closedBtn.Width, closedBtn.Height);
            Pen pen = new Pen(closedBtnColor, 3);
            graphis.DrawLine(pen, 4, 4, rect.Right - 4, rect.Bottom - 4);
            graphis.DrawLine(pen, rect.Right - 4, rect.Top + 4, rect.Left + 4, rect.Bottom - 4);
        }

        private void ClosedBtn_MouseLeave(object sender)
        {
            closedBtnColor = Color.White;
            closedBtn.Refresh();
        }

        private void ClosedBtn_MouseEnter(object sender, ScMouseEventArgs e)
        {
            closedBtnColor = Color.Red;
            closedBtn.Refresh();
        }

        private void ScWarningInfoBox_SizeChanged(object sender, SizeF oldSize)
        {
            ReCreateTable();

            //
            RectangleF r = contentTable.GetCellContentRect(0, 0);
            titleBtn.Width = 200;
            titleBtn.Height = r.Height;
            titleBtn.Location = new PointF(r.Left + 10, r.Top);

            //
            r = contentTable.GetCellContentRect(0, 0);
            closedBtn.Width = r.Height;
            closedBtn.Height = r.Height;
            closedBtn.Location = new PointF(r.Right - closedBtn.Width, r.Top);

            //
            r = contentTable.GetCellContentRect(1, 0);
            infoLayer.Width = r.Width - 40;
            infoLayer.Height = r.Height - 20;
            infoLayer.Location = new PointF(r.Left + r.Width/2 - infoLayer.Width /2, r.Top + r.Height / 2 - infoLayer.Height / 2);

            //
            r = contentTable.GetCellContentRect(2, 0);
            okBtn.Width = 80;
            okBtn.Height = r.Height - 15;
            okBtn.Location = new PointF(r.Right - okBtn.Width - 15, r.Top + r.Height / 2 - okBtn.Height/2);
        }


        void ReCreateTable()
        {
            RectangleF r = new RectangleF(0, 0, Width, Height);

            mainTable = new Table(r, 1, 1);
            mainTable.SetDefaultCellMargin(margin);

            contentTable = new Table(mainTable.GetCellContentRect(0, 0), 3, 1);

            TableLine tableLine = new TableLine(LineDir.HORIZONTAL);
            tableLine.lineComputeMode = LineComputeMode.ABSOLUTE;

            tableLine.computeParam = 20;
            contentTable.SetLineArea(0, tableLine);

            tableLine.computeParam = 40;
            contentTable.SetLineArea(2, tableLine);

            tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;
            tableLine.computeParam = 1f;
            contentTable.SetLineArea(1, tableLine);

            contentTable.ComputeLinesArea(LineDir.HORIZONTAL);
        }
    }
}
