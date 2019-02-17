using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ControlList
    {
        public Dictionary<ScTabHeadItem, Control> tabPageDict = new Dictionary<ScTabHeadItem, Control>();

        ScSharpTab scSharpTab;

        public ControlList(ScSharpTab scSharpTab)
        {
            this.scSharpTab = scSharpTab;
        }
        public void Add(Control control)
        {
            ScSharpTabPage tabPage = (ScSharpTabPage)control;
            tabPage.TextChangedEvent += TabPage_TextChangedEvent;

            ScTabHeadItem item = scSharpTab.head.AddItem(control.Text);
            tabPageDict.Add(item, control);

            ScSharpTabPage scSharpTabPage = (ScSharpTabPage)control;
            scSharpTabPage.scTabHeadItem = item;
        }

        private void TabPage_TextChangedEvent(object sender, string text)
        {
            foreach(var item in tabPageDict)
            {
                if(item.Value == sender)
                {
                    item.Key.Name = text;
                }
            }
        }
    }
}
