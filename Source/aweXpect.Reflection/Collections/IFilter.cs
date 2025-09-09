using System.Threading.Tasks;

namespace aweXpect.Reflection.Collections;

/// <summary>
///     Filter for <typeparamref name="TEntity" />.
/// </summary>
public interface IFilter<in TEntity>
{
	/// <summary>
	///     Checks if the filter applies to the given <typeparamref name="TEntity" />.
	/// </summary>
#if NET8_0_OR_GREATER
	ValueTask<bool> Applies(TEntity value);
#else
	Task<bool> Applies(TEntity value);
#endif

	/// <summary>
	///     Describes the filter around the given <paramref name="text" />.
	/// </summary>
	string Describes(string text);
}
