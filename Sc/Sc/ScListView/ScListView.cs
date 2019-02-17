using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{

    public partial class ScListView : ScScrollContainer
    { 
        public float itemWidth;
        public float itemHeight = 50;
        Margin itemMargin = new Margin(0,0,0,0);
        int colItemCount = 1;
        int rowItemCount = 1;

        ScLayer headerLayer;
    
        public float headerHeight = 50;
        public float contentViewHeight;

        TableEx viewLayoutTable;
        TableEx headerLayoutTable;
        TableEx itemLayoutTable;
        public TableEx itemContentLayoutTable;

        public string[] headerNames;

        ScListViewItem selectedItem;
        ScListViewItem mouseHoverItem;

        public ScListView()
        {
            headerLayer = new ScLayer();
            Add(headerLayer);

            SizeChanged += ScListView_SizeChanged;
            wrapper.SizeChanged += Wrapper_SizeChanged;

            IsHitThrough = true;
        }

      

        public void AddItem(ScLayer[] fieldElemLayers, ScListViewItem.ReLayoutEventHandler itemReLayoutEvent = null)
        {
            CreateItemLayoutTable(wrapper.controls.Count() + 1);

            int idx = wrapper.controls.Count();
            int rowIdx = idx / colItemCount;
            int colIdx = idx % colItemCount;
            RectangleF r = itemLayoutTable.GetCellRect(rowIdx, colIdx);
            r= itemLayoutTable.TransToGlobalRect(r);

            ScListViewItem item = new ScListViewItem(fieldElemLayers);
            item.ReLayoutEvent += itemReLayoutEvent;
            item.listView = this;
            item.DirectionRect = r;

            item.GotFocus += Item_GotFocus;
            item.MouseHoverEvent += Item_MouseHoverEvent;
            item.LostFocus += Item_LostFocus;
 
            AddContentControl(item);      
            item.Focus();
            Refresh();
        }

        
        private void Item_MouseHoverEvent(object sender)
        {
            mouseHoverItem = (ScListViewItem)sender;
        }
        private void Item_GotFocus(object sender, EventArgs e)
        {
            selectedItem = (ScListViewItem)sender;
        }

        private void Item_LostFocus(object sender, EventArgs e)
        {
            if(selectedItem == sender)
                selectedItem = null;
        }

        public void RemoveSelectedItem()
        {
            ScListViewItem newSelectedItem = null;
            int idx = controls.IndexOf(selectedItem);

            if (idx < wrapper.controls.Count() - 1)
                newSelectedItem = (ScListViewItem)wrapper.controls.ElementAt(idx + 1);
            else if(idx > 0)
                newSelectedItem = (ScListViewItem)wrapper.controls.ElementAt(idx - 1);

            SuspendLayout();
            RemoveContentControl(selectedItem);      
            ResumeLayout(true);
            newSelectedItem.Focus();
            Refresh();
        }
        public void RemoveItem(int idx)
        {
            ScListViewItem item;
            ScListViewItem newSelectedItem = null;
            int selectedIdx = controls.IndexOf(selectedItem);

            if (selectedIdx == idx)
            {
                item = selectedItem;

                if (idx < wrapper.controls.Count() - 1)
                    newSelectedItem = (ScListViewItem)wrapper.controls.ElementAt(idx + 1);
                else if (idx > 0)
                    newSelectedItem = (ScListViewItem)wrapper.controls.ElementAt(idx - 1);
            }
            else
            {
                item = (ScListViewItem)controls.ElementAt(idx);
            }


            SuspendLayout();
            RemoveContentControl(item);
            ResumeLayout(true);

            if (newSelectedItem != null)
                newSelectedItem.Focus();

            Refresh();
        }


        public void RemoveItem(ScListViewItem item)
        {
            ScListViewItem newSelectedItem = null;
            int selectedIdx = controls.IndexOf(selectedItem);

            if (selectedItem == item)
            {
                if (selectedIdx < wrapper.controls.Count() - 1)
                    newSelectedItem = (ScListViewItem)wrapper.controls.ElementAt(selectedIdx + 1);
                else if (selectedIdx > 0)
                    newSelectedItem = (ScListViewItem)wrapper.controls.ElementAt(selectedIdx - 1);
            }

            SuspendLayout();
            RemoveContentControl(item);
            ResumeLayout(true);

            if (newSelectedItem != null)
                newSelectedItem.Focus();

            Refresh();
        }



        private void ScListView_SizeChanged(object sender, SizeF oldSize)
        {
            CreateViewLayoutTable(wrapper.controls.Count());

            //
            RectangleF rc = headerLayoutTable.GetRowRect(0);

            rc = headerLayoutTable.GetTableRect();
            headerLayer.DirectionRect = rc;

            rc = viewLayoutTable.GetRowRect(1);
            view.DirectionRect = rc;

        }

        private void Wrapper_SizeChanged(object sender, SizeF oldSize)
        {          
            //    
            //int idx = 0;
            //int rowIdx = 0;
            //int colIdx = 0;
            //RectangleF rect;

            //CreateItemLayoutTable(wrapper.controls.Count());

            //foreach (ScListViewItem itemLayer in wrapper.controls)
            //{    
            //    rowIdx = idx / colItemCount;
            //    colIdx = idx % colItemCount;

            //    rect = itemLayoutTable.GetCellRect(rowIdx, colIdx);
            //    itemLayer.ReLayout(rect);
            //    idx++;
            //}
        }

    }
}
