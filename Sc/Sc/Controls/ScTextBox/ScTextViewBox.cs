using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ScTextViewBox : ScLayer
    {
        float downWrapperX;
        float downWrapperLocationX;

        ScTextView textView;



        public delegate void TextViewKeyDownEventHandler(object sender, KeyEventArgs e);
        public event TextViewKeyDownEventHandler TextViewKeyDownEvent = null;

        public delegate void ValueChangedEventHandler(object sender);
        public event ValueChangedEventHandler ValueChangedEvent = null;

        public delegate void TextViewLostFocusEventHandler(object sender, EventArgs e);
        public event TextViewLostFocusEventHandler TextViewLostFocusEvent = null;

        public string BackGroundText
        {
            get { return textView.BackGroundText; }
            set
            {
                textView.BackGroundText = value;
            }
        }


        public Color BackGroundTextColor
        {
            get { return textView.BackGroundTextColor; }
            set
            {
                textView.BackGroundTextColor = value;
            }
        }


        public ScTextViewBox(ScMgr scmgr = null)
             : base(scmgr)
        {

            textView = new ScTextView(scmgr);
            textView.IsMultipleRow = false;
            Add(textView);

            MouseMove += TextBox_MouseMove;
            MouseDown += TextBox_MouseDown;
            SizeChanged += ScTextViewBox_SizeChanged;

            textView.CursorPositionEvent += TextView_CursorPositionEvent;       
            textView.KeyDown += TextView_KeyDown;
            textView.TextViewLostFocusEvent += TextView_TextViewLostFocusEvent;
            textView.TextViewValueChangedEvent += TextView_TextViewValueChangedEvent;

            IsHitThrough = true;
        }

        private void TextView_TextViewLostFocusEvent(object sender, EventArgs e)
        {
            if (TextViewLostFocusEvent != null)
                TextViewLostFocusEvent(this, e);
        }



        private void ScTextViewBox_SizeChanged(object sender, SizeF oldSize)
        {
            if(textView.Width < Width)
                textView.Width = Width;
        }

        private void TextView_TextViewValueChangedEvent(object sender)
        {
            if (ValueChangedEvent != null)
                ValueChangedEvent(this);
        }

        private void TextView_KeyDown(object sender, KeyEventArgs e)
        {
            if(TextViewKeyDownEvent != null)
                TextViewKeyDownEvent(sender, e);
        }

        public string Text
        {
            get { return textView.Text; }
            set { textView.Text = value; }
        }


        private void TextView_CursorPositionEvent(RectangleF cursorRect)
        {

            if (textView.Location.X <= 0 && textView.Width <= Width)
            {
                textView.Location = new PointF(0, textView.Location.Y);
                Refresh();
            }
            else if (textView.Location.X + textView.Width < Width && textView.Width > Width)
            {
                textView.Location = new PointF(Width - textView.Width, textView.Location.Y);
                Refresh();
            }
            else if(cursorRect.Right >= Width)
            {
                float x = textView.Location.X  - (cursorRect.Right - Width + 20);

                if (x + textView.Width < Width)
                    x = Width - textView.Width;

                textView.Location = new PointF(x, textView.Location.Y);
                Refresh();
            }
        }

        public D2DFont ForeFont
        {
            get
            {
                return textView.ForeFont;
            }
            set
            {
                textView.ForeFont = value;
                Height = textView.Height;
            }
        }


        public Color ForeColor
        {
            get
            {
                return textView.ForeColor;
            }
            set
            {
                textView.ForeColor = value;
            }
        }


        public float BoxWidth
        {   
            set
            {
                textView.boxWidth = value;
            }
        }

        public bool IsOnlyRead
        {
            get
            {
                return textView.isOnlyRead;
            }
            set
            {
                textView.isOnlyRead = value;
            }
        }

   

        private void TextBox_MouseMove(object sender, ScMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (textView.Width <= Width ||( e.Location.X < Width && e.Location.X >= 0))
                    return;

                float offsetX;

                if (e.Location.X >= Width)
                {
                    offsetX = -(e.Location.X - Width);
                }
                else
                {
                    offsetX = -e.Location.X;
                }

                Move(offsetX);
                Refresh();
            }
        }

        void Move(float offsetX)
        {
            float x = downWrapperLocationX + offsetX;

            if (x > 0)
            {
                x = 0;
                offsetX = -downWrapperLocationX;
            }
            else if (x + textView.Width < Width)
            {
                x = Width - textView.Width;
                offsetX = Height - (downWrapperLocationX + textView.Width);
            }

            textView.Location = new PointF(x, textView.Location.Y);
        }

        private void TextBox_MouseDown(object sender, ScMouseEventArgs e)
        {
            downWrapperX = e.Location.X;
            downWrapperLocationX = textView.Location.X;
        }

    }
}
