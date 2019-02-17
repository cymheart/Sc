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


    public class ScVxButton2 : ScLayer
    {
        List<ScTxtInfo> rowTextInfoList = new List<ScTxtInfo>();
        Table mainTable;
        Table txtTable;
        Table contentTable;


        public int txtRowCount = 1;
        int state = 0;

        public int TxtRowCount
        {
            get
            {
                return txtRowCount;
            }
            set
            {
                txtRowCount = value;
                ReCreateTable();

                for (int i = 0; i < txtRowCount; i++)
                {
                    rowTextInfoList.Add(null);
                }
            }
        }

        public Image img { get; set; }
        public Color bgColor = Color.White;
        public Font txtFont = new Font("微软雅黑", 12);
        public Color txtColor = Color.Black;

        public Color progressbarColor = Color.FromArgb(255, 105, 163, 175);
        public Color enterHaloColor = Color.FromArgb(255,33,46,52);


        ScLinearAnimation mouseEnterLinear;
        ScAnimation mouseEnterAnim;

        ScLinearAnimation mouseEnterLinear1;
        ScAnimation mouseEnterAnim1;

        public bool isPaddingPercentage = false; 
        public float hPadding;
        public float vPadding;

        public float splitValue;


        public float startColor = 250;
        public float endColor = 120;
        public int enterHaloAlpha = 120;

        public bool isSelected = false;

        public RectangleF imgDisplayArea;
        bool isUseMouseEvent = true;

        float growHeight = 0;

        public ScVxButton2()
        {
            SizeChanged += ScVxButton_SizeChanged;

            MouseEnter += BtnLayer_MouseEnter;
            MouseLeave += BtnLayer_MouseLeave;
            MouseDown += BtnLayer_MouseDown;


            GDIPaint += ScVxButton_GDIPaint;


            mouseEnterAnim1 = new ScAnimation(this, 70, true);
            mouseEnterAnim1.AnimationEvent += ScAnim_AnimationEvent1;

            mouseEnterAnim = new ScAnimation(this, 600, true);
            mouseEnterAnim.AnimationEvent += ScAnim_AnimationEvent;
        }

        ~ScVxButton2()
        {
            mouseEnterAnim.Stop();
            mouseEnterAnim1.Stop();
        }


        public bool IsUseMouseEvent
        {
            get { return isUseMouseEvent; }
            set
            {
                MouseEnter -= BtnLayer_MouseEnter;
                MouseLeave -= BtnLayer_MouseLeave;
                MouseDown -= BtnLayer_MouseDown;

                isUseMouseEvent = value;

                if(isUseMouseEvent)
                {
                    MouseEnter += BtnLayer_MouseEnter;
                    MouseLeave += BtnLayer_MouseLeave;
                    MouseDown += BtnLayer_MouseDown;
                }
            }
        }

        private void ScAnim_AnimationEvent1(ScAnimation scAnimation)
        {
            growHeight = mouseEnterLinear1.GetCurtValue();

            if (mouseEnterLinear1.IsStop)
            {
                mouseEnterAnim1.Stop();

                StartProgressAnim(startColor, endColor);
            }

            Refresh();
        }

        public void StartAnim1()
        {
            mouseEnterAnim1.Stop();
            mouseEnterLinear1 = new ScLinearAnimation(0, Height - 1, mouseEnterAnim1);
            mouseEnterAnim1.Start();
        }


        private void ScAnim_AnimationEvent(ScAnimation scAnimation)
        {
            if (mouseEnterLinear == null)
                return;

            float value = mouseEnterLinear.GetCurtValue();
            enterHaloAlpha = (int)value;

            if (mouseEnterLinear.IsStop)
            {
                mouseEnterAnim.Stop();
                mouseEnterLinear = null;

                if(value == endColor)
                    StartProgressAnim(endColor, startColor);
                else
                    StartProgressAnim(startColor, endColor);
            }

            Refresh();
        }

        public void StartProgressAnim(float startValue, float stopValue)
        {
            mouseEnterAnim.Stop();
            mouseEnterLinear = new ScLinearAnimation(startValue, stopValue, mouseEnterAnim);
            mouseEnterAnim.Start();
        }

  
        private void BtnLayer_MouseDown(object sender, ScMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isSelected = true;
                Refresh();
            }
        }

        private void ScVxButton_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;
            graphis.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Brush brush;
            RectangleF rect;

            //bg

            if (state == 1 && !isSelected)
            {

                float h = (Height / 2 - growHeight/ 2);
                float y = Height - growHeight - h;
                RectangleF r = new RectangleF(0, y, Width, h * 2);

                GraphicsPath path = new GraphicsPath();
                path.AddArc(r, 180, 180);
                path.AddLine(r.Right, r.Y + h, r.Right, Height);
                path.AddLine(r.Right, Height, 0, Height);
                path.AddLine(0, Height, 0, r.Y + h);
                path.CloseFigure();


                rect = new RectangleF(0, 0, Width, Height);
                LinearGradientBrush linearBrush = new LinearGradientBrush(rect, Color.FromArgb(0, 185, 212, 227), Color.FromArgb(enterHaloAlpha, 185, 212, 227), LinearGradientMode.Vertical);
                graphis.FillPath(linearBrush, path);

                path.Dispose();
                linearBrush.Dispose();

                //brush = new SolidBrush(enterHaloColor);
                //rect = new RectangleF(0, 0, Width, Height);
                //graphis.FillRectangle(brush, rect);
                //brush.Dispose();
            }

            if (isSelected == true)
            {
                rect = new RectangleF(0, 0, Width, Height);

              
                brush = new SolidBrush(Color.FromArgb(255,enterHaloColor));

                Pen pen = new Pen(Color.FromArgb(160, 200, 213, 219));
                Pen pen2 = new Pen(Color.FromArgb(255, 29, 29, 39));

                graphis.FillRectangle(brush, rect);

                PointF pt1 = new PointF(0, 0);
                PointF pt2 = new PointF(0, Height);
                graphis.DrawLine(pen2, pt1, pt2);

                pt1 = new PointF(Width - 1, 0);
                pt2 = new PointF(Width - 1, Height);
                graphis.DrawLine(pen, pt1, pt2);


                pt1 = new PointF(0, 0);
                pt2 = new PointF(Width - 1, 0);
                graphis.DrawLine(pen2, pt1, pt2);

                pt1 = new PointF(0, Height - 1);
                pt2 = new PointF(Width - 1, Height - 1);
                graphis.DrawLine(pen, pt1, pt2);

                brush.Dispose();
            }

            //img
            if (img != null)
            {
                RectangleF imgRect = contentTable.GetCellContentRect(0, 0);
                graphis.DrawImage(img, imgRect, imgDisplayArea, GraphicsUnit.Pixel);
            }

            //txt
            ScTxtInfo txtInfo = new ScTxtInfo();

            for (int i = 0; i < TxtRowCount; i++)
            {
                rect = txtTable.GetCellContentRect(i, 0);

                if (rowTextInfoList[i] != null)
                {
                    if (rowTextInfoList[i].txt != null)
                        txtInfo.txt = rowTextInfoList[i].txt;
                    else
                        txtInfo.txt = "";

                    if (rowTextInfoList[i].txtFont != null)
                        txtInfo.txtFont = rowTextInfoList[i].txtFont;
                    else
                        txtInfo.txtFont = txtFont;

                    if (rowTextInfoList[i].txtColor != null)
                        txtInfo.txtColor = rowTextInfoList[i].txtColor;
                    else
                        txtInfo.txtColor = txtColor;

                    txtInfo.incRect = rowTextInfoList[i].incRect;
                }
                else
                {
                    continue;
                }


                RectangleF tmp;

                if (txtInfo.incRect == null)
                    tmp = rect;
                else
                {
                    RectangleF incRect = txtInfo.incRect.Value;
                    tmp = new RectangleF(rect.X + incRect.X, rect.Y + incRect.Y, rect.Width + incRect.Width, rect.Height + incRect.Height);
                }

                brush = new SolidBrush(txtInfo.txtColor);
             //   graphis.FillRectangle(Brushes.Blue, rect);
                DrawUtils.LimitBoxDraw(graphis, txtInfo.txt, txtInfo.txtFont, brush, tmp, false, true, 0);
                brush.Dispose();
            }
        }

        private void BtnLayer_MouseLeave(object sender)
        {
            state = 0;
            mouseEnterAnim1.Stop();
            mouseEnterAnim.Stop();
            Refresh();
        }

        private void BtnLayer_MouseEnter(object sender, ScMouseEventArgs e)
        {
            state = 1;
            enterHaloAlpha = 250;
            StartAnim1();
            //StartProgressAnim(startColor, endColor);

        }


        public void SetVarValueText(int index, float maxValue, string fontTxt, string backTxt, Font font, Color color, RectangleF? incRect, float lineHeight)
        {
            ScTxtInfo txtInfo;

            if (rowTextInfoList[index] != null)
            {
                txtInfo = rowTextInfoList[index];
            }
            else
            {
                txtInfo = new ScTxtInfo(this);
            }

            txtInfo.maxVarValue = maxValue;
            txtInfo.fontTxt = fontTxt;
            txtInfo.backTxt = backTxt;
            txtInfo.txtColor = color;
            txtInfo.txtFont = font;
            txtInfo.lineHeight = lineHeight;
            txtInfo.incRect = incRect;
            rowTextInfoList[index] = txtInfo;
        }

        public void SetText(int index, string txt, Font font, Color color, float lineHeight)
        {
            SetText(index, txt, font, color, null, lineHeight);
        }

        public void SetText(int index, string txt, Font font, Color color, RectangleF? incRect, float lineHeight)
        {
            ScTxtInfo txtInfo;

            if (rowTextInfoList[index] != null)
            {
                txtInfo = rowTextInfoList[index];
            }
            else
            {
                txtInfo = new ScTxtInfo();
            }

            txtInfo.txt = txt;
            txtInfo.txtColor = color;
            txtInfo.txtFont = font;
            txtInfo.lineHeight = lineHeight;
            txtInfo.incRect = incRect;
            rowTextInfoList[index] = txtInfo;
        }


        public void UpdateVarValue(int index, float value)
        {
            if (rowTextInfoList[index].maxVarValue == value)
                return;

            rowTextInfoList[index].maxVarValue = value;
            rowTextInfoList[index].StartScaleAnim();
        }

        public void ReCreateTable()
        {
            RectangleF rect = new RectangleF(0, 0, Width, Height);
            mainTable = new Table(rect, 1, 1);

            float hPaddingLen;
            float vPaddingLen;

            if(isPaddingPercentage)
            {
                hPaddingLen = hPadding * Width;
                vPaddingLen = vPadding * Height;
            }
            else
            {
                hPaddingLen = hPadding;
                vPaddingLen = vPadding;
            }


            Margin defaultCellMargin = new Margin(hPaddingLen, vPaddingLen, hPaddingLen, vPaddingLen);
            mainTable.SetDefaultCellMargin(defaultCellMargin);


            //
            contentTable = new Table(mainTable.GetCellContentRect(0,0), 1, 2);
            TableLine tableLine = new TableLine(LineDir.VERTICAL);
            tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;

            tableLine.computeParam = splitValue;
            contentTable.SetLineArea(0, tableLine);

            tableLine.computeParam = 1 - splitValue;
            contentTable.SetLineArea(1, tableLine);

            contentTable.ComputeLinesArea(LineDir.VERTICAL);


            //
            txtTable = new Table(contentTable.GetCellContentRect(0, 1), TxtRowCount, 1);

            tableLine = new TableLine(LineDir.HORIZONTAL);
            tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;
            int i = 0;

            foreach (ScTxtInfo txtInfo in rowTextInfoList)
            {
                tableLine.computeParam = rowTextInfoList[i].lineHeight;
                txtTable.SetLineArea(i, tableLine);
                i++;
            }

            txtTable.ComputeLinesArea(LineDir.HORIZONTAL);
        }


        private void ScVxButton_SizeChanged(object sender, SizeF oldSize)
        {  
            ReCreateTable();

            ScTxtInfo txtInfo;

            for(int i=0; i<rowTextInfoList.Count(); i++)
            {
                txtInfo = rowTextInfoList[i];

                txtInfo.AnimScaleEvent += TxtInfo_AnimScaleEvent;

                if (txtInfo.type == 0)
                    continue;

                txtInfo.StartScaleAnim();
            }
        }

        private void TxtInfo_AnimScaleEvent()
        {
            Refresh();
        }
    }
}