using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScShadow :ScLayer
    {
        

        public float CornersRadius = 5;
        public float ShadowRadius = 5;
        public Color ShadowColor = Color.Black;

        string key;

        public ScShadow(ScMgr scmgr = null)
            :base(scmgr)
        {
            IsHitThrough = true; 
            D2DPaint += ScShadow_D2DPaint;
        }

        private void ScShadow_D2DPaint(D2DGraphics g)
        {
            Dot9BitmapD2D dot9Bitmapshadow = GetDot9BitmapShadow();

            if (dot9Bitmapshadow == null)
                return;

            dot9Bitmapshadow.ComputeBitmapStretch((int)Width, (int)Height);
            dot9Bitmapshadow.DrawTo(g);
        }

        Dot9BitmapD2D GetDot9BitmapShadow()
        {
            if (Width == 0 || Height == 0)
                return null;

            float n = ShadowRadius * 4 + CornersRadius * 2;

            if (Width <= n || Height <= n)
                return null;


            key = CornersRadius.ToString() + ShadowRadius.ToString() + ShadowColor.ToString();
            Dot9BitmapD2D dot9BitmapShadow;

            if (ScMgr.dot9BitmaShadowDict.ContainsKey(key))
            {
                dot9BitmapShadow = ScMgr.dot9BitmaShadowDict[key];
            }
            else
            {
                dot9BitmapShadow = Dot9BitmapD2D.CreateDot9BoxShadowBitmap((D2DGraphics)ScMgr.Graphics, CornersRadius, ShadowRadius, ShadowColor);
                ScMgr.dot9BitmaShadowDict.Add(key, dot9BitmapShadow);
            }

            return dot9BitmapShadow;
        }
    }
}
