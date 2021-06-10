using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary.Tests.TestData.TestTypes.ComplexDataStructure2
{
    public class OtherType : ModelType
    {
        #region Overhead
        public override bool Equals(object @object)
        {
            return base.Equals(@object);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return base.ToString();
        }
        #endregion
    }
}
