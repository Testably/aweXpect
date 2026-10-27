using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatObject
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ObjectEqualityResult<object?, IThat<object?>, object?> IsEqualTo(
		this IThat<object?> subject,
		object? expected)
	{
		ObjectMatchType<object?> options = ObjectMatchType<object?>.Default;
		return new ObjectEqualityResult<object?, IThat<object?>, object?>(
			Core.Extending.ExpectationBuilderExtensions.AddEqualityConstraint(
				subject.Get().ExpectationBuilder, expected, options,
				out IOptionsHolderSource<ObjectMatchType<object?>> optionsSource),
			subject,
			optionsSource);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ObjectEqualityResult<T?, IThat<T?>, T?> IsEqualTo<T>(
		this IThat<T?> subject,
		T? expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where T : struct
	{
		ObjectMatchType<T?> options = ObjectMatchType<T?>.Default;
		return new ObjectEqualityResult<T?, IThat<T?>, T?>(
			Core.Extending.ExpectationBuilderExtensions.AddEqualityConstraint(
				subject.Get().ExpectationBuilder, expected, options,
				out IOptionsHolderSource<ObjectMatchType<T?>> optionsSource),
			subject,
			optionsSource);
	}

	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ObjectEqualityResult<T, IThat<T>, T> IsEqualTo<T>(
		this IThat<T> subject,
		T? expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where T : struct
	{
		ObjectMatchType<T> options = ObjectMatchType<T>.Default;
		return new ObjectEqualityResult<T, IThat<T>, T>(
			Core.Extending.ExpectationBuilderExtensions.AddEqualityConstraint(
				subject.Get().ExpectationBuilder, expected, options,
				out IOptionsHolderSource<ObjectMatchType<T>> optionsSource),
			subject,
			optionsSource);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static ObjectEqualityResult<object?, IThat<object?>, object?> IsNotEqualTo(
		this IThat<object?> subject,
		object? unexpected)
	{
		ObjectMatchType<object?> options = ObjectMatchType<object?>.Default;
		return new ObjectEqualityResult<object?, IThat<object?>, object?>(
			Core.Extending.ExpectationBuilderExtensions.AddEqualityConstraint(
				subject.Get().ExpectationBuilder, unexpected, options,
				out IOptionsHolderSource<ObjectMatchType<object?>> optionsSource, isNegated: true),
			subject,
			optionsSource);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static ObjectEqualityResult<T?, IThat<T?>, T?> IsNotEqualTo<T>(
		this IThat<T?> subject,
		T? unexpected,
		[CallerArgumentExpression("unexpected")]
		string doNotPopulateThisValue = "")
		where T : struct
	{
		ObjectMatchType<T?> options = ObjectMatchType<T?>.Default;
		return new ObjectEqualityResult<T?, IThat<T?>, T?>(
			Core.Extending.ExpectationBuilderExtensions.AddEqualityConstraint(
				subject.Get().ExpectationBuilder, unexpected, options,
				out IOptionsHolderSource<ObjectMatchType<T?>> optionsSource, isNegated: true),
			subject,
			optionsSource);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static ObjectEqualityResult<T, IThat<T>, T> IsNotEqualTo<T>(
		this IThat<T> subject,
		T? unexpected,
		[CallerArgumentExpression("unexpected")]
		string doNotPopulateThisValue = "")
		where T : struct
	{
		ObjectMatchType<T> options = ObjectMatchType<T>.Default;
		return new ObjectEqualityResult<T, IThat<T>, T>(
			Core.Extending.ExpectationBuilderExtensions.AddEqualityConstraint(
				subject.Get().ExpectationBuilder, unexpected, options,
				out IOptionsHolderSource<ObjectMatchType<T>> optionsSource, isNegated: true),
			subject,
			optionsSource);
	}
}
