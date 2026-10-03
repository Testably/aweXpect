using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;
using aweXpect.Results;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	public sealed partial class WithoutValue
	{
		/// <summary>
		///     Verifies that the delegate does not throw an exception of exactly type <typeparamref name="TException" />
		///     (subtypes are allowed).
		/// </summary>
		[GuaranteesNotNull]
		public ExpectationResult DoesNotThrowExactly<TException>()
			where TException : Exception
			=> new(ExpectationBuilder.AddConstraint((it, grammars) =>
				new DoesNotThrowExactlyConstraint(it, grammars, typeof(TException))));

		/// <summary>
		///     Verifies that the delegate does not throw an exception of exactly type <paramref name="type" />
		///     (subtypes are allowed).
		/// </summary>
		[GuaranteesNotNull]
		public ExpectationResult DoesNotThrowExactly(Type type)
		{
			type.ThrowIfNotAnExceptionType();
			return new(ExpectationBuilder.AddConstraint(type, static (exceptionType, it, grammars) =>
				new DoesNotThrowExactlyConstraint(it, grammars, exceptionType)));
		}

		private sealed class DoesNotThrowExactlyConstraint(string it, ExpectationGrammars grammars, Type exceptionType)
			: ConstraintResult(grammars),
				IValueConstraint<DelegateValue>
		{
			private DelegateValue? _actual;
			private bool _isNegated;

			/// <inheritdoc cref="ConstraintResult.FailureCause" />
			public override Exception? FailureCause
				=> Outcome is Outcome.Failure or Outcome.FailureBothWays ? _actual?.Exception : null;

			/// <inheritdoc />
			public ConstraintResult IsMetBy(DelegateValue value)
			{
				_actual = value;
				UpdateOutcome(value);
				return this;
			}

			public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				if (!_isNegated)
				{
					stringBuilder.Append("does not throw exactly ")
						.Append(Formatter.Format(exceptionType).PrependAOrAn());
				}
				else
				{
					stringBuilder.Append("throws exactly ")
						.Append(Formatter.Format(exceptionType).PrependAOrAn());
				}
			}

			public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			{
				if (_actual?.IsNull != false)
				{
					AppendNullResult(stringBuilder, it, _actual);
				}
				else if (_actual.ExceededTimeout is { } exceededTimeout)
				{
					stringBuilder.ItDidNotFinishWithin(it, exceededTimeout);
				}
				else if (_actual.Exception is null)
				{
					stringBuilder.Append(it).Append(" did not throw any exception");
				}
				else
				{
					stringBuilder.Append(it).Append(" did throw ");
					stringBuilder.Append(FormatForMessage(_actual.Exception, indentation));
				}
			}

			public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
			{
				value = default;
				return false;
			}

			/// <remarks>
			///     A negating expectation, such as <c>DoesNotComplyWith</c>, negates the result after the evaluation, so
			///     the outcome is updated as well.
			/// </remarks>
			public override ConstraintResult Negate()
			{
				_isNegated = !_isNegated;
				if (_actual is not null)
				{
					UpdateOutcome(_actual);
				}

				return this;
			}

			private void UpdateOutcome(DelegateValue value)
			{
				if (value.IsNull || value.ExceededTimeout is not null)
				{
					Outcome = Outcome.FailureBothWays;
					return;
				}

				Outcome = _isNegated == !value.Exception.IsExactlyOfType(exceptionType)
					? Outcome.Failure
					: Outcome.Success;
			}
		}
	}
}
