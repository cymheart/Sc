using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ScTextView : ScLayer
    {
        public Graphics g;
        string txt = "";
        string selectedTxt = "";
        List<RectangleF> txtHitRect = new List<RectangleF>();
        ScAnimation scAnim;

        Font foreFont = new Font("微软雅黑", 13);
        public Color ForeColor = Color.Black;

        GDIHitTestMetrics[] hitTestMetrics = new GDIHitTestMetrics[2];
        bool[] isTrailingHit = new bool[2];

        GDIHitTestMetrics[] metricsList;
        GDIHitTestMetrics[] rowMetricsList;

        int cursorState = 0;
        int cursorIdx = 0;

        bool isMultipleRow;
        public bool isOnlyRead = false;


        public delegate void CursorPositionEventHandler(RectangleF cursorRect);
        public event CursorPositionEventHandler CursorPositionEvent;

        public delegate void TextViewValueChangedEventHandler(object sender);
        public event TextViewValueChangedEventHandler TextViewValueChangedEvent = null;

        bool isDownCtrlKey = false;

        public ScTextView(ScMgr scMgr = null)
        {
            this.ScMgr = scMgr;
            LostFocus += TxtView_LostFocus;
            GotFocus += TxtView_GotFocus;
            MouseDown += ScTextBox_MouseDown;
            MouseMove += TxtView_MouseMove;
            MouseUp += TxtView_MouseUp;
            KeyDown += TxtView_KeyDown;
            KeyUp += ScTextView_KeyUp;
            GDIPaint += TxtView_GDIPaint;
            CharEvent += TxtView_CharEvent;
            ImeStringEvent += TxtView_ImeStringEvent;

            Cursor = Cursors.IBeam;

            IsHitThrough = true;
            

            //
            scAnim = new ScAnimation(this, -1, true);
            scAnim.AnimationEvent += ScAnim_AnimationEvent;
            scAnim.DurationMS = 500;
        }

      
        ~ScTextView()
        {
            scAnim.Stop();
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

                if(isMultipleRow == false)
                {
                    Size s = TextRenderer.MeasureText("x", ForeFont);
                    Height = s.Height;               
                }
            }
        }


        public Font ForeFont
        {
            get
            {
                return foreFont;
            }
            set
            {
                foreFont = value;

                if (isMultipleRow == false)
                {
                    Size s = TextRenderer.MeasureText("x", ForeFont);
                    Height = s.Height;
                }
            }
        }

        private void TxtView_MouseUp(object sender, ScMouseEventArgs e)
        {
            if (isOnlyRead)
                return;

            if (metricsList == null)
                selectedTxt = "";

            // txtView.SetCursor(Cursors.IBeam);
        }

        private void TxtView_ImeStringEvent(object sender, string imeString)
        {
            AddNewText(imeString);
        }


        private void TxtView_CharEvent(object sender, char c)
        {
            AddNewText(c.ToString());               
        }

        void AddNewText(string newText)
        {
            GDIHitTestMetrics start, end;
            bool startTrailingHit, endTrailingHit;
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


            if (string.IsNullOrWhiteSpace(txt))
            {
                txt = newText;
                startTextPosition = -1;
            }
            else
            {
                string a = txt.Substring(0, startTextPosition);
                string b = txt.Substring(endTextPosition + 1, txt.Length - endTextPosition - 1);
                txt = a + newText + b;
            }


            RectangleF oldrc = DrawBox;

         
            CreateHitInfo(txt);

            int n = 0;
            if (newText != null)
                n = newText.Count();

            if(startTextPosition == -1 || 
                startTextPosition + n >= txtHitRect.Count())
            {
                isTrailingHit[1] = isTrailingHit[0] = true;
            }
            else
            {
                isTrailingHit[1] = isTrailingHit[0] = false;
            }

            metricsList = HitTestTextRange(startTextPosition + n, 1);
            hitTestMetrics[1] = hitTestMetrics[0] = metricsList[0];
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

            InvalidateGlobalRect(GDIDataD2DUtils.UnionRectF(oldrc, DrawBox));
        }

        void CreateHitInfo(string value)
        {
            SizeF sizeF;

            bool ret = CreateHwnd();

            if (ret == false)
                return;

            if (isMultipleRow == false)
            {
                sizeF = g.MeasureString(value, ForeFont);


                if (sizeF.Width <= 100)
                    Width = 100;
                else
                    Width = sizeF.Width + 20;
            }
            else
            {
                sizeF = g.MeasureString(value, ForeFont, (int)Width);

                if (sizeF.Height <= 15)
                    Height = 15;
                else
                    Height = sizeF.Height;
            }

            RectangleF rect2 = new RectangleF(0, 0, Width, Height);
            MeasureCharacterRangesRegions(rect2);
        }

        private void TxtView_GotFocus(object sender, EventArgs e)
        {
            StartAnim();
            Refresh();
        }

        private void TxtView_KeyDown(object sender, KeyEventArgs e)
        {
            if (isOnlyRead)
                return;

            if (e.KeyCode == Keys.Back)
            {
                Backspace();
            }
            else if (e.KeyCode == Keys.ControlKey)
            {
                isDownCtrlKey = true;
            }    
            else if(e.KeyCode == Keys.Left)
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

        private void ScTextView_KeyUp(object sender, KeyEventArgs e)
        {
            if (isOnlyRead)
                return;

            if (e.KeyCode == Keys.ControlKey)
            {
                isDownCtrlKey = false;
            }
        }

        void Backspace()
        {
         if(hitTestMetrics[1].TextPosition == hitTestMetrics[0].TextPosition &&
                isTrailingHit[1] == isTrailingHit[0])
            {
                if (isTrailingHit[1] == true)
                {
                    isTrailingHit[1] = false;
                }
                else
                {
                    if(hitTestMetrics[1].TextPosition != 0)
                        hitTestMetrics[1].TextPosition--;
                }
            }

            AddNewText("");
        }

        private void TxtView_LostFocus(object sender, EventArgs e)
        {
            StopAnim();
            Refresh();
        }

        private void ScTextBox_MouseDown(object sender, ScMouseEventArgs e)
        {
            if (isOnlyRead)
                return;

            metricsList = null;
            Focus();

            hitTestMetrics[0] = HitTestPoint(e.Location, out isTrailingHit[0]);

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

        void DownDirBtn(int dir)
        {
            metricsList = null;
            int idx;

            if (dir == -1)
            {
                if (isTrailingHit[0] == false)
                {
                    if (hitTestMetrics[0].TextPosition != 0)
                    {
                        hitTestMetrics[0].TextPosition--;
                        idx = hitTestMetrics[0].TextPosition;
                        hitTestMetrics[0].Left = txtHitRect[idx].Left;
                        hitTestMetrics[0].Top = txtHitRect[idx].Top;
                        hitTestMetrics[0].Width = txtHitRect[idx].Width;
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
                        hitTestMetrics[0].TextPosition++;
                        idx = hitTestMetrics[0].TextPosition;
                        hitTestMetrics[0].Left = txtHitRect[idx].Left;
                        hitTestMetrics[0].Top = txtHitRect[idx].Top;
                        hitTestMetrics[0].Width = txtHitRect[idx].Width;
                        hitTestMetrics[0].Height = txtHitRect[idx].Height;
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


        private void TxtView_MouseMove(object sender, ScMouseEventArgs e)
        {
            if (isOnlyRead)
                return;

            if (e.Button == MouseButtons.Left)
            {
                hitTestMetrics[1] = HitTestPoint(e.Location, out isTrailingHit[1]);
      
                GDIHitTestMetrics start, end;
                bool startTrailingHit, endTrailingHit;
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

                if (string.IsNullOrWhiteSpace(txt))
                    selectedTxt = "";
                else
                   selectedTxt = txt.Substring(startTextPosition, len);

                if (len > 0)
                    metricsList = HitTestTextRange(startTextPosition, len);
                else
                    metricsList = null;

                cursorIdx = 1;
                StopAnim();
                StartAnim();
                Refresh();
            }
        }



        private void TxtView_GDIPaint(GDIGraphics g)
        {
            Graphics gdiGraph = g.GdiGraph;
            gdiGraph.SmoothingMode = SmoothingMode.HighQuality;// 指定高质量、低速度呈现。
            gdiGraph.TextRenderingHint = TextRenderingHint.SystemDefault;
            Brush brush;

            //brush = new SolidBrush(Color.Red);
            //RectangleF rectx = new RectangleF(0, 0, Width, Height);
            //gdiGraph.FillRectangle(brush, rectx);

            //if(txtHitRect.Count > 0)
            //    gdiGraph.DrawRectangles(Pens.Red, txtHitRect.ToArray());

            if (metricsList != null && Focused)
            {
                brush = new SolidBrush(Color.SkyBlue);

                foreach (GDIHitTestMetrics m in metricsList)
                {
                    RectangleF textRegionRect =
                         new RectangleF(m.Left, m.Top, m.Width, m.Height);

                    gdiGraph.FillRectangle(brush, textRegionRect);
                }
            }


            brush = new SolidBrush(ForeColor);
            RectangleF rect = new RectangleF(0, 0, Width, Height);
            gdiGraph.DrawString(txt, ForeFont, brush, rect);



            if (cursorState == 1 && Focused)
            {
                rect =
                    new RectangleF(
                        hitTestMetrics[cursorIdx].Left,
                         hitTestMetrics[cursorIdx].Top,
                         hitTestMetrics[cursorIdx].Width,
                         hitTestMetrics[cursorIdx].Height);


                PointF pt0;
                PointF pt1;

                if (isTrailingHit[cursorIdx] == false)
                {
 
                    pt0 = new PointF((float)Math.Ceiling(rect.Left), (float)Math.Ceiling(rect.Top));
                    pt1 = new PointF((float)Math.Ceiling(rect.Left), (float)Math.Ceiling(rect.Bottom));
                }
                else
                {
                    pt0 = new PointF((float)Math.Ceiling(rect.Right), (float)Math.Ceiling(rect.Top));
                    pt1 = new PointF((float)Math.Ceiling(rect.Right), (float)Math.Ceiling(rect.Bottom));
                }


                Color color = Color.FromArgb(255, 0, 0, 0);
                Pen pen = new Pen(color);
                gdiGraph.DrawLine(pen, pt0, pt1);

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
                AddNewText(value);

                if (TextViewValueChangedEvent != null)
                    TextViewValueChangedEvent(this);
            }
        }



        GDIHitTestMetrics[] HitTestTextRange(int textPosition, int textLength)
        {
            RectangleF baseRect;
            RectangleF rect;
            List<GDIHitTestMetrics> hitTextMetricsList = new List<GDIHitTestMetrics>();
            GDIHitTestMetrics hitTextMetrics;
            int startPos;

            if (txtHitRect.Count == 0)
            {
                SizeF sizeF;
                if (g != null)
                    sizeF = g.MeasureString("x", ForeFont);
                else
                    sizeF = TextRenderer.MeasureText("x", ForeFont);

                hitTextMetrics = new GDIHitTestMetrics()
                {
                    TextPosition = 0,
                    Left = 0,
                    Top = 0,
                    Width = 1,
                    Height = sizeF.Height,
                    Length = 1
                };

                hitTextMetricsList.Add(hitTextMetrics);
                return hitTextMetricsList.ToArray();
            }

            if(textPosition >= txtHitRect.Count)
            {
                textPosition = txtHitRect.Count() - 1; 
            }

            if (textPosition < 0)
            {
                textPosition = 0;
            }

            baseRect = txtHitRect[textPosition];
            startPos = textPosition;

            //
            for (int i = textPosition - 1; i >= 0; i--)
            {
                rect = txtHitRect[i];

                if ((rect.Top >= baseRect.Top &&
                    rect.Top < baseRect.Bottom) ||
                    (rect.Bottom >= baseRect.Bottom &&
                    rect.Bottom < baseRect.Top))
                {
                    if (rect.Top < baseRect.Top)
                    {
                        baseRect.Y = rect.Top;
                        baseRect.Height += baseRect.Top - rect.Top;
                    }

                    if (rect.Bottom > baseRect.Bottom)
                        baseRect.Height += rect.Bottom - baseRect.Bottom;
                }
            }


            //
            for (int i = textPosition + 1; i < textLength + textPosition; i++)
            {
                rect = txtHitRect[i];

                if ((rect.Top >= baseRect.Top && 
                    rect.Top < baseRect.Bottom) ||
                    (rect.Bottom >= baseRect.Bottom &&
                    rect.Bottom < baseRect.Top))
                {
                    if (rect.Top < baseRect.Top)
                    {
                        baseRect.Y = rect.Top;
                        baseRect.Height += baseRect.Top - rect.Top;
                    }

                    if (rect.Bottom > baseRect.Bottom)
                        baseRect.Height += rect.Bottom - baseRect.Bottom;

                    baseRect.Width = rect.Right - baseRect.Left;
                }
                else
                {
                    hitTextMetrics = new GDIHitTestMetrics()
                    {
                        TextPosition = startPos,
                        Left = baseRect.Left,
                        Top = baseRect.Top,
                        Width = baseRect.Width,
                        Height = baseRect.Height,
                        Length = i - startPos + 1
                    };

                    hitTextMetricsList.Add(hitTextMetrics);

                    startPos = i;
                    baseRect = txtHitRect[i];
                }
            }


            //
            for (int i = textLength + textPosition; i < txtHitRect.Count(); i++)
            {
                rect = txtHitRect[i];

                if ((rect.Top >= baseRect.Top &&
                    rect.Top < baseRect.Bottom) ||
                    (rect.Bottom >= baseRect.Bottom &&
                    rect.Bottom < baseRect.Top))
                {
                    if (rect.Top < baseRect.Top)
                    {
                        baseRect.Y = rect.Top;
                        baseRect.Height += baseRect.Top - rect.Top;
                    }

                    if (rect.Bottom > baseRect.Bottom)
                        baseRect.Height += rect.Bottom - baseRect.Bottom;
                }
                else
                {
                    break;
                }
            }

            hitTextMetrics = new GDIHitTestMetrics()
            {
                TextPosition = startPos,
                Left = baseRect.Left,
                Top = baseRect.Top,
                Width = baseRect.Width,
                Height = baseRect.Height,
                Length = textLength + textPosition - startPos
            };

            hitTextMetricsList.Add(hitTextMetrics);
            return hitTextMetricsList.ToArray();
        }

        GDIHitTestMetrics HitTestPoint(PointF pt, out bool isTrailingHit)
        {
            GDIHitTestMetrics hitTextMetrics = new GDIHitTestMetrics();
            int pos = 0;
            GDIHitTestMetrics m;
            isTrailingHit = false;
            
            //
            if (txtHitRect.Count == 0)
            {
                bool ret = CreateHwnd();

                if (ret == false)
                    return hitTextMetrics;

                SizeF sizeF;
                if (g != null)
                    sizeF = g.MeasureString("x", ForeFont);
                else
                    sizeF = TextRenderer.MeasureText("x", ForeFont);

                hitTextMetrics = new GDIHitTestMetrics()
                {
                    TextPosition = pos,
                    Left = 0,
                    Top = 0,
                    Width = 1,
                    Height = sizeF.Height,
                    Length = 1
                };

                return hitTextMetrics;
            }

            //if (rowMetricsList == null)
                rowMetricsList = ComputeRowMetricsList();

            //
            float h = rowMetricsList[rowMetricsList.Count() - 1].Top + rowMetricsList[rowMetricsList.Count() - 1].Height;
           
            for (int i = 0; i < rowMetricsList.Count(); i++)
            {
                m = rowMetricsList[i];
      
                if ((pt.Y >= m.Top && pt.Y <= (m.Top + m.Height)) || 
                    pt.Y < rowMetricsList[0].Top || pt.Y > h)
                {
                    if (pt.Y < rowMetricsList[0].Top)
                        m = rowMetricsList[0];
                    else if (pt.Y > h)
                        m = rowMetricsList[rowMetricsList.Count() - 1];

                    if (pt.X >= m.Left && pt.X <= (m.Left + m.Width))
                    {
                        for (int j = m.TextPosition; j < m.TextPosition + m.Length; j++)
                        {
                            if (pt.X >= txtHitRect[j].Left && 
                                pt.X <= (txtHitRect[j].Left + txtHitRect[j].Width))
                            {
                                float a = pt.X - txtHitRect[j].Left;
                                float b = txtHitRect[j].Right - pt.X;

                                if (a <= b)
                                    isTrailingHit = false;
                                else
                                    isTrailingHit = true;

                                pos = j;
                                break;
                            }
                        }
                    }
                    else if (pt.X < m.Left)
                    {
                        pos = m.TextPosition;
                        isTrailingHit = false;
                        break;
                    }
                    else
                    {
                        pos = m.TextPosition + m.Length - 1;
                        isTrailingHit = true;
                        break;
                    }
                }   
            }

            hitTextMetrics = new GDIHitTestMetrics()
            {
                TextPosition = pos,
                Left = txtHitRect[pos].Left,
                Top = txtHitRect[pos].Top,
                Width = txtHitRect[pos].Width,
                Height = txtHitRect[pos].Height,
                Length = 1
            };

            return hitTextMetrics;
        }






        GDIHitTestMetrics[] ComputeRowMetricsList()
        {
            RectangleF baseRect = txtHitRect[0];
            GDIHitTestMetrics hitTestMetrics = new GDIHitTestMetrics();
            int startPos = 0;
            List<GDIHitTestMetrics> rowMetricsSet = new List<GDIHitTestMetrics>();
            RectangleF rect;

            for (int i = 0; i < txtHitRect.Count(); i++)
            {
                rect = txtHitRect[i];

                if ((rect.Top >= baseRect.Top &&
                    rect.Top < baseRect.Bottom) ||
                    (rect.Bottom >= baseRect.Bottom &&
                    rect.Bottom < baseRect.Top))
                {
                    if (rect.Top < baseRect.Top)
                    {
                        baseRect.Y = rect.Top;
                        baseRect.Height += baseRect.Top - rect.Top;
                    }

                    if (rect.Bottom > baseRect.Bottom)
                        baseRect.Height += rect.Bottom - baseRect.Bottom;

                    baseRect.Width = rect.Right - baseRect.Left;
                }
                else
                {
                    hitTestMetrics = new GDIHitTestMetrics()
                    {
                        TextPosition = startPos,
                        Left = baseRect.Left,
                        Top = baseRect.Top,
                        Width = baseRect.Width,
                        Height = baseRect.Height,
                        Length = i - startPos
                    };

                    rowMetricsSet.Add(hitTestMetrics);

                    startPos = i;
                    baseRect = txtHitRect[i];
                }
            }


            hitTestMetrics = new GDIHitTestMetrics()
            {
                TextPosition = startPos,
                Left = baseRect.Left,
                Top = baseRect.Top,
                Width = baseRect.Width,
                Height = baseRect.Height,
                Length = txtHitRect.Count() - startPos
            };

            rowMetricsSet.Add(hitTestMetrics);

            return rowMetricsSet.ToArray();

        }

        public void StopAnim()
        {
            scAnim.Stop();
        }

        public void StartAnim()
        {
            cursorState = 1;
            scAnim.Start();
        }

        public bool CreateHwnd()
        {
            if (ScMgr == null)
                return false; 

            g = Graphics.FromHwnd(ScMgr.control.Handle);
            return true;
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

            Rectangle rc = new Rectangle(x, y, w, h);
            Invalidate(rc);
        }

        private void MeasureCharacterRangesRegions(RectangleF layoutRect)
        {
            int count = txt.Count();
    
            int n = count / 32;
            int m = count % 32;

            if (m > 0)
                n++;

            // Set string format.
            StringFormat stringFormat = new StringFormat();
            stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;

            CharacterRange crange;
            List<CharacterRange> characterRangeList = new List<CharacterRange>();
            int amount = 0;

            Region[] txtRegions;

            List<Region> txtRegionList = new List<Region>();
            CreateHwnd();

            if (g == null)
                return;


            float fitm = 0;

            if (n > 1)
                fitm = 1;


            for (int j = 0; j < n; j++)
            {
                amount = j * 32 + 32;
                if (amount > txt.Count())
                    amount = txt.Count();

                for (int i = j * 32; i < amount; i++)
                {
                    crange = new CharacterRange(i, 1);
                    characterRangeList.Add(crange);
                }

                stringFormat.SetMeasurableCharacterRanges(characterRangeList.ToArray());
                txtRegions = g.MeasureCharacterRanges(txt, ForeFont, layoutRect, stringFormat);

                RectangleF rs;
                foreach (Region r in txtRegions)
                {
                    txtRegionList.Add(r);
                    rs = r.GetBounds(g);          
                }

                characterRangeList.Clear();
            }

            RectangleF rect;
            RectangleF? oldrect = null;

            txtHitRect.Clear();

            foreach (Region r in txtRegionList)
            {
                rect = r.GetBounds(g);  
                      
                if (rect.Width == 0)
                {
                    RectangleF lastRect = txtHitRect[txtHitRect.Count() - 1];
                    rect.X = lastRect.Right;
                    rect.Y = lastRect.Top;
                    rect.Width = 0;
                    rect.Height = lastRect.Height;
                }

                if (oldrect != null)
                {
                    rect.X = oldrect.Value.X + oldrect.Value.Width;
                    rect.Width -= fitm;
                }

                txtHitRect.Add(rect);
                oldrect = rect;
            }
        }


    }
}
