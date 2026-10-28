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
	public static FieldsOfType WhichAreOfType<TField>(this Filtered.Fields @this)
		=> WhichAreOfType(@this, typeof(TField));

	/// <summary>
	///     Filter for fields of type <paramref name="fieldType" />.
	/// </summary>
	public static FieldsOfType WhichAreOfType(this Filtered.Fields @this,
		Type fieldType)
	{
		Filter.GenericFilter<FieldInfo> filter = Filter.Suffix<FieldInfo>(
			fieldInfo => fieldInfo.FieldType.IsOrInheritsFrom(fieldType),
			() => $"of type {Formatter.Format(fieldType)} ");
		return new FieldsOfType(@this, filter);
	}

	/// <summary>
	///     Filter for fields not of type <typeparamref name="TField" />.
	/// </summary>
	public static Filtered.Fields WhichAreNotOfType<TField>(this Filtered.Fields @this)
		=> WhichAreNotOfType(@this, typeof(TField));

	/// <summary>
	///     Filter for fields not of type <paramref name="fieldType" />.
	/// </summary>
	public static Filtered.Fields WhichAreNotOfType(this Filtered.Fields @this,
		Type fieldType)
		=> @this.Which(Filter.Suffix<FieldInfo>(
			fieldInfo => !fieldInfo.FieldType.IsOrInheritsFrom(fieldType),
			() => $"not of type {Formatter.Format(fieldType)} "));

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
			=> new(_source, _filter.Extend(
				(result, fieldInfo)
					=> result || fieldInfo.FieldType.IsOrInheritsFrom(fieldType),
				description
					=> $"{description}or of type {Formatter.Format(fieldType)} "));
	}
}
