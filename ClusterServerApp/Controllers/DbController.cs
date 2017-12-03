using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using ClusterServerApp.Models;
using Newtonsoft.Json;

namespace ClusterServerApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Db")]
    public class DbController : ApiController
    {
        [HttpGet]
        [Route("GetConfig")]
        public IHttpActionResult GetConfig()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBCS"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM SystemConfig", con))
                    {
                        con.Open();

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                return Ok(JsonConvert.SerializeObject(new SystemConfig()
                                {
                                    MaxProcesses = int.Parse(rdr["MaxProcesses"].ToString()),
                                    ProcessesRunning = int.Parse(rdr["ProcessesRunning"].ToString())
                                }));
                            }
                        }
                    }
                }

                return NotFound();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("SetProcessGuid")]
        [HttpPost]
        public IHttpActionResult SetProcessGuid([FromBody] SetGuidParams parameters)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBCS"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("exec spAssignProcessGuid @url, @guid", con))
                    {
                        cmd.Parameters.AddWithValue("@url", parameters.ServerUrl);
                        cmd.Parameters.AddWithValue("@guid", parameters.ProcessGuid);

                        con.Open();

                        cmd.ExecuteNonQuery();
                        return Ok();
                    }
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        [Route("DeleteProcessGuid")]
        public IHttpActionResult DeleteProcessGuid(string guid)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBCS"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("exec spDeleteProcessGuid @guid", con))
                    {
                        cmd.Parameters.AddWithValue("@guid", guid);

                        con.Open();

                        cmd.ExecuteNonQuery();
                        return Ok();
                    }
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        [Route("ProcessCanceled")]
        public IHttpActionResult ProcessCanceled(string guid)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBCS"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("select count(Id) from Processes Where Guid=@guid", con))
                    {
                        cmd.Parameters.AddWithValue("@guid", guid);

                        con.Open();

                        bool res = (int)cmd.ExecuteScalar() == 0;
                        return Ok(res);
                    }
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }


        [HttpGet]
        [Route("SetConfig")]
        public IHttpActionResult SetConfig(int max_requests)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBCS"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("update SystemConfig set MaxProcesses=@mr", con))
                    {
                        cmd.Parameters.AddWithValue("@mr", max_requests);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        return Ok();
                    }
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }



    #region Helper Classes

    public class SetGuidParams
    {
        public string ServerUrl { get; set; }
        public string ProcessGuid { get; set; }
    }

    #endregion
}
