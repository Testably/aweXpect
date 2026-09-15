using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Metadata;
using aweXpect.Customization;
using aweXpect.Equivalency;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatObject
{
	/// <summary>
	///     Verifies that the subject is equivalent to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<TSubject, IThat<TSubject>> IsEquivalentTo<TSubject, TExpected>(
		this IThat<TSubject> subject,
		[RequiresMemberMetadata] TExpected expected,
		Func<EquivalencyOptions<TExpected>, EquivalencyOptions>? options = null)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		EquivalencyOptions equivalencyOptions = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get();
		if (options != null)
		{
			equivalencyOptions = options(new EquivalencyOptions<TExpected>(equivalencyOptions));
		}

		expectationBuilder.AddEquivalencyContext(equivalencyOptions);

		ObjectEqualityOptions<TSubject> equalityOptions = new();
		equalityOptions.Equivalent(equivalencyOptions);
		return new AndOrResult<TSubject, IThat<TSubject>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<TSubject, TExpected>(it, grammars, expected, equalityOptions)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is equivalent to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     This overload allows passing a literal <see langword="null" />, for which the generic type cannot be inferred.
	/// </remarks>
	public static AndOrResult<TSubject, IThat<TSubject>> IsEquivalentTo<TSubject>(
		this IThat<TSubject> subject,
		[RequiresMemberMetadata] object? expected,
		Func<EquivalencyOptions<object?>, EquivalencyOptions>? options = null)
		=> subject.IsEquivalentTo<TSubject, object?>(expected, options);

	/// <summary>
	///     Verifies that the subject is not equivalent to the <paramref name="unexpected" /> value.
	/// </summary>
	public static AndOrResult<TSubject, IThat<TSubject>> IsNotEquivalentTo<TSubject, TExpected>(
		this IThat<TSubject> subject,
		[RequiresMemberMetadata] TExpected unexpected,
		Func<EquivalencyOptions<TExpected>, EquivalencyOptions>? options = null)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		EquivalencyOptions equivalencyOptions = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get();
		if (options != null)
		{
			equivalencyOptions = options(new EquivalencyOptions<TExpected>(equivalencyOptions));
		}

		expectationBuilder.AddEquivalencyContext(equivalencyOptions);

		ObjectEqualityOptions<TSubject> equalityOptions = new();
		equalityOptions.Equivalent(equivalencyOptions);
		return new AndOrResult<TSubject, IThat<TSubject>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<TSubject, TExpected>(it, grammars, unexpected, equalityOptions).Invert()),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is not equivalent to the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     This overload allows passing a literal <see langword="null" />, for which the generic type cannot be inferred.
	/// </remarks>
	public static AndOrResult<TSubject, IThat<TSubject>> IsNotEquivalentTo<TSubject>(
		this IThat<TSubject> subject,
		[RequiresMemberMetadata] object? unexpected,
		Func<EquivalencyOptions<object?>, EquivalencyOptions>? options = null)
		=> subject.IsNotEquivalentTo<TSubject, object?>(unexpected, options);
}
