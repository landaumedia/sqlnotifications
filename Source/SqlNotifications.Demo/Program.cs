using SqlNotifications.Demo.Scenarios.BigTable;
using SqlNotifications.Demo.Scenarios.LocalUserWithDatabaseStorage;

namespace SqlNotifications.Demo
{
    static class Program
    {
        static void Main(string[] args)
        {
            new LocalUserWithDatabaseStorage().Start();

            //var bigTableScenario = new BigTableScenario();

            //bigTableScenario.Prepare();
            //bigTableScenario.Start();

        }
    }
}
