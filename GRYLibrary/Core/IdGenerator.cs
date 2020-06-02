using System;
using System.Collections.Generic;
using System.Numerics;

namespace GRYLibrary.Core
{
    public class IdGenerator<T>
    {
        private readonly ISet<T> _GeneratedIds = new HashSet<T>();
        private readonly Func<T, T> _GenerateNewId;
        T LastId = default;
        public IdGenerator(Func<T, T> generateNewId)
        {
            this._GenerateNewId = generateNewId;
        }
        public T GenerateNewId()
        {
            this.LastId = this._GenerateNewId(this.LastId);
            return this.LastId;
        }
        public ISet<T> GeneratedIds()
        {
            return new HashSet<T>(this._GeneratedIds);
        }
    }
    public class IdGenerator
    {
        public static IdGenerator<int> GetDefaultIntIdGenerator()
        {
            return new IdGenerator<int>((int lastGeneratedId) => lastGeneratedId + 1);
        }
        public static IdGenerator<long> GetDefaultLongIdGenerator()
        {
            return new IdGenerator<long>((long lastGeneratedId) => lastGeneratedId + 1);
        }
        public static IdGenerator<BigInteger> GetDefaultBigIntegerIdGenerator()
        {
            return new IdGenerator<BigInteger>((BigInteger lastGeneratedId) => lastGeneratedId + 1);
        }
        public static IdGenerator<Guid> GetDefaultGuidIdGenerator()
        {
            return new IdGenerator<Guid>((Guid lastGeneratedId) => Guid.NewGuid());
        }
    }
}

