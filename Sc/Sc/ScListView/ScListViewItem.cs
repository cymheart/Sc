using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScListViewItem : ScLayer
    {
        public delegate void MouseHoverEventHandler(object sender);
        public event MouseHoverEventHandler MouseHoverEvent = null;

        public delegate void ReLayoutEventHandler(object sender);
        public event ReLayoutEventHandler ReLayoutEvent = null;


        //
        List<RectangleF> fieldElemRectList = new List<RectangleF>();
        public ScListView listView = null;
        int state = 0;

        public ScListViewItem(ScLayer [] fieldElemLayers)
        {
            foreach(ScLayer elem in fieldElemLayers)
            {
                Add(elem);
            }

            SizeChanged += ScListViewItem_SizeChanged;
            MouseDown += ScListViewItem_MouseDown;
            MouseHover += ScListViewItem_MouseHover;
            GotFocus += ScListViewItem_GotFocus;
            LostFocus += ScListViewItem_LostFocus;
        }

        private void ScListViewItem_MouseHover(object sender, ScMouseEventArgs e)
        {
            state = 2;

            if (MouseHoverEvent != null)
                MouseHoverEvent(sender);

            Refresh();
        }

        private void ScListViewItem_LostFocus(object sender, EventArgs e)
        {
            state = 0;
            Refresh();
        }

        private void ScListViewItem_GotFocus(object sender, EventArgs e)
        {
            state = 1;
            Refresh();
        }

        private void ScListViewItem_MouseDown(object sender, ScMouseEventArgs e)
        {
            Focus();
        }

        public void ReLayout(RectangleF rect)
        {
            DirectionRect = rect;
        }

        private void ScListViewItem_SizeChanged(object sender, SizeF oldSize)
        {
            if (ReLayoutEvent != null)
            {
                ReLayoutEvent(this);
            }
            else
            {
                TableEx layoutTable = listView.itemContentLayoutTable;
                ScLayer elem;
                int rowIdx;
                int colIdx;
                RectangleF rect;
                float  y;

                for (int i=0; i< controls.Count(); i++)
                {
                    elem = controls[i];
                    rowIdx = i / layoutTable.colAmount;
                    colIdx = i % layoutTable.colAmount;
                    rect = layoutTable.GetCellRect(rowIdx, colIdx);

                    y = rect.Height / 2 - elem.Height / 2;       
                    rect.Y = (float)Math.Ceiling(y);
                    rect.X = (float)Math.Ceiling(rect.X);
                    rect.Width = (float)Math.Ceiling(rect.Width);
                    rect.Height = (float)Math.Ceiling(elem.Height);
                    elem.DirectionRect = rect;
                }
            }
        }
    }
}
