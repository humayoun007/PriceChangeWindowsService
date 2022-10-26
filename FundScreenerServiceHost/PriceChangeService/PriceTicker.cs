using FundScreenerServiceHost.Models;
using FundScreenerServiceHost.PriceChangeContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundScreenerServiceHost.PriceChangeService
{
    public class PriceTicker : IPriceTicker, IDisposable
    {
        private readonly string connectionString;

        public PriceTicker(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Stock> GetAllStocks()
        {
            var stocks = new List<Stock>();

            using (var sqlConnection = new SqlConnection(this.connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "SELECT * FROM [Stocks]";

                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            var code = sqlDataReader
                                    .GetString(sqlDataReader
                                    .GetOrdinal("code"));
                            var name = sqlDataReader
                                    .GetString(sqlDataReader
                                    .GetOrdinal("Name"));
                            var price = sqlDataReader
                                    .GetDecimal(sqlDataReader
                                    .GetOrdinal("Price"));

                            stocks.Add(new Stock
                            {
                                Code = code,
                                Name = name,
                                Price = price
                            });
                        }
                    }
                }
            }

            return stocks;
        }

       

        public void Dispose()
        {            
        }
    }
}
