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
    public class ScVxSearch : ScLayer
    {
        Table mainTable;
        Font font = new Font("微软雅黑", 10);
        public Color bgColor = Color.FromArgb(220, 29, 39, 49);
        public Color txtColor = Color.FromArgb(255, 125, 175, 175);

        public Color searchColor = Color.FromArgb(255, 125, 175, 175);

        ScTextViewBox textBox;
        ScLayer search;

        public delegate void SearchEventHandler(string searchTxt);
        public event SearchEventHandler SearchEvent = null;


        public ScVxSearch()
        {
            textBox = new ScTextViewBox();
            textBox.ForeColor = Color.WhiteSmoke;
            textBox.ForeFont = new Font("微软雅黑", 15);
            Add(textBox);

            search = new ScLayer();
            Add(search);
           
            GDIPaint += ScVxDate_GDIPaint;
            SizeChanged += ScVxDate_SizeChanged;


            search.GDIPaint += Search_GDIPaint;
            search.MouseEnter += Search_MouseEnter;
            search.MouseLeave += Search_MouseLeave;

            search.MouseDown += Search_MouseDown;


            textBox.TextViewKeyDownEvent += TextBox_TextViewKeyDownEvent;
           
        }

        private void TextBox_TextViewKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (SearchEvent != null)
                    SearchEvent(textBox.Text);
            }
        }

        public void SetSerachEvent(SearchEventHandler searchEvent)
        {
            SearchEvent += searchEvent;
        }

        private void Search_MouseDown(object sender, ScMouseEventArgs e)
        {
            if (SearchEvent != null)
                SearchEvent(textBox.Text);
        }

        private void Search_MouseLeave(object sender)
        {
            searchColor = Color.FromArgb(255, 125, 175, 175);
            Refresh();
        }

        private void Search_MouseEnter(object sender, ScMouseEventArgs e)
        {
            searchColor = Color.FromArgb(255, 252, 241, 114);
            Refresh();
        }

        private void Search_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            RectangleF rect = new RectangleF(0, 0, Width, Height);
            RectangleF r = new RectangleF(rect.X + 8, rect.Y + 8, 12, 12);

            Pen pen = new Pen(searchColor, 2);
            graphis.DrawEllipse(pen, r);

            graphis.DrawLine(pen, rect.X + 18, rect.Y + 18, rect.X + 26, rect.Y + 26);

        }

        private void ScVxDate_SizeChanged(object sender, SizeF oldSize)
        {
            RectangleF rect = new RectangleF(0, 0, Width, Height);
            mainTable = new Table(rect, 1, 2);

            TableLine tableLine = new TableLine(LineDir.VERTICAL);
            tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;

            tableLine.computeParam = 0.85f;
            mainTable.SetLineArea(0, tableLine);

            tableLine.computeParam = 0.15f;
            mainTable.SetLineArea(1, tableLine);

            mainTable.ComputeLinesArea(LineDir.VERTICAL);

            //
            rect = mainTable.GetCellContentRect(0, 0);
            textBox.Width = rect.Width - 10;

            float x = rect.Width / 2 - textBox.Width / 2;
            float y = rect.Height / 2 - textBox.Height / 2;
            textBox.Location = new PointF(x, y);

            //
            rect = mainTable.GetCellContentRect(0, 1);
            search.Width = rect.Width;
            search.Height = rect.Height;
            search.Location = new PointF(rect.X, rect.Y);
        }

        private void ScVxDate_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;
            graphis.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            RectangleF rect = new RectangleF(0, 0, Width - 1, Height - 1);

            //
            Pen pen = new Pen(txtColor);
            DrawUtils.DrawRoundRectangle(graphis, pen, rect, 6);


            //
            LinePos linePos = mainTable.GetColLinePos(0);
            linePos.start = new PointF((int)linePos.start.X, (int)(linePos.start.Y + Height / 7));
            linePos.end = new PointF((int)linePos.end.X, (int)(linePos.end.Y - Height / 7));
            graphis.DrawLine(pen, linePos.start, linePos.end);

        }
    }
}
