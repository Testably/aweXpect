using System.IO;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     See https://awexpect.com/docs/extensions/write-extensions
/// </summary>
public static class DummyExtensions
{
	/// <summary>
	///     Verifies that the <paramref name="subject" /> is an absolute path.
	/// </summary>
	public static AndOrResult<string, IThat<string>> IsAbsolutePath(
		this IThat<string> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsAbsolutePathConstraint(it, grammars)),
			subject);

	private sealed class IsAbsolutePathConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<string>(grammars),
			IValueConstraint<string>
	{
		public ConstraintResult IsMetBy(string actual)
		{
			Actual = actual;
			Outcome = Path.IsPathRooted(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is an absolute path");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is no negated path");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
