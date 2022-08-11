using System.Drawing;
using System.Drawing.Imaging;

namespace Andreal.Core.UI;

internal static class StackBlur
{
    private const byte R = 0;
    private const byte G = 1;
    private const byte B = 2;
    private const byte A = 3;

    private static readonly uint[] StackBlur8Mul =
    {
        512, 512, 456, 512, 328, 456, 335, 512, 405, 328, 271, 456, 388, 335, 292, 512, 454, 405, 364, 328, 298,
        271, 496, 456, 420, 388, 360, 335, 312, 292, 273, 512, 482, 454, 428, 405, 383, 364, 345, 328, 312, 298,
        284, 271, 259, 496, 475, 456, 437, 420, 404, 388, 374, 360, 347, 335, 323, 312, 302, 292, 282, 273, 265,
        512, 497, 482, 468, 454, 441, 428, 417, 405, 394, 383, 373, 364, 354, 345, 337, 328, 320, 312, 305, 298,
        291, 284, 278, 271, 265, 259, 507, 496, 485, 475, 465, 456, 446, 437, 428, 420, 412, 404, 396, 388, 381,
        374, 367, 360, 354, 347, 341, 335, 329, 323, 318, 312, 307, 302, 297, 292, 287, 282, 278, 273, 269, 265,
        261, 512, 505, 497, 489, 482, 475, 468, 461, 454, 447, 441, 435, 428, 422, 417, 411, 405, 399, 394, 389,
        383, 378, 373, 368, 364, 359, 354, 350, 345, 341, 337, 332, 328, 324, 320, 316, 312, 309, 305, 301, 298,
        294, 291, 287, 284, 281, 278, 274, 271, 268, 265, 262, 259, 257, 507, 501, 496, 491, 485, 480, 475, 470,
        465, 460, 456, 451, 446, 442, 437, 433, 428, 424, 420, 416, 412, 408, 404, 400, 396, 392, 388, 385, 381,
        377, 374, 370, 367, 363, 360, 357, 354, 350, 347, 344, 341, 338, 335, 332, 329, 326, 323, 320, 318, 315,
        312, 310, 307, 304, 302, 299, 297, 294, 292, 289, 287, 285, 282, 280, 278, 275, 273, 271, 269, 267, 265,
        263, 261, 259
    };

    private static readonly uint[] StackBlur8Shr =
    {
        09, 11, 12, 13, 13, 14, 14, 15, 15, 15, 15, 16, 16, 16, 16, 17, 17, 17, 17, 17, 17, 17, 18, 18, 18, 18, 18,
        18, 18, 18, 18, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 20, 20, 20, 20, 20, 20, 20, 20, 20,
        20, 20, 20, 20, 20, 20, 20, 20, 20, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21,
        21, 21, 21, 21, 21, 21, 21, 21, 21, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22,
        22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 23, 23, 23, 23, 23, 23, 23, 23,
        23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
        23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 24, 24, 24, 24, 24, 24, 24, 24,
        24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
        24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
        24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24
    };

    /// <summary>
    ///     <para>Stack Blur Algorithm by Mario Klingemann (mario@quasimondo.com).</para>
    ///     <para>Modified by TheSnowfield; Implemented in C# by Awbugl.</para>
    /// </summary>
    internal static unsafe void StackBlurRGBA32(Bitmap bitmap, byte round)
    {
        var bitmapData = bitmap.LockBits(new(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,
                                         PixelFormat.Format32bppArgb);

        var width = (uint)bitmapData.Width;
        var height = (uint)bitmapData.Height;
        var dataStride = (uint)bitmapData.Stride;
        var image = (uint*)bitmapData.Scan0.ToPointer();

        var wm = width - 1;
        var hm = height - 1;

        if (round != 0 && width != 0 && height != 0 && dataStride != 0)
        {
            if (round == 255) round = 254;

            dataStride /= 4;

            var mulSum = StackBlur8Mul[round];
            var shrSum = StackBlur8Shr[round];

            var div = (uint)(round + round + 1);

            fixed (uint* stackDataPtr = new uint[div])
            {
                uint x, y = 0;

                uint stackStart, sumR, sumG, sumB, sumA, sumInR, sumInG, sumInB, sumInA, sumOutR, sumOutG, sumOutB,
                     sumOutA;

                uint i, t;
                uint stackPtr;

                byte* srcPixPtr;
                byte* dstPixPtr;
                byte* stackPixPtr;

                uint nRound = round;

                do
                {
                    sumR = sumG = sumB = sumA = sumInR
                        = sumInG = sumInB = sumInA = sumOutR = sumOutG = sumOutB = sumOutA = 0;

                    var rowAddr = y * dataStride;
                    srcPixPtr = (byte*)(image + rowAddr);
                    i = 0;

                    do
                    {
                        t = i + 1;

                        stackPixPtr = (byte*)(stackDataPtr + i);

                        *(stackPixPtr + R) = *(srcPixPtr + R);
                        *(stackPixPtr + G) = *(srcPixPtr + G);
                        *(stackPixPtr + B) = *(srcPixPtr + B);
                        *(stackPixPtr + A) = *(srcPixPtr + A);

                        sumR += *(stackPixPtr + R) * t;
                        sumG += *(stackPixPtr + G) * t;
                        sumB += *(stackPixPtr + B) * t;
                        sumA += *(stackPixPtr + A) * t;

                        sumOutR += *(stackPixPtr + R);
                        sumOutG += *(stackPixPtr + G);
                        sumOutB += *(stackPixPtr + B);
                        sumOutA += *(stackPixPtr + A);

                        if (i <= 0) continue;

                        t = nRound + 1 - i;

                        if (i <= wm) srcPixPtr += 4;

                        stackPixPtr = (byte*)(stackDataPtr + (i + nRound));

                        *(stackPixPtr + R) = *(srcPixPtr + R);
                        *(stackPixPtr + G) = *(srcPixPtr + G);
                        *(stackPixPtr + B) = *(srcPixPtr + B);
                        *(stackPixPtr + A) = *(srcPixPtr + A);

                        sumR += *(stackPixPtr + R) * t;
                        sumG += *(stackPixPtr + G) * t;
                        sumB += *(stackPixPtr + B) * t;
                        sumA += *(stackPixPtr + A) * t;

                        sumInR += *(stackPixPtr + R);
                        sumInG += *(stackPixPtr + G);
                        sumInB += *(stackPixPtr + B);
                        sumInA += *(stackPixPtr + A);
                    } while (++i <= nRound);

                    stackPtr = nRound;
                    var xp = nRound;

                    if (xp > wm) xp = wm;

                    srcPixPtr = (byte*)(image + (xp + rowAddr));
                    dstPixPtr = (byte*)(image + rowAddr);
                    x = 0;

                    do
                    {
                        *(dstPixPtr + R) = (byte)((sumR * mulSum) >> (int)shrSum);
                        *(dstPixPtr + G) = (byte)((sumG * mulSum) >> (int)shrSum);
                        *(dstPixPtr + B) = (byte)((sumB * mulSum) >> (int)shrSum);
                        *(dstPixPtr + A) = (byte)((sumA * mulSum) >> (int)shrSum);

                        dstPixPtr += 4;

                        sumR -= sumOutR;
                        sumG -= sumOutG;
                        sumB -= sumOutB;
                        sumA -= sumOutA;

                        stackStart = stackPtr + div - nRound;

                        if (stackStart >= div) stackStart -= div;

                        stackPixPtr = (byte*)(stackDataPtr + stackStart);

                        sumOutR -= *(stackPixPtr + R);
                        sumOutG -= *(stackPixPtr + G);
                        sumOutB -= *(stackPixPtr + B);
                        sumOutA -= *(stackPixPtr + A);

                        if (xp < wm)
                        {
                            srcPixPtr += 4;
                            ++xp;
                        }

                        *(stackPixPtr + R) = *(srcPixPtr + R);
                        *(stackPixPtr + G) = *(srcPixPtr + G);
                        *(stackPixPtr + B) = *(srcPixPtr + B);
                        *(stackPixPtr + A) = *(srcPixPtr + A);

                        sumInR += *(stackPixPtr + R);
                        sumInG += *(stackPixPtr + G);
                        sumInB += *(stackPixPtr + B);
                        sumInA += *(stackPixPtr + A);

                        sumR += sumInR;
                        sumG += sumInG;
                        sumB += sumInB;
                        sumA += sumInA;

                        if (++stackPtr >= div) stackPtr = 0;

                        stackPixPtr = (byte*)(stackDataPtr + stackPtr);

                        sumOutR += *(stackPixPtr + R);
                        sumOutG += *(stackPixPtr + G);
                        sumOutB += *(stackPixPtr + B);
                        sumOutA += *(stackPixPtr + A);

                        sumInR -= *(stackPixPtr + R);
                        sumInG -= *(stackPixPtr + G);
                        sumInB -= *(stackPixPtr + B);
                        sumInA -= *(stackPixPtr + A);
                    } while (++x < width);
                } while (++y < height);

                var stride = dataStride << 2;
                x = 0;

                do
                {
                    sumR = sumG = sumB = sumA = sumInR
                        = sumInG = sumInB = sumInA = sumOutR = sumOutG = sumOutB = sumOutA = 0;

                    srcPixPtr = (byte*)(image + x);
                    i = 0;

                    do
                    {
                        t = i + 1;

                        stackPixPtr = (byte*)(stackDataPtr + i);

                        *(stackPixPtr + R) = *(srcPixPtr + R);
                        *(stackPixPtr + G) = *(srcPixPtr + G);
                        *(stackPixPtr + B) = *(srcPixPtr + B);
                        *(stackPixPtr + A) = *(srcPixPtr + A);

                        sumR += *(stackPixPtr + R) * t;
                        sumG += *(stackPixPtr + G) * t;
                        sumB += *(stackPixPtr + B) * t;
                        sumA += *(stackPixPtr + A) * t;

                        sumOutR += *(stackPixPtr + R);
                        sumOutG += *(stackPixPtr + G);
                        sumOutB += *(stackPixPtr + B);
                        sumOutA += *(stackPixPtr + A);

                        if (i <= 0) continue;

                        t = nRound + 1 - i;

                        if (i <= hm) srcPixPtr += stride;

                        stackPixPtr = (byte*)(stackDataPtr + (i + nRound));

                        *(stackPixPtr + R) = *(srcPixPtr + R);
                        *(stackPixPtr + G) = *(srcPixPtr + G);
                        *(stackPixPtr + B) = *(srcPixPtr + B);
                        *(stackPixPtr + A) = *(srcPixPtr + A);

                        sumR += *(stackPixPtr + R) * t;
                        sumG += *(stackPixPtr + G) * t;
                        sumB += *(stackPixPtr + B) * t;
                        sumA += *(stackPixPtr + A) * t;

                        sumInR += *(stackPixPtr + R);
                        sumInG += *(stackPixPtr + G);
                        sumInB += *(stackPixPtr + B);
                        sumInA += *(stackPixPtr + A);
                    } while (++i <= nRound);

                    stackPtr = nRound;
                    var yp = nRound;

                    if (yp > hm) yp = hm;

                    srcPixPtr = (byte*)(image + (x + yp * dataStride));
                    dstPixPtr = (byte*)(image + x);
                    y = 0;

                    do
                    {
                        *(dstPixPtr + R) = (byte)((sumR * mulSum) >> (int)shrSum);
                        *(dstPixPtr + G) = (byte)((sumG * mulSum) >> (int)shrSum);
                        *(dstPixPtr + B) = (byte)((sumB * mulSum) >> (int)shrSum);
                        *(dstPixPtr + A) = (byte)((sumA * mulSum) >> (int)shrSum);

                        dstPixPtr += stride;

                        sumR -= sumOutR;
                        sumG -= sumOutG;
                        sumB -= sumOutB;
                        sumA -= sumOutA;

                        stackStart = stackPtr + div - nRound;
                        if (stackStart >= div) stackStart -= div;

                        stackPixPtr = (byte*)(stackDataPtr + stackStart);

                        sumOutR -= *(stackPixPtr + R);
                        sumOutG -= *(stackPixPtr + G);
                        sumOutB -= *(stackPixPtr + B);
                        sumOutA -= *(stackPixPtr + A);

                        if (yp < hm)
                        {
                            srcPixPtr += stride;
                            ++yp;
                        }

                        *(stackPixPtr + R) = *(srcPixPtr + R);
                        *(stackPixPtr + G) = *(srcPixPtr + G);
                        *(stackPixPtr + B) = *(srcPixPtr + B);
                        *(stackPixPtr + A) = *(srcPixPtr + A);

                        sumInR += *(stackPixPtr + R);
                        sumInG += *(stackPixPtr + G);
                        sumInB += *(stackPixPtr + B);
                        sumInA += *(stackPixPtr + A);

                        sumR += sumInR;
                        sumG += sumInG;
                        sumB += sumInB;
                        sumA += sumInA;

                        if (++stackPtr >= div) stackPtr = 0;

                        stackPixPtr = (byte*)(stackDataPtr + stackPtr);

                        sumOutR += *(stackPixPtr + R);
                        sumOutG += *(stackPixPtr + G);
                        sumOutB += *(stackPixPtr + B);
                        sumOutA += *(stackPixPtr + A);

                        sumInR -= *(stackPixPtr + R);
                        sumInG -= *(stackPixPtr + G);
                        sumInB -= *(stackPixPtr + B);
                        sumInA -= *(stackPixPtr + A);
                    } while (++y < height);
                } while (++x < width);
            }
        }

        bitmap.UnlockBits(bitmapData);
    }
}
