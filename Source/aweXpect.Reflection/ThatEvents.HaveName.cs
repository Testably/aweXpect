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

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> have
	///     the <paramref name="expected" /> name.
	/// </summary>
	public static StringEqualityTypeResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> HaveName(
		this IThat<IEnumerable<EventInfo?>> subject, string expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>>(subject.Get()
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
		: ConstraintResult.WithValue<IEnumerable<EventInfo?>>(grammars),
			IValueConstraint<IEnumerable<EventInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual)
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
