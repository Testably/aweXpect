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
	///     (e.g. <see cref="TypesContainingMembers.Exactly(Times)" />) to require a specific count.
	/// </remarks>
	public static TypesContainingMembers WhichContainProperties(this Filtered.Types @this,
		Func<Filtered.Properties, Filtered.Properties> filter)
		=> Containing<PropertyInfo, Filtered.Properties>(@this, types => types.Properties(), filter);
}
