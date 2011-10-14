using System;
using System.Data.SqlClient;
using LandauMedia.Storage;
using LandauMedia.Wire;

namespace SqlNotifications.Demo.Scenarios.LocalUserWithDatabaseStorage
{
    public class LocalUserWithDatabaseStorage
    {
        string connectionString = "SERVER=Localhost;DATABASE=NotificationTest;User=Guest";

        public void Start()
        {
            Func<Type, INotification> factory = t => (INotification)Activator.CreateInstance(t);

            var notificationTracker = Notify.For()
                .Database(connectionString)
                .WithNotifications(new[] {typeof(UserNotificationSetup)})
                .UseDefaultTimestampBased()
                .WithVersionStorage(new DatabaseVersionStorage(connectionString))
                .WithNotificationFactory(factory)
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

        void UpdateUser()
        {
            string updateUserCommand = "UPDATE [User] SET Description = Description + '1'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(updateUserCommand, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        void InsertUser()
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