using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class ScProgressNodeAnim
    {
        public float animScaleValue;
        public ScLinearAnimation linear = null;
        public ScAnimation scaleAnim;
        RectangleF rect;
        ScLayer sclayer;

        public delegate void AnimScaleEventHandler();
        public event AnimScaleEventHandler AnimScaleEvent;

        public ScProgressNodeAnim(ScLayer sclayer, RectangleF rect)
        {
            this.sclayer = sclayer;
            this.rect = rect;

            scaleAnim = new ScAnimation(sclayer, 50, true);
            scaleAnim.AnimationEvent += ScaleAnim_AnimationEvent;
        }

        ~ScProgressNodeAnim()
        {
            scaleAnim.Stop();
        }

        private void ScaleAnim_AnimationEvent(ScAnimation scAnimation)
        {
            animScaleValue = linear.GetCurtValue();

            if (animScaleValue >= 1.0f)
                StopScaleAnim();

            if (AnimScaleEvent != null)
                AnimScaleEvent();


        }

        public void StartScaleAnim()
        {
            scaleAnim.Stop();
            linear = new ScLinearAnimation(0, 1.0f, scaleAnim);
            scaleAnim.Start();
        }


        public void StopScaleAnim()
        {
            scaleAnim.Stop();
            linear = null;
        }
    }
}
