using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace aweXpect.Reflection.Formatting;

internal class MethodFormatter : IValueFormatter
{
	public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
	{
		if (value is MethodInfo methodInfo)
		{
			if (methodInfo.ReturnType == typeof(void))
			{
				stringBuilder.Append("void");
			}
			else
			{
				AppendType(stringBuilder, methodInfo.ReturnType);
			}

			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, methodInfo.DeclaringType);
			stringBuilder.Append('.');
			stringBuilder.Append(methodInfo.Name);
			if (methodInfo.IsGenericMethod)
			{
				stringBuilder.Append('<');
				stringBuilder.Append(string.Join(", ", methodInfo.GetGenericArguments().Select(x => x.Name)));
				stringBuilder.Append('>');
			}

			stringBuilder.Append('(');
			AppendParameters(stringBuilder, methodInfo.GetParameters());
			stringBuilder.Append(')');
			return true;
		}

		return false;
	}

	private static void AppendParameters(StringBuilder stringBuilder, ParameterInfo[] parameters)
	{
		int index = 0;
		foreach (ParameterInfo? parameter in parameters)
		{
			if (index++ > 0)
			{
				stringBuilder.Append(", ");
			}

			AppendType(stringBuilder, parameter.ParameterType);
			stringBuilder.Append(' ');
			stringBuilder.Append(parameter.Name);
			if (parameter.HasDefaultValue)
			{
				stringBuilder.Append(" = ");
				Formatter.Format(stringBuilder, parameter.DefaultValue);
			}
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
}
