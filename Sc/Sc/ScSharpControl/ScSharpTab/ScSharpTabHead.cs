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
    public partial class ScSharpTabHead : UserControl
    {
        ScMgr scMgr;
        ScLayer root;
        ScTabHead scTabHead;

        public ScSharpTabHead()
        {
            InitializeComponent();
  
            #region 防止打开闪烁
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            #endregion

            this.Dock = DockStyle.Fill;

            scMgr = new ScMgr(Width, Height);
            scMgr.BackgroundColor = Color.FromArgb(255, 246, 247, 251);
            Controls.Add(scMgr.control);

            root = scMgr.GetRootLayer();
            scTabHead = new ScTabHead(scMgr, root.Width, root.Height);
            scTabHead.Location = new Point(0, 0);
            scTabHead.Dock = ScDockStyle.Fill;
            root.Add(scTabHead);

  
        }


        public void SetItemSize(Size size)
        {
            scTabHead.SetItemSize(size);
        }

        public ScTabHeadItem SetSelectedTabPage(int selectedIndex)
        {
            return scTabHead.SetSelectedIndex(selectedIndex);
        }

        public int SetSelectedTabPage(ScTabHeadItem selectedTab)
        {
            return scTabHead.SetSelectedItem(selectedTab);
        }

        public void AddSelectedItemEvent(ScTabHead.SelectedItemEventHandler SelectedItemEvent)
        {
            scTabHead.SelectedItemEvent += SelectedItemEvent;
        }

        public ScTabHeadItem AddItem(string name)
        {
            ScTabHeadItem item = scTabHead.AddItem(name);
            return item;
        }

    }
}
