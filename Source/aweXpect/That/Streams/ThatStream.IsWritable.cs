using System.IO;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatStream
{
	/// <summary>
	///     Verifies that the subject <see cref="Stream" /> is writable.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Stream?, IThat<Stream?>> IsWritable(
		this IThat<Stream?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsWritableConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject <see cref="Stream" /> is not writable.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Stream?, IThat<Stream?>> IsNotWritable(
		this IThat<Stream?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsWritableConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsWritableConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Stream?>(it, grammars),
			IValueConstraint<Stream?>
	{
		public ConstraintResult IsMetBy(Stream? actual)
		{
			Actual = actual;
			Outcome = actual?.CanWrite == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is writable");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not writable");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was");
	}
}
