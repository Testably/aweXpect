using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filter for fields of exact type <typeparamref name="TField" />.
	/// </summary>
	public static FieldsOfType OfExactType<TField>(this Filtered.Fields @this)
		=> OfExactType(@this, typeof(TField));

	/// <summary>
	///     Filter for fields of exact type <paramref name="fieldType" />.
	/// </summary>
	public static FieldsOfType OfExactType(this Filtered.Fields @this,
		Type fieldType)
	{
		IChangeableFilter<FieldInfo> filter = Filter.Suffix<FieldInfo>(
			fieldInfo => fieldInfo.FieldType.IsOrInheritsFrom(fieldType, true),
			$"of exact type {Formatter.Format(fieldType)} ");
		return new FieldsOfType(@this.Which(filter), filter);
	}

	public partial class FieldsOfType
	{
		/// <summary>
		///     Allow an alternative type <typeparamref name="TField" /> exactly.
		/// </summary>
		public FieldsOfType OrOfExactType<TField>()
			=> OrOfExactType(typeof(TField));

		/// <summary>
		///     Allow an alternative type <paramref name="fieldType" /> exactly.
		/// </summary>
		public FieldsOfType OrOfExactType(Type fieldType)
		{
			filter.UpdateFilter(
				(result, fieldInfo)
					=> result || fieldInfo.FieldType.IsOrInheritsFrom(fieldType, true),
				description
					=> $"{description}or of exact type {Formatter.Format(fieldType)} ");
			return this;
		}
	}
}
