using Avalonia;
using FFMpegCore;
using FFMpegCore.Extensions.SkiaSharp;
using FFMpegCore.Pipes;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

        public static IEnumerable<BitmapVideoFrameWrapper> CreateFrames(List<FileInfo> images)
        {
            foreach (FileInfo file in images)
            {
                using var bitmap = SKBitmap.Decode(file.FullName);
                using BitmapVideoFrameWrapper wrappedBitmap = new(bitmap);
                yield return wrappedBitmap;
            }
        }

        public static async Task<bool> GenerateVideo(IEnumerable<IVideoFrame> frames, string outputFolder)
        {
            RawVideoPipeSource source = new(frames) { FrameRate = 25 };
            bool success = await FFMpegArguments
                .FromPipeInput(source)
                .OutputToFile(
                    Path.Combine(outputFolder, "TimeLapserdak." + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".mp4"), 
                    overwrite: true, 
                    options => options.WithVideoCodec("mpeg4").UsingMultithreading(true).WithVideoBitrate(3500).WithSpeedPreset(FFMpegCore.Enums.Speed.UltraFast))
                .ProcessAsynchronously();

            return success;
        }
    }
}
