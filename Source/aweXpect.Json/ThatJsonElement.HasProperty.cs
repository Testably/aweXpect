using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject is an <see cref="JsonValueKind.Object" /> with a property with the
	///     <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="name" /> is taken literally, so that it can contain <c>.</c>, <c>[</c> or <c>]</c>. A property
	///     with the value <see langword="null" /> exists, unless
	///     <see cref="JsonPropertyResult{TType,TThat}.TreatingNullAsAbsent(bool)" /> is set.
	/// </remarks>
	public static JsonPropertyResult<JsonElement, IThat<JsonElement>> HasProperty(this IThat<JsonElement> subject,
		string name)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		JsonValueTarget target = JsonValueTarget.Property(name);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement>>(
			subject.Get().ExpectationBuilder.AddConstraint(target,
				static IValueConstraint<JsonElement> (target, it, grammars) => target.CreateConstraint(it, grammars)),
			subject, target);
	}

	/// <summary>
	///     Verifies that the subject is an <see cref="JsonValueKind.Object" /> with a property with the
	///     <paramref name="name" /> whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="name" /> is taken literally, so that it can contain <c>.</c>, <c>[</c> or <c>]</c>. The
	///     <paramref name="expectations" /> are only verified, when the property exists.
	/// </remarks>
	public static JsonPropertyResult<JsonElement, IThat<JsonElement>> HasProperty(this IThat<JsonElement> subject,
		string name,
		Action<IThatSubject<JsonElement>> expectations)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonValueTarget target = JsonValueTarget.Property(name);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(target,
			static IValueConstraint<JsonElement> (target, it, grammars) => target.CreateConstraint(it, grammars));
		JsonValueExpectations.AddOnTarget(expectationBuilder, target, expectations);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement>>(expectationBuilder, subject, target);
	}

	/// <summary>
	///     Verifies that the subject is an <see cref="JsonValueKind.Object" /> without a property with the
	///     <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="name" /> is taken literally. A property with the value <see langword="null" /> exists and
	///     fails the expectation, unless <see cref="JsonPropertyResult{TType,TThat}.TreatingNullAsAbsent(bool)" /> is set.
	/// </remarks>
	public static JsonPropertyResult<JsonElement, IThat<JsonElement>> DoesNotHaveProperty(
		this IThat<JsonElement> subject,
		string name)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		JsonValueTarget target = JsonValueTarget.Property(name);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement>>(
			subject.Get().ExpectationBuilder.AddConstraint(target,
				static IValueConstraint<JsonElement> (target, it, grammars)
					=> target.CreateConstraint(it, grammars).Invert()),
			subject, target);
	}
}
