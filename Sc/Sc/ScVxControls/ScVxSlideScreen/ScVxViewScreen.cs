using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScVxViewScreen : ScLayer
    {
        int rowItemAmount = 6;
        int colItemAmount = 6;
        int totalItemAmount;

        float animBaseSpaceScale = 0.04f;
        float animPos;
        float val;

        ScAnimation rowLayerAnim;
        ScLinearAnimation rowLayerAnimLinear;

        ScAnimation rowLayerBackAnim;
        ScLinearAnimation rowLayerBackAnimLinear;

        public delegate void RowLayerAnimEventHandler();
        public event RowLayerAnimEventHandler RowLayerBackAnimEvent;
        public ScVxViewScreen()
        {
            SizeChanged += ScVxViewScreen_SizeChanged;

            rowLayerAnim = new ScAnimation(this, 200, true);
            rowLayerAnim.AnimationEvent += RowLayerAnim_AnimationEvent;

            rowLayerBackAnim = new ScAnimation(this, 200, true);
            rowLayerBackAnim.AnimationEvent += RowLayerBackAnim_AnimationEvent;
        }

      

        ~ScVxViewScreen()
        {
            rowLayerAnim.Stop();
            rowLayerBackAnim.Stop();
        }

        private void RowLayerAnim_AnimationEvent(ScAnimation scAnimation)
        {
            float v = 0;
            float len = rowLayerAnimLinear.GetCurtValue();

            if (rowLayerAnimLinear.IsStop)
            {
                scAnimation.Stop();
            }

            for (int i = 0; i < controls.Count; i++)
            {
                ScLayer rowlayer = controls[i];
                v = len * i * animBaseSpaceScale + animPos;
                rowlayer.Location = new PointF(v, rowlayer.Location.Y);
            }

            Refresh();
        }


        private void RowLayerBackAnim_AnimationEvent(ScAnimation scAnimation)
        {
            float v = 0;
            float len = rowLayerBackAnimLinear.GetCurtValue();

            if (rowLayerBackAnimLinear.IsStop)
            {
                scAnimation.Stop();

                if (RowLayerBackAnimEvent != null)
                    RowLayerBackAnimEvent();
            }

            for (int i = 0; i < controls.Count; i++)
            {
                ScLayer rowlayer = controls[i];
                v = len * i * animBaseSpaceScale + animPos;
                rowlayer.Location = new PointF(v, rowlayer.Location.Y);
            }

            Refresh();
        }


        public void StartRowLayerAnim()
        {
            val = Location.X;
            rowLayerAnim.Stop();
            rowLayerAnimLinear = new ScLinearAnimation(0, val, rowLayerAnim);
            rowLayerAnim.Start();
        }

        public void StartRowLayerBackAnim()
        {
            float v = val;

            rowLayerBackAnim.Stop();
            rowLayerBackAnimLinear = new ScLinearAnimation(v, 0, rowLayerBackAnim);
            rowLayerBackAnim.Start();
        }

        public int RowItemAmount
        {
            get { return rowItemAmount; }
            set
            {
                rowItemAmount = value;
                totalItemAmount = rowItemAmount * colItemAmount;
            }
        }

        public int ColItemAmount
        {
            get { return colItemAmount; }
            set
            {
                colItemAmount = value;
                totalItemAmount = rowItemAmount * colItemAmount;
            }
        }


        public int TotalItemAmount
        {
            get { return totalItemAmount; }
        }


        public Size GetItemSize()
        {
            RectangleF colRect = new RectangleF(0, 0, controls[0].Width, controls[0].Height);
            Table coltable = new Table(colRect, 1, colItemAmount);
            RectangleF cellRect = coltable.GetCellContentRect(0, 0);

            float w = cellRect.Width / 20;
            float h = cellRect.Height / 40;
            Margin defaultCellMargin = new Margin(w, h, w, h);
            coltable.SetDefaultCellMargin(defaultCellMargin);

            cellRect = coltable.GetCellContentRect(0, 0);
            Size sz = new Size((int)cellRect.Width, (int)cellRect.Height);
            return sz;
        } 

        private void ScVxViewScreen_SizeChanged(object sender, SizeF oldSize)
        {
            RectangleF tableRect = new RectangleF(20, 40, Width - 40, Height - 80);
            Table screentable = new Table(tableRect, rowItemAmount, 1);
            ScLayer rowLayer;
            RectangleF rowRect;

            controls.Clear();

            for (int i = 0; i < rowItemAmount; i++)
            {
                rowLayer = new ScLayer();
                rowLayer.DirectParentClipLayer = ScMgr.GetRootLayer();
                rowRect = screentable.GetCellContentRect(i, 0);

                rowLayer.Location = new PointF((int)rowRect.X, (int)rowRect.Y);
                rowLayer.Width = (int)rowRect.Width;
                rowLayer.Height = (int)rowRect.Height;

                Add(rowLayer);
            }

            animPos = controls[0].Location.X;
        }


        public List<ScLayer> AddItemList(List<ScLayer> itemList, int startIdx, float zoom, bool isAnim,  bool isReturnItem = false)
        {
            RectangleF colRect = new RectangleF(0, 0, controls[0].Width, controls[0].Height);
            Table coltable = new Table(colRect, 1, colItemAmount);
            RectangleF cellRect = coltable.GetCellRect(0,0);
            float w = cellRect.Width / 20;
            float h = cellRect.Height / 40;
            Margin defaultCellMargin = new Margin(w, h, w, h);
            coltable.SetDefaultCellMargin(defaultCellMargin);


            int n = startIdx;
            ScLayer item;

            foreach (ScLayer rowLayer in controls)
                rowLayer.Clear();

            if (itemList.Count() == 0)
                return null;

            foreach (ScLayer rowLayer in controls)
            {
               // rowLayer.SuspendLayout();

                for(int i=0; i< coltable.colAmount;i++)
                {
                    if (n == itemList.Count)
                    {
                       // rowLayer.ResumeLayout(false);

                        if (isReturnItem)
                        {
                            List<ScLayer> usedItemList = new List<ScLayer>();
                            for (int j = 0; j < n; j++)
                                usedItemList.Add(itemList[j]);
                            return usedItemList;
                        }

                        return null;
                    }

                    cellRect = coltable.GetCellContentRect(0, i);
                    item = itemList[n++];
                  //  item.Separate();

                  //  item.SuspendLayout();

                    if (item.GetType() == typeof(ScVxButton))
                    {
                        if(!isAnim)
                            ((ScVxButton)item).animProgress = 0;

                        ((ScVxButton)item).SetZoomValue(zoom);
                    }

                    item.Location = new PointF((int)cellRect.X, (int)cellRect.Y);
                    //item.RotateAngle = 25;

                    if(item.Width != (int)cellRect.Width )
                        item.Width = (int)cellRect.Width;

                    if (item.Height != (int)cellRect.Height)
                        item.Height = (int)cellRect.Height;

                 //   item.ResumeLayout(false);

                    rowLayer.Add(item);
                }

              //  rowLayer.ResumeLayout(false);
            }


            if (isReturnItem)
            {
                List<ScLayer> usedItemList = new List<ScLayer>();
                for (int i = 0; i < n; i++)
                    usedItemList.Add(itemList[i]);
                return usedItemList;
            }

            return null;
        }
    }
}
