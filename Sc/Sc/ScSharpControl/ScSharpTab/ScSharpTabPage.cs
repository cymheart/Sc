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
    public partial class ScSharpTabPage : UserControl
    {
        string text;
        public ScTabHeadItem scTabHeadItem;

        public delegate void TextChangedEventHandler(object sender, string text);
        public event TextChangedEventHandler TextChangedEvent;
        public ScSharpTabPage()
        {
            InitializeComponent();

            #region 防止打开闪烁
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            #endregion
        }

        public override string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
                if (TextChangedEvent != null)
                    TextChangedEvent(this, value);
            }
        }
    }
}
