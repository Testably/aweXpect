using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Reflection.Collections;

/// <summary>
///     An immutable filter for <typeparamref name="TEntity" /> with one of several attributes, which considers
///     inherited attributes unless it is restricted to the declared ones via <see cref="Directly" />.
/// </summary>
/// <remarks>
///     The scope applies to all alternatives, so that it does not depend on the position of <see cref="Directly" /> in
///     the chain.
/// </remarks>
internal sealed class AttributeFilter<TEntity> : IFilter<TEntity>
{
	private readonly Alternative[] _alternatives;
	private readonly bool _isDirect;

	/// <summary>
	///     Creates a filter for the entities that <paramref name="matches" />, which receives whether inherited
	///     attributes are considered, described by <paramref name="describe" />, which receives whether only declared
	///     attributes are considered.
	/// </summary>
	public AttributeFilter(Func<TEntity, bool, bool> matches, Func<bool, string> describe)
		: this([new Alternative(matches, describe),], false)
	{
	}

	private AttributeFilter(Alternative[] alternatives, bool isDirect)
	{
		_alternatives = alternatives;
		_isDirect = isDirect;
	}

	/// <inheritdoc cref="IFilter{TEntity}.Applies(TEntity, CancellationToken)" />
	public ValueTask<bool> Applies(TEntity value, CancellationToken cancellationToken)
	{
		bool inherit = !_isDirect;
		foreach (Alternative alternative in _alternatives)
		{
			if (alternative.Matches(value, inherit))
			{
				return new ValueTask<bool>(true);
			}
		}

		return new ValueTask<bool>(false);
	}

	/// <inheritdoc cref="IFilter{TEntity}.Describes(string)" />
	public string Describes(string text)
	{
		StringBuilder sb = new(text);
		foreach (Alternative alternative in _alternatives)
		{
			sb.Append(alternative.Describe(_isDirect));
		}

		return sb.ToString();
	}

	/// <summary>
	///     Returns a copy of this filter that also accepts the entities that <paramref name="matches" />.
	/// </summary>
	public AttributeFilter<TEntity> Or(Func<TEntity, bool, bool> matches, Func<bool, string> describe)
		=> new([.._alternatives, new Alternative(matches, describe),], _isDirect);

	/// <summary>
	///     Returns a copy of this filter that only considers the attributes declared directly on the entity.
	/// </summary>
	public AttributeFilter<TEntity> Directly()
		=> new(_alternatives, true);

	private sealed class Alternative(Func<TEntity, bool, bool> matches, Func<bool, string> describe)
	{
		public Func<TEntity, bool, bool> Matches { get; } = matches;
		public Func<bool, string> Describe { get; } = describe;
	}
}
