using QueueTriggerAPI.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueueTriggerAPI.Helper
{
    class MessageSeperator
    {
        public QueueItem Seperate(string myQueueItem)
        {
            string[] splitString = myQueueItem.Split("-");
            QueueItem queueItem = new QueueItem(splitString[0], splitString[1]);
            return queueItem;
        }
    }
}
