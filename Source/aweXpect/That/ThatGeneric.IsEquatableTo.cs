using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatGeneric
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value
	///     using the <see cref="IEquatable{T}.Equals(T)" /> method.
	/// </summary>
	public static AndOrResult<TEquatable, IThat<TEquatable>> IsEquatableTo<T, TEquatable>(
		this IThat<TEquatable> source,
		T expected)
		where TEquatable : IEquatable<T>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEquatableToConstraint<T, TEquatable>(it, grammars, expected)),
			source);

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value
	///     using the <see cref="IEquatable{T}.Equals(T)" /> method.
	/// </summary>
	public static AndOrResult<TEquatable, IThat<TEquatable>> IsNotEquatableTo<T, TEquatable>(
		this IThat<TEquatable> source,
		T unexpected)
		where TEquatable : IEquatable<T>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEquatableToConstraint<T, TEquatable>(it, grammars, unexpected).Invert()),
			source);

	private sealed class IsEquatableToConstraint<T, TEquatable>(
		string it,
		ExpectationGrammars grammars,
		T expected)
		: ConstraintResult.WithValue<TEquatable>(grammars),
			IValueConstraint<TEquatable>
		where TEquatable : IEquatable<T>
	{
		public ConstraintResult IsMetBy(TEquatable actual)
		{
			Actual = actual;
			Outcome = actual is not null && actual.Equals(expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is equatable to ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not equatable to ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was in ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
