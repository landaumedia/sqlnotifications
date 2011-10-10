using System;
using System.Reflection;
using Krowiorsch.Dojo.Wire;

namespace Krowiorsch.Dojo
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ConnectionString = "SERVER=Localhost;DATABASE=NotificationTest;User=Guest";

            var notificationTracker = Notifications.WireUp()
                .ForDatabase(ConnectionString)
                .WithNotificationsOfAssembly(Assembly.GetExecutingAssembly())
                .PublishingTo(new NLogPublisher())
                .Build();

            notificationTracker.Start();

            Console.ReadLine();
            
        }
    }
}
