﻿using Swifter.RW;
using Swifter.Tools;
using Swifter.Writers;
using System;
using System.Collections.Generic;

namespace Swifter.Readers
{
    /// <summary>
    /// 数据读取器键类型转换的接口。
    /// </summary>
    internal interface IAsDataReader
    {
        IDataReader Content { get; }
    }

    /// <summary>
    /// 数据读取器键类型转换的类型。
    /// </summary>
    /// <typeparam name="TIn">输入类型</typeparam>
    /// <typeparam name="TOut">输出类型</typeparam>
    internal sealed class AsDataReader<TIn, TOut> : IDataReader<TOut>, IAsDataReader, IDirectContent
    {
        /// <summary>
        /// 原始数据读取器。
        /// </summary>
        public readonly IDataReader<TIn> dataReader;

        /// <summary>
        /// 创建数据读取器键类型转换类的实例。
        /// </summary>
        /// <param name="dataReader">原始数据读取器</param>
        public AsDataReader(IDataReader<TIn> dataReader)
        {
            this.dataReader = dataReader;
        }

        /// <summary>
        /// 转换键，并返回该键对应的值读取器。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回值读取器</returns>
        public IValueReader this[TOut key] => dataReader[XConvert<TOut, TIn>.Convert(key)];

        /// <summary>
        /// 获取转换后的键集合。
        /// </summary>
        public IEnumerable<TOut> Keys => ArrayHelper.CreateAsIterator<TIn, TOut>(dataReader.Keys);

        /// <summary>
        /// 获取数据源键的数量。
        /// </summary>
        public int Count => dataReader.Count;

        IDataReader IAsDataReader.Content => dataReader;

        /// <summary>
        /// 获取原始数据读取器的数据源 Id。
        /// </summary>
        public object ReferenceToken => dataReader.ReferenceToken;

        object IDirectContent.DirectContent
        {
            get
            {
                if (dataReader is IDirectContent directContent)
                {
                    return directContent.DirectContent;
                }

                throw new NotSupportedException(StringHelper.Format("This data {0} does not support direct {1} content.", "reader", "get"));
            }
            set
            {
                if (dataReader is IDirectContent directContent)
                {
                    directContent.DirectContent = value;

                    return;
                }

                throw new NotSupportedException(StringHelper.Format("This data {0} does not support direct {1} content.", "reader", "set"));
            }
        }

        /// <summary>
        /// 将数据中的所有转换后的键与值写入到数据写入器中。
        /// </summary>
        /// <param name="dataWriter">数据写入器</param>
        public void OnReadAll(IDataWriter<TOut> dataWriter) =>
            dataReader.OnReadAll(new AsReadAllWriter<TIn, TOut>(dataWriter));

        /// <summary>
        /// 转换键，并将该键对应的值写入到值写入器中。
        /// </summary>
        /// <param name="key">指定键</param>
        /// <param name="valueWriter">值写入器</param>
        public void OnReadValue(TOut key, IValueWriter valueWriter) =>
            dataReader.OnReadValue(XConvert<TOut, TIn>.Convert(key), valueWriter);

        /// <summary>
        /// 将数据中的所有转换后的键与值进行筛选，并将满足筛选的键与值写入到数据写入器中。
        /// </summary>
        /// <param name="dataWriter">数据写入器</param>
        /// <param name="valueFilter">键值筛选器</param>
        public void OnReadAll(IDataWriter<TOut> dataWriter, IValueFilter<TOut> valueFilter) =>
            dataReader.OnReadAll(new AsReadAllWriter<TIn, TOut>(dataWriter), new AsReadAllFilter<TIn, TOut>(valueFilter));
    }
}