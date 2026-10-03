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
	public sealed partial class WithValue<T>
	{
		/// <summary>
		///     Verifies that the delegate does not throw any exception.
		/// </summary>
		[GuaranteesNotNull]
		public DelegateWithValueResult<T> DoesNotThrow()
			=> new(ExpectationBuilder.AddConstraint((it, grammars) =>
				new DoesNotThrowConstraint(it, grammars, typeof(Exception))));

		/// <summary>
		///     Verifies that the delegate does not throw an exception of type <typeparamref name="TException" />.
		/// </summary>
		/// <remarks>
		///     Only an exception of type <typeparamref name="TException" /> or of a derived type fails the expectation,
		///     while any other exception is ignored. Use <see cref="DoesNotThrowExactly{TException}()" /> to ignore
		///     derived types as well.
		/// </remarks>
		[GuaranteesNotNull]
		public DelegateWithValueResult<T> DoesNotThrow<TException>()
			where TException : Exception
			=> new(ExpectationBuilder.AddConstraint((it, grammars) =>
				new DoesNotThrowConstraint(it, grammars, typeof(TException))));

		/// <summary>
		///     Verifies that the delegate does not throw an exception of type <paramref name="type" />.
		/// </summary>
		/// <remarks>
		///     Only an exception of the <paramref name="type" /> or of a derived type fails the expectation, while any
		///     other exception is ignored. Use <see cref="DoesNotThrowExactly(Type)" /> to ignore derived types as well.
		/// </remarks>
		[GuaranteesNotNull]
		public DelegateWithValueResult<T> DoesNotThrow(Type type)
		{
			type.ThrowIfNotAnExceptionType();
			return new(ExpectationBuilder.AddConstraint((it, grammars) =>
				new DoesNotThrowConstraint(it, grammars, type)));
		}

		private sealed class DoesNotThrowConstraint(
			string it,
			ExpectationGrammars grammars,
			Type exceptionType)
			: ConstraintResult(grammars),
				IValueConstraint<DelegateValue<T>>
		{
			private DelegateValue<T>? _actual;
			private bool _isNegated;

			/// <inheritdoc cref="ConstraintResult.FailureCause" />
			public override Exception? FailureCause
				=> Outcome is Outcome.Failure or Outcome.FailureBothWays ? _actual?.Exception : null;

			/// <inheritdoc />
			public ConstraintResult IsMetBy(DelegateValue<T> value)
			{
				_actual = value;
				UpdateOutcome(value);
				return this;
			}

			public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				if (!_isNegated)
				{
					if (exceptionType == typeof(Exception))
					{
						stringBuilder.Append("does not throw any exception");
					}
					else
					{
						stringBuilder.Append("does not throw ")
							.Append(ValueFormatters.Format(Formatter, exceptionType).PrependAOrAn());
					}
				}
				else
				{
					if (exceptionType == typeof(Exception))
					{
						stringBuilder.Append("throws an exception");
					}
					else
					{
						stringBuilder.Append("throws ")
							.Append(ValueFormatters.Format(Formatter, exceptionType).PrependAOrAn());
					}
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
				if (_actual is { Value: TValue typedValue, })
				{
					value = typedValue;
					return true;
				}

				value = default;
				return typeof(TValue).IsAssignableFrom(typeof(T));
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

			private void UpdateOutcome(DelegateValue<T> value)
			{
				if (value.IsNull || value.ExceededTimeout is not null)
				{
					Outcome = Outcome.FailureBothWays;
					return;
				}

				Outcome = _isNegated == (value.Exception is null ||
				                         !value.Exception.IsOfType(exceptionType))
					? Outcome.Failure
					: Outcome.Success;
			}
		}
	}
}
