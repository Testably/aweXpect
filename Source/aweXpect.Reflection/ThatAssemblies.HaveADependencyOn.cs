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

public static partial class ThatAssemblies
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     a dependency on the <paramref name="expected" /> assembly.
	/// </summary>
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>>> HaveADependencyOn(
		this IThat<IEnumerable<Assembly?>> subject, string expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>>>(subject.Get()
				.ExpectationBuilder.AddConstraint((it, grammars)
					=> new HaveADependencyOnConstraint(it, grammars, expected, options)),
			subject,
			options);
	}

	private sealed class HaveADependencyOnConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: ConstraintResult.WithValue<IEnumerable<Assembly?>>(grammars),
			IAsyncConstraint<IEnumerable<Assembly?>>
	{
		private Assembly?[] _matching = [];
		private Assembly?[] _unmatching = [];

		public async Task<ConstraintResult> IsMetBy(IEnumerable<Assembly?> actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			(_matching, _unmatching) = await actual
				.SplitWhereAnyAsync(assembly =>
					assembly?.GetReferencedAssemblies(), dep => options.AreConsideredEqual(dep.Name, expected));
			Outcome = _unmatching.Length == 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have dependency on assembly ")
				.Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained assemblies without the required dependency ");
			Formatter.Format(stringBuilder, _unmatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have dependency on assembly ")
				.Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained assemblies with the unexpected dependency ");
			Formatter.Format(stringBuilder, _matching, FormattingOptions.Indented(indentation));
		}
	}
}
