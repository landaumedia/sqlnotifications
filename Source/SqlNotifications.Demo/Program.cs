using SqlNotifications.Demo.Scenarios.LocalUser;

namespace SqlNotifications.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            new LocalUserScenario().Start();
        }
    }
}
