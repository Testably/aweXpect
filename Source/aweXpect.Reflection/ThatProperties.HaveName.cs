using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Reflection.Extensions;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have
	///     the <paramref name="expected" /> name.
	/// </summary>
	public static StringEqualityTypeResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> HaveName(
		this IThat<IEnumerable<PropertyInfo?>> subject, string expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>>(subject.Get()
				.ExpectationBuilder.AddConstraint((it, grammars)
					=> new HaveNameConstraint(it, grammars, expected, options)),
			subject,
			options);
	}

	private sealed class HaveNameConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: ConstraintResult.WithValue<IEnumerable<PropertyInfo?>>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => options.AreConsideredEqual(type?.Name, expected))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => !options.AreConsideredEqual(type?.Name, expected)),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => options.AreConsideredEqual(type?.Name, expected)),
				FormattingOptions.Indented(indentation));
		}
	}
}
