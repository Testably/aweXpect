using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Extension methods for <see cref="Type" />.
/// </summary>
internal static class TypeHelpers
{
	/// <summary>
	///     Searches for constructors in the <paramref name="type" /> that were directly declared there.
	/// </summary>
	public static ConstructorInfo[] GetDeclaredConstructors(
		this Type type)
	{
		try
		{
			CompilerGeneratedMembers included = IncludedCompilerGeneratedMembers;
			return type
				.GetConstructors(BindingFlags.DeclaredOnly |
				                 BindingFlags.NonPublic |
				                 BindingFlags.Public |
				                 BindingFlags.Instance |
				                 BindingFlags.Static)
				.Where(m => !m.IsCompilerGenerated() ||
				            included.HasFlag(CompilerGeneratedMembers.Constructors))
				.ToArray();
		}
		catch (Exception exception) when (exception
			                                  is TypeLoadException
			                                  or FileNotFoundException
			                                  or FileLoadException)
		{
			return [];
		}
	}

	/// <summary>
	///     Searches for events in the <paramref name="type" />, optionally including inherited ones according to the
	///     <paramref name="memberScope" />.
	/// </summary>
	public static EventInfo[] GetDeclaredEvents(
		this Type type,
		MemberScope memberScope = MemberScope.DeclaredOnly)
	{
		try
		{
			CompilerGeneratedMembers included = IncludedCompilerGeneratedMembers;
			IEnumerable<EventInfo> events = type
				.GetEvents(memberScope.ToBindingFlags() |
				           BindingFlags.NonPublic |
				           BindingFlags.Public |
				           BindingFlags.Instance);
			if (memberScope == MemberScope.IncludingInherited)
			{
				events = events.Concat(type.GetInheritedPrivateMembers(
					baseType => baseType.GetEvents(BindingFlags.DeclaredOnly |
					                               BindingFlags.NonPublic |
					                               BindingFlags.Instance),
					@event => @event.AddMethod is not { IsPrivate: false, } &&
					          @event.RemoveMethod is not { IsPrivate: false, }));
			}

			return events
				.Where(m => !m.IsCompilerGenerated() ||
				            included.HasFlag(CompilerGeneratedMembers.Events))
				.ToArray();
		}
		catch (Exception exception) when (exception
			                                  is TypeLoadException
			                                  or FileNotFoundException
			                                  or FileLoadException)
		{
			return [];
		}
	}

	/// <summary>
	///     Searches for fields in the <paramref name="type" />, optionally including inherited ones according to the
	///     <paramref name="memberScope" />.
	/// </summary>
	public static FieldInfo[] GetDeclaredFields(
		this Type type,
		MemberScope memberScope = MemberScope.DeclaredOnly)
	{
		try
		{
			CompilerGeneratedMembers included = IncludedCompilerGeneratedMembers;
			IEnumerable<FieldInfo> fields = type
				.GetFields(memberScope.ToBindingFlags() |
				           BindingFlags.NonPublic |
				           BindingFlags.Public |
				           BindingFlags.Instance |
				           BindingFlags.Static);
			if (memberScope == MemberScope.IncludingInherited)
			{
				fields = fields.Concat(type.GetInheritedPrivateMembers(
					baseType => baseType.GetFields(BindingFlags.DeclaredOnly |
					                               BindingFlags.NonPublic |
					                               BindingFlags.Instance |
					                               BindingFlags.Static),
					field => field.IsPrivate));
			}

			return fields
				.Where(m => !m.IsSpecialName)
				.Where(m => !m.IsCompilerGenerated() ||
				            included.HasFlag(CompilerGeneratedMembers.Fields))
				.ToArray();
		}
		catch (Exception exception) when (exception
			                                  is TypeLoadException
			                                  or FileNotFoundException
			                                  or FileLoadException)
		{
			return [];
		}
	}

	/// <summary>
	///     Searches for methods in the <paramref name="type" />, optionally including inherited ones according to the
	///     <paramref name="memberScope" />.
	/// </summary>
	public static MethodInfo[] GetDeclaredMethods(
		this Type type,
		MemberScope memberScope = MemberScope.DeclaredOnly)
	{
		try
		{
			CompilerGeneratedMembers includedCompilerGenerated = IncludedCompilerGeneratedMembers;
			SpecialNameMembers includedSpecialName = IncludedSpecialNameMembers;
			IEnumerable<MethodInfo> methods = type
				.GetMethods(memberScope.ToBindingFlags() |
				            BindingFlags.NonPublic |
				            BindingFlags.Public |
				            BindingFlags.Static |
				            BindingFlags.Instance);
			if (memberScope == MemberScope.IncludingInherited)
			{
				methods = methods.Concat(type.GetInheritedPrivateMembers(
					baseType => baseType.GetMethods(BindingFlags.DeclaredOnly |
					                                BindingFlags.NonPublic |
					                                BindingFlags.Instance |
					                                BindingFlags.Static),
					method => method.IsPrivate));
			}

			return methods
				.Where(m => IncludeMethod(m, includedCompilerGenerated, includedSpecialName))
				.ToArray();
		}
		catch (Exception exception) when (exception
			                                  is TypeLoadException
			                                  or FileNotFoundException
			                                  or FileLoadException)
		{
			return [];
		}
	}

	/// <summary>
	///     Searches for properties in the <paramref name="type" />, optionally including inherited ones according to the
	///     <paramref name="memberScope" />.
	/// </summary>
	public static PropertyInfo[] GetDeclaredProperties(
		this Type type,
		MemberScope memberScope = MemberScope.DeclaredOnly)
	{
		try
		{
			CompilerGeneratedMembers included = IncludedCompilerGeneratedMembers;
			IEnumerable<PropertyInfo> properties = type
				.GetProperties(memberScope.ToBindingFlags() |
				               BindingFlags.NonPublic |
				               BindingFlags.Public |
				               BindingFlags.Static |
				               BindingFlags.Instance);
			if (memberScope == MemberScope.IncludingInherited)
			{
				properties = properties.Concat(type.GetInheritedPrivateMembers(
					baseType => baseType.GetProperties(BindingFlags.DeclaredOnly |
					                                   BindingFlags.NonPublic |
					                                   BindingFlags.Instance |
					                                   BindingFlags.Static),
					property => property.GetMethod is not { IsPrivate: false, } &&
					            property.SetMethod is not { IsPrivate: false, }));
			}

			return properties
				.Where(m => !m.IsSpecialName)
				.Where(m => !m.IsCompilerGenerated() ||
				            included.HasFlag(CompilerGeneratedMembers.Properties))
				.ToArray();
		}
		catch (Exception exception) when (exception
			                                  is TypeLoadException
			                                  or FileNotFoundException
			                                  or FileLoadException)
		{
			return [];
		}
	}

	/// <summary>
	///     Maps the <paramref name="memberScope" /> to the corresponding <see cref="BindingFlags" />.
	/// </summary>
	/// <remarks>
	///     <see cref="MemberScope.DeclaredOnly" /> restricts the search to members declared directly on the type, while
	///     <see cref="MemberScope.IncludingInherited" /> also returns inherited members (including inherited static members
	///     via <see cref="BindingFlags.FlattenHierarchy" />). Inherited <see langword="private" /> members are not covered
	///     by <see cref="BindingFlags.FlattenHierarchy" /> and are collected separately via
	///     <see cref="GetInheritedPrivateMembers{T}" />.
	/// </remarks>
	private static BindingFlags ToBindingFlags(this MemberScope memberScope)
		=> memberScope == MemberScope.DeclaredOnly
			? BindingFlags.DeclaredOnly
			: BindingFlags.FlattenHierarchy;

	/// <summary>
	///     Collects the <see langword="private" /> members declared on the base types of the <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     <see cref="BindingFlags.FlattenHierarchy" /> does not return <see langword="private" /> members of base types,
	///     so they are gathered here by walking the base type chain and keeping only those members that reflection
	///     excludes from the derived type (i.e. members without any non-private accessor).
	/// </remarks>
	private static IEnumerable<T> GetInheritedPrivateMembers<T>(
		this Type type,
		Func<Type, IEnumerable<T>> getDeclaredMembers,
		Func<T, bool> isPrivate)
	{
		for (Type? baseType = type.BaseType; baseType is not null; baseType = baseType.BaseType)
		{
			foreach (T member in getDeclaredMembers(baseType))
			{
				if (isPrivate(member))
				{
					yield return member;
				}
			}
		}
	}

	/// <summary>
	///     The compiler-generated members that are currently included via customization.
	/// </summary>
	private static CompilerGeneratedMembers IncludedCompilerGeneratedMembers
		=> Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers().Get();

	/// <summary>
	///     The special-name methods that are currently included via customization.
	/// </summary>
	private static SpecialNameMembers IncludedSpecialNameMembers
		=> Customize.aweXpect.Reflection().IncludedSpecialNameMembers().Get();

	/// <summary>
	///     Determines whether the <paramref name="member" /> (or any of its declaring types) is compiler-generated.
	/// </summary>
	public static bool IsCompilerGenerated(this MemberInfo member)
	{
		for (Type? type = member as Type ?? member.DeclaringType;
		     type is not null;
		     type = type.DeclaringType)
		{
			if (type.IsDefined(typeof(CompilerGeneratedAttribute), false))
			{
				return true;
			}
		}

		return member.IsDefined(typeof(CompilerGeneratedAttribute), false);
	}

	private static bool IncludeMethod(
		MethodInfo method,
		CompilerGeneratedMembers includedCompilerGenerated,
		SpecialNameMembers includedSpecialName)
	{
		if (method.IsSpecialName)
		{
			bool isOperator = method.Name.StartsWith("op_", StringComparison.Ordinal);
			if (isOperator
				    ? includedSpecialName.HasFlag(SpecialNameMembers.Operators)
				    : includedSpecialName.HasFlag(SpecialNameMembers.Accessors))
			{
				return true;
			}

			return method.IsCompilerGenerated() &&
			       includedCompilerGenerated.HasFlag(CompilerGeneratedMembers.Methods);
		}

		return !method.IsCompilerGenerated() ||
		       includedCompilerGenerated.HasFlag(CompilerGeneratedMembers.Methods);
	}

	/// <summary>
	///     Checks if the <paramref name="type" /> has the specified <paramref name="accessModifiers" />.
	/// </summary>
	/// <param name="type">The <see cref="MethodInfo" /> which is checked to have the attribute.</param>
	/// <param name="accessModifiers">
	///     The <see cref="AccessModifiers" />.
	///     <para />
	///     Supports specifying multiple <see cref="AccessModifiers" />.
	/// </param>
	public static bool HasAccessModifier(
		this Type type,
		AccessModifiers accessModifiers)
	{
		if (type.IsNested)
		{
			return HasAccessModifierForNestedClass(type, accessModifiers);
		}

		return HasAccessModifierForNotNestedClass(type, accessModifiers);
	}

	/// <summary>
	///     Checks if the <paramref name="type" /> has an attribute which satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <typeparam name="TAttribute">The type of the <see cref="Attribute" />.</typeparam>
	/// <param name="type">The <see cref="Type" /> which is checked to have the attribute.</param>
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
		this Type? type,
		Func<TAttribute, bool>? predicate = null,
		bool inherit = true)
		where TAttribute : Attribute
	{
		object? attribute = type?.GetCustomAttributes(typeof(TAttribute), inherit)
			.FirstOrDefault();
		if (attribute is TAttribute attributeValue)
		{
			return predicate?.Invoke(attributeValue) ?? true;
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="type" /> has an attribute which satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <param name="type">The <see cref="Type" /> which is checked to have the attribute.</param>
	/// <param name="attributeType">The type of the <see cref="Attribute" />.</param>
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
		this Type? type,
		Type attributeType,
		Func<Attribute, bool>? predicate = null,
		bool inherit = true)
	{
		object? attribute = type?.GetCustomAttributes(attributeType, inherit)
			.FirstOrDefault();
		if (attribute is Attribute attributeValue)
		{
			return predicate?.Invoke(attributeValue) ?? true;
		}

		return false;
	}

	/// <summary>
	///     Determines whether the current <see cref="Type" /> implements the <paramref name="interfaceType" />.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <param name="interfaceType">The interface <see cref="Type" />.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="interfaceType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="interfaceType" /> to be directly implemented in the <paramref name="type" />.
	/// </param>
	public static bool Implements(
		this Type type,
		Type interfaceType,
		bool forceDirect = false)
	{
		if (!interfaceType.IsInterface)
		{
			return false;
		}

		Type[] interfaces = type.GetInterfaces();
		if (forceDirect && type.BaseType != null)
		{
			interfaces = interfaces
				.Except(type.BaseType.GetInterfaces())
				.ToArray();
		}

		return interfaces
			.Any(childInterface =>
			{
				Type currentInterface = childInterface.IsGenericType
					? childInterface.GetGenericTypeDefinition()
					: childInterface;

				return currentInterface == interfaceType;
			});
	}

	/// <summary>
	///     Determines whether the current <see cref="Type" /> inherits from the <paramref name="parentType" />.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <param name="parentType">The parent <see cref="Type" />.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="parentType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="parentType" /> to be the direct parent.
	/// </param>
	public static bool InheritsFrom(
		this Type type,
		Type parentType,
		bool forceDirect = false)
	{
		if (type == parentType)
		{
			return false;
		}

		Type currentType = type;

		int level = 0;
		while (currentType != typeof(object))
		{
			if (parentType.IsEqualTo(currentType))
			{
				return true;
			}

			if (forceDirect && level++ > 0)
			{
				return false;
			}

			if (currentType.Implements(parentType, forceDirect))
			{
				return true;
			}

			if (currentType.BaseType == null)
			{
				break;
			}

			currentType = currentType.BaseType;
		}

		return false;
	}

	/// <summary>
	///     Determines whether the current <see cref="Type" /> is the same type as the <paramref name="other" />.<br />
	///     Generic types are considered equal, if either one or both are open generics or the generic argument types
	///     themselves are equal.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <param name="other">The other <see cref="Type" />.</param>
	public static bool IsEqualTo(
		this Type type,
		Type other)
	{
		if (type.IsGenericType != other.IsGenericType)
		{
			return false;
		}

		if (type.IsGenericType)
		{
			return AreGenericTypesCompatible(type, other);
		}

		return type == other;
	}

	/// <summary>
	///     Determines whether the current <see cref="Type" /> is the same type or inherits from the
	///     <paramref name="parentType" />.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <param name="parentType">The parent <see cref="Type" />.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="parentType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="parentType" /> to be the direct parent.
	/// </param>
	public static bool IsOrInheritsFrom(
		this Type type,
		Type parentType,
		bool forceDirect = false)
	{
		if (type.IsEqualTo(parentType))
		{
			return true;
		}

		return !forceDirect && type.InheritsFrom(parentType);
	}

	/// <summary>
	///     Checks if the <paramref name="type" /> is within the <paramref name="expected" /> namespace, i.e. it has the
	///     <paramref name="expected" /> namespace or one of its sub-namespaces.
	/// </summary>
	/// <remarks>
	///     Unlike a prefix match, <c>Foo.Bar</c> does not consider <c>Foo.BarBaz</c> to be within it, because the next
	///     character after the match must be a namespace separator (<c>.</c>) or the end of the namespace.
	/// </remarks>
	public static bool IsWithinNamespace(this Type? type, string expected)
	{
		string? current = type?.Namespace;
		while (current is not null)
		{
			if (string.Equals(current, expected, StringComparison.Ordinal))
			{
				return true;
			}

			int lastSeparator = current.LastIndexOf('.');
			current = lastSeparator < 0 ? null : current.Substring(0, lastSeparator);
		}

		return false;
	}

	public static bool IsReallyClass(this Type? type)
		=> type?.IsClass == true && !type.IsRecordClass();

	public static bool IsRecordClass(this Type? type)
		=> type?.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) is not
			   null &&
		   type.GetProperty("EqualityContract",
				   BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)?
			   .GetMethod?.HasAttribute<CompilerGeneratedAttribute>() == true;


	public static bool IsReallyStruct(this Type? type) =>
		type is { IsValueType: true, IsEnum: false, } &&
		!IsRecordStruct(type);

	/// <summary>
	///     Gets a value indicating whether the <see cref="Type" /> is a read-only struct
	///     (declared with the <c>readonly</c> modifier).
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	public static bool IsReadOnlyStruct(this Type? type)
		=> type is not null &&
		   type.HasNamedAttribute("System.Runtime.CompilerServices.IsReadOnlyAttribute");

	/// <summary>
	///     Gets a value indicating whether the <see cref="Type" /> is a ref struct
	///     (a stack-only, <c>IsByRefLike</c> value type).
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	public static bool IsRefStruct(this Type? type)
		=> type is not null &&
		   type.HasNamedAttribute("System.Runtime.CompilerServices.IsByRefLikeAttribute");

	public static bool IsRecordStruct(this Type? type) =>
		// As noted here: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/record-structs#open-questions
		// recognizing record structs from metadata is an open point. The following check is based on common sense
		// and heuristic testing, apparently giving good results but not supported by official documentation.
		type?.BaseType == typeof(ValueType) &&
		type.GetMethod("PrintMembers", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null,
			[typeof(StringBuilder),], null) is not null &&
		type.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly, null,
				[type, type,], null)?
			.HasAttribute<CompilerGeneratedAttribute>() == true;

	/// <summary>
	///     Gets a value indicating whether the <see cref="Type" /> is static.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <remarks>https://stackoverflow.com/a/1175901</remarks>
	public static bool IsReallyStatic(this Type? type)
		=> type is { IsAbstract: true, IsSealed: true, IsInterface: false, };

	/// <summary>
	///     Gets a value indicating whether the <see cref="Type" /> is sealed and not static.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <remarks>https://stackoverflow.com/a/1175901</remarks>
	public static bool IsReallySealed(this Type? type)
		=> type is { IsAbstract: false, IsSealed: true, IsInterface: false, };

	/// <summary>
	///     Gets a value indicating whether the <see cref="Type" /> is abstract and not sealed and not an interface.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <remarks>https://stackoverflow.com/a/1175901</remarks>
	public static bool IsReallyAbstract(this Type? type)
		=> type is { IsAbstract: true, IsSealed: false, IsInterface: false, };

	/// <summary>
	///     Check if the generic types are compatible.<br />
	///     Generic types are considered compatible, if either one or both are open generics or the generic argument types
	///     themselves are equal.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="other"></param>
	/// <returns></returns>
	private static bool AreGenericTypesCompatible(Type type, Type other)
	{
		if (type.GetGenericTypeDefinition() != other.GetGenericTypeDefinition())
		{
			return false;
		}

		if (!type.IsGenericTypeDefinition && !other.IsGenericTypeDefinition)
		{
			Type[] typeArguments = type.GetGenericArguments();
			Type[] otherArguments = other.GetGenericArguments();
			// `type` and `other` have the same number of arguments,
			// because otherwise the GetGenericTypeDefinition() check would be different for both!
			for (int i = 0; i < typeArguments.Length; i++)
			{
				if (!typeArguments[i].IsEqualTo(otherArguments[i]))
				{
					return false;
				}
			}
		}

		return true;
	}

	private static bool HasNamedAttribute(this Type type, string attributeFullName)
	{
		try
		{
			return type.GetCustomAttributesData()
				.Any(data => string.Equals(data.AttributeType.FullName, attributeFullName, StringComparison.Ordinal));
		}
		catch (Exception exception) when (exception
			                                  is TypeLoadException
			                                  or FileNotFoundException
			                                  or FileLoadException)
		{
			// Ignore types that cannot be loaded.
			return false;
		}
	}

	private static bool HasAccessModifierForNestedClass(Type type, AccessModifiers accessModifiers)
	{
		if (accessModifiers.HasFlag(AccessModifiers.Internal) &&
		    type.IsNestedAssembly)
		{
			return true;
		}

		if (accessModifiers.HasFlag(AccessModifiers.Protected) &&
		    type.IsNestedFamily)
		{
			return true;
		}

		if (accessModifiers.HasFlag(AccessModifiers.Private) &&
		    type.IsNestedPrivate)
		{
			return true;
		}

		if (accessModifiers.HasFlag(AccessModifiers.Public) &&
		    type.IsNestedPublic)
		{
			return true;
		}

		if (accessModifiers.HasFlag(AccessModifiers.PrivateProtected) &&
		    type.IsNestedFamANDAssem)
		{
			return true;
		}

		if (accessModifiers.HasFlag(AccessModifiers.ProtectedInternal) &&
		    type.IsNestedFamORAssem)
		{
			return true;
		}

		return false;
	}

	private static bool HasAccessModifierForNotNestedClass(Type type,
		AccessModifiers accessModifiers)
	{
		if (accessModifiers.HasFlag(AccessModifiers.Internal) &&
		    !type.IsVisible)
		{
			return true;
		}

		if (accessModifiers.HasFlag(AccessModifiers.Public) &&
		    type.IsPublic)
		{
			return true;
		}

		if (accessModifiers.HasFlag(AccessModifiers.Private) &&
		    type.IsNotPublic)
		{
			return true;
		}

		return false;
	}
}
