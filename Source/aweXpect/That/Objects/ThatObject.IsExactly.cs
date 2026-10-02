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
		where T : class
	{
		type.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<T, IThat<T?>>(expectationBuilder.AddConstraint((it, grammars)
				=> new IsExactlyOfTypeConstraint(expectationBuilder, it, grammars, type)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is not exactly of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T?>> IsNotExactly<T>(
		this IThat<T?> subject,
		Type type)
		where T : class
	{
		type.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<T, IThat<T?>>(expectationBuilder.AddConstraint((it, grammars)
				=> new IsExactlyOfTypeConstraint(expectationBuilder, it, grammars, type).Invert()),
			subject);
	}

	private sealed class IsExactlyOfTypeConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Type type)
		: ConstraintResult.WithNotNullValue<object>(it, grammars),
			IValueConstraint<object?>
	{
		private bool _addsContextWhenNegated;

		public ConstraintResult IsMetBy(object? actual)
		{
			Actual = actual;
			Outcome = actual?.GetType() == type ||
			          (type.IsGenericTypeDefinition && actual?.GetType().IsGenericType == true &&
			           actual.GetType().GetGenericTypeDefinition() == type)
				? Outcome.Success
				: Outcome.Failure;
			_addsContextWhenNegated = Outcome == Outcome.Success;
			if (Outcome == Outcome.Failure)
			{
				AddActualContext(false);
			}

			return this;
		}

		public override ConstraintResult Negate()
		{
			base.Negate();
			// A negation after the evaluation (e.g. by `DoesNotComplyWith`) turns the success into a failure.
			// Only the first one adds the context, as the next one reverts it before a repeated evaluation.
			if (_addsContextWhenNegated)
			{
				_addsContextWhenNegated = false;
				AddActualContext(true);
			}

			return this;
		}

		/// <remarks>
		///     A negation after the evaluation (e.g. by <c>DoesNotComplyWith</c>) adds the context of a success, which a
		///     further negation can turn back into a success, so it is then only shown when the outcome is a failure.
		/// </remarks>
		private void AddActualContext(bool onlyOnFailure)
		{
			if (Actual is not null)
			{
				string actual = Formatter.Format(Actual, FormattingOptions.MultipleLines);
				expectationBuilder.AddContext(new ResultContext.SyncCallback("Actual",
					() => onlyOnFailure && Outcome != Outcome.Failure ? null : actual));
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
