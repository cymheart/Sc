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
    public class ScVxProgressBar :ScLayer
    {
        public string[] progressNodeNames;
        public RectangleF[] progressNodeRects;
        public ScProgressNodeAnim[] progressNodeAnims;
        public int curtProgressNodeIdx = 0;

        float nodesSpacing;
        public float BarHeight = 10;

        RectangleF barRect;

        ScStepAnimation progressStep;
        ScAnimation progressAnim;

        float animProgress = 0;
        float animStep = 8f;

        public delegate void ProgressEventHandler(float progressPos);
        public event ProgressEventHandler ProgressEvent;

        public delegate void MouseMoveEventHandler(int nodeIdx);
        public event MouseMoveEventHandler MouseMoveEvent;

        public ScVxProgressBar()
        {
            IsUseHitGeometryLayerBound = true;
            SizeChanged += ScVxProgressBar_SizeChanged;
            MouseMove += ScVxProgressBar_MouseMove;

            GDIPaint += ScVxProgressBar_GDIPaint;

            progressAnim = new ScAnimation(this, -1, true);
            progressAnim.DurationMS = 25;
            progressAnim.AnimationEvent += ProgressAnim_AnimationEvent;
        }

        ~ScVxProgressBar()
        {
            progressAnim.Stop();
        }


        public void AnimToNode(int nodeIdx)
        {
            if (curtProgressNodeIdx > progressNodeNames.Count() - 1 ||
                curtProgressNodeIdx < 0)
                return;

            if (nodeIdx > progressNodeNames.Count() - 1)
                curtProgressNodeIdx = progressNodeNames.Count() - 1;
            else
                curtProgressNodeIdx = nodeIdx;

            StartProgressAnim();
        }


        private void ScVxProgressBar_MouseMove(object sender, ScMouseEventArgs e)
        {
            RectangleF r;

            for (int i = 0; i < progressNodeRects.Count(); i++)
            {
                r = progressNodeRects[i];

                if(r.Contains(e.Location))
                {
                    if (MouseMoveEvent != null)
                        MouseMoveEvent(i);
                }                      
            }
        }

        public void StartProgressAnim()
        {
            progressAnim.Stop();
            progressStep = new ScStepAnimation(animProgress, animStep, progressAnim);
            progressAnim.Start();
        }

        private void ProgressAnim_AnimationEvent(ScAnimation scAnimation)
        {
            animProgress = progressStep.GetCurtValue();
            //float progressPos = animProgress * 0.01f * Width + 1;
            float progressPos = animProgress + 1;

            RectangleF r;
            ScProgressNodeAnim nodeAnim;

            for (int i = 0; i < progressNodeRects.Count(); i++)
            {
                r = progressNodeRects[i];

                if (progressPos >= r.Right)
                {
                    nodeAnim = progressNodeAnims[i];

                    if (nodeAnim.linear == null && nodeAnim.animScaleValue < 1.0f)
                    {
                        nodeAnim.StartScaleAnim();
                    }

                    if (i == curtProgressNodeIdx && curtProgressNodeIdx < progressNodeRects.Count() - 1)
                    {
                        //animProgress = (r.Right - 1) / (0.01f * Width);
                        animProgress = r.Right + 1;
                        progressAnim.Stop();
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            float val = (animProgress - 1) / (0.01f * Width);
            if (val >= 100)
            {
                animProgress = Width;
               // animProgress = 100;
                progressAnim.Stop();
            }

            if (ProgressEvent != null)
            {
                //progressPos = animProgress * 0.01f * Width + 1;
                progressPos = animProgress + 1;
                ProgressEvent(progressPos);
            }

            Refresh();
        }

        private void ScVxProgressBar_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            //
            Brush progressBrush = new SolidBrush(Color.WhiteSmoke);
            RectangleF progressRect = new RectangleF(0, 0, Width, Height - 1);
            graphis.FillRectangle(progressBrush, progressRect);
            progressBrush.Dispose();

            RectangleF rect = new RectangleF(0, 0, Width, Height);
            LinearGradientBrush progressBrush2 = new LinearGradientBrush(rect, Color.FromArgb(150, 0, 0, 0), Color.FromArgb(10, 0, 0, 0), LinearGradientMode.Vertical);
            graphis.FillRectangle(progressBrush2, rect);
            progressBrush2.Dispose();

            //
            //float progressWidth = animProgress * 0.01f * Width + 1;
            float progressWidth = animProgress + 1;

            progressBrush = new SolidBrush(Color.SkyBlue);
            progressRect = new RectangleF(0, 0, progressWidth, Height - 1);
            graphis.FillRectangle(progressBrush, progressRect);
            progressBrush.Dispose();

            rect = new RectangleF(0, 0, progressWidth, Height);
            progressBrush2 = new LinearGradientBrush(rect, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(140, 0, 0, 0), LinearGradientMode.Vertical);

            Color[] colors = new Color[3];
            colors[0] = Color.FromArgb(0, 0, 0, 0);
            colors[1] = Color.FromArgb(0, 0, 0, 0);
            colors[2] = Color.FromArgb(140, 0, 0, 0);

            ColorBlend blend = new ColorBlend();
            blend.Positions = new float[] { 0.0f, 0.3f, 1.0f };
            blend.Colors = colors;
            progressBrush2.InterpolationColors = blend;

            graphis.FillRectangle(progressBrush2, rect);
            progressBrush2.Dispose();


            //
            RectangleF r;
            ScProgressNodeAnim nodeAnim;

            for (int i=0; i< progressNodeRects.Count(); i++)
            {
                r = progressNodeRects[i];
                nodeAnim = progressNodeAnims[i];

                float w = (r.Width - r.Width / 6) * nodeAnim.animScaleValue;
                float x = r.X + r.Width / 2 - w / 2;

                float h = (r.Height - r.Height / 6) * nodeAnim.animScaleValue;
                float y = r.Y + r.Height / 2 - h / 2;

                RectangleF circleRect = new RectangleF(x, y, w, h);
                GraphicsPath path = CreateCircleGouPath(circleRect);

                progressBrush = new SolidBrush(Color.WhiteSmoke);
                graphis.FillPath(progressBrush, path);

                RectangleF circleRect2 = new RectangleF(x, y, w + 1, h + 1);
                 progressBrush2 = new LinearGradientBrush(circleRect2, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(130, 0, 0, 0), LinearGradientMode.Vertical);

                colors = new Color[3];
                colors[0] = Color.FromArgb(0, 0, 0, 0);
                colors[1] = Color.FromArgb(0, 0, 0, 0);
                colors[2] = Color.FromArgb(130, 0, 0, 0);

                blend = new ColorBlend();
                blend.Positions = new float[] { 0.0f, 0.3f, 1.0f };
                blend.Colors = colors;
                progressBrush2.InterpolationColors = blend;

                graphis.FillPath(progressBrush2, path);
            }
          
        }


        GraphicsPath CreateCircleGouPath(RectangleF r)
        {
            float w = r.Width / 4 * 3;
            float x = r.X + r.Width / 2 - w / 2;

            float h = r.Height / 4 * 3;
            float y = r.Y + r.Height / 2 - h / 2;
            RectangleF gouRect = new RectangleF(x, y, w, h);

            GraphicsPath path = CreateGouPath(gouRect);

            path.StartFigure();
            path.AddEllipse(r);
            path.CloseFigure();

            return path;
        }


        GraphicsPath CreateGouPath(RectangleF rect)
        {
            float x = rect.X;
            float y = rect.Y;
            float w = rect.Width / 8f;
            float h = rect.Height / 8f;

            GraphicsPath path = new GraphicsPath();

            path.StartFigure();

            float x1 = 3 * w + x;
            float y1 = 3 * h + y;
            float x2 = 4 * w + x;
            float y2 = 5 * h + y;
            float cx1 = 3.7f * w + x;
            float cy1 = 4f * h + y;
            float cx2, cy2;
   
            path.AddBezier(
            x1, y1,
            cx1, cy1,
            cx1, cy1,
            x2, y2);

            //
            x1 = 4 * w + x;
            y1 = 5 * h + y;
            x2 = 6 * w + x;
            y2 = h + y;
            cx1 = 4.7f * w + x;
            cy1 = 2.8f * h + y;
            cx2 = 5f * w + x;
            cy2 = 2.1f * h + y;

            path.AddBezier(
           x1, y1,
           cx1, cy1,
           cx2, cy2,
           x2, y2);


            //
            x1 = 6 * w + x;
            y1 = h + y;
            x2 = 7 * w + x;
            y2 = 3 * h + y;
            cx1 = 6.3f * w + x;
            cy1 = 2.3f * h + y;
            cx2 = 6.5f * w + x;
            cy2 = 2.4f * h + y;

            path.AddBezier(
           x1, y1,
           cx1, cy1,
           cx2, cy2,
           x2, y2);


            //
            x1 = 7 * w + x;
            y1 = 3 * h + y;
            x2 = 4 * w + x;
            y2 = 7 * h + y;
            cx1 = 5.5f * w + x;
            cy1 = 4f * h + y;
            cx2 = 5f * w + x;
            cy2 = 5f * h + y;

            path.AddBezier(
           x1, y1,
           cx1, cy1,
           cx2, cy2,
           x2, y2);


            //
            x1 = 4 * w + x;
            y1 = 7 * h + y;
            x2 = w + x;
            y2 = 5 * h + y;     
            cx1 = 3.5f * w + x;
            cy1 = 6f * h + y;
            cx2 = 2.8f * w + x;
            cy2 = 5f * h + y;

            path.AddBezier(
           x1, y1,
           cx1, cy1,
           cx2, cy2,
           x2, y2);


            //
            x1 = w + x;
            y1 = 5 * h + y;
            x2 = 3 * w + x;
            y2 = 3 * h + y;
            cx1 = 1.8f * w + x;
            cy1 = 3.7f * h + y;
            cx2 = 1.8f * w + x;
            cy2 = 3.7f * h + y;

            path.AddBezier(
           x1, y1,
           cx1, cy1,
           cx2, cy2,
           x2, y2);

            path.CloseFigure();

            return path;
        }


        private void ScVxProgressBar_SizeChanged(object sender, SizeF oldSize)
        {
            float nodeHalfWidth = Height / 2;
            RectangleF nodeRect;
            float x;
            List<RectangleF> nodeRectList = new List<RectangleF>();
            List<ScProgressNodeAnim> nodeAnimList = new List<ScProgressNodeAnim>();
            ScProgressNodeAnim nodeAnim;

            for (int i = 1; i < progressNodeNames.Count() + 1; i++)
            {
                x = nodesSpacing * i - nodeHalfWidth;
                nodeRect = new RectangleF(x, 0, Height-1, Height-1);
                nodeRectList.Add(nodeRect);

                //
                nodeAnim = new ScProgressNodeAnim(this, nodeRect);
                nodeAnim.AnimScaleEvent += NodeAnim_AnimScaleEvent;
                nodeAnimList.Add(nodeAnim);
            }

            progressNodeRects = nodeRectList.ToArray();
            progressNodeAnims = nodeAnimList.ToArray();

            //
            float y = nodeHalfWidth - BarHeight / 2;
            barRect = new RectangleF(0, y, Width - 1, BarHeight);

            AnimToNode(curtProgressNodeIdx);
        }

        private void NodeAnim_AnimScaleEvent()
        {
            if (ProgressEvent != null)
            {
                //float progressPos = animProgress * 0.01f * Width + 1;
                float progressPos = animProgress + 1;
                ProgressEvent(progressPos);
            }

            Refresh();
        }

        public void SetProgressNodesInfo(string[] progressNodeNames, float nodesSpacing)
        {
            this.progressNodeNames = progressNodeNames;
            this.nodesSpacing = nodesSpacing;
            Width = (progressNodeNames.Count() + 1) * nodesSpacing;
        }


        protected override GraphicsPath CreateHitGeometryByGDIPLUS(GDIGraphics g)
        {
            float r = BarHeight / 3;

            GraphicsPath path = DrawUtils.CreateRoundedRectanglePath(barRect, r);
            path.FillMode = FillMode.Winding;

            for (int i = 0; i < progressNodeRects.Count(); i++)
            {
                path.StartFigure();
                path.AddEllipse(progressNodeRects[i]);
                path.CloseFigure();
            }

            return path;
        }

    }
}
