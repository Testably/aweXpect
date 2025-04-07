using System.Collections.Generic;
#if !NET8_0_OR_GREATER
using System.Linq;
#endif

namespace aweXpect.Reflection.Collections;

internal static class AccessModifiersExtensions
{
	public static string GetString(this AccessModifiers accessModifier)
	{
		if (accessModifier == AccessModifiers.Public)
		{
			return "public ";
		}

		if (accessModifier == AccessModifiers.Protected)
		{
			return "protected ";
		}

		if (accessModifier == AccessModifiers.Private)
		{
			return "private ";
		}

		if (accessModifier == AccessModifiers.Internal)
		{
			return "internal ";
		}

		if (accessModifier == AccessModifiers.Any)
		{
			return string.Empty;
		}

		List<string> modifiers = [];
		if (accessModifier.HasFlag(AccessModifiers.Public))
		{
			modifiers.Add("public");
		}

		if (accessModifier.HasFlag(AccessModifiers.Protected))
		{
			modifiers.Add("protected");
		}

		if (accessModifier.HasFlag(AccessModifiers.Private))
		{
			modifiers.Add("private");
		}

		if (accessModifier.HasFlag(AccessModifiers.Internal))
		{
			modifiers.Add("internal");
		}

#if NET8_0_OR_GREATER
		return string.Join(", ", modifiers[..^1]) + " or " + modifiers[^1] + " ";
#else
		return string.Join(", ", modifiers.Take(modifiers.Count - 1)) + " or " + modifiers[^1] + " ";
#endif
	}
}
