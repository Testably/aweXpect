using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatObject
{
	/// <summary>
	///     Verifies that the subject is <see langword="null" />.
	/// </summary>
	public static AndOrResult<T?, IThat<T?>> IsNull<T>(
		this IThat<T?> subject)
		where T : class
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsNullConstraint<T?>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is <see langword="null" />.
	/// </summary>
	public static AndOrResult<T?, IThat<T?>> IsNull<T>(
		this IThat<T?> subject)
		where T : struct
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsNullConstraint<T?>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not <see langword="null" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T?>> IsNotNull<T>(
		this IThat<T?> subject)
		where T : class
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsNullConstraint<T?>(it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not <see langword="null" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T?>> IsNotNull<T>(
		this IThat<T?> subject)
		where T : struct
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsNullConstraint<T?>(it, grammars).Invert()),
			subject);

	private sealed class IsNullConstraint<T>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<T>(grammars),
			IValueConstraint<T>
	{
		public ConstraintResult IsMetBy(T actual)
		{
			Actual = actual;
			Outcome = actual is null ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is null");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not null");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" was");
	}
}
