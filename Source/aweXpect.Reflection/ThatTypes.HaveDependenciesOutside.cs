using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have at least one dependency
	///     (a type referenced in their signature) outside the allowed <paramref name="namespaces" /> (including
	///     sub-namespaces), their own namespace and framework assemblies. This is the positive counterpart of
	///     <see cref="DependOnlyOn(IThat{IEnumerable{Type?}}, IEnumerable{string})" /> for finding violators of an
	///     allowed set.
	/// </summary>
	/// <remarks>
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.SettingsCustomization.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so that framework dependencies never count as outside the
	///     allowed set. The default prefixes include <c>Microsoft</c>, so e.g. a dependency on
	///     <c>Microsoft.EntityFrameworkCore</c> is also ignored; customize the prefixes to make such a dependency
	///     count.
	/// </remarks>
	[GuaranteesNotNull]
	public static NamespaceDependencyOutsideResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveDependenciesOutside(
		this IThat<IEnumerable<Type?>?> subject, params string[] namespaces)
		=> subject.HaveDependenciesOutside(namespaces.AsEnumerable());

	/// <inheritdoc cref="HaveDependenciesOutside(IThat{IEnumerable{Type?}}, string[])" />
	[GuaranteesNotNull]
	public static NamespaceDependencyOutsideResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveDependenciesOutside(
		this IThat<IEnumerable<Type?>?> subject, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(subject.ExpectAll(new HaveDependenciesOutsideExpectation(options)), subject, options);
	}

	/// <summary>
	///     …have at least one dependency (a type referenced in their signature) outside the allowed
	///     <paramref name="namespaces" /> (including sub-namespaces), their own namespace and framework assemblies.
	/// </summary>
	/// <inheritdoc cref="HaveDependenciesOutside(IThat{IEnumerable{Type?}}, string[])" path="/remarks" />
	public static NamespaceDependencyOutsideResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveDependenciesOutside(
		this IEnumerableItems<Type?> items, params string[] namespaces)
		=> items.HaveDependenciesOutside(namespaces.AsEnumerable());

	/// <inheritdoc cref="HaveDependenciesOutside(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyOutsideResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveDependenciesOutside(
		this IEnumerableItems<Type?> items, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(items.ExpectItems(new HaveDependenciesOutsideExpectation(options)), items.Subject, options);
	}

	/// <inheritdoc cref="HaveDependenciesOutside(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyOutsideResult<TCollection, TCollection> HaveDependenciesOutside<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items, params string[] namespaces)
		where TCollection : struct, IEnumerable<Type?>
		=> items.HaveDependenciesOutside(namespaces.AsEnumerable());

	/// <inheritdoc cref="HaveDependenciesOutside(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyOutsideResult<TCollection, TCollection> HaveDependenciesOutside<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items, IEnumerable<string> namespaces)
		where TCollection : struct, IEnumerable<Type?>
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(items.ExpectItems(new HaveDependenciesOutsideExpectation(options)), items.Subject, options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have at least one dependency
	///     (a type referenced in their signature) outside the allowed <paramref name="namespaces" /> (including
	///     sub-namespaces), their own namespace and framework assemblies. This is the positive counterpart of
	///     <see cref="DependOnlyOn(IThat{IAsyncEnumerable{Type?}}, IEnumerable{string})" /> for finding violators
	///     of an allowed set.
	/// </summary>
	/// <inheritdoc cref="HaveDependenciesOutside(IThat{IEnumerable{Type?}}, string[])" path="/remarks" />
	[GuaranteesNotNull]
	public static NamespaceDependencyOutsideResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveDependenciesOutside(
		this IThat<IAsyncEnumerable<Type?>?> subject, params string[] namespaces)
		=> subject.HaveDependenciesOutside(namespaces.AsEnumerable());

	/// <inheritdoc cref="HaveDependenciesOutside(IThat{IAsyncEnumerable{Type?}}, string[])" />
	[GuaranteesNotNull]
	public static NamespaceDependencyOutsideResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveDependenciesOutside(
		this IThat<IAsyncEnumerable<Type?>?> subject, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(subject.ExpectAll(new HaveDependenciesOutsideExpectation(options)), subject, options);
	}

	/// <inheritdoc cref="HaveDependenciesOutside(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyOutsideResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveDependenciesOutside(
		this IAsyncEnumerableItems<Type?> items, params string[] namespaces)
		=> items.HaveDependenciesOutside(namespaces.AsEnumerable());

	/// <inheritdoc cref="HaveDependenciesOutside(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyOutsideResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveDependenciesOutside(
		this IAsyncEnumerableItems<Type?> items, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(items.ExpectItems(new HaveDependenciesOutsideExpectation(options)), items.Subject, options);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have at least one dependency
	///     (a type referenced in their signature) outside the allowed set formed by the filtered collections of
	///     types <paramref name="target" /> and <paramref name="additional" />, their own namespace and framework
	///     assemblies. This is the positive counterpart of
	///     <see cref="DependOnlyOn(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" /> for finding
	///     violators of an allowed set.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency is inside the allowed set when it
	///     is a member of the union of the resolved collections (by <see cref="Type" /> identity; a generic type
	///     definition in a collection matches any construction of it). A type's own namespace never counts as
	///     outside, including its sub-namespaces unless
	///     <see cref="TypeSetDependencyOutsideResult{TType, TValue}.ExcludingOwnSubNamespaces" /> is used.
	///     <para />
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.SettingsCustomization.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so that framework dependencies never count as outside the
	///     allowed set. The default prefixes include <c>Microsoft</c>, so e.g. a dependency on
	///     <c>Microsoft.EntityFrameworkCore</c> is also ignored; customize the prefixes to make such a dependency
	///     count.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeSetDependencyOutsideResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveDependenciesOutside(
		this IThat<IEnumerable<Type?>?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(subject.ExpectAll(new HaveDependenciesOutsideTypeSetExpectation(options)), subject, options);
	}

	/// <summary>
	///     …have at least one dependency (a type referenced in their signature) outside the allowed set formed by the
	///     filtered collections of types <paramref name="target" /> and <paramref name="additional" />, their own
	///     namespace and framework assemblies.
	/// </summary>
	/// <inheritdoc
	///     cref="HaveDependenciesOutside(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])"
	///     path="/remarks" />
	public static TypeSetDependencyOutsideResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveDependenciesOutside(
		this IEnumerableItems<Type?> items, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(items.ExpectItems(new HaveDependenciesOutsideTypeSetExpectation(options)), items.Subject,
			options);
	}

	/// <inheritdoc cref="HaveDependenciesOutside(IEnumerableItems{Type?}, Filtered.Types, Filtered.Types[])" />
	public static TypeSetDependencyOutsideResult<TCollection, TCollection> HaveDependenciesOutside<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items, Filtered.Types target,
		params Filtered.Types[] additional)
		where TCollection : struct, IEnumerable<Type?>
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(items.ExpectItems(new HaveDependenciesOutsideTypeSetExpectation(options)), items.Subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have at least one dependency
	///     (a type referenced in their signature) outside the allowed set formed by the filtered collections of
	///     types <paramref name="target" /> and <paramref name="additional" />, their own namespace and framework
	///     assemblies. This is the positive counterpart of
	///     <see cref="DependOnlyOn(IThat{IAsyncEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" /> for
	///     finding violators of an allowed set.
	/// </summary>
	/// <inheritdoc
	///     cref="HaveDependenciesOutside(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])"
	///     path="/remarks" />
	[GuaranteesNotNull]
	public static TypeSetDependencyOutsideResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveDependenciesOutside(
		this IThat<IAsyncEnumerable<Type?>?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(subject.ExpectAll(new HaveDependenciesOutsideTypeSetExpectation(options)), subject, options);
	}

	/// <inheritdoc cref="HaveDependenciesOutside(IEnumerableItems{Type?}, Filtered.Types, Filtered.Types[])" />
	public static TypeSetDependencyOutsideResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveDependenciesOutside(
		this IAsyncEnumerableItems<Type?> items, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(items.ExpectItems(new HaveDependenciesOutsideTypeSetExpectation(options)), items.Subject,
			options);
	}
#endif

	private sealed class HaveDependenciesOutsideExpectation(
		IReadOnlyOptionsHolder<NamespaceDependencyOptions> options)
		: DependencyViolationsExpectation(true)
	{
		protected override ValueTask<IReadOnlyList<string>> GetViolations(Type item,
			CancellationToken cancellationToken)
			=> new(item.GetDependencyNamespaceViolations(options.Value));

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
			=> stringBuilder.Append(grammars.IsPlural() ? "have dependencies outside " : "has dependencies outside ")
				.Append(options.Value.Describe());
	}

	private sealed class HaveDependenciesOutsideTypeSetExpectation(
		IReadOnlyOptionsHolder<TypeSetDependencyOptions> options)
		: DependencyViolationsExpectation(true)
	{
		protected override async ValueTask<IReadOnlyList<string>> GetViolations(Type item,
			CancellationToken cancellationToken)
			=> item.GetDependencyTypeSetViolations(await options.Value.Resolve(cancellationToken));

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
			=> stringBuilder.Append(grammars.IsPlural() ? "have dependencies outside " : "has dependencies outside ")
				.Append(options.Value.Describe());
	}
}
