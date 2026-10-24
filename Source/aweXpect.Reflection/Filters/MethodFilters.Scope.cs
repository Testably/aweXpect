using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     A filterable collection of <see cref="MethodInfo" /> that considers inherited attributes, allowing to restrict the
	///     filter to the attributes declared directly on the method via <see cref="Directly" />.
	/// </summary>
	public sealed class InheritanceScopeResult : Filtered.Methods
	{
		private readonly AttributeFilter<MethodInfo> _filter;
		private readonly Filtered.Methods _source;

		internal InheritanceScopeResult(Filtered.Methods source, AttributeFilter<MethodInfo> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}

		/// <summary>
		///     Only considers the attributes declared directly on the method, not those inherited from a base type or an
		///     overridden member.
		/// </summary>
		public InheritanceScopeResult Directly()
			=> new(_source, _filter.Directly());
	}
}
