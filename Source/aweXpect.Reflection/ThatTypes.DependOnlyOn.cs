using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) only types in the <paramref name="namespaces" /> (including sub-namespaces), their own namespace
	///     or framework assemblies.
	/// </summary>
	/// <remarks>
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so
	///     that framework namespaces do not have to be listed explicitly. The default prefixes include
	///     <c>Microsoft</c>, so e.g. <c>Microsoft.EntityFrameworkCore</c> is also ignored; forbid such a dependency
	///     explicitly via <c>DoNotDependOn</c> or customize the prefixes.
	/// </remarks>
	public static NamespaceDependencyOnlyOnResult<IEnumerable<Type?>> DependOnlyOn(
		this IThat<IEnumerable<Type?>> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyOnlyOnResult<IEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IEnumerable<Type?>>((it, grammars)
					=> new DependOnlyOnConstraint(it, grammars, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) only types in the <paramref name="namespaces" /> (including sub-namespaces), their own namespace
	///     or framework assemblies.
	/// </summary>
	/// <remarks>
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so
	///     that framework namespaces do not have to be listed explicitly. The default prefixes include
	///     <c>Microsoft</c>, so e.g. <c>Microsoft.EntityFrameworkCore</c> is also ignored; forbid such a dependency
	///     explicitly via <c>DoNotDependOn</c> or customize the prefixes.
	/// </remarks>
	public static NamespaceDependencyOnlyOnResult<IAsyncEnumerable<Type?>> DependOnlyOn(
		this IThat<IAsyncEnumerable<Type?>> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyOnlyOnResult<IAsyncEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
					=> new DependOnlyOnConstraint(it, grammars, options)),
			subject,
			options);
	}
#endif

	private sealed class DependOnlyOnConstraint(
		string it,
		ExpectationGrammars grammars,
		NamespaceDependencyOptions options)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly Dictionary<Type, IReadOnlyList<string>> _violations = new();

		private bool DependsOnlyOnAllowed(Type? type)
		{
			if (type is null)
			{
				return false;
			}

			IReadOnlyList<string> violations = type.GetDependencyNamespaceViolations(options);
			if (violations.Count > 0)
			{
				_violations[type] = violations;
			}

			return violations.Count == 0;
		}

#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, DependsOnlyOnAllowed);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, DependsOnlyOnAllowed);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all depend only on ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> DependencyViolationRenderer.AppendItemsWithDisallowedDependencies(stringBuilder, it,
				" contained types with disallowed dependencies ", NotMatching, _violations, indentation);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all depend only on ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained types depending only on the allowed namespaces ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
