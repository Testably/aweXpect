using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     A filterable collection of <see cref="PropertyInfo" /> that considers inherited attributes, allowing to restrict the
	///     filter to the attributes declared directly on the property via <see cref="Directly" />.
	/// </summary>
	public sealed class InheritanceScopeResult : Filtered.Properties
	{
		private readonly AttributeFilter<PropertyInfo> _filter;
		private readonly Filtered.Properties _source;

		internal InheritanceScopeResult(Filtered.Properties source, AttributeFilter<PropertyInfo> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}

		/// <summary>
		///     Only considers the attributes declared directly on the property, not those inherited from a base type or an
		///     overridden member.
		/// </summary>
		public InheritanceScopeResult Directly()
			=> new(_source, _filter.Directly());
	}
}
