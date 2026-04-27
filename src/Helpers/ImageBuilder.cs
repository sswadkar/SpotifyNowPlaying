namespace Loupedeck.SpotifyNowPlaying
{
    using System;

    using SkiaSharp;

    public sealed class ImageBuilder : IDisposable
    {
        private readonly SKBitmap _bitmap;
        private readonly SKCanvas _canvas;

        public Int32 Width { get; }

        public Int32 Height { get; }

        public ImageBuilder(Int32 width, Int32 height)
        {
            this.Width = width;
            this.Height = height;
            this._bitmap = new SKBitmap(width, height);
            this._canvas = new SKCanvas(this._bitmap);
        }

        public void Clear(SKColor color) => this._canvas.Clear(color);

        public void DrawText(String text, Int32 x, Int32 y, SKColor color, Int32 fontSize, SKTextAlign textAlign = SKTextAlign.Left)
        {
            if (String.IsNullOrEmpty(text))
            {
                return;
            }

            using var paint = new SKPaint
            {
                Color = color,
                TextSize = fontSize,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName("Inter")
                    ?? SKTypeface.FromFamilyName("Helvetica Neue")
                    ?? SKTypeface.Default,
                TextAlign = textAlign
            };

            var baseline = y - paint.FontMetrics.Ascent;
            this._canvas.DrawText(text, x, baseline, paint);
        }

        public void DrawHorizontallyCenteredText(String text, Int32 fontSize, SKColor color, Int32 y)
        {
            this.DrawText(text, this.Width / 2, y, color, fontSize, SKTextAlign.Center);
        }

        public void FillRectangle(Int32 x, Int32 y, Int32 width, Int32 height, SKColor color)
        {
            using var paint = new SKPaint
            {
                Color = color,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            this._canvas.DrawRect(x, y, width, height, paint);
        }

        public BitmapImage ToBitmapImage()
        {
            this._canvas.Flush();
            using var image = SKImage.FromBitmap(this._bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return BitmapImage.FromArray(data.ToArray());
        }

        public void Dispose()
        {
            this._canvas.Dispose();
            this._bitmap.Dispose();
        }
    }
}
