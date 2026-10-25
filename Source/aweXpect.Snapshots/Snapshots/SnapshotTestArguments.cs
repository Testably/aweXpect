using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using aweXpect.Core.Adapters;
using aweXpect.Customization;

namespace aweXpect.Snapshots;

/// <summary>
///     Names a snapshot after the arguments of the running test, see
///     <see cref="SnapshotsCustomizationExtensions.SnapshotsCustomization.NamedAfterTestArguments" />.
/// </summary>
/// <remarks>
///     The name has to be the same on every machine, culture and target framework, as it is part of the file name.
///     Two cases whose arguments are written alike share a snapshot, which fails like any snapshot that is used for
///     another value in the same test run, so a collision is never silent.
/// </remarks>
internal static class SnapshotTestArguments
{
	/// <summary>
	///     The longest name, which leaves room in the file name for the class, the method, the variant and the suffixes,
	///     even when the store escapes some characters.
	/// </summary>
	private const int MaxLength = 64;

	private const int HashLength = 8;

	/// <summary>
	///     Returns the name of the arguments of the running test, or <see langword="null" /> when the setting is off, the
	///     test framework does not expose the running test, or the test has no arguments.
	/// </summary>
	public static string? GetName()
	{
		if (!Customize.aweXpect.Snapshots().NamedAfterTestArguments.Get() ||
		    !TestFrameworkRegistry.TryGetCurrentTest(out TestIdentity? currentTest) ||
		    currentTest.Arguments.Count == 0)
		{
			return null;
		}

		return GetName(currentTest.Arguments);
	}

	private static string GetName(IReadOnlyList<object?> arguments)
	{
		StringBuilder stringBuilder = new();
		for (int i = 0; i < arguments.Count; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append('_');
			}

			Append(stringBuilder, arguments[i]);
		}

		return Shorten(stringBuilder.ToString());
	}

	/// <remarks>
	///     A <see cref="double" /> or <see cref="float" /> is written with <c>R</c>, as .NET Framework writes only 15
	///     significant digits by default, so that its name matches the one on .NET. A collection is written item by item,
	///     as its <see cref="object.ToString()" /> is usually the name of its type.
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
				stringBuilder.Append(number.ToString("R", CultureInfo.InvariantCulture));
				break;
			case float number:
				stringBuilder.Append(number.ToString("R", CultureInfo.InvariantCulture));
				break;
			case IFormattable formattable:
				stringBuilder.Append(formattable.ToString(null, CultureInfo.InvariantCulture));
				break;
			case ICollection collection:
				stringBuilder.Append('[');
				bool isFirst = true;
				foreach (object? item in collection)
				{
					if (!isFirst)
					{
						stringBuilder.Append(',');
					}

					isFirst = false;
					Append(stringBuilder, item);
				}

				stringBuilder.Append(']');
				break;
			default:
				stringBuilder.Append(value);
				break;
		}
	}

	/// <summary>
	///     Keeps a name of up to <see cref="MaxLength" /> characters, and replaces the end of a longer one with <c>~</c>
	///     and the hash of the whole name.
	/// </summary>
	/// <remarks>
	///     The name is not cut within a surrogate pair, so that it stays valid text.
	/// </remarks>
	private static string Shorten(string name)
	{
		if (name.Length <= MaxLength)
		{
			return name;
		}

		int length = MaxLength - HashLength - 1;
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
