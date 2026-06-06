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
	///     The <paramref name="filter" /> receives the methods declared on each individual type and may use the
	///     full method-filter DSL (e.g. <c>methods.With&lt;FactAttribute&gt;().OrWith&lt;TheoryAttribute&gt;()</c>).<br />
	///     By default a type passes when it contains at least one matching method. Append a quantifier
	///     (e.g. <see cref="TypesContainingMembers.Exactly(Times)" />) to require a specific count.<br />
	///     The <paramref name="memberScope" /> controls whether inherited methods are considered.
	/// </remarks>
	public static TypesContainingMembers WhichContainMethods(this Filtered.Types @this,
		Func<Filtered.Methods, Filtered.Methods> filter,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> Containing<MethodInfo, Filtered.Methods>(@this, types => types.Methods(memberScope), filter);
}
