using System;
using System.Linq;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional predicates to filter the generic arguments.
/// </summary>
/// <remarks>
///     An immutable value: <see cref="WithPredicate(Func{Type, string?, ValueTask{bool}}, Func{string})" /> returns new
///     options.
/// </remarks>
internal sealed class GenericArgumentFilterOptions
{
	private readonly Func<string>[] _descriptions;
	private readonly Func<Type, string?, ValueTask<bool>>[] _predicates;

	/// <inheritdoc cref="GenericArgumentFilterOptions" />
	public GenericArgumentFilterOptions(Func<Type, string?, ValueTask<bool>> predicate, Func<string> description)
		: this([predicate,], [description,])
	{
	}

	/// <inheritdoc cref="GenericArgumentFilterOptions" />
	public GenericArgumentFilterOptions(Func<Type, string?, bool> predicate, Func<string> description)
		: this([ToAsyncPredicate(predicate),], [description,])
	{
	}

	private GenericArgumentFilterOptions(Func<Type, string?, ValueTask<bool>>[] predicates,
		Func<string>[] descriptions)
	{
		_predicates = predicates;
		_descriptions = descriptions;
	}

	/// <summary>
	///     Returns options with the additional <paramref name="predicate" /> and <paramref name="description" />.
	/// </summary>
	public GenericArgumentFilterOptions WithPredicate(Func<Type, string?, ValueTask<bool>> predicate,
		Func<string> description)
		=> new([.. _predicates, predicate,], [.. _descriptions, description,]);

	/// <summary>
	///     Verifies that the <paramref name="argument" /> matches all predicates.
	/// </summary>
	public ValueTask<bool> Matches(Type argument, string? genericArgumentName = null)
	{
		if (_predicates.Length == 0)
		{
			return new ValueTask<bool>(true);
		}

		return _predicates.AllAsync(predicate
			=> UserCode.InvokeAsync(() => predicate(argument, genericArgumentName), "the predicate"));
	}

	/// <summary>
	///     Returns the combination of all descriptions joined by <c>" and "</c>.
	/// </summary>
	public string GetDescription()
		=> string.Join(" and ", _descriptions.Select(@delegate => @delegate()));

	private static Func<Type, string?, ValueTask<bool>> ToAsyncPredicate(Func<Type, string?, bool> predicate)
		=> (type, name) => new ValueTask<bool>(predicate(type, name));
}
