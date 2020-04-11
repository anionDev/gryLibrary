using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.AdvancedXMLSerialysis
{
    public class GenericXMLSerializer<T>
    {
        public Dictionary<Func<Type,bool>/*IsApplicable-function*/,Func<object, object>/*Convert-function*/> Mapping { get; set; } = new Dictionary<Func<Type, bool>, Func<object, object>>();

        public string Serialize(T @object)
        {
            throw new NotImplementedException();
        }
        public T Deserialize(string serializedObject)
        {
            throw new NotImplementedException();
        }
        public void AddDefaultTypeMapping()
        {
            //this.Mapping.Add(()=>Utilities.ObjectIsList/*TODO*/, null/*todo*/);
            //todo do this for dictionary, array, set, enumerable as well
        }
    }
}
