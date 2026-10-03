using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
	///     The check runs only when a failure message is built, because the message text is needed to decide it. With
	///     <paramref name="onlyOnFailure" />, the context is also left out unless the <paramref name="result" /> failed
	///     in the end, as a negation after the evaluation (e.g. by <c>DoesNotComplyWith</c>) adds the context of a
	///     success, which a further negation can turn back into a success.
	/// </remarks>
	public static void AddStringContext(this ExpectationBuilder expectationBuilder, string title, string value,
		ConstraintResult result, bool onlyOnFailure = false)
		=> expectationBuilder.AddContext(new StringContext(title, value, result, onlyOnFailure));

	/// <remarks>
	///     A dedicated context instead of a callback, because a succeeding string expectation adds it as well, and a
	///     closure with its delegate would be allocated for a message that is rarely built.
	/// </remarks>
	private sealed class StringContext(string title, string value, ConstraintResult result, bool onlyOnFailure)
		: ResultContext(title)
	{
		public override Task<string?> GetContent(CancellationToken cancellationToken = default)
			=> Task.FromResult((onlyOnFailure && result.Outcome != Outcome.Failure) || IsShownCompletely()
				? null
				: value);

		private bool IsShownCompletely()
		{
			// The formatter escapes line breaks and tabs before it shortens the value, which still shows it completely.
			if (GetEscapedLength() > Customize.aweXpect.Formatting().MaximumStringLength.Get())
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

		private int GetEscapedLength()
			=> value.Length + value.Count(c => c is '\n' or '\r' or '\t');
	}
}
