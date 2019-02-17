using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace Sc
{
    public class ScVxSlideTools : ScLayer
    {
        ScLayer wrapper;
        float downWrapperX;
        float downWrapperLocationX;

        public float itemWidth;
        public float itemHeight;

        List<ScLayer> itemList = new List<ScLayer>();


        public ScVxSlideTools()
        {
            wrapper = new ScLayer();
            Add(wrapper);

            SizeChanged += ScVxSlideTools_SizeChanged;
            

            MouseDown += ScScrollContainer_MouseDown;
            MouseMove += ScScrollContainer_MouseMove;
            MouseUp += ScScrollContainer_MouseUp;

        }

        private void ScVxSlideTools_SizeChanged(object sender, SizeF oldSize)
        {
            wrapper.Height = Height;

            float itemTotalWidth = itemWidth * itemList.Count();

            if (itemTotalWidth < Width)
                wrapper.Width = Width;
            else
                wrapper.Width = itemTotalWidth;

            wrapper.Location = new PointF(0, 0);


            RectangleF tableRect = new RectangleF(0, 0, wrapper.Width, wrapper.Height);
            Table  toolstable = new Table(tableRect, 1, itemList.Count());
            int i = 0;
            RectangleF itemRect;

            wrapper.Clear();

            foreach (ScLayer item in itemList)
            {
                itemRect = toolstable.GetCellContentRect(0, i++);
                item.Location = new PointF((int)itemRect.X, (int)itemRect.Y);
                item.Width = (int)itemRect.Width;
                item.Height = (int)itemRect.Height;
                wrapper.Add(item);     
            }
        }


        public void AddItems(List<ScLayer> items)
        {
            foreach (ScLayer item in items)
            {
                itemList.Add(item);

                item.MouseDown += Item_MouseDown;
            }
        }

        private void Item_MouseDown(object sender, ScMouseEventArgs e)
        {
            foreach(ScVxButton2 item in itemList)
            {
                if (item != sender)
                {
                    item.isSelected = false;
                    item.Refresh();
                }
            }
           
        }

        private void ScScrollContainer_MouseUp(object sender, ScMouseEventArgs e)
        {
            // SetCursor(Cursors.Hand);
        }



        private void ScScrollContainer_MouseMove(object sender, ScMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float offsetX = e.Location.X - downWrapperX;
                float x = downWrapperLocationX + offsetX;

                if (x > 0)
                {
                    x = 0;
                    offsetX= -downWrapperLocationX;
                }
                else if (x + wrapper.Width < Width)
                {
                    x = Width - wrapper.Width;
                    offsetX = Height - (downWrapperLocationX + wrapper.Width);
                }

                wrapper.Location = new PointF(x, wrapper.Location.Y);

                Refresh();
            }           
        }

        private void ScScrollContainer_MouseDown(object sender, ScMouseEventArgs e)
        {
            downWrapperX = e.Location.X;
            downWrapperLocationX = wrapper.Location.X;
        }


        public ScLayer GetContainer()
        {
            return wrapper;
        }
    }
}
