using System;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Extension methods for <see cref="MemberInfo" />.
/// </summary>
internal static class MemberInfoHelpers
{
	/// <summary>
	///     Checks if the <paramref name="memberInfo" /> has the specified <paramref name="accessModifiers" />.
	/// </summary>
	public static bool HasAccessModifier(
		this MemberInfo? memberInfo,
		AccessModifiers accessModifiers)
		=> memberInfo switch
		{
			Type type => type.HasAccessModifier(accessModifiers),
			ConstructorInfo constructorInfo => constructorInfo.HasAccessModifier(accessModifiers),
			EventInfo eventInfo => eventInfo.HasAccessModifier(accessModifiers),
			FieldInfo fieldInfo => fieldInfo.HasAccessModifier(accessModifiers),
			MethodInfo methodInfo => methodInfo.HasAccessModifier(accessModifiers),
			PropertyInfo propertyInfo => propertyInfo.HasAccessModifier(accessModifiers),
			_ => false,
		};

	/// <summary>
	///     Gets the <see cref="AccessModifiers" /> of the <paramref name="memberInfo" />.
	/// </summary>
	public static AccessModifiers GetAccessModifier(
		this MemberInfo? memberInfo)
	{
		if (memberInfo.HasAccessModifier(AccessModifiers.Public))
		{
			return AccessModifiers.Public;
		}

		if (memberInfo.HasAccessModifier(AccessModifiers.Private))
		{
			if (memberInfo.HasAccessModifier(AccessModifiers.Protected))
			{
				return AccessModifiers.PrivateProtected;
			}

			return AccessModifiers.Private;
		}

		if (memberInfo.HasAccessModifier(AccessModifiers.Protected))
		{
			if (memberInfo.HasAccessModifier(AccessModifiers.Internal))
			{
				return AccessModifiers.ProtectedInternal;
			}

			return AccessModifiers.Protected;
		}

		if (memberInfo.HasAccessModifier(AccessModifiers.Internal))
		{
			return AccessModifiers.Internal;
		}

		return AccessModifiers.Any;
	}
}
