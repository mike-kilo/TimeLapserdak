using SkiaSharp;

namespace TimeLapserdak.Helpers
{
    public static class Enums
    {
        public enum Orientation
        {
            Landscape,
            Square,
            Portrait,
        }

        public static double ToAspectRatio(this Orientation orientation)
        {
            return orientation switch
            {
                Orientation.Landscape => 16.0 / 9.0,
                Orientation.Square => 1.0,
                Orientation.Portrait => 9.0 / 16.0,
                _ => 16.0 / 9.0,
            };
        }

        public static SKImageInfo ToBitmapSize(this Orientation orientation)
        {
            return orientation switch
            {
                Orientation.Landscape => new SKImageInfo(1920, 1080),
                Orientation.Square => new SKImageInfo(1080, 1080),
                Orientation.Portrait => new SKImageInfo(1080, 1920),
                _ => new SKImageInfo(1920, 1080),
            };
        }

        public static string ToFFMpegAspect(this Orientation orientation)
        {
            return orientation switch
            {
                Orientation.Landscape => "16:9",
                Orientation.Square => "1:1",
                Orientation.Portrait => "9:16",
                _ => "16:9",
            };
        }
    }
}
