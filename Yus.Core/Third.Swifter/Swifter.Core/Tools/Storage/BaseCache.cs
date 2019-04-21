﻿using Swifter.Tools;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Swifter.Tools
{
    /// <summary>
    /// 提供高效读取的缓存字典。
    /// </summary>
    /// <typeparam name="TKey">缓存键类型</typeparam>
    /// <typeparam name="TValue">缓存值类型</typeparam>
    [Serializable]
    public abstract class BaseCache<TKey, TValue>
    {
        /// <summary>
        /// 表示可以扩容的最大缓存容量。
        /// </summary>
        public const int MaxCapacity = 0x40000000;

        internal Entity[] entities;
        internal int count;

        [Serializable]
        internal sealed class Entity
        {
            public Entity(
                TKey key, 
                int hashCode, 
                TValue value,
                Entity next
                )
            {
                this.key = key;
                this.hashCode = hashCode;
                this.value = value;
                this.next = next;
            }

            public readonly int hashCode;
            public readonly TKey key;
            public TValue value;
            public Entity next;
        }

        /// <summary>
        /// 初始化高效读取的缓存字典。
        /// </summary>
        /// <param name="capacity">初始容量</param>
        public BaseCache(int capacity)
        {
            Initialize(capacity);
        }

        void Initialize(int capacity)
        {
            if (capacity > MaxCapacity)
            {
                capacity = MaxCapacity;
            }

            var i = 2;

            while (i < capacity)
            {
                i <<= 1;
            }

            Resize(i);
        }

        /// <summary>
        /// 读取或设置缓存的值。
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>返回缓存的值</returns>
        /// <exception cref="KeyNotFoundException">读取时，当 Key 不存在是发生此异常。</exception>
        public TValue this[TKey key]
        {
            [MethodImpl(VersionDifferences.AggressiveInlining)]
            get
            {
                return (FindEntity(key) ?? throw new KeyNotFoundException()).value;
            }
            [MethodImpl(VersionDifferences.AggressiveInlining)]
            set
            {
                Set(key, value, false);
            }
        }

        /// <summary>
        /// 获取缓存数量。
        /// </summary>
        public int Count => count;

        /// <summary>
        /// 计算缓存键的 HashCode。
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>返回 HashCode</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        protected abstract int ComputeHashCode(TKey key);

        /// <summary>
        /// 比较两个缓存键是否相等。
        /// </summary>
        /// <param name="key1">缓存键 1</param>
        /// <param name="key2">缓存键 2</param>
        /// <returns>返回是否相等</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        protected abstract bool Equals(TKey key1, TKey key2);

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        int ComputeIndex(int hashCode)
        {
            return ComputeIndex(entities, hashCode);
        }

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        static int ComputeIndex(Entity[] entities, int hashCode)
        {
            return hashCode & (entities.Length - 1);
        }
        
        void Resize(int size)
        {
            if (size <= 0 || size > MaxCapacity)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(size),
                    size,
                    "Capacity size exceeded range."
                    );
            }

            var entities = new Entity[size];

            if (this.entities != null)
            {
                foreach (var item in this.entities)
                {
                    for (var current = item; current != null; current = current.next)
                    {
                        DirectAdd(
                            entities,
                            current.key,
                            current.hashCode,
                            ComputeIndex(entities, current.hashCode),
                            current.value
                            );
                    }
                }
            }

            this.entities = entities;
        }
        
        void Expand()
        {
            Resize(entities.Length << 1);
        }

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        void DirectAdd(TKey key, int hashCode, TValue value)
        {
            var index = ComputeIndex(hashCode);

            DirectAdd(key, hashCode, index, value);
        }

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        void DirectAdd(TKey key, int hashCode, int index, TValue value)
        {
            DirectAdd(entities, key, hashCode, index, value);

            ++count;

            if (count == entities.Length)
            {
                Expand();
            }
        }

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        static void DirectAdd(Entity[] entities, TKey key, int hashCode, int index, TValue value)
        {
            entities[index] = new Entity(key, hashCode, value, entities[index]);
        }

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        Entity FindEntity(TKey key, int hashCode, int index)
        {
            for (var current = entities[index]; current != null; current = current.next)
            {
                if (current.hashCode == hashCode && Equals(current.key, key))
                {
                    return current;
                }
            }

            return null;
        }

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        Entity FindEntity(TKey key, int hashCode)
        {
            return FindEntity(key, hashCode, ComputeIndex(hashCode));
        }

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        Entity FindEntity(TKey key)
        {
            return FindEntity(key, ComputeHashCode(key));
        }

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        void Set(TKey key, TValue value, bool isAdd)
        {
            var entity = GetOrAdd(key, value);

            if (entity != null)
            {
                if (isAdd)
                {
                    throw new ArgumentException("Cache can't add duplicate keys.");
                }

                entity.value = value;
            }
        }

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        Entity GetOrAdd(TKey key, TValue value)
        {
            var hashCode = ComputeHashCode(key);

            var index = ComputeIndex(hashCode);

            var entity = FindEntity(key, hashCode, index);

            if (entity != null)
            {
                return entity;
            }

            DirectAdd(key, hashCode, index, value);

            return null;
        }

        /// <summary>
        /// 获取指定键的值集合。
        /// </summary>
        /// <param name="key"></param>
        /// <returns>返回一个值的迭代器。</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public IEnumerable<TValue> GetValues(TKey key)
        {
            var hashCode = ComputeHashCode(key);

            var entity = FindEntity(key, hashCode);

            while (entity != null)
            {
                if (entity.hashCode == hashCode && Equals(entity.key, key))
                {
                    yield return entity.value;
                }

                entity = entity.next;
            }
        }

        /// <summary>
        /// 获取或新增一个缓存。
        /// 该方法是线程同步的，它会将当前对象作为锁。
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="func">获取缓存值</param>
        /// <returns>返回缓存值</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public TValue LockGetOrAdd(TKey key, Func<TKey, TValue> func)
        {
            var hashCode = ComputeHashCode(key);

            var entity = FindEntity(key, hashCode);

            if (entity != null)
            {
                return entity.value;
            }

            lock (this)
            {
                var index = ComputeIndex(hashCode);

                entity = FindEntity(key, hashCode, index);

                if (entity != null)
                {
                    return entity.value;
                }

                var value = func(key);

                DirectAdd(key, hashCode, index, value);

                return value;
            }
        }

        /// <summary>
        /// 设置或新增一个缓存。
        /// 该方法是线程同步的，它会将当前对象作为锁。
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        public void LockSetOrAdd(TKey key, TValue value)
        {
            lock (this)
            {
                Set(key, value, false);
            }
        }

        /// <summary>
        /// 清空所有键值。
        /// </summary>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public void Clear()
        {
            entities = null;

            count = 0;

            Initialize(0);
        }

        /// <summary>
        /// 获取或新增一个缓存，该方法不是线程同步的。
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="func">获取缓存值</param>
        /// <returns>返回缓存值</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> func)
        {
            var hashCode = ComputeHashCode(key);
            
            var index = ComputeIndex(hashCode);

            var entity = FindEntity(key, hashCode, index);

            if (entity != null)
            {
                return entity.value;
            }

            var value = func(key);

            DirectAdd(key, hashCode, index, value);

            return value;
        }

        /// <summary>
        /// 获取或新增一个缓存。
        /// 该方法是线程同步的，它会将第一个参数作为锁。
        /// </summary>
        /// <typeparam name="T">当前实例的类型，它必须实现 IGetOrCreate&lt;TToken&gt; 接口。</typeparam>
        /// <typeparam name="TToken">Token 类型</typeparam>
        /// <param name="this">当前实例</param>
        /// <param name="token">token 值</param>
        /// <returns>返回缓存值</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        protected static TValue GetOrCreate<T, TToken>(T @this, TToken token) where T : BaseCache<TKey, TValue>, IGetOrCreate<TToken>
        {
            var key = @this.AsKey(token);

            var hashCode = @this.ComputeHashCode(key);

            var entity = @this.FindEntity(key, hashCode);

            if (entity != null)
            {
                return entity.value;
            }

            lock (@this)
            {
                var index = @this.ComputeIndex(hashCode);

                entity = @this.FindEntity(key, hashCode, index);

                if (entity != null)
                {
                    return entity.value;
                }

                var value = @this.AsValue(token);

                @this.DirectAdd(key, hashCode, index, value);

                return value;
            }
        }

        /// <summary>
        /// 获取或新增一个缓存。
        /// 该方法是线程同步的，它会将第一个参数作为锁。
        /// </summary>
        /// <typeparam name="T">当前实例的类型，它必须实现 IGetOrCreate&lt;TToken&gt; 接口。</typeparam>
        /// <param name="this">当前实例</param>
        /// <param name="key">token 值</param>
        /// <returns>返回缓存值</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        protected static TValue GetOrCreate<T>(T @this, TKey key) where T : BaseCache<TKey, TValue>, IGetOrCreate<TKey>
        {
            var hashCode = @this.ComputeHashCode(key);

            var entity = @this.FindEntity(key, hashCode);

            if (entity != null)
            {
                return entity.value;
            }

            lock (@this)
            {
                var index = @this.ComputeIndex(hashCode);

                entity = @this.FindEntity(key, hashCode, index);

                if (entity != null)
                {
                    return entity.value;
                }

                var value = @this.AsValue(key);

                @this.DirectAdd(key, hashCode, index, value);

                return value;
            }
        }

        /// <summary>
        /// 尝试添加一个缓存。
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <returns></returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public bool TryAdd(TKey key, TValue value)
        {
            return GetOrAdd(key, value) == null;
        }

        /// <summary>
        /// 直接添加一个缓存，不管是否存在。
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public void DirectAdd(TKey key, TValue value)
        {
            DirectAdd(key, ComputeHashCode(key), value);
        }

        /// <summary>
        /// 新增缓存。
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <exception cref="ArgumentException">当 Key 存在时发生此异常</exception>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public void Add(TKey key, TValue value)
        {
            Set(key, value, true);
        }

        /// <summary>
        /// 新增缓存。
        /// 该方法是线程同步的，它会将当前对象作为锁。
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <exception cref="ArgumentException">当 Key 存在时发生此异常</exception>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public void LockAdd(TKey key, TValue value)
        {
            lock (this)
            {
                Set(key, value, true);
            }
        }

        /// <summary>
        /// 直接添加一个缓存，不管是否存在。
        /// 该方法是线程同步的，它会将当前对象作为锁。
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <exception cref="ArgumentException">当 Key 存在时发生此异常</exception>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public void LockDirectAdd(TKey key, TValue value)
        {
            lock (this)
            {
                DirectAdd(key, ComputeHashCode(key), value);
            }
        }

        /// <summary>
        /// 尝试获取缓存的值。
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">返回缓存的值</param>
        /// <returns>返回 Key 是否存在</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public bool TryGetValue(TKey key, out TValue value)
        {
            var entity = FindEntity(key);

            if (entity == null)
            {
                value = default(TValue);

                return false;
            }

            value = entity.value;

            return true;
        }

        /// <summary>
        /// 移除指定缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>返回是否有移除缓存</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public bool Remove(TKey key)
        {
            var hashCode = ComputeHashCode(key);

            var index = ComputeIndex(hashCode);

            for (Entity current = entities[index], last = null; current != null; last = current, current = current.next)
            {
                if (current.hashCode == hashCode && Equals(current.key, key))
                {
                    if (last == null)
                    {
                        entities[index] = current.next;
                    }
                    else
                    {
                        last.next = current.next;
                    }

                    --count;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除指定键的所有缓存。
        /// </summary>
        /// <param name="key">缓存键</param>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public void RemoveAll(TKey key)
        {
            var hashCode = ComputeHashCode(key);

            var index = ComputeIndex(hashCode);

            for (Entity current = entities[index], last = null; current != null; last = current, current = current.next)
            {
                while (current.hashCode == hashCode && Equals(current.key, key))
                {
                    if (last == null)
                    {
                        entities[index] = current.next;
                    }
                    else
                    {
                        last.next = current.next;
                    }

                    --count;

                    current = current.next;

                    if (current == null)
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 允许高效缓存获取或创建的接口。
        /// </summary>
        /// <typeparam name="TToken">Token 类型</typeparam>
        public interface IGetOrCreate<TToken>
        {
            /// <summary>
            /// Token 转键。
            /// </summary>
            /// <param name="token">token</param>
            /// <returns>返回键</returns>
            TKey AsKey(TToken token);

            /// <summary>
            /// Token 转值。
            /// </summary>
            /// <param name="token">token</param>
            /// <returns>返回值</returns>
            TValue AsValue(TToken token);

            /// <summary>
            /// 获取或新增一个缓存。
            /// 该方法是线程同步的，它会将第一个参数作为锁。
            /// </summary>
            /// <param name="token">token 值</param>
            /// <returns>返回缓存值</returns>
            [MethodImpl(VersionDifferences.AggressiveInlining)]
            TValue GetOrCreate(TToken token);
        }
    }
}