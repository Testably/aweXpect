using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Helper methods to determine the nullability of members.
/// </summary>
/// <remarks>
///     A member is considered nullable if its type is a <see cref="Nullable{T}" /> value type or a reference
///     type whose declared annotation is nullable (e.g. <c>string?</c>). The declared annotation is decoded
///     from the <c>NullableAttribute</c> / <c>NullableContextAttribute</c> metadata on all target frameworks,
///     so the behavior cannot diverge between them: members without annotations (oblivious code) and
///     unconstrained generic type parameters count as non-nullable, and post-condition attributes like
///     <c>[AllowNull]</c> or <c>[MaybeNull]</c> are ignored.
/// </remarks>
internal static class NullabilityHelpers
{
	private const byte Annotated = 2;
	private const byte NotAnnotated = 1;
	private const string NullableAttributeName = "System.Runtime.CompilerServices.NullableAttribute";
	private const string NullableContextAttributeName = "System.Runtime.CompilerServices.NullableContextAttribute";

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is nullable.
	/// </summary>
	/// <remarks>
	///     A property is considered nullable if its type is a <see cref="Nullable{T}" /> value type or a
	///     reference type annotated as nullable (according to the nullable reference type metadata).
	/// </remarks>
	public static bool IsNullable(this PropertyInfo? propertyInfo)
	{
		if (propertyInfo is null)
		{
			return false;
		}

		if (propertyInfo.PropertyType.IsValueType)
		{
			return Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null;
		}

		return IsNullableReferenceType(propertyInfo);
	}

	/// <summary>
	///     Checks if the <paramref name="fieldInfo" /> is nullable.
	/// </summary>
	/// <remarks>
	///     A field is considered nullable if its type is a <see cref="Nullable{T}" /> value type or a
	///     reference type annotated as nullable (according to the nullable reference type metadata).
	/// </remarks>
	public static bool IsNullable(this FieldInfo? fieldInfo)
	{
		if (fieldInfo is null)
		{
			return false;
		}

		if (fieldInfo.FieldType.IsValueType)
		{
			return Nullable.GetUnderlyingType(fieldInfo.FieldType) != null;
		}

		return IsNullableReferenceType(fieldInfo);
	}

	/// <summary>
	///     Returns the nullable fields and properties of the <paramref name="type" />, including inherited
	///     members or only those declared directly on the type according to the <paramref name="memberScope" />.
	/// </summary>
	public static MemberInfo[] GetNullableMembers(this Type type,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> type.GetMembersByNullability(memberScope).Nullable;

	/// <summary>
	///     Returns the non-nullable fields and properties of the <paramref name="type" />, including inherited
	///     members or only those declared directly on the type according to the <paramref name="memberScope" />.
	/// </summary>
	public static MemberInfo[] GetNotNullableMembers(this Type type,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> type.GetMembersByNullability(memberScope).NotNullable;

	/// <summary>
	///     Partitions the fields and properties of the <paramref name="type" /> into nullable and
	///     non-nullable members in a single pass, including inherited members or only those declared
	///     directly on the type according to the <paramref name="memberScope" />.
	/// </summary>
	public static (MemberInfo[] Nullable, MemberInfo[] NotNullable) GetMembersByNullability(this Type type,
		MemberScope memberScope = MemberScope.DeclaredOnly)
	{
		List<MemberInfo> nullable = [];
		List<MemberInfo> notNullable = [];
		foreach (FieldInfo field in type.GetDeclaredFields(memberScope))
		{
			(field.IsNullable() ? nullable : notNullable).Add(field);
		}

		foreach (PropertyInfo property in type.GetDeclaredProperties(memberScope))
		{
			(property.IsNullable() ? nullable : notNullable).Add(property);
		}

		return (nullable.ToArray(), notNullable.ToArray());
	}

	/// <summary>
	///     Determines the nullability of a reference type member from the nullable reference type metadata.
	/// </summary>
	/// <remarks>
	///     The compiler stores the annotation in a <c>NullableAttribute</c> on the member (a scalar
	///     <see cref="byte" /> or a <see cref="byte" /> array whose first element describes the top-level type)
	///     and omits it when the value equals the context stored in a <c>NullableContextAttribute</c> on one of
	///     the declaring types. A flag value of <c>2</c> means "annotated" (nullable).
	///     <para />
	///     Members declared as a bare generic type parameter (flag value <c>1</c>) are additionally resolved
	///     through the constructed declaring type, so that e.g. a member of type <c>T</c> accessed via a type
	///     deriving from <c>Base&lt;string?&gt;</c> counts as nullable.
	///     <para />
	///     The metadata is decoded directly (instead of using <c>NullabilityInfoContext</c>, which is unavailable
	///     on netstandard2.0) so that the same member yields the same result on every target framework.
	/// </remarks>
	private static bool IsNullableReferenceType(MemberInfo memberInfo)
	{
		byte flag = GetNullableFlag(memberInfo)
		            ?? GetNullableContextFlag(memberInfo.DeclaringType)
		            ?? 0;
		if (flag == Annotated)
		{
			return true;
		}

		return flag == NotAnnotated && IsNullableViaGenericArgument(memberInfo);
	}

	/// <summary>
	///     Returns the nullability flag of the <c>NullableAttribute</c> on the <paramref name="memberInfo" />,
	///     or <see langword="null" /> if the member has no such attribute.
	/// </summary>
	private static byte? GetNullableFlag(MemberInfo memberInfo)
	{
		foreach (CustomAttributeData attributeData in memberInfo.CustomAttributes)
		{
			if (attributeData.AttributeType.FullName == NullableAttributeName &&
			    attributeData.ConstructorArguments.Count == 1)
			{
				CustomAttributeTypedArgument argument = attributeData.ConstructorArguments[0];
				if (argument.Value is byte flag)
				{
					return flag;
				}

				if (argument.Value is IReadOnlyList<CustomAttributeTypedArgument> { Count: > 0, } flags &&
				    flags[0].Value is byte firstFlag)
				{
					return firstFlag;
				}

				return 0;
			}
		}

		return null;
	}

	/// <summary>
	///     Returns the nullability flag of the first <c>NullableContextAttribute</c> found on the
	///     <paramref name="type" /> or one of its declaring types, or <see langword="null" /> if none is found.
	/// </summary>
	private static byte? GetNullableContextFlag(Type? type)
	{
		for (Type? declaringType = type; declaringType is not null; declaringType = declaringType.DeclaringType)
		{
			foreach (CustomAttributeData attributeData in declaringType.CustomAttributes)
			{
				if (attributeData.AttributeType.FullName == NullableContextAttributeName &&
				    attributeData.ConstructorArguments.Count == 1 &&
				    attributeData.ConstructorArguments[0].Value is byte contextFlag)
				{
					return contextFlag;
				}
			}
		}

		return null;
	}

	/// <summary>
	///     Checks if a member declared as a bare generic type parameter (e.g. <c>T</c>) is nullable because
	///     the type argument of the constructed declaring type is annotated as nullable (e.g. when accessed
	///     via a type deriving from <c>Base&lt;string?&gt;</c>).
	/// </summary>
	/// <remarks>
	///     The annotation of the type arguments is stored in a <c>NullableAttribute</c> on the derived type
	///     whose base type reference instantiates the generic type definition: index 0 describes the base
	///     type itself, followed by its flattened generic arguments.
	/// </remarks>
	private static bool IsNullableViaGenericArgument(MemberInfo memberInfo)
	{
		if (memberInfo.DeclaringType is not { IsGenericType: true, IsGenericTypeDefinition: false, } declaringType)
		{
			return false;
		}

		Type genericTypeDefinition = declaringType.GetGenericTypeDefinition();
		if (GetDeclaredMemberType(memberInfo, genericTypeDefinition) is not
		    { IsGenericParameter: true, } genericParameter)
		{
			return false;
		}

		for (Type? derivedType = memberInfo.ReflectedType; derivedType is not null; derivedType = derivedType.BaseType)
		{
			Type? baseType = derivedType.BaseType;
			if (baseType is not { IsGenericType: true, } || baseType.GetGenericTypeDefinition() != genericTypeDefinition)
			{
				continue;
			}

			Type genericArgument = baseType.GetGenericArguments()[genericParameter.GenericParameterPosition];
			if (genericArgument.IsGenericParameter || genericArgument.IsValueType)
			{
				return false;
			}

			return IsNullableBaseTypeArgument(derivedType, baseType, genericParameter.GenericParameterPosition);
		}

		return false;
	}

	/// <summary>
	///     Returns the type of the member on the generic type definition that corresponds to the
	///     <paramref name="memberInfo" /> of a constructed generic type.
	/// </summary>
	private static Type? GetDeclaredMemberType(MemberInfo memberInfo, Type genericTypeDefinition)
	{
		foreach (MemberInfo member in genericTypeDefinition.GetMember(memberInfo.Name, memberInfo.MemberType,
			         BindingFlags.DeclaredOnly |
			         BindingFlags.NonPublic |
			         BindingFlags.Public |
			         BindingFlags.Instance |
			         BindingFlags.Static))
		{
			if (member.MetadataToken == memberInfo.MetadataToken)
			{
				return member switch
				{
					FieldInfo fieldInfo => fieldInfo.FieldType,
					PropertyInfo propertyInfo => propertyInfo.PropertyType,
					_ => null,
				};
			}
		}

		return null;
	}

	/// <summary>
	///     Checks if the generic argument at the <paramref name="position" /> of the <paramref name="baseType" />
	///     reference is annotated as nullable in the metadata of the <paramref name="derivedType" />.
	/// </summary>
	private static bool IsNullableBaseTypeArgument(Type derivedType, Type baseType, int position)
	{
		foreach (CustomAttributeData attributeData in derivedType.CustomAttributes)
		{
			if (attributeData.AttributeType.FullName == NullableAttributeName &&
			    attributeData.ConstructorArguments.Count == 1)
			{
				CustomAttributeTypedArgument argument = attributeData.ConstructorArguments[0];
				if (argument.Value is byte flag)
				{
					return flag == Annotated;
				}

				if (argument.Value is IReadOnlyList<CustomAttributeTypedArgument> flags)
				{
					int index = 1;
					Type[] genericArguments = baseType.GetGenericArguments();
					for (int i = 0; i < position; i++)
					{
						index += CountNullabilityStates(genericArguments[i]);
					}

					return index < flags.Count && flags[index].Value is byte argumentFlag && argumentFlag == Annotated;
				}

				return false;
			}
		}

		return GetNullableContextFlag(derivedType) == Annotated;
	}

	/// <summary>
	///     Counts the number of nullability flags the <paramref name="type" /> occupies in the flattened
	///     <c>NullableAttribute</c> encoding: reference types and generic value types occupy one flag plus
	///     the flags of their generic arguments, arrays one flag plus the flags of their element type, and
	///     non-generic value types none.
	/// </summary>
	private static int CountNullabilityStates(Type type)
	{
		Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
		if (underlyingType.IsGenericType)
		{
			int count = 1;
			foreach (Type genericArgument in underlyingType.GetGenericArguments())
			{
				count += CountNullabilityStates(genericArgument);
			}

			return count;
		}

		if (underlyingType.HasElementType)
		{
			return (underlyingType.IsArray ? 1 : 0) + CountNullabilityStates(underlyingType.GetElementType()!);
		}

		return type.IsValueType ? 0 : 1;
	}
}
