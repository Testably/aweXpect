using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare an implicit conversion
	///     operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> HaveImplicitConversionOperator<TSource,
		TTarget>(
		this IThat<IEnumerable<Type?>> subject,
		bool inherit = false)
		=> subject.HaveImplicitConversionOperator(typeof(TSource), typeof(TTarget), inherit);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare an implicit conversion
	///     operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> HaveImplicitConversionOperator(
		this IThat<IEnumerable<Type?>> subject,
		Type source,
		Type target,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new HaveConversionOperatorConstraint(it, grammars, true, source, target, inherit)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare an implicit conversion
	///     operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>>
		HaveImplicitConversionOperator<TSource, TTarget>(
			this IThat<IAsyncEnumerable<Type?>> subject,
			bool inherit = false)
		=> subject.HaveImplicitConversionOperator(typeof(TSource), typeof(TTarget), inherit);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare an implicit conversion
	///     operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> HaveImplicitConversionOperator(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Type source,
		Type target,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new HaveConversionOperatorConstraint(it, grammars, true, source, target, inherit)),
			subject);
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare an implicit
	///     conversion operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotHaveImplicitConversionOperator<TSource,
		TTarget>(
		this IThat<IEnumerable<Type?>> subject,
		bool inherit = false)
		=> subject.DoNotHaveImplicitConversionOperator(typeof(TSource), typeof(TTarget), inherit);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare an implicit
	///     conversion operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotHaveImplicitConversionOperator(
		this IThat<IEnumerable<Type?>> subject,
		Type source,
		Type target,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new DoNotHaveConversionOperatorConstraint(it, grammars, true, source, target, inherit)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare an implicit
	///     conversion operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>>
		DoNotHaveImplicitConversionOperator<TSource, TTarget>(
			this IThat<IAsyncEnumerable<Type?>> subject,
			bool inherit = false)
		=> subject.DoNotHaveImplicitConversionOperator(typeof(TSource), typeof(TTarget), inherit);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare an implicit
	///     conversion operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> DoNotHaveImplicitConversionOperator(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Type source,
		Type target,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new DoNotHaveConversionOperatorConstraint(it, grammars, true, source, target, inherit)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare an explicit conversion
	///     operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> HaveExplicitConversionOperator<TSource,
		TTarget>(
		this IThat<IEnumerable<Type?>> subject,
		bool inherit = false)
		=> subject.HaveExplicitConversionOperator(typeof(TSource), typeof(TTarget), inherit);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare an explicit conversion
	///     operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> HaveExplicitConversionOperator(
		this IThat<IEnumerable<Type?>> subject,
		Type source,
		Type target,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new HaveConversionOperatorConstraint(it, grammars, false, source, target, inherit)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare an explicit conversion
	///     operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>>
		HaveExplicitConversionOperator<TSource, TTarget>(
			this IThat<IAsyncEnumerable<Type?>> subject,
			bool inherit = false)
		=> subject.HaveExplicitConversionOperator(typeof(TSource), typeof(TTarget), inherit);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> declare an explicit conversion
	///     operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> HaveExplicitConversionOperator(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Type source,
		Type target,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new HaveConversionOperatorConstraint(it, grammars, false, source, target, inherit)),
			subject);
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare an explicit
	///     conversion operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotHaveExplicitConversionOperator<TSource,
		TTarget>(
		this IThat<IEnumerable<Type?>> subject,
		bool inherit = false)
		=> subject.DoNotHaveExplicitConversionOperator(typeof(TSource), typeof(TTarget), inherit);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare an explicit
	///     conversion operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotHaveExplicitConversionOperator(
		this IThat<IEnumerable<Type?>> subject,
		Type source,
		Type target,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new DoNotHaveConversionOperatorConstraint(it, grammars, false, source, target, inherit)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare an explicit
	///     conversion operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>>
		DoNotHaveExplicitConversionOperator<TSource, TTarget>(
			this IThat<IAsyncEnumerable<Type?>> subject,
			bool inherit = false)
		=> subject.DoNotHaveExplicitConversionOperator(typeof(TSource), typeof(TTarget), inherit);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> declare an explicit
	///     conversion operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> DoNotHaveExplicitConversionOperator(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Type source,
		Type target,
		bool inherit = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new DoNotHaveConversionOperatorConstraint(it, grammars, false, source, target, inherit)),
			subject);
#endif

	private sealed class HaveConversionOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		bool isImplicit,
		Type source,
		Type target,
		bool inherit)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly string _conversionText = ConversionText(isImplicit, source, target);

#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => type.GetConversionOperator(isImplicit, source, target, inherit) is not null);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => type.GetConversionOperator(isImplicit, source, target, inherit) is not null);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have ").Append(_conversionText);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained types without ").Append(_conversionText).Append(' ');
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have ").Append(_conversionText);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained types with ").Append(_conversionText).Append(' ');
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class DoNotHaveConversionOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		bool isImplicit,
		Type source,
		Type target,
		bool inherit)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly string _conversionText = ConversionText(isImplicit, source, target);

#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => type.GetConversionOperator(isImplicit, source, target, inherit) is null);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => type.GetConversionOperator(isImplicit, source, target, inherit) is null);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all do not have ").Append(_conversionText);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained types with ").Append(_conversionText).Append(' ');
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a type with ").Append(_conversionText);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained types without ").Append(_conversionText).Append(' ');
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private static string ConversionText(bool isImplicit, Type source, Type target)
		=> (isImplicit ? "an implicit conversion operator from " : "an explicit conversion operator from ") +
		   Formatter.Format(source) + " to " + Formatter.Format(target);
}
