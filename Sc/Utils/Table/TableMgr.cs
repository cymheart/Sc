using System.Drawing;

namespace Utils
{
    public class TableMgr
    {
        TableEx rootTable;


        public TableMgr()
        {

        }

        public void SetTable(TableEx table)
        {
            rootTable = table;
        }

        public void ReLayout(Graphics g = null)
        {
            rootTable.ReLayout(g);
        }

        public void ProcessValue()
        {
            rootTable.ProcessAllCellValue();
        }
    }
}
