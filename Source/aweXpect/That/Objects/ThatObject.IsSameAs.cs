using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatObject
{
	/// <summary>
	///     Verifies the actual value to be the same as the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T?, IThat<T?>> IsSameAs<T>(this IThat<T?> source, object? expected)
		where T : class
		=> new(source.Get().ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new IsSameAsConstraint<T>(it, grammars, expected)),
			source);

	/// <summary>
	///     Verifies the actual value to not be the same as the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T?, IThat<T?>> IsNotSameAs<T>(this IThat<T?> source, object? unexpected)
		where T : class
		=> new(source.Get().ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new IsSameAsConstraint<T>(it, grammars, unexpected).Invert()),
			source);

	private sealed class IsSameAsConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		object? expected)
		: ConstraintResult.WithNotNullValue<T>(it, grammars),
			IValueConstraint<T>
	{
		public ConstraintResult IsMetBy(T actual)
		{
			Actual = actual;
			Outcome = ReferenceEquals(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("refers to ");
			Formatter.Format(stringBuilder, expected, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It);
			stringBuilder.Append(" was ");
			Formatter.Format(stringBuilder, Actual, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("does not refer to ");
			Formatter.Format(stringBuilder, expected, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It);
			stringBuilder.Append(" did");
		}
	}
}
