using System.Text;

namespace GRYLibrary.Core.CryptoSystems
{
    public abstract class CryptographyAlgorithm
    {
        /// <summary>
        /// Represents an identifier for this algorithm.
        /// </summary>
        /// <returns>
        /// Returns an identifier with en exact length of 10.
        /// </returns>
        public abstract byte[] GetIdentifier();
        public Encoding Encoding { get; set; } = new UTF8Encoding(false);
    }
}
