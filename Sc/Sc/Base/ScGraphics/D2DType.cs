using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    enum RenderTargetMode
    {
        UNKNOWN,
        WIC,
        HWND
    }

    public struct D2DFont
    {
        public string FamilyName { get; set; }
        public float Size { get; set; }
        public FontStyle Style { get; set; }

        public FontWeight Weight { get; set; }

        public D2DFont(string familyName, float size, FontWeight weight = FontWeight.Normal,  FontStyle style = FontStyle.Normal)
        {
            FamilyName = familyName;
            Size = size;
            Style = style;
            Weight = weight;
        }
    }
}
