using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScListView : ScGridView
    {
        public ColumnSetting.CreateControlHandler CreateItemControl;
        public ColumnSetting.DisplayItemHandler DisplayItemValue;

        public ScListView(ScMgr scmgr)
            : base(scmgr)
        {         
            //样式设置
            Setting();
        }


        /// <summary>
        /// 样式设置
        /// </summary>
        void Setting()
        {
            //透明度背景色设置
            Opacity = 1.0f;
            BackgroundColor = Color.FromArgb(255, 255, 255, 255);

            //滚动条设置
            VerScrollSize = 10;
            HorScrollSize = 10;
            ScrollBarSliderColor = Color.FromArgb(255, 100, 100, 100);


            //边距设置
            IsUseInside = true;
            Margin = new Utils.Margin(0, 0, 0, 0);
            SideSpacing = new Utils.Margin(3,3, 3, 3);
            OutsideLineColor = Color.FromArgb(0, 220, 220, 220);
            InsideLineColor = Color.FromArgb(0, 200, 200, 200);


            //列头设置
            HeaderStyle = 0;
            HeaderSpacing = 0;
            HeaderHeight = 0;
            //HeaderBackGroundColor = Color.FromArgb(255, 0, 0, 0);
            HeaderSizeMode = ScLayerLayoutViewerHeaderSizeMode.ADAPTIVE;
            HeaderControlerSize = 0;


            //内容行设置
            RowHeight = 30f;
            RowSpacing = 0f;
            //ContainerBackGroundColor = Color.FromArgb(255, 0, 0, 155);
            ItemMinSize = 20;


            //阴影设置
            IsUseShadow = true;
            ShadowStyle = 2;
            ShadowRange = 15;
            ShadowCornersRadius = 1;
            ShadowColor = Color.FromArgb(250, 0, 0, 0);

        }


        /// <summary>
        /// 生成列
        /// </summary>
        public void CreateContentInfoSeting()
        {
            ClearColumnSetting();

            ColumnSetting columnSetting = new ColumnSetting("col", "列", true, false, 200);
            columnSetting.CreateHeaderControl += CreateHeaderControlField;
            columnSetting.CreateItemControl += CreateItemControl;
            columnSetting.DisplayItemValue += DisplayItemValue;
            AppendColumnSetting(columnSetting);

            AppendColumnSettingEnd();
        }


        public void CreateDefaultContentInfoSeting()
        {
            ClearColumnSetting();

            ColumnSetting columnSetting = new ColumnSetting("col", "列", true, false, 200);
            columnSetting.CreateHeaderControl += CreateHeaderControlField;
            columnSetting.CreateItemControl += CreateItemControlField;
            columnSetting.DisplayItemValue += DisplayItemValue;
            AppendColumnSetting(columnSetting);

            AppendColumnSettingEnd();
        }

        ScLayer CreateHeaderControlField(ScMgr scmgr, ColumnSetting columnSetting)
        {
            ScLabel label = new ScLabel(scmgr);
            label.Dock = ScDockStyle.Fill;
            label.ForeFont = new D2DFont("微软雅黑", 12, SharpDX.DirectWrite.FontWeight.Bold);

            if (!columnSetting.columnBaseInfo.isHideName)
                label.Text = columnSetting.columnBaseInfo.displayName;

            return label;
        }

        ScLayer CreateItemControlField(ScMgr scmgr, ColumnSetting columnSetting)
        {
            ScLabel label = new ScLabel(scmgr);
            label.Dock = ScDockStyle.Fill;
            label.ForeFont = new D2DFont("微软雅黑", 12, SharpDX.DirectWrite.FontWeight.Regular);
            label.ForeColor = Color.FromArgb(255, 58, 166, 254);
            return label;
        }
    }
}
