using System;
using System.Collections.Generic;

namespace aweXpect.Generators.Tests;

/// <summary>
///     Types whose registered members are compared against reflection, both compiled into this assembly and fed to
///     the generator as source.
/// </summary>
public static class Corpus
{
	public interface IHasValue
	{
		int Value { get; }
	}

	public class Base
	{
		public string BaseField = "";
		public int BaseProperty { get; set; }
	}

	public class Derived : Base
	{
		public int DerivedProperty { get; set; }
	}

	public class Shadowing : Base
	{
		public new string BaseProperty { get; set; } = "";
	}

	public class FieldHidingProperty : Base
	{
		public new int BaseProperty = 1;
	}

	public class HidingPrivately : Base
	{
		public int Own { get; set; }
		private new int BaseProperty { get; set; }

		public override string ToString() => $"{BaseProperty}";
	}

	public class HidingPrivatelyDerived : HidingPrivately
	{
		public int More { get; set; }
	}

	public class HidingGenerically : Generic<int>
	{
		public int Own { get; set; }
		private new int Value { get; set; }

		public override string ToString() => $"{Value}";
	}

	public class WithDynamic
	{
		public dynamic Value { get; set; } = 1;
	}

	public class HidingDynamic : WithDynamic
	{
		public int Own { get; set; }
		private new object Value { get; set; } = 2;

		public override string ToString() => $"{Value}";
	}

	public class Outer<T>
	{
		public class Inner<TInner> : Generic<T>
		{
			public int Own { get; set; }
			private new TInner Value { get; set; } = default!;

			public override string ToString() => $"{Value}";
		}
	}

	public class WithIndexer
	{
		public int Count { get; set; }
		public int this[int index] => index;
	}

	public class WithWriteOnly
	{
		private int _value;
		public int Readable { get; set; }

		public int WriteOnly
		{
			set => _value = value;
		}

		public override string ToString() => $"{_value}";
	}

	public class WithStatics
	{
		public const int Constant = 1;
		public static readonly int StaticField = 2;
		public static int StaticProperty { get; set; }
		public int Instance { get; set; }
	}

	public class Generic<T>
	{
		public T Value { get; set; } = default!;
		public List<T> Items { get; set; } = [];
	}

	public class WithExplicitInterface : IHasValue
	{
		public int Own { get; set; }
		int IHasValue.Value => Own;
	}

	public record PositionalRecord(int Id, string Name);

	public struct Point
	{
		public int X;
		public int Y { get; set; }
	}

	public class WithBigTuple
	{
		public (int, int, int, int, int, int, int, int) Eight { get; set; }
	}

	public class WithInitOnly
	{
		public int Value { get; init; }
	}

	public class WithKeywords
	{
		public string @event = "";
		public int @class { get; set; }
	}

	public class WithObsolete
	{
		[Obsolete("gone")] public int Old { get; set; }
		public int Current { get; set; }
	}

	public class WithRefProperty
	{
		private int _value;
		public int Other { get; set; }
		public ref int Value => ref _value;
	}

	public class WithVisibilities
	{
		private readonly int _private = 1;
		internal int Internal = 2;
		protected int Protected = 3;
		public int Public = 4;

		public override string ToString() => $"{_private}{Internal}{Protected}{Public}";
	}
}
