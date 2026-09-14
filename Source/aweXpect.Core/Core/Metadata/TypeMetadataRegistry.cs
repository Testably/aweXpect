using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

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
		=> Instance.AddField(typeof(T), name, typeof(TMember), Wrap(getValue));

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
		=> Instance.AddProperty(typeof(T), name, typeof(TMember), Wrap(getValue));

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

	private static Func<object, object?> Wrap<T, TMember>(Func<T, TMember> getValue)
		=> subject => getValue((T)subject);

	internal sealed class Registration
	{
		private readonly ConcurrentDictionary<Type, TypeMetadata> _metadata = new();
		private int _order;

		public void AddField(Type type, string name, Type memberType, Func<object, object?> getValue)
			=> GetOrAdd(type).Fields[name] = new RegisteredMember(name, memberType, getValue, NextOrder());

		public void AddProperty(Type type, string name, Type memberType, Func<object, object?> getValue)
			=> GetOrAdd(type).Properties[name] = new RegisteredMember(name, memberType, getValue, NextOrder());

		public void AddEvent(Type type, string name, Func<Action<object?[]>, Delegate> createHandler,
			Action<object, Delegate> addHandler, Action<object, Delegate> removeHandler)
			=> GetOrAdd(type).Events[name] = new RegisteredEvent(name, createHandler, addHandler, removeHandler);

		/// <summary>
		///     Whether any member or event was registered for the <paramref name="type" />.
		/// </summary>
		public bool TryGet(Type type, [NotNullWhen(true)] out TypeMetadata? metadata)
			=> _metadata.TryGetValue(type, out metadata);

		private TypeMetadata GetOrAdd(Type type) => _metadata.GetOrAdd(type, _ => new TypeMetadata());

		/// <remarks>
		///     Registrations keep the order the generator emitted them in, so that a failure message lists the members of
		///     a registered type in the same order as the reflected one.
		/// </remarks>
		private int NextOrder() => Interlocked.Increment(ref _order);
	}

	internal sealed class TypeMetadata
	{
		public ConcurrentDictionary<string, RegisteredMember> Fields { get; } = new(StringComparer.Ordinal);
		public ConcurrentDictionary<string, RegisteredMember> Properties { get; } = new(StringComparer.Ordinal);
		public ConcurrentDictionary<string, RegisteredEvent> Events { get; } = new(StringComparer.Ordinal);
	}

	internal sealed class RegisteredMember(string name, Type memberType, Func<object, object?> getValue, int order)
	{
		public string Name { get; } = name;
		public Type MemberType { get; } = memberType;
		public Func<object, object?> GetValue { get; } = getValue;
		public int Order { get; } = order;
	}

	internal sealed class RegisteredEvent(
		string name,
		Func<Action<object?[]>, Delegate> createHandler,
		Action<object, Delegate> addHandler,
		Action<object, Delegate> removeHandler)
	{
		public string Name { get; } = name;
		public Func<Action<object?[]>, Delegate> CreateHandler { get; } = createHandler;
		public Action<object, Delegate> AddHandler { get; } = addHandler;
		public Action<object, Delegate> RemoveHandler { get; } = removeHandler;
	}
}
