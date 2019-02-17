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
    public class ScVxMainBox:ScLayer
    {
        ScLayer titleLayer;
        ScLayer contentLayer;
        ScLayer dateLayer;
        ScLayer drakLayer;

        Table titleTable;

        ScLayer titleLeftLayer;
        ScLayer titleLeftLayer2;
        ScLayer titleMiddleLayer;
        ScLayer titleRightLayer;

        public Color bgColor = Color.FromArgb(255, 29, 39, 49);

        public int drakAlpha = 0;

        ScAnimation alphaAnim;

        public ScVxMainBox()
        {
            titleLayer = new ScLayer();
            contentLayer = new ScLayer();
            dateLayer = new ScLayer();
            drakLayer = new ScLayer();

            Add(titleLayer);
            Add(contentLayer);
            Add(dateLayer);
            Add(drakLayer);

            SizeChanged += ScVxMainBox_SizeChanged;
            GDIPaint += ScVxMainBox_GDIPaint;

            drakLayer.GDIPaint += DrakLayer_GDIPaint;


  
            alphaAnim = new ScAnimation(this, 200, true);
            alphaAnim.AnimationEvent += AlphaAnim_AnimationEvent;
        }

    
        private void AlphaAnim_AnimationEvent(ScAnimation scAnimation)
        {
           // float value = alphaAnim.GetCurtValue();
        }



        private void DrakLayer_GDIPaint(GDIGraphics g)
        {
            if (drakAlpha == 0)
                return;

            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.Default;


            Color c = Color.FromArgb(drakAlpha, Color.Black);
            Brush brush = new SolidBrush(c);
            RectangleF rect = new RectangleF(0, 0, Width, Height);
            graphis.FillRectangle(brush, rect);
            brush.Dispose();
        }

        private void ScVxMainBox_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.Default;

            Brush brush = new SolidBrush(bgColor);
            RectangleF rect = new RectangleF(0, 0, Width, Height);
            graphis.FillRectangle(brush, rect);

            brush.Dispose();
        }

        public void AddTitleLeft(ScLayer layer)
        {
            Remove(titleLeftLayer);

            RectangleF rect;

            if (titleTable != null)
            {
                rect = titleTable.GetCellContentRect(0, 0);
                layer.Width = rect.Width;
                layer.Height = rect.Height;
                layer.Location = new PointF(rect.X, rect.Y);
            }

            titleLayer.Add(layer);
            titleLeftLayer = layer;
        }

        public void AddTitleLeft2(ScLayer layer)
        {
            Remove(titleLeftLayer2);

            RectangleF rect;

            if (titleTable != null)
            {
                rect = titleTable.GetCellContentRect(0, 1);
                layer.Width = rect.Width;
                layer.Height = rect.Height;
                layer.Location = new PointF(rect.X, rect.Y);
            }

            titleLayer.Add(layer);
            titleLeftLayer2 = layer;
        }

        public void AddTitleMiddle(ScLayer layer)
        {
            Remove(titleMiddleLayer);

            RectangleF rect;

            if (titleTable != null)
            {
                rect = titleTable.GetCellContentRect(0, 1);
                layer.Width = rect.Width;
                layer.Height = rect.Height;
                layer.Location = new PointF(rect.X, rect.Y);
            }

            titleLayer.Add(layer);
            titleMiddleLayer = layer;
        }


        public void AddTitleRight(ScLayer layer)
        {
            Remove(titleRightLayer);

            RectangleF rect;

            if (titleTable != null)
            {
                rect = titleTable.GetCellContentRect(0, 1);
                layer.Width = rect.Width;
                layer.Height = rect.Height;
                layer.Location = new PointF(rect.X, rect.Y);
            }

            titleLayer.Add(layer);
            titleRightLayer = layer;
        }

        public void AddContent(ScLayer cLayer)
        {
            contentLayer.Clear();
            cLayer.Width = contentLayer.Width - 40;
            cLayer.Height = contentLayer.Height - 10;
            cLayer.Location = new PointF(20, 0);
            contentLayer.Add(cLayer);
        }


        public void AddDate(ScLayer dLayer)
        {
            dateLayer.Clear();
            dLayer.Width = dateLayer.Width;
            dLayer.Height = dateLayer.Height;
            dLayer.Location = new PointF(0, 0);
            dateLayer.Add(dLayer);
        }

        private void ScVxMainBox_SizeChanged(object sender, SizeF oldSize)
        {
            //
            titleLayer.Width = Width;
            titleLayer.Height = 100;
            titleLayer.Location = new PointF(0, 0);

            //
            RectangleF rect = new RectangleF(0, 0, titleLayer.Width, titleLayer.Height);
            titleTable = new Table(rect, 1, 4);

            TableLine tableLine = new TableLine(LineDir.VERTICAL);
            tableLine.lineComputeMode = LineComputeMode.ABSOLUTE;

            tableLine.computeParam = 200f;
            titleTable.SetLineArea(0, tableLine);
            titleTable.SetLineArea(1, tableLine);

            tableLine.computeParam = 400f;
            titleTable.SetLineArea(3, tableLine);

            TableLine tableLine2 = new TableLine(LineDir.VERTICAL);
            tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;

            tableLine2.computeParam = 1.0f;
            titleTable.SetLineArea(2, tableLine2);
            titleTable.ComputeLinesArea(LineDir.VERTICAL);


            if(titleLeftLayer != null)
            {
                rect = titleTable.GetCellContentRect(0, 0);

                titleLeftLayer.Width = rect.Width;
                titleLeftLayer.Height = rect.Height;
                titleLeftLayer.Location = new PointF(rect.X, rect.Y);
            }

            if (titleLeftLayer2 != null)
            {
                rect = titleTable.GetCellContentRect(0, 1);

                titleLeftLayer2.Width = rect.Width;
                titleLeftLayer2.Height = rect.Height;
                titleLeftLayer2.Location = new PointF(rect.X, rect.Y);
            }

            if (titleMiddleLayer != null)
            {
                rect = titleTable.GetCellContentRect(0, 2);

                titleMiddleLayer.Width = rect.Width;
                titleMiddleLayer.Height = rect.Height;
                titleMiddleLayer.Location = new PointF(rect.X, rect.Y);
            }

            if (titleRightLayer != null)
            {
                rect = titleTable.GetCellContentRect(0, 3);

                titleRightLayer.Width = rect.Width;
                titleRightLayer.Height = rect.Height;
                titleRightLayer.Location = new PointF(rect.X, rect.Y);
            }


            //
            contentLayer.Width = Width;
            contentLayer.Height = Height - titleLayer.Height;
            contentLayer.Location = new PointF(0, titleLayer.Location.Y + titleLayer.Height);

            //
            dateLayer.Width = 250;
            dateLayer.Height = 30;

            float y = contentLayer.Location.Y - dateLayer.Height / 2;
            float x = 30;
            dateLayer.Location = new PointF(x, y);

            if (contentLayer.controls.Count > 0)
            {
                ScLayer cLayer = contentLayer.controls[0];
                cLayer.Width = contentLayer.Width - 40;
                cLayer.Height = contentLayer.Height - 10;
                cLayer.Location = new PointF(20, 0);
            }


            if (dateLayer.controls.Count > 0)
            {
                ScLayer dLayer = dateLayer.controls[0];
                dLayer.Width = dateLayer.Width;
                dLayer.Height = dateLayer.Height;
                dLayer.Location = new PointF(0, 0);
            }


            //
            drakLayer.Width = Width;
            drakLayer.Height = Height;


        }

    }
}
