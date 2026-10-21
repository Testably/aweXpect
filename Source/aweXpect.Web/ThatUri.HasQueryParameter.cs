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
	public static HasQueryParameterValueResult HasQueryParameter(this IThat<Uri?> subject, string name)
	{
		ThrowHelper.ThrowIfNullOrWhiteSpace(name, nameof(name));
		// The value can be continued with `WhoseValue`, whose result on the member does not stop the further
		// processing for a relative URI, so the check stays a separate operand here.
		return new(subject.FailIfRelative().Get().ExpectationBuilder
				.AddConstraint(name, static (name, it, grammars) =>
					new HasQueryParameterConstraint(it, grammars, name)),
			subject,
			name);
	}

	/// <summary>
	///     Verifies that the subject does not have a query parameter with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The name is compared case-sensitively after URL-decoding.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> DoesNotHaveQueryParameter(this IThat<Uri?> subject, string name)
	{
		ThrowHelper.ThrowIfNullOrWhiteSpace(name, nameof(name));
		return new(subject.Get().ExpectationBuilder
				.AddConstraint(name, static (name, it, grammars) =>
					new HasQueryParameterConstraint(it, grammars, name).Invert()),
			subject);
	}

	private sealed class HasQueryParameterConstraint(string it, ExpectationGrammars grammars, string name)
		: AbsoluteUriConstraint(it, grammars)
	{
		protected override bool IsSatisfiedBy(Uri? actual)
		{
			if (actual?.HasQueryParameter(name) == true)
			{
				return true;
			}

			FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
			return false;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has a query parameter ", "have a query parameter "));
			Formatter.Format(stringBuilder, name);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have a query parameter ", "do not have a query parameter "));
			Formatter.Format(stringBuilder, name);
		}
	}
}
