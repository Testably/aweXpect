using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace aweXpect.Core.Constraints;

public abstract partial class ConstraintResult
{
	/// <summary>
	///     A <see cref="ConstraintResult" /> which only adds an expectation text.
	/// </summary>
	/// <param name="grammars">The expectation grammars.</param>
	/// <param name="expectation">
	///     The expectation text in the normal case.<br />
	///     Will not add anything when set to <see langword="null" />.
	/// </param>
	/// <param name="negatedExpectation">
	///     The expectation text in the negated case.<br />
	///     Will not add anything when set to <see langword="null" />.
	/// </param>
	public class ExpectationOnly<T>(
		ExpectationGrammars grammars,
		string? expectation = null,
		string? negatedExpectation = null)
		: ConstraintResult(grammars), IValueConstraint<T>
	{
		private bool _hasOutcome;
		private Outcome _outcome = Outcome.Success;

		/// <inheritdoc />
		public override Outcome Outcome
		{
			get => _outcome;
			protected set
			{
				_outcome = value;
				_hasOutcome = true;
			}
		}

		/// <inheritdoc />
		/// <remarks>
		///     A derived class which sets the <see cref="Outcome" /> takes part in combinations like any other result.
		/// </remarks>
		internal override bool IsExpectationOnly => !_hasOutcome;

		/// <inheritdoc cref="IValueConstraint{TValue}.IsMetBy(TValue)" />
		public ConstraintResult IsMetBy(T actual) => this;

		/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
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

		/// <summary>
		///     Appends the expectation to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
		///     not negated.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expectation != null)
			{
				stringBuilder.Append(expectation);
			}
		}

		/// <summary>
		///     Appends the expectation to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
		///     negated.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (negatedExpectation != null)
			{
				stringBuilder.Append(negatedExpectation);
			}
		}

		/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}

		/// <inheritdoc cref="ConstraintResult.Negate()" />
		public override ConstraintResult Negate()
		{
			Grammars = Grammars.Negate();
			return this;
		}
	}
}
