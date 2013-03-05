using System;
using ITMService.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;

/* this controller class will access all the builds and provide back a list of them
 * 
 * */

namespace ITMService.Controllers
{
    public class BuildController : ApiController
    {

        Build[] builds = new Build[]{// an array of Builds
            new Build {buildID = "1", title = "first one", tags = "tag 1, tag 2", manifestPath = "1.json", previewImagePath = "1_preview.png"},
            new Build {buildID = "2", title = "2nd one", tags = "tag 1, tag 2", manifestPath = "2.json", previewImagePath = "2_preview.png"},
            new Build {buildID = "3", title = "third", tags = "tag 1, tag 2", manifestPath = "3.json", previewImagePath = "3_preview.png"},
        };

        public List<Build> GetAllBuilds()// will return all builds in the database
        {
            // setup the reader and DB connections
            SqlDataReader rdr = null;//
            SqlConnection buildsDB = new SqlConnection();

            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = "SELECT * FROM Builds";// get all of them

            SqlCommand buildsConn = new SqlCommand(commStr, buildsDB);

            List<Build> returnBuilds = new List<Build>();

            buildsDB.Open();//open the db
            try
            {
                rdr = buildsConn.ExecuteReader();

                if (rdr.HasRows)// get all the builds 
                {
                  
                    while (rdr.Read())
                    {
                        Build b = new Build();
                        b.buildID = rdr["buildID"].ToString();
                        b.title = rdr["title"].ToString();
                        b.tags = rdr["tags"].ToString();
                        b.previewImagePath = "previewImagePath";
                        b.manifestPath = rdr["manifestPath"].ToString();
                         //DateTime.Parse(rdr["dateCreated"].ToString());

                        returnBuilds.Add(b);
                    }
                    rdr.Close();
                    buildsDB.Close();
                    
                }

            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
            finally
            {
                rdr.Close();
                rdr = null;
                buildsDB.Close();
            }

            return returnBuilds;// return the set
        }

        public Build GetBuildById(int id) // will get one build based on the id and return it 
        {
         
            // setup the reader and DB connections
            SqlDataReader rdr = null;//
            SqlConnection buildsDB = new SqlConnection();

            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = String.Format("SELECT * FROM Builds WHERE applicationID = {0}", Convert.ToString(id));// get all of them

            SqlCommand buildsConn = new SqlCommand(commStr, buildsDB);

            Build b = null;// set to null for now

            buildsDB.Open();//open the db
            try
            {
                rdr = buildsConn.ExecuteReader();

                if (rdr.HasRows)// get all the builds 
                {
                    while (rdr.Read())
                    {
                        b = new Build();

                        b.buildID = rdr["buildID"].ToString();
                        b.title = rdr["title"].ToString();
                        b.tags = rdr["tags"].ToString();
                        b.previewImagePath = "previewImagePath";
                        b.manifestPath = rdr["manifestPath"].ToString();
                        b.applicationID = id;
                        //DateTime.Parse(rdr["dateCreated"].ToString());
                    }
                    
                    
                }

                if (b == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);// let user know it wasn't found
                }

                

            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
            finally
            {
                rdr.Close();
                rdr = null;
                buildsDB.Close();
            }
            return b;
        }

        public IEnumerable<Build> GetBuildsByUserId(int id)// returns all builds based on the user's id
        {
            // setup the connection
            return builds;
        }

        // this method will take on the whole build
        [HttpPost]
        [ActionName("media")]// create the build and post it in the database
        public Dictionary<string,int> PostMedia(Build build)
        {
            if (ModelState.IsValid && build != null)
            {
                string s = "test";
            }
            else
            {
                // return it with zero so the app knows it didn't go through
                Dictionary<string,int> retDic = new Dictionary<string,int>();
                retDic.Add("applicationID",0);
                return retDic;
            }

            // setup the reader and DB connections

            SqlConnection buildsDB = new SqlConnection();

            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();


            // set the parameters and the command string
            Boolean isNew = true;// flag to decide if this is new
            string commStr = "INSERT INTO Builds (title, tags, manifestPath, buildID) VALUES (@title,@tags,@manifestPath,@buildID)";
            if(GetBuildById(build.applicationID) != null){
                isNew = false;
                commStr = String.Format("UPDATE Builds SET title=@title,tags=@tags,manifestPath=@manifestPath,buildID=@buildID WHERE applicationID = {0}", Convert.ToString(build.applicationID));
                deleteBuildItems(build.applicationID);
            }
            SqlCommand cmdBuild = new SqlCommand(commStr,buildsDB);

            cmdBuild.Parameters.Add("@title", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@title"].Value = build.title;
            cmdBuild.Parameters.Add("@tags", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@tags"].Value = build.tags;
            cmdBuild.Parameters.Add("@manifestPath", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@manifestPath"].Value = build.manifestPath;
            cmdBuild.Parameters.Add("@buildID", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@buildID"].Value = build.buildID;

            SqlDataReader rdr = null;// set to null and then assign when reading

            buildsDB.Open();//open the db

            int newID = 0;// used to capture the new appID when inserting a new one
            try
            {
                rdr = cmdBuild.ExecuteReader();
                if (rdr.HasRows)
                {
                    int i = 0;// set column index
                    while (rdr.Read())
                    {
                        Debug.WriteLine(rdr.GetName(i));
                        i++;
                    }
                }

                newID = 1;

            }
            catch (Exception e)
            {
                // return it with zero so the app knows it didn't go through
                Dictionary<string, int> retDic = new Dictionary<string, int>();
                retDic.Add("applicationID", 0);
                return retDic;
            }
            finally
            {
                buildsDB.Close();
            }
            // if there were new ones or updated ones, then return the message properly
            //HttpResponseMessage hr = (rows > 0) ? new HttpResponseMessage(HttpStatusCode.OK) : new HttpResponseMessage(HttpStatusCode.BadRequest);

            // return it with zero so the app knows it didn't go through
            Dictionary<string, int> appIDDic = new Dictionary<string, int>();
            if (isNew)
            {
                appIDDic.Add("applicationID", build.applicationID);
            }
            else
            {
                appIDDic.Add("applicationID", newID);
            }
            
            return appIDDic;// returns a dictionary with the appID in it
        }

        [ActionName("delete")]
        public HttpResponseMessage DeleteBuild(int id)// delete all the buildItems and then delete the build
        {
            deleteBuildItems(id);// delete all these build items
            SqlConnection buildsDB = new SqlConnection();

            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = String.Format("DELETE FROM Builds WHERE applicationID = {0}", Convert.ToString(id));

            SqlCommand buildsConn = new SqlCommand(commStr, buildsDB);

            buildsDB.Open();//open the db
            try
            {
                buildsConn.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
            finally
            {
                buildsDB.Close();
                
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [NonAction]
        private void deleteBuildItems(int applicationID)// this will delete all the build items for the buildID provided
        {// setup the db connection

            SqlConnection buildsDB = new SqlConnection();
            
            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = String.Format("DELETE FROM BuildItems WHERE applicationID = {0}", Convert.ToString(applicationID));

            SqlCommand buildsConn = new SqlCommand(commStr, buildsDB);

            buildsDB.Open();//open the db
            try
            {
                buildsConn.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
            finally
            {
                buildsDB.Close();
            }
        }

       
    }
}
