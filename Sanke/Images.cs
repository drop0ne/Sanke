using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sanke
{
    public static class Images
    {
        // Static readonly fields for different game images
        public readonly static ImageSource Empty = LoadImage("Empty.png");
        public readonly static ImageSource Body = LoadImage("Body.png");
        public readonly static ImageSource Head = LoadImage("Head.png");
        public readonly static ImageSource Food = LoadImage("Food.png");
        public readonly static ImageSource DeadBody = LoadImage("DeadBody.png");
        public readonly static ImageSource DeadHead = LoadImage("DeadHead.png");

        // Helper method to load images from the Assets folder
        private static ImageSource LoadImage(string FileName)
        {
            return new BitmapImage(new Uri($"Assets/{FileName}", UriKind.Relative));
        }
    }
}