using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all methods in the filtered collection return type <typeparamref name="TReturn" />.
	/// </summary>
	public static MethodsReturnResult<IEnumerable<MethodInfo>, IThat<IEnumerable<MethodInfo>>> Return<TReturn>(
		this IThat<IEnumerable<MethodInfo>> subject)
		=> Return(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that all methods in the filtered collection return type <paramref name="returnType" />.
	/// </summary>
	public static MethodsReturnResult<IEnumerable<MethodInfo>, IThat<IEnumerable<MethodInfo>>> Return(
		this IThat<IEnumerable<MethodInfo>> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, false);
		return new MethodsReturnResult<IEnumerable<MethodInfo>, IThat<IEnumerable<MethodInfo>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<MethodInfo>>((it, grammars)
				=> new ReturnConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all methods in the filtered collection return type <typeparamref name="TReturn" />.
	/// </summary>
	public static MethodsReturnResult<IAsyncEnumerable<MethodInfo>, IThat<IAsyncEnumerable<MethodInfo>>>
		Return<TReturn>(
			this IThat<IAsyncEnumerable<MethodInfo>> subject)
		=> Return(subject, typeof(TReturn));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all methods in the filtered collection return type <paramref name="returnType" />.
	/// </summary>
	public static MethodsReturnResult<IAsyncEnumerable<MethodInfo>, IThat<IAsyncEnumerable<MethodInfo>>> Return(
		this IThat<IAsyncEnumerable<MethodInfo>> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, false);
		return new MethodsReturnResult<IAsyncEnumerable<MethodInfo>, IThat<IAsyncEnumerable<MethodInfo>>>(
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
		: CollectionConstraintResult<MethodInfo>(grammars),
			IValueConstraint<IEnumerable<MethodInfo>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<MethodInfo>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<MethodInfo> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, method => typeFilterOptions.Matches(method.ReturnType));
#endif

		public ConstraintResult IsMetBy(IEnumerable<MethodInfo> actual)
			=> SetValue(actual, method => typeFilterOptions.Matches(method.ReturnType));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			typeFilterOptions.AppendDescription(stringBuilder, Grammars);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching methods ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			typeFilterOptions.AppendDescription(stringBuilder, Grammars);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching methods ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
