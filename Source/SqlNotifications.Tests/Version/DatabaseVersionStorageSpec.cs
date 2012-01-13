//using LandauMedia.Storage;
//using Machine.Specifications;

//// ReSharper disable InconsistentNaming
//// ReSharper disable UnusedMember.Local
//// ReSharper disable UnusedMember.Global

//namespace LandauMedia.Version
//{
//    [Subject(typeof(DatabaseVersionStorage))]
//    public class When_DatabaseVersionStorage_Store_value_1_for_key_test : with_Databaseversion_Storage
//    {
//        Because of = () =>
//            _versionStorage.Store("test", 1);

//        It should_return_the_value_one_for_key_test = () =>
//            _versionStorage.Load("test").ShouldEqual((ulong)1);

//        It should_handle_the_key_caseinsensitiv = () =>
//            _versionStorage.Load("TEST").ShouldEqual((ulong)1);

//        It should_return_0_for_non_existingKey = () =>
//            _versionStorage.Load("nonExistingKey").ShouldEqual((ulong)0);

//    }

//    [Subject(typeof(DatabaseVersionStorage))]
//    public class When_Store_a_existing_key : with_Databaseversion_Storage
//    {
//        Establish context = () =>
//            _versionStorage.Store("test", 5);

//        Because of = () =>
//            _versionStorage.Store("test", 10);

//        It should_load_the_new_key = () =>
//            _versionStorage.Load("test").ShouldEqual((ulong)10);
//    }

//    [Subject(typeof(DatabaseVersionStorage))]
//    public class when_store_a_key_with_umlauts : with_Databaseversion_Storage
//    {
//        Because of = () =>
//            _versionStorage.Store("KeyWithUmlautsÄÖÜ", 1);

//        It should_load_the_value_1 = () =>
//            _versionStorage.Load("KeyWithUmlautsÄÖÜ").ShouldEqual((ulong)1);
//    }

//    public class with_Databaseversion_Storage : With_express_database
//    {
//        Establish context = () =>
//            _versionStorage = new DatabaseVersionStorage(_connectionstring);

//        Cleanup cleanup = () =>
//            _versionStorage.Dispose();

//        protected static DatabaseVersionStorage _versionStorage;
//    }
//}

//// ReSharper restore UnusedMember.Global
//// ReSharper restore UnusedMember.Local
//// ReSharper restore InconsistentNaming