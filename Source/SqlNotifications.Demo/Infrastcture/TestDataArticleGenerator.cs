using System;
using System.Data;
using System.Data.SqlClient;
using NLog;

namespace SqlNotifications.Demo.Infrastcture
{
    public class TestDataArticleGenerator
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        string _connectionString;

        public TestDataArticleGenerator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Generate(int articleCount)
        {
            var rnd = new Random();

            Logger.Info("Start Generating");

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = GenerateCommand(connection))
            {
                connection.Open();

                for (int i = 0; i < articleCount; i++)
                {
                    bool isCancelled = rnd.Next(0, 1000) == 0;              // einer von tausend is canceld
                    DateTime? deliverDate = rnd.Next(0, 100) == 0 ? (DateTime?)null : DateTime.Today.AddDays(rnd.Next(300) * -1);

                    AdaptParameter(command, isCancelled, deliverDate);

                    command.ExecuteNonQuery();
                }
            }

            Logger.Info("Finished Generating");
        }


        private SqlCommand GenerateCommand(SqlConnection connection)
        {
            var command = new SqlCommand(@"INSERT INTO [ArticleTable] ([IsCancelled],[DeliverDate])
                VALUES (@IsCancelled, @DeliverDate)", connection);

            command.Parameters.Add("@IsCancelled", SqlDbType.Bit);
            command.Parameters.Add("@DeliverDate", SqlDbType.DateTime);

            return command;
        }

        void AdaptParameter(SqlCommand command, bool isCancelled, DateTime? deliverDate)
        {
            command.Parameters["@IsCancelled"].Value = isCancelled;

            if(deliverDate.HasValue)
            {
                command.Parameters["@DeliverDate"].Value = deliverDate;
            }
            else
            {
                command.Parameters["@DeliverDate"].Value = DBNull.Value;
            }

        }
    }
}