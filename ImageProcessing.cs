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
        public static bool CropAndResizePicture(FileInfo file, PixelRect crop, string outputFolder, Enums.Orientation orientation = Enums.Orientation.Landscape)
        {
            using var bitmap = SKBitmap.Decode(file.FullName);
            if (bitmap is null) return false;
            using var pixmap = new SKPixmap(bitmap.Info, bitmap.GetPixels());
            SKRectI cropRect = new(crop.X, crop.Y, crop.Right, crop.Bottom);
                
            using var croppedBitmap = new SKBitmap(new SKImageInfo(crop.Width, crop.Height));
            if (!bitmap.ExtractSubset(croppedBitmap, cropRect)) return false;
                
            using var outputBitmap = new SKBitmap(orientation.ToBitmapSize());
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

        public static async Task<RawVideoPipeSource?> GenerateVideoPipeSource(IEnumerable<IVideoFrame> frames, double frameRate)
        {
            if (frames is null || !frames.Any()) return null;
            RawVideoPipeSource? source = null;
            await Task.Run(() => source = new(frames) { FrameRate = frameRate });
            return source;
        }

        public static async Task<string> GenerateVideo(RawVideoPipeSource pipeSource, string outputFolder, int framesCount = 0, Enums.Orientation orientation = Enums.Orientation.Landscape)
        {
            Action<double> progressHandler = new(p => { if (framesCount > 0) ProgressChangedEvent?.Invoke(null, p); });

            bool success = false;

            string errorMessage = string.Empty;
            try
            {
                success = await FFMpegArguments
                    .FromPipeInput(pipeSource,
                        options => options
                            .WithHardwareAcceleration(HardwareAccelerationDevice.Auto))
                    .OutputToFile(
                        Path.Combine(outputFolder, "TimeLapserdak." + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".mp4"),
                        overwrite: true,
                        options => options
                            .WithVideoCodec(VideoCodec.LibX264)
                            .UsingMultithreading(true)
                            .WithBitStreamFilter(Channel.Video, Filter.H264_Mp4ToAnnexB)
                            .WithConstantRateFactor(20)
                            .WithFastStart()
                            .WithSpeedPreset(Speed.Slow)
                            .ForcePixelFormat("yuv420p")
                            .WithCustomArgument("-aspect " + orientation.ToFFMpegAspect())
                            .WithFramerate(pipeSource.FrameRate))
                    .NotifyOnProgress(progressHandler, TimeSpan.FromSeconds(1.0 * framesCount / pipeSource.FrameRate))
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
