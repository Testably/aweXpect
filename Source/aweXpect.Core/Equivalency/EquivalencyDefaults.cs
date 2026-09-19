using System;
using System.Globalization;
using System.Reflection;

namespace aweXpect.Equivalency;

/// <summary>
///     Default behaviour to use for equivalency.
/// </summary>
public static class EquivalencyDefaults
{
	/// <summary>
	///     The default selection of the <see cref="EquivalencyComparisonType" /> for
	///     the given <paramref name="type" />.
	/// </summary>
	public static EquivalencyComparisonType DefaultComparisonType(Type type)
	{
		if (type.IsPrimitive
		    || type.IsEnum
		    || type == typeof(string)
		    || type == typeof(decimal)
		    || type == typeof(DateTime)
		    || type == typeof(DateTimeOffset)
		    || type == typeof(TimeSpan)
		    || type == typeof(Guid)
		    || IsHandle(type))
		{
			return EquivalencyComparisonType.ByValue;
		}

		return EquivalencyComparisonType.ByMembers;
	}

	/// <remarks>
	///     A handle describes something else instead of carrying state of its own, so its members are derived views
	///     that can throw (<c>GenericParameterPosition</c> on a <see cref="Type" />, every component of a relative
	///     <see cref="Uri" />) or expand into an unbounded graph, while its <see cref="object.Equals(object)" /> is
	///     exactly the identity the caller means. The runtime type is always a derived type - a <c>RuntimeType</c> is
	///     not <c>typeof(Type)</c> - so the match has to be by assignability.
	/// </remarks>
	private static bool IsHandle(Type type)
		=> typeof(MemberInfo).IsAssignableFrom(type)
		   || typeof(Assembly).IsAssignableFrom(type)
		   || typeof(Module).IsAssignableFrom(type)
		   || typeof(Delegate).IsAssignableFrom(type)
		   || typeof(Uri).IsAssignableFrom(type)
		   || typeof(CultureInfo).IsAssignableFrom(type);
}
