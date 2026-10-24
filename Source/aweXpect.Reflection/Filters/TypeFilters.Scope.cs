using System;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     A filterable collection of <see cref="Type" /> that considers inherited attributes, members or relations,
	///     allowing to restrict the filter to declared attributes or members or to direct relations via
	///     <see cref="Directly" />.
	/// </summary>
	public class InheritanceScopeResult : Filtered.Types
	{
		private readonly Func<MemberScope, IFilter<Type>> _create;
		private readonly Filtered.Types _source;

		internal InheritanceScopeResult(Filtered.Types source, Func<MemberScope, IFilter<Type>> create,
			MemberScope memberScope = MemberScope.Inherited)
			: base(source.Which(create(memberScope)))
		{
			_source = source;
			_create = create;
		}

		/// <summary>
		///     Only considers the attributes or members declared directly on the types, or a direct relation (a direct
		///     base class or a directly implemented interface).
		/// </summary>
		public InheritanceScopeResult Directly()
			=> new(_source, _create, MemberScope.DeclaredOnly);

		/// <summary>
		///     Creates the filter of the <paramref name="memberScope" /> on the source types.
		/// </summary>
		private protected Filtered.Types WithScope(MemberScope memberScope)
			=> new InheritanceScopeResult(_source, _create, memberScope);
	}

	/// <summary>
	///     A filterable collection of <see cref="Type" /> filtered by their members, allowing to restrict them to the
	///     declared members via <see cref="InheritanceScopeResult.Directly" /> or to add the members declared on
	///     <see cref="object" />, <see cref="ValueType" /> and <see cref="Enum" /> via <see cref="IncludingObjectMembers" />.
	/// </summary>
	/// <remarks>
	///     By default the inherited members are included, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access. The two options exclude each
	///     other.
	/// </remarks>
	public sealed class MemberScopeResult : InheritanceScopeResult
	{
		internal MemberScopeResult(Filtered.Types source, Func<MemberScope, IFilter<Type>> create)
			: base(source, create)
		{
		}

		/// <summary>
		///     Also considers the inherited members declared on <see cref="object" />, <see cref="ValueType" /> and
		///     <see cref="Enum" />.
		/// </summary>
		public Filtered.Types IncludingObjectMembers()
			=> WithScope(MemberScope.IncludingObjectMembers);
	}
}
