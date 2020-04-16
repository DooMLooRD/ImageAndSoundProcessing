using ImageProcessing.Core.Helpers;
using ImageProcessing.Core.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace ImageProcessing.Core.Segmentation
{
    public unsafe class RegionGrowing
    {
        public List<Bitmap> CreateMasks(Bitmap bitmap, int threshold, int minPixelsInRegion, string saveFolder)
        {
            var imagePixels = new int[bitmap.Height * bitmap.Width];
            var regions = new List<HashSet<int>>();

            using (SingleBitmapData bitmapData = new SingleBitmapData(bitmap, false))
            {
                var seeds = GenerateSeeds(bitmapData);
                AllocateRegions(bitmapData, seeds, threshold, minPixelsInRegion, imagePixels, regions);
                ProcessUnallocatedPixels(bitmapData, imagePixels, regions);
            }

            var colors = GenerateColors(regions);
            var masks = new List<Bitmap>();

            masks.Add(CreateFullMaskImage(bitmap, imagePixels, colors));

            int counter = 1;
            foreach (var region in regions)
            {
                var mask = CreateMaskImage(bitmap, region);
                mask.Save($"{saveFolder}/{counter++}_mask.png");
                masks.Add(mask);
            }

            return masks;
        }

        public Bitmap SetMask(Bitmap originalImage, Bitmap mask)
        {

            using (SingleBitmapData resultbitmapData = new SingleBitmapData(originalImage, PixelFormat.Format24bppRgb))
            using (SingleBitmapData originalbitmapData = new SingleBitmapData(originalImage, false))
            using (SingleBitmapData maskBitmapData = new SingleBitmapData(mask, false))
            {
                for (int i = 0; i < originalbitmapData.HeightInPixels; i++)
                {
                    for (int j = 0; j < originalbitmapData.WidthInBytes; j += originalbitmapData.BytesPerPixel)
                    {
                        var index = i * originalbitmapData.WidthInBytes + j;
                        var resultIndex = 3 * i * originalbitmapData.WidthInBytes + j * 3;
                        var originalColor = originalbitmapData.FirstPixelPtr[index];
                        var maskColor = maskBitmapData.FirstPixelPtr[index];

                        if (maskColor == 0)
                        {
                            resultbitmapData.FirstPixelPtr[resultIndex] = originalColor;
                            resultbitmapData.FirstPixelPtr[resultIndex + 1] = originalColor;
                            resultbitmapData.FirstPixelPtr[resultIndex + 2] = originalColor;
                        }
                        else
                        {
                            resultbitmapData.FirstPixelPtr[resultIndex] = 255;
                            resultbitmapData.FirstPixelPtr[resultIndex + 1] = 0;
                            resultbitmapData.FirstPixelPtr[resultIndex + 2] = 140;
                        }
                    }
                }

                return resultbitmapData.Bitmap;
            }
        }

        private Bitmap CreateMaskImage(Bitmap bitmap, HashSet<int> region)
        {
            using (SingleBitmapData bitmapData = new SingleBitmapData(bitmap, true))
            {
                for (int i = 0; i < bitmapData.HeightInPixels; i++)
                {
                    for (int j = 0; j < bitmapData.WidthInBytes; j += bitmapData.BytesPerPixel)
                    {
                        var index = i * bitmapData.WidthInBytes + j;

                        bitmapData.FirstPixelPtr[index] = (byte)(region.Contains(index) ? 255 : 0);
                    }
                }

                return bitmapData.Bitmap;
            }
        }

        private Bitmap CreateFullMaskImage(Bitmap bitmap, int[] imagePixels, List<(int, int, int)> colors)
        {
            using (SingleBitmapData bitmapData = new SingleBitmapData(bitmap, PixelFormat.Format24bppRgb))
            {
                for (int i = 0; i < bitmapData.HeightInPixels; i++)
                {
                    for (int j = 0; j < bitmapData.WidthInBytes; j += bitmapData.BytesPerPixel)
                    {
                        var index = i * bitmapData.WidthInBytes + j;

                        var currPixel = imagePixels[index / bitmapData.BytesPerPixel] - 1;
                        bitmapData.FirstPixelPtr[index] = (byte)colors[currPixel].Item1;
                        bitmapData.FirstPixelPtr[index + 1] = (byte)colors[currPixel].Item2;
                        bitmapData.FirstPixelPtr[index + 2] = (byte)colors[currPixel].Item3;
                    }
                }

                return bitmapData.Bitmap;
            }
        }


        private List<(int, int, int)> GenerateColors(List<HashSet<int>> regions)
        {
            var rand = new Random();

            var colors = new List<(int, int, int)>();
            for (int i = 0; i < regions.Count; i++)
            {
                colors.Add((rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255)));
            }

            return colors;
        }


        private List<int> GenerateSeeds(SingleBitmapData bitmapData)
        {
            var seeds = new List<int>();

            for (int i = 0; i < bitmapData.HeightInPixels; i++)
            {
                for (int j = 0; j < bitmapData.WidthInBytes; j++)
                {
                    if (i % 2 == 0 && j % 2 == 0)
                    {
                        seeds.Add(i * bitmapData.WidthInBytes + j);
                    }
                }
            }

            CollectionHelper.Shuffle(seeds);

            return seeds;
        }

        private void AllocateRegions(
            SingleBitmapData bitmapData,
            List<int> seeds,
            int threshold,
            int minPixelsInRegion,
            int[] imagePixels,
            List<HashSet<int>> regions)
        {
            int counter = 1;
            for (int i = 0; i < seeds.Count; i++)
            {
                var seed = seeds[i];

                if (imagePixels[seed] != 0)
                {
                    continue;
                }

                var stack = new Stack<int>();
                var region = new HashSet<int>();
                var visited = new HashSet<int>();

                var regionMin = bitmapData.FirstPixelPtr[seed];
                var regionMax = bitmapData.FirstPixelPtr[seed];

                stack.Push(seed);
                region.Add(seed);
                visited.Add(seed);

                while (stack.Count > 0)
                {
                    var currentPixel = stack.Pop();
                    var neighbourhood = ImageHelper.GetNeighbourhood(bitmapData, currentPixel, visited, imagePixels);

                    foreach (var neighbour in neighbourhood)
                    {
                        visited.Add(neighbour);
                        var neighbourValue = bitmapData.FirstPixelPtr[neighbour];
                        var min = regionMin > neighbourValue ? neighbourValue : regionMin;
                        var max = regionMax < neighbourValue ? neighbourValue : regionMax;

                        if (max - min < threshold)
                        {
                            stack.Push(neighbour);
                            region.Add(neighbour);
                            regionMin = min;
                            regionMax = max;
                        }
                    }
                }

                if (region.Count > minPixelsInRegion)
                {
                    foreach (var item in region)
                    {
                        imagePixels[item] = counter;
                    }

                    regions.Add(region);
                    counter++;
                }
            }
        }

        private void ProcessUnallocatedPixels(SingleBitmapData bitmapData, int[] imagePixels, List<HashSet<int>> regions)
        {
            var unallocatedCount = imagePixels.Count(c => c == 0);
            while (unallocatedCount != 0)
            {
                for (int i = 0; i < imagePixels.Length; i++)
                {
                    if (imagePixels[i] == 0)
                    {
                        var currentValue = bitmapData.FirstPixelPtr[i];
                        var neighbourhood = ImageHelper.GetProcessedNeighbourhood(bitmapData, i, imagePixels);
                        var closestRegion = 0;
                        var closestDiff = 255;

                        foreach (var neighbour in neighbourhood)
                        {
                            var neighbourValue = bitmapData.FirstPixelPtr[neighbour];
                            var currDiff = Math.Abs(currentValue - neighbourValue);

                            if (closestDiff > currDiff)
                            {
                                closestRegion = imagePixels[neighbour];
                                closestDiff = currDiff;
                            }
                        }

                        if (closestRegion != 0)
                        {
                            regions[closestRegion - 1].Add(i);
                        }

                        imagePixels[i] = closestRegion;
                    }
                }

                unallocatedCount = imagePixels.Count(c => c == 0);
            }
        }
    }
}
