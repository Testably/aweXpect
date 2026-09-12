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
		this IThat<SpanWrapper<char>> source,
		IFormatProvider? formatProvider = null)
		where TType : ISpanParsable<TType>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsParsableIntoConstraint<TType>(it, grammars, formatProvider)),
			source,
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
		this IThat<SpanWrapper<byte>> source,
		IFormatProvider? formatProvider = null)
		where TType : IUtf8SpanParsable<TType>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsUtf8ParsableIntoConstraint<TType>(it, grammars, formatProvider)),
			source,
			formatProvider);

	/// <summary>
	///     Verifies that the subject is not parsable into type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="formatProvider" /> provides culture-specific formatting information
	///     in the call to <see cref="IParsable{TType}.Parse(string, IFormatProvider)" />.
	/// </remarks>
	public static AndOrResult<SpanWrapper<char>, IThat<SpanWrapper<char>>> IsNotParsableInto<TType>(
		this IThat<SpanWrapper<char>> source,
		IFormatProvider? formatProvider = null)
		where TType : ISpanParsable<TType>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsParsableIntoConstraint<TType>(it, grammars, formatProvider).Invert()),
			source);

	/// <summary>
	///     Verifies that the subject is not parsable into type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="formatProvider" /> provides culture-specific formatting information
	///     in the call to <see cref="IParsable{TType}.Parse(string, IFormatProvider)" />.
	/// </remarks>
	public static AndOrResult<SpanWrapper<byte>, IThat<SpanWrapper<byte>>> IsNotParsableInto<TType>(
		this IThat<SpanWrapper<byte>> source,
		IFormatProvider? formatProvider = null)
		where TType : IUtf8SpanParsable<TType>
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsUtf8ParsableIntoConstraint<TType>(it, grammars, formatProvider).Invert()),
			source);

	private sealed class IsParsableIntoConstraint<TType> : ConstraintResult.WithValue<SpanWrapper<char>>,
		IValueConstraint<SpanWrapper<char>>
		where TType : ISpanParsable<TType>
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

		public ConstraintResult IsMetBy(SpanWrapper<char> actual)
		{
			Actual = actual;

			try
			{
				_ = TType.Parse(actual.AsSpan(), _formatProvider);
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
			=> stringBuilder.Append(_it).Append(" was not because ").Append(_exceptionMessage);

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

	private sealed class IsUtf8ParsableIntoConstraint<TType> : ConstraintResult.WithValue<SpanWrapper<byte>>,
		IValueConstraint<SpanWrapper<byte>>
		where TType : IUtf8SpanParsable<TType>
	{
		private readonly IFormatProvider? _formatProvider;
		private readonly string _it;
		private string? _exceptionMessage;

		public IsUtf8ParsableIntoConstraint(string it,
			ExpectationGrammars grammars,
			IFormatProvider? formatProvider) : base(grammars)
		{
			_it = it;
			_formatProvider = formatProvider;
			FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
		}

		public ConstraintResult IsMetBy(SpanWrapper<byte> actual)
		{
			Actual = actual;

			try
			{
				_ = TType.Parse(actual.AsSpan(), _formatProvider);
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
			=> stringBuilder.Append(_it).Append(" was not because ").Append(_exceptionMessage);

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
