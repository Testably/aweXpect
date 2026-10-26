using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class DateTimeKindHelpers
{
	/// <summary>
	///     Creates a check that reports the first value whose <see cref="DateTime.Kind" /> is incompatible with an
	///     earlier value, unless a custom comparer is specified in the <paramref name="options" />.
	/// </summary>
	public static Func<Func<DateTime, string?>?> CreateIncompatibleKindCheck(
		OptionsHolder<CollectionOrderOptions<DateTime>> options)
		=> () =>
		{
			// A custom comparer decides on its own how to handle the kinds, e.g. by converting to UTC.
			if (!ReferenceEquals(options.Value.GetComparer(), Comparer<DateTime>.Default))
			{
				return null;
			}

			Func<DateTime?, string?> check = CreateCheck();
			return value => check(value);
		};

	/// <inheritdoc cref="CreateIncompatibleKindCheck(OptionsHolder{CollectionOrderOptions{DateTime}})" />
	public static Func<Func<DateTime?, string?>?> CreateIncompatibleKindCheck(
		OptionsHolder<CollectionOrderOptions<DateTime?>> options)
		=> () => ReferenceEquals(options.Value.GetComparer(), Comparer<DateTime?>.Default) ? CreateCheck() : null;

	/// <summary>
	///     Creates a check like
	///     <see cref="CreateIncompatibleKindCheck(OptionsHolder{CollectionOrderOptions{DateTime}})" /> for values that are
	///     only known to be a <see cref="DateTime" /> at runtime, as in an untyped collection.
	/// </summary>
	public static Func<Func<TMember, string?>?> CreateIncompatibleRuntimeKindCheck<TMember>(
		OptionsHolder<CollectionOrderOptions<TMember>> options)
		=> () =>
		{
			// Without a comparer, the default of the options is not `Comparer<TMember>.Default` for every `TMember`.
			if (options.Value.HasComparer &&!ReferenceEquals(options.Value.GetComparer(), Comparer<TMember>.Default))
			{
				return null;
			}

			Func<DateTime?, string?> check = CreateCheck();
			return value => value is DateTime dateTime ? check(dateTime) : null;
		};

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
