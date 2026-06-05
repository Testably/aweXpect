using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Options;

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
		=> type?.Namespace is { } @namespace && NamespaceMatches(@namespace, expected, includeSubNamespaces: true);

	/// <summary>
	///     Checks whether the <paramref name="actualNamespace" /> matches the <paramref name="expectedNamespace" />,
	///     optionally including its sub-namespaces.
	/// </summary>
	/// <remarks>
	///     The comparison is ordinal and case-sensitive. With <paramref name="includeSubNamespaces" />, <c>Foo.Bar</c>
	///     also matches <c>Foo.Bar.Baz</c>, but never <c>Foo.BarBaz</c> (the next character must be a separator). A
	///     <see langword="null" /> <paramref name="actualNamespace" /> (global namespace) matches only the empty
	///     string, which targets exactly the global namespace (without sub-namespace semantics).
	/// </remarks>
	internal static bool NamespaceMatches(string? actualNamespace, string expectedNamespace, bool includeSubNamespaces)
	{
		if (actualNamespace is null)
		{
			return expectedNamespace.Length == 0;
		}

		if (string.Equals(actualNamespace, expectedNamespace, StringComparison.Ordinal))
		{
			return true;
		}

		return includeSubNamespaces &&
		       actualNamespace.Length > expectedNamespace.Length &&
		       actualNamespace[expectedNamespace.Length] == '.' &&
		       actualNamespace.StartsWith(expectedNamespace, StringComparison.Ordinal);
	}

	/// <summary>
	///     Checks whether the <paramref name="dependency" /> references the <paramref name="target" /> type.
	/// </summary>
	/// <remarks>
	///     A constructed generic target (e.g. <c>List&lt;Foo&gt;</c>) only matches that exact construction,
	///     while a generic type definition target (<c>List&lt;&gt;</c>) matches any construction of it.
	///     Array/by-ref/pointer wrappers are stripped from dependencies at collection time, so the target is
	///     unwrapped symmetrically: <c>typeof(Foo[])</c> matches like <c>typeof(Foo)</c>.
	/// </remarks>
	internal static bool MatchesType(Type dependency, Type target)
	{
		target = StripElementTypes(target);

		if (dependency == target)
		{
			return true;
		}

		return target.IsGenericTypeDefinition &&
		       dependency.IsGenericType &&
		       dependency.GetGenericTypeDefinition() == target;
	}

	/// <summary>
	///     Strips array/by-ref/pointer wrappers from the <paramref name="type" />, returning the innermost
	///     element type.
	/// </summary>
	/// <remarks>
	///     Shared between dependency collection (<c>SignatureDependencyCollector.Unwrap</c>) and target matching
	///     (<see cref="MatchesType" />), so that the two sides of the documented symmetry cannot drift apart.
	/// </remarks>
	private static Type StripElementTypes(Type type)
	{
		while (type.HasElementType && type.GetElementType() is { } elementType)
		{
			type = elementType;
		}

		return type;
	}

	/// <summary>
	///     The display text for the global namespace in expectation and failure messages.
	/// </summary>
	internal const string GlobalNamespaceDisplay = "<global namespace>";

	/// <summary>
	///     Caches the simple assembly name per <see cref="Assembly" />: <see cref="Assembly.GetName()" /> parses a
	///     fresh <see cref="AssemblyName" /> on every call, but the framework check runs per dependency of every
	///     asserted type (outside the <see cref="ResolvedDependencies" /> memoization), while the dependencies
	///     cluster into a handful of distinct assemblies whose name cannot change.
	/// </summary>
	private static readonly ConditionalWeakTable<Assembly, string> AssemblyNames = new();

	/// <summary>
	///     Determines whether the <paramref name="type" /> belongs to a framework assembly, i.e. its assembly name
	///     matches one of the <paramref name="excludedPrefixes" /> at a name-segment boundary
	///     (see <see cref="AssemblyHelpers.IsExcludedAssemblyName" />).
	/// </summary>
	private static bool IsFrameworkDependency(this Type type, string[] excludedPrefixes)
	{
		string assemblyName;
		try
		{
			assemblyName = AssemblyNames.GetValue(type.Assembly, static assembly => assembly.GetName().Name ?? "");
		}
		catch (Exception exception) when (IsUnresolvable(exception))
		{
			return false;
		}

		return assemblyName.IsExcludedAssemblyName(excludedPrefixes);
	}

	/// <summary>
	///     Collects the namespaces of the <paramref name="type" />'s dependencies that are not allowed by the
	///     <paramref name="allowed" /> namespaces, the type's own namespace, or the framework rule.
	/// </summary>
	/// <remarks>
	///     Framework dependencies are ignored. A dependency in the type's own namespace (and, unless
	///     <see cref="NamespaceDependencyOptions.ExcludeSubNamespaces" />, its sub-namespaces) is implicitly allowed.
	///     The global namespace is reported as <c>&lt;global namespace&gt;</c> and can be allowed by specifying an
	///     empty string. The result is de-duplicated.
	/// </remarks>
	internal static IReadOnlyList<string> GetDependencyNamespaceViolations(
		this Type type, NamespaceDependencyOptions allowed)
	{
		string? ownNamespace = type.Namespace;
		string[] excludedPrefixes = Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get();
		List<string> violations = [];
		HashSet<string> seen = new(StringComparer.Ordinal);
		foreach (Type dependency in type.ResolveDependencies())
		{
			if (!IsDependencyViolation(dependency, ownNamespace, allowed, excludedPrefixes))
			{
				continue;
			}

			string display = dependency.Namespace ?? GlobalNamespaceDisplay;
			if (seen.Add(display))
			{
				violations.Add(display);
			}
		}

		violations.Sort(StringComparer.Ordinal);
		return violations;
	}

	/// <summary>
	///     Checks whether the <paramref name="type" /> has at least one dependency namespace violation, stopping at
	///     the first one.
	/// </summary>
	/// <remarks>
	///     Same rules as <see cref="GetDependencyNamespaceViolations" />, for callers (like filters) that only need
	///     a verdict and not the violation list.
	/// </remarks>
	internal static bool HasDependencyNamespaceViolations(this Type type, NamespaceDependencyOptions allowed)
	{
		string? ownNamespace = type.Namespace;
		string[] excludedPrefixes = Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get();
		return type.ResolveDependencies()
			.Any(dependency => IsDependencyViolation(dependency, ownNamespace, allowed, excludedPrefixes));
	}

	private static bool IsDependencyViolation(
		Type dependency, string? ownNamespace, NamespaceDependencyOptions allowed, string[] excludedPrefixes)
	{
		if (dependency.IsFrameworkDependency(excludedPrefixes))
		{
			return false;
		}

		string? dependencyNamespace = dependency.Namespace;
		return !IsOwnNamespace(dependencyNamespace, ownNamespace, allowed.IncludeOwnSubNamespaces) &&
		       !allowed.Matches(dependencyNamespace);
	}

	private static bool IsOwnNamespace(string? dependencyNamespace, string? ownNamespace, bool includeSubNamespaces)
		=> ownNamespace is null
			? dependencyNamespace is null
			: NamespaceMatches(dependencyNamespace, ownNamespace, includeSubNamespaces);

	/// <summary>
	///     Caches the resolved dependencies per <see cref="Type" />: the signature surface of a type cannot change
	///     at runtime and the raw dependency set is independent of any customization (the
	///     <c>ExcludedAssemblyPrefixes</c> are applied on top of it by the callers, and a customized
	///     <c>ExcludedAttributeTypes</c> set bypasses the cache), so chained filters and
	///     assertions do not have to repeat the reflection walk. The weak table does not pin types from
	///     collectible <c>AssemblyLoadContext</c>s.
	/// </summary>
	private static readonly ConditionalWeakTable<Type, Type[]> ResolvedDependencies = new();

	/// <summary>
	///     Resolves the dependencies of the <paramref name="type" /> through which all assertions and filters go.
	/// </summary>
	/// <remarks>
	///     This is a seam: it currently materializes the (unwrapped and de-duplicated)
	///     <see cref="GetSignatureDependencies" /> with per-type memoization, but is the single place a later,
	///     configurable resolver can hook into. Callers must not mutate the returned array.
	/// </remarks>
	internal static Type[] ResolveDependencies(this Type type)
	{
		string[] excludedAttributeTypes = Customize.aweXpect.Reflection().ExcludedAttributeTypes().Get();
		if (excludedAttributeTypes.Length > 0)
		{
			// A customized attribute exclusion changes the dependency set, so it must not be baked into the
			// customization-independent cache; the (rare) customized path recomputes instead.
			return type.GetSignatureDependencies(excludedAttributeTypes).ToArray();
		}

		return ResolvedDependencies.GetValue(type, static t => t.GetSignatureDependencies([]).ToArray());
	}

	/// <summary>
	///     Collects the types referenced in the declared signature surface of the <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     Considers the base type, implemented interfaces, generic arguments and parameter constraints, the types of
	///     fields, properties (and indexer parameters), events, method return/parameter/generic types, constructor
	///     parameters and the types of attributes applied to the type and its members (read via
	///     <see cref="CustomAttributeData" /> without instantiating them). Compiler-generated members are excluded, as
	///     are purely synthetic references that the author never wrote: the implicit <see cref="object" /> /
	///     <see cref="ValueType" /> / <see cref="System.Enum" /> base type, the attributes the compiler emits
	///     onto authored code (nullability metadata, required members, async/iterator state machines, ...) and
	///     the runtime-supplied delegate infrastructure (for delegate types only the <c>Invoke</c> signature
	///     counts).
	///     Array/by-ref/pointer element types and generic type arguments are unwrapped recursively, open generic
	///     parameters are skipped (their constraints are kept), and the result is de-duplicated.
	///     <para />
	///     This is signature-level only: references that appear merely in method bodies (e.g. <c>new Infra.Foo()</c>,
	///     static calls or locals) are not detected. Function-pointer signatures (<c>delegate*&lt;…&gt;</c>) are
	///     not decomposed either: the parameter and return types inside them are invisible to dependency
	///     assertions (the reflection APIs to traverse them only exist on .NET 8+). Nested types are not
	///     traversed: they are separate types with their own dependency surface (the type collections
	///     enumerate them individually).
	/// </remarks>
	internal static IEnumerable<Type> GetSignatureDependencies(this Type type, string[] excludedAttributeTypes)
	{
		SignatureDependencyCollector collector = new(excludedAttributeTypes);

		if (type.IsDelegate())
		{
			// A delegate's runtime infrastructure - the MulticastDelegate base, the (object, IntPtr)
			// constructor and BeginInvoke/EndInvoke - is not authored; only the Invoke signature carries
			// the parameter and return types the author wrote.
			collector.AddGenericArguments(Safe(type.GetGenericArguments));
			collector.AddAttributes(type);
			collector.AddDelegateInvoke(type);
			return collector.Build(type);
		}

		// The implicit object/ValueType/Enum base type is not a dependency the author wrote, so it is skipped to
		// avoid every type trivially "depending on" System.
		if (SafeOrNull(() => type.BaseType) is { } baseType &&
		    baseType != typeof(object) && baseType != typeof(ValueType) && baseType != typeof(Enum))
		{
			collector.Add(baseType);
		}
		collector.AddAll(GetDeclaredInterfaces(type));
		collector.AddGenericArguments(Safe(type.GetGenericArguments));
		collector.AddAttributes(type);
		collector.AddFields(type);
		collector.AddProperties(type);
		collector.AddEvents(type);
		collector.AddMethods(type);
		collector.AddConstructors(type);
		return collector.Build(type);
	}

	/// <summary>
	///     Returns only the interfaces the author wrote on the <paramref name="type" /> itself.
	/// </summary>
	/// <remarks>
	///     <see cref="Type.GetInterfaces" /> returns the transitive closure, so interfaces inherited from the base
	///     type or from other interfaces are subtracted (an interface that is both inherited and explicitly
	///     re-declared cannot be distinguished in metadata and is treated as inherited). The compiler-synthesized
	///     <see cref="IEquatable{T}" /> implementation of records is also skipped.
	/// </remarks>
	private static IEnumerable<Type> GetDeclaredInterfaces(Type type)
	{
		Type[] interfaces = Safe(type.GetInterfaces);
		HashSet<Type> inherited = [];
		if (SafeOrNull(() => type.BaseType) is { } baseType)
		{
			inherited.UnionWith(Safe(baseType.GetInterfaces));
		}

		foreach (Type @interface in interfaces)
		{
			inherited.UnionWith(Safe(@interface.GetInterfaces));
		}

		bool isRecord = type.IsRecordClass() || type.IsRecordStruct();
		foreach (Type @interface in interfaces)
		{
			if (inherited.Contains(@interface) ||
			    (isRecord && IsSynthesizedEquatable(@interface, type)))
			{
				continue;
			}

			yield return @interface;
		}
	}

	private static bool IsSynthesizedEquatable(Type @interface, Type type)
	{
		if (!@interface.IsGenericType || @interface.GetGenericTypeDefinition() != typeof(IEquatable<>))
		{
			return false;
		}

		Type argument = @interface.GetGenericArguments()[0];
		return argument == type ||
		       (argument.IsGenericType && type.IsGenericTypeDefinition &&
		        argument.GetGenericTypeDefinition() == type);
	}

	/// <summary>
	///     Accumulates the (unwrapped, de-duplicated) signature dependencies of a type, one member kind at a time.
	/// </summary>
	/// <remarks>
	///     Extracted from <see cref="GetSignatureDependencies" /> so that each member kind is collected in its own
	///     small method instead of one large one.
	/// </remarks>
	private sealed class SignatureDependencyCollector(string[] excludedAttributeTypes)
	{
		private const BindingFlags Flags = BindingFlags.Public |
		                                    BindingFlags.NonPublic |
		                                    BindingFlags.Instance |
		                                    BindingFlags.Static |
		                                    BindingFlags.DeclaredOnly;

		private readonly HashSet<Type> _dependencies = [];

		public void Add(Type? candidate)
		{
			foreach (Type unwrapped in Unwrap(candidate))
			{
				_dependencies.Add(unwrapped);
			}
		}

		public void AddAll(IEnumerable<Type> candidates)
		{
			foreach (Type candidate in candidates)
			{
				Add(candidate);
			}
		}

		public void AddGenericArguments(IEnumerable<Type> arguments)
		{
			foreach (Type argument in arguments)
			{
				if (argument.IsGenericParameter)
				{
					// `where T : struct` / `where T : unmanaged` compile into a System.ValueType constraint
					// the author can never write directly (CS0702); an authored `where T : Enum` (C# 7.3+)
					// still counts.
					AddAll(Safe(argument.GetGenericParameterConstraints)
						.Where(constraint => constraint != typeof(object) && constraint != typeof(ValueType)));
				}
				else
				{
					Add(argument);
				}
			}
		}

		public void AddAttributes(MemberInfo member)
		{
			IEnumerable<CustomAttributeData> attributes = SafeAttributes(member);

			// For required members, the compiler pairs an [Obsolete] carrying a fixed marker message with
			// [CompilerFeatureRequired(nameof(RequiredMembers))] on the constructor; only that compiler-emitted
			// [Obsolete] is skipped, an authored one (even on the same constructor) still counts.
			bool hasCompilerFeatureRequired = attributes.Any(data
				=> data.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute" &&
				   IsRequiredMembersFeature(data));

			// The compiler emits [DefaultMember("Item")] onto every type declaring an indexer; an authored
			// [DefaultMember] cannot coexist with an indexer (CS0646), so it is only skipped in that case.
			bool hasIndexer = member is Type type && HasIndexer(type);

			bool skipDebuggerStepThrough = IsCompilerEmittedDebuggerStepThrough(member, attributes);

			AddAttributes(attributes, data
				=> (hasCompilerFeatureRequired && IsRequiredMembersObsolete(data)) ||
				   (hasIndexer && data.AttributeType.FullName == "System.Reflection.DefaultMemberAttribute") ||
				   (skipDebuggerStepThrough &&
				    data.AttributeType.FullName == "System.Diagnostics.DebuggerStepThroughAttribute"));
		}

		public void AddAttributes(ParameterInfo parameter)
			=> AddAttributes(SafeAttributes(parameter), null);

		private void AddAttributes(IEnumerable<CustomAttributeData> attributes,
			Func<CustomAttributeData, bool>? isCompilerEmittedPairing)
		{
			// Neither compiler-generated/embedded attributes nor the BCL attributes the compiler emits onto
			// authored code (nullability, required members, async/iterator state machines, ...) are
			// dependencies the author wrote.
			foreach (CustomAttributeData attribute in attributes
				         .Where(data => !data.AttributeType.IsCompilerGenerated() &&
				                        !IsCompilerEmittedAttribute(data.AttributeType) &&
				                        isCompilerEmittedPairing?.Invoke(data) != true))
			{
				Add(attribute.AttributeType);

				// A typeof(...) used as a constructor or named argument is a real signature dependency
				// (e.g. [JsonConverter(typeof(FooConverter))] depends on FooConverter).
				foreach (CustomAttributeTypedArgument argument in SafeAttributeArguments(attribute))
				{
					AddAttributeArgument(argument);
				}
			}
		}

		/// <summary>
		///     Determines whether a <c>[DebuggerStepThrough]</c> on the <paramref name="member" /> is
		///     compiler-emitted and must therefore be skipped.
		/// </summary>
		/// <remarks>
		///     In Debug builds the compiler emits <c>[DebuggerStepThrough]</c> onto the kickoff method of
		///     authored async methods (next to the state machine attribute); in Release builds it emits none.
		///     An authored <c>[DebuggerStepThrough]</c> therefore shows up as a duplicate in Debug builds and
		///     as the only occurrence in Release builds — only a single occurrence in a Debug-built assembly
		///     is (indistinguishably) compiler-emitted. An authored <c>[DebuggerStepThrough]</c> on a
		///     non-async member always counts.
		/// </remarks>
		private static bool IsCompilerEmittedDebuggerStepThrough(MemberInfo member,
			IEnumerable<CustomAttributeData> attributes)
		{
			bool hasAsyncStateMachine = attributes.Any(data => data.AttributeType.FullName
				is "System.Runtime.CompilerServices.AsyncStateMachineAttribute"
				or "System.Runtime.CompilerServices.AsyncIteratorStateMachineAttribute");
			if (!hasAsyncStateMachine)
			{
				return false;
			}

			int debuggerStepThroughCount = attributes.Count(data
				=> data.AttributeType.FullName == "System.Diagnostics.DebuggerStepThroughAttribute");
			return debuggerStepThroughCount == 1 && IsDebugBuilt(SafeOrNull(() => member.Module.Assembly));
		}

		/// <summary>
		///     Caches the Debug-built verdict per <see cref="Assembly" />: in a Debug-built assembly the check
		///     runs for every authored async method (each kickoff carries a compiler-emitted
		///     <c>[DebuggerStepThrough]</c>), but the build configuration of an assembly cannot change.
		/// </summary>
		private static readonly ConditionalWeakTable<Assembly, object> DebugBuilt = new();

		/// <summary>
		///     Checks whether the <paramref name="assembly" /> was compiled in Debug configuration, i.e. carries
		///     a <c>[Debuggable]</c> attribute with the
		///     <see cref="DebuggableAttribute.DebuggingModes.DisableOptimizations" /> flag.
		/// </summary>
		/// <remarks>
		///     When this cannot be determined, a Debug build is assumed, so that the (then ambiguous) single
		///     <c>[DebuggerStepThrough]</c> keeps being treated as compiler-emitted.
		/// </remarks>
		private static bool IsDebugBuilt(Assembly? assembly)
		{
			if (assembly is null)
			{
				return true;
			}

			return (bool)DebugBuilt.GetValue(assembly, static a => DetermineIsDebugBuilt(a));
		}

		private static bool DetermineIsDebugBuilt(Assembly assembly)
		{
			try
			{
				foreach (CustomAttributeData data in assembly.GetCustomAttributesData())
				{
					if (data.AttributeType.FullName != "System.Diagnostics.DebuggableAttribute")
					{
						continue;
					}

					IList<CustomAttributeTypedArgument> arguments = data.ConstructorArguments;
					if (arguments.Count == 1 && arguments[0].Value is int debuggingModes)
					{
						return (debuggingModes &
						        (int)DebuggableAttribute.DebuggingModes.DisableOptimizations) != 0;
					}

					if (arguments.Count == 2 && arguments[1].Value is bool isJitOptimizerDisabled)
					{
						return isJitOptimizerDisabled;
					}
				}

				return false;
			}
			catch (Exception exception) when (IsUnresolvable(exception))
			{
				return true;
			}
		}

		public void AddFields(Type type)
		{
			// Special-name fields are runtime-supplied, not authored: most importantly every enum's `value__`
			// instance field (typed as the underlying integral type), which would otherwise make every enum
			// trivially "depend on" System.
			foreach (FieldInfo field in Safe(() => type.GetFields(Flags))
				         .Where(m => !m.IsCompilerGenerated() && !m.IsSpecialName))
			{
				// Member signature types are resolved lazily on first access and can throw when the defining
				// assembly is missing, so each access is guarded individually to skip only the unresolvable member.
				AddSafe(() => field.FieldType);
				AddAttributes(field);
			}
		}

		public void AddProperties(Type type)
		{
			foreach (PropertyInfo property in Safe(() => type.GetProperties(Flags)).Where(m => !m.IsCompilerGenerated()))
			{
				AddSafe(() => property.PropertyType);
				AddParameters(Safe(property.GetIndexParameters));
				AddAttributes(property);
			}
		}

		public void AddEvents(Type type)
		{
			foreach (EventInfo @event in Safe(() => type.GetEvents(Flags)).Where(m => !m.IsCompilerGenerated()))
			{
				AddSafe(() => @event.EventHandlerType);
				AddAttributes(@event);
			}
		}

		public void AddMethods(Type type)
		{
			foreach (MethodInfo method in Safe(() => type.GetMethods(Flags)).Where(m => !m.IsCompilerGenerated()))
			{
				AddSafe(() => method.ReturnType);
				AddReturnValueAttributes(method);
				AddParameters(Safe(method.GetParameters));
				AddGenericArguments(Safe(method.GetGenericArguments));
				AddAttributes(method);
			}
		}

		public void AddConstructors(Type type)
		{
			foreach (ConstructorInfo constructor in Safe(() => type.GetConstructors(Flags))
				         .Where(m => !m.IsCompilerGenerated()))
			{
				AddParameters(Safe(constructor.GetParameters));
				AddAttributes(constructor);
			}
		}

		public void AddDelegateInvoke(Type type)
		{
			if (SafeOrNull(() => type.GetMethod("Invoke", Flags)) is { } invoke)
			{
				AddSafe(() => invoke.ReturnType);
				AddReturnValueAttributes(invoke);
				AddParameters(Safe(invoke.GetParameters));
				AddAttributes(invoke);
			}
		}

		private void AddParameters(IEnumerable<ParameterInfo> parameters)
		{
			foreach (ParameterInfo parameter in parameters)
			{
				// Member signature types are resolved lazily on first access and can throw when the defining
				// assembly is missing, so each access is guarded individually to skip only the unresolvable member.
				AddSafe(() => parameter.ParameterType);

				// Attributes applied to a parameter (e.g. [Layer1.Target] int value) are authored signature
				// text just like the parameter type itself.
				AddAttributes(parameter);
			}
		}

		private void AddReturnValueAttributes(MethodInfo method)
		{
			// [return: ...] attributes are authored signature text just like parameter attributes.
			if (SafeOrNull(() => method.ReturnParameter) is { } returnParameter)
			{
				AddAttributes(returnParameter);
			}
		}

		private void AddSafe(Func<Type?> get)
		{
			try
			{
				Add(get());
			}
			catch (Exception exception) when (IsUnresolvable(exception))
			{
				// The member's signature type could not be resolved, for example when its assembly is not
				// deployed, so this member is skipped, consistent with GetDeclaredFields/Methods/Properties.
			}
		}

		public HashSet<Type> Build(Type type)
		{
			_dependencies.Remove(type);
			return _dependencies;
		}

		/// <summary>
		///     Attributes the C# compiler emits onto authored code, which are therefore not dependencies the
		///     author wrote: nullability metadata, required members, async/iterator state machines,
		///     covariant-return overrides, extension methods, readonly/ref structs, fixed buffers, tuple names,
		///     <c>params</c> arrays, <c>dynamic</c> and <c>decimal</c> constants, <c>ref readonly</c> parameters
		///     and the parameter pseudo-attributes.
		/// </summary>
		/// <remarks>
		///     Only attribute types the author can never legally apply in the same situation belong here.
		///     Attributes that can also be authored ([Obsolete], [DebuggerStepThrough], [DefaultMember]) are
		///     instead skipped conditionally in <see cref="AddAttributes(MemberInfo)" />, based on the
		///     co-occurrence pattern the compiler produces.
		///     <para />
		///     The parameter pseudo-attributes ([In], [Out], [Optional]) are metadata flags, not attribute
		///     blobs: reflection surfaces the flags the compiler sets for <c>in</c>/<c>ref readonly</c>,
		///     <c>out</c> and optional parameters as these attributes, indistinguishably from authored ones,
		///     so they never count.
		/// </remarks>
		private static readonly HashSet<string> CompilerEmittedAttributes = new(StringComparer.Ordinal)
		{
			"System.Runtime.CompilerServices.NullableAttribute",
			"System.Runtime.CompilerServices.NullableContextAttribute",
			"System.Runtime.CompilerServices.RequiredMemberAttribute",
			"System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute",
			"System.Runtime.CompilerServices.AsyncStateMachineAttribute",
			"System.Runtime.CompilerServices.IteratorStateMachineAttribute",
			"System.Runtime.CompilerServices.AsyncIteratorStateMachineAttribute",
			"System.Runtime.CompilerServices.PreserveBaseOverridesAttribute",
			"System.Runtime.CompilerServices.ExtensionAttribute",
			"System.Runtime.CompilerServices.IsReadOnlyAttribute",
			"System.Runtime.CompilerServices.IsByRefLikeAttribute",
			"System.Runtime.CompilerServices.IsUnmanagedAttribute",
			"System.Runtime.CompilerServices.RequiresLocationAttribute",
			"System.Runtime.CompilerServices.ScopedRefAttribute",
			"System.Runtime.CompilerServices.ParamCollectionAttribute",
			"System.Runtime.CompilerServices.TupleElementNamesAttribute",
			"System.Runtime.CompilerServices.DynamicAttribute",
			"System.Runtime.CompilerServices.DecimalConstantAttribute",
			"System.Runtime.CompilerServices.FixedBufferAttribute",
			"System.ParamArrayAttribute",
			"System.Runtime.InteropServices.InAttribute",
			"System.Runtime.InteropServices.OutAttribute",
			"System.Runtime.InteropServices.OptionalAttribute",
		};

		private bool IsCompilerEmittedAttribute(Type attributeType)
			=> attributeType.FullName is { } fullName &&
			   (CompilerEmittedAttributes.Contains(fullName) ||
			    excludedAttributeTypes.Contains(fullName, StringComparer.Ordinal));

		/// <summary>
		///     The marker message Roslyn puts into the <see cref="ObsoleteAttribute" /> it emits (next to
		///     <c>[CompilerFeatureRequired]</c>) onto constructors of types with required members.
		/// </summary>
		private const string RequiredMembersObsoleteMessage =
			"Constructors of types with required members are not supported in this version of your compiler.";

		private static bool IsRequiredMembersObsolete(CustomAttributeData data)
		{
			if (data.AttributeType != typeof(ObsoleteAttribute))
			{
				return false;
			}

			try
			{
				// The compiler emits [Obsolete(message, error: true)]; requiring the error flag keeps an
				// authored [Obsolete] counting even if it should repeat the marker message verbatim.
				return data.ConstructorArguments.Count == 2 &&
				       data.ConstructorArguments[0].Value is RequiredMembersObsoleteMessage &&
				       data.ConstructorArguments[1].Value is true;
			}
			catch (Exception exception) when (IsUnresolvable(exception))
			{
				// When the arguments cannot be resolved, the compiler-emitted pairing is assumed.
				return true;
			}
		}

		/// <summary>
		///     Checks whether the <c>[CompilerFeatureRequired]</c> names the <c>RequiredMembers</c> feature, so
		///     that the gate for the paired <c>[Obsolete]</c> does not react to other compiler features.
		/// </summary>
		private static bool IsRequiredMembersFeature(CustomAttributeData data)
		{
			try
			{
				return data.ConstructorArguments.Count == 1 &&
				       data.ConstructorArguments[0].Value is "RequiredMembers";
			}
			catch (Exception exception) when (IsUnresolvable(exception))
			{
				// When the arguments cannot be resolved, the compiler-emitted pairing is assumed.
				return true;
			}
		}

		private static bool HasIndexer(Type type)
			=> Safe(() => type.GetProperties(Flags))
				.Any(property => Safe(property.GetIndexParameters).Length > 0);

		private void AddAttributeArgument(CustomAttributeTypedArgument argument)
		{
			if (argument.Value is Type typeArgument)
			{
				// A typeof(...) referencing a compiler-generated type (e.g. the state machine in
				// [AsyncStateMachine(typeof(<M>d__0))]) is not a dependency the author wrote.
				if (!typeArgument.IsCompilerGenerated())
				{
					Add(typeArgument);
				}
			}
			else if (argument.Value is IReadOnlyList<CustomAttributeTypedArgument> arrayArgument)
			{
				foreach (CustomAttributeTypedArgument element in arrayArgument)
				{
					AddAttributeArgument(element);
				}
			}
			else if (argument.ArgumentType.IsEnum)
			{
				// An enum constant in an attribute application (e.g. [Configured(Severity.High)]) is a
				// verbatim authored reference to the enum type, even though its value is boxed as the
				// underlying integral type.
				Add(argument.ArgumentType);
			}
		}

		/// <summary>
		///     Unwraps the <paramref name="type" />: array/by-ref/pointer element types and generic type arguments are
		///     flattened and open generic parameters are skipped. Constructed generic types are kept as written
		///     (e.g. <c>List&lt;Foo&gt;</c>), so that they can be matched exactly; their generic type arguments are
		///     additionally contributed as separate dependencies.
		/// </summary>
		/// <remarks>
		///     Known limitation: function-pointer types (<c>delegate*&lt;…&gt;</c>) are yielded as-is; the
		///     parameter and return types inside their signature are not collected, because
		///     <c>Type.GetFunctionPointerParameterTypes()</c> only exists on .NET 8+ and the unwrap must behave
		///     identically on all target frameworks.
		/// </remarks>
		private static IEnumerable<Type> Unwrap(Type? type)
		{
			if (type is null)
			{
				yield break;
			}

			type = StripElementTypes(type);

			if (type.IsGenericParameter)
			{
				yield break;
			}

			if (type.IsGenericType)
			{
				yield return type;
				foreach (Type argument in Safe(type.GetGenericArguments))
				{
					foreach (Type unwrapped in Unwrap(argument))
					{
						yield return unwrapped;
					}
				}
			}
			else
			{
				yield return type;
			}
		}

		private static IEnumerable<CustomAttributeData> SafeAttributes(MemberInfo member)
		{
			try
			{
				return member.GetCustomAttributesData();
			}
			catch (Exception exception) when (IsUnresolvable(exception))
			{
				return [];
			}
		}

		private static IEnumerable<CustomAttributeData> SafeAttributes(ParameterInfo parameter)
		{
			try
			{
				return parameter.GetCustomAttributesData();
			}
			catch (Exception exception) when (IsUnresolvable(exception))
			{
				return [];
			}
		}

		private static List<CustomAttributeTypedArgument> SafeAttributeArguments(CustomAttributeData attribute)
		{
			try
			{
				List<CustomAttributeTypedArgument> arguments = [..attribute.ConstructorArguments,];
				foreach (CustomAttributeNamedArgument namedArgument in attribute.NamedArguments)
				{
					arguments.Add(namedArgument.TypedValue);
				}

				return arguments;
			}
			catch (Exception exception) when (IsUnresolvable(exception))
			{
				return [];
			}
		}
	}

	/// <summary>
	///     Checks whether the <paramref name="exception" /> marks a member, attribute or assembly as unresolvable,
	///     so that the element is skipped instead of failing the whole assertion: the type-load family (missing or
	///     unloadable assemblies) and <see cref="CustomAttributeFormatException" />, which custom attribute blob
	///     parsing throws (mostly on .NET Framework) for malformed or exotic blobs.
	/// </summary>
	private static bool IsUnresolvable(Exception exception)
		=> exception
			is TypeLoadException
			or FileNotFoundException
			or FileLoadException
			or BadImageFormatException
			or CustomAttributeFormatException;

	private static T[] Safe<T>(Func<T[]> get)
	{
		try
		{
			return get();
		}
		catch (Exception exception) when (IsUnresolvable(exception))
		{
			return [];
		}
	}

	private static T? SafeOrNull<T>(Func<T?> get) where T : class
	{
		try
		{
			return get();
		}
		catch (Exception exception) when (IsUnresolvable(exception))
		{
			return null;
		}
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
