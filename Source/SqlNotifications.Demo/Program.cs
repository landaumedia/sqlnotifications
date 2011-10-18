using SqlNotifications.Demo.Scenarios.LocalUser;

namespace SqlNotifications.Demo
{
    static class Program
    {
        static void Main(string[] args)
        {
            new LocalUserScenario().Start();
        }
    }
}
