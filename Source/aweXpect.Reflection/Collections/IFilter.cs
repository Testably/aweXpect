using System.Threading;
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
	/// <param name="value">The entity to check.</param>
	/// <param name="cancellationToken">The token of the enumeration, which a long-running check should observe.</param>
	ValueTask<bool> Applies(TEntity value, CancellationToken cancellationToken);

	/// <summary>
	///     Describes the filter around the given <paramref name="text" />.
	/// </summary>
	string Describes(string text);
}
