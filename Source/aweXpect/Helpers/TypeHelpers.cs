using System;
using System.Linq;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class TypeHelpers
{
	/// <summary>
	///     Checks if the <paramref name="actual" /> is of the <paramref name="type" /> or a derived type, where an open
	///     generic type matches every type constructed from it or implementing it.
	/// </summary>
	public static bool IsOrImplements(this Type type, object? actual)
	{
		if (type.IsInstanceOfType(actual))
		{
			return true;
		}

		Type? actualType = actual?.GetType();
		if (!type.IsGenericTypeDefinition || actualType is null)
		{
			return false;
		}

		if (!type.IsInterface)
		{
			for (Type? baseType = actualType; baseType is not null; baseType = baseType.BaseType)
			{
				if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == type)
				{
					return true;
				}
			}

			return false;
		}

		if (!ReflectionFallback.IsSupported)
		{
			throw Tracing.WriteException(
				new NotSupportedException(
					$"The interfaces of {Formatter.Format(actualType)} cannot be found by reflection, which is switched off when publishing with trimming or Native AOT enabled. Check against a constructed interface instead of its generic definition. Alternatively, set the runtime switch 'aweXpect.ReflectionFallback.IsSupported' to true to reflect anyway."));
		}

		return actualType.GetInterfaces()
			.Any(childInterface =>
			{
				Type currentInterface = childInterface.IsGenericType
					? childInterface.GetGenericTypeDefinition()
					: childInterface;

				return currentInterface == type;
			});
	}
}
