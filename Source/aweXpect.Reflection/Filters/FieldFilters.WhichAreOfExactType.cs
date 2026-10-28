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
	public static FieldsOfType WhichAreOfExactType<TField>(this Filtered.Fields @this)
		=> WhichAreOfExactType(@this, typeof(TField));

	/// <summary>
	///     Filter for fields of exact type <paramref name="fieldType" />.
	/// </summary>
	public static FieldsOfType WhichAreOfExactType(this Filtered.Fields @this,
		Type fieldType)
	{
		Filter.GenericFilter<FieldInfo> filter = Filter.Suffix<FieldInfo>(
			fieldInfo => fieldInfo.FieldType.IsOrInheritsFrom(fieldType, true),
			() => $"of exact type {Formatter.Format(fieldType)} ");
		return new FieldsOfType(@this, filter);
	}

	/// <summary>
	///     Filter for fields not of exact type <typeparamref name="TField" />.
	/// </summary>
	public static Filtered.Fields WhichAreNotOfExactType<TField>(this Filtered.Fields @this)
		=> WhichAreNotOfExactType(@this, typeof(TField));

	/// <summary>
	///     Filter for fields not of exact type <paramref name="fieldType" />.
	/// </summary>
	public static Filtered.Fields WhichAreNotOfExactType(this Filtered.Fields @this,
		Type fieldType)
		=> @this.Which(Filter.Suffix<FieldInfo>(
			fieldInfo => !fieldInfo.FieldType.IsOrInheritsFrom(fieldType, true),
			() => $"not of exact type {Formatter.Format(fieldType)} "));

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
			=> new(_source, _filter.Extend(
				(result, fieldInfo)
					=> result || fieldInfo.FieldType.IsOrInheritsFrom(fieldType, true),
				description
					=> $"{description}or of exact type {Formatter.Format(fieldType)} "));
	}
}
