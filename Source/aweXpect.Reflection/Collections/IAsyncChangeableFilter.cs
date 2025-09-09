using System;
using System.Threading.Tasks;

namespace aweXpect.Reflection.Collections;

/// <summary>
///     A filter for <typeparamref name="TEntity" /> that can be replaced.
/// </summary>
public interface IAsyncChangeableFilter<TEntity> : IFilter<TEntity>
{
	/// <summary>
	///     Updates the original filter by applying the <paramref name="predicate" /> on the original result
	///     and updating the <paramref name="description" />.
	/// </summary>
#if NET8_0_OR_GREATER
	void UpdateFilter(Func<bool, TEntity, ValueTask<bool>> predicate, Func<string, string> description);
#else
	void UpdateFilter(Func<bool, TEntity, Task<bool>> predicate, Func<string, string> description);
#endif
}
