using System.Reflection;
using System.Text;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Formatting;

internal class PropertyFormatter : IValueFormatter
{
	public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
	{
		if (value is PropertyInfo propertyInfo)
		{
			var propertyAccess = propertyInfo.GetAccessModifier();
			stringBuilder.Append(propertyAccess.GetString(" "));
			Formatter.Format(stringBuilder, propertyInfo.PropertyType);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, propertyInfo.DeclaringType);
			stringBuilder.Append('.');
			stringBuilder.Append(propertyInfo.Name);
			stringBuilder.Append(" { ");
			if (propertyInfo.CanRead)
			{
				var getAccess = propertyInfo.GetMethod.GetAccessModifier();
				if (propertyAccess != getAccess)
				{
					stringBuilder.Append(getAccess.GetString(" "));
				}
				stringBuilder.Append("get; ");
			}

			if (propertyInfo.CanWrite)
			{
				var setAccess = propertyInfo.SetMethod.GetAccessModifier();
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
