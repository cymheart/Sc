using SharpDX;

namespace Sc
{
    /// <summary>
    /// A Color to be used by the <see cref="CustomTextRenderer"/>.
    /// </summary>
    public class ColorDrawingEffect : ComObject
    {
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public Color4 Color { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorDrawingEffect"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public ColorDrawingEffect(Color4 color)
        {
            Color = color;
        }
    }
}