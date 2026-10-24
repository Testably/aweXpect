using System;
using aweXpect.Core;
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
			Core.Extending.ExpectationBuilderExtensions.AddStringOccurrenceConstraint(
				subject.Get().ExpectationBuilder, expected, quantifier, options),
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
			Core.Extending.ExpectationBuilderExtensions.AddStringOccurrenceConstraint(
				subject.Get().ExpectationBuilder, unexpected, quantifier, options, isNegated: true),
			subject,
			quantifier,
			options);
	}
}
