using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatObject
{
	/// <summary>
	///     Verifies that the subject is of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T?>> Is<T>(
		this IThat<T?> subject,
		Type type)
		where T : class
	{
		type.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<T, IThat<T?>>(expectationBuilder.AddConstraint(type, static (expectedType, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, expectedType)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is not of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T?>> IsNot<T>(
		this IThat<T?> subject,
		Type type)
		where T : class
	{
		type.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<T, IThat<T?>>(expectationBuilder.AddConstraint(type,
				static (unexpectedType, it, grammars)
					=> new IsOfTypeConstraint(it, grammars, unexpectedType).Invert()),
			subject);
	}

	private sealed class IsOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		Type type)
		: ConstraintResult.WithNotNullValue<object>(it, grammars),
			IValueConstraint<object?>
	{
		public ConstraintResult IsMetBy(object? actual)
		{
			Actual = actual;
			Outcome = type.IsOrImplements(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (Actual is { } actual)
			{
				contexts.Add(new ResultContext.SyncCallback("Actual",
					() => Formatter.Format(actual, FormattingOptions.MultipleLines)));
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is of type ", "are of type "));
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual!.GetType());
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not of type ", "are not of type "));
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
