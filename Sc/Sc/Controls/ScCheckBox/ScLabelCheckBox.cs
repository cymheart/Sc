using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class ScLabelCheckBox : ScLayer
    {
        public ScCheckBox CheckBox;
        ScLabel Label;
        public ScLabelCheckBox(ScMgr scmgr = null)
            :base(scmgr)
        {
            CheckBox = new ScCheckBox(scmgr);
            Add(CheckBox);

            Label = new ScLabel(scmgr);
            Add(Label);

            SizeChanged += ScLabelCheckBox_SizeChanged;
        }

        private void ScLabelCheckBox_SizeChanged(object sender, System.Drawing.SizeF oldSize)
        {
             
        }
    }
}
