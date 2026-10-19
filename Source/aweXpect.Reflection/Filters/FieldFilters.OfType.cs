using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filter for fields of type <typeparamref name="TField" />.
	/// </summary>
	public static FieldsOfType OfType<TField>(this Filtered.Fields @this)
		=> OfType(@this, typeof(TField));

	/// <summary>
	///     Filter for fields of type <paramref name="fieldType" />.
	/// </summary>
	public static FieldsOfType OfType(this Filtered.Fields @this,
		Type fieldType)
	{
		IChangeableFilter<FieldInfo> filter = Filter.Suffix<FieldInfo>(
			fieldInfo => fieldInfo.FieldType.IsOrInheritsFrom(fieldType),
			$"of type {Formatter.Format(fieldType)} ");
		return new FieldsOfType(@this.Which(filter), filter);
	}

	public partial class FieldsOfType
	{
		/// <summary>
		///     Allow an alternative type <typeparamref name="TField" />.
		/// </summary>
		public FieldsOfType OrOfType<TField>()
			=> OrOfType(typeof(TField));

		/// <summary>
		///     Allow an alternative type <paramref name="fieldType" />.
		/// </summary>
		public FieldsOfType OrOfType(Type fieldType)
		{
			filter.UpdateFilter(
				(result, fieldInfo)
					=> result || fieldInfo.FieldType.IsOrInheritsFrom(fieldType),
				description
					=> $"{description}or of type {Formatter.Format(fieldType)} ");
			return this;
		}
	}
}
