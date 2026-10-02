using System;
using System.Globalization;
using System.Net;
using System.Numerics;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif
using System.Text;

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
		    || IsDateOrTime(type)
		    || IsNumber(type)
		    || IsHandle(type)
		    || EquivalencyContent.IsComparedByContent(type))
		{
			return EquivalencyComparisonType.ByValue;
		}

		return EquivalencyComparisonType.ByMembers;
	}

	/// <remarks>
	///     The public members of a date or a time of day are views of the same value, so one difference would be
	///     reported once per view.
	///     <para />
	///     netstandard2.0 has no <c>DateOnly</c> or <c>TimeOnly</c>, but is served to runtimes that have them, so they
	///     are matched by name there.
	/// </remarks>
	private static bool IsDateOrTime(Type type)
#if NET8_0_OR_GREATER
		=> type == typeof(DateOnly) || type == typeof(TimeOnly);
#else
		=> type.FullName is "System.DateOnly" or "System.TimeOnly";
#endif

	/// <remarks>
	///     The public members of a number either only describe it (<c>Sign</c>, <c>IsEven</c>) or do not exist at all
	///     (<c>Int128</c>), so they cannot tell two values apart.
	///     <para />
	///     netstandard2.0 has no <c>Half</c>, <c>NFloat</c>, <c>Int128</c> or <c>UInt128</c>, but is served to runtimes
	///     that have them, so they are matched by name there.
	/// </remarks>
	private static bool IsNumber(Type type)
	{
#if NET8_0_OR_GREATER
		if (type == typeof(Half)
		    || type == typeof(NFloat)
		    || type == typeof(Int128)
		    || type == typeof(UInt128))
		{
			return true;
		}
#else
		if (type.FullName is "System.Half"
		    or "System.Runtime.InteropServices.NFloat"
		    or "System.Int128"
		    or "System.UInt128")
		{
			return true;
		}
#endif

		return type == typeof(BigInteger)
		       || type == typeof(Complex);
	}

	/// <remarks>
	///     A handle describes something else instead of carrying state of its own, so its members are derived views
	///     that can throw (<c>GenericParameterPosition</c> on a <see cref="Type" />, every component of a relative
	///     <see cref="Uri" />) or expand into an unbounded graph, while its <see cref="object.Equals(object)" /> is
	///     exactly the identity the caller means. The runtime type is always a derived type - a <c>RuntimeType</c> is
	///     not <c>typeof(Type)</c> - so the match has to be by assignability.<br />
	///     An <see cref="IPAddress" /> and an <see cref="Encoding" /> are values, but their members fail the same way
	///     (<c>ScopeId</c> throws for an IPv4 address, <c>Address</c> for an IPv6 one, and the <c>Preamble</c> of an
	///     encoding is a span that reflection cannot read), while their <see cref="object.Equals(object)" /> compares
	///     exactly what sets them apart.
	/// </remarks>
	private static bool IsHandle(Type type)
		=> typeof(MemberInfo).IsAssignableFrom(type)
		   || typeof(Assembly).IsAssignableFrom(type)
		   || typeof(Module).IsAssignableFrom(type)
		   || typeof(Delegate).IsAssignableFrom(type)
		   || typeof(Uri).IsAssignableFrom(type)
		   || typeof(CultureInfo).IsAssignableFrom(type)
		   || typeof(IPAddress).IsAssignableFrom(type)
		   || typeof(Encoding).IsAssignableFrom(type);
}
