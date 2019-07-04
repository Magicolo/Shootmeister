using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Linealge
{
    public readonly struct Matrix : IEnumerable<double>, ICloneable
    {
        public struct Enumerator : IEnumerator<double>
        {
            public ref double Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _matrix.Values[_index];
            }
            double IEnumerator<double>.Current => Current;
            object IEnumerator.Current => Current;

            Matrix _matrix;
            int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(in Matrix matrix)
            {
                _matrix = matrix;
                _index = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => ++_index < _matrix.Values.Length;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset() => _index = -1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() => _matrix = default;
        }

        public readonly struct RowEnumerable : IEnumerable<Vector>
        {
            public uint Count => _matrix._rows;

            readonly Matrix _matrix;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public RowEnumerable(in Matrix matrix) { _matrix = matrix; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public RowEnumerator GetEnumerator() => new RowEnumerator(_matrix);
            IEnumerator<Vector> IEnumerable<Vector>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct RowEnumerator : IEnumerator<Vector>
        {
            public Vector Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _matrix.Row((uint)_index);
            }
            object IEnumerator.Current => Current;

            Matrix _matrix;
            int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public RowEnumerator(in Matrix matrix)
            {
                _matrix = matrix;
                _index = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => ++_index < _matrix._rows;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset() => _index = -1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() => _matrix = default;
        }

        public readonly struct ColumnEnumerable : IEnumerable<Vector>
        {
            public uint Count => _matrix._columns;

            readonly Matrix _matrix;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ColumnEnumerable(in Matrix matrix) { _matrix = matrix; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ColumnEnumerator GetEnumerator() => new ColumnEnumerator(_matrix);
            IEnumerator<Vector> IEnumerable<Vector>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct ColumnEnumerator : IEnumerator<Vector>
        {
            public Vector Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _matrix.Column((uint)_index);
            }
            object IEnumerator.Current => Current;

            Matrix _matrix;
            int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ColumnEnumerator(in Matrix matrix)
            {
                _matrix = matrix;
                _index = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => ++_index < _matrix._columns;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset() => _index = -1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() => _matrix = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix operator *(in Matrix left, in Matrix right)
        {
            left.Multiply(right, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix Identity(uint x, uint y)
        {
            var matrix = new Matrix(x, y);
            var count = Math.Min(x, y);
            for (var i = 0u; i < count; i++) matrix[i, i] = 1;
            return matrix;
        }

        public ref double this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Values[index];
        }
        public ref double this[uint x, uint y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref this[Index(x, y)];
        }

        public RowEnumerable Rows
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new RowEnumerable(this);
        }
        public ColumnEnumerable Columns
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new ColumnEnumerable(this);
        }

        public readonly double[] Values;
        readonly uint _columns;
        readonly uint _rows;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix(double[,] values) : this((uint)values.GetLength(0), (uint)values.GetLength(1)) => values.CopyTo(Values, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix(uint columns, uint rows) : this(columns, rows, new double[columns * rows]) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix(uint columns, uint rows, params double[] values)
        {
            _columns = columns;
            _rows = rows;
            Values = values;
            if (Values.Length != _columns * _rows) throw new ArgumentException(nameof(values));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Index(uint x, uint y) => y + x * _rows;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Row(uint index) => new Vector(index, _columns, _rows, Values);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Column(uint index) => new Vector(index * _rows, _rows, 1, Values);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(double scalar)
        {
            for (var i = 0u; i < Values.Length; i++) Values[i] += scalar;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subtract(double scalar)
        {
            for (var i = 0u; i < Values.Length; i++) Values[i] -= scalar;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Multiply(double scalar)
        {
            for (var i = 0u; i < Values.Length; i++) Values[i] *= scalar;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Divide(double scalar)
        {
            for (var i = 0u; i < Values.Length; i++) Values[i] /= scalar;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Add(in Vector vector)
        {
            var count = Math.Min(_columns, vector.Count);
            fixed (double* a = Values)
            fixed (double* b = vector.Values)
                for (uint x = 0u, i = vector.Offset; x < count; x++, i += vector.Step)
                    for (var y = 0u; y < _rows; y++) a[Index(x, y)] += b[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Subtract(in Vector vector)
        {
            var count = Math.Min(_columns, vector.Count);
            fixed (double* a = Values)
            fixed (double* b = vector.Values)
                for (uint x = 0u, i = vector.Offset; x < count; x++, i += vector.Step)
                    for (var y = 0u; y < _rows; y++) a[Index(x, y)] -= b[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Multiply(in Vector vector)
        {
            var count = Math.Min(_columns, vector.Count);
            fixed (double* a = Values)
            fixed (double* b = vector.Values)
                for (uint x = 0, i = vector.Offset; x < count; x++, i += vector.Step)
                    for (var y = 0u; y < _rows; y++) a[Index(x, y)] *= b[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Divide(in Vector vector)
        {
            var count = Math.Min(_columns, vector.Count);
            fixed (double* a = Values)
            fixed (double* b = vector.Values)
                for (uint x = 0u, i = vector.Offset; x < count; x++, i += vector.Step)
                    for (var y = 0u; y < _rows; y++) a[Index(x, y)] /= b[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool Multiply(in Matrix matrix, out Matrix result)
        {
            if (_columns == matrix._rows)
            {
                result = new Matrix(matrix._columns, _rows);
                fixed (double* a = Values)
                fixed (double* b = matrix.Values)
                fixed (double* c = result.Values)
                    for (var x = 0u; x < result._columns; x++)
                        for (var y = 0u; y < result._rows; y++)
                            for (var i = 0u; i < _columns; i++)
                                c[result.Index(x, y)] += a[Index(i, y)] * b[matrix.Index(x, i)];
                return true;
            }
            result = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool Determinant(out double determinant)
        {
            if (_columns == _rows)
            {
                var skips = stackalloc bool[(int)_columns];
                fixed (double* pointer = Values) determinant = Determinant(0, pointer, skips);
                return true;
            }

            determinant = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Matrix Transpose()
        {
            var matrix = new Matrix(_rows, _columns);
            fixed (double* a = Values)
            fixed (double* b = matrix.Values)
                for (var x = 0u; x < _columns; x++)
                    for (var y = 0u; y < _rows; y++) b[matrix.Index(y, x)] = a[Index(x, y)];
            return matrix;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => string.Join(Environment.NewLine, Rows);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix Clone() => new Matrix(_columns, _rows, Values.Clone() as double[]);
        object ICloneable.Clone() => Clone();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<double> IEnumerable<double>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe double Determinant(uint y, double* pointer, bool* skips)
        {
            if (y == _rows) return 1;

            var sum = 0.0;
            var sign = 1.0;
            for (var x = 0u; x < _columns; x++)
            {
                if (skips[x]) continue;
                skips[x] = true;
                sum += pointer[Index(x, y)] * Determinant(y + 1, pointer, skips) * sign;
                sign = -sign;
                skips[x] = false;
            }
            return sum;
        }
    }
}