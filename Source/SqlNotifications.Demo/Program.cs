using SqlNotifications.Demo.Scenarios;

namespace SqlNotifications.Demo
{
    static class Program
    {
        static void Main(string[] args)
        {
            //new LocalUserWithDatabaseStorage().Start();

            //var bigTableScenario = new BigTableScenario();
            //bigTableScenario.Prepare();
            //bigTableScenario.Start();

            new BlogPostScenario().Start();
        }
    }
}
