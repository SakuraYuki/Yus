﻿using Swifter.Readers;
using Swifter.Tools;
using System.Collections.Generic;

namespace Swifter.Writers
{
    sealed class AsReadAllWriter<TIn, TOut> : IDataWriter<TIn>
    {
        public readonly IDataWriter<TOut> dataWriter;

        public AsReadAllWriter(IDataWriter<TOut> dataWriter)
        {
            this.dataWriter = dataWriter;
        }

        public IValueWriter this[TIn key] => dataWriter[XConvert<TIn, TOut>.Convert(key)];

        public IEnumerable<TIn> Keys => ArrayHelper.CreateAsIterator<TOut, TIn>(dataWriter.Keys);

        public int Count => dataWriter.Count;

        public void Initialize()=> dataWriter.Initialize();

        public void Initialize(int capacity) => dataWriter.Initialize(capacity);

        public void OnWriteAll(IDataReader<TIn> dataReader) => dataWriter.OnWriteAll(new AsWriteAllReader<TOut, TIn>(dataReader));

        public void OnWriteValue(TIn key, IValueReader valueReader)=> dataWriter.OnWriteValue(XConvert<TIn, TOut>.Convert(key), valueReader);
    }
}