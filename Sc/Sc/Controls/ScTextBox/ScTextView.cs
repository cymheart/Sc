using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ScTextView : ScLayer
    {
        public delegate void CursorPositionEventHandler(RectangleF cursorRect);
        public event CursorPositionEventHandler CursorPositionEvent;

        public delegate void TextViewValueChangedEventHandler(object sender);
        public event TextViewValueChangedEventHandler TextViewValueChangedEvent = null;


        public delegate void TextViewLostFocusEventHandler(object sender, EventArgs e);
        public event TextViewLostFocusEventHandler TextViewLostFocusEvent = null;


        SolidColorBrush SceneColorBrush;
        ScTextRenderer scTextRenderer;



        string txt = "";
        string selectedTxt = "";
        D2DGraphics d2d;


        public string BackGroundText = "";
        public Color BackGroundTextColor = Color.Gray;
        bool isHideBackGroundText = false;


        HitTestMetrics[] hitTestMetrics = new HitTestMetrics[2];
        RawBool[] isTrailingHit = new RawBool[2];
        RawBool[] isInside = new RawBool[2];

        HitTestMetrics[] metricsList;

        int cursorState = 0;
        int cursorIdx = 0;

        public bool isOnlyRead = false;
        bool isMultipleRow;
        bool isDownCtrlKey = false;

        public float boxWidth = 0;

        ScAnimation scAnim;
        D2DFont foreFont;
        Color foreColor;

        public TextFormat CurrentTextFormat { get; private set; }
        public TextLayout CurrentTextLayout { get; private set; }
        public TextRange CurrentTextRange { get { return new TextRange(0, txt.Length); } }

        private ColorDrawingEffect RedDrawingeffect { get; set; }
        private ColorDrawingEffect BlueDrawingEffect { get; set; }
        private ColorDrawingEffect GreenDrawingEffect { get; set; }


        public ScTextView(ScMgr scmgr = null)
            :base(scmgr)
        {
            foreFont.FamilyName = "宋体";
            foreFont.Size = 24;
            foreFont.Style = SharpDX.DirectWrite.FontStyle.Normal;
            foreColor = Color.Red;

            LostFocus += TxtView_LostFocus;
            GotFocus += TxtView_GotFocus;
            MouseDown += ScTextView_MouseDown;
            MouseMove += TxtView_MouseMove;
            MouseUp += ScTextView_MouseUp;
            KeyDown += TxtView_KeyDown;
            KeyUp += ScTextView_KeyUp;
            D2DPaint += TxtView_D2DPaint;
            CharEvent += TxtView_CharEvent;
            ImeStringEvent += TxtView_ImeStringEvent;

            Cursor = Cursors.IBeam;
            IsHitThrough = true;


            //
            scAnim = new ScAnimation(this, -1, true);
            scAnim.AnimationEvent += ScAnim_AnimationEvent;
            scAnim.DurationMS = 500;


            ReBulid += ScTextView_ReBulid;
        }

        public override void Dispose()
        {
            scAnim.Dispose();
            base.Dispose();
        }

        private void ScTextView_ReBulid(object sender)
        {
            scTextRenderer.Dispose();
            Text = txt;
        }

        public bool IsOnlyRead
        {
            get
            {
                return isOnlyRead;
            }

            set
            {
                isOnlyRead = value;
            }
        }

        public bool IsMultipleRow
        {
            get
            {
                return isMultipleRow;
            }
            set
            {
                isMultipleRow = value;
            }
        }


        public D2DFont ForeFont
        {
            get
            {
                return foreFont;
            }
            set
            {
                foreFont = value;
                CreateD2D();
                InitTextFormatLayout();
            }
        }

        public Color ForeColor
        {
            get
            {
                return foreColor;
            }
            set
            {
                foreColor = value;
            }
        }

        private void ScTextView_MouseDown(object sender, ScMouseEventArgs e)
        {
            metricsList = null;
            Focus();

            hitTestMetrics[0] = CurrentTextLayout.HitTestPoint(
                 (float)e.Location.X,
                 (float)e.Location.Y,
                 out isTrailingHit[0],
                 out isInside[0]);

            hitTestMetrics[1] = hitTestMetrics[0];
            isInside[1] = isInside[0];
            isTrailingHit[1] = isTrailingHit[0];
            cursorIdx = 0;

            int x = (int)Math.Round(hitTestMetrics[cursorIdx].Left);
            int y = (int)Math.Round(hitTestMetrics[cursorIdx].Top);
            SetImeWindowsPos(x, y);



            StopAnim();
            StartAnim();
            Refresh();
        }


        private void ScTextView_MouseUp(object sender, ScMouseEventArgs e)
        {
            if (metricsList == null)
                selectedTxt = "";
        }

        private void ScTextView_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsOnlyRead)
                return;

            if (e.KeyCode == Keys.ControlKey)
            {
                isDownCtrlKey = false;
            }
        }

        public string Text
        {
            get
            {
                return txt;
            }

            set
            {
                txt = "";
                CreateD2D();
                InitTextFormatLayout();

                hitTestMetrics[0] = CurrentTextLayout.HitTestPoint(
                0,0,
                out isTrailingHit[0],
                out isInside[0]);

                hitTestMetrics[1] = hitTestMetrics[0];
                isInside[1] = isInside[0];
                isTrailingHit[1] = isTrailingHit[0];
                cursorIdx = 0;



                if (value == null)
                    value = "";

                AddNewText(value);
            }
        }



        private void TxtView_ImeStringEvent(object sender, string imeString)
        {
            if (IsOnlyRead)
                return;

            AddNewText(imeString);
        }


        private void TxtView_CharEvent(object sender, char c)
        {
            if (IsOnlyRead)
                return;

            AddNewText(c.ToString());
        }


        void AddNewText(string newText)
        {
            HitTestMetrics start, end;
            RawBool startTrailingHit, endTrailingHit;
            int startTextPosition, endTextPosition;
            string a = "", b = "";
            int newHitIdx = 0;

            if (hitTestMetrics[1].TextPosition == hitTestMetrics[0].TextPosition &&
              (isTrailingHit[1] == false && isTrailingHit[0] == true) ||
              hitTestMetrics[1].TextPosition < hitTestMetrics[0].TextPosition)
            {
                start = hitTestMetrics[1];
                startTextPosition = start.TextPosition;
                startTrailingHit = isTrailingHit[1];

                end = hitTestMetrics[0];
                endTextPosition = end.TextPosition;
                endTrailingHit = isTrailingHit[0];
            }
            else
            {
                start = hitTestMetrics[0];
                startTextPosition = start.TextPosition;
                startTrailingHit = isTrailingHit[0];

                end = hitTestMetrics[1];
                endTextPosition = end.TextPosition;
                endTrailingHit = isTrailingHit[1];
            }

            if(string.IsNullOrWhiteSpace(txt))
            {
                a = "";
                b = "";
            }
            else if (startTextPosition == endTextPosition && startTrailingHit == false  && endTrailingHit == false)  //点击前端
            {
                int pos = startTextPosition;
                a = txt.Substring(0, pos);
                b = txt.Substring(pos, txt.Length - pos);
            }
            else if(startTextPosition == endTextPosition && startTrailingHit == true && endTrailingHit == false) //点击后端
            {
                int pos = startTextPosition + 1;
                a = txt.Substring(0, pos);
                b = txt.Substring(pos, txt.Length - pos);
            }
            else if (startTrailingHit == true && endTrailingHit == true) //开始位置"后端", 结束位置"后端"
            {
                a = txt.Substring(0, startTextPosition + 1);
                b = txt.Substring(endTextPosition + 1, txt.Length - endTextPosition - 1);
            }
            else if (startTrailingHit == true && endTrailingHit == false) //开始位置"后端", 结束位置"前端"
            {
                a = txt.Substring(0, startTextPosition + 1);
                b = txt.Substring(endTextPosition, txt.Length - endTextPosition);
            }
            else if (startTrailingHit == false && endTrailingHit == true) //开始位置"前端", 结束位置"后端"
            {
                a = txt.Substring(0, startTextPosition);
                b = txt.Substring(endTextPosition + 1, txt.Length - endTextPosition - 1);
            }
            else if (startTrailingHit == false && endTrailingHit == false) //开始位置"前端", 结束位置"前端"
            {
                a = txt.Substring(0, startTextPosition);
                b = txt.Substring(endTextPosition, txt.Length - endTextPosition);
            }

            RectangleF oldrc = DrawBox;

            txt = a + newText + b;

            CreateD2D();
            InitTextFormatLayout();

            newHitIdx = a.Count() + newText.Count();

            if (newHitIdx < 0)
                newHitIdx = 0;

            metricsList = CurrentTextLayout.HitTestTextRange(newHitIdx, 1, 0.0f, 0.0f);

            hitTestMetrics[1] = hitTestMetrics[0] = metricsList[0];
            isInside[1] = isInside[0];
            isTrailingHit[1] = isTrailingHit[0] = false;
            cursorIdx = 0;

            metricsList = null;

            int x = (int)Math.Round(hitTestMetrics[cursorIdx].Left);
            int y = (int)Math.Round(hitTestMetrics[cursorIdx].Top);
            SetImeWindowsPos(x, y);


            if (CursorPositionEvent != null)
            {
                RectangleF rect =
                    new RectangleF(
                        hitTestMetrics[cursorIdx].Left,
                         hitTestMetrics[cursorIdx].Top,
                         hitTestMetrics[cursorIdx].Width,
                         hitTestMetrics[cursorIdx].Height);

                CursorPositionEvent(rect);
            }

            if (TextViewValueChangedEvent != null)
                TextViewValueChangedEvent(this);


            InvalidateGlobalRect(GDIDataD2DUtils.UnionRectF(oldrc, DrawBox));
        }

        private void TxtView_GotFocus(object sender, EventArgs e)
        {
            isHideBackGroundText = true;
            StartAnim();
            Refresh();
        }


        private void TxtView_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsOnlyRead)
                return;

            if (e.KeyCode == Keys.Back)
            {
                Backspace();
            }
            else if (e.KeyCode == Keys.ControlKey)
            {
                isDownCtrlKey = true;
            }
            else if (e.KeyCode == Keys.Left)
            {
                DownDirBtn(-1);
            }
            else if (e.KeyCode == Keys.Right)
            {
                DownDirBtn(1);
            }

            if (e.KeyCode == Keys.V && isDownCtrlKey)
            {
                string txt;
                IDataObject iData = Clipboard.GetDataObject();

                if (iData.GetDataPresent(DataFormats.Text))
                {
                    txt = (string)iData.GetData(DataFormats.Text);
                    AddNewText(txt);
                }
            }
            else if (e.KeyCode == Keys.C && isDownCtrlKey)
            {
                Clipboard.SetDataObject(selectedTxt);
            }

        }

        void Backspace()
        {
            if (hitTestMetrics[1].TextPosition == hitTestMetrics[0].TextPosition &&
                isTrailingHit[1] == isTrailingHit[0])
            {
                if (isTrailingHit[1] == true)
                {
                    isTrailingHit[1] = false;
                }
                else
                {
                    if (hitTestMetrics[1].TextPosition != 0)
                        hitTestMetrics[1].TextPosition--;
                }
            }

            AddNewText("");
        }

        void DownDirBtn(int dir)
        {
            metricsList = null;
 
            if (dir == -1)
            {
                if (isTrailingHit[0] == false)
                {
                    if (hitTestMetrics[0].TextPosition != 0)
                    {
                        metricsList = CurrentTextLayout.HitTestTextRange(hitTestMetrics[0].TextPosition -1, 1, 0.0f, 0.0f);
                        hitTestMetrics[0] = metricsList[0];
                    }
                }
                else
                {
                    isTrailingHit[0] = false;
                }
            }
            else
            {
                if (isTrailingHit[0] == false)
                {
                    isTrailingHit[0] = true;
                }
                else
                {
                    if (hitTestMetrics[0].TextPosition != txt.Count() - 1)
                    {
                        metricsList = CurrentTextLayout.HitTestTextRange(hitTestMetrics[0].TextPosition + 1, 1, 0.0f, 0.0f);
                        hitTestMetrics[0] = metricsList[0];
                    }
                }
            }

            hitTestMetrics[1] = hitTestMetrics[0];
            isTrailingHit[1] = isTrailingHit[0];
            cursorIdx = 0;

            int x = (int)Math.Round(hitTestMetrics[cursorIdx].Left);
            int y = (int)Math.Round(hitTestMetrics[cursorIdx].Top);
            SetImeWindowsPos(x, y);

            StopAnim();
            StartAnim();
            Refresh();
        }

        private void TxtView_LostFocus(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txt))
                isHideBackGroundText = false;

            StopAnim();

            if (TextViewLostFocusEvent != null)
                TextViewLostFocusEvent(sender, e);

            Refresh();
        }


        private void TxtView_MouseMove(object sender, ScMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                hitTestMetrics[1] = CurrentTextLayout.HitTestPoint(
                 (float)e.Location.X,
                 (float)e.Location.Y,
                 out isTrailingHit[1],
                 out isInside[1]);

                HitTestMetrics start, end;
                RawBool startTrailingHit, endTrailingHit;
                int startTextPosition, endTextPosition;

                if (hitTestMetrics[1].TextPosition == hitTestMetrics[0].TextPosition &&
                    (isTrailingHit[1] == false && isTrailingHit[0] == true) ||
                    hitTestMetrics[1].TextPosition < hitTestMetrics[0].TextPosition)
                {
                    start = hitTestMetrics[1];
                    startTextPosition = start.TextPosition;
                    startTrailingHit = isTrailingHit[1];

                    end = hitTestMetrics[0];
                    endTextPosition = end.TextPosition;
                    endTrailingHit = isTrailingHit[0];
                }  
                else
                {
                    start = hitTestMetrics[0];
                    startTextPosition = start.TextPosition;
                    startTrailingHit = isTrailingHit[0];

                    end = hitTestMetrics[1];
                    endTextPosition = end.TextPosition;
                    endTrailingHit = isTrailingHit[1];
                }


                if (startTrailingHit == true)
                    startTextPosition += 1;

                if (endTrailingHit == false)
                    endTextPosition -= 1;

                int len = endTextPosition - startTextPosition + 1;

                if (len > 0)
                    metricsList = CurrentTextLayout.HitTestTextRange(startTextPosition, len, 0.0f, 0.0f);
                else
                    metricsList = null;

                if (metricsList != null)
                {
                    cursorIdx = 1;
                    StopAnim();
                    StartAnim();
                    Refresh();
                }
            }
        }

        public void CreateD2D()
        {
            d2d = (D2DGraphics)ScMgr.Graphics;

            if (d2d != null)
            {
                scTextRenderer = new ScTextRenderer(D2DGraphics.d2dFactory, d2d.renderTarget);
            }
        }
        private void InitTextFormatLayout()
        {
            if (d2d == null)
                return;

            TextMetrics metrics;
            SizeF minSize;

            CurrentTextFormat = new TextFormat(D2DGraphics.dwriteFactory, foreFont.FamilyName, foreFont.Size);
            //{ TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center};

            CurrentTextFormat.WordWrapping = WordWrapping.NoWrap;

            CurrentTextLayout = new TextLayout(D2DGraphics.dwriteFactory, txt, CurrentTextFormat, 0, 0);

            metrics = CurrentTextLayout.Metrics;
            minSize = new SizeF(metrics.WidthIncludingTrailingWhitespace, metrics.Height);

            float w = (float)Math.Ceiling(minSize.Width);

            if (w > boxWidth)
                Width = w;
            else
                Width = boxWidth;

            Height = (float)Math.Ceiling(minSize.Height);




            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(foreColor);
            //RedDrawingeffect = new ColorDrawingEffect(rawColor);
            //CurrentTextLayout.SetDrawingEffect(RedDrawingeffect, new TextRange(0, txt.Count() - 1));

            SolidColorBrush brush = new SolidColorBrush(d2d.renderTarget, rawColor);
            CurrentTextLayout.SetDrawingEffect(brush, new TextRange(0, txt.Count()));



            //BlueDrawingEffect = new ColorDrawingEffect(SharpDX.Color.Blue);
            //GreenDrawingEffect = new ColorDrawingEffect(SharpDX.Color.Green);

            //CurrentTextLayout.SetFontSize(10.0f, new TextRange(6, 14));

            //CurrentTextLayout.SetDrawingEffect(BlueDrawingEffect, new TextRange(14, 7));
            //CurrentTextLayout.SetDrawingEffect(GreenDrawingEffect, new TextRange(21, 8));
            //CurrentTextLayout.SetUnderline(true, new TextRange(0, 20));
            //CurrentTextLayout.SetStrikethrough(true, new TextRange(22, 7));


            //SolidColorBrush greenBrush = new SolidColorBrush(d2d.renderTarget, SharpDX.Color.Black);
            //CurrentTextLayout.SetDrawingEffect(greenBrush, new TextRange(10, 1));

            /*
            // Set a stylistic typography
            using (var typo = new Typography(d2d.dwriteFactory))
            {
                typo.AddFontFeature(new FontFeature(FontFeatureTag.StylisticSet7, 1));
                CurrentTextLayout.SetTypography(typo, CurrentTextRange);
            }
            */
        }


        private void TxtView_D2DPaint(D2DGraphics g)
        {
            if(!string.IsNullOrWhiteSpace(BackGroundText) && !isHideBackGroundText)
            {
                g.RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
                SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, GDIDataD2DUtils.TransToRawColor4(BackGroundTextColor));
                TextFormat textFormat = new TextFormat(D2DGraphics.dwriteFactory, foreFont.FamilyName, foreFont.Weight, foreFont.Style, foreFont.Size)
                { TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Center };

                textFormat.WordWrapping = WordWrapping.Wrap;

                SharpDX.RectangleF textRegionRect =
                     new SharpDX.RectangleF(0,0, Width -1, Height - 1);

                g.RenderTarget.DrawText(BackGroundText, textFormat, textRegionRect, brush, DrawTextOptions.Clip);
            }


            if(metricsList != null)
            {
                g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;


                SharpDX.Direct2D1.Brush brush = new SolidColorBrush(g.RenderTarget, SharpDX.Color.SkyBlue);

                foreach (HitTestMetrics m in metricsList)
                {
                   SharpDX.RectangleF textRegionRect =
                        new SharpDX.RectangleF((float)Math.Ceiling(m.Left) , (float)Math.Ceiling(m.Top), (float)Math.Ceiling(m.Width), (float)Math.Ceiling(m.Height));

                    g.RenderTarget.FillRectangle(textRegionRect, brush);
                }
            }

            g.RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;

            if (CurrentTextLayout != null && scTextRenderer != null)
                CurrentTextLayout.Draw(scTextRenderer, 0, 0);


            if (cursorState == 1)
            {
                g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;

                RawRectangleF rect =
                    new RawRectangleF(
                        hitTestMetrics[cursorIdx].Left,
                         hitTestMetrics[cursorIdx].Top,
                         hitTestMetrics[cursorIdx].Left + hitTestMetrics[cursorIdx].Width,
                         hitTestMetrics[cursorIdx].Top + hitTestMetrics[cursorIdx].Height);

                RawVector2 pt0;
                RawVector2 pt1;

                if (isTrailingHit[cursorIdx] == false)
                {
                    pt0 = new RawVector2((float)Math.Ceiling(rect.Left), (float)Math.Ceiling(rect.Top));
                    pt1 = new RawVector2((float)Math.Ceiling(rect.Left), (float)Math.Ceiling(rect.Bottom));
                }
                else
                {
                    pt0 = new RawVector2((float)Math.Ceiling(rect.Right), (float)Math.Ceiling(rect.Top));
                    pt1 = new RawVector2((float)Math.Ceiling(rect.Right), (float)Math.Ceiling(rect.Bottom));
                }

                RawColor4 color = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 255, 0, 0));
                SharpDX.Direct2D1.Brush brushx = new SolidColorBrush(g.RenderTarget, color);

                if(pt0.X == 0 && pt1.X == 0)
                    pt1.X = pt0.X = 1;
                
                g.RenderTarget.DrawLine(pt0, pt1, brushx);    
              }

        }

        public void StopAnim()
        {
            cursorState = 0;
            scAnim.Stop();
        }

        public void StartAnim()
        {
            cursorState = 1;
            scAnim.Start();
        }

        private void ScAnim_AnimationEvent(ScAnimation scAnimation)
        {
            if (cursorState == 0)
                cursorState = 1;
            else
                cursorState = 0;

            int x = (int)hitTestMetrics[cursorIdx].Left - 2;
            int y = (int)hitTestMetrics[cursorIdx].Top - 2;
            int w = (int)Math.Round(hitTestMetrics[cursorIdx].Width + 4);
            int h = (int)Math.Round(hitTestMetrics[cursorIdx].Height + 4);

            Rectangle rc = new Rectangle(x,y,w,h);
            Invalidate(rc);
        }
    }
}
