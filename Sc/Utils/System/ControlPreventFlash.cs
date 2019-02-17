
using System.Drawing;
using System.Windows.Forms;

namespace Utils
{
    public class ControlDoubleBuffer
    {
        static public void SetControlsDoubleBuffer(Control parentCtl)
        {
            foreach (Control c in parentCtl.Controls)
            {
                c.GetType().GetProperty(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic).SetValue(c, true, null);

                if (c.Controls.Count > 0)
                    SetControlsDoubleBuffer(c);//窗体内其余控件还可能嵌套控件(比如panel),要单独抽出,因为要递归调用
            }
        }

    }


    public class MySplitContainer : SplitContainer
    {
        public MySplitContainer()
        {
            this.SetStyle(
                  ControlStyles.UserPaint |
                  ControlStyles.AllPaintingInWmPaint |
                  ControlStyles.OptimizedDoubleBuffer, true);
        }
    }


    public class MyPanel:Panel
    {
        protected override Point ScrollToControl(Control activeControl)
        {
            return this.AutoScrollPosition;
        }
    }
}
