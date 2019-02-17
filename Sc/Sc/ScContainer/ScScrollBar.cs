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
        public ScScrollContainer scrollContainer;
        ScScrollBarSlider slider;
        float downSliderPosY;
        float downSliderLocationY;
        float locationSliderY;

        public delegate void SliderMoveEventHandler(float yMoveValue);
        public event SliderMoveEventHandler SliderMoveEvent;


        public delegate void SliderDownEventHandler(float downSliderPosY);
        public event SliderDownEventHandler SliderDownEvent;

        public ScScrollBar()
        {
            Name = "scrollBar";

            slider = new ScScrollBarSlider();
            slider.Name = "slider";
            Add(slider);

            MouseMove += ScScrollBar_MouseMove;
            slider.MouseDown += Slider_MouseDown;

            SizeChanged += ScScrollBar_SizeChanged;
        }

       
        public ScScrollBar(float w, float h)
        {
            Width = w;
            Height = h;

            slider = new ScScrollBarSlider(w, h);
            slider.Name = "slider";
            Add(slider);

            MouseMove += ScScrollBar_MouseMove;
            slider.MouseDown += Slider_MouseDown;
        }

        public void SetScrollContainerMoveEvent()
        {
            scrollContainer.ContainerMoveEvent += ScrollContainer_ContainerMoveEvent;
            scrollContainer.ContainerDownEvent += ScrollContainer_ContainerDownEvent;
        }


        private void ScScrollBar_SizeChanged(object sender, SizeF oldSize)
        {
            slider.Width = Width;
        }



        private void ScScrollBar_MouseMove(object sender, ScMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float offsetY = e.Location.Y - downSliderPosY;
                float y = downSliderLocationY + offsetY;

                if (y < 0)
                {
                    y = 0;
                    offsetY = -downSliderLocationY;
                }
                else if (y + slider.Height > Height)
                {
                    y = Height - slider.Height;
                    offsetY = Height - (downSliderLocationY + slider.Height);
                }

                slider.Location = new PointF(slider.Location.X, y);
               
                SliderMoveEvent(offsetY);
                scrollContainer.Refresh();

            }
        }

        private void Slider_MouseDown(object sender, ScMouseEventArgs e)
        {
            downSliderPosY = e.Location.Y + slider.Location.Y;
            downSliderLocationY = slider.Location.Y; 

            SliderDownEvent(downSliderPosY);
        }

        private void ScrollContainer_ContainerDownEvent(float downContainerPosY)
        {
            locationSliderY = slider.Location.Y;
        }

        private void ScrollContainer_ContainerMoveEvent(float yMoveValue)
        {
            ScWrapper container = scrollContainer.GetContainer();

            float yoffset = Height / container.Height * yMoveValue;
            float y = locationSliderY - yoffset;

            if (y < 0)
                y = 0;
            else if (y + slider.Height > Height)
                y = Height - slider.Height;

            slider.Location = new PointF(slider.Location.X, y);
            Refresh();
        }


        public void SetSliderPositon(float sliderOffsetY, float sliderHeight)
        {
            slider.Location = new PointF(slider.Location.X, Location.Y + sliderOffsetY);
            slider.Height = sliderHeight;
        }
    }
}
