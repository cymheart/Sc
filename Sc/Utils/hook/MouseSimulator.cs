using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{

    /// <summary>  
    /// And X, Y point on the screen  
    /// </summary>  
    public struct MousePoint
    {

        public MousePoint(Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        public int X;
        public int Y;

        public static implicit operator Point(MousePoint p)
        {
            return new Point(p.X, p.Y);
        }

    }

    /// <summary>  
    /// Mouse buttons that can be pressed  
    /// </summary>  
    public enum MouseButton
    {
        Left = 0x2,
        Right = 0x8,
        Middle = 0x20
    }

    /// <summary>  
    /// Operations that simulate mouse events  
    /// </summary>  
    public static class MouseSimulator
    {

        #region Windows API Code  

        [DllImport("user32.dll")]
        static extern int ShowCursor(bool show);

        [DllImport("user32.dll")]
        static extern void mouse_event(int flags, int dX, int dY, int buttons, int extraInfo);

        const int MOUSEEVENTF_MOVE = 0x1;
        const int MOUSEEVENTF_LEFTDOWN = 0x2;
        const int MOUSEEVENTF_LEFTUP = 0x4;
        const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        const int MOUSEEVENTF_RIGHTUP = 0x10;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        const int MOUSEEVENTF_MIDDLEUP = 0x40;
        const int MOUSEEVENTF_WHEEL = 0x800;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        #endregion

        #region Properties  

        /// <summary>  
        /// Gets or sets a structure that represents both X and Y mouse coordinates  
        /// </summary>  
        public static MousePoint Position
        {
            get
            {
                return new MousePoint(Cursor.Position);
            }
            set
            {
                Cursor.Position = value;
            }
        }

        /// <summary>  
        /// Gets or sets only the mouse's x coordinate  
        /// </summary>  
        public static int X
        {
            get
            {
                return Cursor.Position.X;
            }
            set
            {
                Cursor.Position = new Point(value, Y);
            }
        }

        /// <summary>  
        /// Gets or sets only the mouse's y coordinate  
        /// </summary>  
        public static int Y
        {
            get
            {
                return Cursor.Position.Y;
            }
            set
            {
                Cursor.Position = new Point(X, value);
            }
        }

        #endregion

        #region Methods  

        /// <summary>  
        /// Press a mouse button down  
        /// </summary>  
        /// <param name="button"></param>  
        public static void MouseDown(MouseButton button)
        {
            mouse_event(((int)button), 0, 0, 0, 0);
        }

        public static void MouseDown(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    MouseDown(MouseButton.Left);
                    break;
                case MouseButtons.Middle:
                    MouseDown(MouseButton.Middle);
                    break;
                case MouseButtons.Right:
                    MouseDown(MouseButton.Right);
                    break;
            }
        }

        /// <summary>  
        /// Let a mouse button up  
        /// </summary>  
        /// <param name="button"></param>  
        public static void MouseUp(MouseButton button)
        {
            mouse_event(((int)button) * 2, 0, 0, 0, 0);
        }

        public static void MouseUp(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    MouseUp(MouseButton.Left);
                    break;
                case MouseButtons.Middle:
                    MouseUp(MouseButton.Middle);
                    break;
                case MouseButtons.Right:
                    MouseUp(MouseButton.Right);
                    break;
            }
        }

        /// <summary>  
        /// Click a mouse button (down then up)  
        /// </summary>  
        /// <param name="button"></param>  
        public static void Click(MouseButton button)
        {
            MouseDown(button);
            MouseUp(button);
        }

        public static void Click(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    Click(MouseButton.Left);
                    break;
                case MouseButtons.Middle:
                    Click(MouseButton.Middle);
                    break;
                case MouseButtons.Right:
                    Click(MouseButton.Right);
                    break;
            }
        }

        /// <summary>  
        /// Double click a mouse button (down then up twice)  
        /// </summary>  
        /// <param name="button"></param>  
        public static void DoubleClick(MouseButton button)
        {
            Click(button);
            Click(button);
        }

        public static void DoubleClick(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    DoubleClick(MouseButton.Left);
                    break;
                case MouseButtons.Middle:
                    DoubleClick(MouseButton.Middle);
                    break;
                case MouseButtons.Right:
                    DoubleClick(MouseButton.Right);
                    break;
            }
        }

        /// <summary>  
        /// Show a hidden current on currently application  
        /// </summary>  
        public static void Show()
        {
            ShowCursor(true);
        }

        /// <summary>  
        /// Hide mouse cursor only on current application's forms  
        /// </summary>  
        public static void Hide()
        {
            ShowCursor(false);
        }

        #endregion

    }

}