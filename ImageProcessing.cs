using Avalonia;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Extensions.SkiaSharp;
using FFMpegCore.Helpers;
using FFMpegCore.Pipes;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TimeLapserdak
{
    public static class ImageProcessing
    {
        public static bool CropAndResizePicture(FileInfo file, PixelRect crop, string outputFolder)
        {
            using var bitmap = SKBitmap.Decode(file.FullName);
            if (bitmap is null) return false;
            using var pixmap = new SKPixmap(bitmap.Info, bitmap.GetPixels());
            SKRectI cropRect = new(crop.X, crop.Y, crop.Right, crop.Bottom);
                
            using var croppedBitmap = new SKBitmap(new SKImageInfo(crop.Width, crop.Height));
            if (!bitmap.ExtractSubset(croppedBitmap, cropRect)) return false;
                
            using var outputBitmap = new SKBitmap(new SKImageInfo(1920, 1080));
            if (!croppedBitmap.ScalePixels(outputBitmap, SKFilterQuality.High)) return false;
                
            using var data = outputBitmap.Encode(SKEncodedImageFormat.Jpeg, 95);
            try
            {
                File.WriteAllBytes(Path.Combine(outputFolder, file.Name), data.ToArray());
            }
            catch 
            {
                return false;
            }

            return true;
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

        public static event EventHandler<double> ProgressChangedEvent = null!;

        public static async Task<string> GenerateVideo(IEnumerable<IVideoFrame> frames, double frameRate, string outputFolder)
        {
            Action<double> progressHandler = new(p => ProgressChangedEvent?.Invoke(null, p));

            RawVideoPipeSource source = null;
            bool success = false;
            await Task.Run(() => source = new(frames) { FrameRate = frameRate });
            if (source is null) return "Encountered a problem with creating video frames";

            string errorMessage = string.Empty;
            try
            {
                success = await FFMpegArguments
                    .FromPipeInput(source,
                        options => options
                            .WithHardwareAcceleration(HardwareAccelerationDevice.Auto))
                    .OutputToFile(
                        Path.Combine(outputFolder, "TimeLapserdak." + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".mp4"),
                        overwrite: true,
                        options => options.WithVideoCodec(VideoCodec.LibX264)
                            .UsingMultithreading(true)
                            .WithConstantRateFactor(28)
                            .WithVariableBitrate(5)
                            .WithFastStart()
                            .WithFramerate(frameRate)
                            .WithSpeedPreset(Speed.UltraFast))
                    .NotifyOnProgress(progressHandler, TimeSpan.FromSeconds(1.0 * frames.Count() / frameRate))
                    .ProcessAsynchronously(throwOnError: true);
            }
            catch (Exception ex)
            {
                success = false;
                errorMessage = ex.Message;
            }

            if (!success) errorMessage = @"Generating the video was unsuccessful:" + Environment.NewLine +
                    errorMessage + Environment.NewLine +
@"Check your FFMpeg settings and try again.
If the problem persists, contact the developer.";

            return errorMessage;
        }

        public static bool IsFFMpegAvailable()
        {
            var ffmpegExists = true;

            try
            {
                FFMpegHelper.VerifyFFMpegExists(new FFOptions { });
            }
            catch (Instances.Exceptions.InstanceFileNotFoundException)
            {
                ffmpegExists = false;
            }
            catch (Exception) 
            { 
                ffmpegExists = false;
            }
            
            return ffmpegExists;
        }
    }
}
