using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have an implicit conversion
	///     operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveImplicitConversionOperator<TSource,
		TTarget>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.HaveImplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have an implicit conversion
	///     operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveImplicitConversionOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		HaveConversionOperatorExpectation expectation = new(true, source, target);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <summary>
	///     …have an implicit conversion operator from <typeparamref name="TSource" /> to
	///     <typeparamref name="TTarget" />.
	/// </summary>
	/// <inheritdoc cref="HaveImplicitConversionOperator{TSource, TTarget}(IThat{IEnumerable{Type}})" path="/remarks" />
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveImplicitConversionOperator<TSource,
		TTarget>(
		this IEnumerableItems<Type?> items)
		=> items.HaveImplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     …have an implicit conversion operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <inheritdoc cref="HaveImplicitConversionOperator(IThat{IEnumerable{Type}}, Type, Type)" path="/remarks" />
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveImplicitConversionOperator(
		this IEnumerableItems<Type?> items,
		Type source,
		Type target)
	{
		HaveConversionOperatorExpectation expectation = new(true, source, target);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

	/// <inheritdoc cref="HaveImplicitConversionOperator(IEnumerableItems{Type}, Type, Type)" />
	public static InheritanceScopeResult<TCollection, TCollection> HaveImplicitConversionOperator<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items,
		Type source,
		Type target)
		where TCollection : struct, IEnumerable<Type?>
	{
		HaveConversionOperatorExpectation expectation = new(true, source, target);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveImplicitConversionOperator{TSource, TTarget}(IThat{IEnumerable{Type}})" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>
		HaveImplicitConversionOperator<TSource, TTarget>(
			this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.HaveImplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <inheritdoc cref="HaveImplicitConversionOperator(IThat{IEnumerable{Type}}, Type, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveImplicitConversionOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		HaveConversionOperatorExpectation expectation = new(true, source, target);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <inheritdoc cref="HaveImplicitConversionOperator{TSource, TTarget}(IEnumerableItems{Type})" />
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>
		HaveImplicitConversionOperator<TSource, TTarget>(
			this IAsyncEnumerableItems<Type?> items)
		=> items.HaveImplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <inheritdoc cref="HaveImplicitConversionOperator(IEnumerableItems{Type}, Type, Type)" />
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveImplicitConversionOperator(
		this IAsyncEnumerableItems<Type?> items,
		Type source,
		Type target)
	{
		HaveConversionOperatorExpectation expectation = new(true, source, target);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have an implicit
	///     conversion operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotHaveImplicitConversionOperator<TSource,
		TTarget>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.DoNotHaveImplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have an implicit
	///     conversion operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotHaveImplicitConversionOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		HaveConversionOperatorExpectation expectation = new(true, source, target);
		return new(subject.ExpectNone(expectation), subject, expectation.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotHaveImplicitConversionOperator{TSource, TTarget}(IThat{IEnumerable{Type}})" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>
		DoNotHaveImplicitConversionOperator<TSource, TTarget>(
			this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.DoNotHaveImplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <inheritdoc cref="DoNotHaveImplicitConversionOperator(IThat{IEnumerable{Type}}, Type, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotHaveImplicitConversionOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		HaveConversionOperatorExpectation expectation = new(true, source, target);
		return new(subject.ExpectNone(expectation), subject, expectation.Scope);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have an explicit conversion
	///     operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveExplicitConversionOperator<TSource,
		TTarget>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.HaveExplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have an explicit conversion
	///     operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveExplicitConversionOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		HaveConversionOperatorExpectation expectation = new(false, source, target);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <summary>
	///     …have an explicit conversion operator from <typeparamref name="TSource" /> to
	///     <typeparamref name="TTarget" />.
	/// </summary>
	/// <inheritdoc cref="HaveExplicitConversionOperator{TSource, TTarget}(IThat{IEnumerable{Type}})" path="/remarks" />
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveExplicitConversionOperator<TSource,
		TTarget>(
		this IEnumerableItems<Type?> items)
		=> items.HaveExplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     …have an explicit conversion operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <inheritdoc cref="HaveExplicitConversionOperator(IThat{IEnumerable{Type}}, Type, Type)" path="/remarks" />
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveExplicitConversionOperator(
		this IEnumerableItems<Type?> items,
		Type source,
		Type target)
	{
		HaveConversionOperatorExpectation expectation = new(false, source, target);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

	/// <inheritdoc cref="HaveExplicitConversionOperator(IEnumerableItems{Type}, Type, Type)" />
	public static InheritanceScopeResult<TCollection, TCollection> HaveExplicitConversionOperator<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items,
		Type source,
		Type target)
		where TCollection : struct, IEnumerable<Type?>
	{
		HaveConversionOperatorExpectation expectation = new(false, source, target);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveExplicitConversionOperator{TSource, TTarget}(IThat{IEnumerable{Type}})" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>
		HaveExplicitConversionOperator<TSource, TTarget>(
			this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.HaveExplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <inheritdoc cref="HaveExplicitConversionOperator(IThat{IEnumerable{Type}}, Type, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveExplicitConversionOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		HaveConversionOperatorExpectation expectation = new(false, source, target);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <inheritdoc cref="HaveExplicitConversionOperator{TSource, TTarget}(IEnumerableItems{Type})" />
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>
		HaveExplicitConversionOperator<TSource, TTarget>(
			this IAsyncEnumerableItems<Type?> items)
		=> items.HaveExplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <inheritdoc cref="HaveExplicitConversionOperator(IEnumerableItems{Type}, Type, Type)" />
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveExplicitConversionOperator(
		this IAsyncEnumerableItems<Type?> items,
		Type source,
		Type target)
	{
		HaveConversionOperatorExpectation expectation = new(false, source, target);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have an explicit
	///     conversion operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotHaveExplicitConversionOperator<TSource,
		TTarget>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.DoNotHaveExplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have an explicit
	///     conversion operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotHaveExplicitConversionOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		HaveConversionOperatorExpectation expectation = new(false, source, target);
		return new(subject.ExpectNone(expectation), subject, expectation.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotHaveExplicitConversionOperator{TSource, TTarget}(IThat{IEnumerable{Type}})" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>
		DoNotHaveExplicitConversionOperator<TSource, TTarget>(
			this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.DoNotHaveExplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <inheritdoc cref="DoNotHaveExplicitConversionOperator(IThat{IEnumerable{Type}}, Type, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotHaveExplicitConversionOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		HaveConversionOperatorExpectation expectation = new(false, source, target);
		return new(subject.ExpectNone(expectation), subject, expectation.Scope);
	}
#endif

	private sealed class HaveConversionOperatorExpectation(bool isImplicit, Type source, Type target)
		: ItemExpectation<Type>
	{
		private readonly string _conversionText = ConversionText(isImplicit, source, target);

		public OptionsHolder<MemberScopes> Scope { get; } = new(MemberScopes.None);

		public override ValueTask<PredicateResult> IsMatch(Type item, CancellationToken cancellationToken)
			=> new(item.GetConversionOperator(isImplicit, source, target, !Scope.Value.IsDirect()) is not null);

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		{
			if (Scope.Value.IsDirect())
			{
				stringBuilder.Append(grammars.IsPlural() ? "declare " : "declares ");
			}
			else
			{
				stringBuilder.Append(grammars.IsPlural() ? "have " : "has ");
			}

			stringBuilder.Append(_conversionText);
		}
	}

	private static string ConversionText(bool isImplicit, Type source, Type target)
		=> (isImplicit ? "an implicit conversion operator from " : "an explicit conversion operator from ") +
		   Formatter.Format(source) + " to " + Formatter.Format(target);
}
