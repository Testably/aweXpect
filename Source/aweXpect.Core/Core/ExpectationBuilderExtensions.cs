using System;

namespace aweXpect.Core;

/// <summary>
///     Extension methods to simplify usage of the <see cref="ExpectationBuilder" />.
/// </summary>
public static class ExpectationBuilderExtensions
{
	/// <summary>
	///     Specifies a constraint that applies to the member selected
	///     by the <paramref name="memberAccessor" /> displayed as <paramref name="displayName" />.
	/// </summary>
	public static ExpectationBuilder.MemberExpectationBuilder<TSource, TTarget>
		ForMember<TSource, TTarget>(
			this ExpectationBuilder expectationBuilder,
			Func<TSource, TTarget> memberAccessor,
			string displayName,
			bool replaceIt = true)
		=> expectationBuilder.ForMember(
			MemberAccessor<TSource, TTarget>.FromFunc(memberAccessor, displayName),
			replaceIt: replaceIt);
}
