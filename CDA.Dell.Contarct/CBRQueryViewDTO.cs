using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDA.Dell.Contarct
{
   
    //[DataContract(Namespace = "http://www.dell.com/LKM")]
    public class CBRQueryViewDTO
    {
        [DisplayName("Serial Number")]
        public string DPKSerialNumber { get; set; }

        [DisplayName("DPK Status")]
        public string DPKStatus { get; set; }

        [DisplayName("CBR Status")]
        public string CBRStatus { get; set; }


        [DisplayName("CBR ACK NACK")]
        public string CBR_ACK_NACK { get; set; }

        [DisplayName("CBR NACK Reason")]
        public string CBR_NACK_Reason { get; set; }

        [DisplayName("CBR Report Unique Id")]
        public string CBR_Report_Unique_Id { get; set; }

        [DisplayName("CBR Sent Date")]
        public string CBRSentDate { get; set; }

        [DisplayName("CBR Ack Nack Date")]
        public string CBRAckNackDate { get; set; }

        [DisplayName("Service Tag")]
        public string ServiceTag { get; set; }

        [DisplayName("Order Number")]
        public string SalesOrderNumber { get; set; }


        [DisplayName("Include Archive")]
        [Visibility(false)]
        public bool IncludeArchive { get; set; }
    }
    public class VisibilityAttribute : Attribute
    {
        private bool visible;

        public VisibilityAttribute(bool visible)
        {
            this.visible = visible;
        }

        public bool Visibilty { get { return visible; } }
    }
}
