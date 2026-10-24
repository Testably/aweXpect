using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject is an <see cref="JsonValueKind.Array" /> with an element at the zero-based
	///     <paramref name="index" /> which satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expectations" /> are only verified, when the array has an element at the
	///     <paramref name="index" />.
	/// </remarks>
	public static AndOrResult<JsonElement, IThat<JsonElement>> HasElementAt(this IThat<JsonElement> subject,
		int index,
		Action<IThatSubject<JsonElement>> expectations)
	{
		ThrowHelper.ThrowIfNegative(index, nameof(index));
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonValueTarget target = JsonValueTarget.ElementAt(index);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(target,
			static IValueConstraint<JsonElement> (target, it, grammars) => target.CreateConstraint(it, grammars));
		JsonValueExpectations.AddOnTarget(expectationBuilder, target, expectations);
		return new AndOrResult<JsonElement, IThat<JsonElement>>(expectationBuilder, subject);
	}
}
