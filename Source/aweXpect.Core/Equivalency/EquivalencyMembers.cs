using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using aweXpect.Core.Helpers;
using aweXpect.Core.Metadata;

namespace aweXpect.Equivalency;

/// <summary>
///     A field or property of a type, resolved either from the <see cref="TypeMetadataRegistry" /> or by reflection.
/// </summary>
internal readonly struct EquivalencyMember(string name, Type declaredType, Func<object, object?> getValue)
{
	public string Name { get; } = name;
	public Type DeclaredType { get; } = declaredType;
	public Func<object, object?> GetValue { get; } = getValue;
}

/// <summary>
///     Resolves the members that participate in an equivalency comparison.
/// </summary>
/// <remarks>
///     A registered type is served from the <see cref="TypeMetadataRegistry" />, so that publishing with trimming or
///     Native AOT enabled does not remove the members. Every other type is reflected over, as before.
/// </remarks>
internal static class EquivalencyMembers
{
	public static IEnumerable<EquivalencyMember> GetFields(Type type, IncludeMembers includeMembers)
	{
		if (TryGetRegistered(type, includeMembers, out TypeMetadataRegistry.TypeMetadata? metadata))
		{
			return metadata.Fields.Values
				.OrderBy(member => member.Order)
				.Select(member => new EquivalencyMember(member.Name, member.MemberType, member.GetValue));
		}

		return type.GetFields(includeMembers)
			.Select(field => new EquivalencyMember(field.Name, field.FieldType, subject => field.GetValue(subject)));
	}

	public static IEnumerable<EquivalencyMember> GetProperties(Type type, IncludeMembers includeMembers)
	{
		if (TryGetRegistered(type, includeMembers, out TypeMetadataRegistry.TypeMetadata? metadata))
		{
			return metadata.Properties.Values
				.OrderBy(member => member.Order)
				.Select(member => new EquivalencyMember(member.Name, member.MemberType, member.GetValue));
		}

		return type.GetProperties(includeMembers)
			.Select(property
				=> new EquivalencyMember(property.Name, property.PropertyType, subject => property.GetValue(subject)));
	}

	/// <summary>
	///     Returns the accessor for the field <paramref name="name" /> on the <paramref name="type" />, or
	///     <see langword="null" /> when it has none.
	/// </summary>
	public static Func<object, object?>? FindField(Type type, string name, IncludeMembers includeMembers)
	{
		if (TryGetRegistered(type, includeMembers, out TypeMetadataRegistry.TypeMetadata? metadata))
		{
			return metadata.Fields.TryGetValue(name, out TypeMetadataRegistry.RegisteredMember? member)
				? member.GetValue
				: null;
		}

		FieldInfo? field = type.GetField(name, includeMembers.GetBindingFlags());
		return field is null ? null : subject => field.GetValue(subject);
	}

	/// <summary>
	///     Returns the accessor for the property <paramref name="name" /> on the <paramref name="type" />, or
	///     <see langword="null" /> when it has none.
	/// </summary>
	public static Func<object, object?>? FindProperty(Type type, string name, IncludeMembers includeMembers)
	{
		if (TryGetRegistered(type, includeMembers, out TypeMetadataRegistry.TypeMetadata? metadata))
		{
			return metadata.Properties.TryGetValue(name, out TypeMetadataRegistry.RegisteredMember? member)
				? member.GetValue
				: null;
		}

		PropertyInfo? property = type.GetProperty(name, includeMembers.GetBindingFlags());
		return property is null ? null : subject => property.GetValue(subject);
	}

	/// <remarks>
	///     A type counts as registered only when it has a field or a property: an event-only registration says nothing
	///     about the members, so such a type is reflected over like an unregistered one.
	///     <para />
	///     The registry only holds public members, and the zero-member guard cannot catch a comparison that comes up
	///     short rather than empty, so asking a registered type for non-public members has to fail instead of silently
	///     comparing fewer of them.
	/// </remarks>
	private static bool TryGetRegistered(Type type, IncludeMembers includeMembers,
		[NotNullWhen(true)] out TypeMetadataRegistry.TypeMetadata? metadata)
	{
		if (!TypeMetadataRegistry.Instance.TryGet(type, out metadata) ||
		    (metadata.Fields.IsEmpty && metadata.Properties.IsEmpty))
		{
			return false;
		}

		if ((includeMembers & ~IncludeMembers.Public) != IncludeMembers.None)
		{
			throw new InvalidOperationException(
					$"Only public members of {Formatter.Format(type)} can be compared, because its members are provided by the generated registration, which - like publishing with trimming or Native AOT enabled - does not preserve non-public members. Restrict the equivalency options to `IncludeMembers.Public`, or compare this type by value.")
				.LogTrace();
		}

		return true;
	}
}
