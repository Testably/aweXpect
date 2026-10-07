using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using aweXpect.Equivalency;
using aweXpect.Recording;

namespace aweXpect.Core.Metadata;

/// <summary>
///     Registry for the members and events that expectations access on a subject.
/// </summary>
/// <remarks>
///     Reflecting over a type cannot work when the application is published with trimming or Native AOT enabled,
///     because members that are only referenced reflectively are removed. The registrations provide them statically
///     instead, and are normally emitted by the source generator from the call sites that need them.
///     <para />
///     Only public instance members are registered, because a generated accessor cannot reach the non-public members
///     of a type from another assembly on every target. A comparison that requests non-public members reflects over
///     the whole type, which works unchanged in a normal build and remains best effort when publishing with trimming
///     or Native AOT enabled, where members removed by the trimmer are silently left out of the comparison.
/// </remarks>
public static class TypeMetadataRegistry
{
	/// <remarks>
	///     An instance instead of static fields, so that it can be verified without affecting the registrations used by
	///     the running test.
	/// </remarks>
	internal static Registration Instance { get; } = new();

	/// <summary>
	///     Registers the public field <paramref name="name" /> of <typeparamref name="T" />.
	/// </summary>
	public static void RegisterField<T, TMember>(string name, Func<T, TMember> getValue)
		=> Instance.AddField(typeof(T), name, typeof(TMember), Wrap(getValue), ValueComparer.For(getValue));

	/// <summary>
	///     Registers the public field <paramref name="name" /> of the type of <paramref name="probe" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="probe" /> is never invoked. It only provides the type argument for types whose name
	///     cannot be written in source, such as anonymous types.
	/// </remarks>
	public static void RegisterField<T, TMember>(T probe, string name, Func<T, TMember> getValue)
		=> RegisterField(name, getValue);

	/// <summary>
	///     Registers the public property <paramref name="name" /> of <typeparamref name="T" />.
	/// </summary>
	public static void RegisterProperty<T, TMember>(string name, Func<T, TMember> getValue)
		=> Instance.AddProperty(typeof(T), name, typeof(TMember), Wrap(getValue), ValueComparer.For(getValue));

	/// <summary>
	///     Registers the public property <paramref name="name" /> of the type of <paramref name="probe" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="probe" /> is never invoked. It only provides the type argument for types whose name
	///     cannot be written in source, such as anonymous types.
	/// </remarks>
	public static void RegisterProperty<T, TMember>(T probe, string name, Func<T, TMember> getValue)
		=> RegisterProperty(name, getValue);

	/// <summary>
	///     Registers the property <paramref name="name" /> that <typeparamref name="T" /> implements explicitly for an
	///     interface.
	/// </summary>
	/// <remarks>
	///     The <paramref name="name" /> is the name of the implementation, which is qualified by its interface, such as
	///     <c>Namespace.IHasValue.Value</c>. The comparison only falls back to it for an expected member of the short
	///     name that <typeparamref name="T" /> does not have.
	/// </remarks>
	public static void RegisterExplicitProperty<T, TMember>(string name, Func<T, TMember> getValue)
		=> Instance.AddExplicitProperty(typeof(T), name, typeof(TMember), Wrap(getValue));

	/// <summary>
	///     Registers the event <paramref name="name" /> of <typeparamref name="T" />.
	/// </summary>
	/// <remarks>
	///     <paramref name="createHandler" /> receives the callback that records an occurrence and returns a delegate of
	///     the event handler type. Creating the handler in generated code avoids binding it reflectively, which cannot
	///     work for a handler with value-type parameters.
	/// </remarks>
	public static void RegisterEvent<T>(string name,
		Func<Action<object?[]>, Delegate> createHandler,
		Action<T, Delegate> addHandler,
		Action<T, Delegate> removeHandler)
		=> Instance.AddEvent(typeof(T), name, createHandler,
			(subject, handler) => addHandler((T)subject, handler),
			(subject, handler) => removeHandler((T)subject, handler));

	/// <summary>
	///     Registers the dictionaries with keys of type <typeparamref name="TKey" /> and values of type
	///     <typeparamref name="TValue" />, so that the equivalency comparison reads their key comparer.
	/// </summary>
	/// <remarks>
	///     The comparison only knows the type arguments of a dictionary at runtime, and reading its key comparer for them
	///     otherwise needs dynamic code.
	/// </remarks>
	public static void RegisterDictionary<TKey, TValue>()
	{
		DictionaryKeyComparer keyComparer = new DictionaryKeyComparer<TKey, TValue>();
		Instance.AddKeyComparer(typeof(IDictionary<TKey, TValue>), keyComparer);
		Instance.AddKeyComparer(typeof(IReadOnlyDictionary<TKey, TValue>), keyComparer);
	}

	/// <summary>
	///     Registers the sets with items of type <typeparamref name="T" />, so that the equivalency comparison reads
	///     their comparer.
	/// </summary>
	/// <remarks>
	///     The comparison only knows the type argument of a set at runtime, and reading its comparer for it otherwise
	///     needs dynamic code.
	/// </remarks>
	public static void RegisterSet<T>()
	{
		SetItemComparer itemComparer = new SetItemComparer<T>();
		Instance.AddItemComparer(typeof(ISet<T>), itemComparer);
#if NET8_0_OR_GREATER
		Instance.AddItemComparer(typeof(IReadOnlySet<T>), itemComparer);
#endif
	}

	/// <summary>
	///     Registers <typeparamref name="T" /> as a collection, the registered fields and properties of which are the
	///     members it declares itself, so that the equivalency comparison compares them in addition to its items.
	/// </summary>
	/// <remarks>
	///     The members that the collection types and interfaces of the framework declare, such as <c>Count</c>, are
	///     not registered, because they describe the collection, which the comparison of the items covers. The
	///     registration therefore only serves the comparison with another collection: against a value that is no
	///     collection, all members are compared, which are found by reflection.
	/// </remarks>
	public static void RegisterCollection<T>()
		=> Instance.AddCollection(typeof(T));

	/// <summary>
	///     Runs <paramref name="register" /> and publishes the registrations it makes on the calling thread together, once
	///     it returns.
	/// </summary>
	/// <remarks>
	///     A lookup on another thread sees a type either as it was before or with every member and event registered for
	///     it, never with only some of them. When <paramref name="register" /> throws, none of its registrations are
	///     published. A nested call joins the outer one.
	/// </remarks>
	public static void RegisterBatch(Action register)
		=> Instance.Batch(register);

	private static Func<object, object?> Wrap<T, TMember>(Func<T, TMember> getValue)
		=> subject => getValue((T)subject);

	internal sealed class Registration
	{
		private readonly ConcurrentDictionary<Type, TypeMetadata> _metadata = new();
		private readonly ThreadLocal<Dictionary<Type, TypeMetadata>?> _pending = new();
		private int _order;
		private int _version;

		/// <summary>
		///     Changes whenever a registration is published, so that what was derived from the registrations can tell
		///     that it is outdated.
		/// </summary>
		public int Version => Volatile.Read(ref _version);

		public void AddField(Type type, string name, Type memberType, Func<object, object?> getValue,
			ValueComparer? valueComparer = null)
			=> Add(type, metadata
				=> metadata.Fields[name] =
					new RegisteredMember(name, memberType, getValue, NextOrder(), valueComparer));

		public void AddProperty(Type type, string name, Type memberType, Func<object, object?> getValue,
			ValueComparer? valueComparer = null)
			=> Add(type, metadata
				=> metadata.Properties[name] =
					new RegisteredMember(name, memberType, getValue, NextOrder(), valueComparer));

		public void AddExplicitProperty(Type type, string name, Type memberType, Func<object, object?> getValue)
			=> Add(type, metadata
				=> metadata.ExplicitProperties[name] = new RegisteredMember(name, memberType, getValue, NextOrder()));

		public void AddEvent(Type type, string name, Func<Action<object?[]>, Delegate> createHandler,
			Action<object, Delegate> addHandler, Action<object, Delegate> removeHandler)
			=> Add(type, metadata => metadata.Events[name] =
				new RegisteredEvent(name, createHandler, addHandler, removeHandler, NextOrder()));

		public void AddKeyComparer(Type dictionaryInterface, DictionaryKeyComparer keyComparer)
			=> Add(dictionaryInterface, metadata => metadata.KeyComparer = keyComparer);

		public void AddItemComparer(Type setInterface, SetItemComparer itemComparer)
			=> Add(setInterface, metadata => metadata.ItemComparer = itemComparer);

		public void AddCollection(Type type)
			=> Add(type, metadata => metadata.IsCollection = true);

		/// <summary>
		///     Whether any member or event was registered for the <paramref name="type" />.
		/// </summary>
		public bool TryGet(Type type, [NotNullWhen(true)] out TypeMetadata? metadata)
			=> _metadata.TryGetValue(type, out metadata);

		/// <remarks>
		///     The registrations are collected per thread, because a module initializer that registers in a batch must
		///     not capture the registrations another thread makes meanwhile.
		/// </remarks>
		public void Batch(Action register)
		{
			if (_pending.Value is not null)
			{
				register();
				return;
			}

			Dictionary<Type, TypeMetadata> pending;
			_pending.Value = new Dictionary<Type, TypeMetadata>();
			try
			{
				register();
				pending = _pending.Value!;
			}
			finally
			{
				_pending.Value = null;
			}

			foreach (KeyValuePair<Type, TypeMetadata> entry in pending)
			{
				Publish(entry.Key, entry.Value);
			}
		}

		private void Add(Type type, Action<TypeMetadata> add)
		{
			if (_pending.Value is { } pending)
			{
				if (!pending.TryGetValue(type, out TypeMetadata? metadata))
				{
					metadata = new TypeMetadata();
					pending.Add(type, metadata);
				}

				add(metadata);
				return;
			}

			TypeMetadata single = new();
			add(single);
			Publish(type, single);
		}

		/// <remarks>
		///     A published entry is never changed, but replaced by a merged copy, so that a lookup that already holds it
		///     keeps a consistent view.
		/// </remarks>
		private void Publish(Type type, TypeMetadata metadata)
		{
			_metadata.AddOrUpdate(type, metadata, (_, published) => published.MergedWith(metadata));
			Interlocked.Increment(ref _version);
		}

		/// <remarks>
		///     Registrations keep the order the generator emitted them in, so that a failure message lists the members of
		///     a registered type in the same order as the reflected one, and a recording attaches to the events in the
		///     order reflection would return them.
		/// </remarks>
		private int NextOrder() => Interlocked.Increment(ref _order);
	}

	internal sealed class TypeMetadata
	{
		private RegisteredEvent[]? _orderedEvents;
		private MemberSnapshot? _orderedMembers;

		public ConcurrentDictionary<string, RegisteredMember> Fields { get; } = new(StringComparer.Ordinal);
		public ConcurrentDictionary<string, RegisteredMember> Properties { get; } = new(StringComparer.Ordinal);

		public ConcurrentDictionary<string, RegisteredMember> ExplicitProperties { get; } =
			new(StringComparer.Ordinal);

		public ConcurrentDictionary<string, RegisteredEvent> Events { get; } = new(StringComparer.Ordinal);

		/// <summary>
		///     The reader of the key comparer, registered for a generic dictionary interface.
		/// </summary>
		public DictionaryKeyComparer? KeyComparer { get; set; }

		/// <summary>
		///     The reader of the item comparer, registered for a generic set interface.
		/// </summary>
		public SetItemComparer? ItemComparer { get; set; }

		/// <summary>
		///     Whether the type is registered as a collection, the fields and properties of which are only the
		///     members it declares itself.
		/// </summary>
		public bool IsCollection { get; set; }

		/// <summary>
		///     The registered events in the order of their registration.
		/// </summary>
		/// <remarks>
		///     Computed once, because a published entry is never changed, and every recording reads them.
		/// </remarks>
		public RegisteredEvent[] OrderedEvents
			=> _orderedEvents ??= Events.Values.OrderBy(@event => @event.Order).ToArray();

		/// <summary>
		///     The registered fields and properties in the order of their registration.
		/// </summary>
		/// <remarks>
		///     Computed once, because a published entry is never changed, and every compared object reads them.
		/// </remarks>
		public MemberSnapshot OrderedMembers => _orderedMembers ??= new MemberSnapshot(
			Order(Fields), Order(Properties),
			!(Fields.IsEmpty && Properties.IsEmpty && ExplicitProperties.IsEmpty));

		/// <summary>
		///     A copy of this metadata in which the <paramref name="registered" /> members and events replace those of the
		///     same name.
		/// </summary>
		public TypeMetadata MergedWith(TypeMetadata registered)
		{
			TypeMetadata merged = new()
			{
				KeyComparer = registered.KeyComparer ?? KeyComparer,
				ItemComparer = registered.ItemComparer ?? ItemComparer,
				IsCollection = registered.IsCollection || IsCollection,
			};
			Merge(merged.Fields, Fields, registered.Fields);
			Merge(merged.Properties, Properties, registered.Properties);
			Merge(merged.ExplicitProperties, ExplicitProperties, registered.ExplicitProperties);
			Merge(merged.Events, Events, registered.Events);
			return merged;
		}

		private static void Merge<T>(ConcurrentDictionary<string, T> merged,
			ConcurrentDictionary<string, T> published, ConcurrentDictionary<string, T> registered)
		{
			foreach (KeyValuePair<string, T> entry in published.Concat(registered))
			{
				merged[entry.Key] = entry.Value;
			}
		}

		private static EquivalencyMember[] Order(ConcurrentDictionary<string, RegisteredMember> members)
			=> members.Values
				.OrderBy(member => member.Order)
				.Select(member => new EquivalencyMember(member.Name, member.MemberType, member.GetValue,
					member.ValueComparer))
				.ToArray();
	}

	/// <summary>
	///     The registered fields and properties of a type, and whether it has any member that is compared.
	/// </summary>
	internal sealed class MemberSnapshot(EquivalencyMember[] fields, EquivalencyMember[] properties, bool hasMembers)
	{
		public EquivalencyMember[] Fields { get; } = fields;
		public EquivalencyMember[] Properties { get; } = properties;
		public bool HasMembers { get; } = hasMembers;
	}

	internal sealed class RegisteredMember(
		string name,
		Type memberType,
		Func<object, object?> getValue,
		int order,
		ValueComparer? valueComparer = null)
	{
		public string Name { get; } = name;
		public Type MemberType { get; } = memberType;
		public Func<object, object?> GetValue { get; } = getValue;
		public int Order { get; } = order;
		public ValueComparer? ValueComparer { get; } = valueComparer;
	}

	/// <summary>
	///     Compares the values of a member of a primitive or enum type on two objects of the registered type without
	///     boxing them.
	/// </summary>
	internal abstract class ValueComparer
	{
		public abstract Type MemberType { get; }

		/// <summary>
		///     Reads the member of the <paramref name="actual" /> and then of the <paramref name="expected" /> object
		///     and returns whether the values are equal, and only when they are not, the values themselves.
		/// </summary>
		/// <remarks>
		///     The values are equal exactly when their <see cref="object.Equals(object)" /> says so, which for a
		///     primitive or enum type never throws.
		/// </remarks>
		public abstract (bool IsEqual, object? Actual, object? Expected) Compare(object actual, object expected);

		public static ValueComparer? For<T, TMember>(Func<T, TMember> getValue)
			=> typeof(TMember).IsPrimitive || typeof(TMember).IsEnum
				? new Typed<T, TMember>(getValue)
				: null;

		private sealed class Typed<T, TMember>(Func<T, TMember> getValue) : ValueComparer
		{
			public override Type MemberType => typeof(TMember);

			public override (bool IsEqual, object? Actual, object? Expected) Compare(object actual, object expected)
			{
				TMember actualValue = getValue((T)actual);
				TMember expectedValue = getValue((T)expected);
				return EqualityComparer<TMember>.Default.Equals(actualValue, expectedValue)
					? (true, null, null)
					: (false, actualValue, expectedValue);
			}
		}
	}

	internal sealed class RegisteredEvent(
		string name,
		Func<Action<object?[]>, Delegate> createHandler,
		Action<object, Delegate> addHandler,
		Action<object, Delegate> removeHandler,
		int order) : IRecordableEvent
	{
		public Func<Action<object?[]>, Delegate> CreateHandler { get; } = createHandler;
		public Action<object, Delegate> AddHandler { get; } = addHandler;
		public Action<object, Delegate> RemoveHandler { get; } = removeHandler;
		public int Order { get; } = order;
		public string Name { get; } = name;
	}
}
