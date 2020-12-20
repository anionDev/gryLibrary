# GenericXMLSerializer

GenericXMLSerializer is a simple way to serialize objects in an XML-document.
`GenericXMLSerializer` has some special properties/features. `GenericXMLSerializer`
- can handle cyclic attribute-references
- can obviously be edited in a plain texteditor (because it is XML)

When you want so XML-serialize an object you have a problem when your data have cyclic attribute-references.
Since XML is a hierarchy you can not use XML (or JSON or something similar) so serialize these objects.
This is bad because XML has many positive properties and it would be nice to be able to XML-serialize cyclic objects.
`GenericXMLSerializer` allows exactly this by serialize your object in another way than a "normal" XMLSerializer would do.

Usually when you use an XMLSerializer and you have have duplicated objects this makes the result bigger unnecessarily.
Example:
You create an order-object like this:
```
object order=CreateNewOrder()
order.BuyerName=buyer.Name
order.Product=...
order.Price=...
```
And if the billing-address and the delivery-address of the order is the same you may also write something like
```
order.BillingAddress=order.Buyer.Adress
order.DeliveryAddress=order.Buyer.Adress
```
Now using a normal XML-Serializer the resulting XML-document would look like
```
<Order>
  <BuyerName>...<BuyerName/>
  <Product>...<Product/>
  <Price>...<Price/>
  <BillingAddress>
     some address
  <BillingAddress/>
  <DeliveryAddress>
     exactly the same address again
  <DeliveryAddress/>
</Order>
```

It is really unnecessary that the address is stored in the XML-document twice. `PropertyEqualsCalculator` solves this. `PropertyEqualsCalculator` may have some overhead but for big objects which occur several times in the object-tree this can reduce the size of the object because `PropertyEqualsCalculator` will store all complex objects only once in the resulting XML-document.

## Usage

It is pretty simple and generic:

```
using GRYLibrary.Core.AdvancedObjectAnalysis;
using System.Xml;
using System.Xml.Schema;

namespace MyNamespace{

  class MyClass : IXmlSerializable
  {

    //...

    #region Overhead

    public XmlSchema GetSchema()
    {
      return Generic.GenericGetSchema(this.GetType());
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

}
```

`GenericXMLSerializer` does the rest for you.

## Requirements

TODO

## Hints

`Generic.GenericGetSchema()` will return null like it is [recommended by Microsoft](https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.ixmlserializable.getschema?#remarks). But since this recommendation may change in future you should also use `Generic.GenericGetSchema()` since the implementation of `GenericGetSchema()`will be adjusted when Microsoft recommend that `GetSchema()` should return something else.
