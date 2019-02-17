using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public interface IScGraphics
    {
        void SetClip(RectangleF clipRect);
        void ResetClip();

        void TranslateTransform(float dx, float dy);
        void ResetTransform();

        void ReSize(int width, int height);
        
        void BeginDraw();
        void EndDraw();
    }
}
