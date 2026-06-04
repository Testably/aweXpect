using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Extension methods for <see cref="MethodInfo" />.
/// </summary>
internal static class MethodInfoHelpers
{
	/// <summary>
	///     Checks if the <paramref name="methodInfo" /> has the specified <paramref name="accessModifiers" />.
	/// </summary>
	/// <param name="methodInfo">The <see cref="MethodInfo" /> which is checked to have the attribute.</param>
	/// <param name="accessModifiers">
	///     The <see cref="AccessModifiers" />.
	///     <para />
	///     Supports specifying multiple <see cref="AccessModifiers" />.
	/// </param>
	public static bool HasAccessModifier(
		this MethodInfo? methodInfo,
		AccessModifiers accessModifiers)
	{
		if (methodInfo == null)
		{
			return false;
		}

		if (accessModifiers.HasFlag(AccessModifiers.Internal) &&
		    methodInfo.IsAssembly)
		{
			return true;
		}

		if (accessModifiers.HasFlag(AccessModifiers.Protected) &&
		    methodInfo.IsFamily)
		{
			return true;
		}

		if (accessModifiers.HasFlag(AccessModifiers.Private) &&
		    methodInfo.IsPrivate)
		{
			return true;
		}

		if (accessModifiers.HasFlag(AccessModifiers.Public) &&
		    methodInfo.IsPublic)
		{
			return true;
		}

		if (accessModifiers.HasFlag(AccessModifiers.PrivateProtected) &&
		    methodInfo.IsFamilyAndAssembly)
		{
			return true;
		}

		if (accessModifiers.HasFlag(AccessModifiers.ProtectedInternal) &&
		    methodInfo.IsFamilyOrAssembly)
		{
			return true;
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="methodInfo" /> has an attribute which satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <typeparam name="TAttribute">The type of the <see cref="Attribute" />.</typeparam>
	/// <param name="methodInfo">The <see cref="MethodInfo" /> which is checked to have the attribute.</param>
	/// <param name="predicate">
	///     (optional) A predicate to check the attribute values.
	///     <para />
	///     If not set (<see langword="null" />), will only check if the attribute is present.
	/// </param>
	/// <param name="inherit">
	///     <see langword="true" /> to search the inheritance chain to find the attributes; otherwise,
	///     <see langword="false" />.<br />
	///     Defaults to <see langword="true" />
	/// </param>
	public static bool HasAttribute<TAttribute>(
		this MethodInfo methodInfo,
		Func<TAttribute, bool>? predicate = null,
		bool inherit = true)
		where TAttribute : Attribute
	{
		object? attribute = Attribute.GetCustomAttributes(methodInfo, typeof(TAttribute), inherit)
			.FirstOrDefault();
		if (attribute is TAttribute castedAttribute)
		{
			return predicate?.Invoke(castedAttribute) ?? true;
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="methodInfo" /> has an attribute which satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <param name="methodInfo">The <see cref="MethodInfo" /> which is checked to have the attribute.</param>
	/// <param name="attributeType">The type of the attribute to check for.</param>
	/// <param name="predicate">
	///     (optional) A predicate to check the attribute values.
	///     <para />
	///     If not set (<see langword="null" />), will only check if the attribute is present.
	/// </param>
	/// <param name="inherit">
	///     <see langword="true" /> to search the inheritance chain to find the attributes; otherwise,
	///     <see langword="false" />.<br />
	///     Defaults to <see langword="true" />
	/// </param>
	public static bool HasAttribute(
		this MethodInfo? methodInfo,
		Type attributeType,
		Func<Attribute, bool>? predicate = null,
		bool inherit = true)
	{
		object? attribute = methodInfo?.GetCustomAttributes(attributeType, inherit)
			.FirstOrDefault();
		if (attribute is Attribute attributeValue)
		{
			return predicate?.Invoke(attributeValue) ?? true;
		}

		return false;
	}

	/// <summary>
	///     Gets a value indicating whether the <see cref="MethodInfo" /> is sealed (virtual and final).
	/// </summary>
	/// <param name="methodInfo">The <see cref="MethodInfo" />.</param>
	public static bool IsReallySealed(this MethodInfo? methodInfo)
		=> methodInfo is { IsVirtual: true, IsFinal: true, };

	/// <summary>
	///     Gets a value indicating whether the <see cref="MethodInfo" /> overrides a base class method.
	/// </summary>
	/// <param name="methodInfo">The <see cref="MethodInfo" />.</param>
	public static bool IsOverride(this MethodInfo? methodInfo)
		=> methodInfo is not null
		   && methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType;

	/// <summary>
	///     Gets a value indicating whether the <see cref="MethodInfo" /> is declared with the <see langword="async" />
	///     keyword.
	/// </summary>
	/// <remarks>
	///     Detection is based on the <see cref="AsyncStateMachineAttribute" /> which the compiler emits for methods
	///     declared with the <see langword="async" /> keyword. This also covers <see langword="async" />
	///     <see langword="void" /> methods and is more accurate than inspecting the return type.
	/// </remarks>
	/// <param name="methodInfo">The <see cref="MethodInfo" />.</param>
	public static bool IsAsync(this MethodInfo? methodInfo)
		=> methodInfo?.GetCustomAttribute<AsyncStateMachineAttribute>() is not null;

	/// <summary>
	///     Gets a value indicating whether the <see cref="MethodInfo" /> is an extension method.
	/// </summary>
	/// <remarks>
	///     Classic extension methods and instance extension methods declared with the C# extension block syntax are
	///     marked with the <see cref="ExtensionAttribute" /> on their public implementation. Static extension methods
	///     declared with the extension block syntax are not, so they are detected by correlating the implementation with
	///     the <c>&lt;G&gt;$…</c> grouping type that the compiler emits for the extension block.
	/// </remarks>
	/// <param name="methodInfo">The <see cref="MethodInfo" />.</param>
	public static bool IsExtensionMethod(this MethodInfo? methodInfo)
	{
		if (methodInfo is null)
		{
			return false;
		}

		return methodInfo.IsDefined(typeof(ExtensionAttribute), false) ||
		       methodInfo.IsStaticExtensionMethod();
	}

	/// <summary>
	///     Gets a value indicating whether the <see cref="MethodInfo" /> is an operator (e.g. <c>op_Addition</c>,
	///     <c>op_Equality</c>, …).
	/// </summary>
	/// <remarks>
	///     Detection is based on the special-name flag combined with the <c>op_</c> name prefix which the compiler emits for
	///     operator overloads.
	/// </remarks>
	/// <param name="methodInfo">The <see cref="MethodInfo" />.</param>
	public static bool IsOperator(this MethodInfo? methodInfo)
		=> methodInfo is { IsSpecialName: true, }
		   && methodInfo.Name.StartsWith("op_", StringComparison.Ordinal);

	/// <summary>
	///     Gets a value indicating whether the <see cref="MethodInfo" /> is the public accessor implementation of an
	///     extension property declared with the C# extension block syntax (e.g. <c>get_IsBlank</c>, <c>set_Value</c>).
	/// </summary>
	/// <remarks>
	///     These accessors are emitted onto the public static class but - unlike regular property accessors - are not
	///     flagged as special-name (there is no property on the public class for them to be special-named against), so
	///     they would otherwise leak into the method results. They are recognised by correlating the implementation with
	///     the matching special-name skeleton in the <c>&lt;G&gt;$…</c> grouping type that the compiler emits for the
	///     surrounding extension block.
	/// </remarks>
	/// <param name="methodInfo">The <see cref="MethodInfo" />.</param>
	public static bool IsExtensionPropertyAccessor(this MethodInfo methodInfo)
	{
		// The public accessor implementation is always static; instance/static extension property accessors alike are
		// emitted as static methods on the public class. Anything non-static cannot be such an accessor.
		if (!methodInfo.IsStatic)
		{
			return false;
		}

		Type? declaringType = methodInfo.DeclaringType;
		if (declaringType?.IsDefined(typeof(ExtensionAttribute), false) != true)
		{
			return false;
		}

		foreach (Type nestedType in declaringType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
		{
			if (!nestedType.IsExtensionGroupingType())
			{
				continue;
			}

			foreach (MethodInfo skeleton in nestedType.GetMethods(BindingFlags.Public |
			                                                      BindingFlags.NonPublic |
			                                                      BindingFlags.Static |
			                                                      BindingFlags.Instance |
			                                                      BindingFlags.DeclaredOnly))
			{
				if (skeleton.IsSpecialName &&
				    string.Equals(skeleton.Name, methodInfo.Name, StringComparison.Ordinal))
				{
					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	///     Detects static extension methods declared with the C# extension block syntax. Unlike instance extension
	///     methods, their public implementation is not marked with the <see cref="ExtensionAttribute" />, so the
	///     implementation is correlated with the matching skeleton in the <c>&lt;G&gt;$…</c> grouping type that the
	///     compiler emits for the surrounding extension block.
	/// </summary>
	private static bool IsStaticExtensionMethod(this MethodInfo methodInfo)
	{
		// The public implementation of a static extension method is itself a static method without the
		// [Extension] attribute; instance (and classic) extension methods always carry the attribute and are
		// handled before reaching this point. Anything non-static therefore cannot be a static extension method.
		if (!methodInfo.IsStatic)
		{
			return false;
		}

		Type? declaringType = methodInfo.DeclaringType;
		if (declaringType?.IsDefined(typeof(ExtensionAttribute), false) != true)
		{
			return false;
		}

		foreach (Type nestedType in declaringType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
		{
			if (!nestedType.IsExtensionGroupingType())
			{
				continue;
			}

			int extensionArity = nestedType.GetGenericArguments().Length;
			foreach (MethodInfo skeleton in nestedType.GetMethods(BindingFlags.Public |
			                                                      BindingFlags.NonPublic |
			                                                      BindingFlags.Static |
			                                                      BindingFlags.DeclaredOnly))
			{
				if (!skeleton.IsSpecialName &&
				    string.Equals(skeleton.Name, methodInfo.Name, StringComparison.Ordinal) &&
				    ParametersMatch(skeleton, methodInfo, extensionArity))
				{
					return true;
				}
			}
		}

		return false;
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
			// On the skeleton the extension's type parameters are declared on the grouping type, while the
			// implementation merges them (ahead of the method's own type parameters) into the method itself.
			// Offsetting the skeleton's method-level type parameters by the extension arity re-aligns the two.
			if (!string.Equals(ParameterTypeKey(skeletonParameters[i].ParameterType, extensionArity),
				    ParameterTypeKey(implementationParameters[i].ParameterType, 0), StringComparison.Ordinal))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	///     Builds the comparison key for a parameter type when correlating a public implementation with its
	///     grouping-type skeleton.
	/// </summary>
	/// <remarks>
	///     Generic type parameters are named differently on the grouping type (<c>$T0</c>, <c>$T1</c>, …) than on the
	///     public implementation (the source names), so they are keyed by their position within a single merged
	///     numbering: parameters declared on the grouping type keep their position, while those declared on the method
	///     are shifted past the extension's type parameters (<paramref name="methodGenericOffset" />) which the
	///     implementation hoists ahead of its own. Constructed generic types, arrays and by-ref/pointer types are keyed
	///     recursively so their generic arguments and element types are compared too. All other types are keyed by their
	///     full name (avoiding false matches between unrelated types that share a simple name), falling back to the
	///     simple name when the full name is unavailable.
	/// </remarks>
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
}
