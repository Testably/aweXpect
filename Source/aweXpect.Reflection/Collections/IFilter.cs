namespace aweXpect.Reflection.Collections;

/// <summary>
///     Filter for <typeparamref name="TEntity" />.
/// </summary>
public interface IFilter<in TEntity>
{
	/// <summary>
	///     Checks if the filter applies to the given <typeparamref name="TEntity" />.
	/// </summary>
	bool Applies(TEntity value);

	/// <summary>
	///     Describes the filter around the given <paramref name="text" />.
	/// </summary>
	string Describes(string text);
}
