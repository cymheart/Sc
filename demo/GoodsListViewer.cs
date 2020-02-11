using Sc;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace demo
{
    public class TestData
    {
        public string test;
    }

    public class GoodsListViewer : IDisposable
    {
        public ScMgr scMgr;
        ScGridView gridView;
        ScLayer root;

        List<TestData> testDatalistFront = new List<TestData>();
        List<TestData> testDatalistBack = new List<TestData>();


        //
        ScListView listView;


        public GoodsListViewer(Control control)
        {
            scMgr = new ScMgr(control.Width, control.Height);
            scMgr.BackgroundColor = Color.FromArgb(255, 246, 245, 251);
            control.Controls.Add(scMgr.control);
            scMgr.control.Dock = DockStyle.Fill;
            root = scMgr.GetRootLayer();
            root.Dock = ScDockStyle.Fill;
            // root.Padding = new Utils.Margin(100, 100, 100, 100);


            gridView = new ScGridView(scMgr);

          
           

            //样式设置
            Setting();


            //生成列
            CreateColumnSetting();


            root.Add(gridView);

         

            scMgr.ReBulid();
            CreateBackDataList();

            List<TestData> tmp = testDatalistFront;
            testDatalistFront = testDatalistBack;
            testDatalistBack = tmp;
            testDatalistBack.Clear();

            UpdateDataSource();



            //
         
        }


        /// <summary>
        /// 样式设置
        /// </summary>
        void Setting()
        {
            gridView.Dock = ScDockStyle.Fill;

            //透明度背景色设置
            gridView.Opacity = 1.0f;
            gridView.BackgroundColor = Color.FromArgb(255, 255, 255, 255);

            //滚动条设置
            gridView.VerScrollSize = 10;
            gridView.HorScrollSize = 10;
            gridView.ScrollBarSliderColor = Color.FromArgb(255, 100, 100, 100);


            //边距设置
            gridView.IsUseInside = true;
            gridView.Margin = new Utils.Margin(50, 70, 50, 70);
            gridView.SideSpacing = new Utils.Margin(10, 10, 10, 10);
            gridView.OutsideLineColor = Color.FromArgb(255, 220, 220, 220);
            gridView.InsideLineColor = Color.FromArgb(255, 200, 200, 200);


            //列头设置
            gridView.HeaderStyle = 0;
            gridView.HeaderSpacing = 1;
            gridView.HeaderHeight = 50;
            //gridView.HeaderBackGroundColor = Color.FromArgb(255, 0, 0, 0);
            gridView.HeaderSizeMode = ScLayerLayoutViewerHeaderSizeMode.ADAPTIVE;
            gridView.HeaderControlerSize = 20;


            //内容行设置
            gridView.RowHeight = 60f;
            gridView.RowSpacing = 1f;
            //gridView.ContainerBackGroundColor = Color.FromArgb(255, 0, 0, 155);
            gridView.ItemMinSize = 20;
          


            //阴影设置
            gridView.IsUseShadow = true;
            gridView.ShadowStyle = 2;
            gridView.ShadowRange = 15;
            gridView.ShadowCornersRadius = 1;
            gridView.ShadowColor = Color.FromArgb(250, 0, 0, 0);


            //列头标题
            gridView.CreateHeaderTitleEvent += GridView_CreateHeaderTitleEvent;
            gridView.CreateHeaderTitleLayer();
        }


        private ScLayer GridView_CreateHeaderTitleEvent(ScMgr scmgr)
        {
            ScLabel headerLabel = new ScLabel(scmgr);
            headerLabel.Name = "Title";
            headerLabel.Dock = ScDockStyle.Fill;
            headerLabel.Text = "订单列表";
            headerLabel.TextPadding = new Margin(20, 0, 0, 0);
            headerLabel.ForeFont = new D2DFont("微软雅黑", 35, SharpDX.DirectWrite.FontWeight.Bold);
            headerLabel.Alignment = TextAlignment.Leading;
            headerLabel.BackgroundColor = Color.FromArgb(100, 255, 0, 0);

            return headerLabel;
        }


        /// <summary>
        /// 生成列
        /// </summary>
        void CreateColumnSetting()
        {
            ColumnSetting columnSetting = new ColumnSetting("Test", "测试列1", true, false, 200);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new ColumnSetting("Test2", "测试列2", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest1;
            columnSetting.DisplayItemValue += DisplayItem1;
            gridView.AppendColumnSetting(columnSetting);

            columnSetting = new ColumnSetting("Test3", "测试列3", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest3;
            columnSetting.DisplayItemValue += DisplayItem3;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new ColumnSetting("Test4", "测试列4", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new ColumnSetting("Test5", "测试列5", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new ColumnSetting("Test6", "测试列6", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new ColumnSetting("Test7", "测试列7", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new ColumnSetting("Test8", "测试列8", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new ColumnSetting("Test9", "测试列9", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new ColumnSetting("Test10", "测试列10", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);

            gridView.AppendColumnSettingEnd();
        }

        ScLayer CreateHeaderControlFieldTest(ScMgr scmgr, ColumnSetting columnSetting)
        {
            ScLabel label = new ScLabel(scmgr);
            label.Dock = ScDockStyle.Fill;  
            label.ForeFont = new D2DFont("微软雅黑", 17, SharpDX.DirectWrite.FontWeight.Bold);

            if(!columnSetting.columnBaseInfo.isHideName)
                label.Text = columnSetting.columnBaseInfo.displayName;

            return label;
        }


        ScLayer CreateItemControlFieldTest1(ScMgr scmgr, ColumnSetting columnSetting)
        {
            ScLayer layer = new ScLayer(scmgr);
            layer.Dock = ScDockStyle.Fill;

            ScCheckBox checkBox = new ScCheckBox(scmgr);
            checkBox.CheckType = 0;
            checkBox.boxSideWidth = 1;
            checkBox.FillMargin = new Margin(2, 2, 3, 3);
            checkBox.CheckColor = Color.DarkRed;
            checkBox.Dock = ScDockStyle.Center;
            checkBox.Size = new SizeF(15, 15);

            checkBox.SetDrawCheckDirectParentLayer(layer);
            layer.Add(checkBox);
            return layer;

        }


        ScLayer CreateItemControlFieldTest(ScMgr scmgr, ColumnSetting columnSetting)
        {
            ScLabel label = new ScLabel(scmgr);
            label.Dock = ScDockStyle.Fill;
            label.ForeFont = new D2DFont("微软雅黑", 17, SharpDX.DirectWrite.FontWeight.Bold);
            label.ForeColor = Color.FromArgb(255, 58, 166, 254);
            return label;
        }


        ScLayer CreateItemControlFieldTest3(ScMgr scmgr, ColumnSetting columnSetting)
        {
            listView = new ScListView(scmgr);
            listView.Name = "ListView";
            listView.IsUseShadow = false;
            listView.ShadowRange = 4;
            listView.Margin = new Margin(10, 10, 10, 10);

            listView.DisplayItemValue += DisplayItem;
            listView.CreateDefaultContentInfoSeting();
            listView.Dock = ScDockStyle.Fill;
  
            ScLayer listViewPack;
            if (listView.IsUseShadow)
            {
                listViewPack = new ScLayer();
                listViewPack.Name = "ListViewPack";
                listViewPack.Dock = ScDockStyle.Fill;
                listViewPack.Add(listView);
                return listViewPack;
            }
            else
            {
                return listView;
            }
        }

        void DisplayItem(ScLayer columnItem, int dataRowIdx)
        {
            ScLabel label = (ScLabel)columnItem;

            if (label == null)
                return;

            if (dataRowIdx % 2 == 0)
            {
                label.ForeColor = Color.FromArgb(255, 0, 0, 0);
                label.ForeFont = new D2DFont("微软雅黑", 12, SharpDX.DirectWrite.FontWeight.Regular);
            }
            else
            {
                label.ForeColor = Color.FromArgb(255, 0, 0, 255);
                label.ForeFont = new D2DFont("微软雅黑", 17, SharpDX.DirectWrite.FontWeight.Bold);
            }


            label.Text = testDatalistFront[dataRowIdx].test;
            label.Value = label.Text;
        }

        void DisplayItem1(ScLayer columnItem, int dataRowIdx)
        {
           
        }

        void DisplayItem3(ScLayer columnItem, int dataRowIdx)
        {
            ScListView listView;

            if(columnItem.Name == "ListViewPack")
                listView = (ScListView)(columnItem.controls[1]);
            else
                listView = (ScListView)(columnItem);

            listView.ResetDataRowCount(testDatalistFront.Count());

        }
        public void UpdateDataSource()
        {
          
            gridView.ResetDataRowCount(testDatalistFront.Count());
           // listView.ResetDataRowCount(testDatalistFront.Count());
        }


        public void CreateBackDataList()
        {
            testDatalistBack.Clear();

            for (int i = 0; i < 106; i++)
            {
                TestData testData = new TestData();
                testData.test = "测试数据" + i;
                testDatalistBack.Add(testData);
            }

        }



        public void Dispose()
        {
            scMgr.Dispose();
        }
    }
}
