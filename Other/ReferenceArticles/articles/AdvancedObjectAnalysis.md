# AdvancedObjectAnalysis

AdvancedObjectAnalysis is the name of a set of some types in the GRYLibrary. These types are doing certain common things generically. This things are the following functions:

- Equals/GetHashCode
- ToString
- Serialize/Deserialize

# Background

In nearly every type you create you must overwrite the Equals- and GetHashCode-method. Often this is semantically the same code, but you can not simply copy&paste it because the names of the variables are different.

Example: 

We create a type and must overwrite the Equals- and GetHashCode-method. To objects (of this type) should be equal when their most conrete types are equals and their property-values are equal.

We do this very often but we still have some recurring issues:

- Are everywhere (where it is required) null-checks?
- Is [SequenceEqual](https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.sequenceequal?view=netcore-3.1) used when the targettype of a property is a [IList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ilist?view=netcore-3.1)?
- Is [SetEquals](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iset-1.setequals?view=netcore-3.1) used when the targettype of a property is a [ISet](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iset-1?view=netcore-3.1)?
- Is the Equals-operation implemented correctly in the property-target-type?
- Will cyclic object-structures (2 objects references each other by a property) cause an endlees-loop while comparing them?
- ...

So, there are some issues just for the Equals-operation.

And there are some related frequently occurring topics:

- Overwrite ToString()
- Serialization

Especially the cyclic object-structures are always a problem. We simply want to xml-serialize any object without having to worry about cyclic structures so that we can spend more time for the actual algorithms we want to implement.

Admitted:

You simply can not exactly map an object into xml if the object contains cyclic object-structures. But in many cases we do not want to execute XPath-expressions on it, transform it with XSLT or query data with XQuery. And for this many cases we want a xml which does not have to look like the originally object, but we want to be able to edit it with a text-editor, even it is not as easy as it would be in "simply" xml-serialized objects.

Is is also conceivable that we can store the xml-serialized configuration-object of our application in a versioned repository (e. g. a git-repository) so that we can see any change in our configuration. With xml, this is possible as long as there a no cyclic object-structures.

This is the reason we do not use a [BinaryFormatter](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.formatters.binary.binaryformatter?view=netcore-3.1) often.

AdvancedObjectAnalysis tries to solve this problems. AdvancedObjectAnalysis looks into the objects and can serialize or compare the objects, but AdvancedObjectAnalysis it memorizes the already compared objects to avoid endless-loops. When serializing an object, every object (referenced by the object we want to serialize) will be serialized as simple object without the objects of the properties but the links are stored. You can compare it with a reference-graph. And when you serialize it then you simply serialize every vertex with its outgoing edges. When deserializing the property-values will be "composed" according to the edges in the graph. That's all. Maybe it sounds easy. In theory: It is easy. In Practise: It is s little bit tricky in detail and that is the reason why the types which are doing this are "Advanced".

# Usage

Calling the functions of AdvancedObjectAnalysis is pretty using the `Generic`-type. Example:

```
using GRYLibrary.Core.AdvancedObjectAnalysis;
using System.Xml.Serialization;

public class MyType : IXMLSerializable
    {

        // Your code

        #region Overhead
        public override bool Equals(object @object)
        {
            return Generic.GenericEquals(this, @object);
        }

        public override int GetHashCode()
        {
            return Generic.GenericGetHashCode(this);
        }

        public override string ToString()
        {
            return Generic.GenericToString(this);
        }

        public XmlSchema GetSchema()
        {
            return Generic.GenericGetSchema(this);
        }

        public void ReadXml(XmlReader reader)
        {
            Generic.GenericReadXml(this, reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            Generic.GenericWriteXml(this, writer);
        }
        #endregion
    }
```
