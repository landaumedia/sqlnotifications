using System;
using System.Data.SqlClient;
using System.Reflection;

namespace SqlNotifications.Demo
{
    class Program
    {
        static string connectionString = "SERVER=Localhost;DATABASE=NotificationTest;User=Guest";

        static void Main(string[] args)
        {
            var notificationTracker = LandauMedia.Tracker.Notifications.WireUp()
                .ForDatabase(connectionString)
                .WithNotificationsOfAssembly(Assembly.GetExecutingAssembly())
                .UseDefaultTimestampBased()
                .Build();

            using (notificationTracker.Start())
            {
                string readLine;

                while ((readLine = Console.ReadLine()) != "quit")
                {
                    if (readLine == null)
                        continue;

                    if (readLine.StartsWith("u"))
                    {
                        UpdateUser();
                    }

                    if (readLine.StartsWith("i"))
                    {
                        InsertUser();
                    }
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
                command.ExecuteNonQuery();
            }
        }

        static void InsertUser()
        {
            string updateUserCommand = "INSERT INTO [User] ([Username],[Description]) VALUES (@1, @2)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(updateUserCommand, connection))
            {
                command.Parameters.AddWithValue("@1", new Guid().ToString());
                command.Parameters.AddWithValue("@2", "Description");

                connection.Open();
                command.ExecuteScalar();
            }
        }
    }
}
