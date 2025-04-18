using System;

namespace aweXpect.Reflection.Collections;

/// <summary>
///     A filter for <typeparamref name="TEntity" /> that can be replaced.
/// </summary>
public interface IChangeableFilter<TEntity> : IFilter<TEntity>
{
	/// <summary>
	///     Updates the original filter by applying the <paramref name="predicate" /> on the original result
	/// and updating the <paramref name="description" />.
	/// </summary>
	void UpdateFilter(Func<bool, TEntity, bool> predicate, Func<string, string> description);
}
