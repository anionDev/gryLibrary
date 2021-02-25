using GRYLibrary.Core.Miscellaneous;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GRYLibrary.Core.CryptoSystems.ConcreteHashAlgorithms
{
    public class SHA256PureCSharp : HashAlgorithm
    {
        public override byte[] GetIdentifier()
        {
            return Utilities.PadLeft(Encoding.ASCII.GetBytes("SHA256PC#"), 10);
        }
        public uint[] H_InitialValue = new uint[8] {
            0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a, 0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19,
        };
        public uint[] K_InitialValue = new uint[64]{
            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
            0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
            0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
            0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
            0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
            0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
            0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
            0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2,
        };

        public override byte[] Hash(byte[] data)
        {
            uint[] H = this.H_InitialValue.ToArray();
            uint[] K = this.K_InitialValue.ToArray();

            byte[] message = data;
            int L_messageLengthInBits = data.Length * 8;

            message = message.Concat(new byte[] { 128 }).ToArray();

            int K_amountOfBitsToAppend = 512 - ((L_messageLengthInBits + 8 + 64) % 512);
            Debug.Assert(K_amountOfBitsToAppend % 8 == 0);
            int K_amountOfBytesToAppend = K_amountOfBitsToAppend / 8;
            message = message.Concat(new byte[K_amountOfBytesToAppend]).ToArray();
            Debug.Assert((L_messageLengthInBits + 8 + K_amountOfBitsToAppend + 64) % 512 == 0);

            message = message.Concat(Utilities.UnsignedInteger64BitToByteArray((ulong)L_messageLengthInBits)).ToArray();
            Debug.Assert(message.Length % 64 == 0);

            int chunkSizeInBits = 512;
            int chunkSizeInBytes = chunkSizeInBits / 8;
            int amountOfChunks = message.Length / chunkSizeInBytes;
            for (int chunkIndex = 0; chunkIndex < amountOfChunks; chunkIndex++)
            {
                byte[] currentChunk = message.Skip(chunkIndex * chunkSizeInBytes).Take(chunkSizeInBytes).ToArray();
                Debug.Assert(currentChunk.Length == chunkSizeInBytes);
                uint[] W = new uint[64];
                uint[] currentChunkAsUnsignedIntegerArray = Utilities.ByteArrayToUnsignedInteger32BitArray(currentChunk);
                Debug.Assert(currentChunkAsUnsignedIntegerArray.Length == chunkSizeInBytes / 4);
                Array.Copy(currentChunkAsUnsignedIntegerArray, W, currentChunkAsUnsignedIntegerArray.Length);
                for (int i = 16; i < 64; i++)
                {
                    uint s0 = XOr(XOr(RightRotate(W[i - 15], 7), RightRotate(W[i - 15], 18)), RightShift(W[i - 15], 3));
                    uint s1 = XOr(XOr(RightRotate(W[i - 2], 17), RightRotate(W[i - 2], 19)), RightShift(W[i - 2], 10));
                    W[i] = Add(W[i - 16], s0, W[i - 7], s1);
                }

                uint a = H[0];
                uint b = H[1];
                uint c = H[2];
                uint d = H[3];
                uint e = H[4];
                uint f = H[5];
                uint g = H[6];
                uint h = H[7];
                for (int i = 0; i < 64; i++)
                {
                    uint S1 = XOr(XOr(RightRotate(e, 6), RightRotate(e, 11)), RightRotate(e, 25));
                    uint ch = XOr(And(e, f), And(Not(e), g));
                    uint temp1 = Add(h, S1, ch, K[i], W[i]);
                    uint S0 = XOr(XOr(RightRotate(a, 2), RightRotate(a, 13)), RightRotate(a, 22));
                    uint maj = XOr(XOr(And(a, b), And(a, c)), And(b, c));
                    uint temp2 = Add(S0, maj);
                    h = g;
                    g = f;
                    f = e;
                    e = Add(d, temp1);
                    d = c;
                    c = b;
                    b = a;
                    a = Add(temp1, temp2);
                }
                H[0] = Add(a, H[0]);
                H[1] = Add(b, H[1]);
                H[2] = Add(c, H[2]);
                H[3] = Add(d, H[3]);
                H[4] = Add(e, H[4]);
                H[5] = Add(f, H[5]);
                H[6] = Add(g, H[6]);
                H[7] = Add(h, H[7]);

            }
            return Utilities.UnsignedInteger32BitToByteArray(H[0])
           .Concat(Utilities.UnsignedInteger32BitToByteArray(H[1]))
           .Concat(Utilities.UnsignedInteger32BitToByteArray(H[2]))
           .Concat(Utilities.UnsignedInteger32BitToByteArray(H[3]))
           .Concat(Utilities.UnsignedInteger32BitToByteArray(H[4]))
           .Concat(Utilities.UnsignedInteger32BitToByteArray(H[5]))
           .Concat(Utilities.UnsignedInteger32BitToByteArray(H[6]))
           .Concat(Utilities.UnsignedInteger32BitToByteArray(H[7]))
           .ToArray();
        }

        private uint Add(params uint[] summands)
        {
            uint result = 0;
            for (int i = 0; i < summands.Length; i++)
            {
                result += summands[i];
            }
            return result;
        }
        private uint XOr(uint left, uint right)
        {
            return left ^ right;
        }
        private uint RightShift(uint value, int amountOfDigits)
        {
            Debug.Assert(0 < amountOfDigits);
            return value >> amountOfDigits;
        }
        private uint RightRotate(uint value, int amountOfDigits)
        {
            Debug.Assert(0 < amountOfDigits && amountOfDigits < 32);
            return (value >> amountOfDigits) | (value << (32 - amountOfDigits));
        }
        private uint And(uint left, uint right)
        {
            return left & right;
        }
        private uint Not(uint value)
        {
            return ~value;
        }
    }
}
