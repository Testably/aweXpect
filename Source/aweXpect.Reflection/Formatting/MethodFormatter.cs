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
			ParameterFormatter.AppendReturnType(stringBuilder, methodInfo);
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
			ParameterFormatter.AppendParameters(stringBuilder, methodInfo.GetParameters());
			stringBuilder.Append(')');
			return true;
		}

		return false;
	}
}
