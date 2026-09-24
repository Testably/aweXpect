using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
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
	///     See <see cref="String{TValue, TType, TThat}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class Int<TValue, TType, TThat>(
		TThat subject,
		Func<TValue, int?> mapper,
		string propertyExpression,
		Action<int?, string>? validation = null,
		ExpectationGrammars grammars = ExpectationGrammars.None)
		where TThat : IThat<TType>
	{
		/// <summary>
		///     …is equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> EqualTo(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a?.Equals(e) == true,
				$"equal to {Formatter.Format(expected)}");
		}

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> NotEqualTo(
			int? unexpected)
		{
			validation?.Invoke(unexpected, nameof(unexpected));
			return Add(unexpected, (a, u) => a?.Equals(u) != true,
				$"not equal to {Formatter.Format(unexpected)}",
				$"equal to {Formatter.Format(unexpected)}");
		}

		/// <summary>
		///     …is greater than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> GreaterThan(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a > e,
				$"greater than {Formatter.Format(expected)}",
				isOrderedAgainstNull: expected is null);
		}

		/// <summary>
		///     …is greater than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> GreaterThanOrEqualTo(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a >= e,
				$"greater than or equal to {Formatter.Format(expected)}",
				isOrderedAgainstNull: expected is null);
		}

		/// <summary>
		///     …is less than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> LessThan(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a < e,
				$"less than {Formatter.Format(expected)}",
				isOrderedAgainstNull: expected is null);
		}

		/// <summary>
		///     …is less than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> LessThanOrEqualTo(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a <= e,
				$"less than or equal to {Formatter.Format(expected)}",
				isOrderedAgainstNull: expected is null);
		}

		/// <summary>
		///     …is between the <paramref name="minimum" />…
		/// </summary>
		public BetweenResult<AndOrResult<TType, TThat>, int?> Between(
			int? minimum)
		{
			validation?.Invoke(minimum, nameof(minimum));
			return new BetweenResult<AndOrResult<TType, TThat>, int?>(maximum =>
			{
				validation?.Invoke(maximum, nameof(maximum));
				ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
				return Add(minimum, (a, e) => a >= e && a <= maximum,
					$"between {Formatter.Format(minimum)} and {Formatter.Format(maximum)}",
					isOrderedAgainstNull: minimum is null || maximum is null);
			});
		}

		private AndOrResult<TType, TThat> Add(
			int? expected,
			Func<int?, int?, bool> condition,
			string expectation,
			string? negatedExpectation = null,
			bool isOrderedAgainstNull = false)
			=> new(subject.Get().ExpectationBuilder
					.AddConstraint((it, constraintGrammars) =>
						new StructPropertyConstraint<TValue, int>(
							it, constraintGrammars | grammars,
							expected,
							mapper,
							propertyExpression,
							condition,
							expectation,
							negatedExpectation,
							isOrderedAgainstNull: isOrderedAgainstNull)),
				subject);
	}

	/// <summary>
	///     Result for a <see langword="long" /> property that continues on the <see cref="IThat{TItem}" /> subject.
	/// </summary>
	public class Long<TItem>(
		IThat<TItem> subject,
		Func<TItem, long?> mapper,
		string propertyExpression,
		Action<long?, string>? validation = null,
		Func<Exception, bool>? isExpectedPropertyException = null)
		: Long<TItem, TItem, IThat<TItem>>(subject, mapper, propertyExpression, validation,
			ExpectationGrammars.None, isExpectedPropertyException);

	/// <summary>
	///     Result for a <see langword="long" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     See <see cref="String{TValue, TType, TThat}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	///     <para />
	///     The <paramref name="isExpectedPropertyException" /> marks the exceptions from the <paramref name="mapper" />
	///     that are a legitimate answer about the property instead of a defect: they fail the expectation and its
	///     negation alike, because the property was never read.
	/// </remarks>
	public class Long<TValue, TType, TThat>(
		TThat subject,
		Func<TValue, long?> mapper,
		string propertyExpression,
		Action<long?, string>? validation = null,
		ExpectationGrammars grammars = ExpectationGrammars.None,
		Func<Exception, bool>? isExpectedPropertyException = null)
		where TThat : IThat<TType>
	{
		/// <summary>
		///     …is equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> EqualTo(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a?.Equals(e) == true,
				$"equal to {Formatter.Format(expected)}");
		}

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> NotEqualTo(
			long? unexpected)
		{
			validation?.Invoke(unexpected, nameof(unexpected));
			return Add(unexpected, (a, u) => a?.Equals(u) != true,
				$"not equal to {Formatter.Format(unexpected)}",
				$"equal to {Formatter.Format(unexpected)}");
		}

		/// <summary>
		///     …is greater than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> GreaterThan(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a > e,
				$"greater than {Formatter.Format(expected)}",
				isOrderedAgainstNull: expected is null);
		}

		/// <summary>
		///     …is greater than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> GreaterThanOrEqualTo(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a >= e,
				$"greater than or equal to {Formatter.Format(expected)}",
				isOrderedAgainstNull: expected is null);
		}

		/// <summary>
		///     …is less than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> LessThan(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a < e,
				$"less than {Formatter.Format(expected)}",
				isOrderedAgainstNull: expected is null);
		}

		/// <summary>
		///     …is less than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> LessThanOrEqualTo(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a <= e,
				$"less than or equal to {Formatter.Format(expected)}",
				isOrderedAgainstNull: expected is null);
		}

		/// <summary>
		///     …is between the <paramref name="minimum" />…
		/// </summary>
		public BetweenResult<AndOrResult<TType, TThat>, long?> Between(
			long? minimum)
		{
			validation?.Invoke(minimum, nameof(minimum));
			return new BetweenResult<AndOrResult<TType, TThat>, long?>(maximum =>
			{
				validation?.Invoke(maximum, nameof(maximum));
				ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
				return Add(minimum, (a, e) => a >= e && a <= maximum,
					$"between {Formatter.Format(minimum)} and {Formatter.Format(maximum)}",
					isOrderedAgainstNull: minimum is null || maximum is null);
			});
		}

		private AndOrResult<TType, TThat> Add(
			long? expected,
			Func<long?, long?, bool> condition,
			string expectation,
			string? negatedExpectation = null,
			bool isOrderedAgainstNull = false)
			=> new(subject.Get().ExpectationBuilder
					.AddConstraint((it, constraintGrammars) =>
						new StructPropertyConstraint<TValue, long>(
							it, constraintGrammars | grammars,
							expected,
							mapper,
							propertyExpression,
							condition,
							expectation,
							negatedExpectation,
							isExpectedPropertyException,
							isOrderedAgainstNull)),
				subject);
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
	///     See <see cref="String{TValue, TType, TThat}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class DateTimeKind<TValue, TType, TThat>(
		TThat subject,
		Func<TValue, DateTimeKind?> mapper,
		string propertyExpression,
		ExpectationGrammars grammars = ExpectationGrammars.None)
		where TThat : IThat<TType>
	{
		/// <summary>
		///     …is equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> EqualTo(
			DateTimeKind expected)
			=> Add(expected, (a, e) => a?.Equals(e) == true,
				$"equal to {Formatter.Format(expected)}");

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> NotEqualTo(
			DateTimeKind unexpected)
			=> Add(unexpected, (a, u) => a?.Equals(u) != true,
				$"not equal to {Formatter.Format(unexpected)}",
				$"equal to {Formatter.Format(unexpected)}");

		private AndOrResult<TType, TThat> Add(
			DateTimeKind expected,
			Func<DateTimeKind?, DateTimeKind?, bool> condition,
			string expectation,
			string? negatedExpectation = null)
			=> new(subject.Get().ExpectationBuilder
					.AddConstraint((it, constraintGrammars) =>
						new StructPropertyConstraint<TValue, DateTimeKind>(
							it, constraintGrammars | grammars,
							expected,
							mapper,
							propertyExpression,
							condition,
							expectation,
							negatedExpectation)),
				subject);
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
	///     See <see cref="String{TValue, TType, TThat}" /> for the role of the <paramref name="grammars" /> and of the
	///     split between <typeparamref name="TValue" /> and <typeparamref name="TType" />.
	/// </remarks>
	public class TimeSpan<TValue, TType, TThat>(
		TThat subject,
		Func<TValue, TimeSpan?> mapper,
		string propertyExpression,
		Action<TimeSpan?, string>? validation = null,
		ExpectationGrammars grammars = ExpectationGrammars.None)
		where TThat : IThat<TType>
	{
		/// <summary>
		///     …is equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> EqualTo(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a?.Equals(e) == true,
				$"equal to {Formatter.Format(expected)}");
		}

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> NotEqualTo(
			TimeSpan? unexpected)
		{
			validation?.Invoke(unexpected, nameof(unexpected));
			return Add(unexpected, (a, u) => a?.Equals(u) != true,
				$"not equal to {Formatter.Format(unexpected)}",
				$"equal to {Formatter.Format(unexpected)}");
		}

		/// <summary>
		///     …is greater than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> GreaterThan(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a > e,
				$"greater than {Formatter.Format(expected)}",
				isOrderedAgainstNull: expected is null);
		}

		/// <summary>
		///     …is greater than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> GreaterThanOrEqualTo(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a >= e,
				$"greater than or equal to {Formatter.Format(expected)}",
				isOrderedAgainstNull: expected is null);
		}

		/// <summary>
		///     …is less than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> LessThan(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a < e,
				$"less than {Formatter.Format(expected)}",
				isOrderedAgainstNull: expected is null);
		}

		/// <summary>
		///     …is less than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TType, TThat> LessThanOrEqualTo(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return Add(expected, (a, e) => a <= e,
				$"less than or equal to {Formatter.Format(expected)}",
				isOrderedAgainstNull: expected is null);
		}

		/// <summary>
		///     …is between the <paramref name="minimum" />…
		/// </summary>
		public BetweenResult<AndOrResult<TType, TThat>, TimeSpan?> Between(
			TimeSpan? minimum)
		{
			validation?.Invoke(minimum, nameof(minimum));
			return new BetweenResult<AndOrResult<TType, TThat>, TimeSpan?>(maximum =>
			{
				validation?.Invoke(maximum, nameof(maximum));
				ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
				return Add(minimum, (a, e) => a >= e && a <= maximum,
					$"between {Formatter.Format(minimum)} and {Formatter.Format(maximum)}",
					isOrderedAgainstNull: minimum is null || maximum is null);
			});
		}

		private AndOrResult<TType, TThat> Add(
			TimeSpan? expected,
			Func<TimeSpan?, TimeSpan?, bool> condition,
			string expectation,
			string? negatedExpectation = null,
			bool isOrderedAgainstNull = false)
			=> new(subject.Get().ExpectationBuilder
					.AddConstraint((it, constraintGrammars) =>
						new StructPropertyConstraint<TValue, TimeSpan>(
							it, constraintGrammars | grammars,
							expected,
							mapper,
							propertyExpression,
							condition,
							expectation,
							negatedExpectation,
							isOrderedAgainstNull: isOrderedAgainstNull)),
				subject);
	}

	/// <summary>
	///     Result for a <see langword="string" /> property of a <typeparamref name="TValue" /> which continues on
	///     <typeparamref name="TThat" /> with an underlying value of type <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="grammars" /> travel with the continuation instead of with the method, so that the same
	///     property reads in the active voice (<c>with Message equal to …</c>) when it is nested under another
	///     expectation and as a standalone sentence (<c>has Message equal to …</c>) otherwise.
	///     <para />
	///     <typeparamref name="TValue" /> differs from <typeparamref name="TType" /> when the expectation is narrowed
	///     to a subtype after the value was already provided, e.g. a delegate that supplies an
	///     <see cref="Exception" /> to <c>Throws&lt;TException&gt;()</c>: the constraint has to accept the value the
	///     source provides, while the result carries the narrowed type.
	/// </remarks>
	public class String<TValue, TType, TThat>(
		TThat subject,
		Func<TValue, string?> mapper,
		string propertyExpression,
		Action<string?, string>? validation = null,
		ExpectationGrammars grammars = ExpectationGrammars.None,
		bool includeValueInContext = false)
		where TThat : IThat<TType>
	{
		/// <summary>
		///     …contains the <paramref name="expected" /> value.
		/// </summary>
		/// <remarks>
		///     A <see langword="null" /> or empty <paramref name="expected" /> throws, because neither is a substring
		///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
		///     string always does.
		/// </remarks>
		public StringEqualityResult<TType, TThat> Containing(
			string expected)
		{
			ThrowIfNullOrEmpty(expected, nameof(expected));
			validation?.Invoke(expected, nameof(expected));
			StringEqualityOptions options = new();
			options.Containing();
			return new StringEqualityResult<TType, TThat>(Build(expected, options, false), subject, options);
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
			StringEqualityOptions options = new();
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
			StringEqualityOptions options = new();
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
			StringEqualityOptions options = new();
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
				.AddConstraint((expectationBuilder, it, constraintGrammars) =>
				{
					StringConstraint<TValue> constraint = new(
						includeValueInContext ? expectationBuilder : null,
						it,
						constraintGrammars | grammars,
						expected,
						mapper,
						propertyExpression,
						options);
					return invert ? constraint.Invert() : constraint;
				});
	}

	private sealed class StructPropertyConstraint<TItem, TProperty>(
		string it,
		ExpectationGrammars grammars,
		TProperty? expected,
		Func<TItem, TProperty?> mapper,
		string propertyExpression,
		Func<TProperty?, TProperty?, bool> condition,
		string expectation,
		string? negatedExpectation,
		Func<Exception, bool>? isExpectedPropertyException = null,
		bool isOrderedAgainstNull = false)
		: ConstraintResult.WithNotNullValue<TItem>(it, grammars),
		IValueConstraint<TItem>
		where TProperty : struct
	{
		private Exception? _exception;
		private TProperty? _value;

		/// <inheritdoc />
		public override Outcome Outcome
		{
			get => _exception is null && !isOrderedAgainstNull ? base.Outcome : Outcome.Failure;
			protected set => base.Outcome = value;
		}

		/// <inheritdoc />
		public override Exception? FailureCause => _exception;

		public ConstraintResult IsMetBy(TItem actual)
		{
			Actual = actual;
			try
			{
				_value = mapper(actual);
			}
			catch (Exception exception) when (isExpectedPropertyException?.Invoke(exception) == true)
			{
				_exception = exception;
				return this;
			}

			Outcome = condition(_value, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> Append(stringBuilder, false, expectation);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_exception is not null)
			{
				stringBuilder.Append(It).Append(" could not read the ").Append(propertyExpression)
					.Append(", because it did throw ")
					.Append(ThatDelegate.FormatForMessage(_exception, indentation));
				return;
			}

			stringBuilder.Append(It).Append(" had ").Append(propertyExpression).Append(' ');
			Formatter.Format(stringBuilder, _value);
		}

		// A comparison that is itself a negation (`not equal to`) is negated by its positive counterpart instead of by
		// a second `not`.
		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> Append(stringBuilder, negatedExpectation is null, negatedExpectation ?? expectation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);

		private void Append(StringBuilder stringBuilder, bool isNegated, string comparison)
		{
			string negation = isNegated ? "not " : "";
			if (Grammars.HasFlag(ExpectationGrammars.Active))
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

			stringBuilder.Append(comparison);
		}
	}

	private sealed class StringConstraint<TItem>(
		ExpectationBuilder? expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expected,
		Func<TItem, string?> mapper,
		string propertyExpression,
		StringEqualityOptions options) : ConstraintResult.WithNotNullValue<TItem>(it, grammars),
		IAsyncConstraint<TItem>
	{
		private string? _value;

		public async Task<ConstraintResult> IsMetBy(TItem actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			_value = mapper(actual);
			Outcome = await options.AreConsideredEqual(_value, expected) ? Outcome.Success : Outcome.Failure;
			if (expectationBuilder is not null && !string.IsNullOrEmpty(_value))
			{
				expectationBuilder.AddContext(new ResultContext.Fixed(propertyExpression, _value!));
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			ExpectationGrammars equalityGrammars = Grammars;
			if (Grammars.HasFlag(ExpectationGrammars.Active))
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
				stringBuilder.Append(Grammars.Verb("has ", "have ")).Append(propertyExpression).Append(' ');
			}

			stringBuilder.Append(options.GetExpectation(expected, equalityGrammars));
		}

		/// <remarks>
		///     The property is named instead of <c>it</c>, because a chain of member expectations leaves ambiguous
		///     which member <c>it</c> refers to.
		/// </remarks>
		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(propertyExpression, Grammars, _value, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
