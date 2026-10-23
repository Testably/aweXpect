using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

/// <summary>
///     Extensions properties to filter <see cref="Filtered.Properties" />.
/// </summary>
public static partial class PropertyFilters
{
	/// <summary>
	///     Additional filters on properties of a specific type.
	/// </summary>
	public partial class PropertiesOfType : Filtered.Properties
	{
		private readonly Filter.GenericFilter<PropertyInfo> _filter;
		private readonly Filtered.Properties _source;

		internal PropertiesOfType(Filtered.Properties source, Filter.GenericFilter<PropertyInfo> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}
	}
}
