using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;

namespace Window
{
    static partial class HeatMap
    {
        private static float _brushStop = 0.8F;
        private static int _heatPointRadius = 2;
        //private static byte _intensity = 20;//0x5f;
        static int[] palette;//画板
        public static void Init()
        {
            bmpMask = new Bitmap(xGirds, yGirds, PixelFormat.Format32bppArgb);
            // Loads the palette.
            palette = loadPalette();
          
            //threadOutHeapmap = new Thread(outHeatmap);
            //threadOutHeapmap.Start();
            //outHeatmapInit();
        }
        static Thread threadOutHeapmap;
        private static readonly Random _rand = new Random();
        //static bool outHeatmapOnce=false;
        public static void outHeapmapForPool()
        {
            //ThreadPool.QueueUserWorkItem(outHeatmap);
            outHeatmap();
            //Thread t = new Thread(outHeatmap);
            //t.Start();
        }
        public static float densityColorFactor = 3;
        public static float maxColorDensity = 0;
        public static void calculateColorFactor(float maxcolor)
        {
            // 12个网格的count/(每个网格面积*12)=密度
            // maxColor*系数1=240
            // 12个网格的count/(每个网格面积*12)*系数1=240;
            // densityColorFactor=1/(每个网格面积*12)*系数1
            var factor1 = 240 / maxcolor;//最大颜色设置成200
            var _12个网格面积 = 12* cellWidth * cellWidth / 100;
            densityColorFactor = factor1/ _12个网格面积;
            maxColorDensity = maxcolor;
        }
        public static Bitmap outHeatmapFromQueue()
        {
            int hcount = 0;
            Bitmap bmpNew;
            if (mapData!=null)
            {
                //outHeatmapOnce = false;
                bmpMask = new Bitmap(xGirds, yGirds, PixelFormat.Format32bppArgb);
                // Create new graphics surface from the bitmap.
                Graphics surface = Graphics.FromImage(bmpMask);
                ColorBlend colors;
                for (int i = 0; i < mapData.Length; i++)
                {
                    for (int j = 0; j < mapData[i].Length; j++)
                    {
                        var p = mapData[i][j].count;

                        colors = getColorBlend((int)(p * densityColorFactor));

                        hcount++;
                        if(p>0)drawHeatPoint(surface, i, j, colors, _heatPointRadius);
                    }
                }
                bmpNew = doColorize(bmpMask);
                return bmpNew;
                //bmp = bmpMask;//doColorize(bmpMask);
            }
            return null;
        }

        public static void outHeatmap()
        {
            int hcount = 0;
            if (mapData != null)
            {
                //outHeatmapOnce = false;
                bmpMask = new Bitmap(xGirds, yGirds, PixelFormat.Format32bppArgb);
                // Create new graphics surface from the bitmap.
                Graphics surface = Graphics.FromImage(bmpMask);
                ColorBlend colors;
                for (int i = 0; i < mapData.Length; i++)
                {
                    for (int j = 0; j < mapData[i].Length; j++)
                    {
                        var p = mapData[i][j].count;

                        colors = getColorBlend((int)(p * densityColorFactor));

                        hcount++;
                        if (p > 0) drawHeatPoint(surface, i, j, colors, _heatPointRadius);
                    }
                }
                bmp = doColorize(bmpMask);
                //bmp = bmpMask;//doColorize(bmpMask);
            }
        }


        static Bitmap bmpMask;
        public static Bitmap bmp;
        private static void doDrawMask(List<Point> heatPoints)
        {
            //bmpMask = new Bitmap(xGirds, yGirds, PixelFormat.Format32bppArgb);
            //// Create new graphics surface from the bitmap.
            //Graphics surface = Graphics.FromImage(bmpMask);

            //ColorBlend colors = getColorBlend();

            //// Draw mask heat points on the surface.
            //foreach (Point heatPoint in heatPoints)
            //{
            //    drawHeatPoint(surface, heatPoint, colors, _heatPointRadius);
            //}
        }

        private static void drawHeatPoint(Graphics surface,int X,int Y,
            ColorBlend colors, int radius)
        {
            // Create the ellipse path.
            var ellipsePath = new GraphicsPath();
            ellipsePath.AddEllipse(X - radius, Y - radius,
                radius*2, radius*2);

            // Create the brush.
            PathGradientBrush brush = new PathGradientBrush(ellipsePath);
            ColorBlend gradientSpecifications = colors;
            brush.InterpolationColors = gradientSpecifications;

            //// Use the brush to fill the ellipse.
            //surface.FillEllipse(brush, X - radius,
            //    Y - radius, radius*2, radius*2);

            // Use the brush to fill the ellipse.
            surface.FillRectangle(brush, X - radius,
                Y - radius, radius * 2, radius * 2);
            //Pen pen = new Pen(Color.Red,1f);
            //surface.DrawRectangle(pen, X - radius,
            //    Y - radius, radius * 2, radius * 2);
        }

        private static ColorBlend getColorBlend(int _intensity)
        {
            if (_intensity > 255) _intensity = 255;
            ColorBlend colors = new ColorBlend(3);

            // Set brush stops.
            colors.Positions = new float[3] { 0, _brushStop, 1 };

            // The intensity value adjusts alpha of gradient colors.
            colors.Colors = new Color[3]
            {
                Color.FromArgb(0, Color.White),
                // The following colors can be any color - Only the alpha  value is used.
                Color.FromArgb(_intensity, Color.Black),
                Color.FromArgb(_intensity, Color.Black)
            };
            return colors;
        }

        private static Bitmap doColorize(Bitmap originalMask)
        {
           
           
            try
            {
                // Create an empty bitmap for output.
                Bitmap output = new Bitmap(originalMask.Width, originalMask.Height,
           PixelFormat.Format32bppArgb);
                var touming = Color.FromArgb(0,0,0,0);
                for (int y = 0; y < originalMask.Height; y++)
                {
                    for (int x = 0; x < originalMask.Width; x++)
                    {
                        // Calucate the pixel of output image according to the original pixel and palette.
                        if (originalMask.GetPixel(x, y)!= touming)
                        {
                            output.SetPixel(x, y, Color.FromArgb(palette[(byte)~(((uint)(originalMask.GetPixel(x, y).ToArgb())) >> 24)]));
                        }
                    }
                }
                return output;
            }
            catch
            {

            }

            return null;
        }


        /// <summary>
        /// Loads the palette from the file.
        /// </summary>
        /// <returns></returns>
        private static int[] loadPalette()
        {
            int[] palette = new int[256];
            Color c;
            for (int i = 0; i < palette.Length - 1; i++)
            {
                c = HSVtoRGB(i, 255, 255, 255-i);
                palette[i] = c.ToArgb();
            }
            // Set the last color to 0x00000000 to make sure areas 
            // with no heat point remain original.
            palette[palette.Length - 1] = 0;
            return palette;
        }
        //private static Color HSV2RGB(double H, double S, double V,int alpha)
        //{
        //    double R, G, B;
        //    R = G = B = 0;
        //    H /= 60;
        //    int i = Convert.ToInt32(Math.Floor(H));
        //    double f = H - i;
        //    double a = V * (1 - S);
        //    double b = V * (1 - S * f);
        //    double c = V * (1 - S * (1 - f));
        //    switch (i)
        //    {
        //        case 0: R = V; G = c; B = a; break;
        //        case 1: R = b; G = V; B = a; break;
        //        case 2: R = a; G = V; B = c; break;
        //        case 3: R = a; G = b; B = V; break;
        //        case 4: R = c; G = a; B = V; break;
        //        case 5: R = V; G = a; B = b; break;
        //    }
        //    R = R > 1.0 ? 255 : R * 255;
        //    G = G > 1.0 ? 255 : G * 255;
        //    B = B > 1.0 ? 255 : B * 255;
        //    return Color.FromArgb(alpha, Convert.ToInt32(R), Convert.ToInt32(G), Convert.ToInt32(B));
        //}

        public static Color HSVtoRGB(int hsvH, int hsvS, int hsvV, int alpha)
        {
            if (hsvH == 360) hsvH = 359; // 360为全黑，原因不明
            if (hsvH<0)hsvH = 0;
            float R = 0f, G = 0f, B = 0f;
            if (hsvS == 0)
            {
                return Color.FromArgb((int)hsvV, (int)hsvV, (int)hsvV);
            }
            float S = hsvS * 1.0f / 255, V = hsvV * 1.0f / 255;
            int H1 = (int)(hsvH * 1.0f / 60), H = (int)hsvH;
            float F = H * 1.0f / 60 - H1;
            float P = V * (1.0f - S);
            float Q = V * (1.0f - F * S);
            float T = V * (1.0f - (1.0f - F) * S);
            switch (H1)
            {
                case 0: R = V; G = T; B = P; break;
                case 1: R = Q; G = V; B = P; break;
                case 2: R = P; G = V; B = T; break;
                case 3: R = P; G = Q; B = V; break;
                case 4: R = T; G = P; B = V; break;
                case 5: R = V; G = P; B = Q; break;
            }
            R = R * 255;
            G = G * 255;
            B = B * 255;
            while (R > 255) R -= 255;
            while (R < 0) R += 255;
            while (G > 255) G -= 255;
            while (G < 0) G += 255;
            while (B > 255) B -= 255;
            while (B < 0) B += 255;
            return Color.FromArgb(alpha, (int)R, (int)G, (int)B);
        }
    }
}
