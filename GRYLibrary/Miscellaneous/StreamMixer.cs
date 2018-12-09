using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GRYLibrary.Miscellaneous
{
    public class StreamMixer
    {
        private IDictionary<string, Stream> Outputs { get; set; } = new Dictionary<string, Stream>();
        private IDictionary<string, Stream> Inputs { get; set; } = new Dictionary<string, Stream>();
        private ISet<TupleWithValueComparisonEquals<string, string>> Connections { get; set; } = new HashSet<TupleWithValueComparisonEquals<string, string>>();
        public void AddInputStream(string name, Stream stream)
        {
            this.Inputs.Add(name, stream);
        }
        public void RemoveInputStream(string name)
        {
            this.Inputs.Remove(name);
        }
        public void AddOutputStream(string name, Stream stream)
        {
            this.Outputs.Add(name, stream);
        }
        public void RemoveOutputStream(string name)
        {
            this.Outputs.Remove(name);
        }
        public void AddConnection(string inputName, string outputName)
        {
            Connections.Add(new TupleWithValueComparisonEquals<string, string>(inputName, outputName));
            throw new System.NotImplementedException();
        }
        public void RemoveConnection(string inputName, string outputName)
        {
            Connections.Remove(new TupleWithValueComparisonEquals<string, string>(inputName, outputName));
            throw new System.NotImplementedException();
        }
        public IDictionary<string, Stream> GetallInputs()
        {
            return new Dictionary<string, Stream>(this.Inputs);
        }
        public IDictionary<string, Stream> GetallOutputs()
        {
            return new Dictionary<string, Stream>(this.Outputs);
        }
    }
}
