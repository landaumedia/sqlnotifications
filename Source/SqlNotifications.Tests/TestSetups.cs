using System;
using System.Data.SqlClient;
using System.IO;
using LandauMedia.Infrastructure.SqlTasks;
using Machine.Fakes;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable PublicMembersMustHaveComments

namespace LandauMedia
{
    public class With_local_SqlStandardDatabase
    {
        protected static string _connectionString;

        Establish context = () =>
            _connectionString = "SERVER=Localhost;DATABASE=NotificationTest;User=Guest";
    }


    public class With_express_database : WithFakes
    {
        protected static string _connectionstring;

        Establish context = () =>
        {
            var pathtodb = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sample.mdf");
            
            _connectionstring =
                string.Format(@"Data Source=(local);AttachDbFilename={0};Integrated Security=True;User Instance=True", pathtodb);
        };
    }

    public class With_database_with_table_user_on_schema_testing : With_express_database
    {
        Establish context = () =>
        {
            new SqlConnection(_connectionstring).ExecuteCommand(@"CREATE SCHEMA Testing");
            new SqlConnection(_connectionstring).ExecuteCommand(@"CREATE TABLE Testing.[User] (	Id int NOT NULL	)");
        };

        Cleanup removeobjects = () =>
        {
            new SqlConnection(_connectionstring).ExecuteCommand(@"DROP TABLE Testing.[User]");
            new SqlConnection(_connectionstring).ExecuteCommand(@"DROP SCHEMA Testing");
        };
    }

    public class With_database_with_table_user_on_schema_testing_with_timestampfield : With_express_database
    {
        Establish context = () =>
        {
            new SqlConnection(_connectionstring).ExecuteCommand(@"CREATE SCHEMA Testing");
            new SqlConnection(_connectionstring).ExecuteCommand(@"CREATE TABLE Testing.[User] (	Id int NOT NULL, ts timestamp NOT NULL)");
        };

        Cleanup removeobjects = () =>
        {
            new SqlConnection(_connectionstring).ExecuteCommand(@"DROP TABLE Testing.[User]");
            new SqlConnection(_connectionstring).ExecuteCommand(@"DROP SCHEMA Testing");
        };
    }
}

// ReSharper restore UnusedMember.Global
// ReSharper restore UnusedMember.Local
// ReSharper restore InconsistentNaming
// ReSharper restore PublicMembersMustHaveComments