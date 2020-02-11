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


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public partial class UpdateLayerFrm : Form
    {
        #region api
        public const int WM_NCVATIVATE = 0x86;
        public const int WM_NCPAINT = 0x0085;
        public const int WM_NCCALCSIZE = 0x83;
        public const int WM_NCHITTEST = 0X84;
        public const int WM_CHAR = 0x102;
        public const int GCS_RESULTSTR = 0x800;
        public const int WM_IME_SETCONTEXT = 0x281;
        public const int WM_IME_CHAR = 0x286;
        private const int WM_IME_COMPOSITION = 0x010F;
        private const int GCS_COMPSTR = 0x0008;
        private const int HC_ACTION = 0;
        private const int PM_REMOVE = 0x0001;

        private const int CFS_DEFAULT = 0x0000;
        private const int CFS_FORCE_POSITION = 0x0020;
        private const int CFS_POINT = 0x0002;
        private const int CFS_RECT = 0x0001;

        public struct POINT
        {
            public int x;
            public int y;
        }

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public struct COMPOSITIONFORM
        {
            public uint dwStyle;
            public POINT ptCurrentPos;
            public RECT rcArea;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr handle);
        [DllImport("imm32.dll")]
        static extern IntPtr ImmGetContext(IntPtr handle);
        [DllImport("imm32.dll")]
        static extern bool ImmReleaseContext(IntPtr handle, IntPtr imc);


        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        static extern int ImmGetCompositionStringW(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);

        [DllImport("imm32.dll")]
        static extern int ImmAssociateContext(IntPtr handle, IntPtr imeHandle);

        [DllImport("imm32.dll")]
        static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompForm);
        #endregion

        IntPtr m_hImc;
        public ScMgr scMgr;
        public Bitmap bitmap;

        public delegate void ImeStringEventHandler(string imeString);
        public event ImeStringEventHandler ImeStringEvent;

        public delegate void CharEventHandler(char c);
        public event CharEventHandler CharEvent;

        public delegate void DirectionKeyEventHandler(object sender, KeyEventArgs e);
        public event DirectionKeyEventHandler DirectionKeyEvent;

        Color translatorColor = Color.FromArgb(255,255,0,255);
        public UpdateLayerFrm()
        {
            InitializeComponent();

            //减少闪烁
            SetStyles();

            //初始化
            Init();
        }

        public new void Show()
        {
            if(scMgr != null)  
                scMgr.Refresh();

            base.Show();
        }


        #region 初始化
        private void Init()
        {
            //无边框模式
            FormBorderStyle = FormBorderStyle.None;
            //自动拉伸背景图以适应窗口
            BackgroundImageLayout = ImageLayout.Stretch;
            BackgroundImage = null;
            m_hImc = ImmGetContext(this.Handle);

        }

        #endregion

        #region 减少闪烁
        private void SetStyles()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer, true);
            //强制分配样式重新应用到控件上
            UpdateStyles();
            base.AutoScaleMode = AutoScaleMode.None;
        }
        #endregion

        #region 还原任务栏右键菜单
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cParms = base.CreateParams;
                cParms.ExStyle |= 0x00080000; // WS_EX_LAYERED
                return cParms;
            }
        }
  
        #endregion

        #region 不规则无毛边方法
        public void SetBits()
        {
            if (bitmap != null)
            {
                //绘制绘图层背景        
                //if (!Bitmap.IsCanonicalPixelFormat(bitmap.PixelFormat) || !Bitmap.IsAlphaPixelFormat(bitmap.PixelFormat))
                //    throw new ApplicationException("图片必须是32位带Alhpa通道的图片。");

                IntPtr oldBits = IntPtr.Zero;
                IntPtr screenDC = Win32.GetDC(IntPtr.Zero);
                IntPtr hBitmap = IntPtr.Zero;
                IntPtr memDc = Win32.CreateCompatibleDC(screenDC);

                try
                {
                    Win32.Point topLoc = new Win32.Point(Left, Top);
                    Win32.Size bitMapSize = new Win32.Size(Width, Height);
                    Win32.BLENDFUNCTION blendFunc = new Win32.BLENDFUNCTION();
                    Win32.Point srcLoc = new Win32.Point(0, 0);

                    hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                    oldBits = Win32.SelectObject(memDc, hBitmap);

                    blendFunc.BlendOp = Win32.AC_SRC_OVER;
                    blendFunc.SourceConstantAlpha = Byte.Parse(255.ToString());
                    blendFunc.AlphaFormat = Win32.AC_SRC_ALPHA;
                    blendFunc.BlendFlags = 0;

                    Win32.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, Win32.ULW_ALPHA);
                }
                finally
                {
                    if (hBitmap != IntPtr.Zero)
                    {
                        Win32.SelectObject(memDc, oldBits);
                        Win32.DeleteObject(hBitmap);      
                    }
                    Win32.ReleaseDC(IntPtr.Zero, screenDC);
                    Win32.DeleteDC(memDc);
                }
            }
        }
        #endregion

        #region 重载背景与拉伸更改时事件
        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            base.OnBackgroundImageChanged(e);
            SetBits();
        }

        //protected override void OnResize(EventArgs e)
        //{    
        //    base.OnResize(e);
        //    //SetBits();
        //}
        #endregion



        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    KeyEventArgs keyEvent = new KeyEventArgs(keyData);
                    DirectionKeyEvent(this, keyEvent);
                    return true;
            }

            return base.ProcessDialogKey(keyData);
        }


        public void SetImeWindowsPos(int x, int y)
        {
            COMPOSITIONFORM Composition = new COMPOSITIONFORM();
            Composition.dwStyle = CFS_POINT;
            Composition.ptCurrentPos.x = x;
            Composition.ptCurrentPos.y = y;

            ImmSetCompositionWindow(m_hImc, ref Composition);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_IME_SETCONTEXT && m.WParam.ToInt32() == 1)
            {
                ImmAssociateContext(this.Handle, m_hImc);
            }
            switch (m.Msg)
            {
                case WM_CHAR:
                    KeyEventArgs e = new KeyEventArgs(((Keys)((int)((long)m.WParam))) | ModifierKeys);
                    char a = (char)e.KeyData; //英文 

                    if ((a >= 32) && (a <= 255))
                        CharEvent(a);
                    break;

                case WM_IME_COMPOSITION:

                    if ((m.LParam.ToInt32() & GCS_RESULTSTR) != 0)
                    {
                        int size = ImmGetCompositionStringW(m_hImc, GCS_RESULTSTR, null, 0);

                        byte[] buffer = new byte[size];
                        ImmGetCompositionStringW(m_hImc, GCS_RESULTSTR, buffer, size);
                        string str = System.Text.Encoding.Unicode.GetString(buffer);
                        ImeStringEvent(str);
                    }
                    break;
            }
        }


        new public void Invalidate(Rectangle rc, bool invalidateChildren)
        {
            scMgr.PaintToBitmap(rc);
            SetBits();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            scMgr.Paint(e);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }
    }
}
