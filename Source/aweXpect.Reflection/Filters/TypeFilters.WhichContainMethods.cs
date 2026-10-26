using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that contain methods matching the <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="filter" /> receives the methods of each individual type and may use the
	///     full method-filter DSL (e.g. <c>methods.With&lt;FactAttribute&gt;().OrWith&lt;TheoryAttribute&gt;()</c>).<br />
	///     By default a type passes when it contains at least one matching method. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TResult}(TResult, Times)" />) to require a specific count.<br />
	///     The inherited methods are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access.
	///     Restrict them with <see cref="TypesContainingMembersScopeResult.Directly" /> or add the members of these
	///     roots with <see cref="TypesContainingMembersScopeResult.IncludingObjectMembers" />.
	/// </remarks>
	public static TypesContainingMembersScopeResult WhichContainMethods(this Filtered.Types @this,
		Func<Filtered.Methods, Filtered.Methods> filter)
		=> ContainingMembers<MethodInfo, Filtered.Methods>(@this, (types, memberScope) => types.Methods(memberScope), filter);
}
