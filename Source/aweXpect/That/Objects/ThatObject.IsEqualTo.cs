using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatObject
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ObjectEqualityResult<object?, IThat<object?>, object?> IsEqualTo(
		this IThat<object?> subject,
		object? expected)
	{
		ObjectEqualityOptions<object?> options = new();
		return new ObjectEqualityResult<object?, IThat<object?>, object?>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static (state, it, grammars)
					=> new IsEqualToConstraint<object?, object?>(it, grammars, state.Expected, null, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ObjectEqualityResult<T?, IThat<T?>, T?> IsEqualTo<T>(
		this IThat<T?> subject,
		T? expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where T : struct
	{
		ObjectEqualityOptions<T?> options = new();
		return new ObjectEqualityResult<T?, IThat<T?>, T?>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static (state, it, grammars)
					=> new NullableIsEqualToConstraint<T>(it, grammars, state.Expected, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ObjectEqualityResult<T, IThat<T>, T> IsEqualTo<T>(
		this IThat<T> subject,
		T? expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where T : struct
	{
		ObjectEqualityOptions<T> options = new();
		return new ObjectEqualityResult<T, IThat<T>, T>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static (state, it, grammars)
					=> new IsEqualToConstraint<T>(it, grammars, state.Expected, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static ObjectEqualityResult<object?, IThat<object?>, object?> IsNotEqualTo(
		this IThat<object?> subject,
		object? unexpected)
	{
		ObjectEqualityOptions<object?> options = new();
		return new ObjectEqualityResult<object?, IThat<object?>, object?>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Options: options),
				static (state, it, grammars)
					=> new IsEqualToConstraint<object?, object?>(it, grammars, state.Unexpected, null, state.Options)
						.Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static ObjectEqualityResult<T?, IThat<T?>, T?> IsNotEqualTo<T>(
		this IThat<T?> subject,
		T? unexpected,
		[CallerArgumentExpression("unexpected")]
		string doNotPopulateThisValue = "")
		where T : struct
	{
		ObjectEqualityOptions<T?> options = new();
		return new ObjectEqualityResult<T?, IThat<T?>, T?>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Options: options),
				static (state, it, grammars)
					=> new NullableIsEqualToConstraint<T>(it, grammars, state.Unexpected, state.Options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static ObjectEqualityResult<T, IThat<T>, T> IsNotEqualTo<T>(
		this IThat<T> subject,
		T? unexpected,
		[CallerArgumentExpression("unexpected")]
		string doNotPopulateThisValue = "")
		where T : struct
	{
		ObjectEqualityOptions<T> options = new();
		return new ObjectEqualityResult<T, IThat<T>, T>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Options: options),
				static (state, it, grammars)
					=> new IsEqualToConstraint<T>(it, grammars, state.Unexpected, state.Options).Invert()),
			subject,
			options);
	}

	private sealed class IsEqualToConstraint<TSubject, TExpected>(
		string it,
		ExpectationGrammars grammars,
		TExpected expected,
		string? expectedExpression,
		ObjectEqualityOptions<TSubject> options)
		: ConstraintResult.WithEqualToValue<TSubject>(it, grammars, expected is null),
			IAsyncConstraint<TSubject>
	{
		private IObjectMatchResult? _matchResult;

		public async ValueTask<ConstraintResult> IsMetBy(TSubject actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			_matchResult = await options.AreConsideredEqualWithExplanation(actual, expected);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> options.AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				expectedExpression ?? Formatter.Format(expected, FormattingOptions.Indented(indentation)), Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_matchResult!.GetExtendedFailure(It, Grammars, Actual, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				expectedExpression ?? Formatter.Format(expected, FormattingOptions.Indented(indentation)), Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_matchResult!.GetExtendedFailure(It, Grammars, Actual, expected));
	}

	private sealed class IsEqualToConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		T? expected,
		ObjectEqualityOptions<T> options)
		: ConstraintResult.WithValue<T>(it, grammars),
			IAsyncConstraint<T>
		where T : struct
	{
		private IObjectMatchResult? _matchResult;

		public async ValueTask<ConstraintResult> IsMetBy(T actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			_matchResult = await options.AreConsideredEqualWithExplanation(actual, expected);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> options.AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				Formatter.Format(expected, FormattingOptions.Indented(indentation)), Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_matchResult!.GetExtendedFailure(It, Grammars, Actual, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				Formatter.Format(expected, FormattingOptions.Indented(indentation)), Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_matchResult!.GetExtendedFailure(It, Grammars, Actual, expected));
	}

	private sealed class NullableIsEqualToConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		T? expected,
		ObjectEqualityOptions<T?> options)
		: ConstraintResult.WithEqualToValue<T?>(it, grammars, expected is null),
			IAsyncConstraint<T?>
		where T : struct
	{
		private IObjectMatchResult? _matchResult;

		public async ValueTask<ConstraintResult> IsMetBy(T? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			_matchResult = await options.AreConsideredEqualWithExplanation(actual, expected);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> options.AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				Formatter.Format(expected, FormattingOptions.Indented(indentation)), Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_matchResult!.GetExtendedFailure(It, Grammars, Actual, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				Formatter.Format(expected, FormattingOptions.Indented(indentation)), Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_matchResult!.GetExtendedFailure(It, Grammars, Actual, expected));
	}
}
