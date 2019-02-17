using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScVxTxtInfoPlane : ScLayer
    {
        Table mainTable;
        Table txtTable;

        List<ScTxtInfo> rowTextInfoList = new List<ScTxtInfo>();
        int txtRowCount;
        public Font txtFont = new Font("微软雅黑", 12);
        public Color txtColor = Color.Black;

        public float hPadding;
        public float vPadding;

        public bool isHorLayout = false;
        public bool isShowFieldTitle = false;
        public float titleRatio = 0.5f;

        public ScVxTxtInfoPlane()
        {
            SizeChanged += ScVxCountdownPlane_SizeChanged;
            GDIPaint += ScVxTxtInfoPlane_GDIPaint;
        }

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

        public void UpdateVarValue(int index, float value)
        {
            if (rowTextInfoList[index].maxVarValue == value)
                return;

            rowTextInfoList[index].maxVarValue = value;
            rowTextInfoList[index].StartScaleAnim();
        }
        private void ScVxTxtInfoPlane_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.TextRenderingHint = TextRenderingHint.AntiAlias;

            Brush brush;
            RectangleF rect;
            RectangleF titleRect = new RectangleF();
 
            //txt
            ScTxtInfo txtInfo = new ScTxtInfo();

            for (int i = 0; i < TxtRowCount; i++)
            {
                if(isShowFieldTitle)
                {
                    if (!isHorLayout)
                    {
                        rect = txtTable.GetCellContentRect(i, 1);
                        titleRect = txtTable.GetCellContentRect(i, 0);
                    }
                    else
                    {
                        rect = txtTable.GetCellContentRect(1, i);
                        titleRect = txtTable.GetCellContentRect(0, i);
                    }
                }
                else
                {
                    if (!isHorLayout)
                        rect = txtTable.GetCellContentRect(i, 0);
                    else
                        rect = txtTable.GetCellContentRect(0, i);
                }

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
                    txtInfo.drawPosType = rowTextInfoList[i].drawPosType;
                    txtInfo.isLimitBox = rowTextInfoList[i].isLimitBox;
                    txtInfo.titleTxt = rowTextInfoList[i].titleTxt;
                    txtInfo.isMultipleRow = rowTextInfoList[i].isMultipleRow;
                }
                else
                {
                    continue;
                }


                RectangleF tmp;

                if (txtInfo.incRect == null)
                {
                    tmp = rect;
                }
                else
                {
                    RectangleF incRect = txtInfo.incRect.Value;
                    tmp = new RectangleF(rect.X + incRect.X, rect.Y + incRect.Y, rect.Width + incRect.Width, rect.Height + incRect.Height);
                }

                brush = new SolidBrush(txtInfo.txtColor);
                //  graphis.FillRectangle(Brushes.Blue, rect);
                DrawUtils.LimitBoxDraw(graphis, txtInfo.txt, txtInfo.txtFont, brush, tmp, txtInfo.drawPosType, txtInfo.isLimitBox, txtInfo.isMultipleRow, 0);
                DrawUtils.LimitBoxDraw(graphis, txtInfo.titleTxt, txtInfo.txtFont, brush, titleRect, txtInfo.drawPosType, true, false, 0);
                brush.Dispose();
            }
        }

        public void SetVarValueText(
            int index, string titleTxt, float maxValue, string fontTxt, string backTxt, Font font, 
            Color color, RectangleF? incRect, float lineHeight, DrawPositionType drawPosType, bool isLimitBox)
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
            txtInfo.drawPosType = drawPosType;
            txtInfo.isLimitBox = isLimitBox;
            txtInfo.titleTxt = titleTxt;
            txtInfo.isMultipleRow = false;
            rowTextInfoList[index] = txtInfo;
        }

        public void SetText(int index, string titleTxt, string txt, Font font, Color color, float lineHeight, DrawPositionType drawPosType, bool isLimitBox, bool isMultipleRow)
        {
            SetText(index, titleTxt, txt, font, color, null, lineHeight, drawPosType, isLimitBox, isMultipleRow);
        }

        public void SetText(int index, string titleTxt, string txt, Font font, Color color, RectangleF? incRect, float lineHeight, DrawPositionType drawPosType, bool isLimitBox, bool isMultipleRow)
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
            txtInfo.drawPosType = drawPosType;
            txtInfo.isLimitBox = isLimitBox;
            txtInfo.titleTxt = titleTxt;
            txtInfo.isMultipleRow = isMultipleRow;
            rowTextInfoList[index] = txtInfo;
        }

        private void ScVxCountdownPlane_SizeChanged(object sender, SizeF oldSize)
        {
            ReCreateTable();

            ScTxtInfo txtInfo;

            for (int i = 0; i < rowTextInfoList.Count(); i++)
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

        public void ReCreateTable()
        {
            int n = 1;
            RectangleF rect = new RectangleF(0, 0, Width, Height);
            mainTable = new Table(rect, 1, 1);

            Margin defaultCellMargin = new Margin(hPadding, vPadding, hPadding, vPadding);
            mainTable.SetDefaultCellMargin(defaultCellMargin);

            if (isHorLayout == false)
            {
                if (isShowFieldTitle)
                    n = 2;
                //
                txtTable = new Table(mainTable.GetCellContentRect(0, 0), TxtRowCount, n);

                for(int j = 0; j< TxtRowCount; j++)
                {
                    int m = 0;
                    Margin margin = new Margin(20, 0, 0, 0);

                    if (n == 2)
                        m = 1;

                    txtTable.SetCellMargin(j, m, margin);
                }


                TableLine tableLine = new TableLine(LineDir.HORIZONTAL);
                tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;
                int i = 0;

                foreach (ScTxtInfo txtInfo in rowTextInfoList)
                {
                    tableLine.computeParam = rowTextInfoList[i].lineHeight;
                    txtTable.SetLineArea(i, tableLine);
                    i++;
                }

                txtTable.ComputeLinesArea(LineDir.HORIZONTAL);


                if (isShowFieldTitle)
                {
                    //
                    tableLine = new TableLine(LineDir.VERTICAL);
                    tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;

                    tableLine.computeParam = titleRatio;
                    txtTable.SetLineArea(0, tableLine);

                    tableLine.computeParam = 1 - titleRatio;
                    txtTable.SetLineArea(1, tableLine);

                    txtTable.ComputeLinesArea(LineDir.VERTICAL);
                }

            }
            else
            {
                if (isShowFieldTitle)
                    n = 2;

                txtTable = new Table(mainTable.GetCellContentRect(0, 0), n, TxtRowCount);

                TableLine tableLine = new TableLine(LineDir.VERTICAL);
                tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;
                int i = 0;

                foreach (ScTxtInfo txtInfo in rowTextInfoList)
                {
                    tableLine.computeParam = rowTextInfoList[i].lineHeight;
                    txtTable.SetLineArea(i, tableLine);
                    i++;
                }

                txtTable.ComputeLinesArea(LineDir.VERTICAL);


                if (isShowFieldTitle)
                {
                    //
                    tableLine = new TableLine(LineDir.HORIZONTAL);
                    tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;

                    tableLine.computeParam = titleRatio;
                    txtTable.SetLineArea(0, tableLine);

                    tableLine.computeParam = 1 - titleRatio;
                    txtTable.SetLineArea(1, tableLine);

                    txtTable.ComputeLinesArea(LineDir.HORIZONTAL);
                }
            }
        }
    }
}
