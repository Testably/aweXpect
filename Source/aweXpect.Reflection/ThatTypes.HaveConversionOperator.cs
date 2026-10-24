using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have an implicit conversion
	///     operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.
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
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> HaveImplicitConversionOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type Source, Type Target, ScopeOptions Scope), IEnumerable<Type?>>((source, target, scope),
				static (s, it, grammars)
				=> new HaveConversionOperatorConstraint(it, grammars, true, s.Source, s.Target, s.Scope)),
			subject,
			scope);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have an implicit conversion
	///     operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>
		HaveImplicitConversionOperator<TSource, TTarget>(
			this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.HaveImplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have an implicit conversion
	///     operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveImplicitConversionOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type Source, Type Target, ScopeOptions Scope), IAsyncEnumerable<Type?>>((source, target, scope),
				static (s, it, grammars)
				=> new HaveConversionOperatorConstraint(it, grammars, true, s.Source, s.Target, s.Scope)),
			subject,
			scope);
	}
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have an implicit
	///     conversion operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.
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
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotHaveImplicitConversionOperator(
		this IThat<IEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type Source, Type Target, ScopeOptions Scope), IEnumerable<Type?>>((source, target, scope),
				static (s, it, grammars)
				=> new DoNotHaveConversionOperatorConstraint(it, grammars, true, s.Source, s.Target, s.Scope)),
			subject,
			scope);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have an implicit
	///     conversion operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>
		DoNotHaveImplicitConversionOperator<TSource, TTarget>(
			this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.DoNotHaveImplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have an implicit
	///     conversion operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotHaveImplicitConversionOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type Source, Type Target, ScopeOptions Scope), IAsyncEnumerable<Type?>>((source, target, scope),
				static (s, it, grammars)
				=> new DoNotHaveConversionOperatorConstraint(it, grammars, true, s.Source, s.Target, s.Scope)),
			subject,
			scope);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have an explicit conversion
	///     operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.<br />
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
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.<br />
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
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type Source, Type Target, ScopeOptions Scope), IEnumerable<Type?>>((source, target, scope),
				static (s, it, grammars)
				=> new HaveConversionOperatorConstraint(it, grammars, false, s.Source, s.Target, s.Scope)),
			subject,
			scope);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have an explicit conversion
	///     operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>
		HaveExplicitConversionOperator<TSource, TTarget>(
			this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.HaveExplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have an explicit conversion
	///     operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> HaveExplicitConversionOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type Source, Type Target, ScopeOptions Scope), IAsyncEnumerable<Type?>>((source, target, scope),
				static (s, it, grammars)
				=> new HaveConversionOperatorConstraint(it, grammars, false, s.Source, s.Target, s.Scope)),
			subject,
			scope);
	}
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have an explicit
	///     conversion operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.<br />
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
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.<br />
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
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type Source, Type Target, ScopeOptions Scope), IEnumerable<Type?>>((source, target, scope),
				static (s, it, grammars)
				=> new DoNotHaveConversionOperatorConstraint(it, grammars, false, s.Source, s.Target, s.Scope)),
			subject,
			scope);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have an explicit
	///     conversion operator from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>
		DoNotHaveExplicitConversionOperator<TSource, TTarget>(
			this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.DoNotHaveExplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have an explicit
	///     conversion operator from <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotHaveExplicitConversionOperator(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type source,
		Type target)
	{
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type Source, Type Target, ScopeOptions Scope), IAsyncEnumerable<Type?>>((source, target, scope),
				static (s, it, grammars)
				=> new DoNotHaveConversionOperatorConstraint(it, grammars, false, s.Source, s.Target, s.Scope)),
			subject,
			scope);
	}
#endif

	private sealed class HaveConversionOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		bool isImplicit,
		Type source,
		Type target,
		ScopeOptions scope)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly string _conversionText = ConversionText(isImplicit, source, target);

#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				type => type.GetConversionOperator(isImplicit, source, target, !scope.IsDirect) is not null, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context,
				type => type.GetConversionOperator(isImplicit, source, target, !scope.IsDirect) is not null);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(scope.IsDirect ? "all declare " : "all have ").Append(_conversionText);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It)
				.Append(scope.IsDirect ? " contained types not declaring " : " contained types without ")
				.Append(_conversionText);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(scope.IsDirect ? "not all declare " : "not all have ").Append(_conversionText);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It)
				.Append(scope.IsDirect ? " only contained types declaring " : " only contained types with ")
				.Append(_conversionText);
	}

	private sealed class DoNotHaveConversionOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		bool isImplicit,
		Type source,
		Type target,
		ScopeOptions scope)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly string _conversionText = ConversionText(isImplicit, source, target);

#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				type => type.GetConversionOperator(isImplicit, source, target, !scope.IsDirect) is null, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context,
				type => type.GetConversionOperator(isImplicit, source, target, !scope.IsDirect) is null);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(scope.IsDirect ? "all do not declare " : "all do not have ").Append(_conversionText);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It)
				.Append(scope.IsDirect ? " contained types declaring " : " contained types with ")
				.Append(_conversionText);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(scope.IsDirect ? "also contain a type declaring " : "also contain a type with ")
				.Append(_conversionText);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It)
				.Append(scope.IsDirect ? " only contained types not declaring " : " only contained types without ")
				.Append(_conversionText);
	}

	private static string ConversionText(bool isImplicit, Type source, Type target)
		=> (isImplicit ? "an implicit conversion operator from " : "an explicit conversion operator from ") +
		   Formatter.Format(source) + " to " + Formatter.Format(target);
}
