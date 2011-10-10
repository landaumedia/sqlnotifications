using Krowiorsch.Dojo.Wire;

namespace Krowiorsch.Dojo
{
    public class UserNotification : AbstractNotification
    {
        public UserNotification()
        {
            SetTable("User");
            SetKeyColumn("Id");
            SetIdType<int>();

            IntrestedInColumn("Username");
            IntrestedInColumn("Description");
        }
    }
}