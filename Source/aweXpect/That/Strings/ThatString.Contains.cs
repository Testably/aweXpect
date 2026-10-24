using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the subject contains the <paramref name="expected" /> <see langword="string" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringOccurrenceCountResult<string, IThat<string?>> Contains(
		this IThat<string?> subject,
		string expected)
	{
		expected.ThrowIfNull();
		if (expected == string.Empty)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The 'expected' string cannot be empty.", nameof(expected)));
		}

		Quantifier quantifier = new();
		StringEqualityOptions options = new(nameof(expected));
		return new StringOccurrenceCountResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Quantifier: quantifier, Options: options),
				static (state, it, grammars) =>
					new StringOccurrenceConstraint(it, grammars, state.Expected, state.Quantifier, state.Options)),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the subject does not contain the <paramref name="unexpected" /> <see langword="string" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringOccurrenceCountResult<string, IThat<string?>> DoesNotContain(
		this IThat<string?> subject,
		string unexpected)
	{
		unexpected.ThrowIfNull();
		if (unexpected == string.Empty)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The 'unexpected' string cannot be empty.", nameof(unexpected)));
		}

		Quantifier quantifier = new();
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringOccurrenceCountResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Unexpected: unexpected, Quantifier: quantifier, Options: options),
				static (state, it, grammars) =>
					new StringOccurrenceConstraint(it, grammars, state.Unexpected, state.Quantifier, state.Options)
						.Invert()),
			subject,
			quantifier,
			options);
	}
}
