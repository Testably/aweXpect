using System;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Results;

/// <summary>
///     An intermediate result of an only-used-by assertion that collects the types within which the users are
///     searched.
/// </summary>
public sealed class OnlyUsedByResult<TResult>
{
	private readonly Func<Filtered.Types, TResult> _callback;
	private bool _hasScope;

	internal OnlyUsedByResult(Func<Filtered.Types, TResult> callback)
	{
		_callback = callback;
	}

	/// <summary>
	///     …searched in the filtered collection of <paramref name="types" />, whose dependencies are read to find the
	///     users.
	/// </summary>
	/// <remarks>
	///     Only the types in this collection are searched for users, so it should contain every type that could use
	///     the subject, e.g. <c>In.AssemblyContaining&lt;T&gt;().Types()</c>. The cost of the assertion grows with
	///     its size. It can only be called once, as each call adds the expectation.
	/// </remarks>
	public TResult SearchedIn(Filtered.Types types)
	{
		ThrowHelper.ThrowIfNull(types, nameof(types));
		if (_hasScope)
		{
			throw Tracing.WriteException(new InvalidOperationException(
				$"The scope cannot be set again: '{nameof(SearchedIn)}' can only be called once."));
		}

		_hasScope = true;
		return _callback(types);
	}
}
