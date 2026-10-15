using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.Web.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	/// <summary>
	///     Verifies that the subject has a query parameter with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The name is compared case-sensitively after URL-decoding.
	/// </remarks>
	[GuaranteesNotNull]
	public static HasQueryParameterValueResult HasQueryParameter(this IThat<Uri?> source, string name)
	{
		ThrowHelper.ThrowIfNullOrWhiteSpace(name, nameof(name));
		return new(source.FailIfRelative().Get().ExpectationBuilder
				.AddConstraint(name, static (name, it, grammars) =>
					new HasQueryParameterConstraint(it, grammars, name)),
			source,
			name);
	}

	/// <summary>
	///     Verifies that the subject does not have a query parameter with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The name is compared case-sensitively after URL-decoding.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> DoesNotHaveQueryParameter(this IThat<Uri?> source, string name)
	{
		ThrowHelper.ThrowIfNullOrWhiteSpace(name, nameof(name));
		return new(source.FailIfRelative().Get().ExpectationBuilder
				.AddConstraint(name, static (name, it, grammars) =>
					new HasQueryParameterConstraint(it, grammars, name).Invert()),
			source);
	}

	private sealed class HasQueryParameterConstraint(string it, ExpectationGrammars grammars, string name)
		: ConstraintResult.WithNotNullValue<Uri>(it, grammars),
			IValueConstraint<Uri?>
	{
		public ConstraintResult IsMetBy(Uri? actual)
		{
			Actual = actual;
			if (actual is null)
			{
				FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
				Outcome = Outcome.Failure;
				return this;
			}

			if (actual.GetQueryParameterValues(name) is not null)
			{
				Outcome = Outcome.Success;
				return this;
			}

			FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has a query parameter ", "have a query parameter "));
			Formatter.Format(stringBuilder, name);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have a query parameter ", "do not have a query parameter "));
			Formatter.Format(stringBuilder, name);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
