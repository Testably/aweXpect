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
		///     Verifies that the delegate does not throw any exception.
		/// </summary>
		[GuaranteesNotNull]
		public ExpectationResult DoesNotThrow()
			=> new(ExpectationBuilder.AddConstraint((it, grammars)
				=> new DoesNotThrowConstraint(it, grammars, typeof(Exception))));

		/// <summary>
		///     Verifies that the delegate does not throw an exception of type <typeparamref name="TException" />.
		/// </summary>
		/// <remarks>
		///     Only an exception of type <typeparamref name="TException" /> or of a derived type fails the expectation,
		///     while any other exception is ignored. Use <see cref="DoesNotThrowExactly{TException}()" /> to ignore
		///     derived types as well.
		/// </remarks>
		[GuaranteesNotNull]
		public ExpectationResult DoesNotThrow<TException>()
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
		public ExpectationResult DoesNotThrow(Type type)
			=> new(ExpectationBuilder.AddConstraint((it, grammars) =>
				new DoesNotThrowConstraint(it, grammars, type)));

		private sealed class DoesNotThrowConstraint(string it, ExpectationGrammars grammars, Type exceptionType)
			: ConstraintResult(grammars),
				IValueConstraint<DelegateValue>
		{
			private DelegateValue? _actual;

			/// <inheritdoc cref="ConstraintResult.FailureCause" />
			public override Exception? FailureCause
				=> Outcome == Outcome.Failure ? _actual?.Exception : null;

			/// <inheritdoc />
			public ConstraintResult IsMetBy(DelegateValue value)
			{
				_actual = value;
				if (value.IsNull)
				{
					Outcome = Outcome.Failure;
					return this;
				}

				Outcome = value.Exception is null ||
				          !exceptionType.IsAssignableFrom(value.Exception.GetType())
					? Outcome.Success
					: Outcome.Failure;
				return this;
			}

			public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				if (exceptionType == typeof(Exception))
				{
					stringBuilder.Append("does not throw any exception");
				}
				else
				{
					stringBuilder.Append("does not throw ")
						.Append(Formatter.Format(exceptionType).PrependAOrAn());
				}
			}

			public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			{
				if (_actual?.IsNull != false)
				{
					stringBuilder.ItWasNull(it);
				}
				else
				{
					stringBuilder.Append(it).Append(" did throw ");
					stringBuilder.Append(FormatForMessage(_actual.Exception, indentation));
				}
			}

			public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
			{
				value = default;
				return false;
			}

			public override ConstraintResult Negate() => this;
		}
	}
}
