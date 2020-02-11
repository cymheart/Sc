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

using SharpDX.DirectWrite;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Utils;

namespace Sc
{
    public class ScGridViewCore : ScLayer
    {
        public delegate void ViewerItemMouseEventHandler(ScLayerLayoutViewerItem viewerItem, Dictionary<string, ScLayer> userLayerDict, ScMouseEventArgs e);
        public event ViewerItemMouseEventHandler ViewerItemMouseEnterEvent = null;
        public event ViewerItemMouseEventHandler ViewerItemMouseDownEvent = null;
        public event ViewerItemMouseEventHandler ViewerItemMouseLeaveEvent = null;
        public event ViewerItemMouseEventHandler ViewerItemMouseDoubleDownEvent = null;

        public delegate void HeaderItemValueChangedEventHandler(string dataName, object value);
        public event HeaderItemValueChangedEventHandler HeaderItemValueChangedEvent = null;

        public delegate void UserLayerValueChangedEventHandler(int dataIdx, string dataName, object value);
        public event UserLayerValueChangedEventHandler UserLayerValueChangedEvent = null;

        public delegate void HeaderItemHitEventHandler(string name);
        public event HeaderItemHitEventHandler HeaderItemHitEvent = null;

        public delegate void CreateHeaderItemDataLayerEventHandler(ScLayer contentLayer, string name);
        public event CreateHeaderItemDataLayerEventHandler CreateHeaderItemDataLayerEvent = null;
        public event CreateHeaderItemDataLayerEventHandler CreateDataItemDataLayerEvent = null;

        public delegate void ItemDataSetValueEventHandler(ScLayerLayoutViewerItem[] viewItems, List<Dictionary<string, ScLayer>> userItemLayerList, int dataStartIdx, int dataEndIdx);
        public event ItemDataSetValueEventHandler ItemDataSetValueEvent = null;

        public delegate void ContentSizeChangedEventHandler();
        public event ContentSizeChangedEventHandler ContentSizeChangedEvent = null;


        ScLayerLayoutViewerHeaderItemContainer header;
        ScLayerLayoutViewerDoubleScreenViewport dbScreen;

        public float HeaderHeight = 40;
        public float HeaderSpacing = 10;
        public float HeaderControlerSize = 20;
        public float RowHeight = 40;
        public float RowSpacing = 1;
        public float RowControlerSize = 0;
        public float ItemMinSize = 220;

        public int HeaderStyle
        {
            get { return header.StyleIdx; }
            set { header.StyleIdx = value; }
        }
            
        public Color HeaderBackGroundColor
        {
            get { return header.BackGroundColor; }
            set
            {
                header.BackGroundColor = value;
            }
        }



        public ScLayerLayoutViewerHeaderSizeMode HeaderSizeMode
        {
            get { return header.HeaderSizeMode; }
            set {
                header.HeaderSizeMode = value;
            }
        }

        public Color ItemSelectorColor
        {
            get { return dbScreen.ItemSelectorColor; }
            set
            {
                dbScreen.ItemSelectorColor = value;
            }
        }
        public ScLayerLayoutViewerHeaderItemContainer Header { get { return header; } }
        public ScLayerLayoutViewerDoubleScreenViewport DbScreen { get { return dbScreen; } }

        public ScLayer ItemDirectClipLayer {
            set { dbScreen.ItemDirectClipLayer = value; }
        }

        ScLayerLayoutViewerColumnInfo[] columnInfos;

        ScLayer headerTitleContainer;

        public ScGridViewCore(ScMgr scmgr = null)
            :base(scmgr)
        {

            headerTitleContainer = new ScLayer(scmgr);
            headerTitleContainer.IsHitThrough = true;
            Add(headerTitleContainer);

            header = new ScLayerLayoutViewerHeaderItemContainer(scmgr);
            header.StyleIdx = 1;
            header.IsUseDebugPanitCode = true;
            header.HeaderOrientation = ScLayerLayoutViewerHeaderOrientation.HORIZONTAL;         
            header.CreateHeaderItemDataLayerEvent += Header_CreateHeaderItemDataLayerEvent;
            header.HeaderItemValueChangedEvent += Header_HeaderItemValueChangedEvent;
            header.SizeChanegedEvent += Header_SizeChanegedEvent;
            Add(header);

          
         
            dbScreen = new ScLayerLayoutViewerDoubleScreenViewport(scmgr);
            dbScreen.LayoutMode = ScLayerLayoutViewerLayoutMode.MORE_ROW_SINGLE_COL_LAYOUT;
            dbScreen.CreateItemDataLayerEvent = PreCreateItemDataLayerEvent;
            dbScreen.ItemDataSetValueEvent += DbScreen_ItemDataSetValueEvent;
            dbScreen.UserLayerValueChangedEvent += DbScreen_UserLayerValueChangedEvent;
            dbScreen.ViewerItemMouseEnterEvent += DbScreen_ViewerItemMouseEnterEvent;
            dbScreen.ViewerItemMouseDownEvent += DbScreen_ViewerItemMouseDownEvent;
            dbScreen.ViewerItemMouseLeaveEvent += DbScreen_ViewerItemMouseLeaveEvent;
            dbScreen.ViewerItemMouseDoubleDownEvent += DbScreen_ViewerItemMouseDoubleDownEvent;
            Add(dbScreen);

            //

            SizeChanged += ScGridView_SizeChanged;
            //  D2DPaint += ScGridViewContainer_D2DPaint;

            BackgroundColor = Color.FromArgb(255, 227, 228, 231);
        }

        public void SetHeaderTitleLayer(ScLayer headerTitle)
        {
            if (headerTitle == null)
                return;

            headerTitle.IsHitThrough = true;
            headerTitleContainer.Add(headerTitle);
        }
       

        private void ScGridViewContainer_D2DPaint(D2DGraphics g)
        {
            //g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
            //RawRectangleF rect = new RawRectangleF(0, 0, Width , Height - 1);
            //RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 227, 228, 231));
            //SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            //g.RenderTarget.FillRectangle(rect, brush);
        }

        private void DbScreen_ViewerItemMouseLeaveEvent(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e)
        {
            if (ViewerItemMouseLeaveEvent != null)
            {
                Dictionary<string, ScLayer> userItemDict = CreateUserItemDict(viewerItem);
                if (userItemDict != null)
                    ViewerItemMouseLeaveEvent(viewerItem, userItemDict, e);
            }
        }

        private void DbScreen_ViewerItemMouseDownEvent(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e)
        {       
            if (ViewerItemMouseDownEvent != null)
            {
                Dictionary<string, ScLayer> userItemDict = CreateUserItemDict(viewerItem);
                if (userItemDict != null)
                    ViewerItemMouseDownEvent(viewerItem, userItemDict, e);
            }
            Refresh();
        }

        private void DbScreen_ViewerItemMouseDoubleDownEvent(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e)
        {
            if (ViewerItemMouseDoubleDownEvent != null)
            {
                Dictionary<string, ScLayer> userItemDict = CreateUserItemDict(viewerItem);
                if (userItemDict != null)
                    ViewerItemMouseDoubleDownEvent(viewerItem, userItemDict, e);
            }
            Refresh();
        }


        private void DbScreen_ViewerItemMouseEnterEvent(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e)
        {
           
            if (ViewerItemMouseEnterEvent != null)
            {
                Dictionary<string, ScLayer> userItemDict = CreateUserItemDict(viewerItem);

                if (userItemDict != null)
                    ViewerItemMouseEnterEvent(viewerItem, userItemDict, e);


            }
        }


        public List<ScLayerLayoutViewerItem> GetSelectedViewerItems()
        {
            return null;
        }

        Dictionary<string, ScLayer> CreateUserItemDict(ScLayerLayoutViewerItem viewerItem)
        {
            ScLayerLayoutViewerHeaderItemContainer dataItemContainer = (ScLayerLayoutViewerHeaderItemContainer)viewerItem.GetContent();
            ScLayerLayoutViewerHeaderItem item;
            Dictionary<string, ScLayer> userItemDict = new Dictionary<string, ScLayer>();
            ScLayer userLayer;

            if (dataItemContainer == null)
                return null;

            for (int j = 0; j < dataItemContainer.controls.Count(); j++)
            {
                item = (ScLayerLayoutViewerHeaderItem)dataItemContainer.controls[j];

                if (item.ContentLayer.controls.Count > 0)
                {
                    if(item.ContentLayer.controls.Count == 2)
                        userLayer = item.ContentLayer.controls[1];
                    else
                        userLayer = item.ContentLayer.controls[0];
                }
                else
                    userLayer = null;

                if (userItemDict.ContainsKey(item.ItemDataName))
                    continue;

                userItemDict.Add(item.ItemDataName, userLayer);
            }

            return userItemDict;
        }

       
        private void Header_HeaderItemValueChangedEvent(string dataName, object value)
        {
            if (HeaderItemValueChangedEvent != null)
                HeaderItemValueChangedEvent(dataName, value);
        }

        private void DbScreen_UserLayerValueChangedEvent(int dataIdx, string dataName, object value)
        {
            if (UserLayerValueChangedEvent != null)
                UserLayerValueChangedEvent(dataIdx, dataName, value);
        }

        public void SetRowCount(int rowCount = 1)
        {
            dbScreen.SetItemCount(rowCount);
        }
        public void AppendRowCount(int rowCount = 1)
        {
            dbScreen.AppendItemCount(rowCount);
        }

        public void RemoveRowCount(int rowCount = 1)
        {
            dbScreen.RemoveItemCount(rowCount);
        }

        public void RemoveAllRow()
        {
            dbScreen.RemoveAllItem();
        }

        public void SetContentShowPos(float pos)
        {
            dbScreen.SetItemShowValueFromData(pos);
        }

        public float GetContentShowPos()
        {
            return dbScreen.ContentPos;
        }

        public float GetContentLastPos()
        {
            return dbScreen.ContentSize.Height;
        }

        public void MoveHeaderLoaction(float pos)
        {
            header.Location = new PointF(pos, header.Location.Y);
        }

        public void SetScreenLoactionX(float pos)
        {
            SuspendLayout();
            dbScreen.SetScreenLocationX(pos);
            ResumeLayout(true);
        }

        public void SetScreenLoactionY(float pos)
        {
            dbScreen.SetScreenLocationY(pos);
        }

        public PointF GetScreenLoaction()
        {
            return dbScreen.GetScreenLocation();
        }

        public SizeF GetContentSize()
        {
            return dbScreen.ContentSize;
        }

        public float GetViewportHeight()
        {
            return dbScreen.Height;
        }

        public float GetViewportWidth()
        {
            return Width;
        }

        public void UpOffsetHeight(float offsetHeight)
        {
            Height = Height - offsetHeight;
        }

        public void UpOffsetWidth(float offsetWidth)
        {
            Width = Width - offsetWidth;
        }

        public void ClearSelectedItems()
        {
            dbScreen.ClearSelectedItems();
        }

        public void CreateHeaderItemFormDataInfo(ScLayerLayoutViewerColumnInfo[] columnInfos)
        {
            this.columnInfos = columnInfos;

            if (ItemMinSize - HeaderControlerSize < HeaderControlerSize)
                ItemMinSize = 2 * HeaderControlerSize;

            header.ItemMinSize = ItemMinSize;


            header.CreateHeaderItemFormDataInfo(columnInfos, HeaderControlerSize, _HeaderItemHitEvent);
        }

       
        private void ScGridView_SizeChanged(object sender, System.Drawing.SizeF oldSize)
        {
            if (HeaderSizeMode == ScLayerLayoutViewerHeaderSizeMode.ADAPTIVE)
            {
                header.Size = new SizeF(Width, HeaderHeight);
                headerTitleContainer.Size = header.Size;
            }
            else {
                header.Height = HeaderHeight;
                headerTitleContainer.Size = header.Size;
            }

            dbScreen.itemSpacing = RowSpacing;
            dbScreen.itemSize = new SizeF(header.Width, RowHeight);
            dbScreen.DirectionRect = new RectangleF(0, header.Height + HeaderSpacing, header.Width, Height - header.Height - HeaderSpacing);

        }

        private void Header_SizeChanegedEvent(ScLayerLayoutViewerHeaderItemContainer headerItemContainer, List<ScLayer> headerItemList)
        {
            headerTitleContainer.Size = header.Size;

            List<ScLayer>[] itemlists = { dbScreen.GetScreen1AllItem(), dbScreen.GetScreen2AllItem() };
            if (itemlists[0].Count == 0 || itemlists[1].Count == 0)
                return;

           
            SuspendLayout();
            HeaderItemSizeChangedEventHandler(headerItemList);
            dbScreen.Width = headerItemContainer.Width;
            ResumeLayout(true);

            if (ContentSizeChangedEvent != null)
                ContentSizeChangedEvent();
        }


        void HeaderItemSizeChangedEventHandler(List<ScLayer> headerItemList)
        {
            int idx;
            ScLayerLayoutViewerHeaderItem item;
            float w;

            List<ScLayer>[] itemlists = { dbScreen.GetScreen1AllItem(), dbScreen.GetScreen2AllItem() };
            ScLayerLayoutViewerHeaderItemContainer dataItemContainer;

            if (itemlists[0].Count == 0 || itemlists[1].Count == 0)
                return;

            foreach (ScLayerLayoutViewerHeaderItem headerItem in headerItemList)
            {
                idx = headerItem.Idx;

                for (int i = 0; i < itemlists.Count(); i++)
                {
                    foreach (ScLayerLayoutViewerItem screenItem in itemlists[i])
                    {
                        dataItemContainer = (ScLayerLayoutViewerHeaderItemContainer)screenItem.GetContent();
                        item = (ScLayerLayoutViewerHeaderItem)dataItemContainer.controls[idx];
                        w = headerItem.Width - (HeaderControlerSize - RowControlerSize);

                        if (w != item.Width)
                            item.Width = w;
                    }
                }
            }
        }

      
        void PreCreateItemDataLayerEvent(ScLayerLayoutViewerItem item)
        {
            item.SuspendLayout();
            ScLayerLayoutViewerHeaderItem headerItem;
            ScLayerLayoutViewerHeaderItem dataItem;

            ScLayerLayoutViewerHeaderItemContainer dataItemContainer = new ScLayerLayoutViewerHeaderItemContainer(item.ScMgr);
            item.AddContent(dataItemContainer);


            dataItemContainer.HeaderOrientation = ScLayerLayoutViewerHeaderOrientation.HORIZONTAL;
            dataItemContainer.CreateHeaderItemDataLayerEvent += DataItemContainer_CreateHeaderItemDataLayerEvent;
            dataItemContainer.ItemMinSize = ItemMinSize - (HeaderControlerSize - RowControlerSize);     
            dataItemContainer.CreateHeaderItemFormDataInfo(columnInfos, RowControlerSize, null);
            dataItemContainer.DirectionRect = new RectangleF(0, 0, item.Width, item.Height);

            for (int i=0; i< header.controls.Count; i++)
            {
                dataItem = (ScLayerLayoutViewerHeaderItem)dataItemContainer.controls[i];
                headerItem = (ScLayerLayoutViewerHeaderItem)header.controls[i];
                dataItem.Width = headerItem.Width - (HeaderControlerSize - RowControlerSize);
            }
    
            item.ResumeLayout(false);
        }

        void _HeaderItemHitEvent(ScLayerLayoutViewerHeaderItem headerItem, ScMouseEventArgs e)
        {
            if(HeaderItemHitEvent != null)
                HeaderItemHitEvent(headerItem.ItemDataName);
        }

        private void Header_CreateHeaderItemDataLayerEvent(ScLayer contentLayer, string name)
        {
            if (CreateHeaderItemDataLayerEvent != null)
                CreateHeaderItemDataLayerEvent(contentLayer, name);
        }

        private void DataItemContainer_CreateHeaderItemDataLayerEvent(ScLayer contentLayer, string name)
        {
            if (CreateDataItemDataLayerEvent != null)
                CreateDataItemDataLayerEvent(contentLayer, name);
        }
  
        private void DbScreen_ItemDataSetValueEvent(ScLayerLayoutViewerItem[] items, int dataStartIdx, int dataEndIdx)
        {
            if (ItemDataSetValueEvent == null)
                return;

            ScLayerLayoutViewerHeaderItemContainer dataItemContainer;
            Dictionary<string, ScLayer> userItemDict;
            List<Dictionary<string, ScLayer>> userItemsList = new List<Dictionary<string, ScLayer>>();
            ScLayerLayoutViewerHeaderItem item;
            ScLayer userLayer;


            foreach (ScLayerLayoutViewerItem screenItem in items)
            {
                dataItemContainer = (ScLayerLayoutViewerHeaderItemContainer)screenItem.GetContent();

                if (dataItemContainer == null)
                    continue;

                userItemDict = new Dictionary<string, ScLayer>();


                for (int j = 0; j < dataItemContainer.controls.Count(); j++)
                {
                    item = (ScLayerLayoutViewerHeaderItem)dataItemContainer.controls[j];

                    if (item.ContentLayer.controls.Count > 0)
                        userLayer = item.ContentLayer.controls[0];
                    else
                        userLayer = null;

                    if (userItemDict.ContainsKey(item.ItemDataName))
                        continue;

                    userItemDict.Add(item.ItemDataName, userLayer);
                }

                userItemsList.Add(userItemDict);
            }


            ItemDataSetValueEvent(items, userItemsList, dataStartIdx, dataEndIdx);
            userItemsList.Clear();
        }       
    }

}
