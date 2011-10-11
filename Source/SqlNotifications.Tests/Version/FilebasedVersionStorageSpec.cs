using System.IO;
using LandauMedia.Wire;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace LandauMedia.Version
{
    [Subject(typeof(FilebasedVersionStorage))]
    public class When_store_a_int_1_with_key_test : With_version_storge_local_File_that_doesn_exist
    {
        Because of = () =>
            _sut.Store("test", 1);

        It should_have_created_the_file = () =>
            _file.Exists.ShouldBeTrue();

        It should_restore_the__value_of_one = () =>
            _sut.Load("test").ShouldEqual<ulong>(1);

        It should_restore_the_default_value_for_nonexisting_enties = () =>
            _sut.Load("nonExisting").ShouldEqual<ulong>(0);
    }

    public class With_version_storge_local_File_that_doesn_exist
    {
        Establish that = () =>
        {
            if(_file.Exists)
                _file.Delete();

            _sut = new FilebasedVersionStorage(_file);
        };
            

        protected static FilebasedVersionStorage _sut;
        protected static FileInfo _file = new FileInfo(@"temp\123.storage");
    }
}

// ReSharper restore UnusedMember.Global
// ReSharper restore UnusedMember.Local
// ReSharper restore InconsistentNaming