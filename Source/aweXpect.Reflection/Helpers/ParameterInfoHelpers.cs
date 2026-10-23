using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using aweXpect.Options;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Extension methods for <see cref="ParameterInfo" />.
/// </summary>
internal static class ParameterInfoHelpers
{
	/// <summary>
	///     Checks if the <paramref name="parameter" /> is a <see langword="ref" /> parameter.
	/// </summary>
	public static bool IsRefParameter(this ParameterInfo parameter)
		=> parameter.ParameterType.IsByRef && !parameter.IsOut && !parameter.IsIn;

	/// <summary>
	///     Checks if the <paramref name="parameter" /> is an <see langword="out" /> parameter.
	/// </summary>
	public static bool IsOutParameter(this ParameterInfo parameter)
		=> parameter.ParameterType.IsByRef && parameter.IsOut;

	/// <summary>
	///     Checks if the <paramref name="parameter" /> is an <see langword="in" /> parameter.
	/// </summary>
	public static bool IsInParameter(this ParameterInfo parameter)
		=> parameter.ParameterType.IsByRef && parameter.IsIn;

	/// <summary>
	///     Checks if the <paramref name="parameter" /> is an optional parameter.
	/// </summary>
	public static bool IsOptionalParameter(this ParameterInfo parameter)
		=> parameter.IsOptional;

	/// <summary>
	///     Checks if the <paramref name="parameter" /> is a <see langword="params" /> parameter.
	/// </summary>
	/// <remarks>
	///     Detects both a <see langword="params" /> array (marked with <see cref="ParamArrayAttribute" />) and a
	///     C# 13 <see langword="params" /> collection (marked with
	///     <c>System.Runtime.CompilerServices.ParamCollectionAttribute</c>). The latter is matched by name to avoid a
	///     dependency on a type that is not available on all target frameworks.
	/// </remarks>
	public static bool IsParamsParameter(this ParameterInfo parameter)
		=> Attribute.IsDefined(parameter, typeof(ParamArrayAttribute))
		   || parameter.GetCustomAttributesData().Any(attribute
			   => attribute.AttributeType.FullName == "System.Runtime.CompilerServices.ParamCollectionAttribute");

	/// <summary>
	///     Gets the type of the <paramref name="parameter" />, unwrapping the by-ref element type for
	///     <see langword="ref" />, <see langword="out" /> and <see langword="in" /> parameters.
	/// </summary>
	public static Type GetUnderlyingType(this ParameterInfo parameter)
		=> parameter.ParameterType.IsByRef
			? parameter.ParameterType.GetElementType()!
			: parameter.ParameterType;

	/// <summary>
	///     Checks if the <paramref name="method" /> has a parameter at an index allowed by the
	///     <paramref name="collectionIndexOptions" /> which matches the <paramref name="parameterFilterOptions" />.
	/// </summary>
	public static ValueTask<bool> HasMatchingParameter(this MethodBase method,
		CollectionIndexOptions collectionIndexOptions, ParameterFilterOptions parameterFilterOptions)
	{
		ParameterInfo[] parameters = method.GetParameters();
		return parameters.AnyAsync(async (p, i) =>
		{
			return collectionIndexOptions.MatchesIndex(i, parameters.Length) == true && await parameterFilterOptions.Matches(p);
		});
	}
}
