using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace aweXpect.Reflection.Formatting;

/// <summary>
///     Wraps a constructor, event, field, method or property, so that the formatters of this package format it.
/// </summary>
/// <remarks>
///     The formatter is registered for this type instead of the reflection types, which this package does not own, so
///     that formatting them outside of this package does not depend on whether a test used this package before.
/// </remarks>
internal sealed class FormattableMember
{
	private static readonly IValueFormatter[] Formatters =
	[
		new ConstructorFormatter(), new EventFormatter(), new FieldFormatter(), new MethodFormatter(),
		new PropertyFormatter(),
	];

	private readonly MemberInfo _member;

	private FormattableMember(MemberInfo member)
	{
		_member = member;
	}

	/// <summary>
	///     Wraps the <paramref name="value" /> if it is a constructor, event, field, method or property, otherwise
	///     returns it unchanged.
	/// </summary>
	public static object? From(object? value)
		=> value is ConstructorInfo or EventInfo or FieldInfo or MethodInfo or PropertyInfo
			? new FormattableMember((MemberInfo)value)
			: value;

	/// <summary>
	///     Wraps each of the <paramref name="values" /> with <see cref="From(object?)" />.
	/// </summary>
	/// <remarks>
	///     A collection is copied, so that a truncated message still names the number of remaining items; any other
	///     sequence stays lazy.
	/// </remarks>
	public static IEnumerable<object?> FromAll<T>(IEnumerable<T> values)
	{
		IEnumerable<object?> wrapped = values.Select(value => From(value));
		return values is ICollection or ICollection<T> or IReadOnlyCollection<T> ? wrapped.ToArray() : wrapped;
	}

	internal sealed class MemberFormatter : IValueFormatter
	{
		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
			=> value is FormattableMember formattableMember &&
			   Formatters.Any(formatter => formatter.TryFormat(stringBuilder, formattableMember._member, options));
	}
}
