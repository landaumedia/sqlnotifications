using System;
using System.Data;
using System.Data.SqlClient;
using LandauMedia.Storage;
using LandauMedia.Wire;
using NLog;

namespace SqlNotifications.Demo.Scenarios.BigTable
{
    public class BigTableScenario
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        const string ConnectionString = "SERVER=Localhost;DATABASE=NotificationTest;User=Guest";
        const int MinRowsCount = 2000000;

        public void Prepare()
        {
            const string countStatement = @"SELECT Count(*) FROM [BigTable]";
            const string insertStatement = @"INSERT INTO [BigTable] ([ID]) VALUES (@Id)";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand insertCommand = new SqlCommand(insertStatement, connection))
            using (SqlCommand countCommand = new SqlCommand(countStatement, connection))
            {
                connection.Open();
                insertCommand.Parameters.Add("@Id", SqlDbType.Int);

                int count = (int)countCommand.ExecuteScalar();

                if (count > MinRowsCount)
                    return;

                int insert = MinRowsCount - count;

                for(int i = 0; i < insert; i++)
                {
                    insertCommand.Parameters["@Id"].Value = i;
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        public void Start()
        {
            Func<Type, INotification> factory = t => (INotification)Activator.CreateInstance(t);

            var notificationTracker = Notify.For()
                .Database(ConnectionString)
                .WithNotifications(new[] { typeof(BigTableNotificationSetup) })
                .UseDefaultTimestampBased()
                .WithVersionStorage(new FilebasedVersionStorage("versions.storage"))
                .WithNotificationFactory(factory)
                .Build();

            using (notificationTracker.Start())
            {
                Logger.Info(() => "Start Notify");

                string readLine;

                while ((readLine = Console.ReadLine()) != "quit")
                {
                    if (readLine == null)
                        continue;

                    if (readLine.StartsWith("i"))
                    {
                        InsertSomething();
                    }
                }
            }


        }
        
        void InsertSomething()
        {
            string insertStatement = "INSERT INTO [BigTable] ([ID]) VALUES (@1)";
            string countStatement = "SELECT Count(*) FROM BigTable";


            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand insertCommand = new SqlCommand(insertStatement, connection))
            using (SqlCommand countCommand = new SqlCommand(countStatement, connection))
            {
                connection.Open();

                var maxValue = (int)countCommand.ExecuteScalar() + 1;

                insertCommand.Parameters.AddWithValue("@1", maxValue);
                
                insertCommand.ExecuteScalar();
            }
        }
    }
}