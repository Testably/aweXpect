using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Verifies that the collection is empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> IsEmpty<TItem>(
		this IThat<IEnumerable<TItem>?> subject)
		=> new(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new IsEmptyConstraint<IEnumerable<TItem>?, TItem>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the collection is empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TEnumerable, IThat<TEnumerable?>> IsEmpty<TEnumerable>(
		this IThat<TEnumerable?> subject)
		where TEnumerable : IEnumerable
		=> new(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new IsEmptyConstraint<TEnumerable, object?>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the collection is not empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> IsNotEmpty<TItem>(
		this IThat<IEnumerable<TItem>?> subject)
		=> new(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new IsEmptyConstraint<IEnumerable<TItem>?, TItem>(it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the collection is not empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TEnumerable, IThat<TEnumerable?>> IsNotEmpty<TEnumerable>(
		this IThat<TEnumerable?> subject)
		where TEnumerable : IEnumerable
		=> new(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new IsEmptyConstraint<TEnumerable, object?>(it, grammars).Invert()),
			subject);
}
