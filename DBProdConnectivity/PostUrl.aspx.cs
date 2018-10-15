using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DBProdConnectivity
{
    public partial class PostUrl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnConnectEmc_Click(object sender, EventArgs e)
        {
            try
            {
                //string URL = "https://ordermessageservice.cf.isus.emc.com/api/v10/orderMessage";
              //  string URL = "https://ssgosgdev.isus.emc.com/api/v10/orderMessage";
                string URL = "https://eventpoeticutilservice-tes-prd.cfcp.isus.emc.com/api/v10/event/11223811";

                HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                request.KeepAlive = true;
                request.Method = "GET";
                request.ContentType = "application/json";


                request.Credentials = new NetworkCredential("tesuser", "Password1");


                request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + "dXNlcjo5NmRlY2Y5MC00NDhhLTQyOTktYTIyMy04ODU3MThhM2Y4OGI=");


                //using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                //{
                //    string json = new JavaScriptSerializer().Serialize(new RootObject()
                //    {
                //        dell_order = new DellOrder()
                //        {
                //            version = "",
                //            order_date = "",
                //        },
                //        order_header = new OrderHeader()
                //        {
                //            so_number = "",
                //            sold_to = "",
                //            install_at = "",


                //        },

                //        order_lines = new List<OrderLine>()
                //        {
                //            new OrderLine() {
                //            line_num = "",
                //            part_number = "",
                //            quantity = ""
                //            },
                //            new OrderLine() {
                //            line_num = "",
                //            part_number = "",
                //            quantity = ""
                //            },
                //        }
                //    });


                //    streamWriter.Write(json);
                //}

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                using (Stream responseStream = response.GetResponseStream())
                {

                    if (response.StatusCode != HttpStatusCode.OK) throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).", response.StatusCode,
                    response.StatusDescription));

                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    txtResponseEmc.Text = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                txtResponseEmc.Text = ex.Message;
            }
        }
    }
}