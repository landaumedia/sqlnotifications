using SqlNotifications.Demo.Scenarios;
using SqlNotifications.Demo.Scenarios.ArticleTable;
using SqlNotifications.Demo.Scenarios.BigTable;

namespace SqlNotifications.Demo
{
    static class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"SERVER=(local)\SQLExpress;Database=testing_sqlnotifications;user=sqlnotifications;password=test";

            //new TestDataArticleGenerator(connectionString).Generate(2000000);
            //new SimpleArticleTableScenario().Start();
            //new LocalUserWithDatabaseStorage().Start();

            //var bigTableScenario = new BigTableScenario();
            //bigTableScenario.Prepare();
            //bigTableScenario.Start();

            new BlogPostScenario().Start();
        }
    }
}
