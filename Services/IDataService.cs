using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace CleanDDTest.Services
{
    public interface IDataService
    {
        //convert image string to byte array so it can be saved in the DB
        public byte[] ImageToByteArray(SixLabors.ImageSharp.Image imageIn);

        //convert image byte array to url string so that it can be disaplyed
        public System.Drawing.Image ByteArrayToImage(byte[] file);

        //get image from file uploaded by user
        public System.Drawing.Image GetImageFromFile(IFormFile file);

        //moved image resize to service. Resizes image
        //public string ImageResize(Image img, int MaxWidth, int MaxHeight);

        //method to compress image before upload to database
        //public byte[] CompressImage(Image img);

       public string ImageResize(SixLabors.ImageSharp.Image img, int MaxWidth, int MaxHeight);

        public byte[] GetImage(IFormFile file);

        //public byte[] AsJpeg(byte[] data);
    }
}
