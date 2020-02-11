using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ScScrollBar:ScLayer
    {
        ScScrollBarSlider slider;
        float downSliderPos;
        float downSliderLocation;
        float contentSize = 0;
        float viewportSize = 0;

        float minSliderSize = 10;
        float ratio = 1;

        bool isContentSizeOver = false;

        ScScrollOrientation scrollOrientation;

        public delegate void SliderMoveEventHandler(float yMoveValue);
        public event SliderMoveEventHandler SliderMoveEvent;


        public delegate void SliderDownEventHandler(float downSliderPosY);
        public event SliderDownEventHandler SliderDownEvent;



        public int SliderAlpha
        {
            get { return slider.alpha; }
            set
            {
                slider.alpha = value;
            }
        }


        public int SliderEnterAlpha
        {
            get { return slider.enterAlpha; }
            set
            {
                slider.enterAlpha = value;
            }
        }

        public int SliderDownAlpha
        {
            get { return slider.downAlpha; }
            set
            {
                slider.downAlpha = value;
            }
        }

        public Color SliderColor
        {
            get { return slider.SliderColor; }
            set
            {
                slider.SliderColor = value;
            }
        }


        public ScScrollBar(ScMgr scmgr = null)
            :base(scmgr)
        {
            Name = "scrollBar";

            slider = new ScScrollBarSlider(scmgr);
            slider.Name = "slider";
            Add(slider);

            MouseMove += ScScrollBar_MouseMove;
            slider.MouseDown += Slider_MouseDown;

            SizeChanged += ScScrollBar_SizeChanged;


            IsHitThrough = false;
        }

        public ScScrollOrientation ScrollOrientation
        {
            get
            {
                return scrollOrientation;
            }
            set
            {
                scrollOrientation = value;
                slider.ScrollOrientation = value;
            }            
        }


        private void ScScrollBar_SizeChanged(object sender, SizeF oldSize)
        {
            switch(ScrollOrientation)
            {
                case ScScrollOrientation.HORIZONTAL_SCROLL:
                    slider.Height = Height;
                    break;

                case ScScrollOrientation.VERTICAL_SCROLL:
                    slider.Width = Width;
                    break;
            }
        }



        private void ScScrollBar_MouseMove(object sender, ScMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                switch (ScrollOrientation)
                {
                    case ScScrollOrientation.HORIZONTAL_SCROLL:
                        HorScroll(e.Location.X);
                        break;

                    case ScScrollOrientation.VERTICAL_SCROLL:
                        VerScroll(e.Location.Y);
                        break;
                }
            }
        }

      
        void VerScroll(float curtMouseYPos)
        {
            float offsetY = curtMouseYPos - downSliderPos;
            float y = downSliderLocation + offsetY;

            if (y < 0)
            {
                y = 0;
                offsetY = -downSliderLocation;
            }
            else if (y + slider.Height > Height)
            {
                y = Height - slider.Height;
                offsetY = Height - (downSliderLocation + slider.Height);
            }

            slider.Location = new PointF(slider.Location.X, y);

            if (SliderMoveEvent != null)
            {
                float viewportPos;

                if (!isContentSizeOver)
                    viewportPos = slider.Location.Y / Height * contentSize;
                else
                    viewportPos = slider.Location.Y * ratio;

                SliderMoveEvent(viewportPos);
            }

            Refresh();
        }

        void HorScroll(float curtMouseXPos)
        {
            float offsetX = curtMouseXPos - downSliderPos;
            float x = downSliderLocation + offsetX;

            if (x < 0)
            {
                x = 0;
                offsetX = -downSliderLocation;
            }
            else if (x + slider.Width > Width)
            {
                x = Width - slider.Width;
                offsetX = Width - (downSliderLocation + slider.Width);
            }

            slider.Location = new PointF(x, slider.Location.Y);

            if (SliderMoveEvent != null)
            {

                float viewportPos;

                if (!isContentSizeOver)
                    viewportPos = slider.Location.X / Width * contentSize;
                else
                    viewportPos = slider.Location.X * ratio;

                SliderMoveEvent(viewportPos);
            }

            Refresh();
        }


        private void Slider_MouseDown(object sender, ScMouseEventArgs e)
        {
            switch(ScrollOrientation)
            {
                case ScScrollOrientation.VERTICAL_SCROLL:
                    downSliderPos = e.Location.Y + slider.Location.Y;
                    downSliderLocation = slider.Location.Y;

                    if(SliderDownEvent != null)
                        SliderDownEvent(downSliderPos);
                    break;

                case ScScrollOrientation.HORIZONTAL_SCROLL:
                    downSliderPos = e.Location.X + slider.Location.X;
                    downSliderLocation = slider.Location.X;

                    if (SliderDownEvent != null)
                        SliderDownEvent(downSliderPos);
                    break;

            }    
        }

        public void SetSliderRatio(float contentSize, float viewportSize)
        {
            if (contentSize == 0)
                return;

            this.contentSize = contentSize;
            this.viewportSize = viewportSize;

            isContentSizeOver = false;

            switch (ScrollOrientation)
            {
                case ScScrollOrientation.VERTICAL_SCROLL:
                    slider.Height = Height / contentSize * viewportSize;

                    if(slider.Height < minSliderSize)
                    {
                        slider.Height = minSliderSize;
                        ratio = (contentSize - viewportSize) / (Height - slider.Height);
                        isContentSizeOver = true;
                    }


                    break;

                case ScScrollOrientation.HORIZONTAL_SCROLL:
                    slider.Width = Width / contentSize * viewportSize;

                    if (slider.Width < minSliderSize)
                    {
                        slider.Width = minSliderSize;
                        ratio = (contentSize - viewportSize) / (Width - slider.Width);
                        isContentSizeOver = true;
                    }

                    break;
            }
        }

        public void SetSliderLocation(float viewportPos)
        {

        }
        public void SetSliderLocationByViewport(float viewportPos)
        {
            switch (ScrollOrientation)
            {
                case ScScrollOrientation.VERTICAL_SCROLL:

                    if(isContentSizeOver == false)
                        slider.Location = new PointF(slider.Location.X, viewportPos / contentSize * Height);
                    else
                        slider.Location = new PointF(slider.Location.X, viewportPos / ratio);


                    break;

                case ScScrollOrientation.HORIZONTAL_SCROLL:
                    if (isContentSizeOver == false)
                        slider.Location = new PointF(viewportPos / contentSize * Width, slider.Location.Y);
                    else
                        slider.Location = new PointF(viewportPos / ratio, slider.Location.Y);
                    break;

            }
        }


    }
}
