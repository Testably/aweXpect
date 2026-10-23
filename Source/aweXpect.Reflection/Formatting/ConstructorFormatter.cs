using System.Reflection;
using System.Text;

namespace aweXpect.Reflection.Formatting;

internal class ConstructorFormatter : IValueFormatter
{
	public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
	{
		if (value is ConstructorInfo constructorInfo)
		{
			Formatter.Format(stringBuilder, constructorInfo.DeclaringType);
			stringBuilder.Append('(');
			ParameterFormatter.AppendParameters(stringBuilder, constructorInfo.GetParameters());
			stringBuilder.Append(')');
			return true;
		}

		return false;
	}
}
