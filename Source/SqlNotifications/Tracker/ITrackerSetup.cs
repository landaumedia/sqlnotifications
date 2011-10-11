using System.Reflection;
using LandauMedia.Storage;

namespace LandauMedia.Tracker
{
    public interface ITrackerSetup
    {
        ITrackerSetup Database(string connectionString);

        ITrackerSetup WithNotificationsOfAssembly(Assembly aseembly);

        ITrackerSetup UseDefaultChangeTracking();

        ITrackerSetup UseDefaultTimestampBased();

        ITrackerSetup WithVersionStorage(IVersionStorage storage);

        NotificationTracker Build();
    }
}