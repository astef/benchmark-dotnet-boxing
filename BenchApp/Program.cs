using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

namespace BenchApp
{
    [WarmupCount(1)]
    [IterationCount(5)]
    //[EventPipeProfiler(EventPipeProfile.GcVerbose)]
    [MemoryDiagnoser]
    public class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<Program>();
        }

        [Params(500, 1000, 2000, 4000, 8000, 16000)]
        public int IterCount { get; set; }

        [Params(5000, 10000, 20000, 40000, 80000, 160000)]
        public int MessageNum { get; set; }

        [Benchmark]
        public void RunWithObjects()
        {
            var domain = new Domain();

            for (int i = 0; i < IterCount; i++)
            {
                RunWithObjectsInternal(domain);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void RunWithObjectsInternal(Domain domain)
        {
            // init
            object[] objBuffer = new object[Domain.ObjectNumber * MessageNum];
            int objBufferCurrentIndex = 0;
            domain.Reset();

            while (objBufferCurrentIndex != objBuffer.Length)
            {
                domain.Increment();

                domain.WriteMessageToObjectBuffer(
                    objBuffer.AsSpan(objBufferCurrentIndex, Domain.ObjectNumber));

                objBufferCurrentIndex += Domain.ObjectNumber;
            }
        }

        [Benchmark]
        public void RunWithBytes()
        {
            var domain = new Domain();

            for (int i = 0; i < IterCount; i++)
            {
                RunWithBytesInternal(domain);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void RunWithBytesInternal(Domain domain)
        {
            // init
            Span<byte> byteBuffer = new byte[Domain.MessageSize * MessageNum];
            var byteBufferCurrentIndex = 0;
            domain.Reset();

            while (byteBufferCurrentIndex != byteBuffer.Length)
            {
                domain.Increment();

                domain.WriteMessageToByteBuffer(
                    byteBuffer.Slice(byteBufferCurrentIndex, Domain.MessageSize));

                byteBufferCurrentIndex += Domain.MessageSize;
            }
        }
    }

    internal sealed class Domain
    {
        public const int MessageSize = 8 + 8 + 8;

        public const int ObjectNumber = 3;

        private readonly Field<long> LongField = new Field<long>(0L);

        private readonly Field<DateTimeOffset> DateTimeOffsetField = new Field<DateTimeOffset>(new DateTimeOffset());

        private readonly Field<double> DoubleField = new Field<double>(0.0d);

        public void Increment()
        {
            LongField.Value++;
            DateTimeOffsetField.Value = DateTimeOffsetField.Value.AddTicks(1);
            DoubleField.Value += 0.001;
        }

        public void Reset()
        {
            LongField.Value = 0L;
            DateTimeOffsetField.Value = new DateTimeOffset();
            DoubleField.Value = 0.0d;
        }

        public void WriteMessageToObjectBuffer(Span<object> objBuffer)
        {
            objBuffer[0] = LongField.Value;
            objBuffer[1] = DateTimeOffsetField.Value.Ticks;
            objBuffer[2] = DoubleField.Value;
        }

        public void WriteMessageToByteBuffer(Span<byte> byteBuffer)
        {
            BitConverter.TryWriteBytes(byteBuffer, LongField.Value);
            BitConverter.TryWriteBytes(byteBuffer, DateTimeOffsetField.Value.Ticks);
            BitConverter.TryWriteBytes(byteBuffer, DoubleField.Value);
        }
    }

    internal sealed class Field<T>
    {
        public Field(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}
