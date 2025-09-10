using System.Collections.Generic;
using System.Linq;
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
	///     no dependency on the <paramref name="unexpected" /> assembly.
	/// </summary>
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>>> HaveNoDependencyOn(
		this IThat<IEnumerable<Assembly?>> subject, string unexpected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IEnumerable<Assembly?>>((it, grammars)
					=> new HaveNoDependencyOnConstraint(it, grammars, unexpected, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     no dependency on the <paramref name="unexpected" /> assembly.
	/// </summary>
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>>>
		HaveNoDependencyOn(
			this IThat<IAsyncEnumerable<Assembly?>> subject, string unexpected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>>>(subject
				.Get()
				.ExpectationBuilder.AddConstraint<IAsyncEnumerable<Assembly?>>((it, grammars)
					=> new HaveNoDependencyOnConstraint(it, grammars, unexpected, options)),
			subject,
			options);
	}
#endif

	private sealed class HaveNoDependencyOnConstraint(
		string it,
		ExpectationGrammars grammars,
		string unexpected,
		StringEqualityOptions options)
		: CollectionConstraintResult<Assembly?>(grammars),
			IAsyncConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, async assembly =>
				assembly == null ||
				!await assembly.GetReferencedAssemblies().AnyAsync(dep =>
					options.AreConsideredEqual(dep.Name, unexpected)));
#endif

		public async Task<ConstraintResult> IsMetBy(IEnumerable<Assembly?> actual, CancellationToken cancellationToken)
			=> await SetValue(actual, async assembly =>
				assembly == null ||
				!await assembly.GetReferencedAssemblies().AnyAsync(dep =>
					options.AreConsideredEqual(dep.Name, unexpected)));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have no dependency on assembly ")
				.Append(options.GetExpectation(unexpected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained assemblies with the unexpected dependency ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have no dependency on assembly ")
				.Append(options.GetExpectation(unexpected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained assemblies without the unexpected dependency ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
