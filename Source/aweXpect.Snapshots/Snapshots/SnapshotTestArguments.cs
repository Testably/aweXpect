using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Core.Adapters;
using aweXpect.Customization;
using aweXpect.Formatting;

namespace aweXpect.Snapshots;

/// <summary>
///     Names a snapshot after the arguments of the running test, see
///     <see cref="SnapshotsCustomizationExtensions.SnapshotsCustomization.NamedAfterTestArguments" />, or after the
///     arguments of <c>.NamedAfter(…)</c>, which are written alike, and limits the length of every name.
/// </summary>
/// <remarks>
///     The name has to be the same on every machine, culture and target framework, as it is part of the file name.
///     Two cases whose arguments are written alike share a snapshot, which fails like any snapshot that is used for
///     another value in the same test run, so a collision is never silent.
/// </remarks>
internal static class SnapshotTestArguments
{
	/// <summary>
	///     The longest name as it is written into the file name, where each character that the store escapes takes the
	///     three characters <c>%XX</c>, which leaves room for the class, the method, the variant and the suffixes.
	/// </summary>
	private const int MaxLength = 64;

	private const int HashLength = 8;

	/// <summary>
	///     Writes a nested <see cref="double" /> or <see cref="float" /> like one at the top, which also keeps it the same
	///     on every target framework.
	/// </summary>
	private static readonly FormattingOptions NestedValueFormatting = FormattingOptions.Snapshot with
	{
		FormatValue = value => value.GetValue() is { } number and (double or float)
			? ValueFormatting.Replace(new Unquoted(Format([number,])))
			: ValueFormatting.Write,
	};

	/// <summary>
	///     Returns the name of the arguments of the running test, before it is shortened, or <see langword="null" /> when
	///     the setting is off, the test framework does not expose the running test, or the test has no arguments.
	/// </summary>
	/// <remarks>
	///     The arguments of the test class come first, but only those that are values, as a test framework also injects
	///     shared fixtures into the constructor, whose members can change from run to run.
	/// </remarks>
	public static string? GetName()
	{
		if (!Customize.aweXpect.Snapshots().NamedAfterTestArguments.Get() ||
		    !TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest))
		{
			return null;
		}

		// A CancellationToken parameter is injected by the test framework and is the same in every case, so it is skipped,
		// and a test whose only parameter is a token keeps the snapshot of a test without parameters.
		object?[] arguments = currentTest.Identity.ClassArguments.Where(IsValue)
			.Concat(currentTest.Identity.Arguments.Where(argument => argument is not CancellationToken))
			.ToArray();
		return arguments.Length == 0 ? null : Format(arguments);
	}

	/// <summary>
	///     Whether the <paramref name="argument" /> is a value, or a collection of values, rather than an injected object.
	/// </summary>
	private static bool IsValue(object? argument)
		=> IsScalar(argument) ||
		   (argument is IEnumerable items && items.Cast<object?>().All(IsScalar));

	private static bool IsScalar(object? value)
		=> value is null or string or bool or char or IFormattable;

	/// <summary>
	///     Writes the <paramref name="values" /> in the invariant culture, separated by <c>_</c>.
	/// </summary>
	/// <remarks>
	///     The name is not shortened yet, see <see cref="Shorten" />, so that a name appended to it is part of the hash.
	/// </remarks>
	public static string Format(IReadOnlyList<object?> values)
	{
		StringBuilder stringBuilder = new();
		for (int i = 0; i < values.Count; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append('_');
			}

			Append(stringBuilder, values[i]);
		}

		return stringBuilder.ToString();
	}

	/// <remarks>
	///     A <see cref="double" /> or <see cref="float" /> is written with the fewest digits that round-trip, see
	///     <see cref="RoundTripNumber" />. A date, time, collection, tuple, record or other object is written like in a
	///     snapshot, as its <see cref="object.ToString()" /> is often the name of its type or uses the current culture, so
	///     that a date is written in ISO 8601, a set is sorted and a collection that contains itself does not overflow the
	///     stack.
	/// </remarks>
	private static void Append(StringBuilder stringBuilder, object? value)
	{
		switch (value)
		{
			case null:
				stringBuilder.Append("null");
				break;
			case string text:
				stringBuilder.Append(text.Length == 0 ? "\"\"" : text);
				break;
			case double number:
				RoundTripNumber.Append(stringBuilder, number, false);
				break;
			case float number:
				RoundTripNumber.Append(stringBuilder, number, true);
				break;
			case byte or sbyte or short or ushort or int or uint or long or ulong or decimal or Enum:
				stringBuilder.Append(((IFormattable)value).ToString(null, CultureInfo.InvariantCulture));
				break;
			case bool or char:
				stringBuilder.Append(value);
				break;
			default:
				aweXpect.Formatting.Format.Formatter.Format(stringBuilder, value, NestedValueFormatting);
				break;
		}
	}

	/// <summary>
	///     Keeps a name of up to <see cref="MaxLength" /> escaped characters, and replaces the end of a longer one with
	///     <c>~</c> and the hash of the whole name.
	/// </summary>
	/// <remarks>
	///     Every name of a snapshot is shortened, also one from <c>.Named(…)</c>. The name is not cut within a surrogate
	///     pair, so that it stays valid text.
	/// </remarks>
	public static string Shorten(string name)
	{
		int escapedLength = 0;
		foreach (char character in name)
		{
			escapedLength += GetEscapedLength(character);
		}

		if (escapedLength <= MaxLength)
		{
			return name;
		}

		int length = 0;
		escapedLength = 0;
		while (escapedLength + GetEscapedLength(name[length]) <= MaxLength - HashLength - 1)
		{
			escapedLength += GetEscapedLength(name[length]);
			length++;
		}

		if (char.IsHighSurrogate(name[length - 1]))
		{
			length--;
		}

#if NET8_0_OR_GREATER
		return string.Concat(name.AsSpan(0, length), "~",
			GetHashCode(name).ToString("x8", CultureInfo.InvariantCulture));
#else
		return name.Substring(0, length) + "~" +
		       GetHashCode(name).ToString("x8", CultureInfo.InvariantCulture);
#endif
	}

	/// <summary>
	///     A text that is written without quotes.
	/// </summary>
	private sealed class Unquoted(string text)
	{
		public override string ToString() => text;
	}

	private static int GetEscapedLength(char character)
		=> DiskSnapshotStore.IsEscaped(character) ? 3 : 1;

	/// <summary>
	///     The FNV-1a hash of the UTF-8 bytes of the <paramref name="name" />, which is the same in every process.
	/// </summary>
	private static uint GetHashCode(string name)
	{
		unchecked
		{
			uint hashCode = 2166136261;
			foreach (byte value in Encoding.UTF8.GetBytes(name))
			{
				hashCode = (hashCode ^ value) * 16777619;
			}

			return hashCode;
		}
	}
}
