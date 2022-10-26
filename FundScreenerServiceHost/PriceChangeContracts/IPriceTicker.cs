using FundScreenerServiceHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundScreenerServiceHost.PriceChangeContracts
{
    public interface IPriceTicker
    {
        List<Stock> GetAllStocks();
    }
}
