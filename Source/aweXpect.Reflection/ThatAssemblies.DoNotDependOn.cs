using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
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
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> DoNotDependOn(
		this IThat<IEnumerable<Assembly?>?> subject, string unexpected)
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IEnumerable<Assembly?>>((it, grammars)
					=> new DoNotDependOnConstraint(it, grammars, unexpected, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     no dependency on the <paramref name="unexpected" /> assembly.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>
		DoNotDependOn(
			this IThat<IAsyncEnumerable<Assembly?>?> subject, string unexpected)
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>(subject
				.Get()
				.ExpectationBuilder.AddConstraint<IAsyncEnumerable<Assembly?>>((it, grammars)
					=> new DoNotDependOnConstraint(it, grammars, unexpected, options)),
			subject,
			options);
	}
#endif

	private sealed class DoNotDependOnConstraint(
		string it,
		ExpectationGrammars grammars,
		string unexpected,
		StringEqualityOptions options)
		: CollectionConstraintResult<Assembly?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, async assembly =>
				assembly == null ||
				!await assembly.GetReferencedAssemblies().AnyAsync(dep =>
					options.AreConsideredEqual(dep.Name, unexpected)), cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context, async assembly =>
				assembly == null ||
				!await assembly.GetReferencedAssemblies().AnyAsync(dep =>
					options.AreConsideredEqual(dep.Name, unexpected)), cancellationToken);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have no dependency on assembly ")
				.Append(options.GetExpectation(unexpected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained assemblies with the unexpected dependency");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have no dependency on assembly ")
				.Append(options.GetExpectation(unexpected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained assemblies without the unexpected dependency");
	}
}
