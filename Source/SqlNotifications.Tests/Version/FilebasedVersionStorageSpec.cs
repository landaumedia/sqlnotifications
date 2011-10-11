using System.IO;
using LandauMedia.Wire;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace LandauMedia.Version
{
    [Subject(typeof(FilebasedVersionStorage))]
    public class When_store_a_int_object : With_version_storge_local_File_that_doesn_exist
    {
        Because of = () =>
            _sut.Store(1);

        It should_have_created_the_file = () =>
            _file.Exists.ShouldBeTrue();
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