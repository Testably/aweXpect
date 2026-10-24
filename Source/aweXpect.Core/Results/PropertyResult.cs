using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
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
	///     Result for a <see cref="DateTimeKind" /> property that continues on the <see cref="IThat{TItem}" /> subject.
	/// </summary>
	public class DateTimeKind<TItem>(
		IThat<TItem> subject,
		Func<TItem, DateTimeKind?> mapper,
		string propertyExpression)
		: DateTimeKind<TItem, TItem, IThat<TItem>>(subject, mapper, propertyExpression);

	/// <summary>
	///     Result for a <see cref="DateTimeKind" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The operators, e.g. <c>EqualTo(…)</c>, are extension methods in
	///     <see cref="DateTimeKindPropertyResultExtensions" />.
	///     <para />
	///     See <see cref="String{TValue, TType, TThat}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class DateTimeKind<TValue, TType, TThat>(
		TThat subject,
		Func<TValue, DateTimeKind?> mapper,
		string propertyExpression,
		ExpectationGrammars grammars = ExpectationGrammars.None)
		: IDateTimeKindPropertyResult<AndOrResult<TType, TThat>>
		where TThat : IThat<TValue>
	{
		/// <inheritdoc cref="IDateTimeKindPropertyResult{TResult}.Compare(PropertyComparison{DateTimeKind})" />
		AndOrResult<TType, TThat> IDateTimeKindPropertyResult<AndOrResult<TType, TThat>>.Compare(
			PropertyComparison<DateTimeKind> comparison)
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
	///     The operators, e.g. <c>EqualTo(…)</c>, are extension methods in <see cref="StringPropertyResultExtensions" />.
	///     <para />
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
		: IStringPropertyResult<TType, TThat>
		where TThat : IThat<TValue>
	{
		/// <inheritdoc cref="IStringPropertyResult{TType, TThat}.Validation" />
		Action<string?, string>? IStringPropertyResult<TType, TThat>.Validation => validation;

		/// <inheritdoc
		///     cref="IStringPropertyResult{TType, TThat}.Compare{TResult}(string?, StringEqualityOptions, bool, Func{ExpectationBuilder, TThat, StringEqualityOptions, TResult})" />
		TResult IStringPropertyResult<TType, TThat>.Compare<TResult>(
			string? expected,
			StringEqualityOptions options,
			bool isNegated,
			Func<ExpectationBuilder, TThat, StringEqualityOptions, TResult> createResult)
			=> createResult(subject.Get().ExpectationBuilder
					.AddConstraint(
						(IncludeValueInContext: includeValueInContext, Grammars: grammars, Expected: expected,
							Mapper: mapper, PropertyExpression: propertyExpression, Options: options, Invert: isNegated),
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
						}),
				subject, options);
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
		IAsyncConstraint<TItem>
	{
		private Exception? _exception;
		private string? _value;

		/// <inheritdoc />
		public override Exception? FailureCause => _exception;

		/// <remarks>
		///     A match type that inspects the content of the value, e.g. a prefix or a pattern, cannot answer for a
		///     <see langword="null" /> value, because it has no content.
		/// </remarks>
		public async ValueTask<ConstraintResult> IsMetBy(TItem actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			_exception = null;
			_value = null;
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

			Outcome = await options.AreConsideredEqual(_value, expected) ? Outcome.Success : Outcome.Failure;
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

			options.AppendContexts(contexts);
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
					? options.GetExtendedFailure(propertyExpression, Grammars, _value, expected)
					: options.GetExtendedMemberFailure(It, propertyExpression, Grammars, _value, expected))
				.Indent(indentation, false));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);

		private static string ToTitle(string propertyExpression)
			=> propertyExpression.Length == 0
				? propertyExpression
				: char.ToUpperInvariant(propertyExpression[0]) + propertyExpression.Substring(1);
	}
}
