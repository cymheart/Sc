using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ScTabHeader : ScLayer
    {
        LinkedList<ScTabHeaderItem> tabHeadItemListDepth = new LinkedList<ScTabHeaderItem>();
        LinkedList<ScTabHeaderItem> tabHeadItemListLocation = new LinkedList<ScTabHeaderItem>();

        Font font = new Font("微软雅黑", 10);
        SizeMode sizeMode = SizeMode.Normal;
        SizeF itemSize = new SizeF(112, 40);
        int itemSpacing = -10;

        ScLayer panelView;
        ScTabHeaderPevNextBtn preNextBtn = null;

        public int upOrBottom = 0;

        int downType = 0;
        public delegate void SelectedItemEventHandler(ScTabHeaderItem selectedItem);
        public event SelectedItemEventHandler SelectedItemEvent;

        ScAnimation scAnim;
        ScStepAnimation stepAnim;

        public float ItemSize;


        ScTabHeaderItem downItem;
        public ScTabHeader(ScMgr scmgr = null)
            : base(scmgr)
        {
           IsUseDebugPanitCode = true;
            panelView = new ScLayer(scmgr);
            Add(panelView);

            panelView.IsUseDebugPanitCode = true;

            preNextBtn = new ScTabHeaderPevNextBtn(scmgr);
            preNextBtn.Visible = false;
            Add(preNextBtn);
          
            scAnim = new ScAnimation(panelView, -1, true);
            scAnim.AnimationEvent += ScAnim_AnimationEvent;

            SizeChanged += ScTabHeader_SizeChanged;
        }

        private void ScTabHeader_SizeChanged(object sender, SizeF oldSize)
        {
            preNextBtn.Width = 50;
            preNextBtn.Height = Height;
            preNextBtn.Location = new PointF(Width - 50, 0);

            panelView.Height = Height;

        }

        public ScTabHeaderItem AddItem(string name)
        {
            Size size = TextRenderer.MeasureText(name, font);
            int w = size.Width + 40;

            ScTabHeaderItem item = new ScTabHeaderItem(ScMgr);



            switch (sizeMode)
            {
                case SizeMode.Normal:
                    item.Width = itemSize.Width;
                    item.Height = itemSize.Height;
                    break;

                case SizeMode.Fixed:
                    item.Width = w;
                    item.Height = size.Height;
                    break;
            }

            item.index = tabHeadItemListLocation.Count;
            item.Name = name;
            item.GetSelectedItemEvent += Item_GetSelectedItemEvent;
            item.MouseDownItemEvent += Item_MouseDownItemEvent;
            item.AnimalStopEvent += Item_AnimalStopEvent;
            item.IsHitThrough = false;

            tabHeadItemListLocation.AddLast(item);
            tabHeadItemListDepth.AddLast(item);
            AdjustItemLocation();

            panelView.Add(item);

            return item;
        }

        private void Item_MouseDownItemEvent(ScTabHeaderItem item)
        {
            MouseDownItem(item);
        }

        private ScTabHeaderItem Item_GetSelectedItemEvent()
        {
            return GetSelectedItem();
        }

        private void Item_AnimalStopEvent(ScTabHeaderItem selectedItem)
        {
            if (SelectedItemEvent != null && downItem != null)
            {
                SelectedItemEvent(downItem);
                downItem = null;
            }
        }

    

        public void MouseDownItem(ScTabHeaderItem downItem)
        {
            LinkedListNode<ScTabHeaderItem> selectedItemNodeInDepth = tabHeadItemListDepth.Last;
            selectedItemNodeInDepth.Value.StartLeaveAnim();

            AdjustItemDepth(downItem);
            ReAddAllItems();
            this.downItem = downItem;
        }


        public int SetSelectedItem(ScTabHeaderItem downItem)
        {
            downItem.SetSelectedItem();
            AdjustItemDepth(downItem);
            ReAddAllItems();
            if (SelectedItemEvent != null)
                SelectedItemEvent(downItem);

            return downItem.index;
        }

        public ScTabHeaderItem SetSelectedIndex(int selectedIndex)
        {
            ScTabHeaderItem item;
            ScTabHeaderItem selectedItem = null;
            LinkedListNode<ScTabHeaderItem> node = tabHeadItemListLocation.First;

            for (; node != null; node = node.Next)
            {
                item = node.Value;

                if (item.index == selectedIndex)
                {
                    item.SetSelectedItem();
                    selectedItem = item;
                }
                else
                {
                    item.SetNormalItem();
                }
            }

            if (selectedItem == null)
                return null;

            AdjustItemDepth(selectedItem);
            ReAddAllItems();
            if (SelectedItemEvent != null)
                SelectedItemEvent(selectedItem);

            return selectedItem;
        }

        ScTabHeaderItem SearchSelectedIndexItem(int selectedIndex)
        {
            LinkedListNode<ScTabHeaderItem> node = tabHeadItemListLocation.First;
            ScTabHeaderItem item;

            for (; node != null; node = node.Next)
            {
                item = node.Value;

                if (item.index == selectedIndex)
                    return item;
            }
            return null;
        }

        public void ReAddAllItems()
        {
            panelView.Clear();

            LinkedListNode<ScTabHeaderItem> node = tabHeadItemListDepth.First;
            for (; node != null; node = node.Next)
            {
                panelView.Add(node.Value);
            }

            if (panelView.Width > Width)
            {
                preNextBtn.Visible = true;
            }

            panelView.Refresh();
        }

        void AdjustItemLocation()
        {
            float posX = 0;
            float posY = 0;

            LinkedListNode<ScTabHeaderItem> node = tabHeadItemListLocation.First;
            ScTabHeaderItem item;

            for (; node != null; node = node.Next)
            {
                item = node.Value;

                item.Location = new PointF(posX, posY);
                posX += item.Width + itemSpacing;
            }

            panelView.Width = posX - itemSpacing;

        }

        void AdjustItemDepth(ScTabHeaderItem downItem)
        {
            LinkedListNode<ScTabHeaderItem> selectedItemNodeInDepth = tabHeadItemListDepth.Last;
            LinkedListNode<ScTabHeaderItem> selectedNodeInLocation = tabHeadItemListLocation.Find(selectedItemNodeInDepth.Value);
            LinkedListNode<ScTabHeaderItem> preNode = selectedNodeInLocation.Previous;
            LinkedListNode<ScTabHeaderItem> nextNode = selectedNodeInLocation.Next;

            if (preNode == null && nextNode == null)
                return;

            tabHeadItemListDepth.Remove(selectedItemNodeInDepth);
            LinkedListNode<ScTabHeaderItem> nodeDepth;

            if (preNode == null)
            {
                nodeDepth = tabHeadItemListDepth.Find(nextNode.Value);
                tabHeadItemListDepth.AddBefore(nodeDepth, selectedItemNodeInDepth);
            }
            else
            {
                nodeDepth = tabHeadItemListDepth.Find(preNode.Value);
                tabHeadItemListDepth.AddAfter(nodeDepth, selectedItemNodeInDepth);
            }

            LinkedListNode<ScTabHeaderItem> node = tabHeadItemListDepth.Find(downItem);
            tabHeadItemListDepth.Remove(node);
            tabHeadItemListDepth.AddLast(node);
        }


        public void SetAllItemUseMsgHints(bool useMsgHints)
        {
            LinkedListNode<ScTabHeaderItem> node = tabHeadItemListDepth.First;
            for (; node != null; node = node.Next)
            {
                node.Value.UseMsgHints = useMsgHints;
            }
        }


        public ScTabHeaderItem GetSelectedItem()
        {
            return tabHeadItemListDepth.Last.Value;
        }


        public void PreOrNextBtnUp()
        {
            scAnim.Stop();
        }

        public void PreBtnDown()
        {
            downType = 0;

            float m = 20;
            float x = panelView.Location.X;
            float y = panelView.Location.Y;
            x += m;

            if (panelView.DirectionRect.Left == 0)
                return;

            if (x > 0)
                x = 0;

            panelView.Location = new PointF(x, y);
            Refresh();

            StartAnim(5);
        }


        public void NextBtnDown()
        {
            downType = 1;
            float m = 20;
            float x = panelView.Location.X;
            float y = panelView.Location.Y;
            x -= m;

            if (panelView.DirectionRect.Right == preNextBtn.Location.X)
                return;

            if (x - m + panelView.Width < preNextBtn.Location.X)
                x = preNextBtn.Location.X - panelView.Width;

            panelView.Location = new PointF(x, y);
            Refresh();

            StartAnim(-5);
        }

        public void StartAnim(float step)
        {
            if (scAnim == null)
                return;

            scAnim.Stop();
            stepAnim = new ScStepAnimation(panelView.Location.X, step, scAnim);
            scAnim.Start();
        }

        private void ScAnim_AnimationEvent(ScAnimation scAnimation)
        {
            float valx;
            AnimationEffect ae = stepAnim;

            if (ae == null)
                return;

            valx = (int)ae.GetCurtValue();

            if (downType == 0)
            {
                if (panelView.DirectionRect.Left == 0)
                    return;

                if (valx > 0)
                    valx = 0;
            }
            else
            {
                if (panelView.DirectionRect.Right == preNextBtn.Location.X)
                    return;

                if (valx + panelView.Width < preNextBtn.Location.X)
                    valx = preNextBtn.Location.X - panelView.Width;
            }

            panelView.Location = new PointF(valx, panelView.Location.Y);
            Refresh();
        }
    }
}
