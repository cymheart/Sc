using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class ScVxMainContentBox : ScLayer
    {
        ScLayer infoLayer;

        ScLayer toolsLayer;
        ScLayer toolsShadowLayer;
        ScLayer toolsContentLayer;

        ScLayer viewLayer;
        ScLayer viewShadowLayer;
        ScLayer viewContentLayer;


        Rectangle infoRect;
        Rectangle toolsRect;
        Rectangle viewRect;
        Rectangle splitLineRect;


        public ScVxMainContentBox()
        {
            infoLayer = new ScLayer(); 
            toolsLayer = new ScLayer();
            toolsShadowLayer = new ScLayer();
            toolsContentLayer = new ScLayer();


            viewLayer = new ScLayer();
            viewShadowLayer = new ScLayer();
            viewContentLayer = new ScLayer();

            toolsShadowLayer.IsHitThrough = true;
            viewShadowLayer.IsHitThrough = true;

            Add(infoLayer);

            toolsLayer.Add(toolsContentLayer);
            toolsLayer.Add(toolsShadowLayer);
            Add(toolsLayer);

            viewLayer.Add(viewContentLayer);
            viewLayer.Add(viewShadowLayer);
            Add(viewLayer);

            SizeChanged += ScVxMainBox_SizeChanged;


            infoLayer.GDIPaint += InfoLayer_GDIPaint;

            toolsShadowLayer.GDIPaint += ToolsShadowLayer_GDIPaint;
            viewShadowLayer.GDIPaint += ViewShadowLayer_GDIPaint;


            GDIPaint += ScVxMainBox_GDIPaint;
        }

        private void InfoLayer_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            graphis.DrawString("生产进度", new Font("微软雅黑", 11), Brushes.White, 20, 25);
        }


        public void AddInfo(ScLayer iLayer)
        {
            float x = infoLayer.Location.X + infoLayer.Width / 4 * 2;
            float y = infoLayer.Location.Y;

            iLayer.Location = new PointF(x, y);
            iLayer.Width = infoLayer.Width / 5 * 2;
            iLayer.Height = infoLayer.Height;

            infoLayer.Add(iLayer);
        }


        public void AddTools(ScLayer toolsLayer)
        {
            toolsContentLayer.Clear();

            toolsLayer.Width = toolsContentLayer.Width;
            toolsLayer.Height = toolsContentLayer.Height;

            toolsContentLayer.Add(toolsLayer);
        }


        public void AddView(ScLayer viewLayer)
        {
            viewContentLayer.Clear();

            viewLayer.Width = viewContentLayer.Width;
            viewLayer.Height = viewContentLayer.Height;

            viewContentLayer.Add(viewLayer);
        }

        public void RemoveView()
        {
            viewContentLayer.Clear();
        }

        private void ScVxMainBox_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.Default;

            Brush brush = new SolidBrush(Color.FromArgb(255, 49, 67, 77));
            Rectangle rect = new Rectangle(0, 0, (int)(Width - 1), (int)(Height - 1));
            graphis.FillRectangle(brush, rect);
            brush.Dispose();

            //
            Color color = Color.FromArgb(255, 125, 175, 175);

            Pen pen = new Pen(color);
            graphis.DrawRectangle(pen, rect);
            pen.Dispose();


            // 
            color = Color.FromArgb(100, 125, 175, 175);
            brush = new SolidBrush(color);
            graphis.FillRectangle(brush, splitLineRect);
            brush.Dispose();
        }

        private void ToolsShadowLayer_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.Default;
            Rectangle rect = new Rectangle(0, 0, (int)toolsShadowLayer.Width,  (int)toolsShadowLayer.Height);
            LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(100, 0,0,0), Color.FromArgb(0, 0, 0, 0), LinearGradientMode.Horizontal);
            graphis.FillRectangle(brush, rect);
            brush.Dispose();
        }

        private void ViewShadowLayer_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            graphis.DrawString("订单完成情况", new Font("微软雅黑",11), Brushes.White, 20, 10);


            graphis.SmoothingMode = SmoothingMode.Default;
            Rectangle rect = new Rectangle(0, 0, (int)viewShadowLayer.Width, (int)viewShadowLayer.Height);
            LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(80, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), LinearGradientMode.Vertical);
            graphis.FillRectangle(brush, rect);
            brush.Dispose();
        }

        private void ScVxMainBox_SizeChanged(object sender, SizeF oldSize)
        {
            int h = 140;

           // int h = (int)Height / 6;
            int w = (int)Width / 5;

            //
            infoRect = new Rectangle(1, 1, w, h);

            //
            int w2 = (int)Width - 2 - infoRect.Width;
            toolsRect = new Rectangle(infoRect.Right, 1, w2, h);
        
            //
            int h2 = (int)Height - 4 - infoRect.Height;
            viewRect = new Rectangle(1, infoRect.Bottom + 2, (int)Width - 2, h2);

            //
            splitLineRect = new Rectangle(infoRect.Left, infoRect.Bottom, (int)Width - 2, 2);


            infoLayer.Location = new PointF(infoRect.X, infoRect.Y);
            infoLayer.Width = infoRect.Width;
            infoLayer.Height = infoRect.Height;

            //
            toolsContentLayer.Location = new PointF(0, 0);
            toolsContentLayer.Width = toolsRect.Width;
            toolsContentLayer.Height = toolsRect.Height;

            if(toolsContentLayer.controls.Count > 0)
            {
                toolsContentLayer.controls[0].Width = toolsContentLayer.Width;
                toolsContentLayer.controls[0].Height = toolsContentLayer.Height;
            }


            toolsShadowLayer.Location = new PointF(0, 0);
            toolsShadowLayer.Width = 30;
            toolsShadowLayer.Height = toolsRect.Height;

            toolsLayer.Location = new PointF(toolsRect.X, toolsRect.Y);
            toolsLayer.Width = toolsRect.Width;
            toolsLayer.Height = toolsRect.Height;


            //
            viewContentLayer.Location = new PointF(0, 0);
            viewContentLayer.Width = viewRect.Width;
            viewContentLayer.Height = viewRect.Height;

            if (viewContentLayer.controls.Count > 0)
            {
                viewContentLayer.controls[0].Width = viewContentLayer.Width;
                viewContentLayer.controls[0].Height = viewContentLayer.Height;
            }


            viewShadowLayer.Location = new PointF(0, 0);
            viewShadowLayer.Width = viewRect.Width;
            viewShadowLayer.Height = 30;

            viewLayer.Location = new PointF(viewRect.X, viewRect.Y);
            viewLayer.Width = viewRect.Width;
            viewLayer.Height = viewRect.Height;


            //
            if (infoLayer.controls.Count > 0)
            {
                ScLayer ilayer = infoLayer.controls[0];

                float x = infoLayer.Location.X + infoLayer.Width / 6 * 3 - 15;
                float y = infoLayer.Location.Y + 18;

                ilayer.Location = new PointF(x, y);
                ilayer.Width = infoLayer.Width / 5 * 2;
                ilayer.Height = infoLayer.Height / 6 * 5;
            }

        }
    }
}
