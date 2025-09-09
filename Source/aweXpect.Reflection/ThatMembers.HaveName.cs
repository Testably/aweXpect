using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMembers
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> have
	///     the <paramref name="expected" /> name.
	/// </summary>
	public static StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> HaveName<TMember>(
		this IThat<IEnumerable<TMember>> subject, string expected)
		where TMember : MemberInfo?
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>>(subject.Get()
				.ExpectationBuilder.AddConstraint((it, grammars)
					=> new HaveNameConstraint<TMember>(it, grammars, expected, options)),
			subject,
			options);
	}

	private sealed class HaveNameConstraint<TMember>(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: ConstraintResult.WithValue<IEnumerable<TMember>>(grammars),
			IAsyncConstraint<IEnumerable<TMember>>
		where TMember : MemberInfo?
	{
		private TMember[] _matching = [];
		private TMember[] _unmatching = [];

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TMember> actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			(_matching, _unmatching) = await actual
				.SplitAsync(memberInfo => options.AreConsideredEqual(memberInfo?.Name, expected));
			Outcome = _unmatching.Length == 0
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching items ");
			Formatter.Format(stringBuilder, _unmatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have name ").Append(options.GetExpectation(expected, Grammars.Negate()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching items ");
			Formatter.Format(stringBuilder, _matching, FormattingOptions.Indented(indentation));
		}
	}
}
