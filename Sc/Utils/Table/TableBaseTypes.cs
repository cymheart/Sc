using System.Drawing;

namespace Utils
{
    public enum LineDir
    {
        /// <summary>
        /// 水平行线
        /// </summary>
        HORIZONTAL,

        /// <summary>
        /// 垂直列线
        /// </summary>
        VERTICAL,
    }
    public enum LineComputeMode
    {
        /// <summary>
        /// 自动
        /// </summary>
        AUTO,

        /// <summary>
        /// 百分比
        /// </summary>
        PERCENTAGE,

        /// <summary>
        /// 绝对值
        /// </summary>               
        ABSOLUTE,
    }


    public enum DirectionMode
    {
        /// <summary>
        /// 上方或左方
        /// </summary>
        UP_OR_LEFT,

        /// <summary>
        /// 下方或右方
        /// </summary>               
        BOTTOM_OR_RIGHT
    }

    public enum AdjustMode
    {
        /// <summary>
        /// 上调整或左调整
        /// </summary>
        UP_OR_LEFT_ADJUST,

        /// <summary>
        /// 下调整或右调整
        /// </summary>               
        DOWN_OR_RIGHT_ADJUST
    }

    public class Margin
    {
        public Margin(float left, float top, float right, float bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public float left { get; set; }
        public float top { get; set; }
        public float right { get; set; }
        public float bottom { get; set; }
    }


    public class SpanOffset
    {
        public SpanOffset(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public int left { get; set; }
        public int top { get; set; }
        public int right { get; set; }
        public int bottom { get; set; }
    }

    public class CellCoord
    {
        public int rowIdx;
        public int colIdx;
        public CellCoord(int rowIdx, int colIdx)
        {
            this.rowIdx = rowIdx;
            this.colIdx = colIdx;
        }
    }

    public class LinePos
    {
        public PointF start { get; set; }
        public PointF end { get; set; }
    }


    public enum TableDockType
    {
        None,
        LeftTop,
        Top,
        RightTop,
        Left,
        Center,
        Right,
        LeftBottom,
        Bottom,
        RightBottom,
    }


    public enum TableSizeType
    {
        Fixed,
        Fill,
        HorizontalFill,
        VerticalFill,
    }

    public enum CellValueType
    {
        String,
        Image,
        FixedSize,
        Table,
    }

    public class CellValueInfo
    {
        public delegate void ValueProcessHandler(CellValueInfo cellValueInfo);
        public ValueProcessHandler valueProcess;

        public CellValueType type;
        public object value;
        public bool isLimitWidth = false;
        public SizeF permmSizePx;
        public SizeF fixedSize;
        public Font font;
        public Table table;
        public Graphics g;
        public RectangleF rect;

    }

    public class TableLine
    {
        public LineDir lineDir { get; set; }
        public LineComputeMode lineComputeMode { get; set; }
        public float computeParam { get; set; }

        public float computedDistance { get; set; }

        public float maxComputedValue { get; set; }

        public float minComputedValue { get; set; }

        public bool enableAutoAdjustParam = true;

        public TableLine(LineDir lineDir)
        {
            this.lineDir = lineDir;
        }
    }
}
