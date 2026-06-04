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
				           BindingFlags.Instance |
				           BindingFlags.Static);
			if (memberScope == MemberScope.IncludingInherited)
			{
				events = events.Concat(type.GetInheritedPrivateMembers(
					baseType => baseType.GetEvents(BindingFlags.DeclaredOnly |
					                               BindingFlags.NonPublic |
					                               BindingFlags.Instance |
					                               BindingFlags.Static),
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
		MemberScope memberScope = MemberScope.DeclaredOnly,
		bool includeOperators = false)
	{
		try
		{
			CompilerGeneratedMembers includedCompilerGenerated = IncludedCompilerGeneratedMembers;
			SpecialNameMembers includedSpecialName = IncludedSpecialNameMembers;
			if (includeOperators)
			{
				includedSpecialName |= SpecialNameMembers.Operators;
			}

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
				.Concat(type.GetExtensionProperties())
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
	///     Determines whether the <paramref name="member" /> (or any of its declaring types) is compiler-generated.
	/// </summary>
	public static bool IsCompilerGenerated(this MemberInfo member)
	{
		for (Type? type = member as Type ?? member.DeclaringType;
		     type is not null;
		     type = type.DeclaringType)
		{
			if (type.IsDefined(typeof(CompilerGeneratedAttribute), false) ||
			    type.IsExtensionGroupingType())
			{
				return true;
			}
		}

		return member.IsDefined(typeof(CompilerGeneratedAttribute), false);
	}

	/// <summary>
	///     Determines whether the <paramref name="type" /> is a compiler-generated grouping type that backs the C#
	///     extension block syntax (the unspeakable <c>&lt;G&gt;$…</c> container emitted for the new extension members).
	/// </summary>
	/// <remarks>
	///     These types are not marked with <see cref="CompilerGeneratedAttribute" />, but they always carry the
	///     <see cref="ExtensionAttribute" /> and have an unspeakable name (which a source identifier can never have).
	/// </remarks>
	internal static bool IsExtensionGroupingType(this Type type)
		=> type.Name.StartsWith("<", StringComparison.Ordinal) &&
		   type.IsDefined(typeof(ExtensionAttribute), false);

	/// <summary>
	///     Collects the extension properties declared with the C# extension block syntax that the
	///     <paramref name="type" /> owns.
	/// </summary>
	/// <remarks>
	///     Extension properties are not visible on the public static class; the real <see cref="PropertyInfo" /> is held
	///     by the compiler-generated <c>&lt;G&gt;$…</c> grouping types nested in the class. They are surfaced here so that
	///     they appear alongside the regular properties of the owning type, even though the grouping types themselves are
	///     excluded from type discovery.
	/// </remarks>
	private static IEnumerable<PropertyInfo> GetExtensionProperties(this Type type)
	{
		foreach (Type nestedType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
		{
			if (!nestedType.IsExtensionGroupingType())
			{
				continue;
			}

			foreach (PropertyInfo property in nestedType.GetProperties(BindingFlags.Public |
			                                                           BindingFlags.NonPublic |
			                                                           BindingFlags.Static |
			                                                           BindingFlags.Instance |
			                                                           BindingFlags.DeclaredOnly))
			{
				yield return property;
			}
		}
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

		if (method.IsExtensionPropertyAccessor())
		{
			// The public accessor implementation of an extension property is not flagged as special-name (there is no
			// property on the public class for it to be special-named against), so it would otherwise leak into the
			// results. It is treated like a regular get_/set_ accessor: excluded by default, opt-in via Accessors.
			return includedSpecialName.HasFlag(SpecialNameMembers.Accessors);
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
	/// <remarks>
	///     With <paramref name="forceDirect" /> a <paramref name="interfaceType" /> reached only through a base class or
	///     through another implemented interface is excluded. Because the compiler emits the full transitive interface set
	///     into the metadata, a redundantly declared interface (e.g. <c>class C : IDerived, IBase</c> where
	///     <c>IDerived : IBase</c>) cannot be distinguished from an inherited one and is therefore treated as not direct.
	/// </remarks>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <param name="interfaceType">The interface <see cref="Type" />.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="interfaceType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="interfaceType" /> to be implemented directly on the <paramref name="type" /> itself and not
	///     inherited from a base class or another implemented interface.
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
		if (forceDirect)
		{
			IEnumerable<Type> inherited = interfaces.SelectMany(@interface => @interface.GetInterfaces());
			if (type.BaseType != null)
			{
				inherited = inherited.Concat(type.BaseType.GetInterfaces());
			}

			interfaces = interfaces
				.Except(inherited)
				.ToArray();
		}

		return interfaces
			.Any(childInterface =>
			{
				if (interfaceType.IsGenericTypeDefinition)
				{
					return childInterface.IsGenericType &&
					       childInterface.GetGenericTypeDefinition() == interfaceType;
				}

				return childInterface == interfaceType;
			});
	}

	/// <summary>
	///     Determines whether the current <see cref="Type" /> inherits from the <paramref name="parentType" />
	///     (a base class) or implements it (an interface), anywhere in the inheritance tree.
	/// </summary>
	/// <remarks>
	///     Used for member-type assignability matching (e.g. <c>OfType</c>); for the base-class-only inheritance check
	///     used by type assertions see <see cref="InheritsFromClass" />.
	/// </remarks>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <param name="parentType">The parent <see cref="Type" />.</param>
	public static bool InheritsFrom(
		this Type type,
		Type parentType)
	{
		if (type == parentType)
		{
			return false;
		}

		Type currentType = type;

		while (currentType != typeof(object))
		{
			if (parentType.IsEqualTo(currentType))
			{
				return true;
			}

			if (currentType.Implements(parentType))
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
	///     Determines whether the current <see cref="Type" /> inherits from the base class <paramref name="baseClass" />.
	/// </summary>
	/// <remarks>
	///     Unlike <see cref="InheritsFrom" />, only the base-class chain is considered; implemented interfaces are ignored.
	/// </remarks>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <param name="baseClass">The base class to check inheritance from.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="baseClass" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="baseClass" /> to be the direct parent.
	/// </param>
	public static bool InheritsFromClass(
		this Type type,
		Type baseClass,
		bool forceDirect = false)
	{
		if (type == baseClass)
		{
			return false;
		}

		Type currentType = type;

		int level = 0;
		while (currentType != typeof(object))
		{
			if (baseClass.IsEqualTo(currentType))
			{
				return true;
			}

			if (forceDirect && level++ > 0)
			{
				return false;
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
	///     Throws an <see cref="ArgumentException" /> if the <paramref name="baseClass" /> is an interface,
	///     because inheritance checks only consider the base-class chain.
	/// </summary>
	public static Type EnsureIsClass(this Type baseClass)
	{
		if (baseClass.IsInterface)
		{
			throw new ArgumentException(
				$"The type to check inheritance from must be a class, but it was the interface {Formatter.Format(baseClass)}. Use 'Implements' to check for interface implementations.");
		}

		return baseClass;
	}

	/// <summary>
	///     Throws an <see cref="ArgumentException" /> if the <paramref name="interfaceType" /> is not an interface,
	///     because implementation checks only consider interfaces.
	/// </summary>
	public static Type EnsureIsInterface(this Type interfaceType)
	{
		if (!interfaceType.IsInterface)
		{
			throw new ArgumentException(
				$"The type to check implementation of must be an interface, but it was {Formatter.Format(interfaceType)}. Use 'InheritsFrom' to check for base-class inheritance.");
		}

		return interfaceType;
	}

	/// <summary>
	///     Throws an <see cref="ArgumentException" /> if the <paramref name="type" /> is an open generic type definition,
	///     because runtime assignability cannot be evaluated for open generics.
	/// </summary>
	public static Type EnsureIsNotOpenGeneric(this Type type)
	{
		if (type.ContainsGenericParameters)
		{
			throw new ArgumentException(
				$"The type to check assignability against must not be an open generic type definition, but it was {Formatter.Format(type)}. Use 'Implements' or 'InheritsFrom' for open generic type definitions.");
		}

		return type;
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
	///     Gets a value indicating whether the <see cref="Type" /> is a delegate
	///     (derives from <see cref="MulticastDelegate" />).
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	public static bool IsDelegate(this Type? type)
		=> type is not null && typeof(MulticastDelegate).IsAssignableFrom(type);

	/// <summary>
	///     Gets a value indicating whether the <see cref="Type" /> is an exception
	///     (derives from <see cref="Exception" />).
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	public static bool IsException(this Type? type)
		=> type is not null && typeof(Exception).IsAssignableFrom(type);

	/// <summary>
	///     Gets a value indicating whether the <see cref="Type" /> is an attribute
	///     (derives from <see cref="Attribute" />).
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	public static bool IsAttribute(this Type? type)
		=> type is not null && typeof(Attribute).IsAssignableFrom(type);

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
	///     Gets a value indicating whether the <see cref="Type" /> is instantiable, i.e. a concrete type that
	///     can be instantiated.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <remarks>
	///     A type is considered instantiable when it is neither abstract, static nor an interface (all of which have
	///     <see cref="Type.IsAbstract" /> set to <see langword="true" />) and is not an open generic type definition.
	/// </remarks>
	public static bool IsReallyInstantiable(this Type? type)
		=> type is { IsAbstract: false, IsGenericTypeDefinition: false, };

	/// <summary>
	///     Gets a value indicating whether the <see cref="Type" /> has an accessible parameterless (default) constructor.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <remarks>
	///     Value types always have a parameterless constructor. For all other types a <see langword="public" />
	///     parameterless constructor must be declared.
	/// </remarks>
	public static bool HasDefaultConstructor(this Type? type)
		=> type is not null &&
		   (type.IsValueType || type.GetConstructor(Type.EmptyTypes) is not null);

	/// <summary>
	///     Gets the <see cref="BindingFlags" /> used to look up operator methods, optionally including operators
	///     inherited from base types when <paramref name="inherit" /> is <see langword="true" />.
	/// </summary>
	/// <remarks>
	///     Operators are always <see langword="public" /> <see langword="static" /> methods. Inherited static members are
	///     only returned when <see cref="BindingFlags.FlattenHierarchy" /> is specified.
	/// </remarks>
	private static BindingFlags OperatorFlags(bool inherit)
		=> BindingFlags.Public | BindingFlags.Static | (inherit ? BindingFlags.FlattenHierarchy : 0);

	/// <summary>
	///     Gets a value indicating whether the <paramref name="type" /> declares the <paramref name="operator" />.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	/// <param name="inherit">
	///     <see langword="true" /> to also consider operators inherited from base types; otherwise,
	///     <see langword="false" /> (the default).
	/// </param>
	public static bool HasOperator(this Type? type, Operator @operator, bool inherit = false)
		=> type?.GetMethods(OperatorFlags(inherit)).Any(m => m.IsOperator(@operator)) == true;

	/// <summary>
	///     Gets a value indicating whether the <paramref name="type" /> declares the <paramref name="operator" /> with an
	///     overload that takes the <paramref name="operand" /> as one of its parameters.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <param name="operator">The <see cref="Operator" /> to look for.</param>
	/// <param name="operand">The operand type that one of the operator's parameters must match exactly.</param>
	/// <param name="inherit">
	///     <see langword="true" /> to also consider operators inherited from base types; otherwise,
	///     <see langword="false" /> (the default).
	/// </param>
	public static bool HasOperator(this Type? type, Operator @operator, Type operand, bool inherit = false)
		=> type?.GetMethods(OperatorFlags(inherit))
			.Any(m => m.IsOperator(@operator) &&
			          m.GetParameters().Any(p => p.ParameterType.IsEqualTo(operand))) == true;

	/// <summary>
	///     Gets the conversion operator on the <paramref name="type" /> that converts from <paramref name="source" /> to
	///     <paramref name="target" />, or <see langword="null" /> if no such operator is declared.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <param name="isImplicit">
	///     <see langword="true" /> to match an implicit (<c>op_Implicit</c>) conversion; <see langword="false" /> to match
	///     an explicit (<c>op_Explicit</c>) conversion.
	/// </param>
	/// <param name="source">The source type (the single parameter of the conversion).</param>
	/// <param name="target">The target type (the return type of the conversion).</param>
	/// <param name="inherit">
	///     <see langword="true" /> to also consider conversion operators inherited from base types; otherwise,
	///     <see langword="false" /> (the default).
	/// </param>
	public static MethodInfo? GetConversionOperator(
		this Type? type, bool isImplicit, Type source, Type target, bool inherit = false)
		=> type?.GetMethods(OperatorFlags(inherit))
			.FirstOrDefault(m =>
			{
				ParameterInfo[] parameters = m.GetParameters();
				return string.Equals(m.Name, OperatorNames.Conversion(isImplicit), StringComparison.Ordinal) &&
				       m.ReturnType == target &&
				       parameters.Length == 1 && parameters[0].ParameterType == source;
			});

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
