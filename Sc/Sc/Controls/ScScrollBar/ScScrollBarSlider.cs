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
        

        ScAnimation scAnim;
        ScLinearAnimation linear;

        public int alpha = 80;
        public int leaveAlpha = 80;
        public int enterAlpha = 170;
        public int downAlpha = 255;
        public Color SliderColor = Color.FromArgb(255, 191, 152, 90);

        public ScScrollOrientation ScrollOrientation { get; set; }

        public ScScrollBarSlider(ScMgr scmgr)
            :base(scmgr)
        {
            D2DPaint += ScScrollBarSlider_D2DPaint;

            MouseEnter += ScScrollBarSlider_MouseEnter;
            MouseDown += ScScrollBarSlider_MouseDown;
            MouseLeave += ScScrollBarSlider_MouseLeave;
            MouseUp += ScScrollBarSlider_MouseUp;

            scAnim = new ScAnimation(this, 100, true);
            scAnim.AnimationEvent += ScAnim_AnimationEvent;
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
            RawRectangleF rect = new RawRectangleF();

            switch (ScrollOrientation)
            {
                case ScScrollOrientation.HORIZONTAL_SCROLL:
                    rect = new RawRectangleF(0, 2, Width , Height - 2);
                    break;

                case ScScrollOrientation.VERTICAL_SCROLL:
                    rect = new RawRectangleF(2, 0, Width - 2, Height);
                    break;
            }

            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
            RawColor4 color = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(alpha, SliderColor.R, SliderColor.G, SliderColor.B));
            SharpDX.Direct2D1.Brush brush = new SolidColorBrush(g.RenderTarget, color);
            g.RenderTarget.FillRectangle(rect, brush);
            brush.Dispose();

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
