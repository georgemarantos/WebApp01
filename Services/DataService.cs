using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Processing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;


namespace CleanDDTest.Services
{
    public class DataService : IDataService
    {
        public  byte[] ImageToByteArray(SixLabors.ImageSharp.Image imageIn)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                imageIn.Save(ms, JpegFormat.Instance);
                return ms.ToArray();
            }
        }



        //public byte[] ImageToByteArray(SixLabors.ImageSharp.Image image)
        //{



        //    ImageConverter imgCon = new ImageConverter();
        //    byte[] imgByte = (byte[])imgCon.ConvertTo(image, typeof(byte[]));

        //    return AsJpeg(imgByte);



        //    //return CompressImage(image);
        //}

        public byte[] AsJpeg(byte[] data)
        {
            using (var inStream = new MemoryStream(data))
            using (var outStream = new MemoryStream())
            {
                var imageStream = System.Drawing.Image.FromStream(inStream);
                imageStream.Save(outStream, ImageFormat.Jpeg);
                return outStream.ToArray();
            }
        }


        //System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        //return encoding.GetBytes(filename);

        //public IActionResult setImage(IFormFile file, ViewModel model)
        //{

        //}

        //public string ByteArrayToImage(byte[] file)
        //{

        //    //investigate opening readstream to pull in image data then saving as a local variable during runtime
        //    //convert byte array to image 

        //    var convertedImg = Convert.ToBase64String(file);

        //    string imageURL = String.Format("data:image/jpg;base64,{0},", convertedImg);
        //    //ViewBag.ImageUrl = imageURL;

        //    // await img = 

        //    return imageURL;

        //}

        public System.Drawing.Image ByteArrayToImage(byte[] file)
        {

            using (MemoryStream ms = new MemoryStream(file, 0, file.Length)) 
            { 
                ImageConverter imageConverter = new();
                System.Drawing.Image? image = System.Drawing.Image.FromStream(ms, false);
                image = new Bitmap(image);
                return image;
            }
        }

        public System.Drawing.Image GetImageFromFile(IFormFile file)
        {
            System.Drawing.Image? img = null;

            MemoryStream stream = new MemoryStream();
            
            file.CopyTo(stream);
            img = System.Drawing.Image.FromStream(stream);
                
            //int width = img.Width / 2;
            //int height = img.Height / 2;
                //img.Mutate(x => x.Resize(width, height));





                //ImageResize(img,100,100);
                //ImageToByteArray(img);

            
                return img;
            
            
            
            
            
            
            
            
            //Image image = Image.FromFile(Convert.ToString(file));
            //return image;
            
        }


        public string ImageResize(SixLabors.ImageSharp.Image img, int MaxWidth, int MaxHeight)
        {
            if (img.Width > MaxWidth || img.Height > MaxHeight)
            {
                double widthratio = (double)img.Width / (double)MaxWidth;
                double heightratio = (double)img.Height / (double)MaxHeight;
                double ratio = Math.Max(widthratio, heightratio);
                int newWidth = (int)(img.Height / ratio);
                int newHeight = (int)(img.Width / ratio);
                return newHeight.ToString() + "," + newWidth.ToString();
            }
            else
            {
                return img.Height.ToString() + "," + img.Width.ToString();
            }
        }


        //public string ImageResize(Image img, int MaxWidth, int MaxHeight)
        //{
        //    if (img.Width > MaxWidth || img.Height > MaxHeight)
        //    {
        //        double widthratio = (double)img.Width / (double)MaxWidth;
        //        double heightratio = (double)img.Height / (double)MaxHeight;
        //        double ratio = Math.Max(widthratio, heightratio);
        //        int newWidth = (int)(img.Height / ratio);
        //        int newHeight = (int)(img.Width / ratio);                 
        //        return newHeight.ToString() + "," + newWidth.ToString();
        //    }
        //    else
        //    {
        //        return img.Height.ToString() + "," + img.Width.ToString();
        //    }
        //}

        //public byte[] CompressImage(Image img)
        //{
        //    byte[] compressedByteArray = null;
        //    Bitmap compressedImg = new(img, new Size(img.Width, img.Height / 2));
        //    using (var stream = new MemoryStream())
        //    {
        //        compressedImg.Save(stream,ImageFormat.Jpeg);
        //         compressedByteArray = stream.ToArray();
        //    }
        //    return compressedByteArray;
        //}


        public byte[] GetImage(IFormFile file)
        {
            string filename = string.Empty;
            string path = string.Empty;
            byte[]? data = null;
            
            if (file.Length > 0)
            {
                filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                //filename = ("Direct-Deposit_") + file.FileName;
                path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload"));
                string fullpath = Path.Combine(path, filename);
                using (var img = SixLabors.ImageSharp.Image.Load(file.OpenReadStream()))
                {
                    string newsize = ImageResize(img,300,200);
                    string[] sizearray = newsize.Split(',');
                    
                    img.Mutate(x => x.Resize(Convert.ToInt32(sizearray[0]), Convert.ToInt32(sizearray[1])));
                    
                    data = AsJpeg(ImageToByteArray(img));
                    //image.Mutate(x => x.Resize(Convert.ToInt32(sizearray[1]), Convert.ToInt32(sizearray[0])));
                    //image.Save(fullpath);
                    //_context.Voidedchecks.AddAsync(image);
                    //TempData["msg"] = "File upload successfully";
                    
                }
            }
                    return data;
        }

       
    }
}
