using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.AdvancedXMLSerialysis
{
    public class GenericXMLSerializer<T>
    {
        public Dictionary<Type/*interface-type or abstract-type*/, Type/*type which should be used*/> Mapping { get; set; } = new Dictionary<Type, Type>();

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
            this.Mapping.Add(typeof(IList<>), typeof(List<>));
            this.Mapping.Add(typeof(ISet<>), typeof(HashSet<>));
            this.Mapping.Add(typeof(IDictionary<,>), typeof(Dictionary<,>));
        }
    }
}
