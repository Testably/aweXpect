using System.Reflection;
using System.Text;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Formatting;

internal class FieldFormatter : IValueFormatter
{
	public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
	{
		if (value is FieldInfo fieldInfo)
		{
			Formatter.Format(stringBuilder, fieldInfo.FieldType);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, fieldInfo.DeclaringType);
			stringBuilder.Append('.');
			stringBuilder.Append(fieldInfo.Name);
			return true;
		}

		return false;
	}
}
