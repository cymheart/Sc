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
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System.Collections.Generic;
using System.Drawing;
using Utils;

namespace Sc
{
    public class ScGridViewCoreContainer : ScLayer
    {
        public delegate void ViewerItemMouseEnterEventHandler(ScLayerLayoutViewerItem viewerItem, Dictionary<string, ScLayer> userLayerDict, ScMouseEventArgs e);
        public event ViewerItemMouseEnterEventHandler ViewerItemMouseEnterEvent = null;
        public event ViewerItemMouseEnterEventHandler ViewerItemMouseDownEvent = null;
        public event ViewerItemMouseEnterEventHandler ViewerItemMouseLeaveEvent = null;
        public event ViewerItemMouseEnterEventHandler ViewerItemMouseDoubleDownEvent = null;

        public delegate void HeaderItemValueChangedEventHandler(string dataName, object value);
        public event HeaderItemValueChangedEventHandler HeaderItemValueChangedEvent = null;

        public delegate void UserLayerValueChangedEventHandler(int dataIdx, string dataName, object value);
        public event UserLayerValueChangedEventHandler UserLayerValueChangedEvent = null;

        public delegate void HeaderItemHitEventHandler(string name);
        public event HeaderItemHitEventHandler HeaderItemHitEvent = null;

        public delegate void CreateDataItemLayerEventHandler(ScLayerLayoutViewerItem dataItem, ScLayer contentLayer, string name);
        public event CreateDataItemLayerEventHandler CreateDataItemDataLayerEvent = null;

        public delegate void CreateHeaderItemDataLayerEventHandler(ScLayer contentLayer, string name);
        public event CreateHeaderItemDataLayerEventHandler CreateHeaderItemDataLayerEvent = null;
 
        public delegate void ItemDataSetValueEventHandler(ScLayerLayoutViewerItem[] viewItems, List<Dictionary<string, ScLayer>> userItemLayerList, int dataStartIdx, int dataEndIdx);
        public event ItemDataSetValueEventHandler ItemDataSetValueEvent = null;

        public delegate ScLayer CreateHeaderTitleEventHandler(ScMgr scmgr);
        public event CreateHeaderTitleEventHandler CreateHeaderTitleEvent = null;

        ScGridViewCore gridViewCore;
        ScScrollBar scrollBarVertical;
        ScScrollBar scrollBarHorizontal;

        ScLayer gridViewPack;
        ScLayer seletorScreen;
        ScShadow shadow;

        /// <summary>
        /// 水平滚动条高度
        /// </summary>
        public float VerScrollSize = 30;

        /// <summary>
        /// 垂直滚动条宽度
        /// </summary>
        public float HorScrollSize = 30;

        public Margin SelectorMargin = new Margin(2, 2, 2, 2);

        /// <summary>
        /// 内外边间距
        /// </summary>
        public Margin SideSpacing = new Margin(15, 15, 15, 15);

        /// <summary>
        /// 外边线颜色
        /// </summary>
        public Color OutsideLineColor = Color.FromArgb(255, 222, 222, 222);

        /// <summary>
        /// 内边线颜色
        /// </summary>
        public Color InsideLineColor = Color.FromArgb(255, 222, 222, 222);


        /// <summary>
        /// 是否启用内边线效果
        /// </summary>
        public bool IsUseInside = true;


        
        public int HeaderStyle
        {
            get { return gridViewCore.HeaderStyle; }
            set { gridViewCore.HeaderStyle = value; }
        }


        /// <summary>
        /// 列头背景色
        /// </summary>
        public Color HeaderBackGroundColor
        {
            get { return gridViewCore.HeaderBackGroundColor; }
            set
            {
                gridViewCore.HeaderBackGroundColor = value;
            }
        }

        /// <summary>
        /// 内容背景色
        /// </summary>
        public Color? ContainerBackGroundColor
        {
            get { return gridViewCore.BackgroundColor; }
            set
            {
                gridViewCore.BackgroundColor = value;
            }
        }


        /// <summary>
        /// 选择器颜色
        /// </summary>
        public Color ItemSelectorColor
        {
            get { return gridViewCore.ItemSelectorColor; }
            set
            {
                gridViewCore.ItemSelectorColor = value;
            }
        }


        /// <summary>
        /// 是否开启阴影
        /// </summary>
        public bool IsUseShadow
        {
            get
            {
                if (ShadowLayer == null)
                    return false;
                return true;
            }
            set
            {
                if (value == true)
                    ShadowLayer = shadow;
                else
                    ShadowLayer = null;
            }
        }


        /// <summary>
        /// 阴影类型
        /// </summary>
        public int ShadowStyle = 0;

        /// <summary>
        /// 阴影范围
        /// </summary>
        public float ShadowRange
        {
            get
            {
                if (shadow == null)
                    return 0;
                return shadow.ShadowRadius;
            }
            set
            {
                if (shadow == null)
                    return;

                if (value >= 0)
                    shadow.ShadowRadius = value;
                else
                    shadow.ShadowRadius = 0;
            }
        }

        /// <summary>
        /// 阴影角半径
        /// </summary>
        public float ShadowCornersRadius
        {
            get
            {
                if (shadow == null)
                    return 0;
                return shadow.CornersRadius;
            }
            set
            {
                if (shadow == null)
                    return;


                if (value >= 0)
                    shadow.CornersRadius = value;
                else
                    shadow.CornersRadius = 0;
            }
        }

        /// <summary>
        /// 阴影颜色
        /// </summary>
        public Color ShadowColor
        {
            get
            {
                if (shadow == null)
                    return Color.FromArgb(0,0,0,0);
                return shadow.ShadowColor;
            }
            set
            {
                if (shadow == null)
                    return;

                shadow.ShadowColor = value;   
            }
        }



        /// <summary>
        /// 列头高度
        /// </summary>
        public float HeaderHeight
        {
            get { return gridViewCore.HeaderHeight; }
            set { gridViewCore.HeaderHeight = value; }
        }


        /// <summary>
        /// 列头和行内容间距
        /// </summary>
        public float HeaderSpacing
        {
            get { return gridViewCore.HeaderSpacing; }
            set { gridViewCore.HeaderSpacing = value; }
        }


        /// <summary>
        /// 列头字段移动控制器尺寸
        /// </summary>
        public float HeaderControlerSize
        {
            get { return gridViewCore.HeaderControlerSize; }
            set { gridViewCore.HeaderControlerSize = value; }
        }

        /// <summary>
        /// 内容行高度
        /// </summary>
        public float RowHeight
        {
            get { return gridViewCore.RowHeight; }
            set { gridViewCore.RowHeight = value; }
        }


        /// <summary>
        /// 行间距
        /// </summary>
        public float RowSpacing
        {
            get { return gridViewCore.RowSpacing; }
            set { gridViewCore.RowSpacing = value; }
        }


        /// <summary>
        /// 内容行控制器尺寸
        /// </summary>
        public float RowControlerSize
        {
            get { return gridViewCore.RowControlerSize; }
            set { gridViewCore.RowControlerSize = value; }
        }

        /// <summary>
        /// 单元内容最小尺寸
        /// </summary>
        public float ItemMinSize
        {
            get { return gridViewCore.ItemMinSize; }
            set { gridViewCore.ItemMinSize = value; }
        }

        /// <summary>
        /// 列头内容排列模式，是否自适应排列
        /// </summary>
        public ScLayerLayoutViewerHeaderSizeMode HeaderSizeMode
        {
            get { return gridViewCore.HeaderSizeMode; }
            set
            {
                gridViewCore.HeaderSizeMode = value;
            }
        }


        //滚动条设置
        public int ScrollBarSliderAlpha
        {
            get { return scrollBarVertical.SliderAlpha; }
            set
            {
                scrollBarVertical.SliderAlpha = value;
                scrollBarHorizontal.SliderAlpha = value;
            }
        }

        public int ScrollBarSliderEnterAlpha
        {
            get { return scrollBarVertical.SliderEnterAlpha; }
            set
            {
                scrollBarVertical.SliderEnterAlpha = value;
                scrollBarHorizontal.SliderEnterAlpha = value;
            }
        }

        public int ScrollBarSliderDownAlpha
        {
            get { return scrollBarVertical.SliderDownAlpha; }
            set
            {
                scrollBarVertical.SliderDownAlpha = value;
                scrollBarHorizontal.SliderDownAlpha = value;
            }
        }


        public Color ScrollBarSliderColor
        {
            get { return scrollBarVertical.SliderColor; }
            set
            {
                scrollBarVertical.SliderColor = value;
                scrollBarHorizontal.SliderColor = value;
            }
        }

   
        public ScGridViewCoreContainer(ScMgr scmgr)
            :base(scmgr)
        {
            IsUseDebugPanitCode = true;

            seletorScreen = new ScLayer(scmgr);
            seletorScreen.Name = "seletorScreen";
            seletorScreen.IsUseDebugPanitCode = true;

            gridViewPack = new ScLayer(scmgr);
            gridViewPack.Name = "gridViewPack";
            gridViewPack.IsUseDebugPanitCode = true;
            Add(gridViewPack);

            gridViewCore = new ScGridViewCore(scmgr);
            gridViewCore.Name = "gridViewCore";
            gridViewCore.IsUseDebugPanitCode = true;
            gridViewCore.ItemDirectClipLayer = seletorScreen;
            gridViewCore.HeaderItemValueChangedEvent += GridViewContainer_HeaderItemValueChangedEvent;
            gridViewCore.ContentSizeChangedEvent += GridViewContainer_ContentSizeChangedEvent;
            gridViewCore.HeaderItemHitEvent += GridViewContainer_HeaderItemHitEvent;
            gridViewCore.CreateHeaderItemDataLayerEvent += GridViewContainer_CreateHeaderItemDataLayerEvent;
            gridViewCore.CreateDataItemDataLayerEvent += GridViewContainer_CreateDataItemDataLayerEvent;
            gridViewCore.ItemDataSetValueEvent += GridViewContainer_ItemDataSetValueEvent;
            gridViewCore.UserLayerValueChangedEvent += GridViewContainer_UserLayerValueChangedEvent;
            gridViewCore.ViewerItemMouseEnterEvent += GridViewContainer_ViewerItemMouseEnterEvent;
            gridViewCore.ViewerItemMouseDownEvent += GridViewContainer_ViewerItemMouseDownEvent;
            gridViewCore.ViewerItemMouseLeaveEvent += GridViewContainer_ViewerItemMouseLeaveEvent;
            gridViewCore.ViewerItemMouseDoubleDownEvent += GridViewContainer_ViewerItemMouseDoubleDownEvent;

            gridViewPack.Add(gridViewCore);

            Add(seletorScreen);


            //
            scrollBarVertical = new ScScrollBar(scmgr);
            scrollBarVertical.ScrollOrientation = ScScrollOrientation.VERTICAL_SCROLL;
            scrollBarVertical.Visible = false;
            scrollBarVertical.SliderMoveEvent += ScrollBarVertical_SliderMoveEvent;
            Add(scrollBarVertical);

            scrollBarHorizontal = new ScScrollBar(scmgr);
            scrollBarHorizontal.ScrollOrientation = ScScrollOrientation.HORIZONTAL_SCROLL;
            scrollBarHorizontal.Visible = false;
            scrollBarHorizontal.SliderMoveEvent += ScrollBarHorizontal_SliderMoveEvent;
            Add(scrollBarHorizontal);


            //
            shadow = new ScShadow(scmgr);
            shadow.CornersRadius = ShadowCornersRadius;
            shadow.ShadowRadius = ShadowRange;
            shadow.ShadowColor = ShadowColor;
            ShadowLayer = shadow;

            SizeChanged += ScGridView_SizeChanged;
            LocationChanged += ScGridView_LocationChanged;
            D2DPaint += ScGridView_D2DPaint;

        }

       

        private void ScGridView_D2DPaint(D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;

            RawRectangleF rect = new RawRectangleF(1, 1, Width, Height);
            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(OutsideLineColor);
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.DrawRectangle(rect, brush);
            brush.Dispose();


            rect = new RawRectangleF(gridViewPack.DirectionRect.Left + 1, gridViewPack.DirectionRect.Top + 1, gridViewPack.DirectionRect.Right + 1, gridViewPack.DirectionRect.Bottom + 1);

            if (rect.Left <= rect.Right && rect.Top <= rect.Bottom)
            {
                rawColor = GDIDataD2DUtils.TransToRawColor4(InsideLineColor);
                brush = new SolidColorBrush(g.RenderTarget, rawColor);
                g.RenderTarget.DrawRectangle(rect, brush);
                brush.Dispose();
            }


           
        }

    
        private void GridViewContainer_ViewerItemMouseDownEvent(ScLayerLayoutViewerItem viewerItem, Dictionary<string, ScLayer> userLayerDict, ScMouseEventArgs e)
        {  
            if (ViewerItemMouseDownEvent != null)
                ViewerItemMouseDownEvent(viewerItem, userLayerDict, e);

             Refresh();

        }

        private void GridViewContainer_ViewerItemMouseDoubleDownEvent(ScLayerLayoutViewerItem viewerItem, Dictionary<string, ScLayer> userLayerDict, ScMouseEventArgs e)
        {
            if (ViewerItemMouseDoubleDownEvent != null)
               ViewerItemMouseDoubleDownEvent(viewerItem, userLayerDict, e);

            Refresh();
        }

        private void GridViewContainer_ViewerItemMouseLeaveEvent(ScLayerLayoutViewerItem viewerItem, Dictionary<string, ScLayer> userLayerDict, ScMouseEventArgs e)
        {
            if (ViewerItemMouseLeaveEvent != null)
                ViewerItemMouseLeaveEvent(viewerItem, userLayerDict, e);
        }

        private void GridViewContainer_ViewerItemMouseEnterEvent(ScLayerLayoutViewerItem viewerItem, Dictionary<string, ScLayer> userLayerDict, ScMouseEventArgs e)
        {
            if (ViewerItemMouseEnterEvent != null)
                ViewerItemMouseEnterEvent(viewerItem, userLayerDict, e);
        }

        private void GridViewContainer_HeaderItemValueChangedEvent(string dataName, object value)
        {
            if (HeaderItemValueChangedEvent != null)
                HeaderItemValueChangedEvent(dataName, value);
        }

        private void GridViewContainer_UserLayerValueChangedEvent(int dataIdx, string dataName, object value)
        {
            if (UserLayerValueChangedEvent != null)
                UserLayerValueChangedEvent(dataIdx, dataName, value);
        }

        private void GridViewContainer_ItemDataSetValueEvent(ScLayerLayoutViewerItem[] viewItems, List<Dictionary<string, ScLayer>> userItemLayerList, int dataStartIdx, int dataEndIdx)
        {
            if (ItemDataSetValueEvent != null)
                ItemDataSetValueEvent(viewItems, userItemLayerList, dataStartIdx, dataEndIdx);
        }

        private void GridViewContainer_CreateDataItemDataLayerEvent(ScLayer contentLayer, string name)
        {
            if (CreateDataItemDataLayerEvent != null)
            {
                ScLayerLayoutViewerItem viewItem = (ScLayerLayoutViewerItem)contentLayer.Parent.Parent.Parent.Parent;
                CreateDataItemDataLayerEvent(viewItem, contentLayer, name);
            }
        }

        private void GridViewContainer_CreateHeaderItemDataLayerEvent(ScLayer contentLayer, string name)
        {
            if (CreateHeaderItemDataLayerEvent != null)
                CreateHeaderItemDataLayerEvent(contentLayer, name);
        }




        private void GridViewContainer_HeaderItemHitEvent(string name)
        {
            if (HeaderItemHitEvent != null)
                HeaderItemHitEvent(name);
        }

        private void GridViewContainer_ContentSizeChangedEvent()
        {
            if (Width == 0)
                return;

            SuspendLayout();
   
            PointF screenLoctain = gridViewCore.GetScreenLoaction();
            float viewportWidth = gridViewCore.GetViewportWidth();
            float screenEndPos = screenLoctain.X + gridViewCore.GetContentSize().Width;

            if (gridViewCore.GetContentSize().Width < viewportWidth && screenLoctain.X < 0)
            {
                gridViewCore.MoveHeaderLoaction(0);
                gridViewCore.SetScreenLoactionX(0);
            }
            else if (screenEndPos <= viewportWidth)
            {
                float x = screenLoctain.X + viewportWidth - screenEndPos;

                if (x <= 0)
                {
                   gridViewCore.MoveHeaderLoaction(x);
                   gridViewCore.SetScreenLoactionX(x);
                }
            }


            if (shadow != null)
            {
                RectangleF oldShadowDrawBox = shadow.DrawBox;
                SetContentShowPos(gridViewCore.GetContentShowPos());
                ResumeLayout(true);
                InvalidateGlobalRect(GDIDataD2DUtils.UnionRectF(oldShadowDrawBox, DrawBox));
                Update();

            }
            else
            {
                SetContentShowPos(gridViewCore.GetContentShowPos());
                ResumeLayout(true);
                Refresh();
                Update();
            }

        }


 
        private void ScrollBarVertical_SliderMoveEvent(float yMoveValue)
        {
            SuspendLayout();
            gridViewCore.SetContentShowPos(yMoveValue);
            ResumeLayout(true);
            Refresh();
            Update();
        }

        private void ScrollBarHorizontal_SliderMoveEvent(float yMoveValue)
        {
            SuspendLayout();
            gridViewCore.MoveHeaderLoaction(-yMoveValue);
            gridViewCore.SetScreenLoactionX(-yMoveValue);
            ResumeLayout(true);
            Refresh();
            Update();
        }

        public void CreateHeaderItemFormDataInfo(ScLayerLayoutViewerColumnInfo[] columnInfos)
        {
            gridViewCore.CreateHeaderItemFormDataInfo(columnInfos);
        }

        public void ClearSelectedItems()
        {
            gridViewCore.ClearSelectedItems();
        }

        public void AppendRowCount(int rowCount = 1)
        {
            gridViewCore.AppendRowCount(rowCount);
            SetScroll();
            Refresh();
        }

        public void SetRowCount(int rowCount = 1)
        {
            gridViewCore.SetRowCount(rowCount);
            SetScroll();
        }

        public void RemoveRowCount(int rowCount = 1)
        {
            gridViewCore.RemoveRowCount(rowCount);
            SetScroll();
            Refresh();
        }

        public void RemoveAllRowCount()
        {
            gridViewCore.RemoveAllRow();
            SetScroll();
            Refresh();
        }

        public float GetContentShowPos()
        {
            return gridViewCore.GetContentShowPos();
        }

        public float GetContentLastPos()
        {
            return gridViewCore.GetContentLastPos();

        }

        public int GetItemIdx(ScLayer item)
        {
            ScLayerLayoutViewerHeaderItem header = (ScLayerLayoutViewerHeaderItem)item.Parent.Parent;
            return header.Idx;
        }

        public void SetContentShowLastPos()
        {
            float lastPos = gridViewCore.GetContentLastPos();
            gridViewCore.SetContentShowPos(lastPos);
            SetScroll();        
        }

        public void SetContentShowPos(float pos)
        {
            gridViewCore.SetContentShowPos(pos);
            SetScroll();
        }

        public void CreateHeaderTitleLayer()
        {
            ScLayer headerTitleLayer;
            if (CreateHeaderTitleEvent != null)
            {
                headerTitleLayer = CreateHeaderTitleEvent(ScMgr);

                if (headerTitleLayer == null)
                    return;

                headerTitleLayer.Dock = ScDockStyle.Fill;
                gridViewCore.SetHeaderTitleLayer(headerTitleLayer);
            }
        }


        public List<ScLayerLayoutViewerItem> GetSelectedViewerItems()
        {
            return gridViewCore.GetSelectedViewerItems();
        }

        
        private void ScGridView_SizeChanged(object sender, SizeF oldSize)
        {
            float sideSpacingW = SideSpacing.left + SideSpacing.right;
            float sideSpacingH = SideSpacing.top + SideSpacing.bottom;
            float packWidth;
            float packHeight;

            float n =  IsUseInside ? 1 : 0;
            packWidth = Width - sideSpacingW - n;
            packHeight = Height - sideSpacingH - n;

            float headerH = gridViewCore.HeaderHeight + gridViewCore.HeaderSpacing;

            seletorScreen.Size = new SizeF(packWidth + SelectorMargin.left + SelectorMargin.right, Height - sideSpacingH - headerH + SelectorMargin.top + SelectorMargin.bottom);  
            seletorScreen.Location = new PointF(SideSpacing.left - SelectorMargin.left, SideSpacing.top + headerH - SelectorMargin.top);

            gridViewPack.DirectionRect = new RectangleF(SideSpacing.left, SideSpacing.top, packWidth, packHeight);
            gridViewCore.DirectionRect = new RectangleF(n, n, packWidth, packHeight);
       
            SetScroll();    
        }

        private void ScGridView_LocationChanged(object sender, PointF oldLocation)
        {
            if (ShadowLayer != null)
            {
                ScShadow shadow = (ScShadow)ShadowLayer;
                shadow.Location = new PointF(DirectionRect.X - shadow.ShadowRadius, DirectionRect.Y + shadow.ShadowRadius);
            }
        }

       
        void SetScroll()
        {
            SizeF contentSize = gridViewCore.GetContentSize();
            float viewportWidth = gridViewCore.GetViewportWidth();
            float viewportHeight = gridViewCore.GetViewportHeight();

            float spacingW = SideSpacing.left + SideSpacing.right;
            float spacingH = SideSpacing.top + SideSpacing.bottom;
            float packWidth;
            float packHeight;
            float n = IsUseInside ? 1 : 0;
            packWidth = Width - spacingW - n;
            packHeight = Height - spacingH - n;

            float headerH = gridViewCore.HeaderHeight + gridViewCore.HeaderSpacing;

            if (contentSize.Height > gridViewCore.GetViewportHeight())
            {
                scrollBarVertical.Width = VerScrollSize;
                scrollBarVertical.Height = Height;

                scrollBarVertical.Visible = true;       
                gridViewPack.Width = packWidth - scrollBarVertical.Width;
                seletorScreen.Width = packWidth + SelectorMargin.left + SelectorMargin.right - scrollBarVertical.Width;
                gridViewCore.Width = packWidth - scrollBarVertical.Width;

            }
            else
            {
                scrollBarVertical.Visible = false;
                gridViewPack.Width = packWidth;
                seletorScreen.Width = packWidth + SelectorMargin.left + SelectorMargin.right;
                gridViewCore.Width = packWidth;
            }

            if (contentSize.Width > gridViewCore.GetViewportWidth())
            {
                scrollBarHorizontal.Width = Width;
                scrollBarHorizontal.Height = HorScrollSize;

                scrollBarHorizontal.Visible = true;
                gridViewPack.Height = packHeight - scrollBarHorizontal.Height;
                seletorScreen.Height = Height - spacingH - headerH + SelectorMargin.top + SelectorMargin.bottom - scrollBarHorizontal.Height;
                gridViewCore.Height = packHeight - scrollBarHorizontal.Height;
            }
            else
            {
                scrollBarHorizontal.Visible = false;
                gridViewPack.Height = packHeight;
                seletorScreen.Height = Height - spacingH - headerH + SelectorMargin.top + SelectorMargin.bottom;
                gridViewCore.Height = packHeight;
            }

            if (scrollBarVertical.Visible && scrollBarHorizontal.Visible)
            {
                scrollBarHorizontal.Width = Width - scrollBarVertical.Width;
                scrollBarVertical.Height = Height - scrollBarHorizontal.Height;
            }

            if (scrollBarVertical.Visible)
            {
                scrollBarVertical.SetSliderRatio(contentSize.Height, gridViewCore.GetViewportHeight());
                scrollBarVertical.Location = new PointF(Width - scrollBarVertical.Width, scrollBarVertical.Location.Y);
                scrollBarVertical.SetSliderLocationByViewport(gridViewCore.GetContentShowPos());
            }

            if (scrollBarHorizontal.Visible)
            {
                PointF screenLoctain = gridViewCore.GetScreenLoaction();
                scrollBarHorizontal.SetSliderRatio(contentSize.Width, gridViewCore.Width);
                scrollBarHorizontal.Location = new PointF(scrollBarHorizontal.Location.X, Height - scrollBarHorizontal.Height);
                scrollBarHorizontal.SetSliderLocationByViewport(-screenLoctain.X);
            }

            if (ShadowLayer != null)
            {
                ScShadow shadow = (ScShadow)ShadowLayer;

                switch (ShadowStyle)
                {
                    case 0:

                        shadow.DirectionRect = new RectangleF(
                            DirectionRect.X - shadow.ShadowRadius,
                            DirectionRect.Y + shadow.ShadowRadius,
                            DirectionRect.Width + shadow.ShadowRadius * 2,
                            DirectionRect.Height);
                        break;

                    case 1:

                        shadow.DirectionRect = new RectangleF(
                            DirectionRect.X + gridViewPack.DirectionRect.X - shadow.ShadowRadius,
                            DirectionRect.Y + gridViewPack.DirectionRect.Y + shadow.ShadowRadius,
                            gridViewPack.Width + shadow.ShadowRadius * 2,
                            gridViewPack.Height);
                        break;


                    case 2:

                        shadow.DirectionRect = new RectangleF(
                           DirectionRect.X - shadow.ShadowRadius,
                           DirectionRect.Y - shadow.ShadowRadius,
                           DirectionRect.Width + shadow.ShadowRadius * 2,
                           DirectionRect.Height + shadow.ShadowRadius * 2);
                        break;


                    case 3:
                        shadow.DirectionRect = new RectangleF(
                            DirectionRect.X + gridViewPack.DirectionRect.X - shadow.ShadowRadius,
                            DirectionRect.Y + gridViewPack.DirectionRect.Y - shadow.ShadowRadius,
                            gridViewPack.Width + shadow.ShadowRadius * 2,
                            gridViewPack.Height + shadow.ShadowRadius * 2);
                        break;

                } 
            }
        }


     
    }
}
