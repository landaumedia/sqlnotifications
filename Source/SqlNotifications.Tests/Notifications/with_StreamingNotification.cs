// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

using System;
using System.Collections.Generic;
using Machine.Specifications;

namespace LandauMedia.Notifications
{
    public class with_StreamingNotification
    {
        Establish context = () =>
        {
            _subject = new StreamingNotification<string>((id,info) => id);
            _subject.DeleteStream.Subscribe(s => _deletionList.Add(s));
            _subject.UpdateStream.Subscribe(s => _updateList.Add(s));
            _subject.InsertStream.Subscribe(s => _insertList.Add(s));
        };
            

        protected static StreamingNotification<string> _subject;

        protected static List<string> _deletionList = new List<string>();
        protected static List<string> _updateList = new List<string>();
        protected static List<string> _insertList = new List<string>();
    }

    [Subject(typeof(StreamingNotification<>))]
    public class when_have_one_insert : with_StreamingNotification
    {
        Because of = () =>
            _subject.OnInsert(null, "insert", null);

        It should_have_one_insert = () =>
            _insertList.Count.ShouldEqual(1);

        It should_have_an_item_with_string_1 = () =>
            _insertList[0].ShouldEqual("insert");
    }

    [Subject(typeof(StreamingNotification<>))]
    public class when_have_one_delete : with_StreamingNotification
    {
        Because of = () =>
            _subject.OnDelete(null, "delete", null);

        It should_have_one_delete = () =>
            _deletionList.Count.ShouldEqual(1);

        It should_have_an_item_with_string_1 = () =>
            _deletionList[0].ShouldEqual("delete");
    }
    
    [Subject(typeof(StreamingNotification<>))]
    public class when_have_one_update : with_StreamingNotification
    {
        Because of = () =>
            _subject.OnUpdate(null, "update", null);

        It should_have_one_delete = () =>
            _updateList.Count.ShouldEqual(1);

        It should_have_an_item_with_string_1 = () =>
            _updateList[0].ShouldEqual("update");
    }

}