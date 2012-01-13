using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace LandauMedia
{
    [Subject("CI")]
    public class When_doing_nothing
    {
        Because of = () => _result = false;

        It should_be_false = () =>
            _result.ShouldBeFalse();
        
        static bool _result;
    }
}

// ReSharper restore UnusedMember.Global
// ReSharper restore UnusedMember.Local
// ReSharper restore InconsistentNaming