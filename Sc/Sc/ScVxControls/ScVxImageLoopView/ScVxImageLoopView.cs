using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace Sc
{
    public class ScVxImageLoopView:ScLayer
    {
        ScLayer leftScreen;
        ScLayer rightScreen;
        ScLayer playControler;

        List<Image> imgList = new List<Image>();
        Image [] orgImgs;

        float downWrapperX;
        float downLeftScreenLocationX;
        float downRightScreenLocationX;

        public int togetherViewCount = 1;

        public bool isFixViewBox = true;

        Table mainTable;

        ScAnimation progressAnim;
        float animStep = 1f;
        bool isStopPlay = false;

        bool isShowPlayControl = true;

        public bool isAutoPlay = true;
        public bool canSlide = true;

        public ScVxImageLoopView()
        {
            leftScreen = new ScLayer();
            leftScreen.GDIPaint += LeftScreen_GDIPaint;

            rightScreen = new ScLayer();
            rightScreen.GDIPaint += RightScreen_GDIPaint;

            Add(leftScreen);
            Add(rightScreen);

            playControler = new ScLayer();
            playControler.IsHitThrough = false;
            playControler.MouseDown += PlayControler_MouseDown;
            playControler.GDIPaint += PlayControler_GDIPaint;
            
            Add(playControler);

            SizeChanged += ScVxImageLoopView_SizeChanged;

            MouseDown += ScVxImageLoopView_MouseDown;
            MouseMove += ScVxImageLoopView_MouseMove;
            MouseUp += ScVxImageLoopView_MouseUp;

            IsHitThrough = false;

            progressAnim = new ScAnimation(this, -1, true);
            progressAnim.DurationMS = 20;
            progressAnim.AnimationEvent += ProgressAnim_AnimationEvent;
        }


        public void StopAllAnim()
        {
            progressAnim.Stop();
        }

        public bool IsShowPlayControl
        {
            get { return isShowPlayControl; }
            set
            {
                isShowPlayControl = value;

                if (isShowPlayControl == false)
                    playControler.Visible = false;
                else
                    playControler.Visible = true;
            }
        }

        private void PlayControler_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;
           

            RectangleF r = new RectangleF(0, 0, playControler.Width, playControler.Height);

            if (isStopPlay)
            {
                float w = playControler.Width / 5;
                Brush brush = new SolidBrush(Color.FromArgb(155, Color.WhiteSmoke));

                RectangleF pauseRect = new RectangleF(w, 0, w, playControler.Height);
                graphis.FillRectangle(brush, pauseRect);
    
                pauseRect = new RectangleF(w * 3, 0, w, playControler.Height);
                graphis.FillRectangle(brush, pauseRect);
                brush.Dispose();
            }
            else
            {
                float w = playControler.Width / 5;
                GraphicsPath path = new GraphicsPath();
                path.StartFigure();


                PointF[] pts = new PointF[]
                {
                    new PointF(w, 0),
                    new PointF(w * 4,  playControler.Height/2),
                    new PointF(w, playControler.Height),
                    new PointF(w, 0)
                };

                path.CloseFigure();
                path.AddLines(pts);

                Brush brush = new SolidBrush(Color.FromArgb(155, Color.WhiteSmoke));
                graphis.FillPath(brush, path);
                brush.Dispose();
            }
        }

        private void PlayControler_MouseDown(object sender, ScMouseEventArgs e)
        {
            if (!isAutoPlay)
                return;

            if (isStopPlay)
            {
                StartProgressAnim();
                isStopPlay = false;
            }
            else
            {
                progressAnim.Stop();
                isStopPlay = true;
            }

            Refresh();
        }

        ~ScVxImageLoopView()
        {
            progressAnim.Stop();
        }

        private void ScVxImageLoopView_MouseUp(object sender, ScMouseEventArgs e)
        {
            if (!isAutoPlay)
                return;

            if (!isStopPlay)
                StartProgressAnim();
        }

        public void StartProgressAnim()
        {
            progressAnim.Stop();
            progressAnim.Start();
           
        }

        private void ProgressAnim_AnimationEvent(ScAnimation scAnimation)
        {    
            downLeftScreenLocationX = leftScreen.Location.X;
            downRightScreenLocationX = rightScreen.Location.X + rightScreen.Width;

            Move(animStep);

            Refresh();
        }

        private void RightScreen_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            RectangleF r;
            for (int i = 0; i < imgList.Count(); i++)
            {
                r = mainTable.GetCellContentRect(0, i);
                graphis.DrawImage(imgList[i], r.X + r.Width/2 - imgList[i].Width / 2, r.Y + r.Height / 2 - imgList[i].Height / 2);
            }
        }

        private void LeftScreen_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            RectangleF r;
            for(int i=0; i< imgList.Count(); i++)
            {
                r = mainTable.GetCellContentRect(0, i);
                graphis.DrawImage(imgList[i], r.X + r.Width / 2 - imgList[i].Width / 2, r.Y + r.Height / 2 - imgList[i].Height / 2);
            }
        }

        private void ScVxImageLoopView_SizeChanged(object sender, SizeF oldSize)
        {
            if (Width <= 0 || Height <= 0 )
                return;

            if(orgImgs != null)
                ReLayout();

            //
            playControler.Width = 20;
            playControler.Height = 20;

            float x = Width / 2 - playControler.Width / 2;
            float y = Height - 10 - playControler.Height;
            playControler.Location = new PointF(x, y);

            if(isAutoPlay)
                StartProgressAnim();

        }


        private void ScVxImageLoopView_MouseMove(object sender, ScMouseEventArgs e)
        {
            if (!canSlide)
                return;

            if (e.Button == MouseButtons.Left)
            {
                float offsetX = e.Location.X - downWrapperX;
                Move(offsetX);
                Refresh();
            }
        }


        void Move(float offsetX)
        {
            if(offsetX < 0)
            {
                float x = downLeftScreenLocationX + offsetX;
                float len = Math.Abs(x);
                int n = (int)len / (int)leftScreen.Width;
                float xpos = x + n * (int)leftScreen.Width;

                leftScreen.Location = new PointF(xpos, 0);
                rightScreen.Location = new PointF(xpos + leftScreen.Width, 0);
            }
            else
            {
                float x = downRightScreenLocationX + offsetX;
                float len = x - Width;
                int n = (int)len / (int)rightScreen.Width;
                float xpos = x - n * (int)leftScreen.Width;

                rightScreen.Location = new PointF(xpos -  rightScreen.Width, 0);
                leftScreen.Location = new PointF(rightScreen.Location.X - leftScreen.Width, 0);
            }
        }

        private void ScVxImageLoopView_MouseDown(object sender, ScMouseEventArgs e)
        {
            if (!canSlide)
                return;

            progressAnim.Stop();

            downWrapperX = e.Location.X;
            downLeftScreenLocationX = leftScreen.Location.X;
            downRightScreenLocationX = rightScreen.Location.X + rightScreen.Width;
        }

        public void SetImages(Image[] imgs)
        {
            orgImgs = imgs;
        }


        public void ReLayout()
        {
            imgList.Clear();

            int w = (int)Width / togetherViewCount;

            for (int i = 0; i < orgImgs.Count(); i++)
            {
                Bitmap newImg;
                Rectangle orgRect = new Rectangle(0, 0, orgImgs[i].Width, orgImgs[i].Height);
                Rectangle destRect;

                if (!isFixViewBox)
                    destRect = orgRect;
                else
                    destRect = new Rectangle(0, 0, w, (int)Height);


                newImg = new Bitmap(destRect.Width, destRect.Height);

                Graphics g = Graphics.FromImage(newImg);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawImage(orgImgs[i], destRect, orgRect, GraphicsUnit.Pixel);
                g.Dispose();

                imgList.Add(newImg);
            }

            int screenW = w * imgList.Count();
            int h = (int)Height;

            leftScreen.Width = screenW;
            leftScreen.Height = h;
            leftScreen.Location = new PointF(0, 0);

            rightScreen.Width = screenW;
            rightScreen.Height = h;
            rightScreen.Location = new PointF(screenW, 0);

            mainTable = new Table(leftScreen.DirectionRect, 1, imgList.Count());

            orgImgs = null;
        }

    }
}
