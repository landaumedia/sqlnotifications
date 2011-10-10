using System;
using System.Data.SqlClient;
using System.Reflection;
using Krowiorsch.Dojo;
using Krowiorsch.Dojo.Wire;

namespace SqlNotifications.Demo
{
    class Program
    {
        static string connectionString = "SERVER=Localhost;DATABASE=NotificationTest;User=Guest";

        static void Main(string[] args)
        {


            var notificationTracker = Notifications.WireUp()
                .ForDatabase(connectionString)
                .WithNotificationsOfAssembly(Assembly.GetExecutingAssembly())
                .PublishingTo(new NLogPublisher())
                .Build();

            notificationTracker.Start();

            var readLine = string.Empty;

            while((readLine = Console.ReadLine()) != "quit")
            {
                if (readLine.StartsWith("u"))
                {
                    UpdateUser();
                }
            }
        }

        static void UpdateUser()
        {
            string updateUserCommand =  "UPDATE [User] SET Description = Description + '1'" ;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(updateUserCommand, connection))
            {
                connection.Open();
                command.ExecuteScalar();
            }
        }
    }
}
