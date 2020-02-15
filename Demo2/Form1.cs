using Sc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo2
{
    public partial class Form1 : Form
    {
        ScMgr scMgr;
        ScLayer root;
        App app;
        UpdateLayerFrm frm;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //////////////////////////////////////////
            //透明无边框窗口测试代码，测试时打开下面的注释
            scMgr = new ScMgr(null, true);
            scMgr.BackgroundColor = Color.FromArgb(100, 0, 0, 251);
            app = new App(scMgr);
            scMgr.ReBulid();
            scMgr.Show();

            ////////////////////////////////////
            //常规窗口测试代码，测试时打开下面的注释
            //scMgr = new ScMgr(panel);
            //scMgr.BackgroundColor = Color.FromArgb(255, 246, 245, 251);
            //app = new App(scMgr);
            //scMgr.ReBulid();
        }


        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if(scMgr != null)  
                scMgr.Refresh();
        }
    }
}
