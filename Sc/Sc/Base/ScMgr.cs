//----------------------------------------------------------------------------
// Simple Control (Sc) - Version 1.0
// A high quality control rendering engine for C#
// Copyright (C) 2016-2017 cymheart
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


using SharpDX;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Utils;

using Rectangle = System.Drawing.Rectangle;
using RectangleF = System.Drawing.RectangleF;
using Matrix = System.Drawing.Drawing2D.Matrix;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Bitmap = System.Drawing.Bitmap;
using WicBitmap = SharpDX.WIC.Bitmap;
using SharpDX.Direct2D1;

namespace Sc
{
    public class ScMgr : IDisposable
    {
        ScLayer rootParent;
        ScLayer rootScLayer = new ScLayer();      
        ScReDrawTree reDrawTree = new ScReDrawTree();

        ScLayer cacheRootScLayer = null;

        public ScLayer FocusScControl { get; set; }

        public Control control { get;}

        public Bitmap bitmap;
        public WicBitmap wicBitmap;

        List<ScLayer> mouseMoveScControlList;
        List<ScLayer> oldMouseMoveScControlList;

        public List<ScLayer> rebulidLayerList = new List<ScLayer>();
        public Dictionary<string, Dot9BitmapD2D> dot9BitmaShadowDict = new Dictionary<string, Dot9BitmapD2D>();
        public Color? BackgroundColor { get; set; }
        public System.Drawing.Image BackgroundImage { get; set; }

        ScGraphics graphics = null;

        MouseEventArgs globalMouseEventArgs = null;

        GraphicsType graphicsType = GraphicsType.GDIPLUS;
        DrawType drawType = DrawType.NOIMAGE;
        public ControlType controlType = ControlType.STDCONTROL;

        private Point mouseOffset; //记录鼠标指针的坐标
        private bool isMouseDown = false; //记录鼠标按键是否按下

        public Matrix matrix = new Matrix();

        public ScMgr(int width, int height, GraphicsType graphicsType = GraphicsType.D2D, DrawType drawType = DrawType.NOIMAGE)
        {
            cacheRootScLayer = rootScLayer;

            this.drawType = drawType;
            this.graphicsType = graphicsType;

            if (drawType == DrawType.NOIMAGE)
            {
                control = new ScLayerControl(this);
                control.Width = width;
                control.Height = height;
            }
            else
            {
                bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                if (graphicsType == GraphicsType.D2D)
                {
                    var wicFactory = new ImagingFactory();
                    wicBitmap = new WicBitmap(wicFactory, width, height, SharpDX.WIC.PixelFormat.Format32bppPBGRA, BitmapCreateCacheOption.CacheOnLoad);
                }
            }

           
            GraphicsType = graphicsType;

            rootScLayer.ScMgr = this;
            rootScLayer.Dock = ScDockStyle.Fill;
            rootScLayer.Name = "__root__";
            rootScLayer.D2DPaint += RootScControl_D2DPaint;

            rootParent = new ScLayer(this);
            rootParent.Add(rootScLayer);

         
            RegControlEvent();
        }

       

        public ScMgr(UpdateLayerFrm form, GraphicsType graphicsType = GraphicsType.D2D)
        {
            cacheRootScLayer = rootScLayer;
            drawType = DrawType.IMAGE;
            controlType = ControlType.UPDATELAYERFORM;
            form.scMgr = this;
            control = form;

            bitmap = new Bitmap(control.Width, control.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            if (graphicsType == GraphicsType.D2D)
            {
                var wicFactory = new ImagingFactory();
                wicBitmap = new WicBitmap(wicFactory, control.Width, control.Height, SharpDX.WIC.PixelFormat.Format32bppPBGRA, BitmapCreateCacheOption.CacheOnLoad);
            }

            form.bitmap = bitmap;

            this.graphicsType = graphicsType;
            GraphicsType = graphicsType;

            rootScLayer.ScMgr = this;
            rootScLayer.Dock = ScDockStyle.Fill;
            rootScLayer.Width = control.Width;
            rootScLayer.Height = control.Height;
            rootScLayer.D2DPaint += RootScControl_D2DPaint;

            rootParent = new ScLayer(this);
            rootParent.DirectionRect = rootScLayer.DirectionRect;
            rootParent.DrawBox = rootScLayer.DirectionRect;
            rootParent.Add(rootScLayer);

            rootScLayer.Layout();

            RegControlEvent();
        }

        public GraphicsType GraphicsType
        {
            get
            {
                return graphicsType;
            }

            set
            {
                graphicsType = value;
                CreateGraphics();
            }
        }

        public ScGraphics Graphics
        {
            get
            {
                return graphics;
            }
        }


        void CreateGraphics()
        {
            switch(GraphicsType)
            {
                case GraphicsType.D2D:
                    CreateD2D();
                    break;

                case GraphicsType.GDIPLUS:
                    CreateGDIDiver();
                    break;
            }
        }

        
        bool CreateD2D()
        {
            foreach (var item in dot9BitmaShadowDict)
            {
                item.Value.Dispose();
            }
            dot9BitmaShadowDict.Clear();

            //
            if (graphics != null)
                graphics.Dispose();
  
            if(cacheRootScLayer != null)
                rootScLayer = cacheRootScLayer;

            if (drawType == DrawType.NOIMAGE &&
                (control.Width <= 0 || control.Height <= 0))
            {
                rootScLayer = null;
                return false;
            }
            else if (drawType != DrawType.NOIMAGE &&
                (bitmap.Width <= 0 || bitmap.Height <= 0))
            {
                rootScLayer = null;
                return false;
            }


            if (drawType == DrawType.NOIMAGE)
                graphics = new D2DGraphics(control);
            else
                graphics = new D2DGraphics(wicBitmap);

         
            foreach (ScLayer layer in rebulidLayerList)
            {
                layer.ScReBulid();
            }

            return true;
        }

        void ReBulidD2D()
        {
                if (drawType == DrawType.NOIMAGE)
                {
                    graphics.ReSize(control.Width, control.Height);

                    foreach (ScLayer layer in rebulidLayerList)
                    {
                        layer.ScReBulid();
                    }
                }
            }

        void CreateGDIDiver()
        {
            if (drawType == DrawType.NOIMAGE)
                graphics = new GDIGraphics(control);
            else
                graphics = new GDIGraphics(bitmap);
        }

        public void AddReBulidLayer(ScLayer layer)
        {
            rebulidLayerList.Add(layer);
        }

        public void ClearBitmapRect(RectangleF clipRect)
        {
            if (bitmap != null && controlType == ControlType.UPDATELAYERFORM)
            {
                Rectangle rc = new Rectangle((int)clipRect.Left, (int)clipRect.Top, (int)clipRect.Width, (int)clipRect.Height);
                ColorDisplace.Displace(bitmap, Color.FromArgb(0, 0, 0, 0), rc, true);
            }
        }

        private void RootScControl_D2DPaint(D2DGraphics g)
        {
            RawRectangleF rect = new RawRectangleF(0, 0, rootScLayer.Width, rootScLayer.Height);

            if (BackgroundColor != null)
            {
                if (BackgroundColor.Value.A == 255)
                {
                    RawColor4 color = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(BackgroundColor.Value.ToArgb()));
                    g.RenderTarget.Clear(color);
                }
                else
                {
                    g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
                    RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(BackgroundColor.Value);
                    SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
                    g.RenderTarget.FillRectangle(rect, brush);
                    brush.Dispose();
                }
            }
        }

        private void RootScControl_GDIPaint(GDIGraphics g)
        {
            Graphics gdiGraph = g.GdiGraph;
            gdiGraph.SmoothingMode = SmoothingMode.Default;

            if (BackgroundColor != null)
            {
                RectangleF rect = new RectangleF(0, 0, rootScLayer.Width, rootScLayer.Height);
                System.Drawing.Brush brush = new SolidBrush(BackgroundColor.Value);
                gdiGraph.FillRectangle(brush, rect);
            }

            if(BackgroundImage != null)
            {
                RectangleF rect = new RectangleF(0, 0, rootScLayer.Width, rootScLayer.Height);
                gdiGraph.DrawImage(BackgroundImage, rect);
            }
        }


        public void SetImeWindowsPos(int x, int y)
        {
            if(controlType == ControlType.STDCONTROL)
                ((ScLayerControl)control).SetImeWindowsPos(x, y);
            else
                ((UpdateLayerFrm)control).SetImeWindowsPos(x, y);
        }


        public void PaintToBitmap()
        {
            if (bitmap == null || drawType != DrawType.IMAGE)
                return;

            Rectangle clipRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            PaintToBitmap(clipRect);
        }

        public void PaintToBitmap(Rectangle rc)
        {
            if (bitmap == null || drawType != DrawType.IMAGE)
                return;

            ScDrawNode rootNode;
            Rectangle clipRect = rc;
            graphics.BeginDraw();
            rootNode = reDrawTree.ReCreateReDrawTree(rootScLayer, clipRect);
            reDrawTree.Draw(graphics);
            graphics.EndDraw();

            if (graphicsType == GraphicsType.D2D && rootNode != null)
            {
                unsafe
                {
                    RectangleF clip = rootNode.clipRect;
                    Rectangle bitmapRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    BitmapData srcBmData = bitmap.LockBits(bitmapRect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);


                    //DataPointer dataPtr = new DataPointer((IntPtr)srcBmData.Scan0, bitmap.Height * srcBmData.Stride);
                    //wicBitmap.CopyPixels(srcBmData.Stride, dataPtr);

                    byte* ptr = (byte*)srcBmData.Scan0;
                    ptr += (int)clip.Y * srcBmData.Stride + (int)clip.X * 4;
                    DataPointer dataPtr = new DataPointer(ptr, (int)clip.Height * srcBmData.Stride);
                    RawBox box = new RawBox((int)clip.X, (int)clip.Y, (int)clip.Width, (int)clip.Height);
                    wicBitmap.CopyPixels(box, srcBmData.Stride, dataPtr);


                    bitmap.UnlockBits(srcBmData);
                }

                //unsafe
                //{
                //    RectangleF clip = rootNode.clipRect;
                //    Rectangle bitmapRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                //    BitmapData srcBmData = bitmap.LockBits(bitmapRect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

    
                //    BitmapLock bmplock = wicBitmap.Lock(BitmapLockFlags.Read);
                //    DataRectangle dataRect = bmplock.Data;
                //    byte* wicPtr = (byte*)dataRect.DataPointer;

                //    byte* dstScan = (byte*)(srcBmData.Scan0);
                //    byte* dstPtr = dstScan + (int)clip.Y * srcBmData.Stride + (int)clip.X * 4;
                //    byte* srcPtr = wicPtr + (int)clip.Y * bmplock.Stride + (int)clip.X * 4;
                //    int pos;

                //    for (int i = 0; i < (int)clip.Height; i++)
                //    {
                //        pos = i * srcBmData.Stride;

                //        for (int j = 0; j < (int)clip.Width; j++)
                //        {
                //            *(uint*)(dstPtr + pos) = *(uint*)(srcPtr + pos);
                //            pos += 4;
                //        }
                //    }

                //    bmplock.Dispose();
                //    bitmap.UnlockBits(srcBmData);
                //}
            }
        }


        public void Paint(PaintEventArgs e)
        {
            if (rootScLayer == null)
                return;

            if (graphics == null)
                return;

            Rectangle refreshArea = e.ClipRectangle;

            if (graphicsType == GraphicsType.GDIPLUS)
            {
                GDIGraphics gdig = (GDIGraphics)graphics;
                gdig.RootGdiGraph = e.Graphics;
                gdig.BeginDraw();
                reDrawTree.ReCreateReDrawTree(rootScLayer, refreshArea);
                reDrawTree.Draw(gdig);
                gdig.EndDraw();
            }
            else if (GraphicsType == GraphicsType.D2D)
            {
                _PaintByD2D(refreshArea);
            }
        }

        void _PaintByD2D(Rectangle refreshArea)
        {
            D2DGraphics d2dGraphics = (D2DGraphics)graphics;

            if (drawType == DrawType.NOIMAGE)
            {
                WindowRenderTarget wRT = (WindowRenderTarget)d2dGraphics.RenderTarget;
                WindowState wstate = wRT.CheckWindowState();
                if (wstate != WindowState.Occluded)
                    _RenderByD2D(d2dGraphics, refreshArea);
            }
            else
            {
                _RenderByD2D(d2dGraphics, refreshArea);
            }
        }

        void _RenderByD2D(D2DGraphics graph, Rectangle refreshArea)
        {
            try
            {
                graph.BeginDraw();
                reDrawTree.ReCreateReDrawTree(rootScLayer, refreshArea);
                reDrawTree.Draw(graph);
                graph.EndDraw();
            }
            catch (Exception ex)
            {
                CreateD2D();
                control.Refresh();
                control.Update();
            }
        }

        public ScLayer GetRootLayer()
        {
            return rootScLayer;
        }
        public void Refresh()
        {
            Refresh(Rectangle.Empty);
        }
        public void Refresh(RectangleF refreshArea)
        {
            Rectangle rect = new Rectangle((int)refreshArea.X -1, (int)refreshArea.Y - 1, (int)Math.Round(refreshArea.Width) + 2, (int)Math.Round(refreshArea.Height) + 2);

            if (controlType == ControlType.UPDATELAYERFORM)
            {
                ((UpdateLayerFrm)control).Invalidate(rect, true);
            }
            else
            {
                control.Invalidate(rect, true);
            }
        }


        public void Update()
        {
            control.Update();
        }


        private void RootScLayer_MouseUp(object sender, ScMouseEventArgs e)
        {
            // 修改鼠标状态isMouseDown的值
            // 确保只有鼠标左键按下并移动时，才移动窗体
            if (e.Button == MouseButtons.Left)
            {
                //松开鼠标时，停止移动
                isMouseDown = false;
                //Top高度小于0的时候，等于0
                if (control.Top < 0)
                {
                    control.Top = 0;
                }
            }
        }

        private void RootScLayer_MouseMove(object sender, ScMouseEventArgs e)
        {
            //确定开启了移动模式后
            if (isMouseDown)
            {
                //移动的位置计算
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                control.Location = mousePos;
            }
        }

        private void RootScLayer_MouseDown(object sender, ScMouseEventArgs e)
        {
            int xOffset;
            int yOffset;

            //点击窗体时，记录鼠标位置，启动移动
            if (e.Button == MouseButtons.Left)
            {
                xOffset = (int)-e.Location.X;
                yOffset = (int)-e.Location.Y;
                mouseOffset = new Point(xOffset, yOffset);
                isMouseDown = true;
            }
        }

        private void Control_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (mouseMoveScControlList == null)
                return;

            PointF ptf;
            PointF scMouseLocation;
            ScMouseEventArgs mouseEventArgs;

            foreach (ScLayer control in mouseMoveScControlList)
            {
                ptf = control.TransGlobalToLocal(e.Location);
                scMouseLocation = new PointF(ptf.X, ptf.Y);
                mouseEventArgs = new ScMouseEventArgs(e.Button, scMouseLocation);
                control.ScMouseDoubleClick(mouseEventArgs);
            }
        }

        private void Control_SizeChanged(object sender, EventArgs e)
        {
            ReBulid();
        }

        public void ReBulid()
        {
            if (control.Width <= 0 || control.Height <= 0)
            {
                rootScLayer = null;
                return;
            }

            if (GraphicsType == GraphicsType.D2D)
            {
                ReBulidD2D();
                rootScLayer = cacheRootScLayer;
            }

            rootScLayer.SuspendLayout();
            rootScLayer.DirectionRect = new RectangleF(0, 0, control.Width, control.Height);
            rootScLayer.DrawBox = rootScLayer.DirectionRect;
            rootScLayer.ResumeLayout(true);

        }

        private void Control_ImeStringEvent(string imeString)
        {
            if (FocusScControl != null)
                FocusScControl.ScImeStringEvent(imeString);
        }

        private void Control_CharEvent(char c)
        {
            if (FocusScControl != null)
                FocusScControl.ScCharEvent(c);
        }

        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            if (FocusScControl != null)
                FocusScControl.ScKeyUp(e);
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (FocusScControl != null)
                FocusScControl.ScKeyDown(e);
        }

        private void Control_LostFocus(object sender, EventArgs e)
        {
            if (FocusScControl != null)
                FocusScControl.ScLostFocus(e);
        }

        private void Control_GotFocus(object sender, EventArgs e)
        {
            if(FocusScControl != null)
                FocusScControl.ScGotFocus(e);
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            if (mouseMoveScControlList == null)
                return;

            foreach (ScLayer control in mouseMoveScControlList)
            {
                control.ScMouseLeave();
            }

            mouseMoveScControlList = null;
            oldMouseMoveScControlList = null;
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (mouseMoveScControlList == null)
                return;

            PointF ptf;
            PointF scMouseLocation;
            ScMouseEventArgs mouseEventArgs;

            foreach (ScLayer control in mouseMoveScControlList)
            {
                if (control.Visible == false)
                    continue;

                ptf = control.TransGlobalToLocal(e.Location);
                scMouseLocation = new PointF(ptf.X, ptf.Y);
                mouseEventArgs = new ScMouseEventArgs(e.Button, scMouseLocation);
                control.ScMouseDown(mouseEventArgs);
            }
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseMoveScControlList == null)
                return;

            PointF ptf;
            PointF scMouseLocation;
            ScMouseEventArgs mouseEventArgs;

            foreach (ScLayer control in mouseMoveScControlList)
            {
                if (control.Visible == false)
                    continue;

                ptf = control.TransGlobalToLocal(e.Location);
                scMouseLocation = new PointF(ptf.X, ptf.Y);
                mouseEventArgs = new ScMouseEventArgs(e.Button, scMouseLocation);
                control.ScMouseUp(mouseEventArgs);
            }
        }


        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            globalMouseEventArgs = e;
            MouseMove();
        }


        public void MouseMove()
        {       
            MouseEventArgs e = globalMouseEventArgs;

            if (e == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                ScMouseMove(e);
                return;
            }

            CheckScControlMouseMove(e.Location);

            ScMouseLeave();
            ScMouseEnter(e);
            ScMouseMove(e);

        }

        void CheckScControlMouseMove(Point mouseLocation)
        {
            if (rootScLayer == null)
                return;

            if (mouseMoveScControlList != null)
                oldMouseMoveScControlList = mouseMoveScControlList;

            mouseMoveScControlList = new List<ScLayer>();

            bool isChildHitThrough = CheckChildScControlMouseMove(rootScLayer, mouseLocation);

            if (isChildHitThrough)
                mouseMoveScControlList.Add(rootScLayer);
        }



        bool CheckChildScControlMouseMove(ScLayer parent, Point mouseLocation)
        {
            ScLayer layer;
            bool isChildHitThrough;
            bool ret = true;

            for (int i = parent.controls.Count() - 1; i >= 0; i--)
            {
                layer = parent.controls[i];

                if (layer.Visible == false)
                    continue;

                if (!rootScLayer.DrawBox.IntersectsWith(layer.DrawBox))
                {
                    continue;
                }

                if (layer.FillContainsPoint(mouseLocation))
                {
                    ret = true;
                    isChildHitThrough = CheckChildScControlMouseMove(layer, mouseLocation);

                    if (isChildHitThrough)
                    {
                        mouseMoveScControlList.Add(layer);

                        if (layer.IsHitThrough)
                            continue;
                        else
                            ret = false;
                    }
                    else
                        ret = false;

                    break;
                }
            }

            return ret;
        }

        void ScMouseMove(MouseEventArgs e)
        {
            PointF ptf;
            PointF scMouseLocation;
            ScMouseEventArgs mouseEventArgs;

            if (mouseMoveScControlList == null)
                return;

            foreach (ScLayer control in mouseMoveScControlList)
            {
                if (control.Visible == false)
                    continue;

                ptf = control.TransGlobalToLocal(e.Location);
                scMouseLocation = new PointF(ptf.X, ptf.Y);
                mouseEventArgs = new ScMouseEventArgs(e.Button, scMouseLocation);
                control.ScMouseMove(mouseEventArgs);
            }
        }

        void ScMouseLeave()
        {
            if (oldMouseMoveScControlList == null)
                return;

            bool isFind = false;

            foreach (ScLayer oldControl in oldMouseMoveScControlList)
            {
                if (oldControl.Visible == false)
                    continue;

                isFind = false;

                foreach (ScLayer newControl in mouseMoveScControlList)
                {
                    if (newControl.Visible == false)
                        continue;

                    if (oldControl == newControl)
                    {
                        isFind = true;
                        break;
                    }
                }

                if(isFind == false)
                {
                    oldControl.ScMouseLeave();
                }
            }
        }

        void ScMouseEnter(MouseEventArgs e)
        {
            bool isFind = false;
            PointF ptf;
            PointF scMouseLocation;
            ScMouseEventArgs mouseEventArgs;

            if (mouseMoveScControlList == null)
                return;

            foreach (ScLayer newControl in mouseMoveScControlList)
            {
                if (newControl.Visible == false)
                    continue;

                isFind = false;

                if (oldMouseMoveScControlList != null)
                {
                    foreach (ScLayer oldControl in oldMouseMoveScControlList)
                    {
                        if (oldControl.Visible == false)
                            continue;

                        if (newControl == oldControl)
                        {
                            isFind = true;
                            break;
                        }
                    }
                }

                if (isFind == false)
                {
                    ptf = newControl.TransGlobalToLocal(e.Location);
                    scMouseLocation = new PointF(ptf.X, ptf.Y);
                    mouseEventArgs = new ScMouseEventArgs(e.Button, scMouseLocation);
                    newControl.ScMouseEnter(mouseEventArgs);
                }
            }
        }


        void RegControlEvent()
        {
            if (control == null)
                return;

            control.MouseDown += Control_MouseDown;


            control.MouseLeave += Control_MouseLeave;
            control.MouseUp += Control_MouseUp;
            control.MouseMove += Control_MouseMove;

            control.MouseDoubleClick += Control_MouseDoubleClick;

            control.GotFocus += Control_GotFocus;
            control.LostFocus += Control_LostFocus;
            control.KeyDown += Control_KeyDown;
            control.KeyUp += Control_KeyUp;
            control.SizeChanged += Control_SizeChanged;

            if (controlType == ControlType.STDCONTROL)
            {
                ((ScLayerControl)control).CharEvent += Control_CharEvent;
                ((ScLayerControl)control).ImeStringEvent += Control_ImeStringEvent;
                ((ScLayerControl)control).DirectionKeyEvent += Control_KeyDown;
            }
            else
            {
                ((UpdateLayerFrm)control).CharEvent += Control_CharEvent;
                ((UpdateLayerFrm)control).ImeStringEvent += Control_ImeStringEvent;
                ((UpdateLayerFrm)control).DirectionKeyEvent += Control_KeyDown;

                rootScLayer.MouseDown += RootScLayer_MouseDown;
                rootScLayer.MouseUp += RootScLayer_MouseUp;
                rootScLayer.MouseMove += RootScLayer_MouseMove;
            }
        }

        public void Dispose()
        {
            if (graphics != null)
            {
                graphics.Dispose();             
            }

            if (dot9BitmaShadowDict != null)
            {
                foreach (var item in dot9BitmaShadowDict)
                {
                    item.Value.Dispose();
                }
                dot9BitmaShadowDict.Clear();
            }

            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }

            if(wicBitmap != null)
            {
                wicBitmap.Dispose();
                wicBitmap = null;
            }

            if (rebulidLayerList != null)
                rebulidLayerList.Clear();

            if (rootParent != null)
                rootParent.Dispose();
        }
    }
}
