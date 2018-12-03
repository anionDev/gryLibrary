using System.Collections.Generic;
using System.IO;

namespace GRYLibrary.Miscellaneous
{
    public class StreamMixer
    {
        private IDictionary<string, Stream> Outputs { get; set; } = new Dictionary<string, Stream>();
        private IDictionary<string, Stream> Inputs { get; set; } = new Dictionary<string, Stream>();
        private IList<TupleWithValueComparisonEquals<string, string>> Connections { get; set; } = new List<TupleWithValueComparisonEquals<string, string>>();
        public void AddInputStream(string name, Stream stream)
        {
            throw new System.NotImplementedException();
        }
        public void RemoveInputStream(string name)
        {
            throw new System.NotImplementedException();
        }
        public void AddOutputStream(string name, Stream stream)
        {
            throw new System.NotImplementedException();
        }
        public void RemoveOutputStream(string name)
        {
            throw new System.NotImplementedException();
        }
        public void AddConnection(string inputName, string outputName)
        {
            throw new System.NotImplementedException();
        }
        public void RemoveConnection(string inputName, string outputName)
        {
            throw new System.NotImplementedException();
        }
    }
}

