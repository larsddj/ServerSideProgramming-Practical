using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QueueTriggerAPI.Helper
{
    class ImageTextHelper
    {
        //here we draw the retrieved text onto the retrieved image
        public async Task<Image> Merge(string text, Image image)
        {
            PointF location = new PointF(0f, 0f);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                using (Font arial = new Font("arial", 64))
                {
                    graphics.DrawString(text, arial, Brushes.White, location);
                }
            }
            return image;
        }
    }
}
