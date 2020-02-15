using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using Sc;
using SharpDX.DirectWrite;

namespace Demo2
{
    public class TestLayer : ScLayer
    {
        ScShadow shadow;
        PointF mousePos;
        public TestLayer(ScMgr scmgr = null)
            : base(scmgr)
        {
            //设置层的阴影
            shadow = new ScShadow(scmgr);
            shadow.CornersRadius = 4;
            shadow.ShadowRadius = 15;       
            shadow.ShadowColor =  Color.Black;
            ShadowLayer = shadow;


            SizeChanged += ScPanel_SizeChanged;
            D2DPaint += ScPanel_D2DPaint;
            MouseMove += TestLayer_MouseMove;     

        }

        private void TestLayer_MouseMove(object sender, ScMouseEventArgs e)
        {
            mousePos = e.Location;
            Refresh();
        }

        private void ScPanel_SizeChanged(object sender, SizeF oldSize)
        {
            //设置阴影层的尺寸
            shadow.DirectionRect = new RectangleF(
                       DirectionRect.X - shadow.ShadowRadius,
                       DirectionRect.Y - shadow.ShadowRadius,
                       DirectionRect.Width + shadow.ShadowRadius * 2,
                       DirectionRect.Height + shadow.ShadowRadius * 2);

        }

        void DrawParamInfo(D2DGraphics g, TextFormat textFormat)
        {
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, new RawColor4(1, 0, 0, 1));
            RawRectangleF rect = new RawRectangleF(0, 0, 100, 200);
            g.RenderTarget.FillRectangle(rect, brush);


            //
            SolidColorBrush blackBrush = new SolidColorBrush(g.RenderTarget, new RawColor4(0, 0, 0, 1));

            string info = 
                "LayerWidth:" + Width + "\n" //
                 + "LayerHeight:" + Height ;

            g.RenderTarget.DrawText(info, textFormat, rect, blackBrush, DrawTextOptions.Clip);

        }


        private void ScPanel_D2DPaint(D2DGraphics g)
        {
            string name = Name;
            g.RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            TextFormat textFormat = new TextFormat(D2DGraphics.dwriteFactory, "微软雅黑", 10)
            { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center, WordWrapping = WordWrapping.Wrap };

            DrawParamInfo(g, textFormat);


            SolidColorBrush brush1 = new SolidColorBrush(g.RenderTarget, new RawColor4(1, 0, 0, 1));
            SolidColorBrush brush2 = new SolidColorBrush(g.RenderTarget, new RawColor4(0, 0, 0, 1));



            RawRectangleF rect = new RawRectangleF(100, 100, 200, 100 + 20);
            g.RenderTarget.FillRectangle(rect, brush1);
            g.RenderTarget.DrawText(mousePos.ToString(), textFormat, rect, brush2, DrawTextOptions.Clip);




            //SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, new RawColor4(1, 0, 0, 1));

            ////
            //StrokeStyleProperties ssp = new StrokeStyleProperties();
            //ssp.DashStyle = DashStyle.DashDot;
            //StrokeStyle strokeStyle = new StrokeStyle(D2DGraphics.d2dFactory, ssp);
            //SolidColorBrush brush2 = new SolidColorBrush(g.RenderTarget, new RawColor4(0, 0, 0, 1));
            //g.RenderTarget.DrawLine(new RawVector2(0, Height / 2), new RawVector2(Width, Height / 2), brush2, 0.5f, strokeStyle);

            ////
            //float widthStep = Width / 10.0f;
            //float seqStep = 20000 / 10.0f;
            //RawRectangleF rect;

            //for (int i = 0; i < 10; i++)
            //{
            //    SolidColorBrush brushx = new SolidColorBrush(g.RenderTarget, new RawColor4(0, 0, 0, 1));
            //    TextFormat textFormat = new TextFormat(D2DGraphics.dwriteFactory, "微软雅黑", 10)
            //    { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center };

            //    textFormat.WordWrapping = WordWrapping.Wrap;

            //    float x = (widthStep * i - 100 + widthStep * i + 100) / 2f;


            //    g.RenderTarget.DrawLine(new RawVector2(x, Height / 2), new RawVector2(x, Height / 2 + 3), brush2, 1f);

            //    rect = new RawRectangleF(widthStep * i - 100, Height / 2, widthStep * i + 100, Height / 2 + 15);
            //    string str = (i * seqStep).ToString();
            //    g.RenderTarget.DrawText(str, textFormat, rect, brush2, DrawTextOptions.Clip);
            //}


            //RawRectangleF rect = new RawRectangleF(0, 1, Width - 1, Height -1);
            //RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 255, 0, 0));
            //SolidColorBrush brush2 = new SolidColorBrush(g.RenderTarget, rawColor);
            //g.RenderTarget.DrawRectangle(rect, brush2, 5);

            //rect = new RawRectangleF(0, 0, Width - 1, 10 - 1);
            //rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 122, 151, 207));
            //brush = new SolidColorBrush(g.RenderTarget, rawColor);
            //g.RenderTarget.FillRectangle(rect, brush);


            //rect = new RawRectangleF(1, 1, Width - 1, Height - 1);
            //rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 214, 215, 220));
            //brush = new SolidColorBrush(g.RenderTarget, rawColor);
            //g.RenderTarget.DrawRectangle(rect, brush);
        }
    }
}
