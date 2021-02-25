
namespace GRYLibrary.Core.CryptoSystems.EncryptionAlgorithms.ConcreteCommonAlgorithms.ConcreteAsymmetricAlgorithms.Helper
{
    public struct Curve
    {
        public static Curve Curve25519 { get; } = new Curve();
        public static Curve Secp256k1 { get; } = new Curve();
        
        //TODO add properties
    }

}
