using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Delegates;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     Result for a property.
/// </summary>
public static class PropertyResult
{
	/// <summary>
	///     Result for an <see langword="int" /> property that continues on the <see cref="IThat{TItem}" /> subject.
	/// </summary>
	public class Int<TItem>(
		IThat<TItem> subject,
		Func<TItem, int?> mapper,
		string propertyExpression,
		Action<int?, string>? validation = null)
		: Int<TItem, TItem, IThat<TItem>>(subject, mapper, propertyExpression, validation);

	/// <summary>
	///     Result for an <see langword="int" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The operators, e.g. <c>EqualTo(…)</c>, are extension methods in <see cref="IntPropertyResultExtensions" />.
	///     <para />
	///     See <see cref="String{TValue, TType, TThat}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class Int<TValue, TType, TThat>(
		TThat subject,
		Func<TValue, int?> mapper,
		string propertyExpression,
		Action<int?, string>? validation = null,
		ExpectationGrammars grammars = ExpectationGrammars.None)
		: IIntPropertyResult<AndOrResult<TType, TThat>>
		where TThat : IThat<TValue>
	{
		/// <inheritdoc cref="IIntPropertyResult{TResult}.Validation" />
		Action<int?, string>? IIntPropertyResult<AndOrResult<TType, TThat>>.Validation => validation;

		/// <inheritdoc cref="IIntPropertyResult{TResult}.Compare(PropertyComparison{int})" />
		AndOrResult<TType, TThat> IIntPropertyResult<AndOrResult<TType, TThat>>.Compare(
			PropertyComparison<int> comparison)
			=> new(AddStructConstraint(subject, grammars, mapper, propertyExpression, comparison), subject);
	}

	/// <summary>
	///     Result for a <see langword="long" /> property that continues on the <see cref="IThat{TItem}" /> subject.
	/// </summary>
	public class Long<TItem>(
		IThat<TItem> subject,
		Func<TItem, long?> mapper,
		string propertyExpression,
		Action<long?, string>? validation = null)
		: Long<TItem, TItem, IThat<TItem>>(subject, mapper, propertyExpression, validation);

	/// <summary>
	///     Result for a <see langword="long" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The operators, e.g. <c>EqualTo(…)</c>, are extension methods in <see cref="LongPropertyResultExtensions" />.
	///     <para />
	///     See <see cref="String{TValue, TType, TThat}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class Long<TValue, TType, TThat>(
		TThat subject,
		Func<TValue, long?> mapper,
		string propertyExpression,
		Action<long?, string>? validation = null,
		ExpectationGrammars grammars = ExpectationGrammars.None)
		: ILongPropertyResult<AndOrResult<TType, TThat>>
		where TThat : IThat<TValue>
	{
		/// <inheritdoc cref="ILongPropertyResult{TResult}.Validation" />
		Action<long?, string>? ILongPropertyResult<AndOrResult<TType, TThat>>.Validation => validation;

		/// <inheritdoc cref="ILongPropertyResult{TResult}.Compare(PropertyComparison{long})" />
		AndOrResult<TType, TThat> ILongPropertyResult<AndOrResult<TType, TThat>>.Compare(
			PropertyComparison<long> comparison)
			=> new(AddStructConstraint(subject, grammars, mapper, propertyExpression, comparison), subject);
	}

	/// <summary>
	///     Result for a <typeparamref name="TEnum" /> property that continues on the <see cref="IThat{TItem}" /> subject.
	/// </summary>
	public class Enum<TItem, TEnum>(
		IThat<TItem> subject,
		Func<TItem, TEnum?> mapper,
		string propertyExpression)
		: Enum<TItem, TItem, IThat<TItem>, TEnum>(subject, mapper, propertyExpression)
		where TEnum : struct, Enum;

	/// <summary>
	///     Result for a <typeparamref name="TEnum" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The operators, e.g. <c>EqualTo(…)</c>, are extension methods in <see cref="EnumPropertyResultExtensions" />.
	///     <para />
	///     See <see cref="String{TValue, TType, TThat}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class Enum<TValue, TType, TThat, TEnum>(
		TThat subject,
		Func<TValue, TEnum?> mapper,
		string propertyExpression,
		ExpectationGrammars grammars = ExpectationGrammars.None)
		: IEnumPropertyResult<TEnum, AndOrResult<TType, TThat>>
		where TThat : IThat<TValue>
		where TEnum : struct, Enum
	{
		/// <inheritdoc cref="IEnumPropertyResult{TEnum, TResult}.Compare(PropertyComparison{TEnum})" />
		AndOrResult<TType, TThat> IEnumPropertyResult<TEnum, AndOrResult<TType, TThat>>.Compare(
			PropertyComparison<TEnum> comparison)
			=> new(AddStructConstraint(subject, grammars, mapper, propertyExpression, comparison), subject);
	}

	/// <summary>
	///     Result for a <see cref="TimeSpan" /> property that continues on the <see cref="IThat{TItem}" /> subject.
	/// </summary>
	public class TimeSpan<TItem>(
		IThat<TItem> subject,
		Func<TItem, TimeSpan?> mapper,
		string propertyExpression,
		Action<TimeSpan?, string>? validation = null)
		: TimeSpan<TItem, TItem, IThat<TItem>>(subject, mapper, propertyExpression, validation);

	/// <summary>
	///     Result for a <see cref="TimeSpan" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The operators, e.g. <c>EqualTo(…)</c>, are extension methods in
	///     <see cref="TimeSpanPropertyResultExtensions" />.
	///     <para />
	///     See <see cref="String{TValue, TType, TThat}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class TimeSpan<TValue, TType, TThat>(
		TThat subject,
		Func<TValue, TimeSpan?> mapper,
		string propertyExpression,
		Action<TimeSpan?, string>? validation = null,
		ExpectationGrammars grammars = ExpectationGrammars.None)
		: ITimeSpanPropertyResult<AndOrResult<TType, TThat>>
		where TThat : IThat<TValue>
	{
		/// <inheritdoc cref="ITimeSpanPropertyResult{TResult}.Validation" />
		Action<TimeSpan?, string>? ITimeSpanPropertyResult<AndOrResult<TType, TThat>>.Validation => validation;

		/// <inheritdoc cref="ITimeSpanPropertyResult{TResult}.Compare(PropertyComparison{TimeSpan})" />
		AndOrResult<TType, TThat> ITimeSpanPropertyResult<AndOrResult<TType, TThat>>.Compare(
			PropertyComparison<TimeSpan> comparison)
			=> new(AddStructConstraint(subject, grammars, mapper, propertyExpression, comparison), subject);
	}

	/// <summary>
	///     Result for a <see langword="string" /> property that continues on the <see cref="IThat{TItem}" /> subject.
	/// </summary>
	public class String<TItem>(
		IThat<TItem> subject,
		Func<TItem, string?> mapper,
		string propertyExpression,
		Action<string?, string>? validation = null)
		: String<TItem, TItem, IThat<TItem>>(subject, mapper, propertyExpression, validation);

	/// <summary>
	///     Result for a <see langword="string" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="grammars" /> travel with the continuation instead of with the method, so that the same
	///     property reads in the active voice (<c>with message equal to …</c>) when it is nested under another
	///     expectation and as a standalone sentence (<c>has message equal to …</c>) otherwise.
	///     <para />
	///     <typeparamref name="TValue" /> differs from <typeparamref name="TType" /> when the expectation is narrowed
	///     to a subtype after the value was already provided, e.g. a delegate that supplies an
	///     <see cref="Exception" /> to <c>Throws&lt;TException&gt;()</c>: the constraint has to accept the value the
	///     source provides, while the result carries the narrowed type. It also differs for a nullable subject, whose
	///     value is no longer <see langword="null" /> once the property could be compared.
	/// </remarks>
	public class String<TValue, TType, TThat>(
		TThat subject,
		Func<TValue, string?> mapper,
		string propertyExpression,
		Action<string?, string>? validation = null,
		ExpectationGrammars grammars = ExpectationGrammars.None,
		bool includeValueInContext = false)
		where TThat : IThat<TValue>
	{
		/// <summary>
		///     …contains the <paramref name="expected" /> value.
		/// </summary>
		/// <remarks>
		///     A <see langword="null" /> or empty <paramref name="expected" /> throws, because neither is a substring
		///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
		///     string always does.
		///     <para />
		///     Without a quantifier, the <paramref name="expected" /> value has to occur at least once; specify how often
		///     it has to occur, e.g. with <c>AtLeast(2)</c> or <c>Never()</c>, and how it is matched, e.g. with
		///     <c>AsWildcard()</c>, on the result.
		/// </remarks>
		public StringOccurrenceCountResult<TType, TThat> Containing(
			string expected)
		{
			ThrowIfNullOrEmpty(expected, nameof(expected));
			validation?.Invoke(expected, nameof(expected));
			Quantifier quantifier = new();
			StringEqualityOptions options = new(nameof(expected));
			return new StringOccurrenceCountResult<TType, TThat>(
				subject.Get().ExpectationBuilder
					.AddConstraint(
						(IncludeValueInContext: includeValueInContext, Grammars: grammars, Expected: expected,
							Mapper: mapper, PropertyExpression: propertyExpression, Quantifier: quantifier,
							Options: options),
						static (state, it, constraintGrammars) =>
							new ContainingConstraint<TValue>(
								state.IncludeValueInContext,
								it,
								constraintGrammars | state.Grammars,
								state.Expected,
								state.Mapper,
								state.PropertyExpression,
								state.Quantifier,
								state.Options)),
				subject,
				quantifier,
				options);
		}

		/// <summary>
		///     …ends with the <paramref name="expected" /> value.
		/// </summary>
		/// <remarks>
		///     A <see langword="null" /> or empty <paramref name="expected" /> throws, because neither is a substring
		///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
		///     string always does.
		/// </remarks>
		public StringEqualityResult<TType, TThat> EndingWith(
			string expected)
		{
			ThrowIfNullOrEmpty(expected, nameof(expected));
			validation?.Invoke(expected, nameof(expected));
			StringEqualityOptions options = new(nameof(expected));
			options.AsSuffix();
			return new StringEqualityResult<TType, TThat>(Build(expected, options, false), subject, options);
		}

		/// <summary>
		///     …is equal to the <paramref name="expected" /> value.
		/// </summary>
		public StringEqualityTypeResult<TType, TThat> EqualTo(
			string? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			StringEqualityOptions options = new(nameof(expected));
			return new StringEqualityTypeResult<TType, TThat>(Build(expected, options, false), subject, options);
		}

		/// <summary>
		///     …does not contain the <paramref name="unexpected" /> value.
		/// </summary>
		/// <remarks>
		///     A <see langword="null" /> or empty <paramref name="unexpected" /> throws, because neither is a substring
		///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
		///     string always does.
		/// </remarks>
		public StringEqualityResult<TType, TThat> NotContaining(
			string unexpected)
		{
			ThrowIfNullOrEmpty(unexpected, nameof(unexpected));
			validation?.Invoke(unexpected, nameof(unexpected));
			StringEqualityOptions options = new(nameof(unexpected));
			options.Containing();
			return new StringEqualityResult<TType, TThat>(Build(unexpected, options, true), subject, options);
		}

		/// <summary>
		///     …does not end with the <paramref name="unexpected" /> value.
		/// </summary>
		/// <remarks>
		///     A <see langword="null" /> or empty <paramref name="unexpected" /> throws, because neither is a substring
		///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
		///     string always does.
		/// </remarks>
		public StringEqualityResult<TType, TThat> NotEndingWith(
			string unexpected)
		{
			ThrowIfNullOrEmpty(unexpected, nameof(unexpected));
			validation?.Invoke(unexpected, nameof(unexpected));
			StringEqualityOptions options = new(nameof(unexpected));
			options.AsSuffix();
			return new StringEqualityResult<TType, TThat>(Build(unexpected, options, true), subject, options);
		}

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public StringEqualityTypeResult<TType, TThat> NotEqualTo(
			string? unexpected)
		{
			validation?.Invoke(unexpected, nameof(unexpected));
			StringEqualityOptions options = new(nameof(unexpected));
			return new StringEqualityTypeResult<TType, TThat>(Build(unexpected, options, true), subject, options);
		}

		/// <summary>
		///     …does not start with the <paramref name="unexpected" /> value.
		/// </summary>
		/// <remarks>
		///     A <see langword="null" /> or empty <paramref name="unexpected" /> throws, because neither is a substring
		///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
		///     string always does.
		/// </remarks>
		public StringEqualityResult<TType, TThat> NotStartingWith(
			string unexpected)
		{
			ThrowIfNullOrEmpty(unexpected, nameof(unexpected));
			validation?.Invoke(unexpected, nameof(unexpected));
			StringEqualityOptions options = new(nameof(unexpected));
			options.AsPrefix();
			return new StringEqualityResult<TType, TThat>(Build(unexpected, options, true), subject, options);
		}

		/// <summary>
		///     …starts with the <paramref name="expected" /> value.
		/// </summary>
		/// <remarks>
		///     A <see langword="null" /> or empty <paramref name="expected" /> throws, because neither is a substring
		///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
		///     string always does.
		/// </remarks>
		public StringEqualityResult<TType, TThat> StartingWith(
			string expected)
		{
			ThrowIfNullOrEmpty(expected, nameof(expected));
			validation?.Invoke(expected, nameof(expected));
			StringEqualityOptions options = new(nameof(expected));
			options.AsPrefix();
			return new StringEqualityResult<TType, TThat>(Build(expected, options, false), subject, options);
		}

		private static void ThrowIfNullOrEmpty(string value, string paramName)
		{
#pragma warning disable S3236 // The caller information would name the local parameter instead of the public one it forwards
			value.ThrowIfNull(paramName);
#pragma warning restore S3236
			if (value.Length == 0)
			{
				// ReSharper disable once LocalizableElement
				throw Tracing.WriteException(new ArgumentException($"The '{paramName}' string cannot be empty.", paramName));
			}
		}

		private ExpectationBuilder Build(
			string? expected,
			StringEqualityOptions options,
			bool invert)
			=> subject.Get().ExpectationBuilder
				.AddConstraint(
					(IncludeValueInContext: includeValueInContext, Grammars: grammars, Expected: expected,
						Mapper: mapper, PropertyExpression: propertyExpression, Options: options, Invert: invert),
					static (state, it, constraintGrammars) =>
					{
						StringConstraint<TValue> constraint = new(
							state.IncludeValueInContext,
							it,
							constraintGrammars | state.Grammars,
							state.Expected,
							state.Mapper,
							state.PropertyExpression,
							state.Options);
						return state.Invert ? constraint.Invert() : constraint;
					});
	}

	private static ExpectationBuilder AddStructConstraint<TValue, TProperty>(
		IThat<TValue> subject,
		ExpectationGrammars grammars,
		Func<TValue, TProperty?> mapper,
		string propertyExpression,
		PropertyComparison<TProperty> comparison)
		where TProperty : struct
		=> subject.Get().ExpectationBuilder
			.AddConstraint(
				(Grammars: grammars, Mapper: mapper, PropertyExpression: propertyExpression, Comparison: comparison),
				static (state, it, constraintGrammars) =>
					new StructPropertyConstraint<TValue, TProperty>(
						it, constraintGrammars | state.Grammars,
						state.Mapper,
						state.PropertyExpression,
						state.Comparison));

	private sealed class StructPropertyConstraint<TItem, TProperty>(
		string it,
		ExpectationGrammars grammars,
		Func<TItem, TProperty?> mapper,
		string propertyExpression,
		PropertyComparison<TProperty> comparison)
		: ConstraintResult.WithNotNullValue<TItem>(it, grammars),
			IValueConstraint<TItem>
		where TProperty : struct
	{
		private Exception? _exception;
		private TProperty? _value;

		/// <inheritdoc />
		public override Exception? FailureCause => _exception;

		public ConstraintResult IsMetBy(TItem actual)
		{
			Actual = actual;
			_exception = null;
			try
			{
				_value = mapper(actual);
			}
			catch (Exception exception)
			{
				_exception = exception;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = comparison.Evaluate(_value);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> Append(stringBuilder, comparison.IsNegated);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_exception is not null)
			{
				stringBuilder.Append(propertyExpression).Append(" did throw ")
					.Append(ThatDelegate.FormatForMessage(_exception, indentation));
				return;
			}

			if (Grammars.IsNested() && !Grammars.IsActive())
			{
				// The "whose" clause made the property the subject.
				stringBuilder.Append(propertyExpression).Append(" was ");
			}
			else
			{
				stringBuilder.Append(It).Append(" had ").Append(propertyExpression).Append(' ');
			}

			Formatter.Format(stringBuilder, _value);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> Append(stringBuilder, !comparison.IsNegated);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);

		private void Append(StringBuilder stringBuilder, bool isNegated)
		{
			string negation = isNegated ? "not " : "";
			if (Grammars.IsActive())
			{
				stringBuilder.Append("with ").Append(propertyExpression).Append(' ').Append(negation);
			}
			else if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("whose ").Append(propertyExpression).Append(" is ").Append(negation);
			}
			else
			{
				stringBuilder.Append(isNegated
						? Grammars.Verb("does not have ", "do not have ")
						: Grammars.Verb("has ", "have "))
					.Append(propertyExpression).Append(' ');
			}

			comparison.AppendExpectation(stringBuilder);
		}
	}

	private sealed class StringConstraint<TItem>(
		bool includeValueInContext,
		string it,
		ExpectationGrammars grammars,
		string? expected,
		Func<TItem, string?> mapper,
		string propertyExpression,
		StringEqualityOptions options) : ConstraintResult.WithNotNullValue<TItem>(it, grammars),
		IAsyncContextConstraint<TItem>
	{
		private StringEqualityOptions? _evaluationOptions;
		private Exception? _exception;
		private MatchResult _matchResult;
		private string? _value;

		/// <inheritdoc />
		public override Exception? FailureCause => _exception;

		/// <remarks>
		///     A match type that inspects the content of the value, e.g. a prefix or a pattern, cannot answer for a
		///     <see langword="null" /> value, because it has no content.
		/// </remarks>
		public async ValueTask<ConstraintResult> IsMetBy(TItem actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_exception = null;
			_value = null;
			_evaluationOptions = options.ForEvaluation(context, cancellationToken);
			// An unusable pattern is rejected whichever value the property has, also when it cannot be read.
			options.ValidateExpected(expected);
			try
			{
				_value = mapper(actual);
			}
			catch (Exception exception)
			{
				_exception = exception;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_matchResult = await _evaluationOptions.MatchWithExplanation(_value, expected);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			if (_value is null && options.InspectsSubject)
			{
				Outcome = Outcome.FailureBothWays;
			}

			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (includeValueInContext && !string.IsNullOrEmpty(_value))
			{
				contexts.Add(new ResultContext.Fixed(ToTitle(propertyExpression), _value));
			}

			(_evaluationOptions ?? options).AppendContexts(contexts);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			ExpectationGrammars equalityGrammars = Grammars;
			if (Grammars.IsActive())
			{
				stringBuilder.Append("with ").Append(propertyExpression).Append(' ');
				equalityGrammars &= ~ExpectationGrammars.Active;
			}
			else if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("whose ").Append(propertyExpression).Append(' ');
				// The member becomes the subject of the clause, so the number of the enclosing subject no longer applies.
				equalityGrammars = (equalityGrammars | ExpectationGrammars.Active) & ~ExpectationGrammars.Plural;
			}
			else
			{
				stringBuilder.Append(Grammars.IsNegated()
						? Grammars.Verb("does not have ", "do not have ")
						: Grammars.Verb("has ", "have "))
					.Append(propertyExpression).Append(' ');
				equalityGrammars &= ~ExpectationGrammars.Negated;
			}

			stringBuilder.Append(options.GetExpectation(expected, equalityGrammars));
		}

		/// <remarks>
		///     The property is named, because a chain of member expectations leaves ambiguous which member <c>it</c>
		///     refers to.
		/// </remarks>
		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_exception is not null)
			{
				stringBuilder.Append(propertyExpression).Append(" did throw ")
					.Append(ThatDelegate.FormatForMessage(_exception, indentation));
				return;
			}

			stringBuilder.Append((Grammars.IsNested() &&
			                      !Grammars.IsActive()
					? (_evaluationOptions ?? options).GetExtendedFailure(propertyExpression, Grammars, _value, expected, _matchResult)
					: (_evaluationOptions ?? options).GetExtendedMemberFailure(It, propertyExpression, Grammars, _value, expected,
						_matchResult))
				.Indent(indentation, false));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private static string ToTitle(string propertyExpression)
		=> propertyExpression.Length == 0
			? propertyExpression
			: char.ToUpperInvariant(propertyExpression[0]) + propertyExpression.Substring(1);

	/// <remarks>
	///     The result names the property as the subject that contained the expected value, like the result of
	///     <see cref="StringOccurrenceConstraint" /> names the subject, because a chain of member expectations leaves
	///     ambiguous which member <c>it</c> refers to.
	/// </remarks>
	private sealed class ContainingConstraint<TItem>(
		bool includeValueInContext,
		string it,
		ExpectationGrammars grammars,
		string expected,
		Func<TItem, string?> mapper,
		string propertyExpression,
		Quantifier quantifier,
		StringEqualityOptions options) : ConstraintResult.WithNotNullValue<TItem>(it, grammars),
		IAsyncContextConstraint<TItem>
	{
		private int _count;
		private StringEqualityOptions? _evaluationOptions;
		private Exception? _exception;
		private bool _isFinished;
		private string? _value;

		/// <inheritdoc />
		public override Exception? FailureCause => _exception;

		/// <remarks>
		///     The occurrences are also counted when the property cannot be read or is <see langword="null" />, as in an
		///     empty value, because the options validate the expected value while they count, so an unusable pattern is
		///     rejected whichever value the property has.
		/// </remarks>
		public async ValueTask<ConstraintResult> IsMetBy(TItem actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_exception = null;
			_value = null;
			_evaluationOptions = options.ForEvaluation(context, cancellationToken);
			if (actual is not null)
			{
				try
				{
					_value = mapper(actual);
				}
				catch (Exception exception)
				{
					_exception = exception;
				}
			}

			(_count, _isFinished) =
				await StringOccurrences.Count(_evaluationOptions, _value ?? string.Empty, expected, quantifier);
			if (_exception is not null || _value is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = quantifier.Check(_count, true) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (!string.IsNullOrEmpty(_value))
			{
				if (includeValueInContext)
				{
					contexts.Add(new ResultContext.Fixed(ToTitle(propertyExpression), _value));
				}
				else
				{
					contexts.AddStringContext(ToTitle(propertyExpression), _value, this);
				}

				contexts.AddStringContext("Expected", expected, this);
			}

			(_evaluationOptions ?? options).AppendContexts(contexts);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			string verb;
			string negatedVerb;
			if (Grammars.IsActive())
			{
				verb = $"with {propertyExpression} containing";
				negatedVerb = $"with {propertyExpression} not containing";
			}
			else if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				verb = $"whose {propertyExpression} contains";
				negatedVerb = $"whose {propertyExpression} does not contain";
			}
			else
			{
				verb = $"{Grammars.Verb("has", "have")} {propertyExpression} containing";
				negatedVerb = $"{Grammars.Verb("does not have", "do not have")} {propertyExpression} containing";
			}

			StringOccurrences.AppendExpectation(stringBuilder, quantifier, IsNegated, verb, negatedVerb, expected,
				options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_exception is not null)
			{
				stringBuilder.Append(propertyExpression).Append(" did throw ")
					.Append(ThatDelegate.FormatForMessage(_exception, indentation));
				return;
			}

			if (_value is null)
			{
				if (Grammars.IsNested() && !Grammars.IsActive())
				{
					stringBuilder.Append(propertyExpression)
						.Append(Grammars.SubjectVerb(propertyExpression, " was ", " were "));
				}
				else
				{
					stringBuilder.Append(It).Append(" had ").Append(propertyExpression).Append(' ');
				}

				stringBuilder.Append("<null>");
				return;
			}

			StringOccurrences.AppendResult(stringBuilder, propertyExpression, expected, _value, _count, _isFinished,
				_evaluationOptions ?? options, indentation);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
