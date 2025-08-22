using System.Reflection;
using System.Text;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Formatting;

internal class PropertyFormatter : IValueFormatter
{
	public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
	{
		if (value is PropertyInfo property)
		{
			var propertyAccess = property.GetAccessModifier();
			stringBuilder.Append(propertyAccess.GetString(" "));
			Formatter.Format(stringBuilder, property.PropertyType);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, property.DeclaringType);
			stringBuilder.Append('.');
			stringBuilder.Append(property.Name);
			stringBuilder.Append(" { ");
			if (property.CanRead)
			{
				var getAccess = property.GetMethod.GetAccessModifier();
				if (propertyAccess != getAccess)
				{
					stringBuilder.Append(getAccess.GetString(" "));
				}
				stringBuilder.Append("get; ");
			}

			if (property.CanWrite)
			{
				var setAccess = property.SetMethod.GetAccessModifier();
				if (propertyAccess != setAccess)
				{
					stringBuilder.Append(setAccess.GetString(" "));
				}
				stringBuilder.Append("set; ");
			}

			stringBuilder.Append('}');
			return true;
		}

		return false;
	}
}
