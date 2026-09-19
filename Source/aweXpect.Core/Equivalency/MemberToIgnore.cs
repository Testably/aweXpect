using System;

namespace aweXpect.Equivalency;

#pragma warning disable S1694 // Convert this abstract class to an interface
/// <summary>
///     Class to specify which members to ignore.
/// </summary>
public abstract class MemberToIgnore
{
	/// <summary>
	///     Checks if the member should be ignored.
	/// </summary>
	public abstract bool IgnoreMember(string memberPath, Type memberType);

	/// <summary>
	///     Ignores all members that satisfy the <paramref name="predicate" />.
	/// </summary>
	public class ByPredicate(Func<string, Type, bool> predicate, string description) : MemberToIgnore
	{
		/// <inheritdoc cref="MemberToIgnore.IgnoreMember(string, Type)" />
		public override bool IgnoreMember(string memberPath, Type memberType)
			=> predicate(memberPath, memberType);

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString() => description;
	}

	/// <summary>
	///     Ignores all fields that satisfy the <paramref name="predicate" />.
	/// </summary>
	public sealed class ByFieldPredicate(Func<string, Type, bool> predicate, string description)
		: ByPredicate(predicate, description);

	/// <summary>
	///     Ignores all properties that satisfy the <paramref name="predicate" />.
	/// </summary>
	public sealed class ByPropertyPredicate(Func<string, Type, bool> predicate, string description)
		: ByPredicate(predicate, description);

	/// <summary>
	///     Ignores all members that have the provided <paramref name="memberName" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="memberName" /> must cover whole segments of the member path, so that it can match a member at
	///     any depth without an abbreviated or misspelled name silently widening the exclusion.
	/// </remarks>
	public class ByName(string memberName) : MemberToIgnore
	{
		/// <inheritdoc cref="MemberToIgnore.IgnoreMember(string, Type)" />
		public override bool IgnoreMember(string memberPath, Type memberType)
		{
			if (memberName.Length == 0 ||
			    !memberPath.EndsWith(memberName, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			int segmentStart = memberPath.Length - memberName.Length;
			return segmentStart == 0 ||
			       memberName[0] == '[' ||
			       memberPath[segmentStart - 1] is '.' or ']';
		}

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString() => $"\"{memberName}\"";
	}
}
#pragma warning restore S1694 // Convert this abstract class to an interface
