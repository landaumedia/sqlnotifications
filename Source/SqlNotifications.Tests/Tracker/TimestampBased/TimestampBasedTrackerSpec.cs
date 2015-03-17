using System;
using LandauMedia.Exceptions;
using LandauMedia.Model;
using LandauMedia.Storage;
using LandauMedia.Wire;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable PublicMembersMustHaveComments
namespace LandauMedia.Tracker.TimestampBased
{
    [Subject(typeof(TimestampBasedTracker))]
    public class When_prepare_a_tracker_for_non_existing_Table : With_express_database
    {
        Establish context = () =>
        {
            _notification = new TestNotification();
            _sut = new TimestampBasedTracker();
        };

        Because of = () =>
            _exception = Catch.Exception(() => _sut.Prepare(_connectionstring, new GenericTableSetup("nonExisting", "dbo", "id"), _notification, new InMemoryVersionStorage(), TrackerOptions.Default));

        It should_throw_an_invalidOperation_Exeception = () =>
            _exception.ShouldBeAssignableTo<TableNotExistException>();

        It should_have_an_exception_with_tablename = () =>
            ((TableNotExistException)_exception).TableName.ShouldEqual("nonExisting");

        It should_have_an_exception_with_schemaName = () =>
            ((TableNotExistException)_exception).SchemaName.ShouldEqual("dbo");

        static TimestampBasedTracker _sut;
        static TestNotification _notification;
        static Exception _exception;
    }

    [Subject(typeof(TimestampBasedTracker))]
    public class When_prepare_a_tracker_for_table_without_timestampfield : With_database_with_table_user_on_schema_testing
    {
        Establish context = () =>
        {
            _notification = new TestNotification();
            _sut = new TimestampBasedTracker();
        };

        Because of = () =>
            _exception = Catch.Exception(() => _sut.Prepare(_connectionstring, new GenericTableSetup("User", "Testing", "id"), _notification, new InMemoryVersionStorage(), TrackerOptions.Default));

        It should_throw_an_invalidOperation_Exeception = () =>
            _exception.ShouldBeAssignableTo<InvalidOperationException>();

        It should_have_message_with_tableName = () =>
            _exception.Message.ShouldContain("User");



        static TimestampBasedTracker _sut;
        static TestNotification _notification;
        static Exception _exception;
    }

    [Subject(typeof(TimestampBasedTracker))]
    public class When_prepare_a_tracker_for_table_with_timestampfield : With_database_with_table_user_on_schema_testing_with_timestampfield
    {
        Establish context = () =>
        {
            _notification = new TestNotification();
            _sut = new TimestampBasedTracker();
        };

        Because of = () =>
            _exception = Catch.Exception(() => _sut.Prepare(_connectionstring, new GenericTableSetup("User", "Testing", "id"), _notification, new InMemoryVersionStorage(), TrackerOptions.Default));

        It should__not_throw_everything = () =>
            _exception.ShouldBeNull();

        static TimestampBasedTracker _sut;
        static TestNotification _notification;
        static Exception _exception;
    }

    public class GenericTableSetup : AbstractNotificationSetup
    {
        public GenericTableSetup(string tableName, string schemaName, string id)
        {
            SetSchemaAndTable(tableName, schemaName);
            SetKeyColumn(id);
            SetIdType<int>();
        }

        public override Type Notification
        {
            get { return typeof(TestNotification); }
        }
    }

    public class TestNotification : INotification
    {
        public int CountInserts { get; set; }
        public int CountUpdates { get; set; }
        public int CountDeletes { get; set; }

        public void OnInsert(INotificationSetup notificationSetup, string id, AdditionalNotificationInformation information)
        {
            CountInserts++;
        }

        public void OnUpdate(INotificationSetup notificationSetup, string id, AdditionalNotificationInformation information)
        {
            CountUpdates++;
        }

        public void OnDelete(INotificationSetup notificationSetup, string id, AdditionalNotificationInformation information)
        {
            CountDeletes++;
        }
    }
}

// ReSharper restore UnusedMember.Global
// ReSharper restore UnusedMember.Local
// ReSharper restore InconsistentNaming
// ReSharper restore PublicMembersMustHaveComments