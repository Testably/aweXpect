using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     The option methods of object comparisons on the <see cref="OptionsHolder{TOptions}" /> of an expectation, whose
///     value is the <see cref="ObjectMatchType{TSubject}" />.
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
	///     The <see cref="ObjectMatchType{TSubject}.OptionName" /> of the <paramref name="matchType" /> names the option,
	///     and a match type of the same option replaces the earlier one.
	/// </remarks>
	/// <exception cref="ArgumentException">The <paramref name="matchType" /> has no option name.</exception>
	/// <exception cref="InvalidOperationException">
	///     Another option already specified how two objects are compared.
	/// </exception>
	public static OptionsHolder<ObjectMatchType<TSubject>> SetMatchType<TSubject>(
		this OptionsHolder<ObjectMatchType<TSubject>> holder, ObjectMatchType<TSubject> matchType)
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
	public static OptionsHolder<ObjectMatchType<TSubject>> Using<TSubject>(
		this OptionsHolder<ObjectMatchType<TSubject>> holder, IEqualityComparer<object> comparer)
	{
		comparer.ThrowIfNull();
		return holder.SetMatchType(ComparerMatchType<TSubject>.For(comparer));
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
	public static OptionsHolder<ObjectMatchType<TSubject>> Using<TSubject>(
		this OptionsHolder<ObjectMatchType<TSubject>> holder, IEqualityComparer<TSubject> comparer)
	{
		comparer.ThrowIfNull();
		return holder.SetMatchType(ComparerMatchType<TSubject>.For(comparer));
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
	public static OptionsHolder<ObjectMatchType<TSubject>> Within<TSubject, TTolerance>(
		this OptionsHolder<ObjectMatchType<TSubject>> holder, TTolerance tolerance)
	{
		ToleranceHelpers.ThrowIfInvalid(tolerance);
		ThrowHelper.ThrowIfCompetingOption(holder.Value.OptionName, nameof(Within));
		if (holder.Value is not ObjectToleranceMatchType<TSubject, TTolerance> matchType)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new InvalidOperationException(
				$"Within requires a comparison within a tolerance of type {Formatter.Format(typeof(TTolerance))}."));
		}

		holder.Value = matchType.Within(tolerance);
		return holder;
	}

	/// <summary>
	///     Interprets the tolerance relative to the expected value, when the expectation compares within a
	///     <typeparamref name="TTolerance" />.
	/// </summary>
	/// <inheritdoc cref="ObjectToleranceMatchType{TSubject, TTolerance}.WithRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" path="/param" />
	public static OptionsHolder<ObjectMatchType<TSubject>> WithRelativeTolerance<TSubject, TTolerance>(
		this OptionsHolder<ObjectMatchType<TSubject>> holder, Func<TTolerance, TTolerance, TTolerance> absoluteTolerance,
		string unit)
	{
		if (holder.Value is ObjectToleranceMatchType<TSubject, TTolerance> matchType)
		{
			holder.Value = matchType.WithRelativeTolerance(absoluteTolerance, unit);
		}

		return holder;
	}
}
