﻿using Swifter.Readers;
using Swifter.RW;
using Swifter.Tools;
using System;
using System.Collections.Generic;

namespace Swifter.Writers
{
    internal sealed class TableToArrayWriter : IDataWriter<int> , IDirectContent
    {
        public readonly ITableWriter tableWriter;

        public TableToArrayWriter(ITableWriter tableWriter)
        {
            this.tableWriter = tableWriter;

            Count = 0;
        }

        public IValueWriter this[int key]=> new WriteCopyer<int>(this, key);

        public IEnumerable<int> Keys => throw new NotSupportedException();

        public int Count { get; private set; }

        object IDirectContent.DirectContent
        {
            get => (tableWriter as IDirectContent)?.DirectContent ?? throw new NotSupportedException(StringHelper.Format("Unable Get Content By '{0}' RW.", tableWriter.ToString()));
            set
            {
                if (tableWriter is IDirectContent directContent)
                {
                    directContent.DirectContent = value;
                }
                else
                {
                    throw new NotSupportedException(StringHelper.Format("Unable Set Content By '{0}' RW.", tableWriter.ToString()));
                }
            }
        }

        public void Initialize()
        {
            tableWriter.Initialize();
        }

        public void Initialize(int capacity)
        {
            tableWriter.Initialize(capacity);
        }

        public void OnWriteAll(IDataReader<int> dataReader)
        {
        }

        public void OnWriteValue(int key, IValueReader valueReader)
        {
            if (key < 0)
            {
                throw new IndexOutOfRangeException(nameof(key) + " : " + key);
            }

            if (key == Count - 1)
            {
                goto ReadObject;
            }

            if (key == Count)
            {
                tableWriter.Next();

                ++Count;

                goto ReadObject;
            }

            throw new ArgumentException(StringHelper.Format("Can only write current or next item By '{0}'.", nameof(TableToArrayWriter)));

            ReadObject:
            valueReader.ReadObject(tableWriter);
        }
    }
}
