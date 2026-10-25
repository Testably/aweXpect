using System;

namespace aweXpect.Json;

/// <summary>
///     A member that is ignored in the comparison of JSON, matched against its JSON path, e.g. <c>$.items[0].id</c>.
/// </summary>
internal abstract class JsonMemberToIgnore
{
	public abstract bool IsIgnored(string memberPath);

	/// <summary>
	///     Ignores the members whose JSON path ends with the whole segments of the <paramref name="memberName" />.
	/// </summary>
	/// <remarks>
	///     The names are compared case-sensitively, as JSON property names are, unlike the member names of an
	///     equivalency comparison.
	/// </remarks>
	internal sealed class ByName(string memberName) : JsonMemberToIgnore
	{
		/// <summary>
		///     The name of the members to ignore, as it was provided.
		/// </summary>
		public string MemberName => memberName;

		public override bool IsIgnored(string memberPath)
		{
			if (!memberPath.EndsWith(memberName, StringComparison.Ordinal))
			{
				return false;
			}

			int segmentStart = memberPath.Length - memberName.Length;
			return segmentStart == 0 || memberName[0] == '[' || memberPath[segmentStart - 1] == '.';
		}

		public override string ToString() => $"\"{memberName}\"";
	}

	internal sealed class ByPredicate(Func<string, bool> predicate, string description) : JsonMemberToIgnore
	{
		public override bool IsIgnored(string memberPath)
			=> predicate(memberPath);

		public override string ToString() => description;
	}
}
