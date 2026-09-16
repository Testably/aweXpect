using System;
using System.Collections.Generic;

namespace aweXpect.Generators.Tests;

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

	public class HidingByReference : Base
	{
		private int _value;
		public int Own { get; set; }
		private new ref int BaseProperty => ref _value;
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

	public interface IHasEvent
	{
		event EventHandler Happened;
	}

	public class Publisher
	{
		public delegate void CountedHandler(int count, string name, bool flag, DateTime at, int? optional);

		public event EventHandler? Changed;
		public event CountedHandler? Counted;
		public static event EventHandler? StaticChanged;
		internal event EventHandler? Internal;
		protected event EventHandler? Protected;
		private event EventHandler? Private;

		public void Raise()
		{
			Changed?.Invoke(this, EventArgs.Empty);
			Counted?.Invoke(1, "", true, DateTime.MinValue, null);
			StaticChanged?.Invoke(this, EventArgs.Empty);
			Internal?.Invoke(this, EventArgs.Empty);
			Protected?.Invoke(this, EventArgs.Empty);
			Private?.Invoke(this, EventArgs.Empty);
		}
	}

	public class PublisherDerived : Publisher
	{
		public event Action? Own;

		public void RaiseOwn() => Own?.Invoke();
	}

	public class PublisherHiding : Publisher
	{
		public new event Action? Changed;

		public void RaiseHiding() => Changed?.Invoke();
	}

	public class PublisherHidingPrivately : Publisher
	{
		public event Action? Own;
		private new event Action? Changed;

		public void RaiseHidingPrivately()
		{
			Own?.Invoke();
			Changed?.Invoke();
		}
	}

	public class PublisherHidingPrivatelyDerived : PublisherHidingPrivately
	{
		public event Action? More;

		public void RaiseMore() => More?.Invoke();
	}

	public class PublisherHidingInternally : Publisher
	{
		public event Action? Own;
		internal new event Action? Changed;

		public void RaiseHidingInternally()
		{
			Own?.Invoke();
			Changed?.Invoke();
		}
	}

	public class PublisherHidingByProperty : Publisher
	{
		public new int Changed { get; set; }
	}

	public class PublisherWithExplicitInterface : IHasEvent
	{
		public event Action? Own;

		event EventHandler IHasEvent.Happened
		{
			add { }
			remove { }
		}

		public void RaiseOwn() => Own?.Invoke();
	}

	public class PublisherWithKeywords
	{
		public event Action? @event;

		public void RaiseEvent() => @event?.Invoke();
	}

	public class PublisherWithObsolete
	{
		[Obsolete("gone")] public event Action? Old;
		public event Action? Current;

		public void RaiseAll()
		{
#pragma warning disable CS0618
			Old?.Invoke();
#pragma warning restore CS0618
			Current?.Invoke();
		}
	}

	public class GenericPublisher<T>
	{
		public event EventHandler<T>? Received;
		public event Action<List<T>>? Batched;

		public void Raise(T value)
		{
			Received?.Invoke(this, value);
			Batched?.Invoke([value,]);
		}
	}

	public class GenericPublisherDerived : GenericPublisher<int>
	{
		public event Action? Own;

		public void RaiseOwn() => Own?.Invoke();
	}
}
