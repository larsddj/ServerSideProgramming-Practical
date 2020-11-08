using System;
using System.Collections.Generic;
using System.Text;

namespace ImageTextFunc.Helper
{
    class IdHelper
    {
        public string generateID()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
