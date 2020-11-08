﻿using Swifter.Readers;
using Swifter.Tools;
using Swifter.Writers;
using System;
using System.Collections.Generic;

namespace Swifter.RW
{
    /// <summary>
    /// 数据读写器键类型转换的接口。
    /// </summary>
    internal interface IAsDataRW
    {
        IDataRW Content { get; }
    }

    /// <summary>
    /// 数据读写器键类型转换的类型。
    /// </summary>
    /// <typeparam name="TIn">输入类型</typeparam>
    /// <typeparam name="TOut">输出类型</typeparam>
    internal sealed class AsDataRW<TIn, TOut> : IDataRW<TOut>, IAsDataRW, IAsDataReader, IAsDataWriter, IDirectContent
    {
        /// <summary>
        /// 原始数据读写器。
        /// </summary>
        public readonly IDataRW<TIn> dataRW;

        /// <summary>
        /// 创建数据读写器键类型转换类的实例。
        /// </summary>
        /// <param name="dataRW">原始数据读写器</param>
        public AsDataRW(IDataRW<TIn> dataRW)
        {
            this.dataRW = dataRW;
        }

        /// <summary>
        /// 转换键，并返回该键对应的值读写器。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回值读写器</returns>
        public IValueRW this[TOut key] => dataRW[XConvert<TOut, TIn>.Convert(key)];

        IValueReader IDataReader<TOut>.this[TOut key] => this[key];

        IValueWriter IDataWriter<TOut>.this[TOut key] => this[key];

        /// <summary>
        /// 获取转换后的键集合。
        /// </summary>
        public IEnumerable<TOut> Keys => ArrayHelper.CreateAsIterator<TIn, TOut>(dataRW.Keys);

        /// <summary>
        /// 获取数据源键的数量。
        /// </summary>
        public int Count => dataRW.Count;

        /// <summary>
        /// 获取原始数据读取器的数据源 Id。
        /// </summary>
        public object ReferenceToken => dataRW.ReferenceToken;

        IDataReader IAsDataReader.Content => dataRW;

        IDataWriter IAsDataWriter.Content => dataRW;

        IDataRW IAsDataRW.Content => dataRW;

        object IDirectContent.DirectContent
        {
            get
            {
                if (dataRW is IDirectContent directContent)
                {
                    return directContent.DirectContent;
                }

                throw new NotSupportedException(StringHelper.Format("This data {0} does not support direct {1} content.", "(read)writer", "get"));
            }
            set
            {
                if (dataRW is IDirectContent directContent)
                {
                    directContent.DirectContent = value;

                    return;
                }

                throw new NotSupportedException(StringHelper.Format("This data {0} does not support direct {1} content.", "(read)writer", "set"));
            }
        }

        /// <summary>
        /// 初始化数据源。
        /// </summary>
        public void Initialize()
        {
            dataRW.Initialize();
        }

        /// <summary>
        /// 初始化具有指定容量的数据源。
        /// </summary>
        /// <param name="capacity">指定容量</param>
        public void Initialize(int capacity)
        {
            dataRW.Initialize(capacity);
        }

        /// <summary>
        /// 将数据中的所有转换后的键与值写入到数据写入器中。
        /// </summary>
        /// <param name="dataWriter">数据写入器</param>
        public void OnReadAll(IDataWriter<TOut> dataWriter)
        {
            dataRW.OnReadAll(new AsReadAllWriter<TIn, TOut>(dataWriter));
        }

        /// <summary>
        /// 将数据中的所有转换后的键与值进行筛选，并将满足筛选的键与值写入到数据写入器中。
        /// </summary>
        /// <param name="dataWriter">数据写入器</param>
        /// <param name="valueFilter">键值筛选器</param>
        public void OnReadAll(IDataWriter<TOut> dataWriter, IValueFilter<TOut> valueFilter)
        {
            dataRW.OnReadAll(new AsReadAllWriter<TIn, TOut>(dataWriter), new AsReadAllFilter<TIn, TOut>(valueFilter));
        }

        /// <summary>
        /// 转换键，并将该键对应的值写入到值写入器中。
        /// </summary>
        /// <param name="key">指定键</param>
        /// <param name="valueWriter">值写入器</param>
        public void OnReadValue(TOut key, IValueWriter valueWriter)
        {
            dataRW.OnReadValue(XConvert<TOut, TIn>.Convert(key), valueWriter);
        }

        /// <summary>
        /// 将数据中的所有转换后的键从读取器中读取所有值写入到数据源中。
        /// </summary>
        /// <param name="dataReader"></param>
        public void OnWriteAll(IDataReader<TOut> dataReader)
        {
            dataRW.OnWriteAll(new AsWriteAllReader<TIn, TOut>(dataReader));
        }

        /// <summary>
        /// 从值读取器中读取一个值设置到指定键的值中。
        /// </summary>
        /// <param name="key">指定键</param>
        /// <param name="valueReader">值读取器</param>
        public void OnWriteValue(TOut key, IValueReader valueReader)
        {
            dataRW.OnWriteValue(XConvert<TOut, TIn>.Convert(key), valueReader);
        }
    }
}