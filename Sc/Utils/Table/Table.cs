using System;
using System.Collections.Generic;
using System.Drawing;

namespace Utils
{

    public class Table
    {

        PointF tbPos;
        float tbWidth;
        float tbHeight;

        public int rowAmount { get; set; }
        public int colAmount { get; set; }

        Margin defaultCellMargin = new Margin(0, 0, 0, 0);
        Dictionary<string, Margin> cellMarginDict = new Dictionary<string, Margin>();
        Dictionary<string, int> cellMouseStateDict = new Dictionary<string, int>();
        Dictionary<string, Table> cellChildTableDict = new Dictionary<string, Table>();
        CellCoord mouseAtCell = new CellCoord(-1, -1);
        bool isMouseAtTable = false;

        List<TableLine> rowLineList = new List<TableLine>();
        List<TableLine> colLineList = new List<TableLine>();


        public delegate void MouseEventDelegate(Table table, CellCoord cellCoord);
        public event MouseEventDelegate MouseLeave;
        public event MouseEventDelegate MouseHover;
        public event MouseEventDelegate MouseMove;

        public Table(float x, float y, float tbWidth, float tbHeight, int rowAmount, int colAmount)
        {
            this.tbPos.X = x;
            this.tbPos.Y = y;
            this.tbHeight = tbHeight;
            this.tbWidth = tbWidth;

            this.rowAmount = rowAmount;
            this.colAmount = colAmount;

            //
            TableLine tableLine;
            float rowComputedDist = tbHeight / rowAmount;
            float rowPercent = 1f / rowAmount;


            for (int i = 0; i < rowAmount; i++)
            {
                tableLine = new TableLine(LineDir.HORIZONTAL);
                tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;
                tableLine.computeParam = rowPercent;
                tableLine.computedDistance = rowComputedDist;
                rowLineList.Add(tableLine);
            }

            //
            float colComputedDist = tbWidth / colAmount;
            float colPercent = 1f / colAmount;

            for (int i = 0; i < colAmount; i++)
            {
                tableLine = new TableLine(LineDir.VERTICAL);
                tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;
                tableLine.computeParam = colPercent;
                tableLine.computedDistance = colComputedDist;
                colLineList.Add(tableLine);
            }
        }

        public Table(RectangleF tbRect, int rowAmount, int colAmount)
           : this(tbRect.X, tbRect.Y, tbRect.Width, tbRect.Height, rowAmount, colAmount)
        {

        }

        public void SetDefaultCellMargin(Margin cellMargin)
        {
            defaultCellMargin.left = cellMargin.left;
            defaultCellMargin.top = cellMargin.top;
            defaultCellMargin.right = cellMargin.right;
            defaultCellMargin.bottom = cellMargin.bottom;
        }

        public void SetCellMargin(int rowIdx, int colIdx, Margin margin)
        {
            string key = rowIdx.ToString() + colIdx.ToString();

            if (cellMarginDict.ContainsKey(key) == false)
            {
                cellMarginDict.Add(key, margin);
            }
            else
            {
                cellMarginDict[key] = margin;
            }
        }


        public Margin GetCellMargin(int rowIdx, int colIdx)
        {
            string key = rowIdx.ToString() + colIdx.ToString();

            if (cellMarginDict.ContainsKey(key) == false)
            {
                return defaultCellMargin;
            }
            else
            {
                return cellMarginDict[key];
            }
        }

        public void SetCellChildTable(int rowIdx, int colIdx, Table childTable)
        {
            string key = rowIdx.ToString() + colIdx.ToString();

            if (cellChildTableDict.ContainsKey(key) == false)
            {
                cellChildTableDict.Add(key, childTable);
            }
            else
            {
                cellChildTableDict[key] = childTable;
            }
        }

        public Table GetCellChildTable(int rowIdx, int colIdx)
        {
            string key = rowIdx.ToString() + colIdx.ToString();

            if (cellChildTableDict.ContainsKey(key) == false)
            {
                return null;
            }
            else
            {
                return cellChildTableDict[key];
            }
        }


        public void ComputeLinesArea(LineDir lineDir)
        {
            List<TableLine> lineList = null;
            float tbLength;

            if (lineDir == LineDir.HORIZONTAL)
            {
                lineList = rowLineList;
                tbLength = tbHeight;
            }
            else
            {
                lineList = colLineList;
                tbLength = tbWidth;
            }

            if (lineList.Count == 1)
                return;

            float usedLength = 0;
            float percent = 0;
            float tbRichHeight;
            float tbPerRichPercent;
            int percentCount = 0;
            TableLine line;


            for (int i = 0; i < lineList.Count; i++)
            {
                line = lineList[i];

                if (line.lineComputeMode == LineComputeMode.ABSOLUTE)
                {
                    usedLength += line.computeParam;

                    if (usedLength > tbLength)
                    {
                        usedLength -= line.computeParam;
                        line.computeParam = tbLength - usedLength;      
                    }

                    line.computedDistance = line.computeParam;
                }

                if (line.lineComputeMode == LineComputeMode.PERCENTAGE)
                {
                    percentCount++;

                    float n = percent + line.computeParam;

                    if (n > 1f + 0.001f)
                    {           
                        line.computeParam = 1f - percent;
                        percent = 1f;

                        for (int j = i+1; j < lineList.Count; j++)
                        {
                            percentCount++;
                            line = lineList[j];
                            line.computeParam = 0;
                            line.computedDistance = 0;
                        }

                        break;
                    }
                    else
                    {
                        percent = n;
                    }       
                }
            }

           
            tbRichHeight = tbLength - usedLength;
            tbPerRichPercent = (1f - percent) / percentCount;

            for (int i = 0; i < lineList.Count; i++)
            {
                line = lineList[i];

                if (line.lineComputeMode == LineComputeMode.PERCENTAGE)
                {
                    line.computeParam += tbPerRichPercent;
                    line.computedDistance = tbRichHeight * line.computeParam;
                }
            }
        }

        public void SetLineArea(int lineIdx, TableLine setLine)
        {
            List<TableLine> lineList = null;
            float tbLength;

            if (setLine.lineDir == LineDir.HORIZONTAL)
            {
                lineList = rowLineList;
                tbLength = tbHeight;
            }
            else
            {
                lineList = colLineList;
                tbLength = tbWidth;
            }

            if (lineList.Count == 1)
                return;

            TableLine adjustLine = lineList[lineIdx];
            adjustLine.lineComputeMode = setLine.lineComputeMode;
            adjustLine.computeParam = setLine.computeParam;     
        }
 
        public void AdjustLineArea(int lineIdx, TableLine setLine, AdjustMode adjustMode)
        {
            SetLineArea(lineIdx, setLine, adjustMode);
            ComputeLinesArea(setLine.lineDir);
        }

        private void SetLineArea(int lineIdx, TableLine setLine, AdjustMode adjustMode)
        {
            List<TableLine> lineList = null;
            float tbLength;

            if (setLine.lineDir == LineDir.HORIZONTAL)
            {
                lineList = rowLineList;
                tbLength = tbHeight;
            }
            else
            {
                lineList = colLineList;
                tbLength = tbWidth;
            }

            if (lineList.Count == 1)
                return;

            TableLine adjustLine = lineList[lineIdx];

            if (adjustLine.lineComputeMode == LineComputeMode.ABSOLUTE)
            {
                adjustLine.lineComputeMode = setLine.lineComputeMode;
                adjustLine.computeParam = setLine.computeParam;
                return;
            }

            TableLine upLine;
            TableLine nextLine;
            float richValue;

            if (lineIdx == lineList.Count - 1 && adjustMode == AdjustMode.DOWN_OR_RIGHT_ADJUST)
            {
                adjustMode = AdjustMode.UP_OR_LEFT_ADJUST;
            }
            else if (lineIdx == 0 && adjustMode == AdjustMode.UP_OR_LEFT_ADJUST)
            {
                adjustMode = AdjustMode.DOWN_OR_RIGHT_ADJUST;
            }

            switch (adjustMode)
            {
                case AdjustMode.UP_OR_LEFT_ADJUST:   //上边线调整

                    richValue = adjustLine.computeParam - setLine.computeParam;
                    adjustLine.computeParam = setLine.computeParam;

                    upLine = lineList[lineIdx - 1];
                    if (upLine.lineComputeMode == LineComputeMode.PERCENTAGE)
                        upLine.computeParam += richValue;

                    break;

                case AdjustMode.DOWN_OR_RIGHT_ADJUST:   //下边线调整

                    richValue = adjustLine.computeParam - setLine.computeParam;
                    adjustLine.computeParam = setLine.computeParam;

                    nextLine = lineList[lineIdx + 1];
                    if (nextLine.lineComputeMode == LineComputeMode.PERCENTAGE)
                        nextLine.computeParam += richValue;

                    break;
            }
        }

        public TableLine GetRowTableLine(int idx)
        {
            if (idx >= rowLineList.Count)
                idx = rowLineList.Count - 1;
            else if (idx < 0)
                idx = 0;

            return rowLineList[idx];
        }

        public TableLine GetColTableLine(int idx)
        {
            if (idx >= colLineList.Count)
                idx = colLineList.Count - 1;
            else if (idx < 0)
                idx = 0;

            return colLineList[idx];
        }

        public LinePos GetRowLinePos(int rowIdx)
        {
            RectangleF rect = GetCellRect(rowIdx, 0);

            LinePos linePos = new LinePos();
            PointF start = new PointF(rect.Left, rect.Bottom);
            PointF end = new PointF(rect.Left + tbWidth, rect.Bottom);

            linePos.start = start;
            linePos.end = end;
            return linePos;
        }

        public LinePos GetRowContentLinePos(int rowIdx)
        {
            CellCoord cellCoord = new CellCoord(rowIdx, 0);
            RectangleF rect = GetCellContentRect(cellCoord);

            LinePos linePos = new LinePos();
            PointF start = new PointF(rect.Left, rect.Bottom);
            PointF end = new PointF(rect.Right, rect.Bottom);

            linePos.start = start;
            linePos.end = end;
            return linePos;
        }


        public LinePos GetColLinePos(int colIdx)
        {
            RectangleF rect = GetCellRect(0, colIdx);

            LinePos linePos = new LinePos();
            PointF start = new PointF(rect.Right, rect.Top);
            PointF end = new PointF(rect.Right, rect.Top + tbHeight);

            linePos.start = start;
            linePos.end = end;
            return linePos;
        }

        public LinePos GetColContentLinePos(int colIdx)
        {
            RectangleF rect = GetCellContentRect(0, colIdx);

            LinePos linePos = new LinePos();
            PointF start = new PointF(rect.Right, rect.Top);
            PointF end = new PointF(rect.Right, rect.Bottom);

            linePos.start = start;
            linePos.end = end;
            return linePos;
        }

        public RectangleF GetTableRect()
        {
            RectangleF rect = new RectangleF(tbPos.X, tbPos.Y, tbWidth, tbHeight);
            return rect;
        }

        public RectangleF GetRowRect(int rowIdx)
        {
            RectangleF rect = GetCellRect(rowIdx, 0);
            rect.Width = tbWidth;
            return rect;
        }

        public RectangleF GetColRect(int colIdx)
        {
            RectangleF rect = GetCellRect(0, colIdx);
            rect.Height = tbHeight;
            return rect;
        }

        public RectangleF GetCellContentRect(int rowIdx, int colIdx)
        {
            return GetCellContentRect(new CellCoord(rowIdx, colIdx));
        }

        public RectangleF GetCellContentRect(CellCoord cellCoord)
        {
            RectangleF rect = GetCellRect(cellCoord);
            Margin cellMargin = GetCellMargin(cellCoord.rowIdx, cellCoord.colIdx);

            rect.X += cellMargin.left;
            rect.Y += cellMargin.top;

            rect.Width -= cellMargin.left + cellMargin.right;
            rect.Height -= cellMargin.top + cellMargin.bottom;

            return rect;
        }

        public RectangleF GetCellRect(int rowIdx, int colIdx)
        {
            return GetCellRect(new CellCoord(rowIdx, colIdx));
        }

        public RectangleF GetCellRect(CellCoord cellCoord)
        {
            TableLine line;
            RectangleF rect = new RectangleF();
            float xpos = tbPos.X;
            float ypos = tbPos.Y;

            for (int i = 0; i < colLineList.Count; i++)
            {
                line = colLineList[i];

                if (i == cellCoord.colIdx)
                    break;

                xpos += line.computedDistance;
            }

            rect.X = xpos;
            rect.Width = colLineList[cellCoord.colIdx].computedDistance;


            //
            for (int i = 0; i < rowLineList.Count; i++)
            {
                line = rowLineList[i];

                if (i == cellCoord.rowIdx)
                    break;

                ypos += line.computedDistance;
            }

            rect.Y = ypos;
            rect.Height = rowLineList[cellCoord.rowIdx].computedDistance;

            return rect;
        }


        public void MouseMoveEvent(PointF mousePt)
        {
            RectangleF rect;
            CellCoord cellCoord;

            if (!isMouseAtTable)
            {
                rect = GetTableRect();
                if (rect.Contains(mousePt))
                {
                    isMouseAtTable = true;
                    cellCoord = FindTableCell(mousePt);

                    if (cellCoord != null)
                    {
                        mouseAtCell.colIdx = cellCoord.colIdx;
                        mouseAtCell.rowIdx = cellCoord.rowIdx;
                        MouseHover(this, cellCoord);
                    }
                }

                return;
            }
            else
            {
                if (mouseAtCell.colIdx == -1 && mouseAtCell.rowIdx == -1)
                {
                    cellCoord = FindTableCell(mousePt);

                    if (cellCoord != null)
                    {
                        MouseHover(this, cellCoord);
                    }
                    else
                    {
                        MouseMove(this, null);
                    }
                }
                else
                {
                    rect = GetCellContentRect(mouseAtCell);

                    if (!rect.Contains(mousePt))
                    {
                        MouseLeave(this, mouseAtCell);
                    }
                    else
                    {
                        MouseMove(this, mouseAtCell);
                        return;
                    }

                    rect = GetTableRect();
                    if (rect.Contains(mousePt))
                    {
                        isMouseAtTable = true;
                        cellCoord = FindTableCell(mousePt);

                        if (cellCoord != null)
                        {
                            mouseAtCell.colIdx = cellCoord.colIdx;
                            mouseAtCell.rowIdx = cellCoord.rowIdx;
                            MouseHover(this, cellCoord);
                        }
                    }
                    else
                    {
                        mouseAtCell.colIdx = -1;
                        mouseAtCell.rowIdx = -1;
                        isMouseAtTable = false;
                        MouseLeave(this, null);
                    }
                }
            }
        }


        CellCoord FindTableCell(PointF pos)
        {
            RectangleF rect;
            CellCoord cellCoord = null;

            for (int row = 0; row < rowAmount; row++)
            {
                for (int col = 0; col < colAmount; col++)
                {
                    rect = GetCellContentRect(row, col);

                    if (rect.Contains(pos))
                    {
                        cellCoord = new CellCoord(row, col);
                        return cellCoord;
                    }
                }
            }

            return null;
        }

    }
}
