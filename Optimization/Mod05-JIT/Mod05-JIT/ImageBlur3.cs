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
    struct Pixel3
    {
        [FieldOffset(0)] public byte Red;
        [FieldOffset(1)] public byte Green;
        [FieldOffset(2)] public byte Blue;
    }

    class ImageBlur3
    {
        private Pixel3[] _image;
        private Pixel3[] _blurredImage;
        private int _rows;
        private int _cols;

        public ImageBlur3(Pixel3[] image, int rows, int cols)
        {
            Trace.Assert(image.Length == rows * cols, "rows*cols isn't equal to the image size");
            _image = image;
            _rows = rows;
            _cols = cols;
        }

        public void Blur()
        {
            _blurredImage = new Pixel3[_rows * _cols];
            for (int x = 1; x < _rows-1; ++x)
            {
                for (int y = 1; y < _cols-1; ++y)
                {
                    BlurPixel(x, y);
                }
            }
        }

        public Pixel3[] BlurredImage { get { return _blurredImage; } }

        private void BlurPixel(int x, int y)
        {
            ushort red = 0, green = 0, blue = 0;
            // Attempt to force array bounds check elimination
            Pixel3[] localImage = _image;
            int localCols = _cols;
            for (int currX = x - 1; currX < x + 1; ++currX)
            {
                for (int currY = y - 1; currY < y + 1; ++currY)
                {
                    Pixel3 pixel = localImage[currX * localCols + currY];
                    red += pixel.Red;
                    green += pixel.Green;
                    blue += pixel.Blue;
                }
            }
            Pixel3 newPixel = new Pixel3();
            newPixel.Red = (byte)(red / 3);
            newPixel.Green = (byte)(green / 3);
            newPixel.Blue = (byte)(blue / 3);
            _image[x * _cols + y] = newPixel;
        }
    }
}
