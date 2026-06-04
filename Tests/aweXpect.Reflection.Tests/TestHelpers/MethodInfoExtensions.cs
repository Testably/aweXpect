using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace aweXpect.Reflection.Tests.TestHelpers;

public static class MethodInfoExtensions
{
	/// <summary>
	///     Checks if the <paramref name="methodInfo" /> is asynchronous (declared with the <see langword="async" /> keyword).
	/// </summary>
	/// <param name="methodInfo">The <see cref="MethodInfo" /> to check.</param>
	public static bool IsReallyAsync(this MethodInfo? methodInfo)
		=> methodInfo?.GetCustomAttribute<AsyncStateMachineAttribute>() is not null;

	/// <summary>
	///     Checks if the <paramref name="methodInfo" /> is an extension method (classic <see langword="this" /> parameter or
	///     declared with the C# extension block syntax).
	/// </summary>
	/// <remarks>
	///     This is a parallel reference implementation used to derive the expected method set in the filter and collection
	///     tests, so it must not call into the production code. It mirrors the same detection heuristic and therefore
	///     shares its limitations; it is not an independent oracle. The behaviour of individual methods is pinned by the
	///     hard-coded assertions in <c>ThatMethod.IsAnExtensionMethod</c>.
	/// </remarks>
	/// <param name="methodInfo">The <see cref="MethodInfo" /> to check.</param>
	public static bool IsReallyExtensionMethod(this MethodInfo? methodInfo)
	{
		if (methodInfo is null)
		{
			return false;
		}

		if (methodInfo.IsDefined(typeof(ExtensionAttribute), false))
		{
			return true;
		}

		// The public implementation of a static extension method is itself a static method without the
		// [Extension] attribute; instance (and classic) extension methods always carry the attribute.
		if (!methodInfo.IsStatic)
		{
			return false;
		}

		Type? declaringType = methodInfo.DeclaringType;
		if (declaringType?.IsDefined(typeof(ExtensionAttribute), false) != true)
		{
			return false;
		}

		return declaringType
			.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
			.Where(nestedType => nestedType.Name.StartsWith("<", StringComparison.Ordinal) &&
			                     nestedType.IsDefined(typeof(ExtensionAttribute), false))
			.Any(groupingType => groupingType
				.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
				            BindingFlags.Static | BindingFlags.DeclaredOnly)
				.Any(skeleton => !skeleton.IsSpecialName &&
				                 string.Equals(skeleton.Name, methodInfo.Name, StringComparison.Ordinal) &&
				                 ParametersMatch(skeleton, methodInfo, groupingType.GetGenericArguments().Length)));
	}

	private static bool ParametersMatch(MethodInfo skeleton, MethodInfo implementation, int extensionArity)
	{
		ParameterInfo[] skeletonParameters = skeleton.GetParameters();
		ParameterInfo[] implementationParameters = implementation.GetParameters();
		if (skeletonParameters.Length != implementationParameters.Length)
		{
			return false;
		}

		for (int i = 0; i < skeletonParameters.Length; i++)
		{
			// The extension's type parameters are declared on the grouping type for the skeleton, but merged
			// (ahead of the method's own) into the method for the implementation, so offset the skeleton's
			// method-level type parameters by the extension arity to re-align the two numberings.
			if (!string.Equals(ParameterTypeKey(skeletonParameters[i].ParameterType, extensionArity),
				    ParameterTypeKey(implementationParameters[i].ParameterType, 0), StringComparison.Ordinal))
			{
				return false;
			}
		}

		return true;
	}

	private static string ParameterTypeKey(Type type, int methodGenericOffset)
	{
		if (type.IsByRef || type.IsArray || type.IsPointer)
		{
			string suffix = type switch
			{
				{ IsByRef: true, } => "&",
				{ IsPointer: true, } => "*",
				_ => "[]",
			};
			return ParameterTypeKey(type.GetElementType()!, methodGenericOffset) + suffix;
		}

		if (type.IsGenericParameter)
		{
			int position = type.DeclaringMethod is null
				? type.GenericParameterPosition
				: type.GenericParameterPosition + methodGenericOffset;
			return "!" + position.ToString(CultureInfo.InvariantCulture);
		}

		if (type.IsGenericType)
		{
			Type definition = type.GetGenericTypeDefinition();
			return (definition.FullName ?? definition.Name) + "[" +
			       string.Join(",", type.GetGenericArguments()
				       .Select(argument => ParameterTypeKey(argument, methodGenericOffset))) + "]";
		}

		return type.FullName ?? type.Name;
	}

	/// <summary>
	///     Checks if the <paramref name="methodInfo" /> is an operator (e.g. <c>op_Addition</c>, <c>op_Equality</c>, …).
	/// </summary>
	/// <param name="methodInfo">The <see cref="MethodInfo" /> to check.</param>
	public static bool IsReallyOperator(this MethodInfo? methodInfo)
		=> methodInfo is { IsSpecialName: true, }
		   && methodInfo.Name.StartsWith("op_", StringComparison.Ordinal);
}
