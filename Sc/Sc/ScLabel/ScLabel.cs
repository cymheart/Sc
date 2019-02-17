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
using Utils;

namespace Sc
{
    public class ScLabel: ScLayer
    {
        public string Text = null;
        public object Value = null;
        public Color ForeColor = Color.Black;
        D2DFont foreFont;

        public TextAlignment Alignment = TextAlignment.Center;

        public Margin TextPadding = new Margin(0, 0, 0, 0);

        public ScLabel(ScMgr scmgr)
            :base(scmgr)
        {
            foreFont = new D2DFont("微软雅黑", 12, SharpDX.DirectWrite.FontWeight.Regular);
            D2DPaint += ScLabel_D2DPaint;
        }

        public D2DFont ForeFont
        {
            get
            {
                return foreFont;
            }
            set
            {
                foreFont = value;
            }
        }

   
        private void ScLabel_D2DPaint(D2DGraphics g)
        {

            RawRectangleF rect = new RawRectangleF(TextPadding.left, TextPadding.top, Width - TextPadding.left - TextPadding.right, Height - TextPadding.top - TextPadding.bottom);

            if (!string.IsNullOrWhiteSpace(Text))
            {
                SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, GDIDataD2DUtils.TransToRawColor4(ForeColor));
                TextFormat textFormat = new TextFormat(D2DGraphics.dwriteFactory, foreFont.FamilyName, foreFont.Weight, foreFont.Style, foreFont.Size)
                { TextAlignment = Alignment, ParagraphAlignment = ParagraphAlignment.Center };

                textFormat.WordWrapping = WordWrapping.Wrap;

                g.RenderTarget.DrawText(Text, textFormat, rect, brush, DrawTextOptions.Clip);
                brush.Dispose();
                textFormat.Dispose();
            }
        }
    }
}
