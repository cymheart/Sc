using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScTxtInfo
    {
        public Font txtFont { get; set; }
        public Color txtColor { get; set; }

        public string fontTxt { get; set; }
        public string backTxt { get; set; }
        public string txt { get; set; }
        public float lineHeight { get; set; }
        public RectangleF? incRect { get; set; }

        public float maxVarValue { get; set; }

        public string titleTxt;

        public DrawPositionType drawPosType;
        public bool isLimitBox;
        public bool isMultipleRow;

        public int type;

        public object tag;
        int durationMS = 30;

        public float animValue;
        public ScLinearAnimation linear = null;
        public ScAnimation scaleAnim;
        ScLayer sclayer;

        public delegate void AnimScaleEventHandler();
        public event AnimScaleEventHandler AnimScaleEvent;
        int step;

        public delegate void AnimValueSetEventHandler(ScTxtInfo txtInfo);
        public event AnimValueSetEventHandler AnimValueSetEvent = null;


        public int DurationMS
        {
            get { return durationMS; }
            set
            {
                durationMS = value;
                scaleAnim.DurationMS = durationMS;
            }
        }


        public ScTxtInfo()
        {
            type = 0;
        }

        public ScTxtInfo(ScLayer sclayer)
        {
            type = 1;
            this.sclayer = sclayer;

            scaleAnim = new ScAnimation(sclayer, -1, true);
            scaleAnim.DurationMS = 30;
            scaleAnim.AnimationEvent += ScaleAnim_AnimationEvent;
        }

      
        ~ScTxtInfo()
        {     
            StopScaleAnim();
        }

        public void StartScaleAnim()
        {
            if (scaleAnim == null)
                return;

            scaleAnim.Stop();  
            
            step = (int)(maxVarValue - animValue) / 10;
            if (step == 0)
                step = 1;

            scaleAnim.Start();
        }

        public void StopScaleAnim()
        {
            if (scaleAnim == null)
                return;

            scaleAnim.Stop();
            linear = null;
        }

        private void ScaleAnim_AnimationEvent(ScAnimation scAnimation)
        {
            if (AnimValueSetEvent != null)
            {
                AnimValueSetEvent(this);
            }
            else
            {
                animValue += step;

                if (step >0 && animValue >= maxVarValue ||
                    step <0 && animValue <= maxVarValue)
                {
                    animValue = maxVarValue;
                    StopScaleAnim();
                }

                txt = fontTxt + animValue + backTxt;
            }

            if (AnimScaleEvent != null)
                AnimScaleEvent();
        }
    }
}
