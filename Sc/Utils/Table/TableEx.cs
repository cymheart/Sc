using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Utils
{
    public class TableEx
    {
        public string name;

        PointF tbPos;
        float tbWidth;
        float tbHeight;

        public TableDockType dockType;
        public TableSizeType tableSizeType;
        Graphics g;

        public int rowAmount { get; set; }
        public int colAmount { get; set; }

        Margin defaultCellMargin = new Margin(0, 0, 0, 0);
        Dictionary<string, Margin> cellMarginDict = new Dictionary<string, Margin>();
        Dictionary<string, SpanOffset> cellSpanDict = new Dictionary<string, SpanOffset>();
        Dictionary<string, CellValueInfo> cellValueInfoDict = new Dictionary<string, CellValueInfo>();

        TableEx parentTable;
        CellCoord inParentCellCoord;

        List<TableLine> rowLineList = new List<TableLine>();
        List<TableLine> colLineList = new List<TableLine>();

        public TableEx(float x, float y, float tbWidth, float tbHeight, int rowAmount, int colAmount)
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
                tableLine.maxComputedValue = -1;
                tableLine.minComputedValue = -1;
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
                tableLine.maxComputedValue = -1;
                tableLine.minComputedValue = -1;
                colLineList.Add(tableLine);
            }
        }

        public TableEx(RectangleF tbRect, int rowAmount, int colAmount)
           : this(tbRect.X, tbRect.Y, tbRect.Width, tbRect.Height, rowAmount, colAmount)
        {

        }

        public TableEx(int rowAmount, int colAmount)
        {
            tbPos.X = 0;
            tbPos.Y = 0;
            tbHeight = 0;
            tbWidth = 0;

            this.rowAmount = rowAmount;
            this.colAmount = colAmount;

            tableSizeType = TableSizeType.Fill;

            TableLine tableLine;

            for (int i = 0; i < rowAmount; i++)
            {
                tableLine = new TableLine(LineDir.HORIZONTAL);
                tableLine.lineComputeMode = LineComputeMode.AUTO;
                tableLine.maxComputedValue = -1;
                tableLine.minComputedValue = -1;
                rowLineList.Add(tableLine);
            }

            //
            for (int i = 0; i < colAmount; i++)
            {
                tableLine = new TableLine(LineDir.VERTICAL);
                tableLine.lineComputeMode = LineComputeMode.AUTO;
                tableLine.maxComputedValue = -1;
                tableLine.minComputedValue = -1;
                colLineList.Add(tableLine);
            }
        }


        public TableLine GetTableLine(int idx, LineDir lineDir)
        {
            List<TableLine> lineList;

            if (lineDir == LineDir.HORIZONTAL)
                lineList = rowLineList;
            else
                lineList = colLineList;

            if (idx >= lineList.Count)
                return null;

            return lineList[idx];
        }

        public void SetTableLine(int idx, TableLine tableLine)
        {
            List<TableLine> lineList;
            int amount;

            if (tableLine.lineDir == LineDir.HORIZONTAL)
            {
                lineList = rowLineList;
                amount = rowAmount;              
            }
            else
            {
                lineList = colLineList;
                amount = colAmount;
            }

            if (idx < lineList.Count)
            {
                TableLine tl = new TableLine(tableLine.lineDir);
                tl.computeParam = tableLine.computeParam;
                tl.computedDistance = tableLine.computedDistance;
                tl.lineComputeMode = tableLine.lineComputeMode;
                tl.minComputedValue = tableLine.minComputedValue;
                tl.maxComputedValue = tableLine.maxComputedValue;
                lineList[idx] = tl; 
            }
            else
            {
                int n = idx - lineList.Count;
                for (int i = 0; i < n; i++)
                {
                    TableLine tl = new TableLine(tableLine.lineDir);
                    tl.computeParam = tableLine.computeParam;
                    tl.computedDistance = tableLine.computedDistance;
                    tl.lineComputeMode = tableLine.lineComputeMode;
                    tl.minComputedValue = tableLine.minComputedValue;
                    tl.maxComputedValue = tableLine.maxComputedValue;
                    lineList.Add(tl);
                    amount++;          
                }

                if (tableLine.lineDir == LineDir.HORIZONTAL)
                {
                    rowAmount = amount;
                }
                else
                {   
                    colAmount = amount;
                }
            }
        }

        public void SetAutoTableLine(int idx, LineDir lineDir)
        {
            TableLine tableLine = new TableLine(lineDir);
            tableLine.lineComputeMode = LineComputeMode.AUTO;
            tableLine.maxComputedValue = -1;
            tableLine.minComputedValue = -1;
            SetTableLine(idx, tableLine);
        }

        public void SetTablePosition(float x, float y)
        {
            this.tbPos.X = x;
            this.tbPos.Y = y;
        }

        public void SetTableSize(float w, float h)
        {
            tbWidth = w;
            tbHeight = h;
        }

        public string GetRowColKey(int rowIdx, int colIdx)
        {
            string key = rowIdx.ToString() + "," + colIdx.ToString();
            return key;
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
            string key = GetRowColKey(rowIdx, colIdx);

            if (cellMarginDict.ContainsKey(key) == false)
            {
                cellMarginDict.Add(key, margin);
            }

            cellMarginDict[key] = margin;
        }
  
        public Margin GetCellMargin(int rowIdx, int colIdx)
        {
            string key = GetRowColKey(rowIdx, colIdx);

            if (cellMarginDict.ContainsKey(key) == false)
            {
                return defaultCellMargin;
            }

            return cellMarginDict[key];
        }


        public void SetCellSpan(int rowIdx, int colIdx, SpanOffset span)
        {
            string key = GetRowColKey(rowIdx, colIdx);

            if (cellSpanDict.ContainsKey(key) == false)
            {
                cellSpanDict.Add(key, span);
            }

            cellSpanDict[key] = span;
     
        }

        public SpanOffset GetCellSpan(int rowIdx, int colIdx)
        {
            string key = GetRowColKey(rowIdx, colIdx);

            if (cellSpanDict.ContainsKey(key) == false)
            {
                return null;
            }
            return cellSpanDict[key];  
        }

        public void SetLinesComputedValueRange(int idx, float minValue, float maxValue, LineDir lineDir)
        {
            if(lineDir == LineDir.HORIZONTAL)
            {
                rowLineList[idx].minComputedValue = minValue;
                rowLineList[idx].maxComputedValue = maxValue;
            }
            else
            {
                colLineList[idx].minComputedValue = minValue;
                colLineList[idx].maxComputedValue = maxValue;
            }
        }

        public void SetCellValue(int rowIdx, int colIdx, CellValueInfo cellValueInfo)
        {
            string key = GetRowColKey(rowIdx, colIdx);

            if (cellValueInfoDict.ContainsKey(key) == false)
            {
                cellValueInfoDict.Add(key, cellValueInfo);
            }
            else
            {
                cellValueInfoDict[key] = cellValueInfo;
            }
        }

        public void SetCellFixedSizeValue(int rowIdx, int colIdx, SizeF fixedSize, CellValueInfo.ValueProcessHandler valueProcess, object value = null, Font font = null)
        {
            CellValueInfo cellValueInfo;
            cellValueInfo = new CellValueInfo();
            cellValueInfo.type = CellValueType.FixedSize;
            cellValueInfo.fixedSize = fixedSize;
            cellValueInfo.value = value;
            cellValueInfo.font = font;
            cellValueInfo.valueProcess += valueProcess;

            string key = GetRowColKey(rowIdx, colIdx);

            if (cellValueInfoDict.ContainsKey(key) == false)
            {
                cellValueInfoDict.Add(key, cellValueInfo);
            }
            else
            {
                cellValueInfoDict[key] = cellValueInfo;
            }
        }

        public void SetCellStringValue(int rowIdx, int colIdx, string value, Font font, CellValueInfo.ValueProcessHandler valueProcess)
        {
            CellValueInfo cellValueInfo;
            cellValueInfo = new CellValueInfo();
            cellValueInfo.type = CellValueType.String;
            cellValueInfo.value = value;
            cellValueInfo.font = font;
            cellValueInfo.valueProcess += valueProcess;

            string key = GetRowColKey(rowIdx, colIdx);

            if (cellValueInfoDict.ContainsKey(key) == false)
            {
                cellValueInfoDict.Add(key, cellValueInfo);
            }
            else
            {
                cellValueInfoDict[key] = cellValueInfo;
            }
        }
        public string GetCellStringValue(int rowIdx, int colIdx)
        {
            string key = GetRowColKey(rowIdx, colIdx);

            if (cellValueInfoDict.ContainsKey(key) == false)
            {
                return "";
            }
            else
            {
                CellValueInfo cellValueInfo;
                cellValueInfo = cellValueInfoDict[key];

                if (cellValueInfo.type == CellValueType.String )
                    return (string)cellValueInfo.value;

                return null;
            }
        }

        public void AddCellChildTable(int rowIdx, int colIdx, TableEx childTable)
        {
            CellValueInfo cellValueInfo;
            cellValueInfo = new CellValueInfo();
            cellValueInfo.type = CellValueType.Table;
            cellValueInfo.value = childTable;

            string key = GetRowColKey(rowIdx, colIdx);

            if (cellValueInfoDict.ContainsKey(key) == false)
            {
                cellValueInfoDict.Add(key, cellValueInfo);
            }
            else
            {
                cellValueInfoDict[key] = cellValueInfo;
            }

            childTable.parentTable = this;
            childTable.inParentCellCoord = new CellCoord(rowIdx, colIdx);
        }

        public TableEx GetCellChildTable(int rowIdx, int colIdx)
        {
            string key = GetRowColKey(rowIdx, colIdx);

            if (cellValueInfoDict.ContainsKey(key) == false)
            {
                return null;
            }
            else
            {
                CellValueInfo cellValueInfo;
                cellValueInfo = cellValueInfoDict[key];

                if (cellValueInfo.type == CellValueType.Table)
                    return (TableEx)cellValueInfo.value;

                return null;
            }
        }


        public CellValueInfo GetCellValueInfo(int rowIdx, int colIdx)
        {
            string key = GetRowColKey(rowIdx, colIdx);

            if (cellValueInfoDict.ContainsKey(key) == false)
            {
                return null;
            }
            else
            {
                return cellValueInfoDict[key];
            }
        }


        public void ReLayout(Graphics g = null)
        {
            this.g = g;

            ModifyTableDockPos(dockType);
            LayoutLinesArea(LineDir.HORIZONTAL);
            LayoutLinesArea(LineDir.VERTICAL);

            TableEx childTable;
            foreach (var item in cellValueInfoDict)
            {
                if (item.Value.type == CellValueType.Table)
                {
                    childTable = (TableEx)(item.Value.value);
                    childTable.ReLayout(g);
                }
            }
        }

        private void LayoutLinesArea(LineDir lineDir)
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

            float usedLength = 0;
            float percent = 0;
            float tbRichHeight;
            float tbPerRichPercent;
            float tbPerRichHeight;
            int percentCount = 0;
            int autoCount = 0;
            TableLine line;
          
    
            for (int i = 0; i < lineList.Count; i++)
            {
                line = lineList[i];

                switch (line.lineComputeMode)
                {
                    case LineComputeMode.AUTO:
                        line.computeParam = ComputeLineAreaSize(i, lineDir);
                        usedLength += line.computeParam;   
                        line.computedDistance = line.computeParam;

                        if(line.enableAutoAdjustParam)
                            autoCount++;

                        break;
              
                    case LineComputeMode.ABSOLUTE:
                        usedLength += line.computeParam;
                        line.computedDistance = line.computeParam;
                        ModifyTableSize(lineDir, line, ref usedLength, ref tbLength);  
                        break;

                    case LineComputeMode.PERCENTAGE:
                        percentCount++;
                        float n = percent + line.computeParam;

                        if (n > 1f + 0.001f)
                        {
                            line.computeParam = 1f - percent;
                            percent = 1f;

                            for (int j = i + 1; j < lineList.Count; j++)
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
                        break;
                }
            }


            if (tbLength == 0 &&      
                (tableSizeType == TableSizeType.Fill ||        
                (tableSizeType == TableSizeType.VerticalFill && lineDir == LineDir.HORIZONTAL) ||  
                (tableSizeType == TableSizeType.HorizontalFill && lineDir == LineDir.VERTICAL)))
            {
                tbLength = usedLength;
                if (lineDir == LineDir.HORIZONTAL)
                    tbHeight = usedLength;
                else
                    tbWidth = usedLength;
            }

            if (percentCount > 0)
            {
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
            else if(autoCount > 0)
            {
                tbRichHeight = tbLength - usedLength;
                tbPerRichHeight = tbRichHeight / autoCount;

                float totalScaleHeight = 0;
                float totalParamHeight = 0;

                for (int i = 0; i < lineList.Count; i++)
                {
                    line = lineList[i];

                    if (line.lineComputeMode == LineComputeMode.AUTO && line.enableAutoAdjustParam == true)
                    {
                        if (line.computeParam == 0)
                        {
                            line.computeParam = Math.Abs(tbPerRichHeight);
                            totalScaleHeight += Math.Abs(tbPerRichHeight);
                        }
                        else
                        {
                            totalScaleHeight += line.computeParam;
                            totalParamHeight += line.computeParam;
                        }
                    }
                }

                float realTotalHeight = totalParamHeight + tbRichHeight;

                for (int i = 0; i < lineList.Count; i++)
                {
                    line = lineList[i];

                    if (line.lineComputeMode == LineComputeMode.AUTO && line.computeParam != 0 && line.enableAutoAdjustParam == true)
                    {
                        line.computeParam = line.computeParam / totalScaleHeight * realTotalHeight;
                        line.computedDistance = line.computeParam;
                    }
                }
            }
        }

        void ModifyTableDockPos(TableDockType dockType)
        {
            RectangleF rect = new RectangleF(tbPos.X, tbPos.Y, tbWidth, tbHeight);
            if (parentTable != null)
            {             
                rect = parentTable.GetCellContentRect(inParentCellCoord.rowIdx, inParentCellCoord.colIdx);
            }

            switch (tableSizeType)
            {
                case TableSizeType.Fill:
                    tbHeight = rect.Height;
                    tbWidth = rect.Width;
                    break;

                case TableSizeType.HorizontalFill:
                    tbWidth = rect.Width;
                    break;

                case TableSizeType.VerticalFill:
                    tbHeight = rect.Height;
                    break;
            }

            switch (dockType)
            {
                case TableDockType.Center:
                    tbPos.X = rect.Width / 2 - tbWidth / 2;
                    tbPos.Y = rect.Height / 2 - tbHeight / 2;
                    return;

                case TableDockType.Left:
                    tbPos.X = 0;
                    tbPos.Y = rect.Height / 2 - tbHeight / 2;
                    return;

                case TableDockType.LeftTop:
                    tbPos.X = 0;
                    tbPos.Y = 0;
                    return;

                case TableDockType.Top:
                    tbPos.X = rect.Width / 2 - tbWidth / 2;
                    tbPos.Y = 0;
                    return;

                case TableDockType.RightTop:
                    tbPos.X = rect.Width - tbWidth;
                    tbPos.Y = 0;
                    return;

                case TableDockType.Right:
                    tbPos.X = rect.Width - tbWidth;
                    tbPos.Y = rect.Height / 2 - tbHeight / 2;
                    return;

                case TableDockType.RightBottom:
                    tbPos.X = rect.Width - tbWidth;
                    tbPos.Y = rect.Height - tbHeight;
                    return;

                case TableDockType.Bottom:
                    tbPos.X = rect.Width / 2 - tbWidth / 2;
                    tbPos.Y = rect.Height - tbHeight;
                    return;

                case TableDockType.LeftBottom:
                    tbPos.X = 0;
                    tbPos.Y = rect.Height - tbHeight;
                    return;
            }
        }


        void ModifyTableSize(LineDir lineDir, TableLine line, ref float usedLength, ref float tbLength)
        {
            if (tableSizeType == TableSizeType.Fill ||
                (tableSizeType == TableSizeType.VerticalFill && lineDir == LineDir.HORIZONTAL) ||
                (tableSizeType == TableSizeType.HorizontalFill && lineDir == LineDir.VERTICAL))
            {
                return;
            }
            else if (usedLength > tbLength)
            {
                usedLength -= line.computeParam;
                line.computeParam = tbLength - usedLength;
                usedLength += line.computeParam;
                line.computedDistance = line.computeParam;
            }
        }

        float ComputeLineAreaSize(int idx, LineDir lineDir)
        {
            Margin margin;
            TableEx ctable;
            float maxSize = 0;
            float size = 0;
            CellValueInfo cellValueInfo;
            string str;
            SizeF strSize;
            int amount = 0;
            int rowIdx, colIdx;
            SizeF fixedSize;

            if (lineDir == LineDir.HORIZONTAL)
                amount = colAmount;
            else
                amount = rowAmount;

            for (int i = 0; i < amount; i++)
            {
                if (lineDir == LineDir.HORIZONTAL)
                {
                    rowIdx = idx;
                    colIdx = i;
                }
                else {
                    rowIdx = i;
                    colIdx = idx;
                }

                margin = GetCellMargin(rowIdx, colIdx);
                cellValueInfo = GetCellValueInfo(rowIdx, colIdx);


                if (cellValueInfo == null)
                {
                    if (lineDir == LineDir.HORIZONTAL)
                        size = margin.top + margin.bottom;
                    else
                        size = margin.left + margin.right;
                }
                else
                {
                    switch (cellValueInfo.type)
                    {
                        case CellValueType.String:
                            str = (string)cellValueInfo.value;

                            if (g != null)
                                strSize = g.MeasureString(str, cellValueInfo.font);
                            else
                                strSize = TextRenderer.MeasureText(str, cellValueInfo.font);

                            if (lineDir == LineDir.HORIZONTAL)
                                size = strSize.Height + margin.top + margin.bottom;
                            else
                                size = strSize.Width + margin.left + margin.right;
                            break;


                        case CellValueType.Image:
                            Image image = (Image)cellValueInfo.value;

                            if (image == null)
                            {
                                size = 0;
                                break;
                            }

                            if (lineDir == LineDir.HORIZONTAL)
                            {
                                float h = image.Height;
                                if (cellValueInfo.permmSizePx.Height > 0)
                                    h = image.Height / cellValueInfo.permmSizePx.Height;

                                size = h + margin.top + margin.bottom;
                            }
                            else
                            {
                                float w = image.Width;
                                if (cellValueInfo.permmSizePx.Width > 0)
                                    w = image.Width / cellValueInfo.permmSizePx.Width;
                                size = w + margin.left + margin.right;
                            }
                            break;

                        case CellValueType.FixedSize:
                            fixedSize = cellValueInfo.fixedSize;

                            if (lineDir == LineDir.HORIZONTAL)
                                size = fixedSize.Height + margin.top + margin.bottom;
                            else
                                size = fixedSize.Width + margin.left + margin.right;
                            break;

                        case CellValueType.Table:
                            ctable = (TableEx)cellValueInfo.value;

                            if (ctable == null)
                                continue;

                            if (lineDir == LineDir.HORIZONTAL)
                            {
                                if (ctable.tbHeight == 0)
                                    ctable.ReLayout(g);
                                size = ctable.tbHeight + margin.top + margin.bottom;                             
                            }
                            else
                            {
                                if (ctable.tbWidth == 0)
                                    ctable.ReLayout(g);
                                size = ctable.tbWidth + margin.left + margin.right;
                            }
                            break;
                    }
                }

                maxSize = Math.Max(maxSize, size);
            }

            //
            return GetLimitLength(maxSize, idx, lineDir);
        }

        float GetLimitLength(float orgLen, int idx, LineDir lineDir)
        {
            //
            float len = orgLen;
            float min, max;

            if (lineDir == LineDir.HORIZONTAL)
            {
                min = rowLineList[idx].minComputedValue;
                max = rowLineList[idx].maxComputedValue;
            }
            else {
                min = colLineList[idx].minComputedValue;
                max = colLineList[idx].maxComputedValue;
            }

            if (orgLen < min && min != -1)
                len = min;
            else if (orgLen > max && max != -1)
                len = max;

            return len;
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

            TableLine adjustLine = lineList[lineIdx];
            adjustLine.lineComputeMode = setLine.lineComputeMode;
            adjustLine.computeParam = setLine.computeParam;
            adjustLine.enableAutoAdjustParam = setLine.enableAutoAdjustParam;
            adjustLine.lineDir = setLine.lineDir;
            adjustLine.minComputedValue = setLine.minComputedValue;
            adjustLine.maxComputedValue = setLine.maxComputedValue;
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

        public PointF TransToGlobalPoint(PointF pt)
        {
            RectangleF rect;
            float x = pt.X, y = pt.Y;
            CellCoord inCellCoord = inParentCellCoord;

            x += tbPos.X;
            y += tbPos.Y;

            for (TableEx tb = parentTable; tb != null; tb = tb.parentTable)
            {
                rect = tb.GetCellContentRect(inCellCoord.rowIdx, inCellCoord.colIdx);
                x += rect.X + tb.tbPos.X;
                y += rect.Y + tb.tbPos.Y;
                inCellCoord = tb.inParentCellCoord;
            }
        
            return new PointF(x, y);
        }

        public RectangleF TransToGlobalRect(RectangleF rect)
        {
            RectangleF r;
            CellCoord inCellCoord = inParentCellCoord;
            float x = rect.X, y = rect.Y;

            x += tbPos.X;
            y += tbPos.Y;

            for (TableEx tb = parentTable; tb != null; tb = tb.parentTable)
            {
                r = tb.GetCellContentRect(inCellCoord.rowIdx, inCellCoord.colIdx);
                x += r.X + tb.tbPos.X;
                y += r.Y + tb.tbPos.Y;
                inCellCoord = tb.inParentCellCoord;
            }

            RectangleF dstRect = new RectangleF(x, y, rect.Width, rect.Height);

            return dstRect;
        }

        public LinePos GetRowLinePos(int rowIdx, DirectionMode dir = DirectionMode.BOTTOM_OR_RIGHT)
        {
            RectangleF rect = GetOriginalCellRect(rowIdx, 0);

            LinePos linePos = new LinePos();
            PointF start, end;

            if (dir == DirectionMode.UP_OR_LEFT)
            {
                start = new PointF(rect.Left, rect.Top);
                end = new PointF(rect.Left + tbWidth, rect.Top);
            }
            else
            {
                start = new PointF(rect.Left, rect.Bottom);
                end = new PointF(rect.Left + tbWidth, rect.Bottom);
            }

            linePos.start = start;
            linePos.end = end;
            return linePos;
        }

        public LinePos GetColLinePos(int colIdx, DirectionMode dir = DirectionMode.BOTTOM_OR_RIGHT)
        {
            RectangleF rect = GetOriginalCellRect(0, colIdx);

            LinePos linePos = new LinePos();
            PointF start, end;

            if (dir == DirectionMode.UP_OR_LEFT)
            {
                start = new PointF(rect.Left, rect.Top);
                end = new PointF(rect.Left, rect.Top + tbHeight);
            }
            else
            {
                start = new PointF(rect.Right, rect.Top);
                end = new PointF(rect.Right, rect.Top + tbHeight);
            }

            linePos.start = start;
            linePos.end = end;
            return linePos;
        }

        public RectangleF GetRect(int startRowIdx, int startColIdx, int endRowIdx, int endColIdx)
        {
            RectangleF r1 = GetOriginalCellRect(startRowIdx, startColIdx);
            RectangleF r2 = GetOriginalCellRect(endRowIdx, endColIdx);

            float left = r2.Left, top = r2.Top, right = r2.Right, bottom = r2.Bottom;

            if (r1.Left < r2.Left)
                left = r1.Left;
       
            if (r1.Top < r2.Top)
                top = r1.Top;
   
            if (r1.Right > r2.Right)
                right = r1.Right;

            if (r1.Bottom > r2.Bottom)
                bottom = r1.Bottom;

            RectangleF rect = new RectangleF(left, top, right - left, bottom - top);
            return rect;
        }

        public LinePos[] GetRectLinePos(int startRowIdx, int startColIdx, int endRowIdx, int endColIdx)
        {
            RectangleF rect = GetRect(startRowIdx, startColIdx, endRowIdx, endColIdx);

            LinePos[] linePosList = new LinePos[4];

            //
            LinePos linePos = new LinePos();
            linePos.start = new PointF(rect.Left, rect.Top);
            linePos.end = new PointF(rect.Left, rect.Bottom);
            linePosList[0] = linePos;

            //
            linePos = new LinePos();
            linePos.start = new PointF(rect.Left, rect.Top);
            linePos.end = new PointF(rect.Right, rect.Top);
            linePosList[1] = linePos;

            //
            linePos = new LinePos();
            linePos.start = new PointF(rect.Right, rect.Top);
            linePos.end = new PointF(rect.Right, rect.Bottom);
            linePosList[2] = linePos;


            //
            linePos = new LinePos();
            linePos.start = new PointF(rect.Left, rect.Bottom);
            linePos.end = new PointF(rect.Right, rect.Bottom);
            linePosList[3] = linePos;

            return linePosList;

        }

        public RectangleF GetTableRect()
        {
            RectangleF rect = new RectangleF(0, 0, tbWidth, tbHeight);
            return rect;
        }

        public RectangleF GetRowRect(int rowIdx)
        {
            RectangleF rect = GetOriginalCellRect(rowIdx, 0);
            rect.Width = tbWidth;
            return rect;
        }

        public RectangleF GetColRect(int colIdx)
        {
            RectangleF rect = GetOriginalCellRect(0, colIdx);
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
            SpanOffset span = GetCellSpan(cellCoord.rowIdx, cellCoord.colIdx);

            if (span == null)
                return GetOriginalCellRect(cellCoord);


            int rowIdx = cellCoord.rowIdx - span.top;
            int colIdx = cellCoord.colIdx - span.left;
            RectangleF r1 = GetOriginalCellRect(rowIdx, colIdx);

            rowIdx = cellCoord.rowIdx + span.bottom;
            colIdx = cellCoord.colIdx + span.right;
            RectangleF r2 = GetOriginalCellRect(rowIdx, colIdx);


            RectangleF rect = new RectangleF(r1.Left, r1.Top, r2.Right - r1.Left, r2.Bottom - r1.Top);
            return rect;
        }

        public RectangleF GetOriginalCellRect(int rowIdx, int colIdx)
        {
            return GetOriginalCellRect(new CellCoord(rowIdx, colIdx));
        }

        public RectangleF GetOriginalCellRect(CellCoord cellCoord)
        {
            TableLine line;
            RectangleF rect = new RectangleF();
            float xpos = 0;
            float ypos = 0;

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


        public void ProcessAllCellValue()
        {
            CellValueInfo cellValueInfo;
            string[] cellCoord;
            RectangleF rect;

            foreach (var item in cellValueInfoDict)
            {
                cellValueInfo = item.Value;
                cellValueInfo.g = g;
                cellCoord = item.Key.Split(',');
                rect = GetCellContentRect(int.Parse(cellCoord[0]), int.Parse(cellCoord[1]));
                cellValueInfo.rect = TransToGlobalRect(rect);

                switch(cellValueInfo.type)
                {
                    case CellValueType.Table:
                        TableEx ctable = (TableEx)cellValueInfo.value;
                        ctable.ProcessAllCellValue();
                        break;

                    default:
                        cellValueInfo.valueProcess(cellValueInfo);
                        break;
                }         
            }
        }

        public void CreateNewAutoRow(CellValueInfo[] cellValueInfos)
        {
            CreateNewRow(cellValueInfos);
        }

        public void CreateNewRow(CellValueInfo[] cellValueInfos,
            bool enableAutoAdjustParam = true,
            LineComputeMode lineComputeMode = LineComputeMode.AUTO, 
            float computeParam = 0, 
            float computedDistance = 0)
        {
            CreateNewRow(enableAutoAdjustParam, lineComputeMode, computeParam, computedDistance);

            for (int i = 0; i < colAmount; i++)
            {
                SetCellValue(rowAmount - 1, i, cellValueInfos[i]);
            }
        }

        public void CreateNewRow(
            bool enableAutoAdjustParam = true,
            LineComputeMode lineComputeMode = LineComputeMode.AUTO, 
            float computeParam = 0, 
            float computedDistance = 0)
        {
            rowAmount += 1;

            TableLine tableLine = new TableLine(LineDir.HORIZONTAL);
            tableLine.enableAutoAdjustParam = enableAutoAdjustParam;
            tableLine.lineComputeMode = lineComputeMode;
            tableLine.computeParam = computeParam;
            tableLine.computedDistance = computedDistance;
            tableLine.maxComputedValue = -1;
            tableLine.minComputedValue = -1;
            rowLineList.Add(tableLine);
        }


        public void CreateNewAutoCol(CellValueInfo[] cellValueInfos)
        {
            CreateNewCol(cellValueInfos);
        }

        public void CreateNewCol(CellValueInfo[] cellValueInfos,    
            bool enableAutoAdjustParam = true,
            LineComputeMode lineComputeMode = LineComputeMode.AUTO, 
            float computeParam = 0,   
            float computedDistance = 0)
        {
            CreateNewCol(enableAutoAdjustParam, lineComputeMode, computeParam, computedDistance);

            for (int i = 0; i < rowAmount; i++)
            {
                SetCellValue(i, colAmount - 1, cellValueInfos[i]);
            }
        }

        public void CreateNewCol(
            bool enableAutoAdjustParam = true,
            LineComputeMode lineComputeMode = LineComputeMode.AUTO, 
            float computeParam = 0, 
            float computedDistance = 0)
        {
            colAmount += 1;

            TableLine tableLine = new TableLine(LineDir.VERTICAL);
            tableLine.enableAutoAdjustParam = enableAutoAdjustParam;
            tableLine.lineComputeMode = lineComputeMode;
            tableLine.computeParam = computeParam;
            tableLine.computedDistance = computedDistance;
            tableLine.maxComputedValue = -1;
            tableLine.minComputedValue = -1;
            colLineList.Add(tableLine);
        }


        public void DrawTableLine(Graphics g, Pen pen, bool isDrawOutBox = true)
        {
            if (isDrawOutBox)
            {
                RectangleF rect = GetTableRect();
                rect = TransToGlobalRect(rect);
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            }

            LinePos linePos;
            PointF start, end;

            for (int i = 0; i < rowAmount - 1; i++)
            {
                linePos = GetRowLinePos(i);
                start = TransToGlobalPoint(linePos.start);
                end = TransToGlobalPoint(linePos.end);

                g.DrawLine(pen, start, end);
            }

            for (int i = 0; i < colAmount - 1; i++)
            {
                linePos = GetColLinePos(i);
                start = TransToGlobalPoint(linePos.start);
                end = TransToGlobalPoint(linePos.end);
                g.DrawLine(pen, start, end);
            }
        }
    }
}
