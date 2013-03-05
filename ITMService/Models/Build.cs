using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITMService.Models
{
    /* This class will serve as the model for Builds*/

    public class Build
    {
        public int applicationID { get; set; }// represents the application id for this build - this is the unique id for this build
        public string title {get; set;}// the title of the build
        public string tags { get; set; }// the tags from the build
        public string manifestPath { get; set; }// path to the manifest file that dictates the media items and the order
        public string buildID { get; set; }// the id from the database for this build
        public string previewImagePath { get; set; }// the path to the previewThumbnail, this is the first image in the sequence


    }
}