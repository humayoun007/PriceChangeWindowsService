using FundScreenerServiceHost.Models;
using FundScreenerServiceHost.PriceChangeContracts;
using FundScreenerServiceHost.PriceChangeService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TableDependency.SqlClient;

namespace FundScreenerServiceHost
{
    public partial class Service1 : ServiceBase
    {
        private readonly SqlTableDependency<Stock> _sqlTableDependency;
        private readonly string _connectionString;
        private readonly IPriceTicker priceTicker;
        
        public Service1()
        {
            InitializeComponent();
            _connectionString = ConfigurationManager
                        .ConnectionStrings["FundScreenerConnection"]
                        .ConnectionString;

            WriteToFile("Initialzie connection string");

            _sqlTableDependency = new SqlTableDependency<Stock>(
                        _connectionString,
                        "Stocks");

            WriteToFile("Initialzie _sqlTableDependency");

            _sqlTableDependency.OnChanged += _sqlTableDependency_OnChanged;
            _sqlTableDependency.OnError += _sqlTableDependency_OnError;
            _sqlTableDependency.Start();

            WriteToFile("_sqlTableDependency started");

            priceTicker = new PriceTicker(_connectionString);

            WriteToFile("Initialzie priceTicker");
        }

        private void _sqlTableDependency_OnChanged(object sender, 
            TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Stock> e)
        {
            ////now it is writing in file but later will pass the data using api call
            WriteToFile(Environment.NewLine);
            WriteToFile($"DML: {e.ChangeType}");
            WriteToFile($"Code: {e.Entity.Code}");
            WriteToFile($"Name: {e.Entity.Name}");
            WriteToFile($"Price: {e.Entity.Price}");
        }

        protected override void OnStart(string[] args)
        {
            foreach (var item in priceTicker.GetAllStocks())
            {
                WriteToFile($"item code: {item.Code}, item name: {item.Name}, item price: {item.Price}");
            }
            
            Thread.Sleep(60000); //1 minutes break
        }        
        
        protected override void OnStop()
        {            
            _sqlTableDependency.Stop();
        }

        private void _sqlTableDependency_OnError(object sender,
            TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            //now it is writing in file 
            WriteToFile($"Error Message: {e.Message}");
            WriteToFile($"Error Message in details: {e.Error?.Message}");
            WriteToFile($"Error Message StackTrace: {e.Error.StackTrace}");
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Log";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        public void Debug()
        {
            OnStart(null);
        }
    }
}
