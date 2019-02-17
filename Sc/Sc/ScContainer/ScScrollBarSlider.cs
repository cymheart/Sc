using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class ScScrollBarSlider: ScLayer
    {
        int alpha = 80;
        int leaveAlpha = 80;
        int enterAlpha = 170;
        int downAlpha = 255;

        ScAnimation scAnim;
        ScLinearAnimation linear;

        public ScScrollBarSlider()
        {
            D2DPaint += ScScrollBarSlider_D2DPaint;
            GDIPaint += ScScrollBarSlider_GDIPaint;

            MouseEnter += ScScrollBarSlider_MouseEnter;
            MouseDown += ScScrollBarSlider_MouseDown;
            MouseLeave += ScScrollBarSlider_MouseLeave;
            MouseUp += ScScrollBarSlider_MouseUp;

            scAnim = new ScAnimation(this, 100, true);
            scAnim.AnimationEvent += ScAnim_AnimationEvent;
        }

        public ScScrollBarSlider(float w, float h)
        {
            Width = w;
            Height = h;

            D2DPaint += ScScrollBarSlider_D2DPaint;
            GDIPaint += ScScrollBarSlider_GDIPaint;

            MouseEnter += ScScrollBarSlider_MouseEnter;
            MouseDown += ScScrollBarSlider_MouseDown;
            MouseLeave += ScScrollBarSlider_MouseLeave;
            MouseUp += ScScrollBarSlider_MouseUp;

            scAnim = new ScAnimation(this, 100, true);
            scAnim.AnimationEvent += ScAnim_AnimationEvent;
        }


        ~ScScrollBarSlider()
        {
            scAnim.Stop();
        }

        private void ScScrollBarSlider_MouseUp(object sender, ScMouseEventArgs e)
        {
            StartAnim(enterAlpha);
        }

        private void ScScrollBarSlider_MouseLeave(object sender)
        {
            StartAnim(leaveAlpha);
        }

        private void ScScrollBarSlider_MouseDown(object sender, ScMouseEventArgs e)
        {
            scAnim.Stop();
            alpha = downAlpha;
            Refresh();
        }

        private void ScScrollBarSlider_MouseEnter(object sender, ScMouseEventArgs e)
        {
            StartAnim(enterAlpha);
        }

        private void ScScrollBarSlider_D2DPaint(D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;

            RawRectangleF rect = new RawRectangleF(2, 0, Width - 2, Height);

            RawColor4 color = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(alpha, 191, 152, 90));
            SharpDX.Direct2D1.Brush brush = new SolidColorBrush(g.RenderTarget, color);

            g.RenderTarget.FillRectangle(rect, brush);

        }

        private void ScScrollBarSlider_GDIPaint(GDIGraphics g)
        {
            g.GdiGraph.SmoothingMode = SmoothingMode.None;// 指定高质量、低速度呈现。

            // g.graphics.SmoothingMode = SmoothingMode.HighQuality;
            Color color = Color.FromArgb(alpha, 191, 152, 90);
            RectangleF rect = new RectangleF(2, 0, Width - 4 - 1, Height - 1);
            System.Drawing.Brush brush = new SolidBrush(color);
            g.GdiGraph.FillRectangle(brush, rect);

        }

        public void StartAnim(int stopAlpha)
        {
            scAnim.Stop();
            linear = new ScLinearAnimation(alpha, stopAlpha, scAnim);
            scAnim.Start();
        }

        private void ScAnim_AnimationEvent(ScAnimation scAnimation)
        {
            alpha = (int)linear.GetCurtValue();

            if (linear.IsStop)
                scAnimation.Stop();

            Refresh();
        }
    }
}
