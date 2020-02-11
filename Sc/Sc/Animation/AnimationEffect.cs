//----------------------------------------------------------------------------
// Simple Control (Sc) - Version 1.1
// A high quality control rendering engine for C#
// Copyright (C) 2016-2020 cymheart
// Contact: 
//
// 
// Sc is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// Sc is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Sc; if not, write to the Free Software
//----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class AnimationEffect
    {
        protected bool isStop = false;
        public virtual float GetCurtValue()
        {
            return 0;
        }

        public bool IsStop
        {
            get
            {
                return isStop;
            }
            set
            {
                isStop = value;
            }
        }
    }

    public class ScLinearAnimation:AnimationEffect
    {
        float n = 0;
        ScAnimation scAnim = null;
        float startValue;
        float stopValue;

        public ScLinearAnimation(float startValue, float stopValue, ScAnimation scAnimation)
        {
            scAnim = scAnimation;
            this.startValue = startValue;
            this.stopValue = stopValue;
            n = (stopValue - startValue) / scAnim.AnimMS * scAnim.DurationMS;
        }

        public override float GetCurtValue()
        {
            float val = startValue + scAnim.FrameIndex * n;

            if ((n >= 0 && val >= stopValue) || (n <= 0 && val <= stopValue))
            {
                val = stopValue;
                isStop = true;
            }

            return val;
        }
    }

    public class ScStepAnimation : AnimationEffect
    {
        float n = 0;
        ScAnimation scAnim = null;
        float startValue;

        public ScStepAnimation(float startValue, float step, ScAnimation scAnimation)
        {
            scAnim = scAnimation;
            this.startValue = startValue;
            n = step;
        }

        public override float GetCurtValue()
        {
            float val = startValue + scAnim.FrameIndex * n;
            return val;
        }
    }
}
