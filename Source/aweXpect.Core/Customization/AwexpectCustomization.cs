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
	///     A value set in the current async flow takes precedence over the global value.
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
		=> Set(key, value);

	private CustomizationLifetime Set(string key, object? value)
	{
		if (_isGlobal)
		{
			return _global.Set(key, value);
		}

		object token = new();
		_store.Value = CustomizationStore.With(_store.Value, key, token, value);
		return new CustomizationLifetime(() => _store.Value = CustomizationStore.Without(_store.Value, key, token));
	}

	/// <summary>
	///     Enables capturing tracing information.
	/// </summary>
	public CustomizationLifetime EnableTracing(ITraceWriter traceWriter)
		=> Set(TraceWriterKey, traceWriter);

	internal ITraceWriter? TraceWriter
		=> ((IAwexpectCustomization)this).Get<ITraceWriter?>(TraceWriterKey, null);

	private sealed class CustomizationValue<TValue>(
		IAwexpectCustomization customization,
		string key,
		TValue defaultValue,
		Action<TValue>? validate = null)
		: ICustomizationValueSetter<TValue>
	{
		/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Get()" />
		public TValue Get() => customization.Get(key, defaultValue);

		/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Set(TValue)" />
		public CustomizationLifetime Set(TValue value)
		{
			validate?.Invoke(value);
			return customization.Set(key, value);
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

		public CustomizationLifetime Set(string key, object? value)
		{
			object token = new();
			lock (_lock)
			{
				Volatile.Write(ref _store, CustomizationStore.With(_store, key, token, value));
			}

			return new CustomizationLifetime(() =>
			{
				lock (_lock)
				{
					Volatile.Write(ref _store, CustomizationStore.Without(_store, key, token));
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

		public static CustomizationStore With(CustomizationStore? store, string key, object token, object? value)
		{
			Dictionary<string, Layer> values = store == null ? new() : new(store._values);
			values.TryGetValue(key, out Layer? below);
			values[key] = new Layer(token, value, below);
			return new CustomizationStore(values);
		}

		/// <summary>
		///     Removes the layer of the <paramref name="token" />, also when it is not the topmost one, so that the key
		///     disappears once all its lifetimes are disposed.
		/// </summary>
		public static CustomizationStore? Without(CustomizationStore? store, string key, object token)
		{
			if (store == null || !store._values.TryGetValue(key, out Layer? top) || !top.Contains(token))
			{
				return store;
			}

			Dictionary<string, Layer> values = new(store._values);
			Layer? newTop = top.Without(token);
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
	private sealed class Layer(object token, object? value, Layer? below)
	{
		private readonly Layer? _below = below;
		private readonly object _token = token;

		public object? Value { get; } = value;

		public bool Contains(object token)
		{
			for (Layer? layer = this; layer != null; layer = layer._below)
			{
				if (layer._token == token)
				{
					return true;
				}
			}

			return false;
		}

		/// <remarks>
		///     Requires the <paramref name="token" /> to be <see cref="Contains(object)">contained</see>.
		/// </remarks>
		public Layer? Without(object token)
			=> _token == token ? _below : new Layer(_token, Value, _below!.Without(token));
	}
}
