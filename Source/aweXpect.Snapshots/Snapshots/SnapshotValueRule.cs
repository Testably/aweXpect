using System;
using aweXpect.Core;
using aweXpect.Equivalency;
using aweXpect.Formatting;

namespace aweXpect.Snapshots;

/// <summary>
///     Selects the values nested in an object snapshot by the name of their member or by their type.
/// </summary>
internal sealed class SnapshotValueRule
{
	private readonly MemberToIgnore.ByName? _member;
	private readonly Type? _type;

	private SnapshotValueRule(MemberToIgnore.ByName? member, Type? type, string placeholderName)
	{
		_member = member;
		_type = type;
		PlaceholderName = placeholderName;
	}

	/// <summary>
	///     The name of a numbered placeholder for the selected values, e.g. <c>Id</c> for <c>{Id_1}</c>.
	/// </summary>
	public string PlaceholderName { get; }

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

		return new SnapshotValueRule(new MemberToIgnore.ByName(memberName), null, GetLastSegment(memberName));
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
		return new SnapshotValueRule(null, underlyingType, arity > 0 ? name.Substring(0, arity) : name);
	}

	/// <summary>
	///     Whether the rule selects the <paramref name="value" />.
	/// </summary>
	public bool Matches(FormattedValue value)
	{
		if (_member is not null)
		{
			return _member.IgnoreMember(value.Path, value.Type);
		}

		return IsOfType(value, _type!);
	}

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
