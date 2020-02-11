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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    
    public class ScLayerLayoutViewerHeaderItemContainer : ScLayer
    {
        public delegate void CreateHeaderItemDataLayerEventHandler(ScLayer contentLayer, string name);
        public event CreateHeaderItemDataLayerEventHandler CreateHeaderItemDataLayerEvent = null;

        public delegate void SizeChanegedEventHandler(ScLayerLayoutViewerHeaderItemContainer headerItemContainer, List<ScLayer> headerItemList);
        public event SizeChanegedEventHandler SizeChanegedEvent = null;

        public delegate void HeaderItemValueChangedEventHandler(string dataName, object value);
        public event HeaderItemValueChangedEventHandler HeaderItemValueChangedEvent = null;

        int itemCreateCount;
        public float ControlerSize;
        public float ItemMinSize = 100;

        ScLayerLayoutViewerColumnInfo[] columnInfos;

        public ScLayerLayoutViewerHeaderOrientation HeaderOrientation;
        public ScLayerLayoutViewerHeaderSizeMode HeaderSizeMode;

        public int StyleIdx = 0;

        public Color BackGroundColor = Color.FromArgb(255, 241, 251, 253);

        public ScLayerLayoutViewerHeaderItemContainer(ScMgr scmgr)
            :base(scmgr)
        {
          
            SizeChanged += ScLayerLayoutViewerHeaderItemContainer_SizeChanged;
            D2DPaint += ScLayerLayoutViewerHeaderItemContainer_D2DPaint;
        }

        private void ScLayerLayoutViewerHeaderItemContainer_D2DPaint(D2DGraphics g)
        {
            switch(StyleIdx)
            {
                case 1:
                    g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
                    RawRectangleF rect = new RawRectangleF(0, 0, Width, Height);
                    RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(BackGroundColor);
                    SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
                    g.RenderTarget.FillRectangle(rect, brush);

                    brush.Dispose();
                    break;
            }
        }

        public void CreateHeaderItemFormDataInfo(
            ScLayerLayoutViewerColumnInfo[] columnInfos, float controlerSize,
            ScLayerLayoutViewerHeaderItem.HeaderItemHitEventHandler headerItemHitEventHandler)
        {

            this.columnInfos = columnInfos;
            itemCreateCount = columnInfos.Count();

            ControlerSize = controlerSize;

            for (int i = 0; i < itemCreateCount; i++)
            {
                ScLayerLayoutViewerHeaderItem headerItem = new ScLayerLayoutViewerHeaderItem(ScMgr);

                headerItem.HeaderOrientation = HeaderOrientation;
                headerItem.ItemMinSize = ItemMinSize;
                headerItem.ControlerSize = ControlerSize;
                headerItem.ItemDataName = columnInfos[itemCreateCount - i - 1].dataName;
                headerItem.ColumnInfo = columnInfos[itemCreateCount - i - 1];
                headerItem.HeaderItemHitEvent += headerItemHitEventHandler;
                headerItem.HeaderItemSizeChangedEvent += HeaderItem_HeaderItemSizeChangedEvent;
                headerItem.HeaderItemValueChangedEvent += HeaderItem_HeaderItemValueChangedEvent;
                headerItem.ComputeHeaderItemWidthLimitEvent += HeaderItem_ComputeHeaderItemWidthLimitEvent;
                headerItem.Idx = i;
                Add(headerItem);

                if (CreateHeaderItemDataLayerEvent != null)
                {
                    CreateHeaderItemDataLayerEvent(headerItem.ContentLayer, headerItem.ItemDataName);

                    if (headerItem.ContentLayer.controls.Count > 0)
                    {
                        ScLayer userLayer = headerItem.ContentLayer.controls[0];
                        headerItem.Controler.MouseMove += userLayer.SelfMouseEnter;
                    }
                }
            }

            switch (HeaderOrientation)
            {

                case ScLayerLayoutViewerHeaderOrientation.HORIZONTAL:
                    for (int i = 0; i < itemCreateCount; i++)
                    {
                        controls[i].Width = columnInfos[itemCreateCount - i - 1].length;
                    }
                    break;

                case ScLayerLayoutViewerHeaderOrientation.VERTICAL:
                    for (int i = 0; i < itemCreateCount; i++)
                    {
                        controls[i].Height = columnInfos[itemCreateCount - i - 1].length;
                    }
                    break;
            }

            if (HeaderSizeMode == ScLayerLayoutViewerHeaderSizeMode.ADAPTIVE)
            {
                ScLayerLayoutViewerHeaderItem headerItem = (ScLayerLayoutViewerHeaderItem)controls[0];
                headerItem.IsUseControler = false;
            }
        }

        private float HeaderItem_ComputeHeaderItemWidthLimitEvent(ScLayerLayoutViewerHeaderItem adjustItem, float adjustWidth)
        {
            float frontWidth = 0;
            int startIdx = 0;
            float limitWidth = adjustWidth;

            for (int i = controls.Count - 1; i >= 0; i--)
            {
                if (controls[i] == adjustItem)
                {
                    frontWidth += adjustWidth;
                    startIdx = i - 1;
                    break;
                }

                frontWidth += controls[i].Width - ControlerSize;
            }

            if (frontWidth > Width)
            {
                limitWidth = adjustWidth - (frontWidth - Width) - 0.1f;
            }

            return limitWidth;
        }

     
        private void HeaderItem_HeaderItemValueChangedEvent(string dataName, object value)
        {
            if (HeaderItemValueChangedEvent != null)
            {
                HeaderItemValueChangedEvent(dataName, value);
            }
        }

        public ScLayerLayoutViewerHeaderItem GetItem(int itemIdx)
        {
            return (ScLayerLayoutViewerHeaderItem)controls[itemIdx];
        }


        private void ScLayerLayoutViewerHeaderItemContainer_SizeChanged(object sender, SizeF oldSize)
        {

            switch (HeaderSizeMode)
            {
                case ScLayerLayoutViewerHeaderSizeMode.ADAPTIVE:
                    HeaderSizeModeAdaptive(oldSize);
                    break;

                case ScLayerLayoutViewerHeaderSizeMode.NONE:
                    HeaderSizeModeNone(oldSize);
                    break;
            }

        }

        void HeaderSizeModeAdaptive(SizeF oldSize)
        {
            switch (HeaderOrientation)
            {
                case ScLayerLayoutViewerHeaderOrientation.HORIZONTAL:
                    AdaptiveAdjustItems(false);
                    HeaderSizeModeNone(oldSize);

                    if (SizeChanegedEvent != null)
                        SizeChanegedEvent(this, controls);
                    break;

                case ScLayerLayoutViewerHeaderOrientation.VERTICAL:

                    HeaderSizeModeNone(oldSize);
                    break;
            }
        }

        void HeaderSizeModeNone(SizeF oldSize)
        {
            ScLayerLayoutViewerHeaderItem item;

            switch (HeaderOrientation)
            {
                case ScLayerLayoutViewerHeaderOrientation.HORIZONTAL:
                    if (oldSize.Height == Height)
                        return;

                    for (int i = controls.Count - 1; i >= 0; i--)
                    {
                        item = (ScLayerLayoutViewerHeaderItem)controls[i];
                        item.Height = Height;
                    }
                    break;

                case ScLayerLayoutViewerHeaderOrientation.VERTICAL:
                    if (oldSize.Width == Width)
                        return;

                    for (int i = controls.Count - 1; i >= 0; i--)
                    {
                        item = (ScLayerLayoutViewerHeaderItem)controls[i];
                        item.Width = Width;
                    }
                    break;
            }
        }


        void AdaptiveAdjustItems(bool isTriggerItemSizeChangedEvent)
        {
            float n;
            float layerWidth;
            float oldWidth = ControlerSize;
            foreach (ScLayer layer in controls)
                oldWidth += layer.Width - ControlerSize;

            SuspendLayout();
            foreach (ScLayerLayoutViewerHeaderItem layer in controls)
            {
                layer.IsTriggerItemSizeChangedEvent = isTriggerItemSizeChangedEvent;

                layerWidth = layer.Width;
                if (layerWidth <= 0)
                    layerWidth = ItemMinSize;

                if (layer.ColumnInfo.isHideColoum)
                    layer.Width = 0;
                else
                {

                    if (oldWidth - ControlerSize == 0)
                    {
                        n = 0.001f;
                    }
                    else {
                        n = (layerWidth - ControlerSize) / (oldWidth - ControlerSize);
                    }

                    layer.Width = (Width - ControlerSize) * n + ControlerSize;
                }
            }

            AdjustItemsLocation(Width);
            ResumeLayout(true);
        }


        float AdjustItemsLocation(float w)
        {
            ScLayerLayoutViewerHeaderItem item;
            float totalWidth = w;

            if (totalWidth == 0)
            {
                totalWidth = ControlerSize;
                foreach (ScLayer layer in controls)
                {
                    totalWidth += layer.Width - ControlerSize;
                }
            }
            
            float pos = totalWidth - ControlerSize;

            for (int i = 0; i < controls.Count(); i++)
            {
                item = (ScLayerLayoutViewerHeaderItem)controls[i];
                pos += ControlerSize - item.Width;
                item.Location = new PointF(pos, 0);
            }

       
            return totalWidth;
        }

        private void HeaderItem_HeaderItemSizeChangedEvent(ScLayerLayoutViewerHeaderItem headerItem)
        {
            switch (HeaderOrientation)
            {
                case ScLayerLayoutViewerHeaderOrientation.HORIZONTAL:
                    HorSizeChanged(headerItem);
                    break;

                case ScLayerLayoutViewerHeaderOrientation.VERTICAL:
                    VerSizeChanged(headerItem);
                    break;
            }
        }

        void HorSizeChanged(ScLayerLayoutViewerHeaderItem headerItem)
        {

            if (HeaderSizeMode == ScLayerLayoutViewerHeaderSizeMode.ADAPTIVE)
            {
                if(Width == 0)
                    return;

                SuspendLayout();
                AdaptiveAdjustItems2(false, headerItem);
                ResumeLayout(true);

            }
            else
            {
                SuspendLayout();
                Width = AdjustItemsLocation(0);
                ResumeLayout(true);
            }

            if (SizeChanegedEvent != null)
                SizeChanegedEvent(this, controls);

        }


        void AdaptiveAdjustItems2(bool isTriggerItemSizeChangedEvent, ScLayerLayoutViewerHeaderItem headerItem)
        {
            float n;
            float layerWidth;
            float frontWidth = 0;
            float orgBackWidth = 0;
            int startIdx = 0;
            ScLayerLayoutViewerHeaderItem item;

            SuspendLayout();

            for (int i = controls.Count - 1; i >= 0; i--)
            {
                item = (ScLayerLayoutViewerHeaderItem)controls[i];
                frontWidth += item.Width - ControlerSize;

                if (item == headerItem)
                {
                    startIdx = i - 1;
                    break;
                }
            }

            for (int i = startIdx; i >= 0; i--)
            {
                orgBackWidth += controls[i].Width - ControlerSize;
            }

            orgBackWidth += ControlerSize;
            float backWidth = Width - frontWidth;

           
            for (int i = startIdx; i >= 0; i--)
            {
                item = (ScLayerLayoutViewerHeaderItem)controls[i];
                item.IsTriggerItemSizeChangedEvent = isTriggerItemSizeChangedEvent;

                layerWidth = item.Width - ControlerSize;
              
                n = layerWidth / (orgBackWidth - ControlerSize);
                item.Width = (backWidth - ControlerSize) * n + ControlerSize;             
            }

            AdjustItemsLocation(Width);
            ResumeLayout(true);
        }



        void VerSizeChanged(ScLayerLayoutViewerHeaderItem headerItem)
        {
            ScLayerLayoutViewerHeaderItem item;
            float totalHeight = ControlerSize;

            foreach (ScLayer layer in controls)
            {
                totalHeight += layer.Height - ControlerSize;
            }


            float pos = totalHeight - ControlerSize;

            for (int i = 0; i < controls.Count(); i++)
            {
                item = (ScLayerLayoutViewerHeaderItem)controls[i];
                pos += ControlerSize - item.Height;
                item.Location = new PointF(0, pos);
            }


            Height = totalHeight;
   
        }


    }
}
