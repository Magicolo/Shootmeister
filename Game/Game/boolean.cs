using System;
using System.Runtime.CompilerServices;

namespace Game
{
    public readonly struct boolean : IEquatable<boolean>, IEquatable<bool>, IComparable<boolean>, IComparable<bool>, IComparable
    {
        public static readonly boolean True = new boolean(true);
        public static readonly boolean False = new boolean(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static boolean operator |(boolean left, boolean right) => (bool)left | (bool)right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static boolean operator &(boolean left, boolean right) => (bool)left & (bool)right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static boolean operator ^(boolean left, boolean right) => (bool)left ^ (bool)right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(boolean left, boolean right) => (bool)left == (bool)right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(boolean left, boolean right) => (bool)left != (bool)right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(boolean value) => value._value != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator boolean(bool value) => new boolean(value);

        readonly byte _value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public boolean(bool value) : this(value ? (byte)1 : (byte)0) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        boolean(byte value) { _value = value; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(boolean other) => ((bool)this).Equals(other);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(bool other) => ((bool)this).Equals(other);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is boolean value1 && Equals(value1) || obj is bool value2 && Equals(value2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(boolean other) => ((bool)this).CompareTo(other);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(bool other) => ((bool)this).CompareTo(other);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object obj) => obj is boolean value1 ? CompareTo(value1) : obj is bool value2 ? CompareTo(value2) : 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => ((bool)this).GetHashCode();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => ((bool)this).ToString();
    }
}