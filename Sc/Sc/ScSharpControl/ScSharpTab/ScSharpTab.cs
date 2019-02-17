using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public partial class ScSharpTab : UserControl
    {
        public ScSharpTabHead head;
        public ControlList Controlsx;
        Size itemSize;
        int selectedIndex;
        bool multiline;
        Point padding;
        ScSharpTabPage selectedTab;

        public event EventHandler SelectedIndexChanged;
        new public event EventHandler Click;

        public ScSharpTab()
        {

            InitializeComponent();


            head = new ScSharpTabHead();
            head.Dock = DockStyle.Fill;
            panelTabHead.Controls.Add(head);

            head.AddSelectedItemEvent(ScTabHead_SelectedItemEvent);

            Controlsx = new ControlList(this);
        }

        public Size ItemSize
        {
            get { return itemSize; }

            set
            {
                itemSize = value;
                tableLayoutPanel1.RowStyles[0].Height = itemSize.Height;
                head.SetItemSize(itemSize);
            }
        }

        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;
                ScTabHeadItem item = head.SetSelectedTabPage(selectedIndex);

                if (item == null)
                    return;

                if (Controlsx.tabPageDict.ContainsKey(item))
                    selectedTab = (ScSharpTabPage)Controlsx.tabPageDict[item];
                else
                    selectedTab = null;
            }
        }


        public ScSharpTabPage SelectedTab
        {
            get
            {
                return selectedTab;
            }
            set
            {
                selectedTab = value;
                selectedIndex = head.SetSelectedTabPage(selectedTab.scTabHeadItem);
            }

        }

        public List<ScSharpTabPage> TabPages
        {
            get
            {
                List<ScSharpTabPage> tabPageList = new List<ScSharpTabPage>();

                foreach(var item in Controlsx.tabPageDict)
                {
                    tabPageList.Add((ScSharpTabPage)item.Value);
                }

                return tabPageList;
            }
        }

        public bool Multiline
        {
            get { return multiline; }
            set { multiline = value; }
        }

        new public Point Padding
        {
            get { return padding; }
            set { padding = value; }
        }

        private void ScTabHead_SelectedItemEvent(ScTabHeadItem selectedItem)
        {
            if (Controlsx.tabPageDict.ContainsKey(selectedItem))
            {
                selectedIndex = selectedItem.index;

                Control tabBodyViewBox = Controlsx.tabPageDict[selectedItem];
                tabBodyViewBox.Dock = DockStyle.Fill;

                panelTabBody.Controls.Clear();
                panelTabBody.Controls.Add(tabBodyViewBox);
                panelTabBody.Refresh();

                if (SelectedIndexChanged != null)
                {
                    EventArgs e = new EventArgs();
                    SelectedIndexChanged(this, e);
                }

                if(Click != null)
                {
                    EventArgs e = new EventArgs();
                    Click(this, e);
                }
            }
        }

    }
}
