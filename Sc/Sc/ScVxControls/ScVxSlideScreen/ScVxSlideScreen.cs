using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ScVxSlideScreen:ScLayer
    {
        public List<ScLayer> itemList = new List<ScLayer>();

        public int singleScreenRowItemAmount = 6;
        public int singleScreenColItemAmount = 6;
        int singleScreenItemTotalAmount = 36;
        int screenAmount;
        int curtPageIdx = 0;
        int curtItemIdx = 0;

        public float itemFixWidth = 0;
        public float itemFixHeight = 0;
        public bool useFixSizeItem = false;

        ScVxViewScreen leftScreen;
        ScVxViewScreen rightScreen;

        RectangleF slideRect;
        PointF mouseDownPos = new PointF();
        PointF mouseDownSlideRectPos;

        ScAnimation slideScreenAnim;
        ScLinearAnimation slideAnimLinear;
        float slideLen;
        bool isSlidering = false;
        int slideDirection = -1;

      
        List<ScLayer> leftScreenItems;
        List<ScLayer> rightScreenItems;

        ScVxPageSelectorEx pageSelector;

        public ScVxSlideScreen()
        {
            leftScreen = new ScVxViewScreen();
            rightScreen = new ScVxViewScreen();

            leftScreen.RowLayerBackAnimEvent += LeftScreen_RowLayerBackAnimEvent;
            rightScreen.RowLayerBackAnimEvent += RightScreen_RowLayerBackAnimEvent;

            Add(leftScreen);
            Add(rightScreen);

            pageSelector = new ScVxPageSelectorEx();
            pageSelector.IsHitThrough = false;
            pageSelector.ps.MouseDownPageEvent += PageSelector_MouseDownPageEvent;

            Add(pageSelector);

            SizeChanged += ScVxSlideScreen_SizeChanged;
            MouseDown += ScVxSlideScreen_MouseDown;

            MouseMove += ScVxSlideScreen_MouseMove;
            MouseUp += ScVxSlideScreen_MouseUp;

            slideScreenAnim = new ScAnimation(this, 400, true);
            slideScreenAnim.AnimationEvent += SlideScreenAnim_AnimationEvent;    
                  
        }

        ~ScVxSlideScreen()
        {
            slideScreenAnim.Stop();
        }

        public Size GetItemSize()
        {
            return leftScreen.GetItemSize();
        }

        public void StopAllTimer()
        {
            slideScreenAnim.Stop();

            foreach (ScLayer sclayer in leftScreen.controls)
                foreach (ScVxButton item in sclayer.controls)
                {
                    item.StopAllAnim();
                }

            foreach (ScLayer sclayer in rightScreen.controls)
                foreach (ScVxButton item in sclayer.controls)
                {
                    item.StopAllAnim();
                }
        }


        private void PageSelector_MouseDownPageEvent(int mouseDownPageIdx)
        {
            ShowPage(mouseDownPageIdx);
        }

        public void ShowPage(int pageIdx)
        {
            float x = -pageIdx * Width;

            foreach (ScLayer sclayer in leftScreen.controls)
                foreach (ScVxButton item in sclayer.controls)
                {
                    item.animProgress = 0;
                    item.StopAllAnim();
                }

            foreach (ScLayer sclayer in rightScreen.controls)
                foreach (ScVxButton item in sclayer.controls)
                {
                    item.animProgress = 0;
                    item.StopAllAnim();
                }


            SetSlidePos(x);

            LeftScreen_RowLayerBackAnimEvent();
            RightScreen_RowLayerBackAnimEvent();

            Refresh();
        }

        private void RightScreen_RowLayerBackAnimEvent()
        {
            foreach(ScLayer sclayer in rightScreen.controls)
                foreach (ScVxButton item in sclayer.controls)
                    item.StartAllAnim();
            isSlidering = false;
        }

        private void LeftScreen_RowLayerBackAnimEvent()
        {

            foreach (ScLayer sclayer in leftScreen.controls)
                foreach (ScVxButton item in sclayer.controls)
                    item.StartAllAnim();     
            isSlidering = false;
        }


        private void SlideScreenAnim_AnimationEvent(ScAnimation scAnimation)
        {
            float pos = slideAnimLinear.GetCurtValue();

            if (slideAnimLinear.IsStop)
            {
                scAnimation.Stop();

                if (slideDirection == -1)
                {
                    rightScreen.StartRowLayerBackAnim();
                }
                else
                {
                    leftScreen.StartRowLayerBackAnim();
                }                                     
            }

            SetSlidePos(pos);   
            Refresh();
        }


        void StartSlideScreenAnim(float endValue)
        {
            slideScreenAnim.Stop();
            slideAnimLinear = new ScLinearAnimation(slideRect.X, endValue, slideScreenAnim);
            slideScreenAnim.Start();

            if (slideDirection == 1)
                leftScreen.StartRowLayerAnim();
            else
                rightScreen.StartRowLayerAnim();
        }


        public void SetPageSelectorTips(ScVxPageTips tips)
        {
            pageSelector.SetTips(tips);
        }

        private void ScVxSlideScreen_GDIPaint(GDIGraphics g)
        {
            Graphics gdiGraph = g.GdiGraph;
  
            RectangleF rect = new RectangleF(0, 0, Width, Height);
            System.Drawing.Brush brush = new SolidBrush(Color.DarkGray);
            gdiGraph.FillRectangle(brush, rect);
            brush.Dispose();
        }


        private void ScVxSlideScreen_MouseUp(object sender, ScMouseEventArgs e)
        {
            if (slideLen == 0 || isSlidering == false)
            {
                isSlidering = false;
                return;
            }

            float len = Math.Abs(slideRect.X);
            int n = (int)len / (int)Width;
            float xpos = slideRect.X + n * (int)Width;

            float w = 0;

            if (slideLen > 0)  //往右滑动
            {
                if (slideRect.X == 0)
                {
                    isSlidering = false;
                    return;
                }

                if (xpos + Width < Width)
                    w = xpos;

                slideDirection = 1;
            }
            else   //往左滑动
            {
                if (slideRect.X == Width - slideRect.Width)
                {
                    isSlidering = false;
                    return;
                }

                if (xpos < 0)
                    w = xpos + Width;

                slideDirection = -1;
            }

            foreach (ScLayer sclayer in leftScreen.controls)
                foreach (ScVxButton item in sclayer.controls)
                {
                    item.animProgress = 0;
                    item.StopAllAnim(); 
                }

            foreach (ScLayer sclayer in rightScreen.controls)
                foreach (ScVxButton item in sclayer.controls)
                {
                    item.animProgress = 0;
                    item.StopAllAnim();
                }

            //slideLen = 0;
            StartSlideScreenAnim(slideRect.X - w);
        }


   
        private void ScVxSlideScreen_MouseMove(object sender, ScMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isSlidering = true;
                slideLen = e.Location.X - mouseDownPos.X;

                if (slideLen > 0 && slideRect.X == 0)
                {
                    isSlidering = false;
                    return;
                }

                if (slideLen < 0 && slideRect.X == Width - slideRect.Width)
                {
                    isSlidering = false;
                    return;
                }

                SetSlidePos(mouseDownSlideRectPos.X + slideLen);
                Refresh();
            }
        }

        private void ScVxSlideScreen_MouseDown(object sender, ScMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPos.X = e.Location.X;
                mouseDownPos.Y = e.Location.Y;
                mouseDownSlideRectPos = new PointF(slideRect.X, slideRect.Y);
            }
        }

        void SetSlidePos(float x)
        {
            slideRect.X = x;

            if (slideRect.X > 0)
                slideRect.X = 0;

            if (slideRect.X < Width - slideRect.Width)
                slideRect.X = Width - slideRect.Width;

            if (x < Width - slideRect.Width)
                x = Width - slideRect.Width;

            float len = Math.Abs(x);
            int n = (int)len / (int)Width;
            float xpos = x + n * (int)Width;

            curtPageIdx = n;
            curtItemIdx = curtPageIdx * singleScreenItemTotalAmount;

           

            if(slideLen <0 && xpos == 0)
            {
                rightScreen.Location = new PointF(xpos, 0);
                leftScreen.Location = new PointF(xpos - leftScreen.Width, 0);

                rightScreenItems = rightScreen.AddItemList(itemList, curtItemIdx, 1.2f, true);
            }
            else
            {
                leftScreen.Location = new PointF(xpos, 0);
                rightScreen.Location = new PointF(xpos + leftScreen.Width, 0);

                leftScreenItems = leftScreen.AddItemList(itemList, curtItemIdx, 1.2f, true);

                int nextPageItemIdx = curtItemIdx + singleScreenItemTotalAmount;
                if (nextPageItemIdx < itemList.Count())
                    rightScreenItems = rightScreen.AddItemList(itemList, nextPageItemIdx, 1.2f, true);
            }


            //       
            if (pageSelector != null)
            {
                float ratio;

                if (slideLen < 0 && xpos == 0)
                {
                    ratio = (rightScreen.Width + rightScreen.Location.X) / rightScreen.Width;
                }
                else
                {
                    ratio = (leftScreen.Width + leftScreen.Location.X) / leftScreen.Width;
                }

                pageSelector.ps.selectedPageIdx = curtPageIdx;
                pageSelector.ps.leftPageRatio = ratio;
                pageSelector.ps.rightPageRatio = -(1 - ratio);
                 
                pageSelector.ps.ReLayout();
            }
        }

        public void SetSingleScreenItemAmount(int rowAmount, int colAmount)
        {
            singleScreenRowItemAmount = rowAmount;
            singleScreenColItemAmount = colAmount;
            ComputeScreenInfo();

            leftScreen.RowItemAmount = singleScreenRowItemAmount;           
            leftScreen.ColItemAmount = singleScreenColItemAmount;

            rightScreen.RowItemAmount = singleScreenRowItemAmount;
            rightScreen.ColItemAmount = singleScreenColItemAmount;
        }

        public int SingleScreenRowItemAmount
        {
            get { return singleScreenRowItemAmount; }
            set
            {
                singleScreenRowItemAmount = value;
                ComputeScreenInfo();
                leftScreen.RowItemAmount = singleScreenRowItemAmount;
                rightScreen.RowItemAmount = singleScreenRowItemAmount;
                ScSizeChanged(new SizeF(Width, Height));
            }
        }

        public int SingleScreenColItemAmount
        {
            get { return singleScreenColItemAmount; }
            set
            {
                singleScreenColItemAmount = value;
                ComputeScreenInfo();
                leftScreen.ColItemAmount = singleScreenColItemAmount;
                rightScreen.ColItemAmount = singleScreenColItemAmount;           
                ScSizeChanged(new SizeF(Width, Height));
            }
        }

        private void ScVxSlideScreen_SizeChanged(object sender, SizeF oldSize)
        {
            if (Width == 0 || Height == 0)
                return;
        
            leftScreen.Width = Width;
            leftScreen.Height = Height;

            rightScreen.Width = Width;
            rightScreen.Height = Height;

            ReLayout();
        }


        public void ReLayout()
        {
            if (useFixSizeItem)
                ComputeRowColItemAmount();
            else
                ComputeScreenInfo();

            float len = Math.Abs(slideRect.X);
            int n = (int)len / (int)Width;
            float xpos = slideRect.X + n * (int)Width;

            leftScreen.Location = new PointF(xpos, 0);
            rightScreen.Location = new PointF(xpos + leftScreen.Width, 0);

            curtItemIdx = n * singleScreenItemTotalAmount;

            //
            if (pageSelector != null)
            {
                pageSelector.ps.itemWidth = 12;
                pageSelector.ps.itemSpace = 10;

                pageSelector.ps.pageAmount = screenAmount;
                pageSelector.ps.selectedPageIdx = curtPageIdx;
                pageSelector.ps.leftPageRatio = 1.0f;
                pageSelector.ps.rightPageRatio = 0f;

                pageSelector.Width = (int)pageSelector.GetFixWidth();
                pageSelector.Height = 22;

                float posx = (int)(Width / 2 - pageSelector.Width / 2);
                float posy = (int)(Height - 15 - pageSelector.Height);

                pageSelector.Location = new PointF(posx, posy);

            }




            //
            leftScreenItems = leftScreen.AddItemList(itemList, curtItemIdx, 1.2f, true, true);

            if (leftScreenItems != null)
            {
                foreach (ScVxButton item in leftScreenItems)
                {
                    item.StartAllAnim();
                }
            }

            int nextPageItemIdx = curtItemIdx + singleScreenItemTotalAmount;
            if (nextPageItemIdx < itemList.Count())
            {
                rightScreenItems = rightScreen.AddItemList(itemList, nextPageItemIdx, 1.2f, true, true);

                if (rightScreenItems != null)
                {
                    foreach (ScVxButton item in rightScreenItems)
                    {
                        item.StartAllAnim();
                    }
                }
            }
        }
 
        public int GetItemPagePos(int itemIdx)
        {
            int m = itemIdx / singleScreenItemTotalAmount;
            int n = itemIdx % singleScreenItemTotalAmount;

            if (n > 0)
                m++;

            if (m != 0)
                m--;

            return m;
        }


        RectangleF GetPageRect(int pageIdx)
        {
            float x = slideRect.X + pageIdx * Width;
            RectangleF pageRect = new RectangleF(x, 0, Width, Height);
            return pageRect;
        }

        public void AddItems(List<ScLayer> items)
        {
            foreach (ScLayer item in items)
            {
                itemList.Add(item);
                item.MouseDown += Item_MouseDown;
            }
        }

        private void Item_MouseDown(object sender, ScMouseEventArgs e)
        {
            SelectedItem((ScLayer)sender);     
        }


        public void SelectedItem(ScLayer item)
        {
            foreach (ScVxButton btn in itemList)
            {
                if (btn == item)
                    continue;

                if (btn.isSelected)
                {
                    btn.isSelected = false;
                    btn.StopHaloAnim();
                    btn.Refresh();
                    break;
                }
            }
        }

        public void ComputeScreenInfo()
        {
            singleScreenItemTotalAmount = singleScreenRowItemAmount * singleScreenColItemAmount;

            screenAmount = itemList.Count() / singleScreenItemTotalAmount;
            int m = itemList.Count() % singleScreenItemTotalAmount;

            if (m != 0)
                screenAmount++;
   
            slideRect = new RectangleF(slideRect.X, slideRect.Y, screenAmount * Width, Height);
        }


        public void ComputeRowColItemAmount()
        {
            int wErrMiss = 0;
            int hErrMiss = 0;
            int itemRowPreAmount = (int) (leftScreen.Width / (itemFixWidth + 15));
            int itemColPreAmount = (int)((leftScreen.Height - 20) / (itemFixHeight + 15));

            if (itemRowPreAmount == 0)
                itemRowPreAmount = 1;

            if (itemColPreAmount == 0)
                itemColPreAmount = 1;

            while (true)
            {
                SetSingleScreenItemAmount(itemRowPreAmount, itemColPreAmount);
                Size sz = GetItemSize();

                float wErr = Math.Abs(sz.Width - itemFixWidth);
                float hErr = Math.Abs(sz.Height - itemFixHeight);

                if (wErr < itemFixWidth / 2 && hErr < itemFixHeight / 2)
                    return;

                if (wErr > itemFixWidth / 2)
                {
                    if(sz.Width < itemFixWidth)
                        itemRowPreAmount--;
                    else
                        itemRowPreAmount++;

                }

                if (hErr > itemFixHeight / 2)
                {
                    if (sz.Height < itemFixHeight)
                        itemColPreAmount--;
                    else
                        itemColPreAmount++;
                }
            }

        }
    }
}
