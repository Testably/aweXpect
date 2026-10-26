using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of exactly type
	///     <typeparamref name="THandler" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<EventInfo?>> AreOfExactType<THandler>(
		this IThat<IEnumerable<EventInfo?>?> subject)
		=> AreOfExactType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of exactly type
	///     <paramref name="handlerType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<EventInfo?>> AreOfExactType(
		this IThat<IEnumerable<EventInfo?>?> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.AddType(handlerType, true);
		return new OfTypeResult<IEnumerable<EventInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IEnumerable<EventInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of exactly type
	///     <typeparamref name="THandler" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<EventInfo?>> AreOfExactType<THandler>(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		=> AreOfExactType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of exactly type
	///     <paramref name="handlerType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<EventInfo?>> AreOfExactType(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.AddType(handlerType, true);
		return new OfTypeResult<IAsyncEnumerable<EventInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IAsyncEnumerable<EventInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}
#endif

	/// <summary>
	///     Verifies that none of the events in the filtered collection have a handler of exactly type
	///     <typeparamref name="THandler" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>?>> AreNotOfExactType<THandler>(
		this IThat<IEnumerable<EventInfo?>?> subject)
		=> AreNotOfExactType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that none of the events in the filtered collection have a handler of exactly type
	///     <paramref name="handlerType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>?>> AreNotOfExactType(
		this IThat<IEnumerable<EventInfo?>?> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.AddType(handlerType, true);
		return new AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IEnumerable<EventInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreNotOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the events in the filtered collection have a handler of exactly type
	///     <typeparamref name="THandler" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>?>>
		AreNotOfExactType<THandler>(
			this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		=> AreNotOfExactType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that none of the events in the filtered collection have a handler of exactly type
	///     <paramref name="handlerType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>?>>
		AreNotOfExactType(
			this IThat<IAsyncEnumerable<EventInfo?>?> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.AddType(handlerType, true);
		return new AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IAsyncEnumerable<EventInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreNotOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject);
	}
#endif
}
