using System;
using System.Collections;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using aweXpect.Formatting;

namespace aweXpect.Snapshots;

/// <summary>
///     The filters that select values nested in an object snapshot, which never throw for a value that cannot be read.
/// </summary>
internal static class ValueFilters
{
	/// <summary>
	///     Whether the <paramref name="path" /> of the value ends with the <paramref name="expected" /> path on whole
	///     segments, e.g. <c>Artist.Id</c> for <c>Albums[0].Artist.Id</c>, but not for <c>Albums[0].Artist.CatalogId</c>.
	/// </summary>
	/// <remarks>
	///     A segment starts after a <c>.</c> or with the <c>[</c> of an index, so <c>[0].Id</c> matches
	///     <c>Tracks[0].Id</c>. The segments are compared ordinally.
	/// </remarks>
	public static bool HasPath(string path, string expected)
	{
		if (!path.EndsWith(expected, StringComparison.Ordinal))
		{
			return false;
		}

		int start = path.Length - expected.Length;
		return start == 0 || path[start - 1] == '.' || expected[0] == '[';
	}

	/// <summary>
	///     Whether the value is of the <paramref name="type" />, by its declared or its runtime type, including
	///     <see cref="Nullable{T}" /> and derived types.
	/// </summary>
	/// <remarks>
	///     The value is only read when its declared type could hold a value of the <paramref name="type" />, e.g. an
	///     <see cref="object" /> member, so that unrelated members are not read. A value that cannot be read is not
	///     selected, as the formatter writes its exception instead.
	/// </remarks>
	public static bool IsOfType(FormattedValue value, Type type)
	{
		Type declaredType = GetUnderlyingType(value.Type);
		if (type.IsAssignableFrom(declaredType))
		{
			return true;
		}

		if (!declaredType.IsAssignableFrom(type))
		{
			return false;
		}

		try
		{
			return type.IsInstanceOfType(value.GetValue());
		}
		catch (Exception)
		{
			return false;
		}
	}

	/// <summary>
	///     Whether the value is exactly of the <paramref name="type" />: its runtime type, or for a
	///     <see langword="null" /> value its declared type without <see cref="Nullable{T}" />.
	/// </summary>
	/// <remarks>
	///     The value is only read when its declared type could hold a value of the <paramref name="type" /> and could
	///     also hold another type, so that unrelated members are not read. A value that cannot be read is not selected.
	/// </remarks>
	public static bool IsOfExactType(FormattedValue value, Type type)
	{
		Type declaredType = GetUnderlyingType(value.Type);
		if (!declaredType.IsAssignableFrom(type))
		{
			return false;
		}

		if (declaredType.IsValueType || declaredType.IsSealed)
		{
			return declaredType == type;
		}

		try
		{
			return (value.GetValue()?.GetType() ?? declaredType) == type;
		}
		catch (Exception)
		{
			return false;
		}
	}

	/// <summary>
	///     The name of the <paramref name="type" /> for a numbered placeholder, without <see cref="Nullable{T}" /> and
	///     without its generic arity, e.g. <c>Guid</c> or <c>List</c>.
	/// </summary>
	public static string GetPlaceholderName(Type type)
	{
		string name = GetUnderlyingType(type).Name;
		int arity = name.IndexOf('`');
		return arity > 0 ? name.Substring(0, arity) : name;
	}

	public static Type GetUnderlyingType(Type type)
		=> Nullable.GetUnderlyingType(type) ?? type;

	/// <summary>
	///     Whether the value is a collection without items: an <see cref="IEnumerable" /> other than a
	///     <see cref="string" /> whose count is known, e.g. an array, a list, a set or a dictionary.
	/// </summary>
	/// <remarks>
	///     A sequence that does not know its count, like a LINQ query or an iterator, is never enumerated here, as it
	///     might only be enumerable once or compute its items, so it is kept. A generic collection that is no
	///     <see cref="ICollection" />, e.g. a <see cref="HashSet{T}" /> of a value type, is asked for its first item
	///     instead, as a collection can be enumerated again. The value is only read when its declared type could hold a
	///     collection, and a value that cannot be read, or whose count throws, is kept, so that the formatter writes it.
	/// </remarks>
	public static bool IsEmptyCollection(FormattedValue value)
	{
		if (!CanHoldCollection(GetUnderlyingType(value.Type)))
		{
			return false;
		}

		try
		{
			return value.GetValue() switch
			{
				string => false,
				ICollection collection => collection.Count == 0,
				IReadOnlyCollection<object?> collection => collection.Count == 0,
				IEnumerable enumerable when IsGenericCollection(enumerable.GetType()) => !HasItems(enumerable),
				_ => false,
			};
		}
		catch (Exception)
		{
			return false;
		}
	}

	private static bool CanHoldCollection(Type type)
		=> type != typeof(string) &&
		   (typeof(IEnumerable).IsAssignableFrom(type) || !(type.IsValueType || type.IsSealed));

#if NET8_0_OR_GREATER
	[UnconditionalSuppressMessage("Trimming", "IL2070",
		Justification = "A dropped interface is reported as absent, so the collection is kept like a sequence.")]
#endif
	private static bool IsGenericCollection(Type type)
		=> type.GetInterfaces().Any(interfaceType => interfaceType.IsGenericType &&
		                                             interfaceType.GetGenericTypeDefinition() is var definition &&
		                                             (definition == typeof(ICollection<>) ||
		                                              definition == typeof(IReadOnlyCollection<>)));

	private static bool HasItems(IEnumerable enumerable)
	{
		IEnumerator enumerator = enumerable.GetEnumerator();
		try
		{
			return enumerator.MoveNext();
		}
		finally
		{
			(enumerator as IDisposable)?.Dispose();
		}
	}
}
