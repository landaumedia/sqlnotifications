namespace LandauMedia.Wire
{
    public static class Notify
    {
        public static INotificationSetup For()
        {
            return new StandardNotificationSetup();
        }
    }
}