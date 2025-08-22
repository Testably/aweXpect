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
			else if (methodInfo.ReturnType.IsGenericParameter)
			{
				stringBuilder.Append(methodInfo.ReturnType.Name);
			}
			else
			{
				Formatter.Format(stringBuilder, methodInfo.ReturnType);
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
			int index = 0;
			foreach (ParameterInfo? parameter in methodInfo.GetParameters())
			{
				if (index++ > 0)
				{
					stringBuilder.Append(", ");
				}

				if (parameter.ParameterType.IsGenericParameter)
				{
					stringBuilder.Append(parameter.ParameterType.Name);
				}
				else
				{
					Formatter.Format(stringBuilder, parameter.ParameterType);
				}

				stringBuilder.Append(' ');
				stringBuilder.Append(parameter.Name);
				if (parameter.HasDefaultValue)
				{
					stringBuilder.Append(" = ");
					Formatter.Format(stringBuilder, parameter.DefaultValue);
				}
			}

			stringBuilder.Append(')');
			return true;
		}

		return false;
	}
}
