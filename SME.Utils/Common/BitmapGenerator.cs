using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common
{
    public class BitmapGenerator
    {
        //Default Constructor 
        public BitmapGenerator() { }
        //property
        public string Text
        {
            get { return this.text; }
        }
        public Bitmap Image
        {
            get { return this.image; }
        }
        public int Width
        {
            get { return this.width; }
        }
        public int Height
        {
            get { return this.height; }
        }
        //Private variable
        private string text;
        private int width;
        private int height;
        private Bitmap image;
        private Random random = new Random();
        //Methods declaration
        public BitmapGenerator(string s, int width, int height)
        {
            this.text = s;
            this.SetDimensions(width, height);
            this.GenerateImages();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                this.image.Dispose();
        }
        private void SetDimensions(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", width,
                    "Argument out of range, must be greater than zero.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", height,
                    "Argument out of range, must be greater than zero.");
            this.width = width;
            this.height = height;
        }
        private void GenerateImages()
        {
            Bitmap objBmp = new Bitmap(width, height);
            Graphics objGraphics = Graphics.FromImage(objBmp);
            Rectangle rect = new Rectangle(0, 0, this.width, this.height);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            objGraphics.Clear(Color.White);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            objGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            objGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            objGraphics.DrawString(this.text, new Font("Arial", 40, FontStyle.Bold), Brushes.DarkBlue, rect, format);
            objGraphics.Flush();
            this.image = objBmp;
            objGraphics.Dispose();
        }
    }
}
