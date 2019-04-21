using Swifter.Readers;
using Swifter.Tools;
using Swifter.Writers;
using System;

namespace Swifter.RW
{

    sealed class DefaultValueReader : IValueReader, IValueReader<Guid>, IValueReader<DateTimeOffset>, IValueReader<TimeSpan>
    {
        public static readonly DefaultValueReader Instance = new DefaultValueReader();

        public object DirectRead() => default(object);

        public void ReadArray(IDataWriter<int> valueWriter) { }

        public bool ReadBoolean() => default(bool);

        public byte ReadByte() => default(byte);

        public char ReadChar() => default(char);

        public DateTime ReadDateTime() => default(DateTime);

        public decimal ReadDecimal() => default(decimal);

        public double ReadDouble() => default(double);

        public short ReadInt16() => default(short);

        public int ReadInt32() => default(int);

        public long ReadInt64() => default(long);

        public T? ReadNullable<T>() where T : struct => default(T?);

        public void ReadObject(IDataWriter<string> valueWriter)
        {
        }

        public sbyte ReadSByte() => default(sbyte);

        public float ReadSingle() => default(float);

        public string ReadString() => default(string);

        public ushort ReadUInt16() => default(ushort);

        public uint ReadUInt32() => default(uint);

        public ulong ReadUInt64() => default(ulong);

        Guid IValueReader<Guid>.ReadValue() => default(Guid);

        TimeSpan IValueReader<TimeSpan>.ReadValue() => default(TimeSpan);

        DateTimeOffset IValueReader<DateTimeOffset>.ReadValue() => default(DateTimeOffset);
    }
}
