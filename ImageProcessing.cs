using Avalonia;
using SkiaSharp;
using System.IO;

namespace TimeLapserdak
{
    public static class ImageProcessing
    {
        public static void CropAndResizePictures(FileInfo file, PixelRect crop, string outputFolder)
        {
            using var bitmap = SKBitmap.Decode(file.FullName);
            using var pixmap = new SKPixmap(bitmap.Info, bitmap.GetPixels());
            SKRectI cropRect = new(crop.X, crop.Y, crop.Right, crop.Bottom);
                
            using var croppedBitmap = new SKBitmap(new SKImageInfo(crop.Width, crop.Height));
            if (!bitmap.ExtractSubset(croppedBitmap, cropRect)) return;
                
            using var outputBitmap = new SKBitmap(new SKImageInfo(1920, 1080));
            if (!croppedBitmap.ScalePixels(outputBitmap, SKFilterQuality.High)) return;
                
            using var data = outputBitmap.Encode(SKEncodedImageFormat.Jpeg, 95);
            File.WriteAllBytes(Path.Combine(outputFolder, file.Name), data.ToArray());
        }
    }
}
