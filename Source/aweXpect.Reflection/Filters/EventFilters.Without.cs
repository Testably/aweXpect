using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filter for events without attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static Filtered.Events Without<TAttribute>(this Filtered.Events @this, bool inherit = true)
		where TAttribute : Attribute
		=> @this.Which(Filter.Suffix<EventInfo>(
			eventInfo => !eventInfo.HasAttribute<TAttribute>(inherit: inherit),
			$"without {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} "));
}
