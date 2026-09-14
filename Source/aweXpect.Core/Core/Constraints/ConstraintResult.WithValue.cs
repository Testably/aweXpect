using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace aweXpect.Core.Constraints;

public abstract partial class ConstraintResult
{
	/// <summary>
	///     A typed <see cref="ConstraintResult" /> which stores the actual value in the <see cref="Actual" /> property and
	///     applies no <see langword="null" /> policy of its own.
	/// </summary>
	/// <remarks>
	///     Use it only when the subject cannot be <see langword="null" /> at all, or when the expectation is one of the
	///     <see langword="null" /> checks that a <see langword="null" /> subject is meant to satisfy, such as <c>IsNull</c>
	///     or <c>IsOneOf</c>. For an expectation that inspects the subject use
	///     <see cref="ConstraintResult.WithNotNullValue{T}" /> instead: deciding the outcome with
	///     <c>Actual is null ? Outcome.Failure : …</c> inside <c>IsMetBy</c> is not equivalent, because
	///     <see cref="Outcome" /> inverts that failure into a success as soon as the expectation is negated, while
	///     <see cref="ConstraintResult.WithNotNullValue{T}" /> decides before the inversion is applied.
	///     <para />
	///     Set <see cref="Actual" /> in one of the <c>IsMetBy</c> overloads of <see cref="IConstraint" /> and overwrite<br />
	///     - <see cref="AppendNormalExpectation" /> / <see cref="AppendNegatedExpectation" />
	///     which add the normal and negated expectation strings<br />
	///     - <see cref="AppendNormalResult" /> / <see cref="AppendNegatedResult" />
	///     which add the normal and negated result strings
	/// </remarks>
	public abstract class WithValue<T>(ExpectationGrammars grammars) : ConstraintResult(grammars)
	{
		private Outcome _outcome = Outcome.Undecided;

		/// <summary>
		///     Flag indicating if the constraint is negated.
		/// </summary>
		protected bool IsNegated { get; private set; }

		/// <summary>
		///     The actual value.
		/// </summary>
		protected T? Actual { get; set; }

		/// <inheritdoc />
		public override Outcome Outcome
		{
			get => (_outcome, _isNegated: IsNegated) switch
			{
				(Outcome.Failure, true) => Outcome.Success,
				(Outcome.Success, true) => Outcome.Failure,
				(_, _) => _outcome,
			};
			protected set => _outcome = value;
		}

		/// <summary>
		///     Appends the expectation to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
		///     not negated.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null);

		/// <summary>
		///     Appends the result to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are not
		///     negated.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null);

		/// <summary>
		///     Appends the expectation to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
		///     negated.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null);

		/// <summary>
		///     Appends the result to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
		///     negated.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null);

		/// <summary>
		///     Appends the result to the <paramref name="stringBuilder" /> when the <see cref="Outcome" />
		///     is <see cref="Outcome.Undecided" />.
		/// </summary>
		protected virtual void AppendUndecidedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("could not verify, because it was already cancelled");

		/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sealed override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.IsNegated())
			{
				AppendNegatedExpectation(stringBuilder, indentation);
			}
			else
			{
				AppendNormalExpectation(stringBuilder, indentation);
			}
		}

		/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sealed override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Outcome == Outcome.Undecided)
			{
				AppendUndecidedResult(stringBuilder, indentation);
			}
			else if (Grammars.IsNegated())
			{
				AppendNegatedResult(stringBuilder, indentation);
			}
			else
			{
				AppendNormalResult(stringBuilder, indentation);
			}
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (Actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(T));
		}

		/// <inheritdoc cref="ConstraintResult.Negate()" />
		public override ConstraintResult Negate()
		{
			Grammars = Grammars.Negate();
			IsNegated = !IsNegated;
			return this;
		}
	}
}
