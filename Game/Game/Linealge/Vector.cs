using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Linealge
{
    public readonly struct Vector : IEnumerable<double>, ICloneable
    {
        public struct Enumerator : IEnumerator<double>
        {
            public ref double Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _vector[(uint)_index];
            }
            double IEnumerator<double>.Current => Current;
            object IEnumerator.Current => Current;

            Vector _vector;
            int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(in Vector vector)
            {
                _vector = vector;
                _index = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => ++_index < _vector.Count;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset() => _index = -1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() => _vector = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector(double[] values) => new Vector(values);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Zero(uint count) => new Vector(new double[count]);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector One(uint count) => From(1.0, count);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector From(double value, uint count)
        {
            var values = new double[count];
            for (var i = 0u; i < values.Length; i++) values[i] = value;
            return new Vector(values);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector From(Func<uint, double> provider, uint count)
        {
            var values = new double[count];
            for (var i = 0u; i < values.Length; i++) values[i] = provider(i);
            return new Vector(values);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector From<TState>(in TState state, Func<uint, TState, double> provider, uint count)
        {
            var values = new double[count];
            for (var i = 0u; i < values.Length; i++) values[i] = provider(i, state);
            return new Vector(values);
        }

        public ref double this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Values[Index(index)];
        }

        public readonly uint Offset;
        public readonly uint Count;
        public readonly uint Step;
        public readonly uint End;
        public readonly double[] Values;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector(params double[] values) : this(0u, (uint)values.Length, 1u, values) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector(uint offset, uint count, uint step, params double[] values)
        {
            Offset = offset;
            Count = count;
            Step = step;
            End = offset + (count - 1) * step;
            Values = values;
            if (End > Values.Length) throw new ArgumentException(nameof(values));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Index(uint index) => Offset + index * Step;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Map(Func<double, double> map)
        {
            fixed (double* pointer = Values) for (var i = Offset; i <= End; i += Step) pointer[i] = map(pointer[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Map<TState>(in TState state, Func<double, TState, double> map)
        {
            fixed (double* pointer = Values) for (var i = Offset; i <= End; i += Step) pointer[i] = map(pointer[i], state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Add(double scalar)
        {
            fixed (double* pointer = Values) for (var i = Offset; i <= End; i += Step) pointer[i] += scalar;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Subtract(double scalar)
        {
            fixed (double* pointer = Values) for (var i = Offset; i <= End; i += Step) pointer[i] -= scalar;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Multiply(double scalar)
        {
            fixed (double* pointer = Values) for (var i = Offset; i <= End; i += Step) pointer[i] *= scalar;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Divide(double scalar)
        {
            fixed (double* pointer = Values) for (var i = Offset; i <= End; i += Step) pointer[i] /= scalar;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Add(in Vector vector)
        {
            var count = Math.Min(Count, vector.Count);
            fixed (double* a = Values)
            fixed (double* b = vector.Values)
                for (uint c = 0u, i = Offset, j = vector.Offset; c < count; c++, i += Step, j += vector.Step) a[i] += b[j];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Subtract(in Vector vector)
        {
            var count = Math.Min(Count, vector.Count);
            fixed (double* a = Values)
            fixed (double* b = vector.Values)
                for (uint c = 0u, i = Offset, j = vector.Offset; c < count; c++, i += Step, j += vector.Step) a[i] -= b[j];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Multiply(in Vector vector)
        {
            var count = Math.Min(Count, vector.Count);
            fixed (double* a = Values)
            fixed (double* b = vector.Values)
                for (uint c = 0u, i = Offset, j = vector.Offset; c < count; c++, i += Step, j += vector.Step) a[i] *= b[j];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Divide(in Vector vector)
        {
            var count = Math.Min(Count, vector.Count);
            fixed (double* a = Values)
            fixed (double* b = vector.Values)
                for (uint c = 0u, i = Offset, j = vector.Offset; c < count; c++, i += Step, j += vector.Step) a[i] /= b[j];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double Dot(in Vector vector)
        {
            var sum = 0.0;
            var count = Math.Min(Count, vector.Count);
            fixed (double* a = Values)
            fixed (double* b = vector.Values)
                for (uint c = 0u, i = Offset, j = vector.Offset; c < count; c++, i += Step, j += vector.Step) sum += a[i] * b[j];
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double Magnitude2()
        {
            var sum = 0.0;
            fixed (double* pointer = Values) for (var i = Offset; i <= End; i += Step) sum += pointer[i] * pointer[i];
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Magnitude() => Math.Sqrt(Magnitude2());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double Normalize()
        {
            var magnitude = Magnitude();
            fixed (double* pointer = Values) for (var i = Offset; i <= End; i += Step) pointer[i] /= magnitude;
            return magnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double Maximum()
        {
            if (Count == 0) return 0.0;

            var value = Values[Offset];
            fixed (double* pointer = Values) for (var i = Offset + 1u; i <= End; i += Step) value = Math.Max(value, pointer[i]);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double Minimum()
        {
            if (Count == 0) return 0.0;

            var value = Values[Offset];
            fixed (double* pointer = Values) for (var i = Offset + 1u; i <= End; i += Step) value = Math.Min(value, pointer[i]);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => $"[{string.Join(", ", this)}]";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Vector Clone()
        {
            var values = new double[Count];
            fixed (double* pointer = Values) for (uint i = 0u, j = Offset; i < values.Length; i++, j += Step) values[i] = pointer[j];
            return new Vector(values);
        }
        object ICloneable.Clone() => Clone();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<double> IEnumerable<double>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}