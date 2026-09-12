using System.IO;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatStream
{
	/// <summary>
	///     Verifies that the subject <see cref="Stream" /> is read-only.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Stream?, IThat<Stream?>> IsReadOnly(
		this IThat<Stream?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsReadOnlyConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject <see cref="Stream" /> is not read-only.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Stream?, IThat<Stream?>> IsNotReadOnly(
		this IThat<Stream?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsReadOnlyConstraint(it, grammars).Invert()),
			source);

	private sealed class IsReadOnlyConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Stream?>(it, grammars),
			IValueConstraint<Stream?>
	{
		public ConstraintResult IsMetBy(Stream? actual)
		{
			Actual = actual;
			Outcome = actual is { CanWrite: false, CanRead: true, } ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is read-only");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not read-only");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was");
	}
}
