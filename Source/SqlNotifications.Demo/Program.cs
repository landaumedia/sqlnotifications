using SqlNotifications.Demo.Scenarios.LocalUser;
using SqlNotifications.Demo.Scenarios.LocalUserWithDatabaseStorage;

namespace SqlNotifications.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            new LocalUserWithDatabaseStorage().Start();
        }
    }
}
