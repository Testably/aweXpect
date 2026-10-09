using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Customization;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatAssemblies
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have dependencies only on
	///     the <paramref name="allowed" /> assemblies.
	/// </summary>
	/// <remarks>
	///     References to assemblies whose name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomization.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored,
	///     so that framework assemblies do not have to be listed explicitly.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> DependOnlyOn(
		this IThat<IEnumerable<Assembly?>?> subject, params string[] allowed)
	{
		StringEqualityOptions options = new(nameof(allowed));
		return new StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IEnumerable<Assembly?>>((it, grammars)
					=> new DependOnlyOnConstraint(it, grammars, allowed, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have dependencies only on
	///     the <paramref name="allowed" /> assemblies.
	/// </summary>
	/// <remarks>
	///     References to assemblies whose name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomization.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored,
	///     so that framework assemblies do not have to be listed explicitly.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>
		DependOnlyOn(
			this IThat<IAsyncEnumerable<Assembly?>?> subject, params string[] allowed)
	{
		StringEqualityOptions options = new(nameof(allowed));
		return new StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>(subject
				.Get()
				.ExpectationBuilder.AddConstraint<IAsyncEnumerable<Assembly?>>((it, grammars)
					=> new DependOnlyOnConstraint(it, grammars, allowed, options)),
			subject,
			options);
	}
#endif

	private sealed class DependOnlyOnConstraint(
		string it,
		ExpectationGrammars grammars,
		string[] allowed,
		StringEqualityOptions options)
		: CollectionConstraintResult<Assembly?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
		private readonly Dictionary<Assembly, string?[]> _disallowedDependencies = new();

#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, DependsOnlyOnAllowed, cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context, DependsOnlyOnAllowed, cancellationToken);

		private async ValueTask<bool> DependsOnlyOnAllowed(Assembly? assembly)
		{
			if (assembly is null)
			{
				return false;
			}

			string?[] violations = await assembly.GetDisallowedAssemblyDependencies(allowed, options);
			if (violations.Length > 0)
			{
				_disallowedDependencies[assembly] = violations;
			}

			return violations.Length == 0;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have dependencies only on ").Append(DescribeAllowed());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained assemblies with disallowed dependencies");

		protected override Func<string?> FormatItems(Assembly?[] items)
			=> () => DependencyViolationRenderer.FormatItemsWithDisallowedDependencies(items, _disallowedDependencies);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have dependencies only on ").Append(DescribeAllowed());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained assemblies depending only on the allowed assemblies");

		private string DescribeAllowed()
			=> allowed.Length == 0
				? "no assemblies"
				: $"assemblies {string.Join(" or ", allowed.Select(expected => options.GetExpectation(expected, ExpectationGrammars.None)))}";
	}
}
