using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ScVxPageSelector:ScLayer
    {
        ScVxPageTips tips = null;

        RectangleF wrapper;

        float downWrapperX;
        float downWrapperLocationX;

        public int pageAmount = 15;
        public float itemWidth = 0;
        public float itemSpace = 0;
        public int showPageAmount = 10;
       
        public int selectedPageIdx = 0;
        public float leftPageRatio = 0.1f;
        public float rightPageRatio = -0.9f;

        public delegate void ContainerMoveEventHandler(float xMoveValue);
        public event ContainerMoveEventHandler ContainerMoveEvent;

        public delegate void ContainerDownEventHandler(float downContainerPosX);
        public event ContainerDownEventHandler ContainerDownEvent;

        public delegate void  MouseDownPageEventHandler(int mouseDownPageIdx);
        public event MouseDownPageEventHandler MouseDownPageEvent;


        public delegate void PageMoveEventHandler();
        public event PageMoveEventHandler PageMoveEvent;

        public ScVxPageSelector()
        {
            SizeChanged += ScVxPageSelector_SizeChanged;
    
            MouseDown += ScScrollContainer_MouseDown;
            MouseMove += ScScrollContainer_MouseMove;

            GDIPaint += ScVxPageSelector_GDIPaint;
        }

      
        public void SetTips(ScVxPageTips tips)
        {
            this.tips = tips;
        }

        public void ReLayout()
        {
            CreatePageBtn();
        }

        private void ScVxPageSelector_GDIPaint(GDIGraphics g)
        {
            int idx =(int)(-wrapper.X / Width);
        }

        public float GetFixWidth()
        {
            int m = showPageAmount;

            if (pageAmount < showPageAmount)
                m = pageAmount;

            float w = (itemWidth + itemSpace) * (m - 1) + itemWidth;
            return w;         
        }

        private void ScVxPageSelector_SizeChanged(object sender, SizeF oldSize)
        {
            wrapper = new RectangleF();
            wrapper.X = 0;
            wrapper.Y = 0;
            wrapper.Width = (pageAmount - 1) * (itemWidth + itemSpace) + itemWidth;
            wrapper.Height = Height;

            CreatePageBtn();
        }

        private void ScScrollContainer_MouseMove(object sender, ScMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float offsetX = e.Location.X - downWrapperX;
                offsetX = (int)(offsetX / (itemWidth + itemSpace)) * (itemWidth + itemSpace);

                float x = downWrapperLocationX + offsetX;

                if (x > 0)
                {
                    x = 0;
                    offsetX = -downWrapperLocationX;
                }
                else if (x + wrapper.Width < Width)
                {
                    x = Width - wrapper.Width;
                    offsetX = Width - (downWrapperLocationX + wrapper.Width);
                }

                wrapper.Location = new PointF(x, wrapper.Location.Y);

                if(ContainerMoveEvent != null)
                    ContainerMoveEvent(offsetX);

                CreatePageBtn();

                if (PageMoveEvent != null)
                    PageMoveEvent();

                Refresh();
            }
        }


        void CreatePageBtn()
        {
            int leftPageAmount = (int)Math.Round(-wrapper.Location.X / (itemWidth + itemSpace));
            int realShowPageAmount = Math.Min(pageAmount - leftPageAmount, showPageAmount);

            Clear();
            ScVxPageBtn btn;
            float xpos;
            int curtShowPageIdx;

            for (int i = 0; i < realShowPageAmount; i++)
            {
                xpos = i * (itemWidth + itemSpace);

                btn = new ScVxPageBtn();
                btn.Width = itemWidth;
                btn.Height = Height;
                btn.Location = new PointF((int)xpos, 0);
                btn.frameWidth = 2f;

                curtShowPageIdx = leftPageAmount + i;

                if (selectedPageIdx == curtShowPageIdx)
                {
                    btn.ratio = leftPageRatio;
                }
                else if (selectedPageIdx == curtShowPageIdx - 1)
                {
                    btn.ratio = rightPageRatio;
                }
                else
                    btn.ratio = 0f;

                btn.MouseDown += Btn_MouseDown;
                btn.MouseEnter += Btn_MouseEnter;
                btn.MouseLeave += Btn_MouseLeave;

                Add(btn);
            }
        }

        public int GetCurtLeftPageNum()
        {
            int leftPageAmount = (int)Math.Round(-wrapper.Location.X / (itemWidth + itemSpace));
            return leftPageAmount;
        }

        public int GetCurtRightPageNum()
        {
            int n = GetCurtLeftPageNum();
            int rightPageAmount = n + controls.Count();
            return rightPageAmount;
        }

        private void Btn_MouseLeave(object sender)
        {
            if (tips == null)
                return;

            tips.Visible = false;
            tips.Refresh();
        }

        private void Btn_MouseEnter(object sender, ScMouseEventArgs e)
        {
            if (tips == null)
                return;

            int i = 0;
            foreach (ScLayer layer in controls)
            {
                i++;
                if (layer == sender)
                    break;
            }

            ScLayer layerx = (ScLayer)sender;
            RectangleF rect = layerx.DrawBox;

            float x = rect.X + rect.Width / 2 - tips.Width / 2;
            float y = rect.Y - tips.Height;
            tips.Location = new PointF(x, y);


            int leftPageAmount = (int)Math.Round(-wrapper.Location.X / (itemWidth + itemSpace));
            int downPageIdx = leftPageAmount + i;

            tips.txt = downPageIdx.ToString();

            tips.Visible = true;
            tips.Refresh();
            
        }

        private void Btn_MouseDown(object sender, ScMouseEventArgs e)
        {
            int i = 0;
            foreach(ScLayer layer in controls)
            {
                i++;
                if (layer == sender)
                    break;
            }

            int leftPageAmount = (int)Math.Round(-wrapper.Location.X / (itemWidth + itemSpace));
            int downPageIdx = leftPageAmount + i - 1;

            if (MouseDownPageEvent != null)
                MouseDownPageEvent(downPageIdx);
        }




        private void ScScrollContainer_MouseDown(object sender, ScMouseEventArgs e)
        {
            downWrapperX = e.Location.X;
            downWrapperLocationX = wrapper.Location.X;

            if (ContainerDownEvent != null)
                ContainerDownEvent(downWrapperX);
        }
    }
}
