using System.Diagnostics; 
using System.Net; 
using System.Net.Http; 
using System.Threading.Tasks; 
using System.Web; 
using System.Web.Http;
using ITMService.Models;
using System.Data.SqlClient;
using System.Collections;
using System;


namespace ITMService.Controllers
{
    public class ItemController : ApiController
    {

        public Task<HttpResponseMessage> PostFormData()
        {
            // Check if the request contains multipart/form-data. 
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            // Read the form data and return an async task. 
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<HttpResponseMessage>(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }

                    // This illustrates how to get the file names. 
                    foreach (MultipartFileData file in provider.FileData)
                    {
                        Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                        Trace.WriteLine("Server file path: " + file.LocalFileName);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK);
                });

            return task;
        }


        [NonAction]// inserts a new builditem 
        private string insertBuildItems(BuildItem b)
        {
            SqlDataReader rdr = null;// setup the sql reader
            SqlConnection buildItemsDB = new SqlConnection();// get the conn
            buildItemsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            buildItemsDB.Open();// open the database

            try
            {
                // check to see if it exists. If this one already exists, then update it with the new content, else insert it
                string itemExists = "INSERT INTO BuildItems (caption, fileName, orderNumber, status, thumbnailPath, timeStamp, title, type, buildID) VALUES (@caption,  @fileName, @orderNumber, @status, @thumbnailPath, @timeStamp, @title, @type, @buildID)";// "," + b.fileName + "," + b.orderNumber + "," + b.status + "," + b.thumbnailPath + "," + b.timeStamp + "," + b.title + "," + b.type + "," + b.buildID + ")";
                SqlCommand cmdBuildItems = new SqlCommand(itemExists, buildItemsDB);

                // add the parameters
                cmdBuildItems.Parameters.Add("@caption", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@caption"].Value = b.caption;
                cmdBuildItems.Parameters.Add("@fileName", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@fileName"].Value = b.fileName;
                cmdBuildItems.Parameters.Add("@orderNumber", System.Data.SqlDbType.Int);
                cmdBuildItems.Parameters["@orderNumber"].Value = b.orderNumber;
                cmdBuildItems.Parameters.Add("@status", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@status"].Value = b.status;
                cmdBuildItems.Parameters.Add("@thumbnailPath", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@thumbnailPath"].Value = b.thumbnailPath;
                cmdBuildItems.Parameters.Add("@timeStamp", System.Data.SqlDbType.DateTime);
                cmdBuildItems.Parameters["@timeStamp"].Value = b.timeStamp;
                cmdBuildItems.Parameters.Add("@title", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@title"].Value = b.title;
                cmdBuildItems.Parameters.Add("@type", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@type"].Value = b.type;
                cmdBuildItems.Parameters.Add("@buildID", System.Data.SqlDbType.BigInt);
                cmdBuildItems.Parameters["@buildID"].Value = b.buildID;


                rdr = cmdBuildItems.ExecuteReader();// need to return the value we just created

                rdr.Close();

                string selectLatestString = "SELECT buildItemID FROM BuildItems WHERE buildID=@buildID AND orderNumber=@orderNumber";
                cmdBuildItems = new SqlCommand(selectLatestString, buildItemsDB);
                cmdBuildItems.Parameters.Add("@buildID", System.Data.SqlDbType.BigInt);
                cmdBuildItems.Parameters["@buildID"].Value = b.buildID;
                cmdBuildItems.Parameters.Add("@orderNumber", System.Data.SqlDbType.Int);
                cmdBuildItems.Parameters["@orderNumber"].Value = b.orderNumber;
                int returnBuildItemID = (int)cmdBuildItems.ExecuteScalar();

                return "new buildItemID = " + Convert.ToString(returnBuildItemID);

            }
            catch (SqlException e)
            {
                return "failure adding record";
            }
            finally
            {

                rdr.Close();
                rdr = null;
                buildItemsDB.Close();
            }
            return "record created";
        }

        [NonAction]
        private string updateBuildItems(BuildItem b)
        {
            SqlDataReader rdr = null;// setup the sql reader
            SqlConnection buildItemsDB = new SqlConnection();// get the conn
            buildItemsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            buildItemsDB.Open();// open the database

            try
            {
                // update the data for this item
                string updateData = "UPDATE BuildItems SET caption=@caption, filename=@fileName,orderNumber=@orderNumber,status=@status,thumbnailPath=@thumbnailPath,timeStamp=@timeStamp,title=@title,type=@type WHERE buildItemID=@buildItemID AND buildID=@buildID";
                SqlCommand cmdBuildItems = new SqlCommand(updateData, buildItemsDB);

                // add the parameters
                cmdBuildItems.Parameters.Add("@caption", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@caption"].Value = b.caption;
                cmdBuildItems.Parameters.Add("@fileName", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@fileName"].Value = b.fileName;
                cmdBuildItems.Parameters.Add("@orderNumber", System.Data.SqlDbType.Int);
                cmdBuildItems.Parameters["@orderNumber"].Value = b.orderNumber;
                cmdBuildItems.Parameters.Add("@status", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@status"].Value = b.status;
                cmdBuildItems.Parameters.Add("@thumbnailPath", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@thumbnailPath"].Value = b.thumbnailPath;
                cmdBuildItems.Parameters.Add("@timeStamp", System.Data.SqlDbType.DateTime);
                cmdBuildItems.Parameters["@timeStamp"].Value = b.timeStamp;
                cmdBuildItems.Parameters.Add("@title", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@title"].Value = b.title;
                cmdBuildItems.Parameters.Add("@type", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@type"].Value = b.type;
                cmdBuildItems.Parameters.Add("@buildID", System.Data.SqlDbType.BigInt);
                cmdBuildItems.Parameters["@buildID"].Value = b.buildID;
                cmdBuildItems.Parameters.Add("@buildItemID", System.Data.SqlDbType.Int);
                cmdBuildItems.Parameters["@buildItemID"].Value = b.buildItemID;

                rdr = cmdBuildItems.ExecuteReader();
                rdr.Close();


            }
            catch (SqlException e)
            {
                return "failure adding record";
            }
            finally
            {
                rdr.Close();
                rdr = null;
                buildItemsDB.Close();
            }
            return "buildItemID = " + Convert.ToString(b.buildItemID);
        }

        
    }
}