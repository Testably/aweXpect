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
		IThat<TItem> source,
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
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, int>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a?.Equals(e) == true,
							$"has {propertyExpression} equal to {Formatter.Format(expected)}")),
				source);
		}

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> NotEqualTo(
			int? unexpected)
		{
			validation?.Invoke(unexpected, nameof(unexpected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, int>(
							it, grammars,
							unexpected,
							mapper,
							propertyExpression,
							(a, u) => a?.Equals(u) != true,
							$"has {propertyExpression} not equal to {Formatter.Format(unexpected)}")),
				source);
		}

		/// <summary>
		///     …is greater than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> GreaterThan(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, int>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a > e,
							$"has {propertyExpression} greater than {Formatter.Format(expected)}")),
				source);
		}

		/// <summary>
		///     …is greater than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> GreaterThanOrEqualTo(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, int>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a >= e,
							$"has {propertyExpression} greater than or equal to {Formatter.Format(expected)}")),
				source);
		}

		/// <summary>
		///     …is less than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> LessThan(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, int>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a < e,
							$"has {propertyExpression} less than {Formatter.Format(expected)}")),
				source);
		}

		/// <summary>
		///     …is less than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> LessThanOrEqualTo(
			int? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, int>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a <= e,
							$"has {propertyExpression} less than or equal to {Formatter.Format(expected)}")),
				source);
		}
	}

	/// <summary>
	///     Result for a <see langword="long" /> property.
	/// </summary>
	public class Long<TItem>(
		IThat<TItem> source,
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
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, long>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a?.Equals(e) == true,
							$"has {propertyExpression} equal to {Formatter.Format(expected)}")),
				source);
		}

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> NotEqualTo(
			long? unexpected)
		{
			validation?.Invoke(unexpected, nameof(unexpected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, long>(
							it, grammars,
							unexpected,
							mapper,
							propertyExpression,
							(a, u) => a?.Equals(u) != true,
							$"has {propertyExpression} not equal to {Formatter.Format(unexpected)}")),
				source);
		}

		/// <summary>
		///     …is greater than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> GreaterThan(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, long>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a > e,
							$"has {propertyExpression} greater than {Formatter.Format(expected)}")),
				source);
		}

		/// <summary>
		///     …is greater than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> GreaterThanOrEqualTo(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, long>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a >= e,
							$"has {propertyExpression} greater than or equal to {Formatter.Format(expected)}")),
				source);
		}

		/// <summary>
		///     …is less than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> LessThan(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, long>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a < e,
							$"has {propertyExpression} less than {Formatter.Format(expected)}")),
				source);
		}

		/// <summary>
		///     …is less than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> LessThanOrEqualTo(
			long? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StructPropertyConstraint<TItem, long>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							(a, e) => a <= e,
							$"has {propertyExpression} less than or equal to {Formatter.Format(expected)}")),
				source);
		}
	}

	/// <summary>
	///     Result for a <see cref="DateTimeKind" /> property.
	/// </summary>
	public class DateTimeKind<TItem>(
		IThat<TItem> source,
		Func<TItem, DateTimeKind?> mapper,
		string propertyExpression)
	{
		/// <summary>
		///     …is equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> EqualTo(
			DateTimeKind expected)
			=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, DateTimeKind>(
						it, grammars,
						expected,
						mapper,
						propertyExpression,
						(a, e) => a?.Equals(e) == true,
						$"has {propertyExpression} equal to {Formatter.Format(expected)}")),
				source);

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> NotEqualTo(
			DateTimeKind unexpected)
			=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, DateTimeKind>(
						it, grammars,
						unexpected,
						mapper,
						propertyExpression,
						(a, u) => a?.Equals(u) != true,
						$"has {propertyExpression} not equal to {Formatter.Format(unexpected)}")),
				source);
	}

	/// <summary>
	///     Result for a <see cref="TimeSpan" /> property.
	/// </summary>
	public class TimeSpan<TItem>(
		IThat<TItem> source,
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
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, TimeSpan>(
						it, grammars,
						expected,
						mapper,
						propertyExpression,
						(a, e) => a?.Equals(e) == true,
						$"has {propertyExpression} equal to {Formatter.Format(expected)}")),
				source);
		}

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> NotEqualTo(
			TimeSpan? unexpected)
		{
			validation?.Invoke(unexpected, nameof(unexpected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, TimeSpan>(
						it, grammars,
						unexpected,
						mapper,
						propertyExpression,
						(a, u) => a?.Equals(u) != true,
						$"has {propertyExpression} not equal to {Formatter.Format(unexpected)}")),
				source);
		}

		/// <summary>
		///     …is greater than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> GreaterThan(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, TimeSpan>(
						it, grammars,
						expected,
						mapper,
						propertyExpression,
						(a, e) => a > e,
						$"has {propertyExpression} greater than {Formatter.Format(expected)}")),
				source);
		}

		/// <summary>
		///     …is greater than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> GreaterThanOrEqualTo(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, TimeSpan>(
						it, grammars,
						expected,
						mapper,
						propertyExpression,
						(a, e) => a >= e,
						$"has {propertyExpression} greater than or equal to {Formatter.Format(expected)}")),
				source);
		}

		/// <summary>
		///     …is less than the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> LessThan(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, TimeSpan>(
						it, grammars,
						expected,
						mapper,
						propertyExpression,
						(a, e) => a < e,
						$"has {propertyExpression} less than {Formatter.Format(expected)}")),
				source);
		}

		/// <summary>
		///     …is less than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public AndOrResult<TItem, IThat<TItem>> LessThanOrEqualTo(
			TimeSpan? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			return new AndOrResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new StructPropertyConstraint<TItem, TimeSpan>(
						it, grammars,
						expected,
						mapper,
						propertyExpression,
						(a, e) => a <= e,
						$"has {propertyExpression} less than or equal to {Formatter.Format(expected)}")),
				source);
		}
	}

	/// <summary>
	///     Result for a <see langword="string" /> property.
	/// </summary>
	public class String<TItem>(
		IThat<TItem> source,
		Func<TItem, string?> mapper,
		string propertyExpression,
		Action<string?, string>? validation = null)
	{
		/// <summary>
		///     …does not contain to the <paramref name="unexpected" /> value.
		/// </summary>
		public StringEqualityResult<TItem, IThat<TItem>> NotContaining(
			string? unexpected)
		{
			validation?.Invoke(unexpected, nameof(unexpected));
			StringEqualityOptions options = new();
			options.Containing();
			return new StringEqualityResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StringConstraint<TItem>(
							it, grammars,
							unexpected,
							mapper,
							propertyExpression,
							options).Invert()),
				source,
				options);
		}

		/// <summary>
		///     …contains to the <paramref name="expected" /> value.
		/// </summary>
		public StringEqualityResult<TItem, IThat<TItem>> Containing(
			string? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			StringEqualityOptions options = new();
			options.Containing();
			return new StringEqualityResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StringConstraint<TItem>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							options)),
				source,
				options);
		}

		/// <summary>
		///     …is equal to the <paramref name="expected" /> value.
		/// </summary>
		public StringEqualityResult<TItem, IThat<TItem>> EqualTo(
			string? expected)
		{
			validation?.Invoke(expected, nameof(expected));
			StringEqualityOptions options = new();
			return new StringEqualityResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StringConstraint<TItem>(
							it, grammars,
							expected,
							mapper,
							propertyExpression,
							options)),
				source,
				options);
		}

		/// <summary>
		///     …is not equal to the <paramref name="unexpected" /> value.
		/// </summary>
		public StringEqualityResult<TItem, IThat<TItem>> NotEqualTo(
			string? unexpected)
		{
			validation?.Invoke(unexpected, nameof(unexpected));
			StringEqualityOptions options = new();
			return new StringEqualityResult<TItem, IThat<TItem>>(source.Get().ExpectationBuilder
					.AddConstraint((it, grammars) =>
						new StringConstraint<TItem>(
							it, grammars,
							unexpected,
							mapper,
							propertyExpression,
							options).Invert()),
				source,
				options);
		}
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
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append($"has {propertyExpression} ");
			stringBuilder.Append(options.GetExpectation(expected, Grammars));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had ").Append(propertyExpression).Append(' ');
			Formatter.Format(stringBuilder, _value);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
