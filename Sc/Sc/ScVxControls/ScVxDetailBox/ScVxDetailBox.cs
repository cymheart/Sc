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
    public enum DetailLayerType
    {
        ClothStyleLayer,
        TimerLayer,
        InfoLayer,
        BodyViewLayer,
        ProgressBarLayer
    }

    public class ScVxDetailBox : ScLayer
    {
        Table mainTable;
        Table infoTable;
        Table detailTable;

        public ScLayer clothStyleLayer;
        public ScLayer timerLayer;
        public ScLayer infoLayer;
        public ScLayer bodyViewLayer;
        public ScLayer progressBarLayer;

        public ScVxDetailBox()
        {
            SizeChanged += ScVxDetailBox_SizeChanged;
        }

        public void AddLayer(DetailLayerType layerType, ScLayer layer)
        {
            switch (layerType)
            {
                case DetailLayerType.ClothStyleLayer:
                    clothStyleLayer = layer;
                    break;

                case DetailLayerType.TimerLayer:
                    timerLayer = layer;
                    break;

                case DetailLayerType.InfoLayer:
                    infoLayer = layer;
                    break;

                case DetailLayerType.BodyViewLayer:
                    bodyViewLayer = layer;
                    break;

                case DetailLayerType.ProgressBarLayer:
                    progressBarLayer = layer;
                    break;
            }

            Add(layer);
        }


        public RectangleF GetRect(DetailLayerType layerType)
        {
            switch(layerType)
            {
                case DetailLayerType.ClothStyleLayer:
                    return infoTable.GetCellContentRect(0, 0);

                case DetailLayerType.TimerLayer:
                    return detailTable.GetCellContentRect(0, 0);

                case DetailLayerType.InfoLayer:
                    return detailTable.GetCellContentRect(1, 0);

                case DetailLayerType.BodyViewLayer:
                    return infoTable.GetCellContentRect(0, 2);

                case DetailLayerType.ProgressBarLayer:
                    return mainTable.GetCellContentRect(1, 0);
            }

            return new RectangleF();
        }

        private void ScVxDetailBox_SizeChanged(object sender, SizeF oldSize)
        {
            CreateTable();

            RectangleF r;

            if (clothStyleLayer != null)
            {
                r = GetRect(DetailLayerType.ClothStyleLayer);
                clothStyleLayer.Width = r.Width;
                clothStyleLayer.Height = r.Height;
                clothStyleLayer.Location = new PointF(r.X, r.Y);
            }

            //
            if (timerLayer != null)
            {
                r = GetRect(DetailLayerType.TimerLayer);
                timerLayer.Width = r.Width;
                timerLayer.Height = r.Height;
                timerLayer.Location = new PointF(r.X, r.Y);
            }


            //
            if (infoLayer != null)
            {
                r = GetRect(DetailLayerType.InfoLayer);
                infoLayer.Width = r.Width;
                infoLayer.Height = r.Height;
                infoLayer.Location = new PointF(r.X, r.Y);
            }


            //
            if (bodyViewLayer != null)
            {
                r = GetRect(DetailLayerType.BodyViewLayer);
                bodyViewLayer.Width = r.Width;
                bodyViewLayer.Height = r.Height;
                bodyViewLayer.Location = new PointF(r.X, r.Y);
            }


            //
            if (progressBarLayer != null)
            {
                r = GetRect(DetailLayerType.ProgressBarLayer);
                progressBarLayer.Width = r.Width;
                progressBarLayer.Height = r.Height;
                progressBarLayer.Location = new PointF(r.X, r.Y);
            }

        }


        void CreateTable()
        {
            RectangleF r = new RectangleF(0, 0, Width, Height);

            mainTable = new Table(r, 2, 1);

            TableLine tableLine = new TableLine(LineDir.HORIZONTAL);
            tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;

            tableLine.computeParam = 0.8f;
            mainTable.SetLineArea(0, tableLine);

            tableLine.computeParam = 0.2f;
            mainTable.SetLineArea(1, tableLine);

            mainTable.ComputeLinesArea(LineDir.HORIZONTAL);

            //
            infoTable = new Table(mainTable.GetCellContentRect(0, 0), 1, 3);

            Margin margin = new Margin(10, 20, 10, 20);
            infoTable.SetCellMargin(0, 0, margin);
            infoTable.SetCellMargin(0, 2, margin);

            margin = new Margin(20, 0, 20, 0);
            infoTable.SetCellMargin(0, 1, margin);

            //
            detailTable = new Table(infoTable.GetCellContentRect(0, 1), 2, 1);

            tableLine = new TableLine(LineDir.HORIZONTAL);
            tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;

            tableLine.computeParam = 0.3f;
            detailTable.SetLineArea(0, tableLine);

            tableLine.computeParam = 0.7f;
            detailTable.SetLineArea(1, tableLine);

            detailTable.ComputeLinesArea(LineDir.HORIZONTAL);



        }
    }
}
