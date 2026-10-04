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
	public static AndOrResult<T, IThat<T?>> IsExactly<T>(
		this IThat<T?> subject,
		Type type)
	{
		type.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<T, IThat<T?>>(expectationBuilder.AddConstraint(type, static (expectedType, it, grammars)
				=> new IsExactlyOfTypeConstraint<T>(it, grammars, expectedType)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is not exactly of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T?>> IsNotExactly<T>(
		this IThat<T?> subject,
		Type type)
	{
		type.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<T, IThat<T?>>(expectationBuilder.AddConstraint(type,
				static (unexpectedType, it, grammars)
					=> new IsExactlyOfTypeConstraint<T>(it, grammars, unexpectedType).Invert()),
			subject);
	}

	private sealed class IsExactlyOfTypeConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		Type type)
		: ConstraintResult.WithNotNullValue<T?>(it, grammars),
			IValueConstraint<T?>
	{
		public ConstraintResult IsMetBy(T? actual)
		{
			Actual = actual;
			Outcome = actual?.GetType() == (Nullable.GetUnderlyingType(type) ?? type) ||
			          (type.IsGenericTypeDefinition && actual?.GetType().IsGenericType == true &&
			           actual.GetType().GetGenericTypeDefinition() == type)
				? Outcome.Success
				: Outcome.Failure;
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
			stringBuilder.Append(Grammars.Verb("is exactly of type ", "are exactly of type "));
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual!.GetType());
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not exactly of type ", "are not exactly of type "));
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
