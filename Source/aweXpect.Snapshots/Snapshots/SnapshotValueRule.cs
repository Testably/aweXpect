using System;
using System.Collections;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Linq.Expressions;
using aweXpect.Core;
using aweXpect.Equivalency;
using aweXpect.Formatting;

namespace aweXpect.Snapshots;

/// <summary>
///     Selects the values nested in an object snapshot, e.g. by the name of their member or by their type.
/// </summary>
internal sealed class SnapshotValueRule
{
	private readonly Func<FormattedValue, bool> _matches;
	private readonly string? _placeholderName;

	private SnapshotValueRule(Func<FormattedValue, bool> matches, string? placeholderName)
	{
		_matches = matches;
		_placeholderName = placeholderName;
	}

	/// <summary>
	///     Selects the members whose path ends with the <paramref name="memberName" /> on whole segments, ignoring the
	///     case, like an ignored member of an equivalency comparison, e.g. <c>Id</c>, <c>Artist.Id</c> or
	///     <c>Tracks[0].Id</c>.
	/// </summary>
	/// <remarks>
	///     The placeholder is named after the last segment, e.g. <c>Id</c> for <c>Artist.Id</c>, so that the same value
	///     in several members of that name keeps the same number.
	/// </remarks>
	public static SnapshotValueRule ForMember(string memberName)
	{
		if (memberName is null)
		{
			throw Tracing.WriteException(
				new ArgumentNullException(nameof(memberName), "The 'memberName' cannot be null."));
		}

		if (string.IsNullOrWhiteSpace(memberName))
		{
			throw Tracing.WriteException(new ArgumentException("The 'memberName' cannot be empty.", nameof(memberName)));
		}

		MemberToIgnore.ByName member = new(memberName);
		return new SnapshotValueRule(value => member.IgnoreMember(value.Path, value.Type), GetLastSegment(memberName));
	}

	/// <summary>
	///     Selects the values for which the <paramref name="predicate" /> returns <see langword="true" />.
	/// </summary>
	/// <remarks>
	///     The placeholder is named after the member or tuple item, e.g. <c>Url</c> or <c>Item1</c>, or <c>Item</c> for
	///     an item of a collection or the value of a dictionary entry.
	/// </remarks>
	public static SnapshotValueRule ForPredicate(Func<FormattedValue, bool> predicate)
	{
		if (predicate is null)
		{
			throw Tracing.WriteException(
				new ArgumentNullException(nameof(predicate), "The 'predicate' cannot be null."));
		}

		return new SnapshotValueRule(predicate, null);
	}

	/// <summary>
	///     Selects the member of <typeparamref name="T" /> that the <paramref name="member" /> accesses, in an object of
	///     the type <typeparamref name="T" /> or of a derived type.
	/// </summary>
	/// <remarks>
	///     The expression is only read, never compiled. Only a field or property of the parameter itself is accepted,
	///     as the formatter does not tell the type of the object that contains a nested member, so
	///     <c>a =&gt; a.Artist.Name</c> could not be told apart from the <c>Name</c> of an <c>Artist</c> anywhere else.
	///     The placeholder is named after the member.
	/// </remarks>
	public static SnapshotValueRule ForMemberOf<T>(Expression<Func<T, object?>> member)
	{
		string memberName = GetMemberName(member);
		return new SnapshotValueRule(value => value.Kind == FormattedValueKind.Member &&
		                                      string.Equals(value.Name, memberName, StringComparison.Ordinal) &&
		                                      typeof(T).IsAssignableFrom(value.DeclaringType), memberName);
	}

	/// <summary>
	///     Selects the values of the <paramref name="type" />, by their declared or their runtime type, including
	///     <see cref="Nullable{T}" /> and derived types.
	/// </summary>
	/// <remarks>
	///     The placeholder is named after the type without its generic arity, e.g. <c>Guid</c> or <c>List</c>.
	/// </remarks>
	public static SnapshotValueRule ForType(Type type)
	{
		Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
		string name = underlyingType.Name;
		int arity = name.IndexOf('`');
		return new SnapshotValueRule(value => IsOfType(value, underlyingType),
			arity > 0 ? name.Substring(0, arity) : name);
	}

	/// <summary>
	///     Selects the values that are an empty collection, see <see cref="IsEmptyCollection" />.
	/// </summary>
	public static SnapshotValueRule ForEmptyCollections()
		=> new(IsEmptyCollection, null);

	/// <summary>
	///     Whether the rule selects the <paramref name="value" />.
	/// </summary>
	public bool Matches(FormattedValue value)
		=> _matches(value);

	/// <summary>
	///     The name of a numbered placeholder for the selected <paramref name="value" />, e.g. <c>Id</c> for
	///     <c>{Id_1}</c>.
	/// </summary>
	public string GetPlaceholderName(FormattedValue value)
		=> _placeholderName ?? (value.Kind == FormattedValueKind.CollectionItem ? "Item" : value.Name);

	/// <remarks>
	///     The value is only read when its declared type could hold a value of the <paramref name="type" />, e.g. an
	///     <see cref="object" /> member, so that unrelated members are not read. A value that cannot be read is not
	///     selected, as the formatter writes its exception instead.
	/// </remarks>
	private static bool IsOfType(FormattedValue value, Type type)
	{
		Type declaredType = Nullable.GetUnderlyingType(value.Type) ?? value.Type;
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
	private static bool IsEmptyCollection(FormattedValue value)
	{
		if (!CanHoldCollection(Nullable.GetUnderlyingType(value.Type) ?? value.Type))
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

	/// <summary>
	///     Reads the name of the field or property of the parameter itself that the <paramref name="member" /> accesses,
	///     without compiling it.
	/// </summary>
	/// <remarks>
	///     A nested member, also of a cast parameter, gets its own message, which names the type that declares it.
	/// </remarks>
	private static string GetMemberName<T>(Expression<Func<T, object?>> member)
	{
		if (member is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(nameof(member), "The 'member' cannot be null."));
		}

		ParameterExpression parameter = member.Parameters[0];
		if (SkipConversions(member.Body) is MemberExpression access)
		{
			if (access.Expression == parameter)
			{
				return access.Member.Name;
			}

			if (IsWithin(access.Expression, parameter))
			{
				throw Tracing.WriteException(new ArgumentException(
					$"The 'member' must access a field or property of the {typeof(T).Name} itself, but '{member}' accesses a nested member. Select it by the type that declares it instead, e.g. 'MemberOf<{access.Member.DeclaringType?.Name}>(x => x.{access.Member.Name})'.",
					nameof(member)));
			}
		}

		throw Tracing.WriteException(new ArgumentException(
			$"The 'member' must access a field or property of the {typeof(T).Name} itself, e.g. 'x => x.Id', but was '{member}'.",
			nameof(member)));
	}

	/// <summary>
	///     Whether the <paramref name="expression" /> is the <paramref name="parameter" /> or a member access on it.
	/// </summary>
	private static bool IsWithin(Expression? expression, ParameterExpression parameter)
	{
		Expression? current = SkipConversions(expression);
		while (current is MemberExpression access)
		{
			current = SkipConversions(access.Expression);
		}

		return current == parameter;
	}

	/// <remarks>
	///     The compiler converts a value type to <see cref="object" />, and a cast converts the parameter.
	/// </remarks>
	private static Expression? SkipConversions(Expression? expression)
	{
		while (expression is UnaryExpression
		       {
			       NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.TypeAs,
		       } conversion)
		{
			expression = conversion.Operand;
		}

		return expression;
	}

	private static string GetLastSegment(string memberName)
	{
		string segment = memberName.Substring(memberName.LastIndexOf('.') + 1);
		int index = segment.IndexOf('[');
		if (index > 0)
		{
			return segment.Substring(0, index);
		}

		return index == 0 ? "Item" : segment;
	}
}
