using System;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;

namespace aweXpect.Xml;

/// <summary>
///     A member that is ignored in the comparison of XML, matched against its path, e.g. <c>/order/item[2]/@price</c>.
/// </summary>
internal abstract class XmlMemberToIgnore
{
	public abstract bool IsIgnored(string path);

	/// <summary>
	///     Ignores the members whose path ends with the segments of the <paramref name="pattern" />, or, when it starts
	///     with the separator, whose path equals them.
	/// </summary>
	/// <remarks>
	///     The names are compared case-sensitively, as in XML. A segment without a position, e.g. <c>item</c>, matches
	///     every element of the name, and a segment with a position, e.g. <c>item[2]</c>, only that one, so that
	///     <c>item[1]</c> also matches an element without siblings of the same name.
	/// </remarks>
	internal sealed class ByPath(string pattern) : XmlMemberToIgnore
	{
		private readonly bool _isAnchored = pattern[0] == XmlComparer.PathSeparator;
		private readonly string[] _segments = Split(pattern);

		public override bool IsIgnored(string path)
		{
			string[] pathSegments = Split(path);
			int offset = pathSegments.Length - _segments.Length;
			if (offset < 0 || (_isAnchored && offset != 0))
			{
				return false;
			}

			for (int index = 0; index < _segments.Length; index++)
			{
				if (!Matches(_segments[index], pathSegments[offset + index]))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		///     The pattern as specified by the user.
		/// </summary>
		public string Pattern { get; } = pattern;

		/// <summary>
		///     Returns the part of the <paramref name="path" /> that the pattern would have to equal to match it, as a
		///     candidate for the pattern a user meant, or <see langword="null" /> if the pattern cannot cover the path.
		/// </summary>
		/// <remarks>
		///     The position is removed from a segment of the path, when the pattern has none for it, so that the paths of
		///     several siblings of the same name are one candidate instead of equally close ones, which suggest nothing.
		/// </remarks>
		public string? GetCandidate(string path)
		{
			string[] pathSegments = Split(path);
			int offset = pathSegments.Length - _segments.Length;
			if (offset < 0 || (_isAnchored && offset != 0))
			{
				return null;
			}

			string[] candidate = new string[_segments.Length];
			for (int index = 0; index < _segments.Length; index++)
			{
				string pathSegment = pathSegments[offset + index];
				candidate[index] = Parse(_segments[index]).Position is null ? Parse(pathSegment).Name : pathSegment;
			}

			string joined = string.Join(XmlComparer.PathSeparator.ToString(), candidate);
			return _isAnchored ? XmlComparer.PathSeparator + joined : joined;
		}

		public override string ToString()
			=> Formatter.Format(Pattern);

		internal static string[] Split(string path)
			=> path.Split([XmlComparer.PathSeparator,], StringSplitOptions.RemoveEmptyEntries);

		/// <remarks>
		///     The positions are compared as text, as a path omits the position of an element without siblings of the same
		///     name, but never writes it with leading zeros.
		/// </remarks>
		private static bool Matches(string patternSegment, string pathSegment)
		{
			(string patternName, string? patternPosition) = Parse(patternSegment);
			(string pathName, string? pathPosition) = Parse(pathSegment);
			return patternName == pathName && (patternPosition is null || patternPosition == (pathPosition ?? "1"));
		}

		private static (string Name, string? Position) Parse(string segment)
		{
			int start = segment.LastIndexOf('[');
			return start > 0 && segment[segment.Length - 1] == ']'
				? (segment.Substring(0, start), segment.Substring(start + 1, segment.Length - start - 2))
				: (segment, null);
		}
	}

	internal sealed class ByPredicate(Func<string, bool> predicate, string expression) : XmlMemberToIgnore
	{
		public override bool IsIgnored(string path)
			=> predicate(path);

		public override string ToString()
			=> expression;
	}
}
