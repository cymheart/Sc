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

namespace demo
{
    public partial class Form1 : Form
    {
        GoodsListViewer goodsListViewer;
        public Form1()
        {
            InitializeComponent();

            Load += Form1_Load;
            SizeChanged += Form1_SizeChanged;  
        }



        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //goodsListViewer.CreateBackDataList();
            goodsListViewer.UpdateDataSource();
            panel.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            goodsListViewer = new GoodsListViewer(panel);
            
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
