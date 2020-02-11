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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ScAnimation:IDisposable
    {
        ScLayer layer;
        private System.Timers.Timer refreshTimer = null;
        bool autoRest = false;
        int durationMS = 20;
        public int animMS = 100;
        int frameIndex = 0;
        bool isDisposed = false;

        public delegate void AnimationEventHandler(ScAnimation scAnimation);
        public event AnimationEventHandler AnimationEvent;

        delegate void UpdateCallback(object obj);
        UpdateCallback updateDet;

        public ScAnimation(ScLayer layer, bool autoRest)
        {
            this.layer = layer;
            this.autoRest = autoRest;
            updateDet = new UpdateCallback(Update);

            layer.AppendAnimation(this);
        }

        public ScAnimation(ScLayer layer, int animMS, bool autoRest)
        {
            this.layer = layer;
            this.autoRest = autoRest;
            this.animMS = animMS;   
            updateDet = new UpdateCallback(Update);

            layer.AppendAnimation(this);
        }

        public int AnimMS
        {
            get
            {
                return animMS;
            }
        }

        public int DurationMS
        {
            get
            {
                return durationMS;
            }
            set
            {
                durationMS = value;
            }
        }

        public int FrameIndex
        {
            get
            {
                return frameIndex;
            }
        }

        public void Start()
        {
            frameIndex = 0;
            StartTimer(durationMS);
        }

        public void Stop()
        {
            StopTimer();  
        }

        void StartTimer(int period)
        {
           if (isDisposed)
                return;

            refreshTimer = new System.Timers.Timer(period);   //实例化Timer类，设置间隔时间为period毫秒   
            refreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(theout); //到达时间的时候执行事件   
            refreshTimer.AutoReset = autoRest;   //设置是执行一次（false）还是一直执行(true)   
            refreshTimer.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件  
        }

        void StopTimer()
        {
            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
                refreshTimer = null;
            }
        }

        public void theout(object source, EventArgs e)
        {
            if (layer == null || layer.ScMgr == null)
                return;

            frameIndex++;
            layer.ScMgr.control.Invoke(updateDet, this);
        }

        void Update(object obj)
        {
            AnimationEvent?.Invoke(this);
        }

        public void Dispose()
        {
            Stop();
            layer = null;
            isDisposed = true;            
        }
    }
   
}
