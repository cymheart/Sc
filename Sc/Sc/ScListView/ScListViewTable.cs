using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public partial class ScListView
    {
        void CreateViewLayoutTable(int itemCount)
        {
            float w = (itemWidth + itemMargin.left + itemMargin.right) * colItemCount;
            rowItemCount = itemCount / colItemCount;

            int m = itemCount % colItemCount;
            if (m > 0) { rowItemCount--; }
            float contentHeight = rowItemCount * (itemHeight + itemMargin.top + itemMargin.bottom);
            float viewHeight = headerHeight + contentViewHeight;

            viewLayoutTable = new TableEx(2, 1);
            viewLayoutTable.SetTablePosition(0, 0);
            viewLayoutTable.SetTableSize(w, viewHeight);

            TableLine tableLine = new TableLine(LineDir.HORIZONTAL);
            tableLine.lineComputeMode = LineComputeMode.ABSOLUTE;

            tableLine.computeParam = headerHeight;
            viewLayoutTable.SetLineArea(0, tableLine);

            tableLine.computeParam = contentViewHeight;
            viewLayoutTable.SetLineArea(1, tableLine);

            //
            itemContentLayoutTable = headerLayoutTable = new TableEx(0, headerNames.Count());
            itemContentLayoutTable.SetTableSize(w, headerHeight);
            viewLayoutTable.AddCellChildTable(0, 0, headerLayoutTable);


            //
            SetTableData();

            viewLayoutTable.ReLayout();
           
        }


        void CreateItemLayoutTable(int itemCount)
        {
            float w = (itemWidth + itemMargin.left + itemMargin.right) * colItemCount;
            rowItemCount = itemCount / colItemCount;

            int m = itemCount % colItemCount;
            if (m > 0) { rowItemCount--; }
            float contentHeight = rowItemCount * (itemHeight + itemMargin.top + itemMargin.bottom);

            ////   
            itemLayoutTable = new TableEx(rowItemCount, colItemCount);
            itemLayoutTable.SetTablePosition(0, 0);
            itemLayoutTable.SetTableSize(w, contentHeight);

            itemLayoutTable.ReLayout();
        }


        void SetTableData()
        {
            headerLayoutTable.SetAutoTableLine(colItemCount, LineDir.VERTICAL);

            //for (int i = 0; i < colItemCount; i++)
            //{
            //    Margin margin = new Margin(20, 0, 20, 0);
            //    table.SetCellMargin(0, i, margin);
            //}
            //

            float fontSize = 10f;
            Font font = new Font("微软雅黑", fontSize, FontStyle.Bold);

            //
            CreateColumnNameRow(headerLayoutTable, headerNames, font, null);
        }

        void CreateColumnNameRow(TableEx table, string[] columnNames, Font font, CellValueInfo.ValueProcessHandler process)
        {
            List<CellValueInfo> cellValueInfoList = new List<CellValueInfo>();
            CellValueInfo cellInfo;

            for (int i = 0; i < table.colAmount; i++)
            {
                cellInfo = new CellValueInfo();
                cellInfo.type = CellValueType.String;
                cellInfo.valueProcess += process;

                cellInfo.value = columnNames[i];
                cellInfo.font = font;
                cellValueInfoList.Add(cellInfo);
            }

            table.CreateNewAutoRow(cellValueInfoList.ToArray());
        }

      
    }
}

