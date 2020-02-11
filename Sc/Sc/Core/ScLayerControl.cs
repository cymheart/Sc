using System;
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


using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ScLayerControl : Control
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
        ScMgr scMgr;

        public delegate void ImeStringEventHandler(string imeString);
        public event ImeStringEventHandler ImeStringEvent;

        public delegate void CharEventHandler(char c);
        public event CharEventHandler CharEvent;

        public delegate void DirectionKeyEventHandler(object sender, KeyEventArgs e);
        public event DirectionKeyEventHandler DirectionKeyEvent;

        public ScLayerControl(ScMgr scMgr)
        {
            this.scMgr = scMgr;



            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.

             if(scMgr.GraphicsType == GraphicsType.GDIPLUS)
                 SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲

            m_hImc = ImmGetContext(this.Handle);         
        }

        public void SetImeWindowsPos(int x, int y)
        {
            COMPOSITIONFORM Composition = new COMPOSITIONFORM();
            Composition.dwStyle = CFS_POINT;
            Composition.ptCurrentPos.x = x;
            Composition.ptCurrentPos.y = y;

            ImmSetCompositionWindow(m_hImc, ref Composition);
        }
   
        protected override void OnPaint(PaintEventArgs e)
        {
            scMgr.Paint(e);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }

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
    }
}
