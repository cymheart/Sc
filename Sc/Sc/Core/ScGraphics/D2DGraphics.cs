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


using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
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
    public class D2DGraphics : ScGraphics
    {
        static public SharpDX.Direct2D1.Factory d2dFactory = null;
        static public SharpDX.DirectWrite.Factory dwriteFactory = null;

        public RenderTarget renderTarget;
        public GdiInteropRenderTarget gdiRenderTarget;

        RawMatrix3x2 matrix = new RawMatrix3x2(
             1.0f, 0.0f,
             0.0f, 1.0f,
             0.0f, 0.0f
             );

        Control control = null;
        SharpDX.WIC.Bitmap wicBitmap;
        RenderTargetMode renderTargetMode;

        public D2DGraphics(Control control)
        {
            renderTargetMode = RenderTargetMode.HWND;

            CreateDeviceIndependentResource();

            if (control.Width <= 0 || control.Height <= 0)
                return;

            this.control = control;

            CreateDeviceDependentResource();
        }

        public D2DGraphics(SharpDX.WIC.Bitmap wicBitmap)
        {
            renderTargetMode = RenderTargetMode.WIC;

            CreateDeviceIndependentResource();

            this.wicBitmap = wicBitmap;
            CreateDeviceDependentResource();
        }

        /// <summary>
        /// 设备独立资源
        /// </summary>
        /// <returns></returns>
        bool CreateDeviceIndependentResource()
        {
            if (d2dFactory == null)
                d2dFactory = new SharpDX.Direct2D1.Factory(SharpDX.Direct2D1.FactoryType.MultiThreaded);

            if (dwriteFactory == null)
                dwriteFactory = new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Shared);

            return true;
        }

        /// <summary>
        /// 设备依赖资源
        /// </summary>
        /// <returns></returns>
        bool CreateDeviceDependentResource()
        {
            switch(renderTargetMode)
            {
                case RenderTargetMode.HWND:
                    CreateRenderTarget(control);
                    break;

                case RenderTargetMode.WIC:
                    CreateRenderTarget(wicBitmap);
                    break;
            }

            return true;
        }

        public void CreateRenderTarget(Control control)
        {
            if (renderTarget != null)
                return;

            var properties = new HwndRenderTargetProperties
            { 
                Hwnd = control.Handle,
                PixelSize = new Size2(control.Width, control.Height), 
                PresentOptions = PresentOptions.Immediately | PresentOptions.RetainContents
            };


            RenderTargetProperties rtProps = new RenderTargetProperties();
            rtProps.Usage = RenderTargetUsage.GdiCompatible;

            renderTarget = new WindowRenderTarget(d2dFactory, rtProps, properties)
            {
                AntialiasMode = AntialiasMode.PerPrimitive,
                TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Cleartype
            };


            IntPtr gdirtPtr;
            renderTarget.QueryInterface(Guid.Parse("e0db51c3-6f77-4bae-b3d5-e47509b35838"), out gdirtPtr);
            gdiRenderTarget = new GdiInteropRenderTarget(gdirtPtr);

        }

        public void CreateRenderTarget(SharpDX.WIC.Bitmap wicBitmap)
        {
            if (renderTarget != null)
                return;
            float dpiX = 96, dpiY = 96;
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
            }

            var renderTargetProperties = new RenderTargetProperties(
                RenderTargetType.Default,
                new SharpDX.Direct2D1.PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Unknown),
                dpiX, dpiY, RenderTargetUsage.GdiCompatible, FeatureLevel.Level_DEFAULT);

            renderTarget = new WicRenderTarget(d2dFactory, wicBitmap, renderTargetProperties);

            IntPtr gdirtPtr;
            renderTarget.QueryInterface(Guid.Parse("e0db51c3-6f77-4bae-b3d5-e47509b35838"), out gdirtPtr);
            gdiRenderTarget = new GdiInteropRenderTarget(gdirtPtr);
        }


        public override void ReSize(int width, int height)
        {
            if (control != null)
            {
                WindowRenderTarget wrt = (WindowRenderTarget)renderTarget;
                wrt.Resize(new Size2(width, height));
            }
        }

        public RenderTarget RenderTarget
        {
            get { return renderTarget; }
        }

        public Graphics CreateGdiGraphics()
        {
            IntPtr hdc = gdiRenderTarget.GetDC(DeviceContextInitializeMode.Copy);
            Graphics gdiGraphics = Graphics.FromHdc(hdc);
            gdiGraphics.Transform = layer.GlobalMatrix;
            return gdiGraphics;
        }

        public void RelaseGdiGraphics(Graphics gdiGraphics)
        {
            gdiRenderTarget.ReleaseDC();
            gdiGraphics.Dispose();
        }

        public override GraphicsType GetGraphicsType()
        {
            return GraphicsType.D2D;
        }
        public override void BeginDraw()
        {
            renderTarget.BeginDraw();
        }

        public override void EndDraw()
        {
            renderTarget.EndDraw();   
        }

        public override void ResetClip()
        {
            renderTarget.PopAxisAlignedClip();
        }

        public override void ResetTransform()
        {
            renderTarget.Transform = matrix;
        }

        public override void SetClip(System.Drawing.RectangleF clipRect)
        {
            RawRectangleF rawRectF = TransRectFToRawRectF(clipRect);
            renderTarget.PushAxisAlignedClip(rawRectF, AntialiasMode.PerPrimitive);
        }

        public override void TranslateTransform(float dx, float dy)
        {
            System.Drawing.Drawing2D.Matrix m = GDIDataD2DUtils.TransRawMatrix3x2ToMatrix(renderTarget.Transform);
            m.Translate(dx, dy);
            renderTarget.Transform = GDIDataD2DUtils.TransMatrixToRawMatrix3x2(m);
        }

        public override System.Drawing.Drawing2D.Matrix Transform
        {
            get
            {
                return GDIDataD2DUtils.TransRawMatrix3x2ToMatrix(renderTarget.Transform);
            }

            set
            {
                renderTarget.Transform = GDIDataD2DUtils.TransMatrixToRawMatrix3x2(value);
            }
        }

    
        RawRectangleF TransRectFToRawRectF(System.Drawing.RectangleF clipRect)
        {
            RawRectangleF rawRectF = 
                new RawRectangleF(
                clipRect.Left, clipRect.Top,
                clipRect.Right, clipRect.Bottom);

            return rawRectF;
        }

        public override void Dispose()
        {
            if (gdiRenderTarget != null)
            {
                gdiRenderTarget.Dispose();
                gdiRenderTarget = null;
            }

            if (renderTarget != null)
            {
                renderTarget.Dispose();
                renderTarget = null;
            }
        }     
    }
}
