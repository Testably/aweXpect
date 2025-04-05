using System;
using aweXpect.Reflection.Extensions;

namespace aweXpect.Reflection;

/// <summary>
///     Expectations on <see cref="Type" />.
/// </summary>
public static partial class ThatType
{
	private static string? GetTypeNameOfType(Type? type)
	{
		if (type == null)
		{
			return null;
		}

		if (type.IsInterface)
		{
			return "interface";
		}

		if (type.IsEnum)
		{
			return "enum";
		}

		if (type.IsRecordClass())
		{
			return "record";
		}

		if (type.IsClass)
		{
			return "class";
		}

		if (type.IsRecordStruct())
		{
			return "record struct";
		}

		if (type.IsValueType)
		{
			return "struct";
		}

		return "<unknown>";
	}
}
