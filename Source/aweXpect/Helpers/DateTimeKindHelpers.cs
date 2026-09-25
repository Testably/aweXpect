using System;
using System.Collections.Generic;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class DateTimeKindHelpers
{
	/// <summary>
	///     Creates a check that reports the first value whose <see cref="DateTime.Kind" /> is incompatible with an
	///     earlier value, unless a custom comparer is specified in the <paramref name="options" />.
	/// </summary>
	public static Func<Func<DateTime, string?>?> CreateIncompatibleKindCheck(
		CollectionOrderOptions<DateTime> options)
		=> () =>
		{
			// A custom comparer decides on its own how to handle the kinds, e.g. by converting to UTC.
			if (!ReferenceEquals(options.GetComparer(), Comparer<DateTime>.Default))
			{
				return null;
			}

			Func<DateTime?, string?> check = CreateCheck();
			return value => check(value);
		};

	/// <inheritdoc cref="CreateIncompatibleKindCheck(CollectionOrderOptions{DateTime})" />
	public static Func<Func<DateTime?, string?>?> CreateIncompatibleKindCheck(
		CollectionOrderOptions<DateTime?> options)
		=> () => ReferenceEquals(options.GetComparer(), Comparer<DateTime?>.Default) ? CreateCheck() : null;

	private static Func<DateTime?, string?> CreateCheck()
	{
		DateTime? first = null;
		return value =>
		{
			if (value is null || value.Value.Kind == DateTimeKind.Unspecified)
			{
				return null;
			}

			if (first is null)
			{
				first = value;
				return null;
			}

			if (EqualityHelpers.AreKindCompatible(first.Value.Kind, value.Value.Kind))
			{
				return null;
			}

			return $"had {Formatter.Format(first)} with kind {first.Value.Kind} and {Formatter.Format(value)} with kind {value.Value.Kind}, which cannot be compared";
		};
	}
}
