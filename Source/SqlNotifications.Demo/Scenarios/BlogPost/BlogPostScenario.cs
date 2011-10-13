using System;
using LandauMedia.Storage;
using LandauMedia.Wire;

namespace SqlNotifications.Demo.Scenarios
{
    public class BlogPostScenario
    {
        public void Start()
        {
            var notificationTracker = Notify.For()
                .Database("Data Source=Web20TestingDB;Initial Catalog=_testing_Web20;User ID=Web20User;Password=2s1DOmUqSdF1Isq8cXmG;Application Name=\"CommandCenter: FeedExporter.vshost.exe\"")
                .WithNotifications(new[] { typeof(BlogPostNotificationSetup), typeof(ForumPostNotificationSetup) })
                .UseDefaultTimestampBased()
                .WithVersionStorage(new FilebasedVersionStorage("versions.storage"))
                .Build();

            notificationTracker.Start();

            Console.WriteLine("Press any key");
            Console.ReadLine();
        }
    }
}