using System.Reflection;
using System.Text;

namespace aweXpect.Reflection.Formatting;

internal class EventFormatter : IValueFormatter
{
	public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
	{
		if (value is EventInfo eventInfo)
		{
			stringBuilder.Append("event ");
			Formatter.Format(stringBuilder, eventInfo.EventHandlerType);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, eventInfo.DeclaringType);
			stringBuilder.Append('.');
			stringBuilder.Append(eventInfo.Name);
			return true;
		}

		return false;
	}
}
