using System.Reflection;

namespace Krowiorsch.Dojo.Wire
{
    public interface INotificationSetup
    {
        INotificationSetup ForDatabase(string connectionString);

        INotificationSetup WithNotificationsOfAssembly(Assembly aseembly);

        INotificationSetup PublishingTo(IPublishingNotifications publishing);

        NotificationTracker Build();
    }
}