using System;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Extension properties for <see cref="PropertyInfo" />.
/// </summary>
internal static class PropertyInfoHelpers
{
	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> has the specified <paramref name="accessModifiers" />.
	/// </summary>
	/// <param name="propertyInfo">The <see cref="PropertyInfo" /> which is checked to have the attribute.</param>
	/// <param name="accessModifiers">
	///     The <see cref="AccessModifiers" />.
	///     <para />
	///     Supports specifying multiple <see cref="AccessModifiers" />.
	/// </param>
	public static bool HasAccessModifier(
		this PropertyInfo? propertyInfo,
		AccessModifiers accessModifiers)
	{
		if (propertyInfo == null)
		{
			return false;
		}

		return propertyInfo.GetMethod.HasAccessModifier(accessModifiers) ||
		       propertyInfo.SetMethod.HasAccessModifier(accessModifiers);
	}

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> has an attribute which satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <typeparam name="TAttribute">The type of the <see cref="Attribute" />.</typeparam>
	/// <param name="propertyInfo">The <see cref="PropertyInfo" /> which is checked to have the attribute.</param>
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
		this PropertyInfo propertyInfo,
		Func<TAttribute, bool>? predicate = null,
		bool inherit = true)
		where TAttribute : Attribute
	{
		object? attribute = Attribute.GetCustomAttributes(propertyInfo, typeof(TAttribute), inherit)
			.FirstOrDefault();
		if (attribute is TAttribute castedAttribute)
		{
			return predicate?.Invoke(castedAttribute) ?? true;
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> has an attribute which satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <param name="propertyInfo">The <see cref="PropertyInfo" /> which is checked to have the attribute.</param>
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
		this PropertyInfo? propertyInfo,
		Type attributeType,
		Func<Attribute, bool>? predicate = null,
		bool inherit = true)
	{
		object? attribute = propertyInfo?.GetCustomAttributes(attributeType, inherit)
			.FirstOrDefault();
		if (attribute is Attribute attributeValue)
		{
			return predicate?.Invoke(attributeValue) ?? true;
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is abstract (based on its accessor methods).
	/// </summary>
	/// <remarks>
	///     A property is considered abstract if its getter or setter method is abstract.
	/// </remarks>
	public static bool IsReallyAbstract(this PropertyInfo? propertyInfo)
		=> propertyInfo?.GetMethod?.IsAbstract == true ||
		   propertyInfo?.SetMethod?.IsAbstract == true;

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is sealed (based on its accessor methods).
	/// </summary>
	/// <param name="propertyInfo">The <see cref="PropertyInfo" /> to check.</param>
	/// <remarks>
	///     A property is considered sealed if its getter or setter method is sealed.
	/// </remarks>
	public static bool IsReallySealed(this PropertyInfo? propertyInfo)
		=> propertyInfo?.GetMethod?.IsFinal == true ||
		   propertyInfo?.SetMethod?.IsFinal == true;

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is virtual (based on its accessor methods).
	/// </summary>
	/// <remarks>
	///     A property is considered virtual if its getter or setter method is virtual.
	/// </remarks>
	public static bool IsReallyVirtual(this PropertyInfo? propertyInfo)
		=> propertyInfo?.GetMethod?.IsVirtual == true ||
		   propertyInfo?.SetMethod?.IsVirtual == true;

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> overrides a base class property (based on its accessor methods).
	/// </summary>
	/// <remarks>
	///     A property is considered an override if its getter or setter method overrides a base class method.
	/// </remarks>
	public static bool IsOverride(this PropertyInfo? propertyInfo)
		=> propertyInfo?.GetMethod.IsOverride() == true ||
		   propertyInfo?.SetMethod.IsOverride() == true;

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is static.
	/// </summary>
	/// <remarks>
	///     A property is considered static if its getter or setter method is static.
	/// </remarks>
	public static bool IsReallyStatic(
		this PropertyInfo? propertyInfo)
		=> propertyInfo?.GetMethod?.IsStatic == true ||
		   propertyInfo?.SetMethod?.IsStatic == true;

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is required (marked with the <c>required</c> modifier).
	/// </summary>
	/// <remarks>
	///     A property is considered required if it carries the
	///     <c>System.Runtime.CompilerServices.RequiredMemberAttribute</c>.
	/// </remarks>
	public static bool IsRequired(this PropertyInfo? propertyInfo)
		=> propertyInfo != null && propertyInfo.GetCustomAttributes(true)
			.Any(attribute => attribute.GetType().FullName == "System.Runtime.CompilerServices.RequiredMemberAttribute");

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is an indexer (has index parameters).
	/// </summary>
	public static bool IsIndexer(this PropertyInfo? propertyInfo)
		=> propertyInfo?.GetIndexParameters().Length > 0;

	/// <summary>
	///     Gets a value indicating whether the <see cref="PropertyInfo" /> is an extension property declared with the
	///     C# extension block syntax.
	/// </summary>
	/// <remarks>
	///     Extension properties (instance and static) are emitted onto the compiler-generated <c>&lt;G&gt;$…</c> grouping
	///     type that backs the surrounding extension block; the public static class exposes only their (non-special-name)
	///     accessor methods. A property is therefore an extension property exactly when its declaring type is such a
	///     grouping type.
	/// </remarks>
	/// <param name="propertyInfo">The <see cref="PropertyInfo" />.</param>
	public static bool IsExtensionProperty(this PropertyInfo? propertyInfo)
		=> propertyInfo?.DeclaringType is { } declaringType && declaringType.IsExtensionGroupingType();

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is read-only (can be read but not written).
	/// </summary>
	public static bool IsReadOnly(this PropertyInfo? propertyInfo)
		=> propertyInfo is { CanRead: true, CanWrite: false, };

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is write-only (can be written but not read).
	/// </summary>
	public static bool IsWriteOnly(this PropertyInfo? propertyInfo)
		=> propertyInfo is { CanRead: false, CanWrite: true, };

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is read-write (can be both read and written).
	/// </summary>
	public static bool IsReadWrite(this PropertyInfo? propertyInfo)
		=> propertyInfo is { CanRead: true, CanWrite: true, };

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is readable (can be read).
	/// </summary>
	public static bool IsReadable(this PropertyInfo? propertyInfo)
		=> propertyInfo is { CanRead: true, };

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is writable (can be written).
	/// </summary>
	public static bool IsWritable(this PropertyInfo? propertyInfo)
		=> propertyInfo is { CanWrite: true, };

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> has a getter.
	/// </summary>
	public static bool HasGetter(this PropertyInfo? propertyInfo)
		=> propertyInfo?.GetMethod != null;

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> has an init-only setter.
	/// </summary>
	/// <remarks>
	///     A setter is init-only if its return parameter carries the
	///     <c>System.Runtime.CompilerServices.IsExternalInit</c> required custom modifier.
	/// </remarks>
	public static bool HasInitSetter(this PropertyInfo? propertyInfo)
		=> propertyInfo?.SetMethod?.ReturnParameter.GetRequiredCustomModifiers()
			.Any(modifier => modifier.FullName == "System.Runtime.CompilerServices.IsExternalInit") == true;

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> has a (regular, non-init) setter.
	/// </summary>
	public static bool HasSetter(this PropertyInfo? propertyInfo)
		=> propertyInfo?.SetMethod != null && !propertyInfo.HasInitSetter();
}
