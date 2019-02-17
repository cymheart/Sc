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
    public class ScTextBox : ScLayer
    {
        ScTextViewBox textBox;

        public delegate void TextBoxKeyDownEventHandler(object sender, KeyEventArgs e);
        public event TextBoxKeyDownEventHandler TextBoxKeyDownEvent;

        public delegate void ValueChangedEventHandler(object sender, object value);
        public event ValueChangedEventHandler ValueChangedEvent = null;


        public ScTextBox(ScMgr scMgr = null)
        {
            Type = "ScTextBox";

            this.ScMgr = scMgr;

            textBox = new ScTextViewBox(scMgr);
            textBox.ForeColor = Color.Black;
            textBox.ForeFont = new Font("微软雅黑", 12);
            Add(textBox);

            SizeChanged += ScTextBox_SizeChanged;
            GDIPaint += ScTextBox_GDIPaint;

            textBox.TextViewKeyDownEvent += TextBox_TextViewKeyDownEvent;

            textBox.ValueChangedEvent += TextBox_ValueChangedEvent;
        }

        private void TextBox_ValueChangedEvent(object sender)
        {
            if (ValueChangedEvent != null)
                ValueChangedEvent(this, Text);
        }

        private void TextBox_TextViewKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (TextBoxKeyDownEvent != null)
                TextBoxKeyDownEvent(this, e);
        }

        public Color ForeColor
        {
            get
            {
                return textBox.ForeColor;
            }
            set
            {
                textBox.ForeColor = value;
            }
        }
        public string Text
        {
            get
            {
                return textBox.Text;
            }

            set
            {
                textBox.Text = value;
                Refresh();
            }
        }

        public bool IsOnlyRead
        {
            get
            {
                return textBox.IsOnlyRead;
            }
            set
            {
                textBox.IsOnlyRead = value;
            }
        }

        private void ScTextBox_GDIPaint(GDIGraphics g)
        {  
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;
            graphis.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;


            RectangleF rect = new RectangleF(0, 0, (float)Math.Ceiling(Width - 1), (float)Math.Ceiling(Height - 1));
            Pen pen = new Pen(Color.FromArgb(255, 191, 152, 90), 1f);
            DrawUtils.DrawRoundRectangle(graphis, pen, rect, 4);
  
        }

        private void ScTextBox_SizeChanged(object sender, SizeF oldSize)
        {
            RectangleF rect = new RectangleF(0, 0, Width, Height);
          
            textBox.Width = rect.Width - 10;
            float x = rect.Width / 2 - textBox.Width / 2;
            float y = rect.Height / 2 - textBox.Height / 2;
            textBox.Location = new PointF((float)Math.Ceiling(x), (float)Math.Ceiling(y));
        }
    }
}
