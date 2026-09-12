using System.IO;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatStream
{
	/// <summary>
	///     Verifies that the subject <see cref="Stream" /> is write-only.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Stream?, IThat<Stream?>> IsWriteOnly(
		this IThat<Stream?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsWriteOnlyConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject <see cref="Stream" /> is not write-only.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Stream?, IThat<Stream?>> IsNotWriteOnly(
		this IThat<Stream?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsWriteOnlyConstraint(it, grammars).Invert()),
			source);

	private sealed class IsWriteOnlyConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Stream?>(it, grammars),
			IValueConstraint<Stream?>
	{
		public ConstraintResult IsMetBy(Stream? actual)
		{
			Actual = actual;
			Outcome = actual is { CanWrite: true, CanRead: false, } ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is write-only");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not write-only");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was");
	}
}
