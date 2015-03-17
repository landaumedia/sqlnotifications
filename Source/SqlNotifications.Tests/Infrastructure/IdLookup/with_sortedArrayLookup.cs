using Machine.Specifications;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local



namespace LandauMedia.Infrastructure.IdLookup
{
    internal class with_sortedArrayLookup
    {
        Establish context = () =>
            _sut = new SortedArrayLookupTable();

        protected static SortedArrayLookupTable _sut;
    }

    [Subject("IdChecking")]
    internal class when_check_for_nonexisting_key_as_string : with_sortedArrayLookup
    {
        Because of = () =>
            _result = _sut.Contains("1");

        It should_be_false = () =>
            _result.ShouldBeFalse();

        static bool _result;
    }

    [Subject("IdChecking")]
    internal class when_check_for_existing_key_as_string : with_sortedArrayLookup
    {
        Establish context = () =>
            _sut.Add("1");

        Because of = () =>
            _result = _sut.Contains("1");

        It should_be_false = () =>
            _result.ShouldBeTrue();

        static bool _result;
    }
}