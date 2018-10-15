using CDA.Dell.Contarct;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

namespace Dell.CDA.OracleDataAccess
{
    public class ReportDataAccess : IReportDataAccess
    {
        public ReportDataAccess()
        {
            DBHelper.SetConnectionString(DBHelper.DatabaseType.LKM);
        }
        public List<CBRQueryViewDTO> GetCBRQueryViewData(CBRQueryViewDTO cbrQueryViewDTO)
        {
            OracleDataReader reader = null;
            try
            {
                reader = DBHelper.ExecuteReader("LKM.PKG_LKM_OEM_REPORT.PRC_CBRQUERY_KEYS_VIEW",
                   new OracleParameter[]
                            {
                                    DBHelper.AddInParameter("i_DPKSerialNo", OracleDbType.Clob, cbrQueryViewDTO.DPKSerialNumber),
                                    DBHelper.AddInParameter("ip_is_archive", OracleDbType.Varchar2, cbrQueryViewDTO.IncludeArchive== true ? 'Y' : 'N'),
                                    DBHelper.AddOutParameter("o_result", OracleDbType.RefCursor)
                            });
                List<CBRQueryViewDTO> cbrQueryViewList = null;
                if (reader != null && reader.HasRows)
                {
                    cbrQueryViewList = new List<CBRQueryViewDTO>();
                    while (reader.Read())
                    {
                        CBRQueryViewDTO cbrQueryView = new CBRQueryViewDTO();
                        cbrQueryView.DPKSerialNumber = reader.IsDBNull(reader.GetOrdinal("PRODUCT_KEY_SERIAL_NUMBER")) ? string.Empty
                            : Convert.ToString(reader["PRODUCT_KEY_SERIAL_NUMBER"]);
                        cbrQueryView.DPKStatus = reader.IsDBNull(reader.GetOrdinal("DPKStatus")) ? string.Empty
                            : Convert.ToString(reader["DPKStatus"]);

                        cbrQueryView.CBRStatus = reader.IsDBNull(reader.GetOrdinal("CBRStatus")) ? string.Empty
                            : Convert.ToString(reader["CBRStatus"]);
                        cbrQueryView.CBR_ACK_NACK = reader.IsDBNull(reader.GetOrdinal("CBR_ACK_NACK")) ? string.Empty
                            : Convert.ToString(reader["CBR_ACK_NACK"]);
                        cbrQueryView.CBR_NACK_Reason = reader.IsDBNull(reader.GetOrdinal("CBR_NACK_Reason")) ? string.Empty
                            : Convert.ToString(reader["CBR_NACK_Reason"]);
                        cbrQueryView.CBR_Report_Unique_Id = reader.IsDBNull(reader.GetOrdinal("CBR_Report_UniqaueId")) ? string.Empty
                            : Convert.ToString(reader["CBR_Report_UniqaueId"]);

                        cbrQueryView.CBRSentDate = reader.IsDBNull(reader.GetOrdinal("CBR_Sent_Date")) ? string.Empty
                            : Convert.ToString(reader["CBR_Sent_Date"]);

                        cbrQueryView.CBRAckNackDate = reader.IsDBNull(reader.GetOrdinal("CBR_ACK_NACK_Date")) ? string.Empty
                            : Convert.ToString(reader["CBR_ACK_NACK_Date"]);
                        cbrQueryView.ServiceTag = reader.IsDBNull(reader.GetOrdinal("Service_Tag")) ? string.Empty
                            : Convert.ToString(reader["Service_Tag"]);
                        cbrQueryView.SalesOrderNumber = reader.IsDBNull(reader.GetOrdinal("ORDER_NUMBER")) ? string.Empty
                            : Convert.ToString(reader["ORDER_NUMBER"]);

                        cbrQueryViewList.Add(cbrQueryView);
                    }
                }
                return cbrQueryViewList;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
