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

            var connectionstring =
                "Data Source=Web20StagingDB;Initial Catalog=_staging_Web20;User ID=Web20User;Password=2s1DOmUqSdF1Isq8cXmG;Application Name=\"CommandCenter: FeedExporter.vshost.exe\"";

            var notificationTracker = Notify.For()
                .Database(connectionstring)
                .WithNotifications(new[] { typeof(BlogPostNotificationSetup), typeof(ForumPostNotificationSetup) })
                .UseDefaultTimestampBased()
                .WithVersionStorage(new DatabaseVersionStorage(connectionstring, "VersionsTest"))
                .WithDefaultTrackerOptions(new TrackerOptions
                {
                    BucketSize = 10000, 
                    InitializationOptions = InitializationOptions.InitializeToCurrentIfNotSet,
                    FetchInterval = TimeSpan.FromSeconds(5)
                })
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