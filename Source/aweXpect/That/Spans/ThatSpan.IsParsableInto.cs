#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatSpan
{
	/// <summary>
	///     Verifies that the subject is parsable into type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="formatProvider" /> provides culture-specific formatting information
	///     in the call to <see cref="IParsable{TType}.Parse(string, IFormatProvider)" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static IsSpanParsableResult<TType> IsParsableInto<TType>(
		this IThat<SpanWrapper<char>> subject,
		IFormatProvider? formatProvider = null)
		where TType : ISpanParsable<TType>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsParsableIntoConstraint<TType>(it, grammars, formatProvider)),
			subject,
			formatProvider);

	/// <summary>
	///     Verifies that the subject is parsable into type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="formatProvider" /> provides culture-specific formatting information
	///     in the call to <see cref="IParsable{TType}.Parse(string, IFormatProvider)" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static IsUtf8SpanParsableResult<TType> IsParsableInto<TType>(
		this IThat<SpanWrapper<byte>> subject,
		IFormatProvider? formatProvider = null)
		where TType : IUtf8SpanParsable<TType>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsUtf8ParsableIntoConstraint<TType>(it, grammars, formatProvider)),
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
	public static AndOrResult<SpanWrapper<char>, IThat<SpanWrapper<char>>> IsNotParsableInto<TType>(
		this IThat<SpanWrapper<char>> subject,
		IFormatProvider? formatProvider = null)
		where TType : ISpanParsable<TType>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsParsableIntoConstraint<TType>(it, grammars, formatProvider).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not parsable into type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="formatProvider" /> provides culture-specific formatting information
	///     in the call to <see cref="IParsable{TType}.Parse(string, IFormatProvider)" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<SpanWrapper<byte>, IThat<SpanWrapper<byte>>> IsNotParsableInto<TType>(
		this IThat<SpanWrapper<byte>> subject,
		IFormatProvider? formatProvider = null)
		where TType : IUtf8SpanParsable<TType>
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsUtf8ParsableIntoConstraint<TType>(it, grammars, formatProvider).Invert()),
			subject);

	private sealed class IsParsableIntoConstraint<TType> : ConstraintResult.WithNotNullValue<SpanWrapper<char>>,
		IValueConstraint<SpanWrapper<char>>
		where TType : ISpanParsable<TType>
	{
		private readonly IFormatProvider? _formatProvider;
		private string? _exceptionMessage;
		private TType? _parsedValue;

		public IsParsableIntoConstraint(string it,
			ExpectationGrammars grammars,
			IFormatProvider? formatProvider) : base(it, grammars)
		{
			_formatProvider = formatProvider;
			FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
		}

		public ConstraintResult IsMetBy(SpanWrapper<char> actual)
		{
			Actual = actual;

			try
			{
				_parsedValue = TType.Parse(actual.AsSpan(), _formatProvider);
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
			stringBuilder.Append(Grammars.Verb("is parsable into ", "are parsable into "));
			Formatter.Format(stringBuilder, typeof(TType));
			if (_formatProvider is not null)
			{
				stringBuilder.Append(" using ");
				Formatter.Format(stringBuilder, _formatProvider);
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was not, because ").Append(_exceptionMessage);

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
			Formatter.Format(stringBuilder, new string(Actual!.AsSpan()));
			stringBuilder.Append(", which is parsable into ");
			Formatter.Format(stringBuilder, _parsedValue);
		}
	}

	private sealed class IsUtf8ParsableIntoConstraint<TType> : ConstraintResult.WithNotNullValue<SpanWrapper<byte>>,
		IValueConstraint<SpanWrapper<byte>>
		where TType : IUtf8SpanParsable<TType>
	{
		private readonly IFormatProvider? _formatProvider;
		private string? _exceptionMessage;
		private TType? _parsedValue;

		public IsUtf8ParsableIntoConstraint(string it,
			ExpectationGrammars grammars,
			IFormatProvider? formatProvider) : base(it, grammars)
		{
			_formatProvider = formatProvider;
			FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
		}

		public ConstraintResult IsMetBy(SpanWrapper<byte> actual)
		{
			Actual = actual;

			try
			{
				_parsedValue = TType.Parse(actual.AsSpan(), _formatProvider);
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

				// Older runtimes name the input "System.ReadOnlySpan<Byte>[length]" in the message instead of its text.
				if (actual is not null)
				{
					ReadOnlySpan<byte> input = actual.AsSpan();
					_exceptionMessage = _exceptionMessage.Replace(input.ToString(), Encoding.UTF8.GetString(input));
				}

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
			=> stringBuilder.Append(It).Append(" was not, because ").Append(_exceptionMessage);

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
			Formatter.Format(stringBuilder, Encoding.UTF8.GetString(Actual!.AsSpan()));
			stringBuilder.Append(", which is parsable into ");
			Formatter.Format(stringBuilder, _parsedValue);
		}
	}
}
#endif
