using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     Result for a property.
/// </summary>
public static class PropertyResult
{
	/// <summary>
	///     Result for an <see langword="int" /> property.
	/// </summary>
	public class Int<TItem>(
		IThat<TItem> subject,
		Func<TItem, int?> mapper,
		string propertyExpression,
		Action<int?, string>? validation = null)
	{
		/// <summary>
		///     …is equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> EqualTo(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, int>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a?.Equals(e) == true,
							$"has {propertyExpression} equal to {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> NotEqualTo(
			int? unexpected)
		{
			validation?.Invoke(unexpected, nameof(unexpected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, int>(
							it, grammars,
							unexpected,
							mapper,
							propertyExpression,
							(a, u) => a?.Equals(u) != true,
							$"has {propertyExpression} not equal to {Formatter.Format(unexpected)}")),
				subject);
		}

		/// <summary>
		///     …is greater than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> GreaterThan(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, int>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a > e,
							$"has {propertyExpression} greater than {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is greater than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> GreaterThanOrEqualTo(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, int>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a >= e,
							$"has {propertyExpression} greater than or equal to {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is less than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> LessThan(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, int>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a < e,
							$"has {propertyExpression} less than {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is less than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> LessThanOrEqualTo(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, int>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a <= e,
							$"has {propertyExpression} less than or equal to {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is between the <paramref name="minimum" />…
		/// </summary>
		public BetweenResult<AndOrResult<TItem, IThat<TItem>>, int?> Between(
			int? minimum)
		{
			validation?.Invoke(minimum, nameof(minimum));
			return new BetweenResult<AndOrResult<TItem, IThat<TItem>>, int?>(maximum =>
			{
				validation?.Invoke(maximum, nameof(maximum));
				return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
						.AddConstraint((it, grammars) =>
							new StructPropertyConstraint<TItem, int>(
								it, grammars,
								minimum,
								mapper,
								propertyExpression,
								(a, e) => a >= e && a <= maximum,
								$"has {propertyExpression} between {Formatter.Format(minimum)} and {Formatter.Format(maximum)}")),
					subject);
			});
		}
	}

	/// <summary>
	///     Result for a <see langword="long" /> property.
	/// </summary>
	public class Long<TItem>(
		IThat<TItem> subject,
		Func<TItem, long?> mapper,
		string propertyExpression,
		Action<long?, string>? validation = null)
	{
		/// <summary>
		///     …is equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> EqualTo(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, long>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a?.Equals(e) == true,
							$"has {propertyExpression} equal to {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> NotEqualTo(
			long? unexpected)
		{
			validation?.Invoke(unexpected, nameof(unexpected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, long>(
							it, grammars,
							unexpected,
							mapper,
							propertyExpression,
							(a, u) => a?.Equals(u) != true,
							$"has {propertyExpression} not equal to {Formatter.Format(unexpected)}")),
				subject);
		}

		/// <summary>
		///     …is greater than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> GreaterThan(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, long>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a > e,
							$"has {propertyExpression} greater than {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is greater than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> GreaterThanOrEqualTo(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, long>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a >= e,
							$"has {propertyExpression} greater than or equal to {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is less than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> LessThan(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, long>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a < e,
							$"has {propertyExpression} less than {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is less than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> LessThanOrEqualTo(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, long>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a <= e,
							$"has {propertyExpression} less than or equal to {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is between the <paramref name="minimum" />…
		/// </summary>
		public BetweenResult<AndOrResult<TItem, IThat<TItem>>, long?> Between(
			long? minimum)
		{
			validation?.Invoke(minimum, nameof(minimum));
			return new BetweenResult<AndOrResult<TItem, IThat<TItem>>, long?>(maximum =>
			{
				validation?.Invoke(maximum, nameof(maximum));
				return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder
						.AddConstraint((it, grammars) =>
							new StructPropertyConstraint<TItem, long>(
								it, grammars,
								minimum,
								mapper,
								propertyExpression,
								(a, e) => a >= e && a <= maximum,
								$"has {propertyExpression} between {Formatter.Format(minimum)} and {Formatter.Format(maximum)}")),
					subject);
			});
		}
	}

	/// <summary>
	///     Result for a <see cref="DateTimeKind" /> property.
	/// </summary>
	public class DateTimeKind<TItem>(
		IThat<TItem> subject,
		Func<TItem, DateTimeKind?> mapper,
		string propertyExpression)
	{
		/// <summary>
		///     …is equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> EqualTo(
			DateTimeKind expected)
			=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, DateTimeKind>(
						it, grammars,
						expected,
						mapper,
						propertyExpression,
						(a, e) => a?.Equals(e) == true,
						$"has {propertyExpression} equal to {Formatter.Format(expected)}")),
				subject);

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> NotEqualTo(
			DateTimeKind unexpected)
			=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, DateTimeKind>(
						it, grammars,
						unexpected,
						mapper,
						propertyExpression,
						(a, u) => a?.Equals(u) != true,
						$"has {propertyExpression} not equal to {Formatter.Format(unexpected)}")),
				subject);
	}

	/// <summary>
	///     Result for a <see cref="TimeSpan" /> property.
	/// </summary>
	public class TimeSpan<TItem>(
		IThat<TItem> subject,
		Func<TItem, TimeSpan?> mapper,
		string propertyExpression,
		Action<TimeSpan?, string>? validation = null)
	{
		/// <summary>
		///     …is equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> EqualTo(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, TimeSpan>(
						it, grammars,
						expected,
						mapper,
						propertyExpression,
						(a, e) => a?.Equals(e) == true,
						$"has {propertyExpression} equal to {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> NotEqualTo(
			TimeSpan? unexpected)
		{
			validation?.Invoke(unexpected, nameof(unexpected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, TimeSpan>(
						it, grammars,
						unexpected,
						mapper,
						propertyExpression,
						(a, u) => a?.Equals(u) != true,
						$"has {propertyExpression} not equal to {Formatter.Format(unexpected)}")),
				subject);
		}

		/// <summary>
		///     …is greater than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> GreaterThan(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, TimeSpan>(
						it, grammars,
						expected,
						mapper,
						propertyExpression,
						(a, e) => a > e,
						$"has {propertyExpression} greater than {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is greater than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> GreaterThanOrEqualTo(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, TimeSpan>(
						it, grammars,
						expected,
						mapper,
						propertyExpression,
						(a, e) => a >= e,
						$"has {propertyExpression} greater than or equal to {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is less than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> LessThan(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, TimeSpan>(
						it, grammars,
						expected,
						mapper,
						propertyExpression,
						(a, e) => a < e,
						$"has {propertyExpression} less than {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is less than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> LessThanOrEqualTo(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, TimeSpan>(
						it, grammars,
						expected,
						mapper,
						propertyExpression,
						(a, e) => a <= e,
						$"has {propertyExpression} less than or equal to {Formatter.Format(expected)}")),
				subject);
		}

		/// <summary>
		///     …is between the <paramref name="minimum" />…
		/// </summary>
		public BetweenResult<AndOrResult<TItem, IThat<TItem>>, TimeSpan?> Between(
			TimeSpan? minimum)
		{
			validation?.Invoke(minimum, nameof(minimum));
			return new BetweenResult<AndOrResult<TItem, IThat<TItem>>, TimeSpan?>(maximum =>
			{
				validation?.Invoke(maximum, nameof(maximum));
				return new AndOrResult<TItem, IThat<TItem>>(subject.Get().ExpectationBuilder.AddConstraint(
						(it, grammars) =>
							new StructPropertyConstraint<TItem, TimeSpan>(
								it, grammars,
								minimum,
								mapper,
								propertyExpression,
								(a, e) => a >= e && a <= maximum,
								$"has {propertyExpression} between {Formatter.Format(minimum)} and {Formatter.Format(maximum)}")),
					subject);
			});
		}
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
			StringEqualityOptions options = new();
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
			StringEqualityOptions options = new();
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
			StringEqualityOptions options = new();
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
			StringEqualityOptions options = new();
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
			value.ThrowIfNull(paramName);
			if (value.Length == 0)
			{
				// ReSharper disable once LocalizableElement
				throw new ArgumentException($"The '{paramName}' string cannot be empty.", paramName);
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
		string expectation) : ConstraintResult.WithNotNullValue<TItem>(it, grammars),
		IValueConstraint<TItem>
		where TProperty : struct
	{
		private TProperty? _value;

		public ConstraintResult IsMetBy(TItem actual)
		{
			Actual = actual;
			_value = mapper(actual);
			Outcome = condition(_value, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectation);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had ").Append(propertyExpression).Append(' ');
			Formatter.Format(stringBuilder, _value);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not ").Append(expectation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
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
				stringBuilder.Append(propertyExpression).Append(' ');
				equalityGrammars |= ExpectationGrammars.Active;
			}
			else
			{
				stringBuilder.Append("has ").Append(propertyExpression).Append(' ');
			}

			stringBuilder.Append(options.GetExpectation(expected, equalityGrammars));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(It, Grammars, _value, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
