using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all methods in the filtered collection return exactly type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IEnumerable<MethodInfo?>> ReturnExactly<TReturn>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> ReturnExactly(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that all methods in the filtered collection return exactly type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IEnumerable<MethodInfo?>> ReturnExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, true);
		return new ReturnResult<IEnumerable<MethodInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IEnumerable<MethodInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new ReturnConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all methods in the filtered collection return exactly type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IAsyncEnumerable<MethodInfo?>> ReturnExactly<TReturn>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> ReturnExactly(subject, typeof(TReturn));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all methods in the filtered collection return exactly type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IAsyncEnumerable<MethodInfo?>> ReturnExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, true);
		return new ReturnResult<IAsyncEnumerable<MethodInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IAsyncEnumerable<MethodInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new ReturnConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}
#endif

	/// <summary>
	///     Verifies that none of the methods in the filtered collection return exactly type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> DoNotReturnExactly<TReturn>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> DoNotReturnExactly(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that none of the methods in the filtered collection return exactly type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> DoNotReturnExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, true);
		return new AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IEnumerable<MethodInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new DoNotReturnConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the methods in the filtered collection return exactly type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>>
		DoNotReturnExactly<TReturn>(
			this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> DoNotReturnExactly(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that none of the methods in the filtered collection return exactly type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> DoNotReturnExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, true);
		return new AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IAsyncEnumerable<MethodInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new DoNotReturnConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject);
	}
#endif
}
