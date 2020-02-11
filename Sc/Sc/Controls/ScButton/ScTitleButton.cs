using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
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
    public class ScTitleButton:ScLayer
    {
        public Color[] colors;
     
        Color color = Color.FromArgb(234, 232, 233);
        Color fontColor = Color.FromArgb(234, 232, 233);

        Color normalColor = Color.FromArgb(234, 232, 233);
        Color enterColor = Color.FromArgb(248, 248, 245);
        Color downColor = Color.FromArgb(153, 114, 49);
        Color disableColor = Color.FromArgb(153, 114, 49);
  
        Color normalFontColor = Color.FromArgb(255, 191, 152, 90);
        Color enterFontColor = Color.FromArgb(248, 248, 245);
        Color downFontColor = Color.FromArgb(255, 233, 233, 233);
        Color disableFontColor = Color.FromArgb(255, 233, 233, 233);

        public Color NormalColor
        {
            get
            {
                return normalColor;
            }
            set
            {
                normalColor = value;
                color = normalColor;
            }
        }


        public Color EnterColor
        {
            get
            {
                return enterColor;
            }
            set
            {
                enterColor = value;
            }
        }

        public Color DownColor
        {
            get
            {
                return downColor;
            }
            set
            {
                downColor = value;
            }
        }


        public Color DisableColor
        {
            get
            {
                return disableColor;
            }
            set
            {
                disableColor = value;
            }
        }


        public Color NormalFontColor
        {
            get
            {
                return normalFontColor;
            }
            set
            {
                normalFontColor = value;
                fontColor = normalFontColor;
            }
        }


        public Color EnterFontColor
        {
            get
            {
                return enterFontColor;
            }
            set
            {
                enterFontColor = value;
            }
        }

        public Color DownFontColor
        {
            get
            {
                return downFontColor;
            }
            set
            {
                downFontColor = value;
            }
        }

        public Color DisableFontColor
        {
            get
            {
                return disableFontColor;
            }
            set
            {
                disableFontColor = value;
            }
        }



        ScAnimation scAnim;
        ScAnimation scFontColorAnim;

        ScLinearAnimation linearR;
        ScLinearAnimation linearG;
        ScLinearAnimation linearB;


        ScLinearAnimation linearFontR;
        ScLinearAnimation linearFontG;
        ScLinearAnimation linearFontB;


        public string Text = "";
        public float RadiusX = 6;
        public float RadiusY = 6;
        public int animMS = 10;
        public int SideShadowAlpha = 20;

        public int AnimMS
        {
            set
            {
                animMS = value;
                scAnim.animMS = value;
                scFontColorAnim.animMS = value;
            }
        }
            

        D2DFont foreFont = new D2DFont("微软雅黑", 12);

        public D2DFont ForeFont
        {
            get
            {
                return foreFont;
            }
            set
            {
                foreFont = value;
            }
        }

        public delegate void AnimalStopEventHandler(ScTitleButton button);
        public event AnimalStopEventHandler AnimalStopEvent = null;

        public delegate void PaintEventHandler(D2DGraphics g, RawRectangleF rect);
        public event PaintEventHandler PaintEvent = null;

        public ScTitleButton(ScMgr scmgr = null)
            :base(scmgr)
        {
            colors = new Color[]
            {
                Color.FromArgb(65, 134, 175, 254),
                Color.FromArgb(155, 118, 155, 253),
                Color.FromArgb(255, 118, 155, 253)
            };

            D2DPaint += ScButton_D2DPaint;


            ScShadow shadow = new ScShadow(scmgr);
            shadow.CornersRadius = 6;
            shadow.ShadowRadius = 3;
            shadow.ShadowColor = Color.FromArgb(15, 0, 0, 0);
            ShadowLayer = shadow;


            this.MouseDown += ScButton_MouseDown;
            this.MouseUp += ScButton_MouseUp;
            this.MouseEnter += ScButton_MouseEnter;
            this.MouseLeave += ScButton_MouseLeave;

            this.D2DPaint += ScButton_D2DPaint;
 

            //scAnim = new ScAnimation(this, animMS, true);
            //scAnim.AnimationEvent += ScAnim_AnimationEvent;

            //scFontColorAnim = new ScAnimation(this, animMS, true);
            //scFontColorAnim.AnimationEvent += ScFontColorAnim_AnimationEvent;


            IsUseOrgHitGeometry = false;

            SizeChanged += ScButton_SizeChanged;
            LocationChanged += ScButton_LocationChanged;


        }

       
        private void ScButton_D2DPaint(D2DGraphics g)
        {
            FillItemGeometry(g);
            DrawString(g);

            if (PaintEvent != null)
            {
                RawRectangleF rect = new RawRectangleF(0, 0, Width, Height);
                PaintEvent(g, rect);
            }
        }

        void FillItemGeometry(D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;

            RawRectangleF rect = new RawRectangleF(1, 1, Width - 1, Height - 1);
            RoundedRectangle roundedRect = new RoundedRectangle()
            {
                RadiusX = this.RadiusX,
                RadiusY = this.RadiusY,
                Rect = rect
            };

            GradientStop[] gradientStops = new GradientStop[5];

            gradientStops[0].Color = GDIDataD2DUtils.TransToRawColor4(colors[0]);
            gradientStops[0].Position = 0f;

            gradientStops[1].Color = GDIDataD2DUtils.TransToRawColor4(colors[1]);
            gradientStops[1].Position = 0.4f;

            gradientStops[2].Color = GDIDataD2DUtils.TransToRawColor4(colors[2]);
            gradientStops[2].Position = 0.5f;

            gradientStops[3].Color = GDIDataD2DUtils.TransToRawColor4(colors[1]);
            gradientStops[3].Position = 0.6f;

            gradientStops[4].Color = GDIDataD2DUtils.TransToRawColor4(colors[0]);
            gradientStops[4].Position = 1f;

            //
            GradientStopCollection gradientStopCollection = new GradientStopCollection(g.RenderTarget, gradientStops, Gamma.StandardRgb, ExtendMode.Clamp);

            //
            LinearGradientBrushProperties props = new LinearGradientBrushProperties()
            {
                StartPoint = new RawVector2(rect.Left, rect.Top),
                EndPoint = new RawVector2(rect.Left, rect.Bottom)

            };

            SharpDX.Direct2D1.LinearGradientBrush linearGradientBrush = new SharpDX.Direct2D1.LinearGradientBrush(g.RenderTarget, props, gradientStopCollection);
            g.RenderTarget.FillRoundedRectangle(roundedRect, linearGradientBrush);

            //
            gradientStops = new GradientStop[5];

            gradientStops[0].Color = GDIDataD2DUtils.TransToRawColor4(colors[2]);
            gradientStops[0].Position = 0f;

            gradientStops[1].Color = GDIDataD2DUtils.TransToRawColor4(colors[1]);
            gradientStops[1].Position = 0.1f;

            gradientStops[2].Color = GDIDataD2DUtils.TransToRawColor4(colors[0]);
            gradientStops[2].Position = 0.5f;

            gradientStops[3].Color = GDIDataD2DUtils.TransToRawColor4(colors[1]);
            gradientStops[3].Position = 0.9f;


            gradientStops[4].Color = GDIDataD2DUtils.TransToRawColor4(colors[2]);
            gradientStops[4].Position = 1f;

            //
            gradientStopCollection = new GradientStopCollection(g.RenderTarget, gradientStops, Gamma.StandardRgb, ExtendMode.Clamp);

            //
            props = new LinearGradientBrushProperties()
            {
                StartPoint = new RawVector2(rect.Left, rect.Top),
                EndPoint = new RawVector2(rect.Right, rect.Top)

            };

            linearGradientBrush = new SharpDX.Direct2D1.LinearGradientBrush(g.RenderTarget, props, gradientStopCollection);
            g.RenderTarget.FillRoundedRectangle(roundedRect, linearGradientBrush);


          

            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(SideShadowAlpha, 0, 0, 0));
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.DrawRoundedRectangle(roundedRect, brush);
        }

        void DrawString(D2DGraphics g)
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                g.RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
                RawRectangleF rect = new RawRectangleF(0, 0, Width - 1, Height - 1);


                SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, GDIDataD2DUtils.TransToRawColor4(fontColor));
                TextFormat textFormat = new TextFormat(D2DGraphics.dwriteFactory, foreFont.FamilyName, foreFont.Weight, foreFont.Style, foreFont.Size)
                { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center };

                textFormat.WordWrapping = WordWrapping.Wrap;

                g.RenderTarget.DrawText(Text, textFormat, rect, brush, DrawTextOptions.Clip);
            }
        }




        private void ScButton_SizeChanged(object sender, SizeF oldSize)
        {
            if (ShadowLayer != null)
            {
                ScShadow shadow = (ScShadow)ShadowLayer;
                shadow.DirectionRect = new RectangleF(DirectionRect.X - shadow.ShadowRadius, DirectionRect.Y + shadow.ShadowRadius + 2, DirectionRect.Width + shadow.ShadowRadius * 2, DirectionRect.Height);
            }
        }

        private void ScButton_LocationChanged(object sender, PointF oldLocation)
        {
            if (ShadowLayer != null)
            {
                ScShadow shadow = (ScShadow)ShadowLayer;
                shadow.Location = new PointF(DirectionRect.X - shadow.ShadowRadius, DirectionRect.Y + shadow.ShadowRadius + 2);
            }
        }

        protected override Geometry CreateHitGeometryByD2D(D2DGraphics g)
        {
            RawRectangleF rect = new RawRectangleF(
                0, 0,
                 Width - 1,
                Height - 1);

            RoundedRectangle roundedRect = new RoundedRectangle()
            {
                RadiusX = this.RadiusX,
                RadiusY = this.RadiusY,
                Rect = rect
            };

            RoundedRectangleGeometry roundedRectGeo = new RoundedRectangleGeometry(D2DGraphics.d2dFactory, roundedRect);
            return roundedRectGeo;
        }


      
        private void ScButton_MouseDown(object sender, ScMouseEventArgs e)
        {
            StartAnim(downColor);
            StartFontColorAnim(downFontColor);
        }

        private void ScButton_MouseUp(object sender, ScMouseEventArgs e)
        {
            StartAnim(enterColor);
            StartFontColorAnim(enterFontColor);
        }

        private void ScButton_MouseEnter(object sender, ScMouseEventArgs e)
        {
            StartAnim(enterColor);
            StartFontColorAnim(enterFontColor);
        }


        private void ScButton_MouseLeave(object sender)
        {
            StartAnim(normalColor);
            StartFontColorAnim(normalFontColor);
        }

        public void StartLeaveAnim()
        {
            StartAnim(normalColor);
            StartFontColorAnim(normalFontColor);
        }
        public void StartAnim(Color stopColor)
        {
            //scAnim.Stop();

            //linearR = new ScLinearAnimation(color.R, stopColor.R, scAnim);
            //linearG = new ScLinearAnimation(color.G, stopColor.G, scAnim);
            //linearB = new ScLinearAnimation(color.B, stopColor.B, scAnim);
            //scAnim.Start();
        }

        private void ScAnim_AnimationEvent(ScAnimation scAnimation)
        {
            int r, g, b;

            r = (int)linearR.GetCurtValue();
            g = (int)linearG.GetCurtValue();
            b = (int)linearB.GetCurtValue();

            color = Color.FromArgb(r, g, b);

            Refresh();
            Update();

            if (linearR.IsStop && linearG.IsStop && linearB.IsStop)
            {
                scAnimation.Stop();

                if (AnimalStopEvent != null)
                    AnimalStopEvent(this);
            }
        }
        public void StartLeaveFontColorAnim()
        {
            StartFontColorAnim(normalFontColor);
        }

        public void StartFontColorAnim(Color stopFontColor)
        {
            //scFontColorAnim.Stop();

            //linearFontR = new ScLinearAnimation(fontColor.R, stopFontColor.R, scFontColorAnim);
            //linearFontG = new ScLinearAnimation(fontColor.G, stopFontColor.G, scFontColorAnim);
            //linearFontB = new ScLinearAnimation(fontColor.B, stopFontColor.B, scFontColorAnim);
            //scFontColorAnim.Start();
        }

        private void ScFontColorAnim_AnimationEvent(ScAnimation scAnimation)
        {
            int r, g, b;

            r = (int)linearFontR.GetCurtValue();
            g = (int)linearFontG.GetCurtValue();
            b = (int)linearFontB.GetCurtValue();

            fontColor = Color.FromArgb(r, g, b);

            Refresh();
            Update();

            if (linearFontR.IsStop && linearFontG.IsStop && linearFontB.IsStop)
            {
                scAnimation.Stop();

                if (AnimalStopEvent != null)
                    AnimalStopEvent(this);
            }
        }
    }
}
