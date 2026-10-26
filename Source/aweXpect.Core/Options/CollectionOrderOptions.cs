using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     The options for specifying a <see cref="IComparer{TElement}" /> to use to compare the order of items.
/// </summary>
/// <remarks>
///     An immutable value: <see cref="SetComparer(IComparer{TElement})" /> returns new options, and
///     <see langword="default" /> uses the default comparer, like <see cref="Default" />.
/// </remarks>
[ImmutableOptions]
public readonly struct CollectionOrderOptions<TElement>
{
	private readonly IComparer<TElement>? _comparer;

	private CollectionOrderOptions(IComparer<TElement> comparer)
	{
		_comparer = comparer;
	}

	/// <summary>
	///     The options that compare the order with the default comparer.
	/// </summary>
	public static CollectionOrderOptions<TElement> Default => default;

	/// <summary>
	///     Indicates whether a comparer was specified.
	/// </summary>
	public bool HasComparer => _comparer is not null;

	/// <summary>
	///     Returns the specified comparer or a default comparer.
	/// </summary>
	public IComparer<TElement> GetComparer() => _comparer ?? GetDefaultComparer();

	/// <summary>
	///     Set the comparer to use to compare the order of items.
	/// </summary>
	/// <remarks>
	///     A later comparer replaces the earlier one.
	/// </remarks>
	public CollectionOrderOptions<TElement> SetComparer(IComparer<TElement> comparer)
	{
		comparer.ThrowIfNull();
		return new CollectionOrderOptions<TElement>(comparer);
	}

	private static IComparer<TElement> GetDefaultComparer()
	{
		if (typeof(TElement) == typeof(string))
		{
			return (IComparer<TElement>)StringComparer.Ordinal;
		}

		if (typeof(TElement) == typeof(object))
		{
			return (IComparer<TElement>)(object)ObjectComparer.Instance;
		}

		return Comparer<TElement>.Default;
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
sealed file class ObjectComparer : IComparer<object>
{
	public static ObjectComparer Instance { get; } = new();

	public int Compare(object? x, object? y)
		=> x is string a && y is string b
			? StringComparer.Ordinal.Compare(a, b)
			: Comparer<object>.Default.Compare(x, y);
}
