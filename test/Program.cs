using Imps.Services.CommonV4;
using Imps.Services.CommonV4.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var _perfCounters = IICPerformanceCounterFactory.GetCounters<DatabasePerfCounters>(".");
            while (true)
            {
                _perfCounters.CommandFailedTotal.Increment();
                _perfCounters.CommandExecutedTotal.Increment();
                System.Threading.Thread.Sleep(1000);
            }

        }
    }
}
