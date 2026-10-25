using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     The option methods of object comparisons on the <see cref="OptionsSlot{TOptions}" /> of an expectation, whose
///     value is the <see cref="ObjectMatchType{TSubject}" />.
/// </summary>
/// <remarks>
///     These options all specify how two objects are compared, so the same option again replaces the earlier match
///     type, while the option of another match type throws.
/// </remarks>
public static class ObjectMatchTypeSlotExtensions
{
	/// <summary>
	///     Specifies the <paramref name="matchType" /> to use for matching two objects.
	/// </summary>
	/// <remarks>
	///     The <see cref="ObjectMatchType{TSubject}.OptionName" /> of the <paramref name="matchType" /> names the option,
	///     and a match type of the same option replaces the earlier one.
	/// </remarks>
	/// <exception cref="ArgumentException">The <paramref name="matchType" /> has no option name.</exception>
	/// <exception cref="InvalidOperationException">
	///     Another option already specified how two objects are compared.
	/// </exception>
	public static OptionsSlot<ObjectMatchType<TSubject>> SetMatchType<TSubject>(
		this OptionsSlot<ObjectMatchType<TSubject>> slot, ObjectMatchType<TSubject> matchType)
	{
		matchType.ThrowIfNull();
		if (matchType.OptionName is not { } optionName)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The match type must have an option name.",
				nameof(matchType)));
		}

		ThrowHelper.ThrowIfCompetingOption(slot.Value.OptionName, optionName);
		slot.Value = matchType;
		return slot;
	}

	/// <summary>
	///     Specifies a specific <see cref="IEqualityComparer{T}" /> to use for comparing <see cref="object" />s.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     Another option already specified how two objects are compared.
	/// </exception>
	public static OptionsSlot<ObjectMatchType<TSubject>> Using<TSubject>(
		this OptionsSlot<ObjectMatchType<TSubject>> slot, IEqualityComparer<object> comparer)
	{
		comparer.ThrowIfNull();
		return slot.SetMatchType(ComparerMatchType<TSubject>.For(comparer));
	}

	/// <summary>
	///     Specifies a specific <see cref="IEqualityComparer{T}" /> to use for comparing
	///     <typeparamref name="TSubject" /> values.
	/// </summary>
	/// <remarks>
	///     A value that is not a <typeparamref name="TSubject" /> is never equal to one.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     Another option already specified how two objects are compared.
	/// </exception>
	public static OptionsSlot<ObjectMatchType<TSubject>> Using<TSubject>(
		this OptionsSlot<ObjectMatchType<TSubject>> slot, IEqualityComparer<TSubject> comparer)
	{
		comparer.ThrowIfNull();
		return slot.SetMatchType(ComparerMatchType<TSubject>.For(comparer));
	}

	/// <summary>
	///     Specifies the <paramref name="tolerance" /> within which the actual value is considered equal to the expected
	///     value.
	/// </summary>
	/// <remarks>
	///     A later tolerance replaces the earlier one, including a relative tolerance.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">
	///     The <paramref name="tolerance" /> is negative or cannot be honoured by the comparison.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	///     Another option already specified how two objects are compared, or the expectation does not compare within a
	///     <typeparamref name="TTolerance" />.
	/// </exception>
	public static OptionsSlot<ObjectMatchType<TSubject>> Within<TSubject, TTolerance>(
		this OptionsSlot<ObjectMatchType<TSubject>> slot, TTolerance tolerance)
	{
		ToleranceHelpers.ThrowIfInvalid(tolerance);
		ThrowHelper.ThrowIfCompetingOption(slot.Value.OptionName, nameof(Within));
		if (slot.Value is not ObjectToleranceMatchType<TSubject, TTolerance> matchType)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new InvalidOperationException(
				$"Within requires a comparison within a tolerance of type {Formatter.Format(typeof(TTolerance))}."));
		}

		slot.Value = matchType.Within(tolerance);
		return slot;
	}

	/// <summary>
	///     Interprets the tolerance relative to the expected value, when the expectation compares within a
	///     <typeparamref name="TTolerance" />.
	/// </summary>
	/// <inheritdoc cref="ObjectToleranceMatchType{TSubject, TTolerance}.WithRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" path="/param" />
	public static OptionsSlot<ObjectMatchType<TSubject>> WithRelativeTolerance<TSubject, TTolerance>(
		this OptionsSlot<ObjectMatchType<TSubject>> slot, Func<TTolerance, TTolerance, TTolerance> absoluteTolerance,
		string unit)
	{
		if (slot.Value is ObjectToleranceMatchType<TSubject, TTolerance> matchType)
		{
			slot.Value = matchType.WithRelativeTolerance(absoluteTolerance, unit);
		}

		return slot;
	}
}
