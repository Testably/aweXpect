using System.Collections.Generic;
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
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> target
	///     the <paramref name="expected" /> framework (e.g. <c>net8.0</c>).
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> Target(
		this IThat<IEnumerable<Assembly?>?> subject, string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IEnumerable<Assembly?>>((it, grammars)
					=> new TargetConstraint(it, grammars, expected, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> target
	///     the <paramref name="expected" /> framework (e.g. <c>net8.0</c>).
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>> Target(
		this IThat<IAsyncEnumerable<Assembly?>?> subject, string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>(subject
				.Get()
				.ExpectationBuilder.AddConstraint<IAsyncEnumerable<Assembly?>>((it, grammars)
					=> new TargetConstraint(it, grammars, expected, options)),
			subject,
			options);
	}
#endif

	private sealed class TargetConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
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
			=> await SetAsyncValue(actual, context,
				assembly => options.AreConsideredEqual(assembly?.GetTargetFramework(), expected), cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context,
				assembly => options.AreConsideredEqual(assembly?.GetTargetFramework(), expected), cancellationToken);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all target ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching assemblies");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all target ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching assemblies");
	}
}
