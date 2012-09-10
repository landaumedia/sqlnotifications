using System;
using System.Threading;
using LandauMedia.Storage;
using LandauMedia.Tracker;
using LandauMedia.Wire;

namespace SqlNotifications.Demo.Scenarios.ArticleTable
{
    public class SimpleArticleTableScenario
    {
        public void Start()
        {
            string connectionString = @"SERVER=(local)\SQLExpress;Database=testing_sqlnotifications;user=sqlnotifications;password=test";

            Func<Type, INotification> factory = t => (INotification)Activator.CreateInstance(t);

            var notificationTracker = Notify.For()
                .Database(connectionString)
                .WithNotifications(new[] { typeof(ArticleTableNotificationSetup) })
                .UseDefaultTimestampBased()
                .WithVersionStorage(new InMemoryVersionStorage())
                .WithNotificationFactory(factory)
                .WithDefaultTrackerOptions(new TrackerOptions() {InitializationOptions = InitializationOptions.InitializeToCurrentIfNotSet})
                .Build();


            using (notificationTracker.Start())
            {
                while (true)
                {
                    Thread.Sleep(200);
                }
            }


        } 
    }
}