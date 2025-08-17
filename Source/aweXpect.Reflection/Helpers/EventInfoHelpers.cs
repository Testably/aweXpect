using System;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Extension events for <see cref="EventInfo" />.
/// </summary>
internal static class EventInfoHelpers
{
	/// <summary>
	///     Checks if the <paramref name="eventInfo" /> has the specified <paramref name="accessModifiers" />.
	/// </summary>
	/// <param name="eventInfo">The <see cref="FieldInfo" /> which is checked to have the attribute.</param>
	/// <param name="accessModifiers">
	///     The <see cref="AccessModifiers" />.
	///     <para />
	///     Supports specifying multiple <see cref="AccessModifiers" />.
	/// </param>
	public static bool HasAccessModifier(
		this EventInfo? eventInfo,
		AccessModifiers accessModifiers)
		=> eventInfo?.AddMethod.HasAccessModifier(accessModifiers) == true;

	/// <summary>
	///     Checks if the <paramref name="eventInfo" /> has an attribute which satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <typeparam name="TAttribute">The type of the <see cref="Attribute" />.</typeparam>
	/// <param name="eventInfo">The <see cref="EventInfo" /> which is checked to have the attribute.</param>
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
		this EventInfo eventInfo,
		Func<TAttribute, bool>? predicate = null,
		bool inherit = true)
		where TAttribute : Attribute
	{
		object? attribute = Attribute.GetCustomAttributes(eventInfo, typeof(TAttribute), inherit)
			.FirstOrDefault();
		if (attribute is TAttribute castedAttribute)
		{
			return predicate?.Invoke(castedAttribute) ?? true;
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="eventInfo" /> has an attribute which satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo" /> which is checked to have the attribute.</param>
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
		this EventInfo? eventInfo,
		Type attributeType,
		Func<Attribute, bool>? predicate = null,
		bool inherit = true)
	{
		object? attribute = eventInfo?.GetCustomAttributes(attributeType, inherit)
			.FirstOrDefault();
		if (attribute is Attribute attributeValue)
		{
			return predicate?.Invoke(attributeValue) ?? true;
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="eventInfo" /> is abstract (based on its accessor methods).
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo" /> to check.</param>
	/// <returns><see langword="true" /> if the event is abstract; otherwise, <see langword="false" />.</returns>
	public static bool IsReallyAbstract(this EventInfo? eventInfo)
		=> eventInfo?.AddMethod?.IsAbstract == true;

	/// <summary>
	///     Checks if the <paramref name="eventInfo" /> is sealed (based on its accessor methods).
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo" /> to check.</param>
	/// <returns><see langword="true" /> if the event is sealed; otherwise, <see langword="false" />.</returns>
	public static bool IsReallySealed(this EventInfo? eventInfo)
		=> eventInfo?.AddMethod?.IsFinal == true;
}
