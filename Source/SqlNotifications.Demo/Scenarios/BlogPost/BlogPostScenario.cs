using System;
using System.Reactive.Linq;
using LandauMedia.Infrastructure;
using LandauMedia.Storage;
using LandauMedia.Tracker;
using LandauMedia.Wire;

namespace SqlNotifications.Demo.Scenarios
{
    public class BlogPostScenario
    {
        public void Start()
        {
            DictionaryCounter counter = new DictionaryCounter();

            var notificationTracker = Notify.For()
                .Database("Data Source=Web20TestingDB;Initial Catalog=_testing_Web20;User ID=Web20User;Password=2s1DOmUqSdF1Isq8cXmG;Application Name=\"CommandCenter: FeedExporter.vshost.exe\"")
                .WithNotifications(new[] { typeof(BlogPostNotificationSetup), typeof(ForumPostNotificationSetup) })
                .UseDefaultTimestampBased()
                .WithVersionStorage(new FilebasedVersionStorage("versions.storage"))
                .WithDefaultTrackerOptions(new TrackerOptions {BucketSize = 10000, InitializationOptions = InitializationOptions.InitializeToCurrent})
                .WithPerformanceCounter(counter)
                .Build();

            Observable.Interval(TimeSpan.FromSeconds(5))
                .Subscribe(i => Console.WriteLine("DatabaseAccess:" + counter.ByName("DatabaseQuery")));

            using (notificationTracker.Start()) 
            {
                Console.WriteLine("Press any key");
                Console.ReadLine();
            }
        }
    }
}