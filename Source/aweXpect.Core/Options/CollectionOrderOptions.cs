using System;
using System.Collections.Generic;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     The options for specifying a <see cref="IComparer{TItem}" /> to use to compare the order of items.
/// </summary>
public record CollectionOrderOptions<TItem>
{
	private IComparer<TItem>? _comparer;

	/// <summary>
	///     Indicates whether a comparer was specified.
	/// </summary>
	public bool HasComparer => _comparer is not null;

	/// <summary>
	///     Returns the specified comparer or a default comparer.
	/// </summary>
	public IComparer<TItem> GetComparer() => _comparer ?? GetDefaultComparer();

	/// <summary>
	///     Set the comparer to use to compare the order of items.
	/// </summary>
	public void SetComparer(IComparer<TItem> comparer)
	{
		comparer.ThrowIfNull();
		_comparer = comparer;
	}

	private static IComparer<TItem> GetDefaultComparer()
	{
		if (typeof(TItem) == typeof(string))
		{
			return (IComparer<TItem>)StringComparer.Ordinal;
		}

		if (typeof(TItem) == typeof(object))
		{
			return (IComparer<TItem>)(object)ObjectComparer.Instance;
		}

		return Comparer<TItem>.Default;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		if (_comparer == null)
		{
			return "";
		}

		return $" using {Formatter.Format(_comparer.GetType())}";
	}
}

/// <summary>
///     Orders two strings ordinally, as for a collection of strings, and any other items with the default comparer.
/// </summary>
file sealed class ObjectComparer : IComparer<object>
{
	public static ObjectComparer Instance { get; } = new();

	public int Compare(object? x, object? y)
		=> x is string a && y is string b
			? StringComparer.Ordinal.Compare(a, b)
			: Comparer<object>.Default.Compare(x, y);
}
