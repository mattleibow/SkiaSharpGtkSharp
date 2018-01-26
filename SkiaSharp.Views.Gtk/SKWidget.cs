using System;
using SkiaSharp;

namespace SkiaSharp.Views.Gtk
{
    [System.ComponentModel.ToolboxItem(true)]
    public class SKWidget : global::Gtk.DrawingArea
    {
        public SKWidget()
        {
            DoubleBuffered = false;
        }

        Gdk.Pixbuf pix;
        SKImageInfo info;

        private unsafe Gdk.Pixbuf GetBuffer()
        {
            info = new SKImageInfo(100, 100);

            if (pix == null)
                pix = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, info.Width, info.Height);

            return pix;
        }

        protected override bool OnExposeEvent(Gdk.EventExpose evnt)
        {
            var window = evnt.Window;
            var area = evnt.Area;

            var pb = GetBuffer();

            using (var surface = SKSurface.Create(info, pix.Pixels, info.RowBytes))
            {
                var canvas = surface.Canvas;

                canvas.Clear(SKColors.Red.WithAlpha(100));

                canvas.DrawRect(new SKRect(10, 10, 50, 50), new SKPaint { Color = SKColors.Green });
                canvas.DrawRect(new SKRect(40, 40, 90, 90), new SKPaint { Color = SKColors.Blue.WithAlpha(127) });

                canvas.DrawText(DateTime.Now.ToString("T"), 50, 50, new SKPaint { TextAlign = SKTextAlign.Center, IsAntialias = true });

                canvas.Flush();

                if (info.ColorType == SKColorType.Bgra8888)
                {
                    // swap R and B
                    var pm = surface.PeekPixels();
                    var bm = new SKBitmap();
                    bm.InstallPixels(pm);
                    for (int x = 0; x < info.Width; x++)
                    {
                        for (int y = 0; y < info.Height; y++)
                        {
                            var c = bm.GetPixel(x, y);
                            c = new SKColor(c.Blue, c.Green, c.Red, c.Alpha);
                            bm.SetPixel(x, y, c);
                        }
                    }
                }
            }

            window.Clear();
            window.DrawPixbuf(null, pb, 0, 0, 0, 0, -1, -1, Gdk.RgbDither.None, 0, 0);

            return true;
        }
    }
}
