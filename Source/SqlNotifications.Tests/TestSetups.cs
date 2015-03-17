using System;
using System.Data.SqlServerCe;
using System.IO;
using LandauMedia.TestHelper;
using Machine.Fakes;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
namespace LandauMedia
{
    [Tags("LocalDatabase")]
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
    
    [Tags("CompactDatabase")]
    public class with_new_compact_database : WithFakes
    {
        protected static string _connectionstring;
        protected static SqlCeConnection _connection;

        Establish context = () =>
        {
            _connectionstring = string.Format("DataSource=\"{0}\"; Password='{1}'", "test.sdf", "test");

            if (File.Exists("test.sdf"))
                File.Delete("test.sdf");

            var en = new SqlCeEngine(_connectionstring);
            en.CreateDatabase();

            _connection = new SqlCeConnection(_connectionstring);
            _connection.Open();
        };


        Cleanup clean = () => _connection.Close();
    }

    public class With_database_with_table_user_on_schema_testing : with_new_compact_database
    {
        Establish context = () => _connection.ExecuteCommand(@"CREATE TABLE [User] (	Id int NOT NULL	)");
        Cleanup removeobjects = () => _connection.ExecuteCommand(@"DROP TABLE [User]");
    }

    public class With_database_with_table_user_on_schema_testing_with_timestampfield : with_new_compact_database
    {
        Establish context = () => _connection.ExecuteCommand(@"CREATE TABLE [User] (	Id int NOT NULL, ts timestamp NOT NULL)");
        Cleanup removeobjects = () => _connection.ExecuteCommand(@"DROP TABLE [User]");
    }
}
