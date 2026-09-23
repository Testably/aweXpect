using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;

namespace aweXpect.Helpers;

internal static class StringContextHelpers
{
	/// <summary>
	///     Adds <paramref name="value" /> as a context with the given <paramref name="title" />, unless the failure
	///     message of the <paramref name="result" /> already shows it completely.
	/// </summary>
	/// <remarks>
	///     The check runs only when a failure message is built, because the message text is needed to decide it.
	/// </remarks>
	public static void AddStringContext(this ExpectationBuilder expectationBuilder, string title, string value,
		ConstraintResult result)
		=> expectationBuilder.AddContext(new ResultContext.SyncCallback(title,
			() => IsShownCompletely(value, result) ? null : value));

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
