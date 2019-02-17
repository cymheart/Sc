using System;
using System.Drawing;
using System.Drawing.Imaging;

public enum BlurType
{
    Both,
    HorizontalOnly,
    VerticalOnly,
}
public class GaussianBlur
{
    private int _radius = 1;
    private int[] _kernel;
    private int _kernelSum;
    private int[,] _multable;
    private BlurType _blurType;

    public GaussianBlur()
    {
        PreCalculateSomeStuff();
    }

    public GaussianBlur(int radius)
    {
        _radius = radius;
        PreCalculateSomeStuff();
    }

    //预计算0~256个alpha数值radius长度步距为1的x^2分布的所有值
    //此处用x^2分布近似正态分布来计算像素的权重比值
    private void PreCalculateSomeStuff()
    {
        int sz = _radius * 2 + 1;
        _kernel = new int[sz];
        _multable = new int[sz, 256];

        for (int i = 1; i <= _radius; i++)
        {
            int szi = _radius - i;
            int szj = _radius + i;
            _kernel[szj] = _kernel[szi] = (szi + 1) * (szi + 1);
            _kernelSum += (_kernel[szj] + _kernel[szi]);
            for (int j = 0; j < 256; j++)
            {
                _multable[szj, j] = _multable[szi, j] = _kernel[szj] * j;
            }
        }

        _kernel[_radius] = (_radius + 1) * (_radius + 1);
        _kernelSum += _kernel[_radius];

        for (int j = 0; j < 256; j++)
        {
            _multable[_radius, j] = _kernel[_radius] * j;
        }
    }

    public Bitmap ProcessImage(Image srcBitmap, BlurType blurType = BlurType.Both, Rectangle? notBlurArea = null)
    {
        return ProcessBitmap((Bitmap)srcBitmap, blurType, notBlurArea);
    }

    public Bitmap ProcessBitmap(Bitmap srcBitmap, BlurType blurType = BlurType.Both, Rectangle? notBlurArea = null)
    {
        Bitmap blurBitmap;

        switch (blurType)
        {
            case BlurType.HorizontalOnly:
            case BlurType.VerticalOnly:
                blurBitmap = ProcessBitmapComponents(srcBitmap, blurType, notBlurArea);
                return blurBitmap;

            case BlurType.Both:
                Bitmap blurBitmap2;
                blurBitmap = ProcessBitmapComponents(srcBitmap, BlurType.HorizontalOnly, notBlurArea);
                blurBitmap2 = ProcessBitmapComponents(blurBitmap, BlurType.VerticalOnly, notBlurArea);
                blurBitmap.Dispose();
                return blurBitmap2;
        }

        return null;
    }


    public Bitmap ProcessBitmapComponents(Bitmap srcBitmap, BlurType blurType, Rectangle? notBlurArea)
    {
        unsafe
        {
            Bitmap dstBitmap = new Bitmap(srcBitmap.Width, srcBitmap.Height);

            int width = srcBitmap.Width;
            int height = srcBitmap.Height;

            Rectangle bitmapRect = new Rectangle(0, 0, width, height);

            //将Bitmap锁定到系统内存中,获得BitmapData
            BitmapData srcBmData = srcBitmap.LockBits(bitmapRect,
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


            //将Bitmap锁定到系统内存中,获得BitmapData
            BitmapData dstBmData = dstBitmap.LockBits(bitmapRect,
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            byte* srcScan = (byte*)(void*)(srcBmData.Scan0);
            byte* dstScan = (byte*)(void*)(dstBmData.Scan0);

            //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
            byte* srcPtr = srcScan;
            byte* dstPtr = dstScan;
            int pos;


            if (notBlurArea == null)
            {
                if (blurType == BlurType.HorizontalOnly)
                {
                    for (int i = 0; i < height; i++)
                    {
                        pos = i * dstBmData.Stride;

                        for (int j = 0; j < width; j++)
                        {
                            *((int*)(dstPtr + pos)) = ComputeBlurColorHorizontal(srcBmData, i, j);
                            pos += 4;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < height; i++)
                    {
                        pos = i * dstBmData.Stride;

                        for (int j = 0; j < width; j++)
                        {
                            *((int*)(dstPtr + pos)) = ComputeBlurColorVertical(srcBmData, i, j);
                            pos += 4;
                        }
                    }
                }
            }
            else
            {
                Rectangle rc = notBlurArea.Value;

                if (blurType == BlurType.HorizontalOnly)
                {
                    for (int i = 0; i < height; i++)
                    {
                        pos = i * dstBmData.Stride;

                        for (int j = 0; j < width; j++)
                        {
                            if (j >= rc.Left && j <= rc.Right &&
                                i >= rc.Top && i <= rc.Bottom)
                            {
                                pos += 4;
                                continue;
                            }

                            *((int*)(dstPtr + pos)) = ComputeBlurColorHorizontal(srcBmData, i, j);
                            pos += 4;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < height; i++)
                    {
                        pos = i * dstBmData.Stride;

                        for (int j = 0; j < width; j++)
                        {
                            if (j >= rc.Left && j <= rc.Right &&
                                i >= rc.Top && i <= rc.Bottom)
                            {
                                pos += 4;
                                continue;
                            }

                            *((int*)(dstPtr + pos)) = ComputeBlurColorVertical(srcBmData, i, j);
                            pos += 4;
                        }
                    }
                }
            }

            srcBitmap.UnlockBits(srcBmData);
            dstBitmap.UnlockBits(dstBmData);

            return dstBitmap;
        }
    }



    int ComputeBlurColorHorizontal(BitmapData srcBmData, int i, int j)
    {
        unsafe
        {
            byte* srcScan = (byte*)(void*)(srcBmData.Scan0);
            int pos;

            int totalSum = 0;
            int alphaSum = 0;
            byte a, r, g, b;
            int m = 0;

            int asum = 0;
            int rsum = 0;
            int gsum = 0;
            int bsum = 0;

            int start = j - Radius;

            if (start < 0)
            {
                start = 0;
                m = Radius - j;
                pos = i * srcBmData.Stride;
            }
            else
            {
                pos = i * srcBmData.Stride + start * 4;
            }
      

            int end = j + Radius;
            if (end > srcBmData.Width)
                end = srcBmData.Width;

            for (int n = start; n < end; n++)
            {
                b = *(srcScan + pos);
                g = *(srcScan + pos + 1);
                r = *(srcScan + pos + 2);
                a = *(srcScan + pos + 3);
                pos += 4;

                asum += _multable[m, a];
                alphaSum += _kernel[m];

                if (a != 0)
                {
                    rsum += _multable[m, r];
                    gsum += _multable[m, g];
                    bsum += _multable[m, b];
                    totalSum += _kernel[m];
                }
                m++;
            }

            if(asum == 0)
                return 0;

            a = (byte)(asum / alphaSum);
            r = (byte)(rsum / totalSum);
            g = (byte)(gsum / totalSum);
            b = (byte)(bsum / totalSum);


       
            int color = a << 24 | r << 16 | g << 8 | b;
            return color;
        }
    }



    int ComputeBlurColorVertical(BitmapData srcBmData, int i, int j)
    {
        unsafe
        {
            byte* srcScan = (byte*)(void*)(srcBmData.Scan0);
            int pos = i * srcBmData.Stride + 4 * j;

            int totalSum = 0;
            int alphaSum = 0;
            byte a, r, g, b;
            int m = 0;

            int asum = 0;
            int rsum = 0;
            int gsum = 0;
            int bsum = 0;

            int start = i - Radius;

            if (start < 0)
            {
                start = 0;
                m = Radius - i;
                pos = 4 * j;
            }
            else
            {
                pos = start * srcBmData.Stride + 4 * j;
            }

            int end = i + Radius;
            if (end > srcBmData.Height)
                end = srcBmData.Height;

            for (int n = start; n < end; n++)
            {
                b = *(srcScan + pos);
                g = *(srcScan + pos + 1);
                r = *(srcScan + pos + 2);
                a = *(srcScan + pos + 3);
                pos += srcBmData.Stride;

                asum += _multable[m, a];
                alphaSum += _kernel[m];

                if (a != 0)
                {
                    rsum += _multable[m, r];
                    gsum += _multable[m, g];
                    bsum += _multable[m, b];
                    totalSum += _kernel[m];
                }

                m++;
            }

            if (asum == 0)
                return 0;

            a = (byte)(asum / alphaSum);
            r = (byte)(rsum / totalSum);
            g = (byte)(gsum / totalSum);
            b = (byte)(bsum / totalSum);

            int color = a << 24 | r << 16 | g << 8 | b;
            return color;
        }
    }


    public int Radius
    {
        get { return _radius; }
        set
        {
            if (value < 1)
            {
                throw new InvalidOperationException("Radius must be greater then 0");
            }
            _radius = value;

        }
    }

    public BlurType BlurType
    {
        get { return _blurType; }
        set
        {
            _blurType = value;
        }
    }

}

