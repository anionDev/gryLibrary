# GenericToString

`GenericToString` provides a simple `ToString`-function for any object.

Sometimes you need a simple quick serializer for your objects. For example if your unit-testcase `Assert.AreEqual(object1, object2)` fails and the type of the objects `object1` and `object2` does not overwrite [ToString](https://docs.microsoft.com/de-de/dotnet/api/system.object.tostring) then you get the message `Assert.AreEqual failed. Expected:<System.Object>. Actual:<System.Object>.` This will probably never really help anyone.
What to do ti avoid this? It is always the same: Write something like
```
public override string ToString()
{
  return $"{nameof(Property1)}: '{Property1}', {nameof(Property2)}: '{Property2}'";
}
```
This is an annoying work. And sometimes it make problems: You can not everywhere implement a `ToString`-function this way because you can get `StackOverflowException`s if all types of the objects in a cyclic-datastructure implement its `ToString`-function this way. So there is a simple always-the-same-onliner-alternative which is simply copy- & paste-able which can handle cycles:
```
using GRYLibrary.Core.AdvancedObjectAnalysis;

namespace MyNamespace{

  class MyClass{

    //...

    #region Overhead

    public override string ToString()
    {
      return Generic.GenericToString(this);
    }

    #endregion

  }

}
```

This will produce a JSON-like object which contains the type and the properties with their values of your object.
`GenericToString` ignores all `ToString-functions` of any object and any attribute to preserve a real JSON-like-look. This works well to compare 2 objects without the requirement of a Debugger to "scroll" through the properties of your object. You also do not need an annoying and boring always-the-same-work-task to implement the `ToString`-method which would always have to be updated when a property of a type will be added or removed.