using System;
using System.Collections.Generic;

namespace aweXpect.Snapshots.PublicApi.Fixture;

public static class Defaults
{
	public const string Text = "line\n\t\"quoted\"\\";
	public const char Letter = '\'';
	public const double NotANumber = double.NaN;
	public const double Infinity = double.PositiveInfinity;
	public const double Fraction = 0.1;
	public const float Single = 1.5f;
	public const long Minimum = long.MinValue;
	public const ulong Maximum = ulong.MaxValue;
	public const string? Nothing = null;

	public static void Values(string? text = null, char letter = 'x', double fraction = 2.5,
		decimal amount = 1.25m, DateTime when = default, int? count = 3, Genres? genres = Genres.Jazz,
		object? value = null, double limit = double.NegativeInfinity, float ratio = float.NaN)
		=> throw new NotSupportedException();

	[Marker("special", Threshold = double.NaN)]
	public static void Special() => throw new NotSupportedException();

	public static T? Generic<T>(T? value = default) => value;
	public static (int Count, string Name) Summary() => (0, "");
	public static dynamic Dynamic(dynamic value) => value;
	public static nint Native(nuint value) => (nint)value;
	public static void References(ref readonly int value, scoped ref int other, in int third = 3)
		=> other = value + third;
}

public readonly struct Money(decimal amount)
{
	public decimal Amount { get; } = amount;

	public static Money operator +(Money left, Money right) => new(left.Amount + right.Amount);
	public static Money operator checked +(Money left, Money right) => new(checked(left.Amount + right.Amount));
	public static Money operator -(Money value) => new(-value.Amount);
	public static bool operator true(Money value) => value.Amount != 0;
	public static bool operator false(Money value) => value.Amount == 0;
	public static Money operator >>>(Money value, int shift) => value;
	public static implicit operator decimal(Money value) => value.Amount;
}

public class Wallet
{
	public decimal Total { get; private set; }

#if NET
	public void operator +=(Money value) => Total += value.Amount;
#endif

	public event EventHandler? Changed
	{
		add => throw new NotSupportedException();
		remove => throw new NotSupportedException();
	}

	public static event Action? Reset;

	private protected void Hidden() => Reset?.Invoke();
	protected private int AlsoHidden => 0;
	protected internal virtual void Shown() => Hidden();
}

public enum Large : ulong
{
	Zero = 0,
	Big = ulong.MaxValue,
	Alias = Zero,
	[Obsolete("Use Big.")] Huge = Big,
}

public class Outer<T>
{
	public class Inner<TOther>
	{
		public Outer<int>.Inner<string>? Sibling { get; set; }
	}

	public class Plain
	{
		public T? Value { get; set; }
	}
}

public static class Constraints
{
	public static void Unmanaged<T>() where T : unmanaged, IComparable<T> => throw new NotSupportedException();
	public static void Nullable<T>() where T : class?, IEnumerable<T>?, new() => throw new NotSupportedException();
	public static void Enum<T>() where T : struct, Enum => throw new NotSupportedException();
	public static void Delegate<T, TBase>() where T : Delegate, TBase => throw new NotSupportedException();
}

public unsafe class Pointers
{
	public int* Raw;
	public delegate*<int, void> Managed;
	public delegate* unmanaged[Cdecl]<int, int> Unmanaged;

	public static void* Address(void* value) => value;
}

#if NET
public interface IChannel
{
	const int Capacity = 8;
	protected static int Count => 0;
	protected internal static int Total => Capacity;
	static string Name => "channel";
	sealed void Close() => throw new NotSupportedException();
	event EventHandler? Opened;
}

public ref struct Span2
{
	public ref int First;
	public ref readonly int Second;
}
#endif
