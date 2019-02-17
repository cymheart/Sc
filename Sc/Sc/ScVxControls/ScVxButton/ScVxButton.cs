using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace Sc
{


    public class ScVxButton:ScLayer
    {
        List<ScTxtInfo> rowTextInfoList = new List<ScTxtInfo>();
        Table mainTable;
        Table txtTable;

        ScVxBtnContentLayer btnLayer;
        ScLayer imgLayer;
        ScLayer spaceLayer;

        public int txtRowCount = 1;
        int state = 0;

        public Bitmap effectBitmap;
        public Bitmap haloBitmap;
        public byte[,] haloAlphaMask;
        public Bitmap shadowBitmap;

        public bool isShowGow = false;
        public bool isSelected = false;

        public int TxtRowCount
        {
            get
            {
                return txtRowCount;
            }
            set
            {
                txtRowCount = value;
                ReCreateTable();

                for (int i = 0; i < txtRowCount; i++)
                {
                    rowTextInfoList.Add(null);
                }            
            }
        }

        public Image img { get; set; }
        public Color bgColor = Color.White;
        public Font txtFont = new Font("微软雅黑", 12);
        public Color txtColor = Color.Black;

        public Color progressbarColor = Color.FromArgb(255, 105, 163, 175);
        public Color selectHaloColor = Color.FromArgb(255, 105, 163, 175);

        public float progress = 0;
        public float animProgress = 0;

        float maxZoom = 1.2f;
        float minZoom = 1.0f;
        float zoom = 1.2f;
        float animZoom = 1.0f;

       

        ScLinearAnimation progressLinear;
        ScLinearAnimation zoomLinear;
        ScLinearAnimation haloLinear;

        ScAnimation progressAnim;
        ScAnimation zoomAnim;
        ScAnimation haloAnim;


        float animHaloAlphaScale = 0.3f;
        float endHaloAlphaScale = 1f;

        GraphicsPath gouPath;
        public bool IsUsedProgressAnim{set; get;}

        public ScVxButton()
        {
            IsUsedProgressAnim = false;

            spaceLayer = new ScLayer();
            btnLayer = new ScVxBtnContentLayer();
            imgLayer = new ScLayer();
     
            Add(spaceLayer);
            spaceLayer.Add(btnLayer);
            Add(imgLayer);

            btnLayer.IsUseHitGeometryLayerBound = true;

           // PostTreatmentEffectGDI += ScVxButton_PostTreatmentEffectGDI;
           // ReleasePostTreatmentEffectGDI += ScVxButton_ReleasePostTreatmentEffectGDI;
            SizeChanged += ScVxButton_SizeChanged;

            LostFocus += ScVxButton_LostFocus;
            GotFocus += ScVxButton_GotFocus;

            btnLayer.GDIPaint += BtnLayer_GDIPaint;
            imgLayer.GDIPaint += ImgLayer_GDIPaint;

            spaceLayer.MouseEnter += BtnLayer_MouseEnter;
            spaceLayer.MouseLeave += BtnLayer_MouseLeave;
            spaceLayer.MouseDown += BtnLayer_MouseDown;

            GDIPaint += ScVxButton_GDIPaint;


            progressAnim = new ScAnimation(this, 400, true);
            progressAnim.AnimationEvent += ScAnim_AnimationEvent;


            haloAnim = new ScAnimation(this, 600, true);
            haloAnim.AnimationEvent += HaloAnim_AnimationEvent;


            //   zoomAnim = new ScAnimation(this, 20, true);
            //   zoomAnim.AnimationEvent += ZoomAnim_AnimationEvent;

        }

        ~ScVxButton()
        {
            StopAllAnim();
        }

        public void StopAllAnim()
        {
            progressAnim.Stop();
            haloAnim.Stop();

            for (int i = 0; i < TxtRowCount; i++)
            {
                rowTextInfoList[i].StopScaleAnim();
            }
        }


        public void StartAllAnim()
        {
            StartProgressAnim();

            if (progress != 100)
            {
                for (int i = 0; i < TxtRowCount; i++)
                {
                    rowTextInfoList[i].StartScaleAnim();
                }  
            }

            if(isSelected)
            {
                StartHaloAnim();
            }
        }


        private void ScVxButton_GotFocus(object sender, EventArgs e)
        {
            UsePosttreatmentEffect = true;
            Refresh();
        }

        private void ScVxButton_LostFocus(object sender, EventArgs e)
        {
            UsePosttreatmentEffect = false;

            if (effectBitmap != null)
            {
                effectBitmap.Dispose();
                effectBitmap = null;
            }
            Refresh();
        }

        private Bitmap ScVxButton_PostTreatmentEffectGDI(object sender, Bitmap backBitmap)
        {
            //if (effectBitmap != null)
            //{
            //    effectBitmap.Dispose();
            //    effectBitmap = null;

            //}

            if (effectBitmap == null)
            {
                Bitmap lightBitmap = (Bitmap)backBitmap.Clone();
                ColorDisplace.Displace(lightBitmap, selectHaloColor);

                GaussianBlur gs = new GaussianBlur(10);
                effectBitmap = gs.ProcessImage(lightBitmap);
                lightBitmap.Dispose();     
            }

            Bitmap bmp = (Bitmap)effectBitmap.Clone();
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(backBitmap, 0, 0);
            g.Dispose();
            return bmp;
        }

        private void ScVxButton_ReleasePostTreatmentEffectGDI(object sender, Bitmap effectBitmap)
        {
            effectBitmap.Dispose();
        }

        private void ZoomAnim_AnimationEvent(ScAnimation scAnimation)
        {
            animZoom = zoomLinear.GetCurtValue();

            ScaleX = animZoom;
            ScaleY = animZoom;

            if (zoomLinear.IsStop)
                scAnimation.Stop();

            Refresh();
        }

        private void ScAnim_AnimationEvent(ScAnimation scAnimation)
        {
            if (progressLinear == null)
                return;

            animProgress = progressLinear.GetCurtValue();

            if (progressLinear.IsStop)
            {
                scAnimation.Stop();
                progressLinear = null;
                StartProgressAnim();
            }

            Refresh();
        }

      
        public void StartProgressAnim()
        {
            if (animProgress == progress)
                return;

            progressAnim.Stop();
            progressLinear = new ScLinearAnimation(animProgress, progress, progressAnim);
            progressAnim.Start();
        }


        void StartZoomAnim()
        {
            if (animZoom == zoom)
                return;

            zoomAnim.Stop();
            zoomLinear = new ScLinearAnimation(animZoom, zoom, zoomAnim);
            zoomAnim.Start();
        }


        private void HaloAnim_AnimationEvent(ScAnimation scAnimation)
        {
            animHaloAlphaScale = haloLinear.GetCurtValue();


            if (haloLinear.IsStop)
            {
                scAnimation.Stop();

                if (endHaloAlphaScale == 1.0f)
                    endHaloAlphaScale = 0.3f;
                else
                    endHaloAlphaScale = 1.0f;

                StartHaloAnim();
            }
      
            Refresh();
        }

        public void StartHaloAnim()
        {
            haloAnim.Stop();
            haloLinear = new ScLinearAnimation(animHaloAlphaScale, endHaloAlphaScale, haloAnim);
            haloAnim.Start();
        }


        public void StopHaloAnim()
        {
            haloAnim.Stop();
        }

        public void SetZoomValue(float zoom)
        {
            maxZoom = zoom;
        }

        public void UpdateProgress(float newProgress)
        {
            progress = newProgress;
            StartProgressAnim();
        }

        private void BtnLayer_MouseDown(object sender, ScMouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                isSelected = true;
                StartHaloAnim();
                Refresh();
            }       
        }

        private void ScVxButton_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            if (haloBitmap != null && isSelected)
            {
                ColorDisplace.DisplaceAlpha(haloBitmap, animHaloAlphaScale, haloAlphaMask);
                graphis.DrawImage(haloBitmap, 0, 0);
            }
            else if(shadowBitmap != null)
            {
                graphis.DrawImage(shadowBitmap, 0, 0);
            }
        }

        private void BtnLayer_MouseLeave(object sender)
        {
            RectangleF oldrc = DrawBox;

            Anchor = new PointF(Width / 2, Height / 2);

            ScaleX = minZoom;
            ScaleY = minZoom;
            zoom = minZoom;

            RectangleF r = new RectangleF(btnLayer.Width - 30, btnLayer.Height - 30, 30, 30);
            gouPath = CreateGouPath(r);

            if (effectBitmap != null)
            {
                effectBitmap.Dispose();
                effectBitmap = null;
            }

             //     StartZoomAnim();
             InvalidateGlobalRect(GDIDataD2DUtils.UnionRectF(oldrc, DrawBox));
        }

        private void BtnLayer_MouseEnter(object sender, ScMouseEventArgs e)
        {
            RectangleF oldrc = DrawBox;

            Anchor = new PointF(Width / 2, Height / 2);

            ScaleX = maxZoom;
            ScaleY = maxZoom;

            RectangleF r = new RectangleF(btnLayer.Width - 30, btnLayer.Height - 30, 30, 30);
            gouPath = CreateGouPath(r);

            InvalidateGlobalRect(GDIDataD2DUtils.UnionRectF(oldrc, DrawBox));
        }

        public void SetVarValueText(int index, ScTxtInfo txtInfo)
        {
            txtInfo.AnimScaleEvent += TxtInfo_AnimScaleEvent;
            rowTextInfoList[index] = txtInfo;
        }

        private void TxtInfo_AnimScaleEvent()
        {
            Refresh();
        }

        public void SetVarValueText(int index, float maxValue, string fontTxt, string backTxt, Font font, Color color, RectangleF? incRect, float lineHeight)
        {
            ScTxtInfo txtInfo;

            if (rowTextInfoList[index] != null)
            {
                txtInfo = rowTextInfoList[index];
            }
            else
            {
                txtInfo = new ScTxtInfo(this);
            }

            txtInfo.maxVarValue = maxValue;
            txtInfo.fontTxt = fontTxt;
            txtInfo.backTxt = backTxt;
            txtInfo.txtColor = color;
            txtInfo.txtFont = font;
            txtInfo.lineHeight = lineHeight;
            txtInfo.incRect = incRect;
            rowTextInfoList[index] = txtInfo;
        }

        public void SetText(int index, string txt, Font font, Color color)
        {
            ScTxtInfo txtInfo;

            if (rowTextInfoList[index] != null)
            {
                txtInfo = rowTextInfoList[index];
            }
            else
            {
                txtInfo = new ScTxtInfo();
            }

            txtInfo.txt = txt;
            txtInfo.txtColor = color;
            txtInfo.txtFont = font;
            rowTextInfoList[index] = txtInfo;
        }

        public ScTxtInfo GetTxtInfo(int idx)
        {
            return rowTextInfoList[idx];
        }

        private void ImgLayer_GDIPaint(GDIGraphics g)
        {
            RectangleF rect = new RectangleF(0, 0, imgLayer.Width, imgLayer.Height);
            Graphics graph = g.GdiGraph;
            graph.SmoothingMode = SmoothingMode.HighQuality;
            graph.DrawImage(img, rect);
        }

        private void BtnLayer_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;
            graphis.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            RectangleF rect = new RectangleF(0, 0, btnLayer.Width , btnLayer.Height);
            Brush brush = new SolidBrush(bgColor);
            graphis.FillRectangle(brush, rect);
            brush.Dispose();

            if(!IsUsedProgressAnim)
            {
                Brush progressBrush = new SolidBrush(progressbarColor);
                float progressWidth = progress * 0.01f * btnLayer.Width;
                RectangleF progressRect = new RectangleF(0, 0, progressWidth, btnLayer.Height);
                graphis.FillRectangle(progressBrush, progressRect);
                progressBrush.Dispose();
            }
            else
            {
                Brush progressBrush;
                float progressWidth;
                RectangleF progressRect;

                if (animProgress < 100)
                {
                    progressBrush = new SolidBrush(Color.FromArgb(130, progressbarColor));
                    progressWidth = progress * 0.01f * btnLayer.Width;
                    progressRect = new RectangleF(0, 0, progressWidth, btnLayer.Height);
                    graphis.FillRectangle(progressBrush, progressRect);
                }

                progressBrush = new SolidBrush(Color.FromArgb(255, progressbarColor));
                progressWidth = animProgress * 0.01f * btnLayer.Width;
                progressRect = new RectangleF(0, 0, progressWidth, btnLayer.Height);
                graphis.FillRectangle(progressBrush, progressRect);
                progressBrush.Dispose();
            }

            //
            ScTxtInfo txtInfo = new ScTxtInfo();

            for (int i=0; i< TxtRowCount; i++)
            {
                rect = txtTable.GetCellContentRect(i, 0);


                if (rowTextInfoList[i] != null)
                {
                    if (rowTextInfoList[i].txt != null)
                        txtInfo.txt = rowTextInfoList[i].txt;
                    else
                        txtInfo.txt = "";

                    if (rowTextInfoList[i].txtFont != null)
                        txtInfo.txtFont = rowTextInfoList[i].txtFont;
                    else
                        txtInfo.txtFont = txtFont;

                    if (rowTextInfoList[i].txtColor != null)
                        txtInfo.txtColor = rowTextInfoList[i].txtColor;
                    else
                        txtInfo.txtColor = txtColor;
                }
                else
                {
                    continue;
                }

                brush = new SolidBrush(txtInfo.txtColor);
                DrawUtils.LimitBoxDraw(graphis, txtInfo.txt, txtInfo.txtFont, brush, rect, false, true, 0);
                brush.Dispose();
            }


            if (isShowGow)
            {
                //
                //  rect = new RectangleF(btnLayer.Width - 30, btnLayer.Height - 30, 30, 30);
                // GraphicsPath path = CreateGouPath(rect);
                graphis.FillPath(Brushes.Green, gouPath);
                //path.Dispose();
            }  


            if(isSelected)
            {
                rect = new RectangleF(0, 0, btnLayer.Width - 1, btnLayer.Height - 1);
                GraphicsPath path = DrawUtils.CreateRoundedRectanglePath(rect, 6);

                Pen pen = new Pen(Color.FromArgb(155, Color.Black));
                graphis.DrawPath(pen, path);
                graphis.DrawPath(pen, path);
            }
        }

        public void ReCreateTable()
        {
            float space = spaceLayer.Location.Y;
            RectangleF rect = new RectangleF(0, 5, spaceLayer.Width, spaceLayer.Height - 10);
            mainTable = new Table(rect, 1, 3);

            TableLine tableLine = new TableLine(LineDir.VERTICAL);
            tableLine.lineComputeMode = LineComputeMode.ABSOLUTE; 

            tableLine.computeParam = 5;
            mainTable.SetLineArea(0, tableLine);

            tableLine.computeParam = spaceLayer.Height + space - 15;
            mainTable.SetLineArea(1, tableLine);

            tableLine.computeParam = spaceLayer.Width - (spaceLayer.Height + space - 15) - 5;
            mainTable.SetLineArea(2, tableLine);
            mainTable.ComputeLinesArea(LineDir.VERTICAL);

            //
            txtTable = new Table(mainTable.GetCellContentRect(0, 2), TxtRowCount, 1);
        }


        private void ScVxButton_SizeChanged(object sender, SizeF oldSize)
        {
            spaceLayer.Location = new PointF(5, 10);
            spaceLayer.Width = Width - 10;
            spaceLayer.Height = Height - 20;

            float space = spaceLayer.Location.Y ;

            ReCreateTable();

            //
            RectangleF rect = mainTable.GetCellContentRect(0, 1);
            imgLayer.Location = new PointF(spaceLayer.Location.X + rect.X, spaceLayer.Location.Y - 13);
            imgLayer.Width = rect.Width;
            imgLayer.Height = spaceLayer.Height + space + 3;

            //
            btnLayer.Location = new PointF(0, 0);
            btnLayer.Width = mainTable.GetTableRect().Width;
            btnLayer.Height = spaceLayer.Height;

            RectangleF r = new RectangleF(btnLayer.Width - 30, btnLayer.Height - 30, 30, 30);
            gouPath = CreateGouPath(r);
            // Anchor = new PointF(Width / 2, Height / 2);
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

    }
}
