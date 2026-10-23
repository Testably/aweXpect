using System;
using System.Linq;
using System.Reflection;
using System.Text;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Formatting;

internal static class ParameterFormatter
{
	public static void AppendParameters(StringBuilder stringBuilder, ParameterInfo[] parameters)
	{
		int index = 0;
		foreach (ParameterInfo? parameter in parameters)
		{
			if (index++ > 0)
			{
				stringBuilder.Append(", ");
			}

			stringBuilder.Append(GetModifier(parameter));
			AppendType(stringBuilder, parameter.GetUnderlyingType());
			stringBuilder.Append(' ');
			stringBuilder.Append(parameter.Name);
			if (parameter.HasDefaultValue)
			{
				stringBuilder.Append(" = ");
				Formatter.Format(stringBuilder, parameter.DefaultValue);
			}
		}
	}

	public static void AppendReturnType(StringBuilder stringBuilder, MethodInfo methodInfo)
	{
		Type returnType = methodInfo.ReturnType;
		if (returnType == typeof(void))
		{
			stringBuilder.Append("void");
		}
		else if (returnType.IsByRef)
		{
			stringBuilder.Append(HasAttribute(methodInfo.ReturnParameter, IsReadOnlyAttribute)
				? "ref readonly "
				: "ref ");
			AppendType(stringBuilder, returnType.GetElementType()!);
		}
		else
		{
			AppendType(stringBuilder, returnType);
		}
	}

	private static void AppendType(StringBuilder stringBuilder, Type type)
	{
		if (type.IsGenericParameter)
		{
			stringBuilder.Append(type.Name);
		}
		else
		{
			Formatter.Format(stringBuilder, type);
		}
	}

	private static string GetModifier(ParameterInfo parameter)
	{
		if (parameter.IsOutParameter())
		{
			return "out ";
		}

		if (parameter.IsInParameter())
		{
			return HasAttribute(parameter, RequiresLocationAttribute) ? "ref readonly " : "in ";
		}

		if (parameter.IsRefParameter())
		{
			return "ref ";
		}

		return parameter.IsParamsParameter() ? "params " : "";
	}

	/// <remarks>
	///     The compiler embeds these attributes into assemblies whose target framework does not provide them, so they
	///     are matched by name. A <see langword="ref readonly" /> parameter is marked like an <see langword="in" />
	///     parameter plus <c>RequiresLocationAttribute</c>.
	/// </remarks>
	private static bool HasAttribute(ParameterInfo parameter, string attributeFullName)
		=> parameter.GetCustomAttributesData().Any(attribute => attribute.AttributeType.FullName == attributeFullName);

	private const string IsReadOnlyAttribute = "System.Runtime.CompilerServices.IsReadOnlyAttribute";
	private const string RequiresLocationAttribute = "System.Runtime.CompilerServices.RequiresLocationAttribute";
}
