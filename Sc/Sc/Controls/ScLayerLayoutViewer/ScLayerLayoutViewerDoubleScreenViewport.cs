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


using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScLayerLayoutViewerDoubleScreenViewport : ScLayer
    {
        public delegate void UserLayerValueChangedEventHandler(int dataIdx, string dataName, object value);
        public event UserLayerValueChangedEventHandler UserLayerValueChangedEvent = null;

        public delegate void ItemDataSetValueEventHandler(ScLayerLayoutViewerItem[] items, int dataStartIdx, int dataEndIdx);
        public event ItemDataSetValueEventHandler ItemDataSetValueEvent = null;

        public delegate void ViewerItemMouseEventHandler(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e);
        public event ViewerItemMouseEventHandler ViewerItemMouseEnterEvent = null;
        public event ViewerItemMouseEventHandler ViewerItemMouseDownEvent = null;
        public event ViewerItemMouseEventHandler ViewerItemMouseLeaveEvent = null;
        public event ViewerItemMouseEventHandler ViewerItemMouseDoubleDownEvent = null;

        public ScLayerLayoutViewerItem.CreateItemDataLayerEventHandler CreateItemDataLayerEvent;


        ScLayerLayoutViewerScreen screen1;
        ScLayerLayoutViewerScreen screen2;

        public SizeF itemSize = new SizeF(100,100);
        public float itemSpacing = 0;

        int itemTotalCount = 0;
        SizeF contentSize;

        public float ContentPos;

        int itemCreateCount;

        ScLayerLayoutViewerLayoutMode layoutMode;

        public ScLayer ItemDirectClipLayer
        {
            set
            {
                screen1.ItemDirectClipLayer = value;
                screen2.ItemDirectClipLayer = value;
            }
        }

        public Color ItemSelectorColor
        {
            get { return screen1.ItemSelectorColor; }
            set
            {
                screen1.ItemSelectorColor = value;
                screen2.ItemSelectorColor = value;
            }
        }



        public ScLayerLayoutViewerScreen InSideScreen1 { get { return screen1; } }
        public ScLayerLayoutViewerScreen InSideScreen2 { get { return screen2; } }

        public ScLayerLayoutViewerDoubleScreenViewport(ScMgr scmgr = null)
            :base(scmgr)
        {
            screen1 = new ScLayerLayoutViewerScreen(scmgr);
            screen1.UserLayerValueChangedEvent += Screen_UserLayerValueChangedEvent;
            screen1.ViewerItemMouseEnterEvent += Screen_ViewerItemMouseEnterEvent;
            screen1.ViewerItemMouseDownEvent += Screen_ViewerItemMouseDownEvent;
            screen1.ViewerItemMouseLeaveEvent += Screen_ViewerItemMouseLeaveEvent;
            screen1.ViewerItemMouseDoubleDownEvent += Screen_ViewerItemMouseDoubleDownEvent;
            screen1.Visible = false;
            //screen1.D2DPaint += Screen1_D2DPaint;
            Add(screen1);
            
            screen2 = new ScLayerLayoutViewerScreen(scmgr);
            screen2.UserLayerValueChangedEvent += Screen_UserLayerValueChangedEvent;
            screen2.ViewerItemMouseEnterEvent += Screen_ViewerItemMouseEnterEvent;
            screen2.ViewerItemMouseDownEvent += Screen_ViewerItemMouseDownEvent;
            screen2.ViewerItemMouseLeaveEvent += Screen_ViewerItemMouseLeaveEvent;
            screen2.ViewerItemMouseDoubleDownEvent += Screen_ViewerItemMouseDoubleDownEvent;
            screen2.Visible = false;
            // screen2.D2DPaint += Screen2_D2DPaint;
            Add(screen2);

            //
            SizeChanged += ScLayoutLayoutViewerDoubleScreenViewport_SizeChanged;


        }

        private void Screen_ViewerItemMouseDoubleDownEvent(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e)
        {
            if (ViewerItemMouseDoubleDownEvent != null)
                ViewerItemMouseDoubleDownEvent(viewerItem, e);
        }

        private void Screen_ViewerItemMouseLeaveEvent(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e)
        {
            if (ViewerItemMouseLeaveEvent != null)
                ViewerItemMouseLeaveEvent(viewerItem, e);
        }

        private void Screen_ViewerItemMouseDownEvent(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e)
        {
            if (ViewerItemMouseDownEvent != null)
                ViewerItemMouseDownEvent(viewerItem, e);
        }

        private void Screen_ViewerItemMouseEnterEvent(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e)
        {
            if (ViewerItemMouseEnterEvent != null)
                ViewerItemMouseEnterEvent(viewerItem, e);
        }

        private void Screen_UserLayerValueChangedEvent(int dataIdx, string dataName, object value)
        {
            if (UserLayerValueChangedEvent != null)
                UserLayerValueChangedEvent(dataIdx, dataName, value);
        }

        private void Screen1_D2DPaint(D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;

            RawRectangleF rect = new RawRectangleF(0, 0, (float)Math.Ceiling(screen1.Width - 1), (float)Math.Ceiling(screen1.Height - 1));
            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(100, 255, 0, 0));
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.FillRectangle(rect, brush);

        }

        private void Screen2_D2DPaint(D2DGraphics g)
        {

            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;

            RawRectangleF rect = new RawRectangleF(0, 0, (float)Math.Ceiling(screen2.Width - 1), (float)Math.Ceiling(screen2.Height - 1));
            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(100, 0, 255, 0));
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.FillRectangle(rect, brush);

          
        }

        public void SetScreenLocationY(float pos)
        {
            screen1.Location = new PointF(screen1.Location.X, pos);
            screen2.Location = new PointF(screen2.Location.X, pos);
         //   Refresh();
        }

        public void SetScreenLocationX(float pos)
        {
            screen1.Location = new PointF(pos, screen1.Location.Y);
            screen2.Location = new PointF(pos, screen2.Location.Y);
           // Refresh();
        }

        public PointF GetScreenLocation()
        {
            return screen1.Location;
        }

        public void ClearSelectedItems()
        {
            foreach(ScLayerLayoutViewerItem item in screen1.controls)
            {
                item.IsSelected = false;
            }

            foreach (ScLayerLayoutViewerItem item in screen2.controls)
            {
                item.IsSelected = false;
            }
        }
    
        public List<ScLayer> GetScreen1AllItem()
        {
            return screen1.controls;
        }

        public List<ScLayer> GetScreen2AllItem()
        {
            return screen2.controls;
        }

        public ScLayerLayoutViewerLayoutMode LayoutMode
        {
            get { return layoutMode; }
            set
            {
                layoutMode = value;
                screen1.LayoutMode = value;
                screen2.LayoutMode = value;
            }
        }

        public SizeF ContentSize
        {
            get
            {
                return contentSize;
            }
        }

        public List<ScLayerLayoutViewerItem> GetAllViewerItems()
        {
            List<ScLayerLayoutViewerItem> selectedItemList = new List<ScLayerLayoutViewerItem>();

            foreach (ScLayerLayoutViewerItem item in screen1.controls)
            {
                if (item.IsSelected)
                    selectedItemList.Add(item);
            }

            foreach (ScLayerLayoutViewerItem item in screen2.controls)
            {
                if (item.IsSelected)
                    selectedItemList.Add(item);
            }

            return selectedItemList;
        }

        public void SetItemCount(int count = 1)
        {
            RemoveAllItem();
            AppendItemCount(count);
        }

        public void AppendItemCount(int count = 1)
        {
            if (count <= 0)
                return;

            switch (LayoutMode)
            {
                case ScLayerLayoutViewerLayoutMode.MORE_ROW_SINGLE_COL_LAYOUT:

                    if (itemTotalCount != 0)
                        contentSize.Height += itemSpacing;
                    contentSize.Height += itemSize.Height;

                    for(int i=0; i< count - 1; i++)
                    {
                        contentSize.Height += itemSpacing + itemSize.Height;
                    }

                    itemTotalCount += count;

                    break;

                case ScLayerLayoutViewerLayoutMode.MORE_COL_SINGLE_ROW_LAYOUT:
                    if (itemTotalCount != 0)
                        contentSize.Width += itemSpacing;
                    contentSize.Width += itemSize.Width;

                    for (int i = 0; i < count - 1; i++)
                    {
                        contentSize.Width += itemSpacing + itemSize.Width;
                    }

                    itemTotalCount += count;

                    break;
            }
        }

        public void RemoveAllItem()
        {
            RemoveItemCount(itemTotalCount);
        }

        public void RemoveItemCount(int count = 1)
        {
            if (count > itemTotalCount)
                count = itemTotalCount;

            else if (count <= 0)
                return;

            switch (LayoutMode)
            {
                case ScLayerLayoutViewerLayoutMode.MORE_ROW_SINGLE_COL_LAYOUT:

                    if(itemTotalCount == count)
                    {
                        contentSize.Height = 0;
                        itemTotalCount = 0;
                        return;
                    }

                    for (int i = 0; i < count; i++)
                    {
                        contentSize.Height -= itemSpacing + itemSize.Height;
                    }

                    itemTotalCount -= count;

                    break;

                case ScLayerLayoutViewerLayoutMode.MORE_COL_SINGLE_ROW_LAYOUT:
                    if (itemTotalCount == count)
                    {
                        contentSize.Width = 0;
                        return;
                    }

                    for (int i = 0; i < count; i++)
                    {
                        contentSize.Width -= itemSpacing + itemSize.Width;
                    }

                    itemTotalCount -= count;

                    break;
            }
        }

        public void SetItemShowValueFromData(float pos)
        {
            switch (LayoutMode)
            {
                case ScLayerLayoutViewerLayoutMode.MORE_ROW_SINGLE_COL_LAYOUT:
                    SetItemShowValueFromDataMoreRowSingleCol(pos);
                    break;

                case ScLayerLayoutViewerLayoutMode.MORE_COL_SINGLE_ROW_LAYOUT:
                    SetItemShowValueFromDataMoreColSingleRow(pos);
                    break;
            }
        }

        public void SetItemShowValueFromDataMoreRowSingleCol(float pos)
        {
            screen1.Visible = true;
            screen2.Visible = true;

            if (Height <= 0 || contentSize.Height <= 0)
            {
                screen1.Visible = false;
                screen2.Visible = false;
                return;
            }


            if (pos + Height > contentSize.Height)
            {
                pos = contentSize.Height - Height;
            }

            if (pos < 0)
            {
                pos = 0;
            }

            if (contentSize.Height <= screen1.Height)
            {
                screen2.Visible = false;
            }


            ContentPos = pos;

            float endPos = pos + Height;
            if (endPos > contentSize.Height)
                endPos = contentSize.Height;

       
            float itemHeight = itemSize.Height + itemSpacing;

            int startScreenIdx = (int)Math.Floor(pos / screen1.Height);
            int startScreenItemIdx = (int)Math.Floor((pos - startScreenIdx * screen1.Height) / itemHeight);

            int endScreenIdx = (int)Math.Floor(endPos / screen1.Height);
            int endScreenItemIdx = (int)Math.Floor((endPos - endScreenIdx * screen1.Height) / itemHeight);

            screen1.Location = new PointF(screen1.Location.X, (float)Math.Ceiling(startScreenIdx * screen1.Height - pos));
            screen2.Location = new PointF(screen2.Location.X, (float)Math.Ceiling(screen1.Location.Y + screen1.Height));

            int dataStartIdx = startScreenIdx * itemCreateCount + startScreenItemIdx;
            int dataEndIdx = endScreenIdx * itemCreateCount + endScreenItemIdx;
            int dataPreCount = dataEndIdx - dataStartIdx + 1;

            if (dataPreCount > itemCreateCount && startScreenIdx == endScreenIdx)
            {
                int richCount = dataPreCount - itemCreateCount;
                endScreenItemIdx -= richCount;
                dataEndIdx -= richCount;
            }
            else if(dataEndIdx >= itemTotalCount)
            {
                int richCount =  dataEndIdx - itemTotalCount + 1;
                endScreenItemIdx -= richCount;
                dataEndIdx = itemTotalCount - 1;
            }
            else if(dataPreCount > itemTotalCount)
            {
                int richCount = dataPreCount - itemTotalCount;
                endScreenItemIdx -= richCount;
                dataEndIdx -= richCount;
            }


            List<ScLayerLayoutViewerItem> itemList = new List<ScLayerLayoutViewerItem>();
            ScLayerLayoutViewerItem viewerItem;
            int dataIdx = dataStartIdx;

            if (startScreenIdx == endScreenIdx)
            {
                for (int i = startScreenItemIdx; i <= endScreenItemIdx; i++)
                {
                    viewerItem = (ScLayerLayoutViewerItem)screen1.controls[i];
                    viewerItem.Visible = true;
                    viewerItem.DataIdx = dataIdx++;
                    itemList.Add(viewerItem);
                }

                for (int i = endScreenItemIdx + 1; i < screen1.controls.Count; i++)
                {
                    screen1.controls[i].Visible = false;
                }
            }
            else
            {
                for (int i = startScreenItemIdx; i < screen1.controls.Count; i++)
                {
                    viewerItem = (ScLayerLayoutViewerItem)screen1.controls[i];
                    viewerItem.Visible = true;
                    viewerItem.DataIdx = dataIdx++;
                    itemList.Add(viewerItem);
                }

                for (int i = 0; i <= endScreenItemIdx; i++)
                {
                    viewerItem = (ScLayerLayoutViewerItem)screen2.controls[i];
                    viewerItem.Visible = true;
                    viewerItem.DataIdx = dataIdx++;
                    itemList.Add(viewerItem);
                }

                for (int i = endScreenItemIdx + 1; i < screen2.controls.Count; i++)
                {
                    screen2.controls[i].Visible = false;
                }

            }

            if (ItemDataSetValueEvent != null) 
                ItemDataSetValueEvent(itemList.ToArray(), dataStartIdx, dataEndIdx);

        }


      
        public void SetItemShowValueFromDataMoreColSingleRow(float pos)
        {
            screen1.Visible = true;
            screen2.Visible = true;

            if (Height == 0 || contentSize.Width == 0)
            {
                screen1.Visible = false;
                screen2.Visible = false;
                return;
            }

            if (pos + Width > contentSize.Width)
            {
                pos = contentSize.Width - Width;
            }

            if (pos < 0)
            {
                pos = 0;
            }

            if (contentSize.Width <= screen1.Width)
            {
                screen2.Visible = false;
            }


            ContentPos = pos;

            int idx = (int)Math.Floor(pos / screen1.Width);

            float screen1EndPos, screen1StartPos;
            int dataStartIdx, dataEndIdx;
            int screen1ItemStartIdx;
            int screen1ItemEndIdx;
            int screen2ItemEndIdx = -1;


            screen1StartPos = idx * screen1.Width;
            screen1EndPos = (idx + 1) * screen1.Width;

            screen1ItemStartIdx = (int)Math.Floor((pos - screen1StartPos) / (itemSize.Width + itemSpacing));

            if (screen1EndPos > contentSize.Width)
                screen1EndPos = contentSize.Width;

            screen1ItemEndIdx = (int)Math.Floor((screen1EndPos - screen1StartPos) / (itemSize.Width + itemSpacing));


            if (screen1ItemEndIdx >= itemCreateCount)
                screen1ItemEndIdx--;


            screen1.Location = new PointF((float)Math.Ceiling(screen1StartPos - pos), screen1.Location.Y);
            screen2.Location = new PointF((float)Math.Ceiling(screen1EndPos - pos), screen2.Location.Y);

            dataStartIdx = idx * screen1.itemCreateCount + screen1ItemStartIdx;
            dataEndIdx = idx * screen1.itemCreateCount + screen1ItemEndIdx;


            if (screen2.Location.Y < pos + Width)
            {
                screen2ItemEndIdx = (int)Math.Floor((pos + Width - screen1EndPos) / (itemSize.Width + itemSpacing));

                if (screen2ItemEndIdx >= itemCreateCount)
                    screen2ItemEndIdx--;


                dataEndIdx = (idx + 1) * screen1.itemCreateCount + screen2ItemEndIdx;
            }


            List<ScLayerLayoutViewerItem> itemList = new List<ScLayerLayoutViewerItem>();
            ScLayerLayoutViewerItem viewerItem;

            int dataIdx = dataStartIdx;

            if (screen1EndPos - pos >= Height)
            {
                for (int i = screen1ItemStartIdx; i <= screen1ItemEndIdx; i++)
                {
                    viewerItem = (ScLayerLayoutViewerItem)screen1.controls[i];
                    viewerItem.Visible = true;
                    viewerItem.DataIdx = dataIdx++;
                    itemList.Add(viewerItem);
                }

                for (int i = screen1ItemEndIdx + 1; i < screen1.controls.Count; i++)
                {
                    screen1.controls[i].Visible = false;
                }
            }
            else
            {
                for (int i = screen1ItemStartIdx; i <= screen1ItemEndIdx; i++)
                {
                    viewerItem = (ScLayerLayoutViewerItem)screen1.controls[i];
                    viewerItem.Visible = true;
                    viewerItem.DataIdx = dataIdx++;
                    itemList.Add(viewerItem);
                }

                for (int i = screen1ItemEndIdx + 1; i < screen1.controls.Count; i++)
                {
                    screen1.controls[i].Visible = false;
                }


                if (screen2.Visible == true)
                {
                    for (int i = 0; i <= screen2ItemEndIdx; i++)
                    {
                        viewerItem = (ScLayerLayoutViewerItem)screen2.controls[i];
                        viewerItem.Visible = true;
                        viewerItem.DataIdx = dataIdx++;
                        itemList.Add(viewerItem);
                    }
                }
            }


            if (ItemDataSetValueEvent != null)
                ItemDataSetValueEvent(itemList.ToArray(), dataStartIdx, dataEndIdx);


            //  Refresh();  
        }

        private void ScLayoutLayoutViewerDoubleScreenViewport_SizeChanged(object sender, SizeF oldSize)
        {
            switch (LayoutMode)
            {
                case ScLayerLayoutViewerLayoutMode.MORE_ROW_SINGLE_COL_LAYOUT:
                    LayoutMoreRow();
                    break;

                case ScLayerLayoutViewerLayoutMode.MORE_COL_SINGLE_ROW_LAYOUT:
                    LayoutMoreCol();
                    break;
            }
        }



        void LayoutMoreCol()
        {
            if (itemSize.Height + itemSpacing == 0)
                return;

            itemCreateCount = (int)Math.Ceiling(Width / (itemSize.Width + itemSpacing));
            float screenWidth = itemCreateCount * (itemSize.Width + itemSpacing);

            contentSize.Height = Height;

            screen1.CreateItemDataLayerEvent = CreateItemDataLayerEvent;
            screen1.itemCreateCount = (int)itemCreateCount;
            screen1.itemSpacing = itemSpacing;
            screen1.itemCreateSize = itemSize;
            screen1.Size = new SizeF(screenWidth, Height);

            screen2.CreateItemDataLayerEvent = CreateItemDataLayerEvent;
            screen2.itemCreateCount = (int)itemCreateCount;
            screen2.itemSpacing = itemSpacing;
            screen2.itemCreateSize = itemSize;
            screen2.Size = new SizeF(screenWidth, Height);

        //    Refresh();
        }


        void LayoutMoreRow()
        {
            if (itemSize.Height + itemSpacing == 0)
                return;

            contentSize.Width = Width;

            itemCreateCount = (int)Math.Ceiling(Height / (itemSize.Height + itemSpacing));
            float screenHeight = itemCreateCount * (itemSize.Height + itemSpacing);
            


            screen1.CreateItemDataLayerEvent = CreateItemDataLayerEvent;
            screen1.itemCreateCount = (int)itemCreateCount;
            screen1.itemSpacing = itemSpacing;
            screen1.itemCreateSize = itemSize;
            screen1.Size = new SizeF(Width, screenHeight);


            screen2.CreateItemDataLayerEvent = CreateItemDataLayerEvent;
            screen2.itemCreateCount = (int)itemCreateCount;
            screen2.itemSpacing = itemSpacing;
            screen2.itemCreateSize = itemSize;
            screen2.Size = new SizeF(Width, screenHeight);

           // Refresh();
        }
    }
}
