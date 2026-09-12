using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatObject
{
	/// <summary>
	///     Verifies that the subject is exactly of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<object?, IThat<object?>> IsExactly(
		this IThat<object?> source,
		Type type)
	{
		type.ThrowIfNull();
		return new AndOrResult<object?, IThat<object?>>(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsExactlyOfTypeConstraint(it, grammars, type)),
			source);
	}

	/// <summary>
	///     Verifies that the subject is not exactly of type <paramref name="type" />.
	/// </summary>
	public static AndOrResult<object?, IThat<object?>> IsNotExactly(
		this IThat<object?> source,
		Type type)
	{
		type.ThrowIfNull();
		return new AndOrResult<object?, IThat<object?>>(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsExactlyOfTypeConstraint(it, grammars, type).Invert()),
			source);
	}

	private sealed class IsExactlyOfTypeConstraint(string it, ExpectationGrammars grammars, Type type)
		: ConstraintResult.WithValue<object>(grammars),
			IValueConstraint<object?>
	{
		public ConstraintResult IsMetBy(object? actual)
		{
			Actual = actual;
			Outcome = actual?.GetType() == type ||
			          (type.IsGenericTypeDefinition && actual?.GetType().IsGenericType == true &&
			           actual.GetType().GetGenericTypeDefinition() == type)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is exactly type ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual, FormattingOptions.Indented(indentation, true));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not exactly type ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
