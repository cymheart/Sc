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
    public class ScLayerLayoutViewerItem : ScLayer
    {
        public delegate void CreateItemDataLayerEventHandler(ScLayerLayoutViewerItem item);
        public event CreateItemDataLayerEventHandler CreateItemDataLayerEvent = null;

        public delegate void UserLayerValueChangedEventHandler(int dataIdx, string dataName, object value);
        public event UserLayerValueChangedEventHandler UserLayerValueChangedEvent = null;

        public delegate void MouseEventHandler(ScLayerLayoutViewerItem viewerItem, ScMouseEventArgs e);
        public event MouseEventHandler MouseEnterEvent = null;
        public event MouseEventHandler MouseDownEvent = null;
        public event MouseEventHandler MouseLeaveEvent = null;
        public event MouseEventHandler MouseDoubleDownEvent = null;

        public int DataIdx;
        public bool IsSelected;

        public Color SelectorColor = Color.FromArgb(100, 126, 171, 255);



        ScLayer hitLayer;
        ScLayer contentLayer;

        ScLayer selector;

        Margin SelectorMargin = new Margin(2,2,2,10);

        public ScLayer Selector{get { return selector; }}
        

        public ScLayerLayoutViewerItem(ScMgr scmgr = null)
            :base(scmgr)
        {
            selector = new ScLayer(scmgr);
            selector.IsUseDebugPanitCode = true;
            selector.D2DPaint += Selector_D2DPaint;
            Add(selector);

            contentLayer = new ScLayer(scmgr);
            Add(contentLayer);

            hitLayer = new ScLayer(scmgr);
            hitLayer.IsHitThrough = true;
            Add(hitLayer);


            hitLayer.MouseEnter += HitLayer_MouseEnter;
            hitLayer.MouseDown += HitLayer_MouseDown;
            hitLayer.MouseLeave += HitLayer_MouseLeave;
            hitLayer.MouseDoubleClick += HitLayer_MouseDoubleClick;

            SizeChanged += ScLayerLayoutViewerItem_SizeChanged;
            D2DPaint += ScLayerLayoutViewerItem_D2DPaint;

        }

     

        private void Selector_D2DPaint(D2DGraphics g)
        {
            if (!IsSelected)
                return;

            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
            RawRectangleF rect = new RawRectangleF(1, 1, selector.Width - 2, selector.Height - 9);
            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(SelectorColor);
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);

            RoundedRectangle rounderRect = new RoundedRectangle
            {
                RadiusX = 1,
                RadiusY = 1,
                Rect = rect
            };

            g.RenderTarget.DrawRoundedRectangle(rounderRect, brush, 2f);
            brush.Dispose();


            //
            GradientStop[] gradientStops = new GradientStop[2];
            gradientStops[0].Color = new RawColor4(0, 0, 0, 0.1f);
            gradientStops[0].Position = 0f;
            gradientStops[1].Color = new RawColor4(0, 0, 0, 0);
            gradientStops[1].Position = 1f;

            //
            GradientStopCollection gradientStopCollection = new GradientStopCollection(g.RenderTarget, gradientStops, Gamma.StandardRgb, ExtendMode.Clamp);

            //
            rect = new RawRectangleF(0, selector.Height - 9, selector.Width - 1, selector.Height - 1);
            LinearGradientBrushProperties props = new LinearGradientBrushProperties()
            {
                StartPoint = new RawVector2(rect.Left, rect.Top),
                EndPoint = new RawVector2(rect.Left, rect.Bottom)

            };

            SharpDX.Direct2D1.LinearGradientBrush linearGradientBrush = new SharpDX.Direct2D1.LinearGradientBrush(g.RenderTarget, props, gradientStopCollection);
            g.RenderTarget.FillRectangle(rect, linearGradientBrush);
            gradientStopCollection.Dispose();
            linearGradientBrush.Dispose();

            //Graphics gdiGraphics = g.CreateGdiGraphics();
            //gdiGraphics.SmoothingMode = SmoothingMode.HighQuality;
            //RectangleF rect1 = new RectangleF(0, 0, (float)Math.Ceiling(Width - 1), (float)Math.Ceiling(Height - 1));
            //Pen pen = new Pen(Color.FromArgb(255, 191, 152, 90), 1f);
            //DrawUtils.DrawRoundRectangle(gdiGraphics, Pens.Gold, rect1, 4);
            //g.RelaseGdiGraphics(gdiGraphics);
            //pen.Dispose();
        }

        private void HitLayer_MouseDown(object sender, ScMouseEventArgs e)
        {
            if (MouseDownEvent != null)
                MouseDownEvent(this, e);
        }

        private void HitLayer_MouseDoubleClick(object sender, ScMouseEventArgs e)
        {
            if (MouseDoubleDownEvent != null)
                MouseDoubleDownEvent(this, e);
        }


        private void HitLayer_MouseEnter(object sender, ScMouseEventArgs e)
        {
            if (MouseEnterEvent != null)
                MouseEnterEvent(this, e);
        }

        private void HitLayer_MouseLeave(object sender)
        {
            if (MouseLeaveEvent != null)
                MouseLeaveEvent(this, null);
        }

        public void AddContent(ScLayer layer)
        {
            contentLayer.Add(layer);
        }

        public ScLayer GetContent()
        {
            if (contentLayer.controls.Count() <= 0)
                return null;

            return contentLayer.controls[0];
        }

        public void ValueChanged(string name, object value)
        {
            if(UserLayerValueChangedEvent != null)
            {
                UserLayerValueChangedEvent(DataIdx, name, value);
            }
        }


        private void ScLayerLayoutViewerItem_D2DPaint(D2DGraphics g)
        {

            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
            RawRectangleF rect = new RawRectangleF(0, 0, Width, Height);
            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 255, 255, 255));
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.FillRectangle(rect, brush);

            brush.Dispose();
        }

        private void ScLayerLayoutViewerItem_SizeChanged(object sender, SizeF oldSize)
        {
            selector.Size = new SizeF(Width + SelectorMargin.left + SelectorMargin.right, Height + SelectorMargin.top + SelectorMargin.bottom);
            selector.Location = new PointF(-SelectorMargin.left, -SelectorMargin.top);

            contentLayer.Size = new SizeF(Width, Height);
            hitLayer.Size = new SizeF(Width, Height);

            if (CreateItemDataLayerEvent != null && contentLayer.controls.Count() == 0)
                CreateItemDataLayerEvent(this);
        }
    }
}
