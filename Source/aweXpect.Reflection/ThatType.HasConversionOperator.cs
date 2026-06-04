using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> declares an implicit conversion operator from
	///     <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> HasImplicitConversionOperator<TSource, TTarget>(
		this IThat<Type?> subject,
		bool inherit = false)
		=> subject.HasImplicitConversionOperator(typeof(TSource), typeof(TTarget), inherit);

	/// <summary>
	///     Verifies that the <see cref="Type" /> declares an implicit conversion operator from
	///     <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> HasImplicitConversionOperator(
		this IThat<Type?> subject,
		Type source,
		Type target,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasConversionOperatorConstraint(it, grammars, true, source, target, inherit)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not declare an implicit conversion operator from
	///     <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotHaveImplicitConversionOperator<TSource, TTarget>(
		this IThat<Type?> subject,
		bool inherit = false)
		=> subject.DoesNotHaveImplicitConversionOperator(typeof(TSource), typeof(TTarget), inherit);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not declare an implicit conversion operator from
	///     <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotHaveImplicitConversionOperator(
		this IThat<Type?> subject,
		Type source,
		Type target,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasConversionOperatorConstraint(it, grammars, true, source, target, inherit).Invert()),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> declares an explicit conversion operator from
	///     <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	public static AndOrResult<Type?, IThat<Type?>> HasExplicitConversionOperator<TSource, TTarget>(
		this IThat<Type?> subject,
		bool inherit = false)
		=> subject.HasExplicitConversionOperator(typeof(TSource), typeof(TTarget), inherit);

	/// <summary>
	///     Verifies that the <see cref="Type" /> declares an explicit conversion operator from
	///     <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	public static AndOrResult<Type?, IThat<Type?>> HasExplicitConversionOperator(
		this IThat<Type?> subject,
		Type source,
		Type target,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasConversionOperatorConstraint(it, grammars, false, source, target, inherit)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not declare an explicit conversion operator from
	///     <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotHaveExplicitConversionOperator<TSource, TTarget>(
		this IThat<Type?> subject,
		bool inherit = false)
		=> subject.DoesNotHaveExplicitConversionOperator(typeof(TSource), typeof(TTarget), inherit);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not declare an explicit conversion operator from
	///     <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotHaveExplicitConversionOperator(
		this IThat<Type?> subject,
		Type source,
		Type target,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasConversionOperatorConstraint(it, grammars, false, source, target, inherit).Invert()),
			subject);

	private sealed class HasConversionOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		bool isImplicit,
		Type source,
		Type target,
		bool inherit)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual.GetConversionOperator(isImplicit, source, target, inherit) is not null
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendConversion(stringBuilder, "has ");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not have ");
			AppendConversionDescription(stringBuilder);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendConversion(stringBuilder, "does not have ");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had ");
			AppendConversionDescription(stringBuilder);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, Actual);
		}

		private void AppendConversion(StringBuilder stringBuilder, string verb)
		{
			stringBuilder.Append(verb);
			AppendConversionDescription(stringBuilder);
		}

		private void AppendConversionDescription(StringBuilder stringBuilder)
		{
			stringBuilder.Append(isImplicit ? "an implicit conversion operator from " : "an explicit conversion operator from ");
			Formatter.Format(stringBuilder, source);
			stringBuilder.Append(" to ");
			Formatter.Format(stringBuilder, target);
		}
	}
}
