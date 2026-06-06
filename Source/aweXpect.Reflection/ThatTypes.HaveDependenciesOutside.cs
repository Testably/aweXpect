using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have at least one dependency
	///     (a type referenced in their signature) outside the allowed <paramref name="namespaces" /> (including
	///     sub-namespaces), their own namespace and framework assemblies — the positive counterpart of
	///     <see cref="DependOnlyOn(IThat{IEnumerable{Type?}}, IEnumerable{string})" /> for finding violators of an
	///     allowed set.
	/// </summary>
	/// <remarks>
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so that framework dependencies never count as outside the
	///     allowed set. The default prefixes include <c>Microsoft</c>, so e.g. a dependency on
	///     <c>Microsoft.EntityFrameworkCore</c> is also ignored; customize the prefixes to make such a dependency
	///     count.
	/// </remarks>
	public static NamespaceDependencyOutsideResult<IEnumerable<Type?>> HaveDependenciesOutside(
		this IThat<IEnumerable<Type?>> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyOutsideResult<IEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IEnumerable<Type?>>((it, grammars)
					=> new HaveDependenciesOutsideConstraint(it, grammars, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have at least one dependency
	///     (a type referenced in their signature) outside the allowed <paramref name="namespaces" /> (including
	///     sub-namespaces), their own namespace and framework assemblies — the positive counterpart of
	///     <see cref="DependOnlyOn(IThat{IAsyncEnumerable{Type?}}, IEnumerable{string})" /> for finding violators
	///     of an allowed set.
	/// </summary>
	/// <remarks>
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so that framework dependencies never count as outside the
	///     allowed set. The default prefixes include <c>Microsoft</c>, so e.g. a dependency on
	///     <c>Microsoft.EntityFrameworkCore</c> is also ignored; customize the prefixes to make such a dependency
	///     count.
	/// </remarks>
	public static NamespaceDependencyOutsideResult<IAsyncEnumerable<Type?>> HaveDependenciesOutside(
		this IThat<IAsyncEnumerable<Type?>> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyOutsideResult<IAsyncEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
					=> new HaveDependenciesOutsideConstraint(it, grammars, options)),
			subject,
			options);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have at least one dependency
	///     (a type referenced in their signature) outside the allowed set formed by the filtered collections of
	///     types <paramref name="target" /> and <paramref name="additional" />, their own namespace and framework
	///     assemblies — the positive counterpart of
	///     <see cref="DependOnlyOn(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" /> for finding
	///     violators of an allowed set.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency is inside the allowed set when it
	///     is a member of the union of the resolved collections (by <see cref="Type" /> identity; a generic type
	///     definition in a collection matches any construction of it). A type's own namespace never counts as
	///     outside, including its sub-namespaces unless
	///     <see cref="TypeSetDependencyOutsideResult{TThat}.ExcludingOwnSubNamespaces" /> is used.
	///     <para />
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so that framework dependencies never count as outside the
	///     allowed set. The default prefixes include <c>Microsoft</c>, so e.g. a dependency on
	///     <c>Microsoft.EntityFrameworkCore</c> is also ignored; customize the prefixes to make such a dependency
	///     count.
	/// </remarks>
	public static TypeSetDependencyOutsideResult<IEnumerable<Type?>> HaveDependenciesOutside(
		this IThat<IEnumerable<Type?>> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions options = new(target, additional);
		return new TypeSetDependencyOutsideResult<IEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IEnumerable<Type?>>((it, grammars)
					=> new HaveDependenciesOutsideTypeSetConstraint(it, grammars, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have at least one dependency
	///     (a type referenced in their signature) outside the allowed set formed by the filtered collections of
	///     types <paramref name="target" /> and <paramref name="additional" />, their own namespace and framework
	///     assemblies — the positive counterpart of
	///     <see cref="DependOnlyOn(IThat{IAsyncEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" /> for
	///     finding violators of an allowed set.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency is inside the allowed set when it
	///     is a member of the union of the resolved collections (by <see cref="Type" /> identity; a generic type
	///     definition in a collection matches any construction of it). A type's own namespace never counts as
	///     outside, including its sub-namespaces unless
	///     <see cref="TypeSetDependencyOutsideResult{TThat}.ExcludingOwnSubNamespaces" /> is used.
	///     <para />
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so that framework dependencies never count as outside the
	///     allowed set. The default prefixes include <c>Microsoft</c>, so e.g. a dependency on
	///     <c>Microsoft.EntityFrameworkCore</c> is also ignored; customize the prefixes to make such a dependency
	///     count.
	/// </remarks>
	public static TypeSetDependencyOutsideResult<IAsyncEnumerable<Type?>> HaveDependenciesOutside(
		this IThat<IAsyncEnumerable<Type?>> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions options = new(target, additional);
		return new TypeSetDependencyOutsideResult<IAsyncEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
					=> new HaveDependenciesOutsideTypeSetConstraint(it, grammars, options)),
			subject,
			options);
	}
#endif

	private sealed class HaveDependenciesOutsideConstraint(
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

		private bool HasDependencyOutsideAllowed(Type? type)
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

			return violations.Count > 0;
		}

#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, HasDependencyOutsideAllowed);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, HasDependencyOutsideAllowed);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have dependencies outside ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained types depending only on the allowed namespaces ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have dependencies outside ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> DependencyViolationRenderer.AppendItemsWithDisallowedDependencies(stringBuilder, it,
				" only contained types with dependencies outside the allowed namespaces ", Matching, _violations,
				indentation);
	}

	private sealed class HaveDependenciesOutsideTypeSetConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeSetDependencyOptions options)
		: CollectionConstraintResult<Type?>(grammars),
			IAsyncConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly Dictionary<Type, IReadOnlyList<string>> _violations = new();

		private bool HasDependencyOutsideAllowed(Type? type, ResolvedTypeSet allowed)
		{
			if (type is null)
			{
				return false;
			}

			IReadOnlyList<string> violations = type.GetDependencyTypeSetViolations(allowed);
			if (violations.Count > 0)
			{
				_violations[type] = violations;
			}

			return violations.Count > 0;
		}

#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
		{
			ResolvedTypeSet allowed = await options.Resolve(cancellationToken);
			return await SetAsyncValue(actual, type => HasDependencyOutsideAllowed(type, allowed));
		}
#endif

		public async Task<ConstraintResult> IsMetBy(IEnumerable<Type?> actual, CancellationToken cancellationToken)
		{
			ResolvedTypeSet allowed = await options.Resolve(cancellationToken);
			return SetValue(actual, type => HasDependencyOutsideAllowed(type, allowed));
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have dependencies outside ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained types depending only on the allowed types ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have dependencies outside ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> DependencyViolationRenderer.AppendItemsWithDisallowedDependencies(stringBuilder, it,
				" only contained types with dependencies outside the allowed types ", Matching, _violations,
				indentation);
	}
}
