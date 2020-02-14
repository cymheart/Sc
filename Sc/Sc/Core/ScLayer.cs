
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

//#define USE_DEBUG_CODE

using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace Sc
{
    public class ScLayer : ScObject,IDisposable
    {
        public ScLayer(ScMgr scmgr = null)
        {
            ScMgr = scmgr;
            Opacity = 1.0f;

            MouseEnter += ScControl_MouseEnter;
            MouseLeave += ScControl_MouseLeave;

            visableAnim = new ScAnimation(this, VisibleAnimMS, true);
            visableAnim.AnimationEvent += VisibleAnim_AnimationEvent;
        }

        ~ScLayer()
        {
            this.Dispose();
        }

        public virtual void Dispose()
        {
            dispose = true;
            ReleaseSelfTransformInfo();

            directClipChildLayerList.Clear();

            foreach (ScAnimation anim in animationList)
            {
                anim.Dispose();
            }

            foreach (ScLayer clayer in controls)
            { 
                clayer.Dispose();      
            }

            animationList.Clear();
            directClipChildLayerList.Clear();
            controls.Clear();

            GC.SuppressFinalize(this);
        }

        #region 内部参数
        
        ScLayer parent;
        public List<ScLayer> controls = new List<ScLayer>();

        ScMgr scMgr = null;
        RectangleF directionRect = new RectangleF(0, 0, 0, 0);
        Geometry hitPathGeometry;    //点击PathGeometry
        GraphicsPath hitGraphicsPath;    //点击GraphicsPath

        float opacity = 1.0f;
        float rotateAngle = 0;
        float scaleX = 1.0f;
        float scaleY = 1.0f;
        Color? backgroundColor = null;

        int suspendLayoutCount = 0;

        bool dispose = false;
        bool visible = true;
        bool focus;
        Cursor cursor = null;
        object tag = null;
        protected ScDockStyle dock = ScDockStyle.None;

        ScLayer directParentClipLayer;
        List<ScLayer> directClipChildLayerList = new List<ScLayer>();
        List<ScAnimation> animationList = new List<ScAnimation>();
        ScLayer shadowLayer;

        /// <summary>
        /// 相对于父的子矩形 
        /// </summary>
        RectangleF boundBox = RectangleF.Empty;

        /// <summary>
        /// 全局重绘rect
        /// </summary>
        RectangleF drawBox = RectangleF.Empty;

        /// <summary>
        /// 变换后的layer显示区域点的位置
        /// </summary>
        PointF[] transLastPts;

        /// <summary>
        /// 变换后的点击PathGeometry 
        /// </summary>
        Geometry transLastHitPathGeometry;

        /// <summary>
        /// 变换后的点击GraphicsPath
        /// </summary>
        GraphicsPath transLastHitGraphicsPath;

        PointF anchor = new PointF(0, 0);
        public Matrix localMatrix = new Matrix();
        Matrix globalMatrix = new Matrix();
        Matrix invertGlobalMatrix = new Matrix();
        Matrix drawGlobalMatrix = null;

        ScAnimation visableAnim;
        ScLinearAnimation opacityLinear;

        bool isUseAnimVisible = false;
        bool isNotAtRootDrawBoxBound = false;
        bool isUseDebugPanitCode = false;
        bool usePosttreatmentEffect = false;
        bool useHitGeometryLayerBound = false;
  
        //
        bool isComputedStraight = false;


        #endregion

        #region 读写参数设置(普通参数设置，不会对布局重绘参数影响)


        public ScLayer Parent
        {
            get { return parent; }
        }

        public List<ScLayer> DirectClipChildLayerList
        {
            get { return directClipChildLayerList; }
        }
        public Matrix GlobalMatrix
        {
            get
            {
                return globalMatrix;
            }
        }

        public Matrix InvertGlobalMatrix
        {
            get
            {
                return invertGlobalMatrix;
            }
        }

        /// <summary>
        /// 是否为计算后的正矩形区域
        /// </summary>
        public bool IsComputedStraight
        {
            get { return isComputedStraight; }
        }

        /// <summary>
        /// 是否层全局DrawBox在根DrawBox范围内
        /// </summary>
        public bool IsNotAtRootDrawBoxBound
        {
            get { return isNotAtRootDrawBoxBound; }
        }

        public Geometry TransLastHitPathGeometry
        {
            get
            {
                return transLastHitPathGeometry;
            }
        }


        public GraphicsPath TransLastHitGraphicsPath
        {
            get
            {
                return transLastHitGraphicsPath;
            }
        }

        public Matrix DrawGlobalMatrix
        {
            get
            {
                return drawGlobalMatrix;
            }
        }

        public PointF[] TransLastPts
        {
            get
            {
                return transLastPts;
            }
        }

        public bool Focused
        {
            get
            {
                return focus;
            }
        }

        public string Name { get; set; }
        public string Type { get; set; }

        public object Tag
        {
            get
            {
                return tag;
            }

            set
            {
                tag = value;
            }
        }

        public Cursor Cursor
        {
            get
            {
                return cursor;
            }

            set
            {
                cursor = value;
            }
        }



        #endregion

        #region 功能函数设置(普通参数设置，不会对布局重绘参数影响)
        public void SetCursor(Cursor cursor)
        {
            this.cursor = cursor;
            ScMgr.control.Cursor = cursor;
        }
        public bool Focus()
        {
            if (focus == true)
                return true;

            if (scMgr.control.Focused)
            {
                EventArgs e = new EventArgs();
                ScLayer oldFocus = scMgr.FocusScControl;

                if (oldFocus != null)
                    oldFocus.ScLostFocus(e);

                scMgr.FocusScControl = this;
                ScGotFocus(e);
                return true;
            }

            scMgr.FocusScControl = this;
            scMgr.control.Focus();
            return true;
        }
        public void SetImeWindowsPos(int x, int y)
        {
            PointF pt = new Point(x, y);
            pt = TransLocalToGlobal(pt);

            if (scMgr.controlType == ControlType.STDCONTROL)
                ((ScLayerControl)ScMgr.control).SetImeWindowsPos((int)pt.X, (int)pt.Y);
            else
                ((UpdateLayerFrm)ScMgr.control).SetImeWindowsPos((int)pt.X, (int)pt.Y);
        }


        public void AppendAnimation(ScAnimation animation)
        {
            animationList.Add(animation);
        }

        #endregion

        #region 可设置普通参数，影响布局，重绘

        public Margin Margin = new Margin(0, 0, 0, 0);
        public Margin Padding = new Margin(0, 0, 0, 0);

        public bool IsComputeBoundBox = false;

        public bool IsRender = true;

        /// <summary>
        /// 是否使用原始方形点击区域
        /// </summary>
        public bool IsUseOrgHitGeometry = true;

        /// <summary>
        /// 是否使用点击区域作为层边界
        /// </summary>
        public bool IsUseHitGeometryLayerBound = false;


        /// <summary>
        /// 是否使用整型坐标绘制控件
        /// </summary>
        public bool IsUseIntegerCoordDraw = true;

      
        /// <summary>
        /// 事件是否可以穿透当前层,作用于父层，或兄弟层次
        /// </summary>
        public bool IsHitThrough = true;

      
        /// <summary>
        /// 层隐藏显示切换过渡时间
        /// </summary>
        public int visibleAnimMS = 20;

        /// <summary>
        /// 是否打开调试绘制代码
        /// </summary>
        public bool IsUseDebugPanitCode
        {
            set
            {
                isUseDebugPanitCode = value;

#if USE_DEBUG_CODE
                if (value == true)
                {
                     D2DPaint += DebugLayer_D2DPaint;
                }
                else
                {
                    D2DPaint -= DebugLayer_D2DPaint;
                }
#endif
            }
        }

        /// <summary>
        /// 背景色
        /// </summary>
        public Color? BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
            }
        }

        public bool Enable = true;

        /// <summary>
        /// 直接作用的父裁剪层
        /// </summary>
        public ScLayer DirectParentClipLayer
        {
            get { return directParentClipLayer; }

            set
            {
                directParentClipLayer = value;       
            }
        }


        /// <summary>
        /// 设置阴影层
        /// </summary>
        public ScLayer ShadowLayer
        {
            get { return shadowLayer; }

            set
            {
                shadowLayer = value;         
            }
        }


        public RectangleF DrawBox
        {
            get
            {
                return drawBox;
            }

            set
            {
                drawBox = value;
            }
        }

        public int VisibleAnimMS
        {
            get { return visibleAnimMS; }
            set
            {
                visibleAnimMS = value;
                visableAnim.animMS = value;
            }
        }


        /// <summary>
        /// 锚点位置
        /// </summary>
        public PointF Anchor
        {
            get
            {
                return anchor;
            }
            set
            {
                anchor = value;
            }
        }

        public ScMgr ScMgr
        {
            get
            {
                return scMgr;
            }
            set
            {
                scMgr = value;
            }
        }

        public ScDockStyle Dock
        {
            get
            {
                return dock;
            }

            set
            {
                dock = value;
            }
        }



        #endregion

        #region 可设置关联性参数，参数设置不触发布局，但影响到是否为正矩形区域判断

        public bool IsUseAnimVisible
        {
            get
            {
                return isUseAnimVisible;
            }
            set
            {
                isUseAnimVisible = value;

                if (ShadowLayer != null)
                    ShadowLayer.IsUseAnimVisible = value;
            }
        }



        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                if (IsUseAnimVisible)
                {
                    StartVisibleAnim(value);
                }
                else
                {
                    visible = value;
                    if (ShadowLayer != null)
                        ShadowLayer.Visible = value;
                }
            }
        }

        /// <summary>
        /// 层透明度设置
        /// </summary>
        public float Opacity
        {
            get { return opacity; }
            set
            {
                opacity = value;

                if (IsStraightRectArea())
                    isComputedStraight = true;
                else
                    isComputedStraight = false;
            }
        }


        /// <summary>
        /// 后处理特效
        /// </summary>
        public bool UsePosttreatmentEffect
        {
            get { return usePosttreatmentEffect; }
            set
            {
                usePosttreatmentEffect = value;

                if (IsStraightRectArea())
                    isComputedStraight = true;
                else
                    isComputedStraight = false;
            }
        }

        /// <summary>
        /// 后处理特效
        /// </summary>
        public bool UseHitGeometryLayerBound
        {
            get { return useHitGeometryLayerBound; }
            set
            {
                useHitGeometryLayerBound = value;

                if (IsStraightRectArea())
                    isComputedStraight = true;
                else
                    isComputedStraight = false;
            }
        }


        #endregion

        #region 可设置关联性参数，参数设置触发布局
        public PointF Location
        {
            get
            {
                return new PointF(directionRect.X, directionRect.Y);
            }

            set
            {
                if (directionRect.X != value.X || directionRect.Y != value.Y)
                {
                    float oldValueX = directionRect.X;
                    float oldValueY = directionRect.Y;

                    directionRect.X = value.X;
                    directionRect.Y = value.Y;

                    ScLocationChanged(new PointF(oldValueX, oldValueY));
                }

                if (suspendLayoutCount == 0)
                    Layout();
            }
        }
        public float Width
        {
            get
            {
                return directionRect.Width;
            }

            set
            {
                if (directionRect.Width != value)
                {
                    float oldValue = directionRect.Width;
                    directionRect.Width = value;
                    ScSizeChanged(new SizeF(oldValue, directionRect.Height));
                }

                if (suspendLayoutCount == 0)
                {
                    Layout();
                }
            }
        }

        public float Height
        {
            get
            {
                return directionRect.Height;
            }

            set
            {
                if (directionRect.Height != value)
                {
                    float oldValue = directionRect.Height;
                    directionRect.Height = value;
                    ScSizeChanged(new SizeF(directionRect.Width, oldValue));
                }

                if (suspendLayoutCount == 0)
                {
                    Layout();
                }
            }
        }

        public SizeF Size
        {
            get
            {
                return new SizeF(Width, Height);
            }

            set
            {
                if (directionRect.Height != value.Height || directionRect.Width != value.Width)
                {
                    float oldValueWidth = directionRect.Width;
                    float oldValueHeight = directionRect.Height;

                    directionRect.Width = value.Width;
                    directionRect.Height = value.Height;

                    ScSizeChanged(new SizeF(oldValueWidth, oldValueHeight));
                }

                if (suspendLayoutCount == 0)
                    Layout();
            }
        }

        public RectangleF DirectionRect
        {
            get
            {
                return directionRect;
            }

            set
            {
                if (directionRect.Height != value.Height || directionRect.Width != value.Width)
                {
                    float oldValueWidth = directionRect.Width;
                    float oldValueHeight = directionRect.Height;

                    directionRect = value;

                    ScSizeChanged(new SizeF(oldValueWidth, oldValueHeight));
                }
                else
                {
                    directionRect = value;
                }

                if (suspendLayoutCount == 0)
                    Layout();
            }
        }

        /// <summary>
        /// 根据Dock类型设置层的宽高，位置
        /// </summary>
        /// <param name="parent"></param>
        public bool SetDirectionRectByDockType()
        {
            float x, y, w, h;

            switch (dock)
            {
                case ScDockStyle.Fill:
                    w = parent.Width - ((parent.Padding.left + parent.Padding.right) + (Margin.left + Margin.right));
                    h = parent.Height - ((parent.Padding.top + parent.Padding.bottom) + +(Margin.top + Margin.bottom));
                    x = parent.Padding.left + Margin.left;
                    y = parent.Padding.top + Margin.top;

                    if (IsUseIntegerCoordDraw)
                    {
                        DirectionRect = new RectangleF((int)x, (int)y, (int)w, (int)h);
                    }
                    else
                    {
                        DirectionRect = new RectangleF(x, y, w, h);
                    }

                    return true;

                case ScDockStyle.Center:
                    x = parent.Width / 2 - Width / 2;
                    y = parent.Height / 2 - Height / 2;

                    if (IsUseIntegerCoordDraw)
                    {
                        Location = new PointF((int)x, (int)y);
                    }
                    else
                    {
                        Location = new PointF(x, y);
                    }

                    return true;

                case ScDockStyle.RightTop:
                    x = parent.Width - Width;
                    y = 0;

                    if (IsUseIntegerCoordDraw)
                    {
                        Location = new PointF((int)x, (int)y);
                    }
                    else {
                        Location = new PointF(x, y);
                    }

                    return true;
            }

            return false;
        }
        public float RotateAngle
        {
            get
            {
                return rotateAngle;
            }

            set
            {
                rotateAngle = value;
                localMatrix = new Matrix();
                localMatrix.Rotate(value);
                localMatrix.Scale(scaleX, scaleY);

                if (suspendLayoutCount == 0)
                    Layout();
            }
        }

        public float ScaleX
        {
            get
            {
                return scaleX;
            }

            set
            {
                scaleX = value;
                localMatrix = new Matrix();

                if(rotateAngle != 0)
                    localMatrix.Rotate(rotateAngle);

                localMatrix.Scale(scaleX, scaleY);

                if (suspendLayoutCount == 0)
                    Layout();
            }
        }

        public float ScaleY
        {
            get
            {
                return scaleY;
            }
            set
            {
                scaleY = value;
                localMatrix = new Matrix();

                if (rotateAngle != 0)
                    localMatrix.Rotate(rotateAngle);

                localMatrix.Scale(scaleX, scaleY);

                if (suspendLayoutCount == 0)
                    Layout();
            }
        }



      

        #endregion

        #region 添加,插入,移除子层
        public void Add(ScLayer childLayer)
        {
            if (childLayer == null)
                return;

            if (childLayer.ShadowLayer != null)
                Add(childLayer.ShadowLayer);

            childLayer.parent = this;
            controls.Add(childLayer);

            childLayer.CreateContextRelationInfo();

            if (suspendLayoutCount == 0)
                childLayer.Layout();
    
        }


        public void Insert(ScLayer pevLayer, ScLayer childLayer)
        {
            int idx = 0;
            for (int i = 0; i < controls.Count(); i++)
            {
                if (controls[i] == pevLayer)
                {
                    idx = i;
                    break;
                }
            }

            childLayer.parent = this;
            controls.Insert(idx, childLayer);

            childLayer.CreateContextRelationInfo();

            if (suspendLayoutCount == 0)
                childLayer.Layout();

            if (childLayer.ShadowLayer != null)
                Insert(childLayer, childLayer.ShadowLayer);
        }


        public void CreateContextRelationInfo()
        {
            if (ReBulid != null && 
                !scMgr.rebulidLayerList.Contains(this))
            {
                scMgr.AddReBulidLayer(this);
            }

            if (DirectParentClipLayer != null &&
                !DirectParentClipLayer.DirectClipChildLayerList.Contains(this))
            {
                DirectParentClipLayer.DirectClipChildLayerList.Add(this);
            }

            suspendLayoutCount = parent.suspendLayoutCount;
            scMgr = parent.scMgr;

            foreach (ScLayer clayer in controls)
            {
                clayer.CreateContextRelationInfo();
            }
        }

        public void Remove(ScLayer childLayer)
        {       
            childLayer.RemoveContextRelationInfo();
            childLayer.Dispose();     
            controls.Remove(childLayer);
        }


        public void RemoveContextRelationInfo()
        {
            if (ReBulid != null &&
                scMgr.rebulidLayerList.Contains(this))
            {
                scMgr.rebulidLayerList.Remove(this);
            }

            if (DirectParentClipLayer != null &&
                DirectParentClipLayer.DirectClipChildLayerList.Contains(this))
            {
                DirectParentClipLayer.DirectClipChildLayerList.Remove(this);
            }

            foreach (ScLayer clayer in controls)
            {
                clayer.RemoveContextRelationInfo();
            }
        }


        public void Clear()
        {
            controls.Clear();
        }

        #endregion
 
        #region 布局控制
        public void SuspendLayout()
        {
            suspendLayoutCount++;
            IncChildSuspendLayout();
        }


        public void ResumeLayout(bool performLayout)
        {
            suspendLayoutCount--;
            if (suspendLayoutCount < 0)
                suspendLayoutCount = 0;

            DecClildSuspendLayout();

            if (performLayout && suspendLayoutCount == 0)
                Layout();
        }


        private void IncChildSuspendLayout()
        {
            foreach (ScLayer clayer in controls)
            {
                clayer.suspendLayoutCount++;
                clayer.IncChildSuspendLayout();
            }
        }

        private void DecClildSuspendLayout()
        {
            foreach (ScLayer clayer in controls)
            {
                clayer.suspendLayoutCount--;
                clayer.DecClildSuspendLayout();
            }
        }

        #endregion      

        #region 布局
        public void Layout()
        {
            if (ComputeSelfLayout() == 0)
                ChildLayerLayout();
        }

        void ChildLayerLayout()
        {
            foreach (ScLayer sclayer in controls)
            {
                if (sclayer.ComputeSelfLayout() == 0)
                    sclayer.ChildLayerLayout();
            }
        }

        public int ComputeSelfLayout()
        {
            int ret;

            if (ScMgr == null || 
                parent == null  ||
                parent.globalMatrix == null || 
                ScMgr.GetRootLayer() == null)
            {
                return -1;
            }

            ReleaseSelfTransformInfo();

            ret = _ComputeSelfLayout();

            return ret;
        }

        public int _ComputeSelfLayout()
        {

            RectangleF rootDrawBox = parent.ScMgr.GetRootLayer().DrawBox;

            //
            if (IsStraightRectArea())
            {
                isComputedStraight = true;
                globalMatrix = new Matrix(1, 0, 0, 1, parent.DrawBox.X + directionRect.Left, parent.DrawBox.Y + directionRect.Top);
                drawBox = new RectangleF(globalMatrix.Elements[4], globalMatrix.Elements[5], Width, Height);

                //
                if (!rootDrawBox.IntersectsWith(drawBox))
                {
                    isNotAtRootDrawBoxBound = true;
                    return 1;
                }

                isNotAtRootDrawBoxBound = false;
                boundBox = directionRect;

                if (!IsUseOrgHitGeometry)
                {
                    //获取点击区域
                    _CreateHitGeometry();

                    //获取变换后点击区域
                    ComputeTransLastHitGeometry();
                }

                return 0;
            }


            float anchorX = Location.X + anchor.X;
            float anchorY = Location.Y + anchor.Y;

            globalMatrix = parent.GlobalMatrix.Clone();

            //获取层全局绘制框,转换层矩形区域为全局区域位置
            drawBox = ComputeBoundBox(globalMatrix, ref transLastPts);

            if (!rootDrawBox.IntersectsWith(drawBox))
            {
                isNotAtRootDrawBoxBound = true;
                return 1;
            }

            isNotAtRootDrawBoxBound = false;

            //平移全局变换矩阵
            globalMatrix.Translate(directionRect.Left - anchorX, directionRect.Top - anchorY);

            //获取全局逆矩阵
            invertGlobalMatrix = globalMatrix.Clone();
            invertGlobalMatrix.Invert();

            //获取自身包围框
            if (IsComputeBoundBox)
            {
                Matrix m = new Matrix();
                PointF[] pts = null;
                boundBox = ComputeBoundBox(m, ref pts);
            }

              
            //获取点击区域
            _CreateHitGeometry();

            //获取变换后点击区域
            ComputeTransLastHitGeometry();

            //
            if (IsStraightRectArea())
                isComputedStraight = true;
            else
                isComputedStraight = false;


            return 0;
        }

        #endregion

        #region 布局相关功能
        public RectangleF ComputeBoundBox(Matrix m, ref PointF[] transPts)
        {

            float anchorX = Location.X + anchor.X;
            float anchorY = Location.Y + anchor.Y;

            transPts = new PointF[]
            {
                new PointF(directionRect.Left - anchorX, directionRect.Top - anchorY),
                new PointF(directionRect.Right- anchorX, directionRect.Top - anchorY),
                new PointF(directionRect.Right - anchorX, directionRect.Bottom - anchorY),
                new PointF(directionRect.Left - anchorX, directionRect.Bottom - anchorY)
            };

            m.Translate(anchorX, anchorY);

            if (!localMatrix.IsIdentity)
                m.Multiply(localMatrix);

            m.TransformPoints(transPts);

            //获取尺寸框
            return GetBoundBox(transPts);
        }

        RectangleF GetBoundBox(PointF[] pts)
        {
            PointF left, top, right, bottom;
            left = top = right = bottom = pts[0];

            for (int i = 1; i < pts.Count(); i++)
            {
                if (pts[i].X < left.X)
                    left = pts[i];

                if (pts[i].X > right.X)
                    right = pts[i];

                if (pts[i].Y < top.Y)
                    top = pts[i];

                if (pts[i].Y > bottom.Y)
                    bottom = pts[i];
            }

            RectangleF boundBox = new RectangleF(left.X, top.Y, right.X - left.X, bottom.Y - top.Y);
            return boundBox;
        }

        private void _CreateHitGeometry()
        {
            if (hitGraphicsPath != null)
                hitGraphicsPath.Dispose();

            if (hitPathGeometry != null)
                hitPathGeometry.Dispose();

            switch (scMgr.GraphicsType)
            {
                case GraphicsType.D2D:
                    if (D2DHitGeometry != null)
                        hitPathGeometry = D2DHitGeometry((D2DGraphics)ScMgr.Graphics);
                    else
                        hitPathGeometry = CreateHitGeometryByD2D((D2DGraphics)ScMgr.Graphics);
                    break;
            }
        }

        protected virtual Geometry CreateHitGeometryByD2D(D2DGraphics g)
        {
            IsUseOrgHitGeometry = true;
            RawRectangleF rc = new RawRectangleF(0, 0, Width, Height);
            RectangleGeometry geometry = new RectangleGeometry(D2DGraphics.d2dFactory, rc);
            return geometry;
        }


        public void ComputeTransLastHitGeometry()
        {
            switch (scMgr.GraphicsType)
            {
                case GraphicsType.GDIPLUS:

                    if (hitGraphicsPath == null)
                        return;

                    transLastHitGraphicsPath = (GraphicsPath)hitGraphicsPath.Clone();
                    transLastHitGraphicsPath.Transform(globalMatrix);
                    break;

                case GraphicsType.D2D:

                    if (hitPathGeometry == null)
                        return;

                    SharpDX.Direct2D1.Factory d2dFactory = D2DGraphics.d2dFactory;
                    RawMatrix3x2 m32 = GDIDataD2DUtils.TransMatrixToRawMatrix3x2(globalMatrix);
                    transLastHitPathGeometry = new TransformedGeometry(d2dFactory, hitPathGeometry, m32);
                    break;
            }
        }


        public GraphicsPath CreateTransLastHitGeometryForGDI(Matrix m)
        {
            if (hitGraphicsPath == null)
                return null;

            GraphicsPath transPath = (GraphicsPath)hitGraphicsPath.Clone();
            transPath.Transform(m);
            return transPath;
        }





        public void ReleaseSelfTransformInfo()
        {
            if (globalMatrix != null)
                globalMatrix.Dispose();

            if (transLastHitGraphicsPath != null)
                transLastHitGraphicsPath.Dispose();

            if (transLastHitPathGeometry != null)
                transLastHitPathGeometry.Dispose();

            if (invertGlobalMatrix != null)
                invertGlobalMatrix.Dispose();
        
            globalMatrix = null;
            transLastHitGraphicsPath = null;
            transLastHitPathGeometry = null;
            invertGlobalMatrix = null;
        }


        /// <summary>
        /// 是否为正矩形区域判断
        /// </summary>
        /// <returns></returns>
        bool IsStraightRectArea()
        {
            if (parent == null)
            {
                if (!UsePosttreatmentEffect && !IsUseHitGeometryLayerBound &
                    rotateAngle == 0 && Opacity == 1.0f && scaleY == 1.0f && scaleX == 1.0f)
                {
                    return true;
                }

                return true;
            }

            float[] parentGlobalMatrixElems = parent.GlobalMatrix.Elements;

            if (!UsePosttreatmentEffect && !IsUseHitGeometryLayerBound &&
              (parentGlobalMatrixElems[0] == 1.0f && parentGlobalMatrixElems[1] == 0 &&
              parentGlobalMatrixElems[2] == 0 && parentGlobalMatrixElems[3] == 1.0f &&
              rotateAngle == 0 && Opacity == 1.0f && scaleY == 1.0f && scaleX == 1.0f))
            {
                return true;
            }

            return false;
        }

        #endregion
    
        #region 刷新
        public void Refresh()
        {
            if (scMgr != null && suspendLayoutCount == 0)
            {
                ScMgr.Refresh(drawBox);
            }

            if (ShadowLayer != null)
            {
                ShadowLayer.Refresh();
            }
        }

        public void Invalidate(RectangleF rc)
        {
            if (scMgr != null && suspendLayoutCount == 0)
            {
                rc = TransLocalToGlobal(rc);
                ScMgr.Refresh(rc);
            }
        }

        public void InvalidateGlobalRect(RectangleF globalRect)
        {
            if (scMgr != null && suspendLayoutCount == 0)
            {
                ScMgr.Refresh(globalRect);
            }
        }

        public void Update()
        {
            if (scMgr != null && suspendLayoutCount == 0)
            {
                ScMgr.Update();
            }
        }

        #endregion

        #region 绘制
        public virtual void OnD2DPaint(ScGraphics g)
        {
          
            D2DGraphics d2dGraph = (D2DGraphics)g;

            if (d2dGraph == null)
                return;

            if (BackgroundColor != null && BackgroundColor.Value.A != 0)
            {
                if (BackgroundColor.Value.A == 255)
                {
                    RawColor4 color = GDIDataD2DUtils.TransToRawColor4(BackgroundColor.Value);
                    d2dGraph.RenderTarget.Clear(color);
                }
                else
                {
                    d2dGraph.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
                    RawRectangleF rect = new RawRectangleF(0, 0, Width, Height);
                    RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(BackgroundColor.Value);
                    SolidColorBrush brush = new SolidColorBrush(d2dGraph.RenderTarget, rawColor);
                    d2dGraph.RenderTarget.FillRectangle(rect, brush);
                    brush.Dispose();
                }
            }

            if (D2DPaint != null)           
                D2DPaint(d2dGraph);
        }

        #endregion

        #region 坐标点空间转换
        public PointF TransGlobalToLocal(PointF globalPt)
        {
            if (isComputedStraight == true)
            {
                PointF pt = new PointF(globalPt.X - drawBox.X, globalPt.Y - drawBox.Y);
                return pt;
            }

            PointF[] pts = new PointF[] { globalPt };

            if(invertGlobalMatrix != null)
                invertGlobalMatrix.TransformPoints(pts);

            return pts[0];
        }

        public RectangleF TransGlobalToLocal(RectangleF globalRect)
        {
            PointF[] pts = new PointF[]
           {
                new PointF(globalRect.Left, globalRect.Top),
                new PointF(globalRect.Right, globalRect.Top),
                new PointF(globalRect.Right, globalRect.Bottom),
                new PointF(globalRect.Left, globalRect.Bottom)
           };


            if (isComputedStraight == true)
            {
                for (int i = 0; i < 4; i++)
                {
                    pts[i] = new PointF(pts[i].X - drawBox.X, pts[i].Y - drawBox.Y);
                }
            }
            else
            {
                invertGlobalMatrix.TransformPoints(pts);
            }

            //获取尺寸框
            RectangleF localRect = GetBoundBox(pts);

          
            return localRect;
        }


        public PointF TransLocalToGlobal(PointF localPt)
        {
            if (globalMatrix == null)
                return new PointF(0,0);

            PointF[] pts = new PointF[] { localPt };  
            globalMatrix.TransformPoints(pts);
            return pts[0];
        }

        public RectangleF TransLocalToGlobal(RectangleF localRect)
        {
            PointF[] pts = new PointF[] 
            {
                new PointF(localRect.Left, localRect.Top),
                new PointF(localRect.Right, localRect.Top),
                new PointF(localRect.Right, localRect.Bottom),
                new PointF(localRect.Left, localRect.Bottom)
            };

            globalMatrix.TransformPoints(pts);

            //获取尺寸框
            RectangleF globalRect = GetBoundBox(pts);
            return globalRect;
        }

        /// <summary>
        /// 指示点是否包含在层的HitPathGeometry中
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool FillContainsPoint(PointF pt)
        {
            if (isComputedStraight && IsUseOrgHitGeometry)
            {
                return drawBox.Contains(pt);
            }

            switch (scMgr.GraphicsType)
            {
                case GraphicsType.GDIPLUS:
                    return transLastHitGraphicsPath.IsVisible(pt);

                case GraphicsType.D2D:
                    return transLastHitPathGeometry.FillContainsPoint(GDIDataD2DUtils.TransToRawVector2(pt));
            }

            return false;
        }

        #endregion

        #region 动态效果

        private void VisibleAnim_AnimationEvent(ScAnimation scAnimation)
        {
            Opacity = opacityLinear.GetCurtValue();

            if (opacityLinear.IsStop)
            {
                scAnimation.Stop();
                if(Opacity == 0)
                    visible = false;
            }

            Refresh();
        }

        public void StartVisibleAnim(bool isVisible)
        {
            visableAnim.Stop();

            float stopOpacity = 0;

            if (isVisible == true)
            {
                visible = true;     
                stopOpacity = 1.0f;
            }

            opacityLinear = new ScLinearAnimation(Opacity, stopOpacity, visableAnim);
            visableAnim.Start();
        }


        #endregion

        #region 调试代码

        private void DebugLayer_D2DPaint(D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            RawRectangleF rect = new RawRectangleF(1, 1, Width - 1, Height - 1);
            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.DarkBlue);
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);


            //宽度
            TextFormat textFormat = new TextFormat(D2DGraphics.dwriteFactory, "微软雅黑", 12)
           { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center};
            RawRectangleF rectw = new RawRectangleF(0, Height - 20, Width - 1, Height - 1);
            g.RenderTarget.DrawText(Width.ToString(), textFormat, rectw, brush);

            //高度
            TextFormat textFormat2 = new TextFormat(D2DGraphics.dwriteFactory, "微软雅黑", 12)
            { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center, FlowDirection = SharpDX.DirectWrite.FlowDirection.TopToBottom };
            RawRectangleF recth = new RawRectangleF(Width - 10, 0, Width - 1, Height - 1);
            g.RenderTarget.DrawText(Height.ToString(), textFormat2, recth, brush);


            //全局坐标位置
            PointF pt = parent.TransLocalToGlobal(Location);
            textFormat.ParagraphAlignment = ParagraphAlignment.Near;
            textFormat.TextAlignment = TextAlignment.Justified;
            RawRectangleF rectxy = new RawRectangleF(0, 0, Width - 1, 10);
            g.RenderTarget.DrawText(pt.ToString(), textFormat, rectxy, brush);


            //层类名称
            string mm = this.GetType().Name;

            if (!string.IsNullOrWhiteSpace(Name))
                mm += "(" + Name + ")";
          

            textFormat.ParagraphAlignment = ParagraphAlignment.Center;
            textFormat.TextAlignment = TextAlignment.Center;
            RawRectangleF rectname = new RawRectangleF(0, 0, Width - 1, 10);
            g.RenderTarget.DrawText(mm, textFormat, rectxy, brush);

            g.RenderTarget.DrawRectangle(rect, brush, 1f);



        }

        #endregion

        #region 鼠标事件

        public virtual void SelfMouseEnter(object sender, ScMouseEventArgs e)
        {
            
        }

        #endregion

        #region 事件
        private void ScControl_MouseLeave(object sender)
        {
            if (cursor == null)
                return;

            ScLayer scCon = this.parent;
            for (; scCon != null; scCon = scCon.parent)
            {
                if (scCon.Cursor != null)
                {
                    ScMgr.control.Cursor = scCon.Cursor;
                    return;
                }
            }

            ScMgr.control.Cursor = Cursors.Arrow;
        }
        private void ScControl_MouseEnter(object sender, ScMouseEventArgs e)
        {
            if (cursor == null)
            {
                ScLayer scCon = this.parent;
                for (; scCon != null; scCon = scCon.parent)
                {
                    if (scCon.Cursor != null)
                    {
                        ScMgr.control.Cursor = scCon.Cursor;
                        return;
                    }
                }
            }
            else
            {
                ScMgr.control.Cursor = cursor;
            }
        }
        public void ScMouseMove(ScMouseEventArgs mouseEventArgs)
        {
            if (MouseMove == null || Enable == false || dispose == true)
                return;

            MouseMove(this, mouseEventArgs);
        }

        public void ScMouseLeave()
        {
            if (MouseLeave == null || Enable == false || dispose == true)
                return;

            MouseLeave(this);
        }

        public void ScMouseEnter(ScMouseEventArgs mouseEventArgs)
        {
            if (MouseEnter == null || Enable == false || dispose == true)
                return;

            MouseEnter(this, mouseEventArgs);
        }

        public void ScMouseDown(ScMouseEventArgs mouseEventArgs)
        {
            if (MouseDown == null || Enable == false || dispose == true)
                return;

            MouseDown(this, mouseEventArgs);
        }

        public void ScMouseUp(ScMouseEventArgs mouseEventArgs)
        {
            if (MouseUp == null || Enable == false || dispose == true)
                return;

            MouseUp(this, mouseEventArgs);
        }

        public void ScMouseWheel(ScMouseEventArgs mouseEventArgs)
        {
            if (MouseWheel == null || Enable == false || dispose == true)
                return;

            MouseWheel(this, mouseEventArgs);
        }

        public void ScMouseDoubleClick(ScMouseEventArgs mouseEventArgs)
        {
            if (MouseDoubleClick == null || Enable == false || dispose == true)
                return;

            MouseDoubleClick(this, mouseEventArgs);
        }


        public void ScGotFocus(EventArgs e)
        {
            if (Enable == false || dispose == true)
                return;

            focus = true;

            if (GotFocus != null)
                GotFocus(this, e);
        }

        public void ScLostFocus(EventArgs e)
        {
            if (Enable == false || dispose == true)
                return;

            focus = false;
            if (LostFocus != null)
                LostFocus(this, e);
        }

        public void ScKeyDown(KeyEventArgs e)
        {
            if (KeyDown != null && Enable != false && dispose == false)
                KeyDown(this, e);
        }

        public void ScKeyUp(KeyEventArgs e)
        {
            if (KeyUp != null && Enable != false && dispose == false)
                KeyUp(this, e);
        }

        public void ScCharEvent(char c)
        {
            if (CharEvent != null && Enable != false && dispose == false)
                CharEvent(this, c);
        }

        public void ScImeStringEvent(string imeString)
        {
            if (ImeStringEvent != null && Enable != false && dispose == false)
                ImeStringEvent(this, imeString);
        }


        public void ScSizeChanged(SizeF oldSize)
        {
            foreach (ScLayer clayer in controls)
            {
                clayer.SetDirectionRectByDockType();
            }

            if (SizeChanged == null || dispose == true)
                return;

            SizeChanged(this, oldSize);
        }

        public void ScLocationChanged(PointF oldLocation)
        {
            if (LocationChanged == null || dispose == true)
                return;

            LocationChanged(this, oldLocation);
        }


        public void ScReBulid()
        {
            if (ReBulid == null || dispose == true)
                return;

            ReBulid(this);
        }


        public System.Drawing.Bitmap ScPostTreatmentEffectGDI(System.Drawing.Bitmap backBitmap)
        {
            if (PostTreatmentEffectGDI == null || dispose == true)
                return null;

            return PostTreatmentEffectGDI(this, backBitmap);
        }

        public void ScReleasePostTreatmentEffectGDI(System.Drawing.Bitmap effectBitmap)
        {
            if (ReleasePostTreatmentEffectGDI == null || dispose == true)
                return;

           ReleasePostTreatmentEffectGDI(this, effectBitmap);
        }



      

        public delegate void ScMouseEventHandler(object sender, ScMouseEventArgs e);
        public delegate void ScKeyEventHandler(object sender, KeyEventArgs e);
        public delegate void ScEventHandler(object sender);
        public delegate void ScSizeChangedEventHandler(object sender, SizeF oldSize);
        public delegate void ScLocationChangedEventHandler(object sender, PointF oldLocation);
        public delegate void ImeStringEventHandler(object sender, string imeString);
        public delegate void CharEventHandler(object sender, char c);

        public delegate void ScD2DPaintEventHandler(D2DGraphics g);

        public delegate void ScReBulidEventHandler(object sender);


        public delegate System.Drawing.Bitmap PostTreatmentEffectGDIEventHandler(object sender, System.Drawing.Bitmap backBitmap);
        public delegate void ReleasePostTreatmentEffectGDIEventHandler(object sender, System.Drawing.Bitmap effectBitmap);


        public delegate Geometry CreateHitGeometryD2DEventHandler(D2DGraphics d2dGraphics);
        public event CreateHitGeometryD2DEventHandler D2DHitGeometry = null;

        public event ScReBulidEventHandler  ReBulid;

        public event ScMouseEventHandler MouseDown;

        public event ScMouseEventHandler MouseDoubleClick;
        //
        // 摘要:
        //     在鼠标指针进入控件时发生。
        public event ScMouseEventHandler MouseEnter;
        //
        // 摘要:
        //     在鼠标指针停放在控件上时发生。
        public event ScMouseEventHandler MouseHover;
        //
        // 摘要:
        //     在鼠标指针离开控件时发生。
        public event ScEventHandler MouseLeave;
        //
        // 摘要:
        //     在鼠标指针移到控件上时发生。
        public event ScMouseEventHandler MouseMove;
        //
        // 摘要:
        //     在鼠标指针在控件上并释放鼠标键时发生。
        public event ScMouseEventHandler MouseUp;

        public event ScMouseEventHandler MouseWheel;

        public event ScKeyEventHandler KeyDown;
        public event ScKeyEventHandler KeyUp;

        public event EventHandler GotFocus;
        public event EventHandler LostFocus;

        public event ScD2DPaintEventHandler D2DPaint;

        public event ImeStringEventHandler ImeStringEvent;
        public event CharEventHandler CharEvent;

        public event ScSizeChangedEventHandler SizeChanged;
        public event ScLocationChangedEventHandler LocationChanged;

        public event PostTreatmentEffectGDIEventHandler PostTreatmentEffectGDI;
        public event ReleasePostTreatmentEffectGDIEventHandler ReleasePostTreatmentEffectGDI;

        #endregion
      
    }
}
