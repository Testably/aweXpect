using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Constraints;

public abstract partial class ConstraintResult
{
	/// <summary>
	///     A typed <see cref="ConstraintResult" /> for equality comparison to ensure that
	///     <see langword="null" /> is handled consistently.
	/// </summary>
	/// <param name="it">The <c>it</c> parameter.</param>
	/// <param name="grammars">The expectation grammars.</param>
	/// <param name="isExpectedNull">
	///     Flag indicating if the expected parameter for the equality comparison is
	///     <see langword="null" /> or not.
	/// </param>
	/// <remarks>
	///     Set <see cref="Actual" /> in one of the <c>IsMetBy</c> overloads of <see cref="IConstraint" /> and overwrite<br />
	///     - <see cref="AppendNormalExpectation" /> / <see cref="AppendNegatedExpectation" />
	///     which add the normal and negated expectation strings<br />
	///     - <see cref="AppendNormalResult" /> / <see cref="AppendNegatedResult" />
	///     which add the normal and negated result strings
	/// </remarks>
	public abstract class WithEqualToValue<T>(string it, ExpectationGrammars grammars, bool isExpectedNull)
		: ConstraintResult(grammars)
	{
		private Outcome _outcome = Outcome.Undecided;

		/// <summary>
		///     Flag indicating if the constraint is negated.
		/// </summary>
		protected bool IsNegated { get; private set; }

		/// <summary>
		///     The `it` parameter.
		/// </summary>
		protected string It { get; } = it;

		/// <summary>
		///     The actual value.
		/// </summary>
		protected T? Actual { get; set; }

		/// <inheritdoc />
		public override Outcome Outcome
		{
			get
			{
				if (Actual is null && IsNegated == isExpectedNull)
				{
					return Outcome.Failure;
				}

				return (_outcome, _isNegated: IsNegated) switch
				{
					(Outcome.Failure, true) => Outcome.Success,
					(Outcome.Success, true) => Outcome.Failure,
					_ => _outcome,
				};
			}
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
			if (Actual is null)
			{
				stringBuilder.ItWasNull(It, Grammars);
			}
			else if (Outcome == Outcome.Undecided)
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
