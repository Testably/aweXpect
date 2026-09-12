#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the subject is parsable into type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="formatProvider" /> provides culture-specific formatting information
	///     in the call to <see cref="IParsable{TType}.Parse(string, IFormatProvider)" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static IsParsableResult<TType> IsParsableInto<TType>(
		this IThat<string?> source,
		IFormatProvider? formatProvider = null)
		where TType : IParsable<TType>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsParsableIntoConstraint<TType>(it, grammars, formatProvider)),
			source,
			formatProvider);

	/// <summary>
	///     Verifies that the subject is not parsable into type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="formatProvider" /> provides culture-specific formatting information
	///     in the call to <see cref="IParsable{TType}.Parse(string, IFormatProvider)" />.
	/// </remarks>
	public static AndOrResult<string?, IThat<string?>> IsNotParsableInto<TType>(
		this IThat<string?> source,
		IFormatProvider? formatProvider = null)
		where TType : IParsable<TType>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsParsableIntoConstraint<TType>(it, grammars, formatProvider).Invert()),
			source);

	private sealed class IsParsableIntoConstraint<TType> : ConstraintResult.WithValue<string?>,
		IValueConstraint<string?>
		where TType : IParsable<TType>
	{
		private readonly IFormatProvider? _formatProvider;
		private readonly string _it;
		private string? _exceptionMessage;

		public IsParsableIntoConstraint(string it,
			ExpectationGrammars grammars,
			IFormatProvider? formatProvider) : base(grammars)
		{
			_it = it;
			_formatProvider = formatProvider;
			FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
		}

		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			try
			{
				_ = TType.Parse(actual, _formatProvider);
				Outcome = Outcome.Success;
			}
			catch (Exception ex)
			{
				if (string.IsNullOrEmpty(ex.Message) || ex.Message.Length < 2)
				{
					_exceptionMessage = "an unknown error occurred";
				}
				else
				{
					_exceptionMessage = char.ToLowerInvariant(ex.Message[0]) + ex.Message[1..^1];
				}

				Outcome = Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is parsable into ");
			Formatter.Format(stringBuilder, typeof(TType));
			if (_formatProvider is not null)
			{
				stringBuilder.Append(" using ");
				Formatter.Format(stringBuilder, _formatProvider);
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.ItWasNull(_it);
			}
			else
			{
				stringBuilder.Append(_it).Append(" was not because ").Append(_exceptionMessage);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not parsable into ");
			Formatter.Format(stringBuilder, typeof(TType));
			if (_formatProvider is not null)
			{
				stringBuilder.Append(" using ");
				Formatter.Format(stringBuilder, _formatProvider);
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_it).Append(" was");
	}
}
#endif
