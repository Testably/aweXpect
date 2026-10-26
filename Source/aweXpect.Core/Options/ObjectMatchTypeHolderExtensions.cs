using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     The option methods of object comparisons on the <see cref="OptionsHolder{TOptions}" /> of an expectation, whose
///     value is the <see cref="ObjectMatchType{TElement}" />.
/// </summary>
/// <remarks>
///     These options all specify how two objects are compared, so the same option again replaces the earlier match
///     type, while the option of another match type throws.
/// </remarks>
public static class ObjectMatchTypeHolderExtensions
{
	/// <summary>
	///     Specifies the <paramref name="matchType" /> to use for matching two objects.
	/// </summary>
	/// <remarks>
	///     The <see cref="ObjectMatchType{TElement}.OptionName" /> of the <paramref name="matchType" /> names the option,
	///     and a match type of the same option replaces the earlier one.
	/// </remarks>
	/// <exception cref="ArgumentException">The <paramref name="matchType" /> has no option name.</exception>
	/// <exception cref="InvalidOperationException">
	///     Another option already specified how two objects are compared.
	/// </exception>
	public static OptionsHolder<ObjectMatchType<TElement>> SetMatchType<TElement>(
		this OptionsHolder<ObjectMatchType<TElement>> holder, ObjectMatchType<TElement> matchType)
	{
		matchType.ThrowIfNull();
		if (matchType.OptionName is not { } optionName)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The match type must have an option name.",
				nameof(matchType)));
		}

		ThrowHelper.ThrowIfCompetingOption(holder.Value.OptionName, optionName);
		holder.Value = matchType;
		return holder;
	}

	/// <summary>
	///     Specifies a specific <see cref="IEqualityComparer{T}" /> to use for comparing <see cref="object" />s.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     Another option already specified how two objects are compared.
	/// </exception>
	public static OptionsHolder<ObjectMatchType<TElement>> Using<TElement>(
		this OptionsHolder<ObjectMatchType<TElement>> holder, IEqualityComparer<object> comparer)
	{
		comparer.ThrowIfNull();
		return holder.SetMatchType(ComparerMatchType<TElement>.For(comparer));
	}

	/// <summary>
	///     Specifies a specific <see cref="IEqualityComparer{T}" /> to use for comparing
	///     <typeparamref name="TElement" /> values.
	/// </summary>
	/// <remarks>
	///     A value that is not a <typeparamref name="TElement" /> is never equal to one.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     Another option already specified how two objects are compared.
	/// </exception>
	public static OptionsHolder<ObjectMatchType<TElement>> Using<TElement>(
		this OptionsHolder<ObjectMatchType<TElement>> holder, IEqualityComparer<TElement> comparer)
	{
		comparer.ThrowIfNull();
		return holder.SetMatchType(ComparerMatchType<TElement>.For(comparer));
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
	public static OptionsHolder<ObjectMatchType<TElement>> Within<TElement, TTolerance>(
		this OptionsHolder<ObjectMatchType<TElement>> holder, TTolerance tolerance)
	{
		ToleranceHelpers.ThrowIfInvalid(tolerance);
		ThrowHelper.ThrowIfCompetingOption(holder.Value.OptionName, nameof(Within));
		if (holder.Value is not ObjectToleranceMatchType<TElement, TTolerance> matchType)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new InvalidOperationException(
				$"Within requires a comparison within a tolerance of type {Formatter.Format(typeof(TTolerance))}."));
		}

		holder.Value = matchType.Within(tolerance);
		return holder;
	}

	/// <summary>
	///     Interprets the tolerance relative to the expected value.
	/// </summary>
	/// <inheritdoc cref="ObjectToleranceMatchType{TElement, TTolerance}.SetRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" path="/param" />
	/// <remarks>
	///     A later relative tolerance replaces the earlier one, and a later <c>Within</c> replaces both.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The expectation does not compare within a <typeparamref name="TTolerance" />, or no tolerance was specified
	///     with <c>Within</c>.
	/// </exception>
	public static OptionsHolder<ObjectMatchType<TElement>> SetRelativeTolerance<TElement, TTolerance>(
		this OptionsHolder<ObjectMatchType<TElement>> holder, Func<TTolerance, TTolerance, TTolerance> absoluteTolerance,
		string unit)
	{
		if (holder.Value is not ObjectToleranceMatchType<TElement, TTolerance> matchType)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new InvalidOperationException(
				$"SetRelativeTolerance requires a comparison within a tolerance of type {Formatter.Format(typeof(TTolerance))}."));
		}

		holder.Value = matchType.SetRelativeTolerance(absoluteTolerance, unit);
		return holder;
	}
}
