using System;
using System.Reflection;
using Krowiorsch.Dojo;
using Krowiorsch.Dojo.Wire;

namespace SqlNotifications.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "SERVER=Localhost;DATABASE=NotificationTest;User=Guest";

            var notificationTracker = Notifications.WireUp()
                .ForDatabase(connectionString)
                .WithNotificationsOfAssembly(Assembly.GetExecutingAssembly())
                .PublishingTo(new NLogPublisher())
                .Build();

            notificationTracker.Start();

            Console.ReadLine();
        }
    }
}
