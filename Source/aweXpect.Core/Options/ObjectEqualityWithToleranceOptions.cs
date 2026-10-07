using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Checks equality of objects with an optional tolerance.
/// </summary>
public class ObjectEqualityWithToleranceOptions<TSubject, TTolerance>(
	Func<TSubject, TSubject, TTolerance, bool> isWithinTolerance,
	Func<TTolerance, string>? toString = null)
	: ObjectEqualityOptions<TSubject>
{
	private Action<TTolerance>? _validateTolerance;

	/// <summary>
	///     Specifies the <paramref name="tolerance" /> within which the actual value is considered equal to the expected
	///     value.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     Another option already specified how two objects are compared, or a tolerance is already set.
	/// </exception>
	public ObjectEqualityOptions<TSubject> Within(TTolerance tolerance)
	{
		ToleranceHelpers.ThrowIfInvalid(tolerance);
		ThrowIfMatchTypeIsSpecified(nameof(Within));
		_validateTolerance?.Invoke(tolerance);
		SetMatchType(new WithinMatchType(() => tolerance, false, isWithinTolerance,
			toString ?? ToleranceHelpers.Format), nameof(Within));
		return this;
	}

	/// <summary>
	///     Specifies the <paramref name="validation" /> that rejects a tolerance passed to
	///     <see cref="Within(TTolerance)" /> which the comparison of <typeparamref name="TSubject" /> cannot honour.
	/// </summary>
	public ObjectEqualityWithToleranceOptions<TSubject, TTolerance> WithToleranceValidation(
		Action<TTolerance> validation)
	{
		_validateTolerance = validation;
		return this;
	}

	/// <summary>
	///     Specifies the <paramref name="defaultTolerance" /> that applies until a tolerance is specified with
	///     <see cref="Within(TTolerance)" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="defaultTolerance" /> is read when the comparison is made or described, or once per
	///     evaluation through <see cref="ForEvaluation(IEvaluationContext, CancellationToken)" />, and it is only named
	///     in the expectation text when it differs from the default value of <typeparamref name="TTolerance" />.
	/// </remarks>
	public ObjectEqualityWithToleranceOptions<TSubject, TTolerance> WithDefaultTolerance(
		Func<TTolerance> defaultTolerance)
	{
		MatchType = new WithinMatchType(defaultTolerance, true, isWithinTolerance,
			toString ?? ToleranceHelpers.Format);
		return this;
	}

	/// <inheritdoc />
	public override ObjectEqualityOptions<TSubject> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (MatchType is not WithinMatchType { IsDefault: true, } matchType)
		{
			return base.ForEvaluation(context, cancellationToken);
		}

		ObjectEqualityOptions<TSubject> options = new();
		options.SetMatchType(matchType.WithResolvedTolerance());
		return options;
	}

	private sealed class WithinMatchType(
		Func<TTolerance> tolerance,
		bool isDefault,
		Func<TSubject, TSubject, TTolerance, bool> isWithinTolerance,
		Func<TTolerance, string> toString)
		: IObjectMatchType
	{
		public bool IsDefault => isDefault;

		public WithinMatchType WithResolvedTolerance()
		{
			TTolerance value = tolerance();
			return new WithinMatchType(() => value, isDefault, isWithinTolerance, toString);
		}

		#region IEquality Members

		/// <inheritdoc cref="IObjectMatchType.AreConsideredEqual{TSubject, TExpected}(TSubject, TExpected)" />
		public ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
			=> new(IsEqual(actual, expected));

		/// <inheritdoc cref="IObjectMatchType.AreConsideredEqualWithExplanation{TActual, TExpected}(TActual, TExpected)" />
		public ValueTask<IObjectMatchResult> AreConsideredEqualWithExplanation<TActual, TExpected>(TActual actual,
			TExpected expected)
			=> ObjectEqualityOptions.ExplainWithActualValue(IsEqual(actual, expected));

		/// <inheritdoc cref="IObjectMatchType.GetExpectation(string, ExpectationGrammars)" />
		public string GetExpectation(string expected, ExpectationGrammars grammars)
			=> $"{grammars.Verb("is", "are")} {(grammars.HasFlag(ExpectationGrammars.Negated) ? "not " : "")}equal to {expected}" + ToString();

		/// <inheritdoc cref="IObjectMatchType.AppendContexts(ResultContextCollector)" />
		public void AppendContexts(ResultContextCollector contexts)
		{
			// The tolerance is part of the expectation, so the comparison has no options to explain.
		}

		/// <inheritdoc cref="IObjectMatchType.PrependItemAndComparison" />
		public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
			=> ObjectEqualityOptions.GetItemExpectation(expected, itemNoun, comparison) + ToString();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
		{
			TTolerance value = tolerance();
			return isDefault && EqualityComparer<TTolerance>.Default.Equals(value, default!)
				? ""
				: toString.Invoke(value);
		}

		private bool IsEqual<TActual, TExpected>(TActual actual, TExpected expected)
		{
			if (actual is null && expected is null)
			{
				return true;
			}

			return actual is TSubject typedActual && expected is TSubject typedExpected &&
			       isWithinTolerance(typedActual, typedExpected, tolerance());
		}

		#endregion
	}
}
