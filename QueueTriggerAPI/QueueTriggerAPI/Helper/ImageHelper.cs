using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace QueueTriggerAPI.Helper
{
    class ImageHelper
    {
        public Image GetImage(string url)
        {
            Image image = null;
            try
            {
                using (WebClient client = new WebClient())
                {
                    Stream stream = client.OpenRead(url);
                    image = new Bitmap(stream);
                    stream.Flush();
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return image;
        }

        public void SaveImageToBlob(Image image)
        {

        }
    }
}
