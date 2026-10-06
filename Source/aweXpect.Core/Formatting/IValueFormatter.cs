using System.Text;

namespace aweXpect.Formatting;

/// <summary>
///     A custom value formatter to use in <see cref="ValueFormatter.Register(IValueFormatter)" />.
/// </summary>
public interface IValueFormatter
{
	/// <summary>
	///     Stores the formatted string in the <paramref name="stringBuilder" /> and returns <see langword="true" />,
	///     if the <paramref name="value" /> can be formatted, otherwise leaves the <paramref name="stringBuilder" />
	///     untouched and returns <see langword="false" />.
	/// </summary>
	bool TryFormat(
		StringBuilder stringBuilder,
		object value,
		FormattingOptions? options);
}
