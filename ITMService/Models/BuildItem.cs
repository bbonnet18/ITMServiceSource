using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITMService.Models
{
    public class BuildItem
    {
        public int buildItemID { get; set; }
        public string caption { get; set; }
        public string fileName { get; set; }
        public int orderNumber { get; set; }
        public string status { get; set; }
        public string thumbnailPath { get; set; }
        public DateTime timeStamp { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public int buildID { get; set; }
    }
}