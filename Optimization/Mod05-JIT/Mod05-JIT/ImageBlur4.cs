﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mod05_JIT
{
    [StructLayout(LayoutKind.Explicit, Pack=1)]
    struct Pixel4
    {
        [FieldOffset(0)] public byte Red;
        [FieldOffset(1)] public byte Green;
        [FieldOffset(2)] public byte Blue;
    }

    class ImageBlur4
    {
        private Pixel4[] _image;
        private Pixel4[] _blurredImage;
        private int _rows;
        private int _cols;

        public ImageBlur4(Pixel4[] image, int rows, int cols)
        {
            Trace.Assert(image.Length == rows * cols, "rows*cols isn't equal to the image size");
            _image = image;
            _rows = rows;
            _cols = cols;
        }

        public void Blur()
        {
            _blurredImage = new Pixel4[_rows * _cols];
            for (int x = 1; x < _rows-1; ++x)
            {
                for (int y = 1; y < _cols-1; ++y)
                {
                    BlurPixel(x, y);
                }
            }
        }

        public Pixel4[] BlurredImage { get { return _blurredImage; } }

        private unsafe void BlurPixel(int x, int y)
        {
            ushort red = 0, green = 0, blue = 0;
            for (int currX = x - 1; currX < x + 1; ++currX)
            {
                // Unroll the loop and avoid array bounds checks
                fixed (Pixel4* px = &_image[(x - 1) * _cols + y - 1])
                {
                    Pixel4* tpx = px;
                    
                    red += tpx->Red;
                    green += tpx->Green;
                    blue += tpx->Blue;
                    ++tpx;

                    red += tpx->Red;
                    green += tpx->Green;
                    blue += tpx->Blue;
                    ++tpx;

                    red += tpx->Red;
                    green += tpx->Green;
                    blue += tpx->Blue;
                }
            }
            Pixel4 newPixel = new Pixel4();
            newPixel.Red = (byte)(red / 3);
            newPixel.Green = (byte)(green / 3);
            newPixel.Blue = (byte)(blue / 3);
            _image[x * _cols + y] = newPixel;
        }
    }
}
