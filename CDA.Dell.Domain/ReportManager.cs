using CDA.Dell.Contarct;
using Dell.CDA.OracleDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDA.Dell.Domain
{
    public class ReportManager
    {
        public IReportDataAccess reportDA
        {
            get { return new ReportDataAccess(); }
        }

        public List<CBRQueryViewDTO> GetCBRQueryViewData(CBRQueryViewDTO cbrQueryViewDTO)
        {

            try
            {

                return reportDA.GetCBRQueryViewData(cbrQueryViewDTO);

            }
            catch (Exception ex)
            {
                // LoggingHelper.Log4NetLogger.Error(errorMessage, ex);
            }
            return null;
        }
    }
}
