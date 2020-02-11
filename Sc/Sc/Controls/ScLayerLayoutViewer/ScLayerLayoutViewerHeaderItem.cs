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
using System.Windows.Forms;

namespace Sc
{
    public class ScLayerLayoutViewerHeaderItem: ScLayer
    {
        public delegate void HeaderItemSizeChangedEventHandler(ScLayerLayoutViewerHeaderItem headerItem);
        public event HeaderItemSizeChangedEventHandler HeaderItemSizeChangedEvent = null;

        public delegate void HeaderItemHitEventHandler(ScLayerLayoutViewerHeaderItem headerItem, ScMouseEventArgs e);
        public event HeaderItemHitEventHandler HeaderItemHitEvent = null;

        public delegate void HeaderItemValueChangedEventHandler(string dataName, object value);
        public event HeaderItemValueChangedEventHandler HeaderItemValueChangedEvent = null;

        public delegate float ComputeHeaderItemWidthLimitEventHandler(ScLayerLayoutViewerHeaderItem adjustItem, float adjustWidth);
        public event ComputeHeaderItemWidthLimitEventHandler ComputeHeaderItemWidthLimitEvent = null;


        ScLayer hitLayer;
        ScLayer controler;
        ScLayer contentLayer;

        public float ControlerSize;
        public float ItemMinSize;

        PointF mouseDownPos;
        float mouseDownSize;

        public string ItemDataName;

        public int Idx;

        public ScLayerLayoutViewerHeaderOrientation HeaderOrientation;

        public bool IsTriggerItemSizeChangedEvent = true;

        public bool IsUseControler = true;

        public ScLayerLayoutViewerColumnInfo ColumnInfo;

        public ScLayer ContentLayer
        {
            get
            {
                return contentLayer;
            }
        }

        public ScLayer Controler
        {
            get
            {
                return controler;
            }
        }

        public ScLayerLayoutViewerHeaderItem(ScMgr scmgr)
            :base(scmgr)
        {
            contentLayer = new ScLayer(scmgr);
            Add(contentLayer);

            controler = new ScLayer(scmgr);
            controler.IsHitThrough = false;
            Add(controler);

            hitLayer = new ScLayer(scmgr);
            Add(hitLayer);

            SizeChanged += ScLayerLayoutViewerHeaderItem_SizeChanged;

            controler.MouseDown += Controler_MouseDown;
            controler.MouseMove += Controler_MouseMove;
            controler.MouseLeave += Controler_MouseLeave;
        
            hitLayer.MouseDown += HitLayer_MouseDown;

           // D2DPaint += ScLayerLayoutViewerHeaderItem_D2DPaint;
          
        }


       
        public void ValueChanged(object value)
        {
            if (HeaderItemValueChangedEvent != null)
            {
                HeaderItemValueChangedEvent(ItemDataName, value);
            }
        }

        private void ScLayerLayoutViewerHeaderItem_D2DPaint(D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
            RawRectangleF rect = new RawRectangleF(0, 0, Width - 1, Height - 1);
            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(55, 0, 0, 255));
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.FillRectangle(rect, brush);

            brush.Dispose();
        }

        private void HitLayer_MouseDown(object sender, ScMouseEventArgs e)
        {
            if (HeaderItemHitEvent != null)
                HeaderItemHitEvent(this, e);
        }

        private void Controler_MouseDown(object sender, ScMouseEventArgs e)
        {
            if (IsUseControler == false)
                return;

            mouseDownPos = Control.MousePosition;
            IsTriggerItemSizeChangedEvent = true;

            switch (HeaderOrientation)
            {
                case ScLayerLayoutViewerHeaderOrientation.HORIZONTAL:
                    mouseDownSize = Width;
                    break;

                case ScLayerLayoutViewerHeaderOrientation.VERTICAL:
                    mouseDownSize = Height;
                    break;
            }

        }

        private void Controler_MouseMove(object sender, ScMouseEventArgs e)
        {
            if (IsUseControler == false)
                return;

            if (e.Button == MouseButtons.Left)
            {
                float offset;

                SuspendLayout();

                switch (HeaderOrientation)
                {
                    case ScLayerLayoutViewerHeaderOrientation.HORIZONTAL:
                        offset = Control.MousePosition.X - mouseDownPos.X;

                        if (mouseDownSize + offset < ItemMinSize)
                            Width = ItemMinSize;
                        else
                        {
                            if (ComputeHeaderItemWidthLimitEvent != null)
                                Width = ComputeHeaderItemWidthLimitEvent(this, mouseDownSize + offset);
                            else
                                Width = mouseDownSize + offset;
                        }
                        break;

                    case ScLayerLayoutViewerHeaderOrientation.VERTICAL:
                        offset = Control.MousePosition.Y - mouseDownPos.Y;

                        if (mouseDownSize + offset < ItemMinSize)
                            Height = ItemMinSize;
                        else
                            Height = mouseDownSize + offset;
                        break;
                }

                ResumeLayout(true);
            }
            else
            {
                switch (HeaderOrientation)
                {
                    case ScLayerLayoutViewerHeaderOrientation.HORIZONTAL:
                        SetCursor(Cursors.VSplit);
                        break;

                    case ScLayerLayoutViewerHeaderOrientation.VERTICAL:
                        SetCursor(Cursors.HSplit);
                        break;
                }
            }     
        }

        private void Controler_MouseLeave(object sender)
        {
            SetCursor(Cursors.Default);
        }


        private void ScLayerLayoutViewerHeaderItem_SizeChanged(object sender, SizeF oldSize)
        {
            switch (HeaderOrientation)
            {
                case ScLayerLayoutViewerHeaderOrientation.HORIZONTAL:
                    HorSizeChanged(oldSize);
                    break;

                case ScLayerLayoutViewerHeaderOrientation.VERTICAL:
                    VerSizeChanged(oldSize);
                    break;
            }
        }


        void HorSizeChanged(SizeF oldSize)
        {
            float contentLayerWidth = Width - ControlerSize;

            controler.DirectionRect = new RectangleF(contentLayerWidth, 0, ControlerSize, Height);
            contentLayer.DirectionRect = new RectangleF(0, 0, contentLayerWidth, Height);
            hitLayer.DirectionRect = new RectangleF(0, 0, contentLayerWidth, Height);

            if (IsTriggerItemSizeChangedEvent && Width != oldSize.Width && HeaderItemSizeChangedEvent != null)
            {
                HeaderItemSizeChangedEvent(this);
            }
        }


        void VerSizeChanged(SizeF oldSize)
        {
            float contentLayerHeight = Height - ControlerSize;

            controler.DirectionRect = new RectangleF(0, contentLayerHeight, Width, ControlerSize);
            contentLayer.DirectionRect = new RectangleF(0, 0, Width, contentLayerHeight);
            hitLayer.DirectionRect = new RectangleF(0, 0, Width, contentLayerHeight);

            if (Height != oldSize.Height && HeaderItemSizeChangedEvent != null)
            {
                HeaderItemSizeChangedEvent(this);
            }
        }

    }
}
