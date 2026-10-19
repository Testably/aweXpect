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
			int index = 0;
			foreach (ParameterInfo? parameter in constructorInfo.GetParameters())
			{
				if (index++ > 0)
				{
					stringBuilder.Append(", ");
				}

				Formatter.Format(stringBuilder, parameter.ParameterType);
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
