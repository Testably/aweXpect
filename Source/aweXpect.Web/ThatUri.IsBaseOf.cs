using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	/// <summary>
	///     Verifies that the subject is a base URI of the <paramref name="expected" /> URI.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsBaseOf(Uri)" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsBaseOf(this IThat<Uri?> subject, Uri expected)
	{
		ThrowIfNotAbsolute(expected, nameof(expected));
		return new(subject.FailIfRelative().Get().ExpectationBuilder.AddConstraint(expected, static (expected, it, grammars) =>
				new IsBaseOfConstraint(it, grammars, expected)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is a base URI of the <paramref name="expected" /> URI.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsBaseOf(Uri)" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsBaseOf(this IThat<Uri?> subject, string expected)
		=> subject.IsBaseOf(ToAbsoluteUri(expected, nameof(expected)));

	/// <summary>
	///     Verifies that the subject is not a base URI of the <paramref name="unexpected" /> URI.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsBaseOf(Uri)" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsNotBaseOf(this IThat<Uri?> subject, Uri unexpected)
	{
		ThrowIfNotAbsolute(unexpected, nameof(unexpected));
		return new(subject.FailIfRelative().Get().ExpectationBuilder.AddConstraint(unexpected, static (unexpected, it, grammars) =>
				new IsBaseOfConstraint(it, grammars, unexpected).Invert()),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is not a base URI of the <paramref name="unexpected" /> URI.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsBaseOf(Uri)" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsNotBaseOf(this IThat<Uri?> subject, string unexpected)
		=> subject.IsNotBaseOf(ToAbsoluteUri(unexpected, nameof(unexpected)));

	private static Uri ToAbsoluteUri(string value, string paramName)
	{
		ThrowHelper.ThrowIfNull(value, paramName);
		if (!Uri.TryCreate(value, UriKind.Absolute, out Uri? uri))
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The '{paramName}' must be a valid absolute URI.", paramName));
		}

		return uri;
	}

	private sealed class IsBaseOfConstraint(string it, ExpectationGrammars grammars, Uri expected)
		: ConstraintResult.WithNotNullValue<Uri>(it, grammars),
			IValueConstraint<Uri?>
	{
		public ConstraintResult IsMetBy(Uri? actual)
		{
			Actual = actual;
			Outcome = actual?.IsBaseOf(expected) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is a base URI of ", "are base URIs of "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not a base URI of ", "are not base URIs of "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
