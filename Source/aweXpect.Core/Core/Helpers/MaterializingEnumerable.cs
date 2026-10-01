using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Helpers;

internal sealed class MaterializingEnumerable<T> : IEnumerable<T>, ICountable, IMaterialization
{
	private readonly IEnumerator<T> _enumerator;
	private readonly List<T> _materializedItems = new();
	private bool _isSourceDisposed;
	private Exception? _sourceException;

	private MaterializingEnumerable(IEnumerable<T> enumerable)
	{
		_enumerator = enumerable.GetEnumerator();
	}

	public int? Count { get; private set; }

	public static IEnumerable<T> Wrap(IEnumerable<T> enumerable)
	{
		if (enumerable is ICollection<T> or MaterializingEnumerable<T>)
		{
			return enumerable;
		}

		return new MaterializingEnumerable<T>(enumerable);
	}

	#region IEnumerable<T> Members

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	/// <remarks>
	///     The items are replayed by index, so that an enumeration nested in another one continues where the other one
	///     stopped, instead of cutting it short.
	/// </remarks>
	public IEnumerator<T> GetEnumerator()
	{
		int index = 0;
		// Stryker disable once Conditional : a mutated condition keeps appending the exhausted enumerator's current item until the test host runs out of memory, which costs a minute per mutant and cannot be killed any cheaper
		while (index < _materializedItems.Count || MoveNext())
		{
			if (index == _materializedItems.Count)
			{
				_materializedItems.Add(_enumerator.Current);
			}

			yield return _materializedItems[index++];
		}
	}

	#endregion

	/// <inheritdoc cref="IMaterialization.ReleaseSource()" />
	public Task ReleaseSource()
	{
		DisposeSource();
		return Task.CompletedTask;
	}

	/// <remarks>
	///     A source that threw is not advanced again, but every further enumeration throws the same exception, so that
	///     it cannot be mistaken for the end of the source. The source is disposed once it threw or is exhausted; a
	///     source that is only read partially is disposed when it is released.
	/// </remarks>
	private bool MoveNext()
	{
		if (_sourceException is not null)
		{
			ExceptionDispatchInfo.Capture(_sourceException).Throw();
		}

		if (_isSourceDisposed)
		{
			return false;
		}

		try
		{
			if (UserCode.Invoke(_enumerator.MoveNext))
			{
				return true;
			}
		}
		catch (Exception exception)
		{
			_sourceException = exception;
			DisposeSource();
			throw;
		}

		Count = _materializedItems.Count;
		DisposeSource();
		return false;
	}

	private void DisposeSource()
	{
		if (_isSourceDisposed)
		{
			return;
		}

		_isSourceDisposed = true;
		try
		{
			_enumerator.Dispose();
		}
		catch (Exception)
		{
			// The outcome is already decided, so an exception while disposing the source must not replace it.
		}
	}
}

internal sealed class MaterializingEnumerable : IEnumerable, ICountable, IMaterialization
{
	private readonly IEnumerator _enumerator;
	private readonly List<object?> _materializedItems = new();
	private bool _isSourceDisposed;
	private Exception? _sourceException;

	private MaterializingEnumerable(IEnumerable enumerable)
	{
		// ReSharper disable once GenericEnumeratorNotDisposed
		_enumerator = enumerable.GetEnumerator();
	}

	public int? Count { get; private set; }

	[return: NotNullIfNotNull(nameof(enumerable))]
	public static IEnumerable? Wrap(IEnumerable? enumerable)
	{
		if (enumerable is ICollection or MaterializingEnumerable or null)
		{
			return enumerable;
		}

		return new MaterializingEnumerable(enumerable);
	}

	#region IEnumerable Members

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	/// <inheritdoc cref="MaterializingEnumerable{T}.GetEnumerator()" />
	private IEnumerator GetEnumerator()
	{
		int index = 0;
		while (index < _materializedItems.Count || MoveNext())
		{
			if (index == _materializedItems.Count)
			{
				_materializedItems.Add(_enumerator.Current);
			}

			yield return _materializedItems[index++];
		}
	}

	#endregion

	/// <inheritdoc cref="IMaterialization.ReleaseSource()" />
	public Task ReleaseSource()
	{
		DisposeSource();
		return Task.CompletedTask;
	}

	/// <inheritdoc cref="MaterializingEnumerable{T}.MoveNext()" />
	private bool MoveNext()
	{
		if (_sourceException is not null)
		{
			ExceptionDispatchInfo.Capture(_sourceException).Throw();
		}

		if (_isSourceDisposed)
		{
			return false;
		}

		try
		{
			if (UserCode.Invoke(_enumerator.MoveNext))
			{
				return true;
			}
		}
		catch (Exception exception)
		{
			_sourceException = exception;
			DisposeSource();
			throw;
		}

		Count = _materializedItems.Count;
		DisposeSource();
		return false;
	}

	private void DisposeSource()
	{
		if (_isSourceDisposed)
		{
			return;
		}

		_isSourceDisposed = true;
		try
		{
			(_enumerator as IDisposable)?.Dispose();
		}
		catch (Exception)
		{
			// The outcome is already decided, so an exception while disposing the source must not replace it.
		}
	}
}
