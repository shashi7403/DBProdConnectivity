using CDA.Dell.Contarct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dell.CDA.OracleDataAccess
{
    public interface IReportDataAccess
    {
        List<CBRQueryViewDTO> GetCBRQueryViewData(CBRQueryViewDTO cbrQuery);
    }
}
