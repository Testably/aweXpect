using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	public sealed partial class WithoutValue
	{
		/// <summary>
		///     Verifies that the delegate executes in…
		/// </summary>
		[GuaranteesNotNull]
		public ExecutesInResult<AndResult<WithoutValue>> ExecutesIn()
		{
			TimeSpanEqualityOptions options = new();
			return new ExecutesInResult<AndResult<WithoutValue>>(
				new AndResult<WithoutValue>(ExpectationBuilder.AddConstraint((it, grammars)
						=> new ExecutesInConstraint(it, grammars, options)),
					this),
				options);
		}

		private sealed class ExecutesInConstraint(
			string it,
			ExpectationGrammars grammars,
			TimeSpanEqualityOptions options)
			: ConstraintResult(grammars),
				IValueConstraint<DelegateValue>
		{
			private DelegateValue? _actual;

			/// <inheritdoc />
			public ConstraintResult IsMetBy(DelegateValue value)
			{
				_actual = value;
				if (value.IsNull)
				{
					Outcome = Outcome.Failure;
					return this;
				}

				Outcome = options.IsWithinLimit(value.Duration) ? Outcome.Success : Outcome.Failure;
				return this;
			}

			public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
				stringBuilder.Append("executes ");
				options.AppendTo(stringBuilder, "in ");
			}

			public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			{
				if (_actual?.IsNull != false)
				{
					stringBuilder.ItWasNull(it);
				}
				else
				{
					stringBuilder.Append(it).Append(" took ");
					options.AppendFailureResult(stringBuilder, _actual.Duration);
				}
			}

			public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
			{
				value = default;
				return false;
			}

			public override ConstraintResult Negate()
				=> throw new NotSupportedException($"Negation of {nameof(ExecutesIn)} is not supported.")
					.LogTrace();
		}
	}
}
