using System;
using System.Collections.Generic;
using System.Threading;

namespace aweXpect.Customization;

/// <summary>
///     Customize the global behaviour of aweXpect.
/// </summary>
public partial class AwexpectCustomization : IAwexpectCustomization
{
	private const string TraceWriterKey = "aweXpect.TraceWriter";
	private readonly GlobalLayer _global;
	private readonly bool _isGlobal;
	private readonly AsyncLocal<CustomizationStore?> _store;
	private AwexpectCustomization? _globalCustomization;

	/// <summary>
	///     Customize the global behaviour of aweXpect.
	/// </summary>
	public AwexpectCustomization()
	{
		_store = new AsyncLocal<CustomizationStore?>();
		_global = new GlobalLayer();
	}

	private AwexpectCustomization(AwexpectCustomization scoped)
	{
		_store = scoped._store;
		_global = scoped._global;
		_isGlobal = true;
	}

	/// <summary>
	///     Customize the defaults for all async flows, e.g. once in an assembly-level setup.
	/// </summary>
	/// <remarks>
	///     A value set in the current async flow takes precedence over the global value. While a value of a group is set
	///     in the current async flow, the other values of that group are taken from the global values at the time of the
	///     set.
	/// </remarks>
	public AwexpectCustomization Global
		=> _isGlobal ? this : _globalCustomization ??= new AwexpectCustomization(this);

	/// <inheritdoc cref="IAwexpectCustomization.Get{TValue}(string, TValue)" />
	TValue IAwexpectCustomization.Get<TValue>(string key, TValue defaultValue)
	{
		if (!_isGlobal)
		{
			CustomizationStore? store = _store.Value;
			if (store != null && store.TryGetValue(key, out object? value))
			{
				return value is TValue typedValue ? typedValue : defaultValue;
			}
		}

		CustomizationStore? globalStore = _global.Store;
		if (globalStore == null)
		{
			return defaultValue;
		}

		return globalStore.Get(key, defaultValue);
	}

	/// <inheritdoc cref="IAwexpectCustomization.Set{TValue}(string, TValue)" />
	CustomizationLifetime IAwexpectCustomization.Set<TValue>(string key, TValue value)
		=> Set(key, value, null);

	private CustomizationLifetime Update<TGroup>(string key, TGroup defaultValue, Func<TGroup, TGroup> update)
	{
		Func<object?, object?> reapply = below => update(below is TGroup group ? group : defaultValue);
		return _isGlobal
			? _global.Set(key, reapply, reapply)
			: Set(key, update(((IAwexpectCustomization)this).Get(key, defaultValue)), reapply);
	}

	private CustomizationLifetime Set(string key, object? value, Func<object?, object?>? reapply)
	{
		if (_isGlobal)
		{
			return _global.Set(key, _ => value, reapply);
		}

		object token = new();
		_store.Value = CustomizationStore.With(_store.Value, key, token, value, reapply);
		return new CustomizationLifetime(() =>
		{
			object? globalValue = null;
			_global.Store?.TryGetValue(key, out globalValue);
			_store.Value = CustomizationStore.Without(_store.Value, key, token, globalValue);
		});
	}

	/// <summary>
	///     Enables capturing tracing information.
	/// </summary>
	public CustomizationLifetime EnableTracing(ITraceWriter traceWriter)
		=> Set(TraceWriterKey, traceWriter, null);

	internal ITraceWriter? TraceWriter
		=> ((IAwexpectCustomization)this).Get<ITraceWriter?>(TraceWriterKey, null);

	private sealed class CustomizationValue<TGroup, TValue>(
		ICustomizationValueUpdater<TGroup> group,
		Func<TGroup, TValue> getter,
		Func<TGroup, TValue, TGroup> setter,
		Action<TValue>? validate = null)
		: ICustomizationValueSetter<TValue>
	{
		/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Get()" />
		public TValue Get() => getter(group.Get());

		/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Set(TValue)" />
		public CustomizationLifetime Set(TValue value)
		{
			validate?.Invoke(value);
			return group.Update(g => setter(g, value));
		}
	}

	/// <summary>
	///     Replaces the immutable store as a whole, so that reading a value never needs a lock.
	/// </summary>
	private sealed class GlobalLayer
	{
		private readonly object _lock = new();
		private CustomizationStore? _store;

		public CustomizationStore? Store => Volatile.Read(ref _store);

		/// <remarks>
		///     The value is computed from the current value under the lock, so that concurrent updates of the same
		///     group cannot lose one another.
		/// </remarks>
		public CustomizationLifetime Set(string key, Func<object?, object?> getValue,
			Func<object?, object?>? reapply)
		{
			object token = new();
			lock (_lock)
			{
				object? current = null;
				_store?.TryGetValue(key, out current);
				Volatile.Write(ref _store, CustomizationStore.With(_store, key, token, getValue(current), reapply));
			}

			return new CustomizationLifetime(() =>
			{
				lock (_lock)
				{
					Volatile.Write(ref _store, CustomizationStore.Without(_store, key, token, null));
				}
			});
		}
	}

	/// <summary>
	///     Immutable, because the <see cref="AsyncLocal{T}" /> shares the same instance with all child flows.
	/// </summary>
	private sealed class CustomizationStore
	{
		private readonly Dictionary<string, Layer> _values;

		private CustomizationStore(Dictionary<string, Layer> values)
		{
			_values = values;
		}

		public TValue Get<TValue>(string key, TValue defaultValue)
		{
			if (_values.TryGetValue(key, out Layer? layer) && layer.Value is TValue typedValue)
			{
				return typedValue;
			}

			return defaultValue;
		}

		public bool TryGetValue(string key, out object? value)
		{
			if (_values.TryGetValue(key, out Layer? layer))
			{
				value = layer.Value;
				return true;
			}

			value = null;
			return false;
		}

		public static CustomizationStore With(CustomizationStore? store, string key, object token, object? value,
			Func<object?, object?>? reapply)
		{
			Dictionary<string, Layer> values = store == null ? new() : new(store._values);
			values.TryGetValue(key, out Layer? below);
			values[key] = new Layer(token, value, reapply, below);
			return new CustomizationStore(values);
		}

		/// <summary>
		///     Removes the layer of the <paramref name="token" />, also when it is not the topmost one, so that the key
		///     disappears once all its lifetimes are disposed.
		/// </summary>
		/// <remarks>
		///     The layers above it are computed again on top of the layer below it, or on top of the
		///     <paramref name="fallbackValue" /> when there is none.
		/// </remarks>
		public static CustomizationStore? Without(CustomizationStore? store, string key, object token,
			object? fallbackValue)
		{
			if (store == null || !store._values.TryGetValue(key, out Layer? top))
			{
				return store;
			}

			Layer? newTop = Layer.Without(top, token, fallbackValue, out bool isFound);
			if (!isFound)
			{
				return store;
			}

			Dictionary<string, Layer> values = new(store._values);
			if (newTop == null)
			{
				values.Remove(key);
			}
			else
			{
				values[key] = newTop;
			}

			return new CustomizationStore(values);
		}
	}

	/// <summary>
	///     A value set for a key on top of the layer below it.
	/// </summary>
	/// <remarks>
	///     A value that changes only some properties of a group can be reapplied to compute it again, when a layer below
	///     it is removed.
	/// </remarks>
	private sealed class Layer(object token, object? value, Func<object?, object?>? reapply, Layer? below)
	{
		private readonly Layer? _below = below;
		private readonly Func<object?, object?>? _reapply = reapply;
		private readonly object _token = token;

		public object? Value { get; } = value;

		public static Layer? Without(Layer? layer, object token, object? fallbackValue, out bool isFound)
		{
			if (layer == null)
			{
				isFound = false;
				return null;
			}

			if (layer._token == token)
			{
				isFound = true;
				return layer._below;
			}

			Layer? newBelow = Without(layer._below, token, fallbackValue, out isFound);
			if (!isFound)
			{
				return layer;
			}

			object? valueBelow = newBelow == null ? fallbackValue : newBelow.Value;
			return new Layer(layer._token, layer._reapply == null ? layer.Value : layer._reapply(valueBelow),
				layer._reapply, newBelow);
		}
	}
}
