using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

/// <summary>
///     Extensions methods to filter <see cref="Filtered.Types" />.
/// </summary>
public static partial class TypeFilters
{
	private static TypesContainingMembers Containing<TMember, TFiltered>(Filtered.Types @this,
		Func<Filtered.Types, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		OptionsHolder<Quantifier> quantifier = new(Quantifier.Default);
		ContainedMembersFilter<TMember, TFiltered> containedMembersFilter =
			new(navigate, filter, quantifier, MemberScopes.None);
		return new TypesContainingMembers(@this, containedMembersFilter.WithQuantifier, quantifier);
	}

	private static TypesContainingMembersScopeResult ContainingMembers<TMember, TFiltered>(
		Filtered.Types @this,
		Func<Filtered.Types, MemberScopes, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		return new TypesContainingMembersScopeResult(@this, memberScope =>
			new ContainedMembersFilter<TMember, TFiltered>(types => navigate(types, memberScope), filter,
				new OptionsHolder<Quantifier>(Quantifier.Default), memberScope).WithQuantifier,
			MemberScopes.None, new OptionsHolder<Quantifier>(Quantifier.Default));
	}

	/// <summary>
	///     Creates the shared <see cref="IContainedMembersFilter" /> that counts the members of a single
	///     <see cref="Type" /> matching the <paramref name="filter" />, reusing the same counting and description logic
	///     as the <see cref="Filtered.Types" /> filters.
	/// </summary>
	internal static IContainedMembersFilter ContainedMembers<TMember, TFiltered>(
		Func<Filtered.Types, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter,
		OptionsHolder<Quantifier> quantifier)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
		=> new ContainedMembersFilter<TMember, TFiltered>(navigate, filter, quantifier, MemberScopes.None);

	/// <summary>
	///     A filterable collection of <see cref="Type" /> that contain matching members, allowing to specify a quantifier
	///     for the number of matching members.
	/// </summary>
	/// <remarks>
	///     The quantifiers are the methods of <see cref="QuantifierExtensions" />, e.g. <c>Once()</c>. Each returns a new
	///     filter, so that a stored filter stays unchanged; a repeated quantifier replaces the earlier one.
	/// </remarks>
	public class TypesContainingMembers : Filtered.Types,
		IOptionsProvider<Quantifier>,
		ICopyOnOption<Quantifier, TypesContainingMembers>
	{
		private readonly OptionsHolder<Quantifier> _quantifier;
		private readonly Filtered.Types _source;
		private readonly Func<OptionsHolder<Quantifier>, IFilter<Type>> _withQuantifier;

		internal TypesContainingMembers(Filtered.Types source,
			Func<OptionsHolder<Quantifier>, IFilter<Type>> withQuantifier,
			OptionsHolder<Quantifier> quantifier)
			: base(source.Which(withQuantifier(quantifier)))
		{
			_source = source;
			_withQuantifier = withQuantifier;
			_quantifier = quantifier;
		}

		/// <inheritdoc cref="IOptionsProvider{Quantifier}.Options" />
		OptionsHolder<Quantifier> IOptionsProvider<Quantifier>.Options => _quantifier;

		/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
		TypesContainingMembers ICopyOnOption<Quantifier, TypesContainingMembers>.WithOptions(Quantifier options)
			=> new(_source, _withQuantifier, new OptionsHolder<Quantifier>(options));
	}

	/// <summary>
	///     A filterable collection of <see cref="Type" /> that contain matching members, allowing to restrict them to
	///     the declared members via <see cref="Directly" /> or to add the members declared on <see cref="object" />,
	///     <see cref="ValueType" /> and <see cref="Enum" /> via <see cref="IncludingObjectMembers" />.
	/// </summary>
	/// <remarks>
	///     By default the inherited members are included, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access. The two options exclude each
	///     other and keep a quantifier that was set before them.
	/// </remarks>
	public sealed class TypesContainingMembersScopeResult : TypesContainingMembers,
		ICopyOnOption<Quantifier, TypesContainingMembersScopeResult>
	{
		private readonly Func<MemberScopes, Func<OptionsHolder<Quantifier>, IFilter<Type>>> _create;
		private readonly MemberScopes _memberScope;
		private readonly OptionsHolder<Quantifier> _quantifier;
		private readonly Filtered.Types _scopeSource;
		private readonly Func<OptionsHolder<Quantifier>, IFilter<Type>> _withQuantifier;

		internal TypesContainingMembersScopeResult(Filtered.Types source,
			Func<MemberScopes, Func<OptionsHolder<Quantifier>, IFilter<Type>>> create,
			MemberScopes memberScope,
			OptionsHolder<Quantifier> quantifier)
			: this(source, create, memberScope, create(memberScope), quantifier)
		{
		}

		private TypesContainingMembersScopeResult(Filtered.Types source,
			Func<MemberScopes, Func<OptionsHolder<Quantifier>, IFilter<Type>>> create,
			MemberScopes memberScope,
			Func<OptionsHolder<Quantifier>, IFilter<Type>> withQuantifier,
			OptionsHolder<Quantifier> quantifier)
			: base(source, withQuantifier, quantifier)
		{
			_scopeSource = source;
			_create = create;
			_memberScope = memberScope;
			_withQuantifier = withQuantifier;
			_quantifier = quantifier;
		}

		/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
		TypesContainingMembersScopeResult ICopyOnOption<Quantifier, TypesContainingMembersScopeResult>.WithOptions(
			Quantifier options)
			=> new(_scopeSource, _create, _memberScope, _withQuantifier, new OptionsHolder<Quantifier>(options));

		/// <summary>
		///     Only considers the members declared directly on the types.
		/// </summary>
		public TypesContainingMembers Directly()
			=> new TypesContainingMembersScopeResult(_scopeSource, _create, MemberScopes.DeclaredOnly,
				new OptionsHolder<Quantifier>(_quantifier.Value));

		/// <summary>
		///     Also considers the inherited members declared on <see cref="object" />, <see cref="ValueType" /> and
		///     <see cref="Enum" />.
		/// </summary>
		public TypesContainingMembers IncludingObjectMembers()
			=> new TypesContainingMembersScopeResult(_scopeSource, _create, MemberScopes.IncludingObjectMembers,
				new OptionsHolder<Quantifier>(_quantifier.Value));
	}

	private sealed class ContainedMembersFilter<TMember, TFiltered> : IContainedMembersFilter
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		private readonly Func<TFiltered, TFiltered> _filter;
		private readonly MemberScopes _memberScope;
		private readonly Func<Filtered.Types, TFiltered> _navigate;
		private readonly OptionsHolder<Quantifier> _quantifier;

		public ContainedMembersFilter(Func<Filtered.Types, TFiltered> navigate,
			Func<TFiltered, TFiltered> filter,
			OptionsHolder<Quantifier> quantifier,
			MemberScopes memberScope)
		{
			ThrowHelper.ThrowIfNull(filter, nameof(filter));
			_navigate = navigate;
			_filter = filter;
			_quantifier = quantifier;
			_memberScope = memberScope;

			// Derive the inner description from an empty probe, e.g. "methods with FactAttribute or with TheoryAttribute ".
			string inner = _filter(_navigate(new Filtered.Types([], static () => ""))).GetDescription();
			if (inner.EndsWith("in "))
			{
				inner = inner.Substring(0, inner.Length - "in ".Length);
			}

			MembersDescription = inner;
		}

		private ContainedMembersFilter(ContainedMembersFilter<TMember, TFiltered> original, OptionsHolder<Quantifier> quantifier)
		{
			_navigate = original._navigate;
			_filter = original._filter;
			_quantifier = quantifier;
			_memberScope = original._memberScope;
			MembersDescription = original.MembersDescription;
		}

		/// <inheritdoc cref="IContainedMembersFilter.MembersDescription" />
		public string MembersDescription { get; }

		/// <summary>
		///     Returns a copy of this filter that uses the given <paramref name="quantifier" />.
		/// </summary>
		public ContainedMembersFilter<TMember, TFiltered> WithQuantifier(OptionsHolder<Quantifier> quantifier)
			=> new ContainedMembersFilter<TMember, TFiltered>(this, quantifier);

#if NET8_0_OR_GREATER
		/// <inheritdoc cref="IContainedMembersFilter.CountMatchingMembers(Type, CancellationToken)" />
		public async ValueTask<int> CountMatchingMembers(Type value, CancellationToken cancellationToken)
		{
			int count = 0;
			await foreach (var _ in Filter(value).WithCancellation(cancellationToken))
			{
				cancellationToken.ThrowIfCancellationRequested();
				count++;
			}

			return count;
		}
#else
		/// <inheritdoc cref="IContainedMembersFilter.CountMatchingMembers(Type, CancellationToken)" />
		public ValueTask<int> CountMatchingMembers(Type value, CancellationToken cancellationToken)
		{
			int count = 0;
			foreach (var _ in Filter(value))
			{
				cancellationToken.ThrowIfCancellationRequested();
				count++;
			}

			return new ValueTask<int>(count);
		}
#endif

		public async ValueTask<bool> Applies(Type value, CancellationToken cancellationToken)
			=> _quantifier.Value.Check(await CountMatchingMembers(value, cancellationToken), true) ?? false;

		private TFiltered Filter(Type value)
			=> UserCode.Invoke(_filter, _navigate(new Filtered.Types([value,], static () => "")), "the filter");

		public string Describes(string text)
			=> $"{text.TrimEnd()} which {(_memberScope.IsDirect() ? "declare" : "contain")} {MembersDescription}{_memberScope.ObjectMembersText()}{_quantifier} ";
	}
}
