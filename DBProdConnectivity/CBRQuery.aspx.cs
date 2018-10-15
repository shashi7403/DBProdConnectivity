using CDA.Dell.Contarct;
using CDA.Dell.Domain;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

namespace DBProdConnectivity
{
    public partial class CBRQuery : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            

        }
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            BindGrid<CBRQueryViewDTO>(CBRQueryViewDTOList, gvCBRQueryReport);
        }
        public static void BindGrid<T>(List<T> list, GridView gridView)
        {
            gridView.DataSource = list;
            gridView.DataBind();
        }
        public List<CBRQueryViewDTO> CBRQueryViewDTOList
        {
            get
             {
                try
                {
                    ReportManager test = new ReportManager();
                    return (new ReportManager().GetCBRQueryViewData(new CBRQueryViewDTO
                    {
                        DPKSerialNumber = txtProductKeySerialNumber.Text.Trim(),
                        IncludeArchive = chkIsArchiveReq.Checked
                    }));
                }
                catch (Exception ex)
                {
                    lblError.Text = ex.Message;
                    Response.Write(ex);
                }


                return null;
            }
        }
    }
}