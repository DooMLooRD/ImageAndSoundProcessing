﻿using ImageProcessing.Core.Helpers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace ImageProcessing.Core.Model
{
    public unsafe class ImageComponents
    {
        public Complex[][] ComplexData { get; set; }

        public Bitmap Image { get; set; }
        public Bitmap PhaseImage { get; set; }
        public Bitmap MagnitudeImage { get; set; }

        public ImageComponents(Complex[][] complexData, Bitmap originalImage)
        {
            ComplexData = complexData;
            Image = originalImage.Clone(new Rectangle(0, 0, originalImage.Width, originalImage.Height), originalImage.PixelFormat);
            FillResultImages(originalImage);
        }

        private void FillResultImages(Bitmap bitmap)
        {
            var phaseInterval = MathHelper.CalculateMinMax(ComplexData, Spectrum.Phase);
            var magnitudeInterval = MathHelper.CalculateMinMax(ComplexData, Spectrum.Magnitude);

            FourierHelper.SwapQuadrants(ComplexData);

            using (var phaseBitmapData = new SingleBitmapData(bitmap, PixelFormat.Format8bppIndexed))
            using (var magnitudeBitmapData = new SingleBitmapData(bitmap, PixelFormat.Format8bppIndexed))
            {
                for (int y = 0; y < phaseBitmapData.HeightInPixels; y++)
                {
                    for (int x = 0;
                        x < phaseBitmapData.WidthInBytes;
                        x += phaseBitmapData.BytesPerPixel)
                    {
                        var index = x + y * phaseBitmapData.WidthInBytes;
                        phaseBitmapData.FirstPixelPtr[index] = (byte)FourierHelper.LogNormalize(ComplexData[y][x].Phase, phaseInterval.Max, 255);
                        magnitudeBitmapData.FirstPixelPtr[index] = (byte)FourierHelper.LogNormalize(ComplexData[y][x].Magnitude, magnitudeInterval.Max, 255);
                    }
                }

                PhaseImage = phaseBitmapData.Bitmap;
                MagnitudeImage = magnitudeBitmapData.Bitmap;
            }

            FourierHelper.SwapQuadrants(ComplexData);
        }
    }
}
