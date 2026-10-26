using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that contain properties matching the <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default a type passes when it contains at least one matching property. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TResult}(TResult, Times)" />) to require a specific count.<br />
	///     The inherited properties are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access.
	///     Restrict them with <see cref="TypesContainingMembersScopeResult.Directly" /> or add the members of these
	///     roots with <see cref="TypesContainingMembersScopeResult.IncludingObjectMembers" />.
	/// </remarks>
	public static TypesContainingMembersScopeResult WhichContainProperties(this Filtered.Types @this,
		Func<Filtered.Properties, Filtered.Properties> filter)
		=> ContainingMembers<PropertyInfo, Filtered.Properties>(@this, (types, memberScope) => types.Properties(memberScope), filter);
}
