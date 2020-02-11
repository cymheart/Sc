using System;
using System.Windows.Forms;

namespace PrintUtils
{
    public partial class AddPrintModuleFrm : Form
    {
        public PrintModuleInfo printModuleInfo = new PrintModuleInfo();
        public AddPrintModuleFrm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            printModuleInfo.key = textBoxKey.Text;
            printModuleInfo.name = textBoxName.Text;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
