using System.Reflection;
using System.Text;

namespace aweXpect.Reflection.Formatting;

internal class EventFormatter : IValueFormatter
{
	public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
	{
		if (value is EventInfo @event)
		{
			stringBuilder.Append("event ");
			Formatter.Format(stringBuilder, @event.EventHandlerType);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, @event.DeclaringType);
			stringBuilder.Append('.');
			stringBuilder.Append(@event.Name);
			return true;
		}

		return false;
	}
}
