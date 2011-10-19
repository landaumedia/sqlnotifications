using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using SqlNotifications.Demo.Scenarios.BigTable;
using SqlNotifications.Demo.Scenarios.LocalUser;

namespace SqlNotifications.Demo
{
    static class Program
    {
        static void Main(string[] args)
        {
            //new LocalUserScenario().Start();

            var bigTableScenario = new BigTableScenario();

            bigTableScenario.Prepare();
            bigTableScenario.Start();

        }
    }
}
