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
		OptionsSlot<ObjectEqualityOptions<object?>> options = new(new ObjectEqualityOptions<object?>());
		return new ObjectEqualityResult<object?, IThat<object?>, object?>(
			Core.Extending.ExpectationBuilderExtensions.AddEqualityConstraint(
				subject.Get().ExpectationBuilder, expected, options),
			subject,
			options);
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
		OptionsSlot<ObjectEqualityOptions<T?>> options = new(new ObjectEqualityOptions<T?>());
		return new ObjectEqualityResult<T?, IThat<T?>, T?>(
			Core.Extending.ExpectationBuilderExtensions.AddEqualityConstraint(
				subject.Get().ExpectationBuilder, expected, options),
			subject,
			options);
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
		OptionsSlot<ObjectEqualityOptions<T>> options = new(new ObjectEqualityOptions<T>());
		return new ObjectEqualityResult<T, IThat<T>, T>(
			Core.Extending.ExpectationBuilderExtensions.AddEqualityConstraint(
				subject.Get().ExpectationBuilder, expected, options),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static ObjectEqualityResult<object?, IThat<object?>, object?> IsNotEqualTo(
		this IThat<object?> subject,
		object? unexpected)
	{
		OptionsSlot<ObjectEqualityOptions<object?>> options = new(new ObjectEqualityOptions<object?>());
		return new ObjectEqualityResult<object?, IThat<object?>, object?>(
			Core.Extending.ExpectationBuilderExtensions.AddEqualityConstraint(
				subject.Get().ExpectationBuilder, unexpected, options, isNegated: true),
			subject,
			options);
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
		OptionsSlot<ObjectEqualityOptions<T?>> options = new(new ObjectEqualityOptions<T?>());
		return new ObjectEqualityResult<T?, IThat<T?>, T?>(
			Core.Extending.ExpectationBuilderExtensions.AddEqualityConstraint(
				subject.Get().ExpectationBuilder, unexpected, options, isNegated: true),
			subject,
			options);
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
		OptionsSlot<ObjectEqualityOptions<T>> options = new(new ObjectEqualityOptions<T>());
		return new ObjectEqualityResult<T, IThat<T>, T>(
			Core.Extending.ExpectationBuilderExtensions.AddEqualityConstraint(
				subject.Get().ExpectationBuilder, unexpected, options, isNegated: true),
			subject,
			options);
	}
}
