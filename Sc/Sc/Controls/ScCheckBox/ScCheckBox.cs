using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScCheckBox : ScLayer
    {
        public float boxSideWidth = 1;
        public Color BoxColor = Color.FromArgb(255, 150, 150, 150);
        public Color FillInBoxColor = Color.FromArgb(85, 121, 198, 248);
        public Color CheckColor = Color.FromArgb(255, 255,  0, 0);

        public int CheckType = 0;
        public Margin FillMargin = new Margin(2, 2, 3, 3);
        public bool IsUseInFill = true;
        public bool IsUseInFill2 = true;


        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;

                if(DrawCheckLayer != null)
                    DrawCheckLayer.Refresh();
            }
        }

        ScLayer DrawCheckLayer;

        int state = 0;
        bool isChecked = false;
       

        public ScCheckBox(ScMgr scmgr = null)
            :base(scmgr)
        {
            DrawCheckLayer = new ScLayer(scmgr);
            Add(DrawCheckLayer);

            DrawCheckLayer.D2DPaint += DrawCheckLayer_D2DPaint;

            SizeChanged += ScCheckBox_SizeChanged;
            D2DPaint += ScCheckBox_D2DPaint;
            MouseDown += ScCheckBox_MouseDown;
            MouseUp += ScCheckBox_MouseUp;
            MouseEnter += ScCheckBox_MouseEnter;
            MouseLeave += ScCheckBox_MouseLeave;

        }

        private void ScCheckBox_MouseUp(object sender, ScMouseEventArgs e)
        {
            state = 1;
            RectangleF rect = new RectangleF(0, 0, Width, Height);
            if (rect.Contains(e.Location))
            {
                isChecked = !isChecked;
                DrawCheckLayer.Refresh();
            }
        }

        private void ScCheckBox_MouseLeave(object sender)
        {
            state = 0;
            Refresh();
        }

        private void ScCheckBox_MouseEnter(object sender, ScMouseEventArgs e)
        {
            state = 1;
            Refresh();
        }

        private void ScCheckBox_MouseDown(object sender, ScMouseEventArgs e)
        {
            state = 2;        
            Refresh();
        }

        public void SetDrawCheckDirectParentLayer(ScLayer directParent)
        {
            if(CheckType == 0)
                DrawCheckLayer.DirectParentClipLayer = directParent;
        }

        private void DrawCheckLayer_D2DPaint(D2DGraphics g)
        {
            if(isChecked)
                DrawCheck(g);
        }

        private void ScCheckBox_SizeChanged(object sender, SizeF oldSize)
        {

            switch (CheckType)
            {
                case 0:
                default:
                    DrawCheckLayer.DirectionRect = new RectangleF(0, -Height, 2 * Width, 2 * Height);
                    break;

                case 1:
                    DrawCheckLayer.DirectionRect = new RectangleF(0, 0, Width, Height);
                    break;

            }

        }

       

        private void ScCheckBox_D2DPaint(D2DGraphics g)
        {
        
            switch(state)
            {
                case 0:
                    PaintState0(g);
                    break;

                case 1:
                    PaintState1(g);
                    break;

                case 2:
                    PaintState2(g);
                    break;
            }
        }

        void PaintState0(D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;

            RawRectangleF rect = new RawRectangleF(1, 1, Width - 1, Height - 1);
            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(BoxColor);
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.DrawRectangle(rect, brush, boxSideWidth);
            brush.Dispose();

            if(IsUseInFill)
                FillIn(g, Color.FromArgb(50, BoxColor.R, BoxColor.G, BoxColor.B));
        }


        void PaintState1(D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
            RawRectangleF rect = new RawRectangleF(1, 1, Width - 1, Height - 1);

            int a = Math.Min(BoxColor.A + 60, 255);
            int R = Math.Max(BoxColor.R - 30, 0);
            int G = Math.Max(BoxColor.G - 30, 0);
            int B = Math.Max(BoxColor.B - 30, 0);

            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(a, R, G, B));
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.DrawRectangle(rect, brush, boxSideWidth);
            brush.Dispose();

            if (IsUseInFill)
                FillIn(g, FillInBoxColor);

            if (IsUseInFill2)
                FillIn2(g, FillInBoxColor);
        }


        void PaintState2(D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
            RawRectangleF rect = new RawRectangleF(1, 1, Width - 1, Height - 1);

            int a = Math.Min(BoxColor.A + 100, 255);
            int R = Math.Max(BoxColor.R - 130, 0);
            int G = Math.Max(BoxColor.G - 130, 0);
            int B = Math.Max(BoxColor.B - 130, 0);

            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(a, R, G, B));
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.DrawRectangle(rect, brush, boxSideWidth);
            brush.Dispose();


            R = Math.Max(FillInBoxColor.R - 50, 0);
            G = Math.Max(FillInBoxColor.G - 50, 0);
            B = Math.Max(FillInBoxColor.B - 50, 0);

            if (IsUseInFill)
                FillIn(g, Color.FromArgb(FillInBoxColor.A, R, G, B));

            if (IsUseInFill2)
                FillIn2(g, Color.FromArgb(FillInBoxColor.A, R, G, B));
 
        }


        void FillIn(D2DGraphics g, Color boxColor)
        {
            RawRectangleF rect = new RawRectangleF(FillMargin.left + 1, FillMargin.top + 1, Width - FillMargin.right, Height - FillMargin.bottom);
            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(boxColor);
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.DrawRectangle(rect, brush, 1);
            brush.Dispose();

            //
            RawRectangleF fillRect = new RawRectangleF(FillMargin.left, FillMargin.top, Width - FillMargin.right, Height - FillMargin.bottom);
            g.RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            GradientStop[] gradientStops = new GradientStop[3];

        
            gradientStops[0].Color = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, boxColor.R, boxColor.G, boxColor.B));
            gradientStops[0].Position = 0f;


            gradientStops[1].Color = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(20, boxColor.R, boxColor.G, boxColor.B));
            gradientStops[1].Position = 0.5f;

            gradientStops[2].Color = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(0, boxColor.R, boxColor.G, boxColor.B));
            gradientStops[2].Position = 1f;

            //
            GradientStopCollection gradientStopCollection = new GradientStopCollection(g.RenderTarget, gradientStops, Gamma.StandardRgb, ExtendMode.Clamp);

            LinearGradientBrushProperties props = new LinearGradientBrushProperties()
            {
                StartPoint = new RawVector2(fillRect.Left, fillRect.Top),
                EndPoint = new RawVector2(fillRect.Right, fillRect.Bottom)
            };

            SharpDX.Direct2D1.LinearGradientBrush linearGradientBrush = new SharpDX.Direct2D1.LinearGradientBrush(g.RenderTarget, props, gradientStopCollection);
            g.RenderTarget.FillRectangle(fillRect, linearGradientBrush);
            gradientStopCollection.Dispose();
            linearGradientBrush.Dispose();
       
        }

        void FillIn2(D2DGraphics g, Color boxColor)
        {
            RawRectangleF rect = new RawRectangleF(0, 0, Width, Height);
            RadialGradientBrushProperties props2 = new RadialGradientBrushProperties()
            {
                Center = new RawVector2((rect.Left + rect.Right) / 2, (rect.Top + rect.Bottom) / 2),
                GradientOriginOffset = new RawVector2(0, 0),
                RadiusX = Width / 2,
                RadiusY = Height / 2
            };

            GradientStop[] gradientStops2 = new GradientStop[2];

            gradientStops2[0].Color = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(55, boxColor.R, boxColor.G, boxColor.B));
            gradientStops2[0].Position = 0f;


            //gradientStops2[1].Color = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(40, 0,0,255));
            //gradientStops2[1].Position = 0.5f;

            gradientStops2[1].Color = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(55, boxColor.R, boxColor.G, boxColor.B));
            gradientStops2[1].Position = 1f;

            //
            GradientStopCollection gradientStopCollection2 = new GradientStopCollection(g.RenderTarget, gradientStops2, Gamma.StandardRgb, ExtendMode.Clamp);
            RadialGradientBrush radialGradientBrush = new RadialGradientBrush(g.RenderTarget, props2, gradientStopCollection2);

            g.RenderTarget.FillRectangle(rect, radialGradientBrush);

            gradientStopCollection2.Dispose();
            radialGradientBrush.Dispose();
        }

        void DrawCheck(D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            Geometry checkGeometry = null;

            switch(CheckType)
            {
                case 0: checkGeometry = CreateCheckGeometry0(g); break;
                case 1: checkGeometry = CreateCheckGeometry1(g); break;
                default: checkGeometry = CreateCheckGeometry0(g); break;
            }
            
            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(CheckColor);
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.FillGeometry(checkGeometry, brush);
            brush.Dispose();
            checkGeometry.Dispose();
        }

  
        Geometry CreateCheckGeometry0(D2DGraphics g)
        {
            RectangleF rect = new RectangleF(0, Height, Width, Height);
            float w = rect.Width / 2;
            float h = rect.Height / 4;

            PathGeometry pathGeometry = new PathGeometry(D2DGraphics.d2dFactory);

            GeometrySink pSink = null;
            pSink = pathGeometry.Open();
            pSink.SetFillMode(SharpDX.Direct2D1.FillMode.Winding);

            pSink.BeginFigure(new RawVector2(rect.Left, rect.Top), FigureBegin.Filled);


    
            RawVector2[] points =
                {
                new RawVector2(rect.Left + w, rect.Top + h * 2),
                new RawVector2(rect.Right + w, rect.Top - h * 2),
                new RawVector2(rect.Left + w, rect.Bottom),
            };

            pSink.AddLines(points);
            pSink.EndFigure(FigureEnd.Closed);
            pSink.Close();
            pSink.Dispose();

            return pathGeometry;
        }



        Geometry CreateCheckGeometry1(D2DGraphics g)
        {
            RectangleF rect = new RectangleF(FillMargin.left , FillMargin.top , Width - FillMargin.right - FillMargin.left, Height - FillMargin.bottom - FillMargin.top);

            float w = rect.Width / 2;
            float h2 = rect.Height / 2;
            float h4 = rect.Height / 4f;

            PathGeometry pathGeometry = new PathGeometry(D2DGraphics.d2dFactory);

            GeometrySink pSink = null;
            pSink = pathGeometry.Open();
            pSink.SetFillMode(SharpDX.Direct2D1.FillMode.Winding);

            pSink.BeginFigure(new RawVector2(rect.Left, rect.Top + h2), FigureBegin.Filled);



            RawVector2[] points =
                {
                new RawVector2(rect.Left + w -  w / 10, rect.Top + h4 * 2.7f),
                new RawVector2(rect.Right, rect.Top),
                new RawVector2(rect.Left + w, rect.Bottom),
            };

            pSink.AddLines(points);
            pSink.EndFigure(FigureEnd.Closed);
            pSink.Close();
            pSink.Dispose();

            return pathGeometry;
        }

    }
}
