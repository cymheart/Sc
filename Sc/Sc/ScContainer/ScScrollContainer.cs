using MouseKeyboardLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ScScrollContainer:ScLayer
    {
        public ScLayer view;
        public ScWrapper wrapper;
        ScScrollBar hScrollBar;

        float locationWrapperY;
        float downWrapperY;
        float downWrapperLocationY;

        bool isLayoutViewLayer = false;

        public delegate void ContainerMoveEventHandler(float yMoveValue);
        public event ContainerMoveEventHandler ContainerMoveEvent;

        public delegate void ContainerDownEventHandler(float downContainerPosY);
        public event ContainerDownEventHandler ContainerDownEvent;

        MouseHook mouseHook = new MouseHook();

        public ScScrollContainer()
        {
            view = new ScLayer();
            Add(view);

            wrapper = new ScWrapper();
            view.Add(wrapper);

            hScrollBar = new ScScrollBar();
            hScrollBar.Visible = false;
            hScrollBar.IsHitThrough = false;
            hScrollBar.scrollContainer = this;
            hScrollBar.SetScrollContainerMoveEvent();
            Add(hScrollBar);

            hScrollBar.SliderMoveEvent += HScrollBar_SliderMoveEvent;
            hScrollBar.SliderDownEvent += HScrollBar_SliderDownEvent;

            MouseDown += ScScrollContainer_MouseDown;
            MouseMove += ScScrollContainer_MouseMove;
            MouseUp += ScScrollContainer_MouseUp;

            mouseHook.MouseWheel += MouseHook_MouseWheel;
            // mouseHook.Start();

            SizeChanged += ScScrollContainer_SizeChanged;

        }

        public ScScrollContainer(int w, int h)
        {
            Width = w;
            Height = h;

            wrapper = new ScWrapper(w, h);
            wrapper.Location = new PointF(0, 0);
            Add(wrapper);

            hScrollBar = new ScScrollBar(8, h);
            hScrollBar.Location = new Point(w - 8, 0);
            hScrollBar.Visible = false;
            hScrollBar.IsHitThrough = false;
            hScrollBar.scrollContainer = this;
            hScrollBar.SetScrollContainerMoveEvent();
            Add(hScrollBar);

            hScrollBar.SliderMoveEvent += HScrollBar_SliderMoveEvent;
            hScrollBar.SliderDownEvent += HScrollBar_SliderDownEvent;

            MouseDown += ScScrollContainer_MouseDown;
            MouseMove += ScScrollContainer_MouseMove;
            MouseUp += ScScrollContainer_MouseUp;

            mouseHook.MouseWheel += MouseHook_MouseWheel;
            // mouseHook.Start();

        }

        private void ScScrollContainer_SizeChanged(object sender, SizeF oldSize)
        {
            //
            hScrollBar.Width = 8;
            hScrollBar.Height = Height;
            hScrollBar.Location = new PointF(Width - 8, 0);

            //
            if (isLayoutViewLayer)
            {
                view.Width = Width - hScrollBar.Width;
                view.Height = Height;
                view.Location = new PointF(0, 0);
            }
        }

        ~ScScrollContainer()
        {
            mouseHook.Stop();
        }

        public void StopMouseHook()
        {
            mouseHook.Stop();
        }

        private void MouseHook_MouseWheel(object sender, MouseEventArgs e)
        {
            RectangleF rc = new RectangleF(0, 0, Width, Height);       
            Point pt = ScMgr.control.PointToClient(Control.MousePosition);
            PointF ptf  = this.TransGlobalToLocal(pt);


            if (rc.Contains(ptf))
            {
                if (e.Delta > 0)
                {
                    downWrapperLocationY = wrapper.Location.Y;

                    if (ContainerDownEvent != null)
                        ContainerDownEvent(0);

                    ScrollMouseMove(15);
                }
                else
                {
                    downWrapperLocationY = wrapper.Location.Y;

                    if (ContainerDownEvent != null)
                        ContainerDownEvent(0);

                    ScrollMouseMove(-15);
                }

                Refresh();
            }
        }

        private void ScScrollContainer_MouseUp(object sender, ScMouseEventArgs e)
        {
           // SetCursor(Cursors.Hand);
        }

        private void ScScrollContainer_MouseMove(object sender, ScMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float offsetY = e.Location.Y - downWrapperY;

                ScrollMouseMove(offsetY);
                Refresh();
            }
     
        }


        void ScrollMouseMove(float offsetY)
        {
            float y = downWrapperLocationY + offsetY;

            if (y > 0)
            {
                y = 0;
                offsetY = -downWrapperLocationY;
            }
            else if (y + wrapper.Height < view.Height)
            {
                y = view.Height - wrapper.Height;
                offsetY = view.Height - (downWrapperLocationY + wrapper.Height);
            }

            wrapper.Location = new PointF(wrapper.Location.X, y);

            if (ContainerMoveEvent != null)
                ContainerMoveEvent(offsetY);
        }

        private void ScScrollContainer_MouseDown(object sender, ScMouseEventArgs e)
        {
            downWrapperY = e.Location.Y;
            downWrapperLocationY = wrapper.Location.Y;

            if(ContainerDownEvent != null)
                ContainerDownEvent(downWrapperY);
        }


        private void HScrollBar_SliderDownEvent(float downSliderPosY)
        {
            locationWrapperY = wrapper.Location.Y;
        }

        private void HScrollBar_SliderMoveEvent(float yMoveValue)
        {
            float yoffset = wrapper.Height / hScrollBar.Height * yMoveValue;
            float y = locationWrapperY - yoffset;

            if (y > 0)
                y = 0;
            else if (y + wrapper.Height < Height)
                y = Height - wrapper.Height;

            wrapper.Location = new PointF(wrapper.Location.X, (float)Math.Round(y));
           // Refresh();
        }

        public ScWrapper GetContainer()
        {
            return wrapper;
        }

        public void AddContentControl(ScLayer control)
        {
            wrapper.Add(control);
            wrapper.FixSize();

            if(wrapper.Height > view.Height)
            {
                hScrollBar.Visible = true;
                FixSliderSize();
            }
            else
            {
                hScrollBar.Visible = false;
                FixSliderSize();
            }
        }

        public void RemoveContentControl(ScLayer control)
        {
            wrapper.Remove(control);
            wrapper.FixSize();

            if (wrapper.Height > view.Height)
            {
                hScrollBar.Visible = true;
                FixSliderSize();
            }
            else
            {
                hScrollBar.Visible = false;
                FixSliderSize();
            }
        }

        void FixSliderSize()
        {
            float sliderHeight = (float)view.Height / wrapper.Height * hScrollBar.Height;

            float y = Math.Abs(wrapper.Location.Y);
            float sliderPosY = y / wrapper.Height * hScrollBar.Height;

            hScrollBar.SetSliderPositon((int)sliderPosY, (int)sliderHeight);     
        }

    }
}
