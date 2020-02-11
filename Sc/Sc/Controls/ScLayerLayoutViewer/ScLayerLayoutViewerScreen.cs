//----------------------------------------------------------------------------
// Simple Control (Sc) - Version 1.1
// A high quality control rendering engine for C#
// Copyright (C) 2016-2020 cymheart
// Contact: 
//
// 
// Sc is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// Sc is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Sc; if not, write to the Free Software
//----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScLayerLayoutViewerScreen:ScLayer
    {
        public delegate void UserLayerValueChangedEventHandler(int dataIdx, string dataName, object value);
        public event UserLayerValueChangedEventHandler UserLayerValueChangedEvent = null;

        public delegate void ViewerItemMouseEventHandler(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e);
        public event ViewerItemMouseEventHandler ViewerItemMouseEnterEvent = null;
        public event ViewerItemMouseEventHandler ViewerItemMouseDownEvent = null;
        public event ViewerItemMouseEventHandler ViewerItemMouseLeaveEvent = null;
        public event ViewerItemMouseEventHandler ViewerItemMouseDoubleDownEvent = null;

        public ScLayerLayoutViewerItem.CreateItemDataLayerEventHandler CreateItemDataLayerEvent;


        public int itemCreateCount = 0;
        public SizeF itemCreateSize;
        public float itemSpacing = 0;

        public Color ItemSelectorColor = Color.FromArgb(100, 126, 171, 255);



        public ScLayerLayoutViewerLayoutMode LayoutMode { get; set; }

        public ScLayer ItemDirectClipLayer;

        public ScLayerLayoutViewerScreen(ScMgr scmgr = null)
            :base(scmgr)
        {
            SizeChanged += ScLayerLayoutViewerScreen_SizeChanged;
            IsHitThrough = false;
        }

        public List<ScLayerLayoutViewerItem> GetSelectedViewerItems()
        {
            List<ScLayerLayoutViewerItem> selectedItemList = new List<ScLayerLayoutViewerItem>();

            foreach(ScLayerLayoutViewerItem item in controls)
            {
                if (item.IsSelected)
                    selectedItemList.Add(item);
            }

            return selectedItemList;
        }


        private void ScLayerLayoutViewerScreen_SizeChanged(object sender, SizeF oldSize)
        {

            if(controls.Count > itemCreateCount && itemCreateCount >= 0)
            {
                int count = controls.Count - itemCreateCount;

                for (int i = 0; i < count; i++)
                {
                    Remove(controls[0]);
                }
            }
            else if(controls.Count < itemCreateCount)
            {
                int count = itemCreateCount - controls.Count;
    
                for (int i = 0; i < count; i++)
                {
                    ScLayerLayoutViewerItem item = new ScLayerLayoutViewerItem(ScMgr);
                    item.CreateItemDataLayerEvent += CreateItemDataLayerEvent;
                    item.UserLayerValueChangedEvent += Item_UserLayerValueChangedEvent;
                    item.MouseEnterEvent += Item_MouseEnterEvent;
                    item.MouseDownEvent += Item_MouseDownEvent;
                    item.MouseLeaveEvent += Item_MouseLeaveEvent;
                    item.MouseDoubleDownEvent += Item_MouseDoubleDownEvent;
                    item.Selector.DirectParentClipLayer = ItemDirectClipLayer;
                    item.SelectorColor = ItemSelectorColor;
                    Add(item); 
                }
            }

            switch(LayoutMode)
            {
                case ScLayerLayoutViewerLayoutMode.MORE_ROW_SINGLE_COL_LAYOUT:
                    LayoutMoreRowSingleCol();
                    break;

                case ScLayerLayoutViewerLayoutMode.MORE_COL_SINGLE_ROW_LAYOUT:
                    LayoutMoreColSingleRow();
                    break;
            }
        }

        private void Item_MouseDoubleDownEvent(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e)
        {
            if (ViewerItemMouseDoubleDownEvent != null)
                ViewerItemMouseDoubleDownEvent(viewerItem, e);
        }

        private void Item_MouseLeaveEvent(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e)
        {
            if (ViewerItemMouseLeaveEvent != null)
                ViewerItemMouseLeaveEvent(viewerItem, e);
        }

        private void Item_MouseDownEvent(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e)
        {
            if (ViewerItemMouseDownEvent != null)
                ViewerItemMouseDownEvent(viewerItem, e);
        }

        private void Item_MouseEnterEvent(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e)
        {
            if (ViewerItemMouseEnterEvent != null)
                ViewerItemMouseEnterEvent(viewerItem, e);
        }


        private void Item_UserLayerValueChangedEvent(int dataIdx, string dataName, object value)
        {
            if (UserLayerValueChangedEvent != null)
                UserLayerValueChangedEvent(dataIdx, dataName, value);
        }

        void LayoutMoreRowSingleCol()
        {
            float x = 0;
            float y = 0;

            RectangleF rect = new RectangleF();

            for (int i = 0; i < controls.Count(); i++)
            {
   
                rect.X = (float)Math.Ceiling(x);
                rect.Y = (float)Math.Ceiling(y);
                rect.Width = (float)Math.Ceiling(Width);
                rect.Height = (float)Math.Ceiling(itemCreateSize.Height);
                controls[i].DirectionRect = rect;

                y += itemCreateSize.Height + itemSpacing;
            }
        }


        void LayoutMoreColSingleRow()
        {
            float x = 0;
            float y = 0;

            RectangleF rect = new RectangleF();

            for (int i = 0; i < controls.Count(); i++)
            {
                rect.X = (float)Math.Ceiling(x);
                rect.Y = (float)Math.Ceiling(y);
                rect.Width = (float)Math.Ceiling(itemCreateSize.Width);
                rect.Height = (float)Math.Ceiling(Height);
                controls[i].DirectionRect = rect;

                x += itemCreateSize.Width + itemSpacing;
            }
        }

    }
}
