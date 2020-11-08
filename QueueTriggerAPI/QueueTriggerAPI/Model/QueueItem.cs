using System;
using System.Collections.Generic;
using System.Text;

namespace QueueTriggerAPI.Model
{
    class QueueItem
    {
        public string message_ { get; set; }
        public string id_ { get; set; }

        public QueueItem(string message, string id)
        {
            message_ = message;
            id_ = id;
        }
    }
}
