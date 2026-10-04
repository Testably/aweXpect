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
		this IThat<string?> subject,
		IFormatProvider? formatProvider = null)
		where TType : IParsable<TType>
		=> new(subject.Get().ExpectationBuilder.AddConstraint(formatProvider, static (provider, it, grammars)
				=> new IsParsableIntoConstraint<TType>(it, grammars, provider)),
			subject,
			formatProvider);

	/// <summary>
	///     Verifies that the subject is not parsable into type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="formatProvider" /> provides culture-specific formatting information
	///     in the call to <see cref="IParsable{TType}.Parse(string, IFormatProvider)" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsNotParsableInto<TType>(
		this IThat<string?> subject,
		IFormatProvider? formatProvider = null)
		where TType : IParsable<TType>
		=> new(subject.Get().ExpectationBuilder.AddConstraint(formatProvider, static (provider, it, grammars)
				=> new IsParsableIntoConstraint<TType>(it, grammars, provider).Invert()),
			subject);

	private sealed class IsParsableIntoConstraint<TType> : ConstraintResult.WithNotNullValue<string?>,
		IValueConstraint<string?>
		where TType : IParsable<TType>
	{
		private readonly IFormatProvider? _formatProvider;
		private Exception? _exception;
		private TType? _parsedValue;

		public IsParsableIntoConstraint(string it,
			ExpectationGrammars grammars,
			IFormatProvider? formatProvider) : base(it, grammars)
		{
			_formatProvider = formatProvider;
		}

		/// <inheritdoc />
		/// <remarks>
		///     The parse exception is why the negated expectation is met, so it only explains a failure when not negated.
		/// </remarks>
		public override Exception? FailureCause => Outcome == Outcome.Failure ? _exception : null;

		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			_exception = null;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			try
			{
				_parsedValue = TType.Parse(actual, _formatProvider);
				Outcome = Outcome.Success;
			}
			catch (Exception ex)
			{
				_exception = ex;
				Outcome = Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is parsable into ", "are parsable into "));
			Formatter.Format(stringBuilder, typeof(TType));
			if (_formatProvider is not null)
			{
				stringBuilder.Append(" using ");
				Formatter.Format(stringBuilder, _formatProvider);
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("Parse of ");
			Formatter.Format(stringBuilder, typeof(TType));
			stringBuilder.Append(" did throw ").Append(_exception!.FormatForMessage(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not parsable into ", "are not parsable into "));
			Formatter.Format(stringBuilder, typeof(TType));
			if (_formatProvider is not null)
			{
				stringBuilder.Append(" using ");
				Formatter.Format(stringBuilder, _formatProvider);
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.Append(", which is parsable into ");
			Formatter.Format(stringBuilder, _parsedValue);
		}
	}
}
#endif
