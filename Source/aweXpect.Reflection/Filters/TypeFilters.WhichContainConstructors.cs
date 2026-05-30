using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that contain constructors matching the <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default a type passes when it contains at least one matching constructor. Append a quantifier
	///     (e.g. <see cref="TypesContainingMembers.Exactly(Times)" />) to require a specific count.
	/// </remarks>
	public static TypesContainingMembers WhichContainConstructors(this Filtered.Types @this,
		Func<Filtered.Constructors, Filtered.Constructors> filter)
		=> Containing<ConstructorInfo, Filtered.Constructors>(@this, types => types.Constructors(), filter);
}
