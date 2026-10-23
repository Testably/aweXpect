using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

/// <summary>
///     Extensions fields to filter <see cref="Filtered.Fields" />.
/// </summary>
public static partial class FieldFilters
{
	/// <summary>
	///     Additional filters on fields of a specific type.
	/// </summary>
	public partial class FieldsOfType : Filtered.Fields
	{
		private readonly Filter.GenericFilter<FieldInfo> _filter;
		private readonly Filtered.Fields _source;

		internal FieldsOfType(Filtered.Fields source, Filter.GenericFilter<FieldInfo> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}
	}
}
