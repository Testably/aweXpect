using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all methods in the filtered collection return type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static MethodsReturnResult<IEnumerable<MethodInfo>?, IThat<IEnumerable<MethodInfo>?>> Return<TReturn>(
		this IThat<IEnumerable<MethodInfo>?> subject)
		=> Return(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that all methods in the filtered collection return type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static MethodsReturnResult<IEnumerable<MethodInfo>?, IThat<IEnumerable<MethodInfo>?>> Return(
		this IThat<IEnumerable<MethodInfo>?> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, false);
		return new MethodsReturnResult<IEnumerable<MethodInfo>?, IThat<IEnumerable<MethodInfo>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<MethodInfo>>((it, grammars)
				=> new ReturnConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all methods in the filtered collection return type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static MethodsReturnResult<IAsyncEnumerable<MethodInfo>?, IThat<IAsyncEnumerable<MethodInfo>?>>
		Return<TReturn>(
			this IThat<IAsyncEnumerable<MethodInfo>?> subject)
		=> Return(subject, typeof(TReturn));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all methods in the filtered collection return type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static MethodsReturnResult<IAsyncEnumerable<MethodInfo>?, IThat<IAsyncEnumerable<MethodInfo>?>> Return(
		this IThat<IAsyncEnumerable<MethodInfo>?> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, false);
		return new MethodsReturnResult<IAsyncEnumerable<MethodInfo>?, IThat<IAsyncEnumerable<MethodInfo>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<MethodInfo>>((it, grammars)
				=> new ReturnConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}
#endif

	/// <summary>
	///     Result that allows chaining additional return types for method collections.
	/// </summary>
	public sealed partial class MethodsReturnResult<TValue, TResult>(
		ExpectationBuilder expectationBuilder,
		TResult subject,
		TypeFilterOptions typeFilterOptions)
		: AndOrResult<TValue, TResult>(expectationBuilder, subject),
			IOptionsProvider<TypeFilterOptions>
		where TResult : IThat<TValue>
	{
		/// <inheritdoc cref="IOptionsProvider{TypeFilterOptions}.Options" />
		TypeFilterOptions IOptionsProvider<TypeFilterOptions>.Options => typeFilterOptions;

		/// <summary>
		///     Allow an alternative return type <typeparamref name="TReturn" />.
		/// </summary>
		public MethodsReturnResult<TValue, TResult> OrReturn<TReturn>()
			=> OrReturn(typeof(TReturn));

		/// <summary>
		///     Allow an alternative return type <paramref name="returnType" />.
		/// </summary>
		public MethodsReturnResult<TValue, TResult> OrReturn(Type returnType)
		{
			typeFilterOptions.RegisterType(returnType, false);
			return this;
		}
	}

	private sealed class ReturnConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeFilterOptions typeFilterOptions)
		: CollectionConstraintResult<MethodInfo>(it, grammars),
			IContextConstraint<IEnumerable<MethodInfo>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<MethodInfo>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<MethodInfo> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				method => typeFilterOptions.Matches(method.ReturnType), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<MethodInfo> actual, IEvaluationContext context)
			=> SetValue(actual, context, method => typeFilterOptions.Matches(method.ReturnType));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			typeFilterOptions.AppendDescription(stringBuilder, Grammars);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching methods");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			typeFilterOptions.AppendDescription(stringBuilder, Grammars);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching methods");
	}
}
