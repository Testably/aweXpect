using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new ReturnConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

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
		: ConstraintResult.WithValue<IEnumerable<MethodInfo>>(grammars),
			IValueConstraint<IEnumerable<MethodInfo>>
	{
		public ConstraintResult IsMetBy(IEnumerable<MethodInfo> actual)
		{
			Actual = actual;
			Outcome = actual.All(methodInfo => typeFilterOptions.Matches(methodInfo.ReturnType))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			typeFilterOptions.AppendDescription(stringBuilder, Grammars);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching methods ");
			Formatter.Format(stringBuilder,
				Actual?.Where(methodInfo => !typeFilterOptions.Matches(methodInfo.ReturnType)),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			typeFilterOptions.AppendDescription(stringBuilder, Grammars);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching methods ");
			Formatter.Format(stringBuilder,
				Actual?.Where(methodInfo => typeFilterOptions.Matches(methodInfo.ReturnType)),
				FormattingOptions.Indented(indentation));
		}
	}
}
