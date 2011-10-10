using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace LandauMedia
{
    public class With_local_SqlStandardDatabase
    {
        protected static string _connectionString;

        Establish context = () =>
            _connectionString = "SERVER=Localhost;DATABASE=NotificationTest;User=Guest";
    }
}

// ReSharper restore UnusedMember.Global
// ReSharper restore UnusedMember.Local
// ReSharper restore InconsistentNaming