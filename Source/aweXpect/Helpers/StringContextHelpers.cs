using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;

namespace aweXpect.Helpers;

internal static class StringContextHelpers
{
	/// <summary>
	///     Adds <paramref name="value" /> as a context with the given <paramref name="title" />, unless it is empty or
	///     the failure message of the <paramref name="result" /> already shows it completely.
	/// </summary>
	/// <remarks>
	///     The check reads the result text, so it runs when the context is added, before the result can be evaluated
	///     again for another item of a collection.
	/// </remarks>
	public static void AddStringContext(this ResultContextCollector contexts, string title, string? value,
		ConstraintResult result)
	{
		if (!string.IsNullOrEmpty(value) && !IsShownCompletely(value!, result))
		{
			contexts.Add(new ResultContext.Fixed(title, value));
		}
	}

	private static bool IsShownCompletely(string value, ConstraintResult result)
	{
		// The formatter escapes line breaks and tabs before it shortens the value, which still shows it completely.
		if (GetEscapedLength(value) > Customize.aweXpect.Formatting().MaximumStringLength.Get())
		{
			return false;
		}

		// Some match types shorten the value further, so it must actually appear in the rendered text.
		string formatted = Formatter.Format(value);
		StringBuilder stringBuilder = new();
		result.AppendExpectation(stringBuilder);
		result.AppendResult(stringBuilder);
		return stringBuilder.ToString().Contains(formatted);
	}

	private static int GetEscapedLength(string value)
		=> value.Length + value.Count(c => c is '\n' or '\r' or '\t');
}
