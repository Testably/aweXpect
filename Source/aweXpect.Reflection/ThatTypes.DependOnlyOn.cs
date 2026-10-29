using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) only types in the <paramref name="namespaces" /> (including sub-namespaces), their own namespace
	///     or framework assemblies.
	/// </summary>
	/// <remarks>
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.SettingsCustomization.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so
	///     that framework namespaces do not have to be listed explicitly. The default prefixes include
	///     <c>Microsoft</c>, so e.g. <c>Microsoft.EntityFrameworkCore</c> is also ignored; forbid such a dependency
	///     explicitly via <c>DoNotDependOn</c> or customize the prefixes.
	/// </remarks>
	[GuaranteesNotNull]
	public static NamespaceDependencyOnlyOnResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOnlyOn(
		this IThat<IEnumerable<Type?>?> subject, params string[] namespaces)
		=> subject.DependOnlyOn(namespaces.AsEnumerable());

	/// <inheritdoc cref="DependOnlyOn(IThat{IEnumerable{Type?}}, string[])" />
	[GuaranteesNotNull]
	public static NamespaceDependencyOnlyOnResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOnlyOn(
		this IThat<IEnumerable<Type?>?> subject, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(subject.ExpectAll(new DependOnlyOnExpectation(options)), subject, options);
	}

	/// <summary>
	///     …depend on (reference in their signature) only types in the <paramref name="namespaces" /> (including
	///     sub-namespaces), their own namespace or framework assemblies.
	/// </summary>
	/// <inheritdoc cref="DependOnlyOn(IThat{IEnumerable{Type?}}, string[])" path="/remarks" />
	public static NamespaceDependencyOnlyOnResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOnlyOn(
		this IEnumerableItems<Type?> items, params string[] namespaces)
		=> items.DependOnlyOn(namespaces.AsEnumerable());

	/// <inheritdoc cref="DependOnlyOn(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyOnlyOnResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOnlyOn(
		this IEnumerableItems<Type?> items, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(items.ExpectItems(new DependOnlyOnExpectation(options)), items.Subject, options);
	}

	/// <inheritdoc cref="DependOnlyOn(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyOnlyOnResult<TCollection, TCollection> DependOnlyOn<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items, params string[] namespaces)
		where TCollection : struct, IEnumerable<Type?>
		=> items.DependOnlyOn(namespaces.AsEnumerable());

	/// <inheritdoc cref="DependOnlyOn(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyOnlyOnResult<TCollection, TCollection> DependOnlyOn<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items, IEnumerable<string> namespaces)
		where TCollection : struct, IEnumerable<Type?>
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(items.ExpectItems(new DependOnlyOnExpectation(options)), items.Subject, options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DependOnlyOn(IThat{IEnumerable{Type?}}, string[])" />
	[GuaranteesNotNull]
	public static NamespaceDependencyOnlyOnResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOnlyOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, params string[] namespaces)
		=> subject.DependOnlyOn(namespaces.AsEnumerable());

	/// <inheritdoc cref="DependOnlyOn(IThat{IEnumerable{Type?}}, string[])" />
	[GuaranteesNotNull]
	public static NamespaceDependencyOnlyOnResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOnlyOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(subject.ExpectAll(new DependOnlyOnExpectation(options)), subject, options);
	}

	/// <inheritdoc cref="DependOnlyOn(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyOnlyOnResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOnlyOn(
		this IAsyncEnumerableItems<Type?> items, params string[] namespaces)
		=> items.DependOnlyOn(namespaces.AsEnumerable());

	/// <inheritdoc cref="DependOnlyOn(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyOnlyOnResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOnlyOn(
		this IAsyncEnumerableItems<Type?> items, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(items.ExpectItems(new DependOnlyOnExpectation(options)), items.Subject, options);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) only types in the filtered collections of types <paramref name="target" /> or
	///     <paramref name="additional" />, their own namespace or framework assemblies.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency is allowed when it is a member of the
	///     union of the resolved collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it). A type's own namespace is always allowed, including its
	///     sub-namespaces unless
	///     <see cref="TypeSetDependencyOnlyOnResult{TType, TValue}.ExcludingOwnSubNamespaces" /> is used.
	///     <para />
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.SettingsCustomization.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so
	///     that framework types do not have to be included explicitly. The default prefixes include
	///     <c>Microsoft</c>, so e.g. <c>Microsoft.EntityFrameworkCore</c> is also ignored; forbid such a dependency
	///     explicitly via <c>DoNotDependOn</c> or customize the prefixes.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeSetDependencyOnlyOnResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOnlyOn(
		this IThat<IEnumerable<Type?>?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(subject.ExpectAll(new DependOnlyOnTypeSetExpectation(options)), subject, options);
	}

	/// <summary>
	///     …depend on (reference in their signature) only types in the filtered collections of types
	///     <paramref name="target" /> or <paramref name="additional" />, their own namespace or framework assemblies.
	/// </summary>
	/// <inheritdoc cref="DependOnlyOn(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" path="/remarks" />
	public static TypeSetDependencyOnlyOnResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOnlyOn(
		this IEnumerableItems<Type?> items, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(items.ExpectItems(new DependOnlyOnTypeSetExpectation(options)), items.Subject, options);
	}

	/// <inheritdoc cref="DependOnlyOn(IEnumerableItems{Type?}, Filtered.Types, Filtered.Types[])" />
	public static TypeSetDependencyOnlyOnResult<TCollection, TCollection> DependOnlyOn<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items, Filtered.Types target,
		params Filtered.Types[] additional)
		where TCollection : struct, IEnumerable<Type?>
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(items.ExpectItems(new DependOnlyOnTypeSetExpectation(options)), items.Subject, options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DependOnlyOn(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" />
	[GuaranteesNotNull]
	public static TypeSetDependencyOnlyOnResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOnlyOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(subject.ExpectAll(new DependOnlyOnTypeSetExpectation(options)), subject, options);
	}

	/// <inheritdoc cref="DependOnlyOn(IEnumerableItems{Type?}, Filtered.Types, Filtered.Types[])" />
	public static TypeSetDependencyOnlyOnResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOnlyOn(
		this IAsyncEnumerableItems<Type?> items, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(items.ExpectItems(new DependOnlyOnTypeSetExpectation(options)), items.Subject, options);
	}
#endif

	/// <summary>
	///     The expectation on the dependencies of a type outside an allowed set, which lists the dependencies outside
	///     the set of each listed type.
	/// </summary>
	/// <param name="isMetWithViolations">
	///     Whether a type with dependencies outside the allowed set meets the expectation, instead of one without.
	/// </param>
	private abstract class DependencyViolationsExpectation(bool isMetWithViolations) : ItemExpectation<Type>
	{
		private readonly Dictionary<Type, IReadOnlyList<string>> _violations = new();

		protected abstract ValueTask<IReadOnlyList<string>> GetViolations(Type item,
			CancellationToken cancellationToken);

		public override async ValueTask<PredicateResult> IsMatch(Type item, CancellationToken cancellationToken)
		{
			IReadOnlyList<string> violations = await GetViolations(item, cancellationToken);
			if (violations.Count > 0)
			{
				_violations[item] = violations;
			}

			return violations.Count > 0 == isMetWithViolations;
		}

		public override string FormatItems(IEnumerable<Type?> items, FormattingOptions options)
			=> DependencyViolationRenderer.FormatItemsWithDisallowedDependencies(items.ToList(), _violations,
				options.TotalItemCount);
	}

	private sealed class DependOnlyOnExpectation(IReadOnlyOptionsHolder<NamespaceDependencyOptions> options)
		: DependencyViolationsExpectation(false)
	{
		protected override ValueTask<IReadOnlyList<string>> GetViolations(Type item,
			CancellationToken cancellationToken)
			=> new(item.GetDependencyNamespaceViolations(options.Value));

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
			=> stringBuilder.Append(grammars.IsPlural() ? "depend only on " : "depends only on ")
				.Append(options.Value.Describe());
	}

	private sealed class DependOnlyOnTypeSetExpectation(IReadOnlyOptionsHolder<TypeSetDependencyOptions> options)
		: DependencyViolationsExpectation(false)
	{
		protected override async ValueTask<IReadOnlyList<string>> GetViolations(Type item,
			CancellationToken cancellationToken)
			=> item.GetDependencyTypeSetViolations(await options.Value.Resolve(cancellationToken));

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
			=> stringBuilder.Append(grammars.IsPlural() ? "depend only on " : "depends only on ")
				.Append(options.Value.Describe());
	}
}
