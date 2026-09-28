using System;
using System.Collections.Generic;
using System.Threading;

namespace aweXpect.Customization;

/// <summary>
///     Customize the global behaviour of aweXpect.
/// </summary>
public partial class AwexpectCustomization : IAwexpectCustomization
{
	private const string GlobalTraceWriterKey = "aweXpect.TraceWriter";
	private readonly GlobalLayer _global;
	private readonly bool _isGlobal;
	private readonly AsyncLocal<CustomizationStore?> _store;
	private readonly AsyncLocal<ITraceWriter?> _traceWriter;
	private AwexpectCustomization? _globalCustomization;

	/// <summary>
	///     Customize the global behaviour of aweXpect.
	/// </summary>
	public AwexpectCustomization()
	{
		_store = new AsyncLocal<CustomizationStore?>();
		_global = new GlobalLayer();
		_traceWriter = new AsyncLocal<ITraceWriter?>();
	}

	private AwexpectCustomization(AwexpectCustomization scoped)
	{
		_store = scoped._store;
		_global = scoped._global;
		_traceWriter = scoped._traceWriter;
		_isGlobal = true;
	}

	/// <summary>
	///     Customize the defaults for all async flows, e.g. once in an assembly-level setup.
	/// </summary>
	/// <remarks>
	///     A value set in the current async flow takes precedence over the global value.<br />
	///     Set global values once, before the tests run: changing a group concurrently from several threads can lose
	///     one of the changes.
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
	{
		if (_isGlobal)
		{
			return _global.Set(key, value);
		}

		object? previousValue = null;
		bool hasPreviousValue = _store.Value?.TryGetValue(key, out previousValue) == true;
		_store.Value = CustomizationStore.With(_store.Value, key, value);
		return new CustomizationLifetime(() => _store.Value = hasPreviousValue
			? CustomizationStore.With(_store.Value, key, previousValue)
			: CustomizationStore.Without(_store.Value, key));
	}

	/// <summary>
	///     Enables capturing tracing information.
	/// </summary>
	public CustomizationLifetime EnableTracing(ITraceWriter traceWriter)
	{
		if (_isGlobal)
		{
			return _global.Set(GlobalTraceWriterKey, traceWriter);
		}

		ITraceWriter? previousTraceWriter = _traceWriter.Value;
		_traceWriter.Value = traceWriter;
		return new CustomizationLifetime(() => _traceWriter.Value = previousTraceWriter);
	}

	internal ITraceWriter? TraceWriter
		=> _traceWriter.Value ?? _global.Store?.Get<ITraceWriter?>(GlobalTraceWriterKey, null);

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
			TValue previousValue = Get();
			CustomizationLifetime groupLifetime = group.Update(g => setter(g, value));
			TGroup updatedGroup = group.Get();
			return new CustomizationLifetime(() =>
			{
				// Undoing the whole update also removes the group from the async flow again, so that global values
				// apply. When the group was changed in the meantime, only this value is restored to keep the changes.
				if (ReferenceEquals(group.Get(), updatedGroup))
				{
					groupLifetime.Dispose();
				}
				else
				{
					group.Update(g => setter(g, previousValue));
				}
			});
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
			lock (_lock)
			{
				object? previousValue = null;
				bool hasPreviousValue = _store?.TryGetValue(key, out previousValue) == true;
				Volatile.Write(ref _store, CustomizationStore.With(_store, key, value));
				return new CustomizationLifetime(() =>
				{
					lock (_lock)
					{
						Volatile.Write(ref _store, hasPreviousValue
							? CustomizationStore.With(_store, key, previousValue)
							: CustomizationStore.Without(_store, key));
					}
				});
			}
		}
	}

	/// <summary>
	///     Immutable, because the <see cref="AsyncLocal{T}" /> shares the same instance with all child flows.
	/// </summary>
	private sealed class CustomizationStore
	{
		private readonly Dictionary<string, object?> _values;

		private CustomizationStore(Dictionary<string, object?> values)
		{
			_values = values;
		}

		public TValue Get<TValue>(string key, TValue defaultValue)
		{
			if (_values.TryGetValue(key, out object? v) && v is TValue typedValue)
			{
				return typedValue;
			}

			return defaultValue;
		}

		public bool TryGetValue(string key, out object? value)
			=> _values.TryGetValue(key, out value);

		public static CustomizationStore With(CustomizationStore? store, string key, object? value)
		{
			Dictionary<string, object?> values = store == null ? new() : new(store._values);
			values[key] = value;
			return new CustomizationStore(values);
		}

		public static CustomizationStore? Without(CustomizationStore? store, string key)
		{
			if (store == null || !store._values.ContainsKey(key))
			{
				return store;
			}

			Dictionary<string, object?> values = new(store._values);
			values.Remove(key);
			return new CustomizationStore(values);
		}
	}
}
