using aweXpect.Reflection.Formatting;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     The results of the generated expectations on a single subject.
/// </summary>
/// <remarks>
///     A result describes the state of the subject instead of the negation of the expectation, so that it stays true
///     in every combination of expectations.
/// </remarks>
internal static class ResultTexts
{
	/// <summary>
	///     Describes the <paramref name="subject" /> as <paramref name="whenTrue" /> or <paramref name="whenFalse" />,
	///     depending on the <paramref name="condition" />, e.g. "was sealed class Track".
	/// </summary>
	public static string Describe(bool condition, string whenTrue, string whenFalse, object subject)
		=> $"{(condition ? whenTrue : whenFalse)} {Formatter.Format(FormattableMember.From(subject))}";

	/// <summary>
	///     Describes the <paramref name="subject" /> after the <paramref name="state" />, e.g. "was class Track".
	/// </summary>
	public static string Describe(string state, object subject)
		=> $"{state} {Formatter.Format(FormattableMember.From(subject))}";
}
