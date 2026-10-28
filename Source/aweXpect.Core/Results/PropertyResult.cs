using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Delegates;

namespace aweXpect.Results;

/// <summary>
///     Result for a property.
/// </summary>
public static class PropertyResult
{
	/// <summary>
	///     Result for an <see langword="int" /> property that continues on the <see cref="IThat{TType}" /> subject.
	/// </summary>
	public class Int<TType>(
		IThat<TType> subject,
		Func<TType, int?> mapper,
		string propertyExpression,
		Action<int?, string>? validation = null)
		: Int<TType, IThat<TType>, TType>(subject, mapper, propertyExpression, validation);

	/// <summary>
	///     Result for an <see langword="int" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The operators, e.g. <c>EqualTo(…)</c>, are extension methods in <see cref="IntPropertyResultExtensions" />.
	///     <para />
	///     See <see cref="String{TType, TThat, TValue}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class Int<TType, TThat, TValue>(
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
	///     Result for a <see langword="long" /> property that continues on the <see cref="IThat{TType}" /> subject.
	/// </summary>
	public class Long<TType>(
		IThat<TType> subject,
		Func<TType, long?> mapper,
		string propertyExpression,
		Action<long?, string>? validation = null)
		: Long<TType, IThat<TType>, TType>(subject, mapper, propertyExpression, validation);

	/// <summary>
	///     Result for a <see langword="long" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The operators, e.g. <c>EqualTo(…)</c>, are extension methods in <see cref="LongPropertyResultExtensions" />.
	///     <para />
	///     See <see cref="String{TType, TThat, TValue}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class Long<TType, TThat, TValue>(
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
	///     Result for a <typeparamref name="TEnum" /> property that continues on the <see cref="IThat{TType}" /> subject.
	/// </summary>
	public class Enum<TType, TEnum>(
		IThat<TType> subject,
		Func<TType, TEnum?> mapper,
		string propertyExpression)
		: Enum<TType, IThat<TType>, TType, TEnum>(subject, mapper, propertyExpression)
		where TEnum : struct, Enum;

	/// <summary>
	///     Result for a <typeparamref name="TEnum" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The operators, e.g. <c>EqualTo(…)</c>, are extension methods in <see cref="EnumPropertyResultExtensions" />.
	///     <para />
	///     See <see cref="String{TType, TThat, TValue}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class Enum<TType, TThat, TValue, TEnum>(
		TThat subject,
		Func<TValue, TEnum?> mapper,
		string propertyExpression,
		ExpectationGrammars grammars = ExpectationGrammars.None)
		: IEnumPropertyResult<AndOrResult<TType, TThat>, TEnum>
		where TThat : IThat<TValue>
		where TEnum : struct, Enum
	{
		/// <inheritdoc cref="IEnumPropertyResult{TResult, TEnum}.Compare(PropertyComparison{TEnum})" />
		AndOrResult<TType, TThat> IEnumPropertyResult<AndOrResult<TType, TThat>, TEnum>.Compare(
			PropertyComparison<TEnum> comparison)
			=> new(AddStructConstraint(subject, grammars, mapper, propertyExpression, comparison), subject);
	}

	/// <summary>
	///     Result for a <see cref="TimeSpan" /> property that continues on the <see cref="IThat{TType}" /> subject.
	/// </summary>
	public class TimeSpan<TType>(
		IThat<TType> subject,
		Func<TType, TimeSpan?> mapper,
		string propertyExpression,
		Action<TimeSpan?, string>? validation = null)
		: TimeSpan<TType, IThat<TType>, TType>(subject, mapper, propertyExpression, validation);

	/// <summary>
	///     Result for a <see cref="TimeSpan" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The operators, e.g. <c>EqualTo(…)</c>, are extension methods in
	///     <see cref="TimeSpanPropertyResultExtensions" />.
	///     <para />
	///     See <see cref="String{TType, TThat, TValue}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class TimeSpan<TType, TThat, TValue>(
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
	///     Result for a <see langword="double" /> property that continues on the <see cref="IThat{TType}" /> subject.
	/// </summary>
	public class Double<TType>(
		IThat<TType> subject,
		Func<TType, double?> mapper,
		string propertyExpression,
		Action<double?, string>? validation = null)
		: Double<TType, IThat<TType>, TType>(subject, mapper, propertyExpression, validation);

	/// <summary>
	///     Result for a <see langword="double" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The operators, e.g. <c>EqualTo(…)</c>, are extension methods in <see cref="DoublePropertyResultExtensions" />.
	///     <para />
	///     See <see cref="String{TType, TThat, TValue}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class Double<TType, TThat, TValue>(
		TThat subject,
		Func<TValue, double?> mapper,
		string propertyExpression,
		Action<double?, string>? validation = null,
		ExpectationGrammars grammars = ExpectationGrammars.None)
		: IDoublePropertyResult<AndOrResult<TType, TThat>, NumberToleranceResult<TType, TThat, double>>
		where TThat : IThat<TValue>
	{
		/// <inheritdoc cref="IDoublePropertyResult{TResult, TToleranceResult}.Validation" />
		Action<double?, string>?
			IDoublePropertyResult<AndOrResult<TType, TThat>, NumberToleranceResult<TType, TThat, double>>.Validation
			=> validation;

		/// <inheritdoc cref="IDoublePropertyResult{TResult, TToleranceResult}.Compare(PropertyComparison{double})" />
		AndOrResult<TType, TThat>
			IDoublePropertyResult<AndOrResult<TType, TThat>, NumberToleranceResult<TType, TThat, double>>.Compare(
				PropertyComparison<double> comparison)
			=> new(AddStructConstraint(subject, grammars, mapper, propertyExpression, comparison), subject);

		/// <inheritdoc
		///     cref="IDoublePropertyResult{TResult, TToleranceResult}.CompareEquality(PropertyEqualityComparison{double})" />
		NumberToleranceResult<TType, TThat, double>
			IDoublePropertyResult<AndOrResult<TType, TThat>, NumberToleranceResult<TType, TThat, double>>.
			CompareEquality(PropertyEqualityComparison<double> comparison)
			=> new(AddStructConstraint(subject, grammars, mapper, propertyExpression, comparison), subject,
				comparison.Options);
	}

	/// <summary>
	///     Result for a <see langword="decimal" /> property that continues on the <see cref="IThat{TType}" /> subject.
	/// </summary>
	public class Decimal<TType>(
		IThat<TType> subject,
		Func<TType, decimal?> mapper,
		string propertyExpression,
		Action<decimal?, string>? validation = null)
		: Decimal<TType, IThat<TType>, TType>(subject, mapper, propertyExpression, validation);

	/// <summary>
	///     Result for a <see langword="decimal" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The operators, e.g. <c>EqualTo(…)</c>, are extension methods in <see cref="DecimalPropertyResultExtensions" />.
	///     <para />
	///     See <see cref="String{TType, TThat, TValue}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class Decimal<TType, TThat, TValue>(
		TThat subject,
		Func<TValue, decimal?> mapper,
		string propertyExpression,
		Action<decimal?, string>? validation = null,
		ExpectationGrammars grammars = ExpectationGrammars.None)
		: IDecimalPropertyResult<AndOrResult<TType, TThat>, NumberToleranceResult<TType, TThat, decimal>>
		where TThat : IThat<TValue>
	{
		/// <inheritdoc cref="IDecimalPropertyResult{TResult, TToleranceResult}.Validation" />
		Action<decimal?, string>?
			IDecimalPropertyResult<AndOrResult<TType, TThat>, NumberToleranceResult<TType, TThat, decimal>>.Validation
			=> validation;

		/// <inheritdoc cref="IDecimalPropertyResult{TResult, TToleranceResult}.Compare(PropertyComparison{decimal})" />
		AndOrResult<TType, TThat>
			IDecimalPropertyResult<AndOrResult<TType, TThat>, NumberToleranceResult<TType, TThat, decimal>>.Compare(
				PropertyComparison<decimal> comparison)
			=> new(AddStructConstraint(subject, grammars, mapper, propertyExpression, comparison), subject);

		/// <inheritdoc
		///     cref="IDecimalPropertyResult{TResult, TToleranceResult}.CompareEquality(PropertyEqualityComparison{decimal})" />
		NumberToleranceResult<TType, TThat, decimal>
			IDecimalPropertyResult<AndOrResult<TType, TThat>, NumberToleranceResult<TType, TThat, decimal>>.
			CompareEquality(PropertyEqualityComparison<decimal> comparison)
			=> new(AddStructConstraint(subject, grammars, mapper, propertyExpression, comparison), subject,
				comparison.Options);
	}

	/// <summary>
	///     Result for a <see langword="string" /> property that continues on the <see cref="IThat{TType}" /> subject.
	/// </summary>
	public class String<TType>(
		IThat<TType> subject,
		Func<TType, string?> mapper,
		string propertyExpression,
		Action<string?, string>? validation = null)
		: String<TType, IThat<TType>, TType>(subject, mapper, propertyExpression, validation);

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
	public class String<TType, TThat, TValue>(
		TThat subject,
		Func<TValue, string?> mapper,
		string propertyExpression,
		Action<string?, string>? validation = null,
		ExpectationGrammars grammars = ExpectationGrammars.None,
		bool includeValueInContext = false)
		: IStringEqualityPropertyResult<StringEqualityTypeResult<TType, TThat>>,
			IStringAffixPropertyResult<StringEqualityResult<TType, TThat>>,
			IStringOccurrencePropertyResult<StringOccurrenceCountResult<TType, TThat>>
		where TThat : IThat<TValue>
	{
		/// <inheritdoc cref="IStringEqualityPropertyResult{TResult}.Validation" />
		Action<string?, string>? IStringEqualityPropertyResult<StringEqualityTypeResult<TType, TThat>>.Validation
			=> validation;

		/// <inheritdoc cref="IStringAffixPropertyResult{TResult}.Validation" />
		Action<string?, string>? IStringAffixPropertyResult<StringEqualityResult<TType, TThat>>.Validation
			=> validation;

		/// <inheritdoc cref="IStringOccurrencePropertyResult{TResult}.Validation" />
		Action<string?, string>? IStringOccurrencePropertyResult<StringOccurrenceCountResult<TType, TThat>>.Validation
			=> validation;

		/// <inheritdoc cref="IStringEqualityPropertyResult{TResult}.Compare(StringPropertyComparison)" />
		StringEqualityTypeResult<TType, TThat> IStringEqualityPropertyResult<StringEqualityTypeResult<TType, TThat>>.
			Compare(StringPropertyComparison comparison)
			=> new(Build(comparison), subject, comparison.Options);

		/// <inheritdoc cref="IStringAffixPropertyResult{TResult}.Compare(StringPropertyComparison)" />
		StringEqualityResult<TType, TThat> IStringAffixPropertyResult<StringEqualityResult<TType, TThat>>.Compare(
			StringPropertyComparison comparison)
			=> new(Build(comparison), subject, comparison.Options);

		/// <inheritdoc cref="IStringOccurrencePropertyResult{TResult}.Compare(StringPropertyComparison)" />
		StringOccurrenceCountResult<TType, TThat>
			IStringOccurrencePropertyResult<StringOccurrenceCountResult<TType, TThat>>.Compare(
				StringPropertyComparison comparison)
			=> new(subject.Get().ExpectationBuilder
					.AddConstraint(
						(IncludeValueInContext: includeValueInContext, Grammars: grammars, Mapper: mapper,
							PropertyExpression: propertyExpression, Comparison: comparison),
						static (state, it, constraintGrammars) =>
							new ContainingConstraint<TValue>(
								state.IncludeValueInContext,
								it,
								constraintGrammars | state.Grammars,
								state.Mapper,
								state.PropertyExpression,
								state.Comparison)),
				subject,
				comparison.Quantifier!,
				comparison.Options);

		private ExpectationBuilder Build(StringPropertyComparison comparison)
			=> subject.Get().ExpectationBuilder
				.AddConstraint(
					(IncludeValueInContext: includeValueInContext, Grammars: grammars, Mapper: mapper,
						PropertyExpression: propertyExpression, Comparison: comparison),
					static (state, it, constraintGrammars) =>
					{
						StringConstraint<TValue> constraint = new(
							state.IncludeValueInContext,
							it,
							constraintGrammars | state.Grammars,
							state.Mapper,
							state.PropertyExpression,
							state.Comparison);
						return state.Comparison.IsNegated ? constraint.Invert() : constraint;
					});
	}

	private static ExpectationBuilder AddStructConstraint<TValue, TProperty>(
		IThat<TValue> subject,
		ExpectationGrammars grammars,
		Func<TValue, TProperty?> mapper,
		string propertyExpression,
		IPropertyComparison<TProperty> comparison)
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
		IPropertyComparison<TProperty> comparison)
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
		Func<TItem, string?> mapper,
		string propertyExpression,
		StringPropertyComparison comparison) : ConstraintResult.WithNotNullValue<TItem>(it, grammars),
		IAsyncContextConstraint<TItem>
	{
		private Exception? _exception;
		private StringPropertyOutcome _outcome;
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
			_outcome = default;
			// An unusable pattern is rejected whichever value the property has, also when it cannot be read.
			comparison.ValidateExpected();
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

			_outcome = await comparison.Evaluate(_value, context, cancellationToken);
			Outcome = _outcome.OutcomeOfOperator;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (includeValueInContext && !string.IsNullOrEmpty(_value))
			{
				contexts.Add(new ResultContext.Fixed(ToTitle(propertyExpression), _value));
			}

			(_outcome.EvaluationOptions ?? comparison.Options.Value).AppendContexts(contexts);
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

			comparison.AppendExpectation(stringBuilder, equalityGrammars);
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

			if (Grammars.IsNested() && !Grammars.IsActive())
			{
				_outcome.AppendResult(stringBuilder, propertyExpression, Grammars, indentation);
			}
			else
			{
				_outcome.AppendMemberResult(stringBuilder, It, propertyExpression, Grammars, indentation);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);
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
		Func<TItem, string?> mapper,
		string propertyExpression,
		StringPropertyComparison comparison) : ConstraintResult.WithNotNullValue<TItem>(it, grammars),
		IAsyncContextConstraint<TItem>
	{
		private Exception? _exception;
		private StringPropertyOutcome _outcome;
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
			_outcome = default;
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

			_outcome = await comparison.Evaluate(_value, context, cancellationToken);
			Outcome = _exception is null ? _outcome.OutcomeOfOperator : Outcome.FailureBothWays;
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

				contexts.AddStringContext("Expected", comparison.Expected, this);
			}

			(_outcome.EvaluationOptions ?? comparison.Options.Value).AppendContexts(contexts);
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

			comparison.AppendOccurrenceExpectation(stringBuilder, IsNegated, verb, negatedVerb);
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

			_outcome.AppendResult(stringBuilder, propertyExpression, Grammars, indentation);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);
	}
}
