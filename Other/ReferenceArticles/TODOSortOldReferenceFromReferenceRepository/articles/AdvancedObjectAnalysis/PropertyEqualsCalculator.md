# PropertyEqualsCalculator

`PropertyEqualsCalculator` is a type which basically contains an equals-function to determine if two given objects are equal.
It should be as easy as possible to use.
`PropertyEqualsCalculator` has some special properties/features. `PropertyEqualsCalculator`
- treats two objects as equal if their type and the values of their properties are equal0
- can handle cyclic attribute-references
- ignores the [Equals](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals)-operation
- treats to enumerables as equal if and only if their elements are equal
When two objects will be compared then by default only non-static properties with public getter and setter are included, fields and other properties will be ignored. This behavior is configurable.

`PropertyEqualsCalculator` will help you to make some things easier:
When you want to implement `Equals` in the standard way ("another object is equal to `this` object when the type and the properties of the object is equal to this) you simply call an always-the-same-onliner which is simply copy- & paste-able and does always work for datastructures which are equal when the types and their properties are equal.
If you add or remove a property after using `PropertyEqualsCalculator` in the `Equals`- and `GetHashCode`-method then you do not must to remember anymore that you must also adjust the `Equals`- and `GetHashCode`-method when you add or remove a property. Furthermore common flaws in the `Equals`-implementation like `this.ListProperty.Equals(otherObject.ListProperty)` (which should probably be `this.ListProperty.SequenceEqual(otherObject.ListProperty)` in the most cases) can not happen anymore.

## Usage

The most simple way to use `PropertyEqualsCalculator` is to call the static methods from the `Generic`-type which is prepared to be used exactly in this usecase:

```
using GRYLibrary.Core.AdvancedObjectAnalysis;

namespace MyNamespace{

  class MyClass{

    //...

    #region Overhead

    public override bool Equals(object @object)
    {
      return Generic.GenericEquals(this, @object);
    }


    public override int GetHashCode()
    {
      return Generic.GenericGetHashCode(this);
    }

    #endregion

  }

}
```

## Configuration

You can configure some properties for the equals-comparison process, e. g. you can configure that every public field and no property will be used.
An example-way to do this is shown in the following listing:

```
using GRYLibrary.Core.AdvancedObjectAnalysis;

namespace MyNamespace{

  class MyClass{

    //...

    #region Overhead

    private readonly PropertyEqualsCalculator _PropertyEqualsCalculator = new PropertyEqualsCalculator(GetPropertyEqualsCalculatorConfiguration());

    private static PropertyEqualsCalculatorConfiguration GetPropertyEqualsCalculatorConfiguration()
    {
      PropertyEqualsCalculatorConfiguration configuration = new PropertyEqualsCalculatorConfiguration();
      configuration.FieldSelector = (field) => field.IsPublic;
      configuration.PropertySelector = (property) => false;
      // configuration.CustomComparer.Add(myCustomComparer)
      return configuration;
    }

    public override bool Equals(object @object)
    {
      return this._PropertyEqualsCalculator.Equals(this, @object);
    }

    public override int GetHashCode()
    {
      return this._PropertyEqualsCalculator.GetHashCode(this);
    }

    #endregion

  }

}
```

You can also edit the list of custom comparer by editing the `PropertyEqualsCalculatorConfiguration.CustomComparer`-list.

## Technical information

The configuration will be applied for *every* object which will is involved when `PropertyEqualsCalculatorConfiguration` is called.
So `FieldSelector` and `PropertySelector` will be used to select not only the properties and fields of the type of the object where the equals-method is called on but also all other properties and fields of the types which can be "reached" transitively from the object where the equals-method is called on.

PropertyEqualsCalculator knows the following types of objects:
- complex objects
- enumerables
- primitive objects

A type (let's call it `type`) will be treated as primitive if and only if
- `type.IsPrimitive` or
- `typeof(string).Equals(type)` or
- `type.IsValueType`

An object will be treated as enumerable if and only if its type inherits from [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable?view=netcore-3.1).

`PropertyEqualsCalculatorConfiguration` has a list of `CustomComparer`. A `CustomComparer` compares to object to "overwrite" the default equals-implementation (which compares the properties/fields of 2 objects). A `CustomComparer` has the following functions:
- `IsApplicable`: To say if the `Customparer` should be applied to 2 objects of a given type.
- `Equals`: To say if two objects are equal (will only be called when `IsApplicable` returns true).
- `GetHashCode`: To calculate a hashcode for a given object.

By default the following `Customparer` will be added to `PropertyEqualsCalculatorConfiguration`:
- `PrimitiveComparer` (applies to objects will be treated as primitive)
- `KeyValuePairComparer` (applies to objects whose type inherits from [KeyValuePair<TKey,TValue>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2))
- `TupleComparer` (applies to objects whose type inherits from [Tuple<T1,T2>](https://docs.microsoft.com/en-us/dotnet/api/system.tuple)))
- `ListComparer` (applies to objects whose type inherits from [IList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ilist) or [IList<T>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1))
- `SetComparer` (applies to objects whose type inherits from [ISet](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iset-1)))
- `DictionaryComparer` (applies to objects whose type inherits from [IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.idictionary) or [Dictionary<TKey,TValue>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2))
- `EnumerableComparer` (applies to objects whose type inherits from [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable))

You may wonder why there is a `CustomComparer` for lists, sets and dictionaries when there is a comparer for enumerables. The reason is that some some enumerables have "special properties". For example: `IList`s have an order, `ISet`s does not have an order. The comparer should consider this: Two `IList`s `[a,b,c]` and `[a,b,c]` are not equal. To `ISet`s `[a,b,c]` and `[a,b,c]` are equal. The equals-implementation should usually consider this properties. `PropertyEqualsCalculator` consider this.
You can edit the list of `CustomComparer`. When doing this you should consider:
- To decide which `CustomComparer` will be applied all of the `CustomComparer` will be asked if it should be applied in the order in which they are in the `CustomComparer`-list. The first Comparer whose `IsApplicable`-function returns true will be used.
- When `EnumerableComparer` comes before `ListComparer` (or `SetComparer` or `DictionaryComparer`) then `ListComparer` (or `SetComparer` or `DictionaryComparer`) will obviously never be applied because the objects for this comparer will be "grabbed" by `EnumerableComparer`. You must consider this behavior when adding your own `CustomComparer`.
- [string](https://docs.microsoft.com/en-us/dotnet/api/system.string) inherits from [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable) so `string`s can be grabbed by the `EnumerableComparer`.

If you call `PropertyEqualsCalculator.Equals(...)` then the objects which will be compared can have cyclic attributes.
Example:
`object1` has the attribute `attribute1` whose target is `object2`. `object2` has the attribute `attribute2` whose target is `object1`. (This would obviously be a cycle with 2 objects. You can also have objects with cyclic attributes with 1 object (an object have an attribute to itself) or with more than 2 objects ("longer" attribute-cycle).)
All of these cycles should be detected by `PropertyEqualsCalculator`. Currently there are no known bugs which could raise `StackOverflowException`s. This does not mean that there are no Stackoverflow-bugs. Since `StackOverflowException`s are a a little bit tricky (because you can not catch them if they are raised due to a StackOverflow) you must keep in mind: Even if the `GRYLibrary`-project has some testcases for cycle-detection you should also test it with your data before using it productively. `PropertyEqualsCalculator` tries to be as much as possible generic to be applicable many objects. There are no requirements to your objects to compare. Feel free to create big and exotic objects and use them with `PropertyEqualsCalculator`. If `PropertyEqualsCalculator` does not work properly then please report a bug by creating an appropriate [issue](https://github.com/anionDev/gryLibrary/issues).
