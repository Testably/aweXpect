using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

/// <summary>
///     Static entry point for assemblies.
/// </summary>
public static class In
{
	/// <summary>
	///     Defines expectations on all loaded assemblies from the current <see cref="System.AppDomain.CurrentDomain" />.
	/// </summary>
	/// <remarks>
	///     Assemblies whose name matches one of the <c>ExcludedAssemblyPrefixes</c> at a name-segment boundary
	///     are skipped — the same matching the dependency assertions use, so one customization value has one
	///     meaning everywhere.
	/// </remarks>
	public static Filtered.Assemblies AllLoadedAssemblies()
	{
		IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
		string[] prefixes = Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get();
		assemblies = assemblies.Where(assembly => !assembly.GetName().Name.IsExcludedAssemblyName(prefixes));

		return new Filtered.Assemblies(assemblies, "in all loaded assemblies");
	}

	/// <summary>
	///     Defines expectations on the given <paramref name="assemblies" />.
	/// </summary>
	public static Filtered.Assemblies Assemblies(params Assembly?[] assemblies)
		=> new(assemblies, $"in the assemblies {Formatter.Format(assemblies)}");

	/// <summary>
	///     Defines expectations on the given <paramref name="assemblies" />.
	/// </summary>
	public static Filtered.Assemblies Assemblies(IEnumerable<Assembly> assemblies)
		=> new(assemblies, $"in the assemblies {Formatter.Format(assemblies)}");

	/// <summary>
	///     Defines expectations on the assembly that contains the <typeparamref name="TType" />.
	/// </summary>
	public static Filtered.Assemblies AssemblyContaining<TType>()
		=> new(typeof(TType).Assembly, $"in assembly containing type {Formatter.Format(typeof(TType))}");

	/// <summary>
	///     Defines expectations on the assembly that contains the <paramref name="type" />.
	/// </summary>
	public static Filtered.Assemblies AssemblyContaining(Type type)
		=> new(type.Assembly, $"in assembly containing type {Formatter.Format(type)}");

	/// <summary>
	///     Defines expectations on the <see cref="Assembly.GetEntryAssembly()" />.
	/// </summary>
	public static Filtered.Assemblies EntryAssembly()
		=> new(Assembly.GetEntryAssembly(), "in entry assembly");

	/// <summary>
	///     Defines expectations on the <see cref="Assembly.GetExecutingAssembly()" />.
	/// </summary>
	public static Filtered.Assemblies ExecutingAssembly()
		=> new(Assembly.GetExecutingAssembly(), "in executing assembly");

	/// <summary>
	///     Defines expectations on all types within the <paramref name="namespace" /> (including sub-namespaces)
	///     from all loaded assemblies of the current <see cref="System.AppDomain.CurrentDomain" />.
	/// </summary>
	/// <remarks>
	///     This matches the <paramref name="namespace" /> and its sub-namespaces, but not namespaces that merely start
	///     with the same text (e.g. <c>Foo.Bar</c> does not include <c>Foo.BarBaz</c>). The comparison is exact and
	///     case-sensitive.
	/// </remarks>
	public static Filtered.Types Namespace(string @namespace)
		=> AllLoadedAssemblies().Types().WithinNamespace(@namespace);

	/// <summary>
	///     Defines expectations on the type <typeparamref name="TType" />.
	/// </summary>
	public static Filtered.Types Type<TType>()
		=> new([typeof(TType),], $"in type {Formatter.Format(typeof(TType))}");

	/// <summary>
	///     Defines expectations on the type <paramref name="type" />
	/// </summary>
	public static Filtered.Types Type(Type type)
		=> new([type,], $"in type {Formatter.Format(type)}");

	/// <summary>
	///     Defines expectations on the types <typeparamref name="TType1" /> and <typeparamref name="TType2" />.
	/// </summary>
	public static Filtered.Types Types<TType1, TType2>()
		=> new([typeof(TType1), typeof(TType2),],
			$"in types {Formatter.Format(typeof(TType1))} and {Formatter.Format(typeof(TType2))}");

	/// <summary>
	///     Defines expectations on the types <typeparamref name="TType1" />, <typeparamref name="TType2" /> and
	///     <typeparamref name="TType3" />.
	/// </summary>
	public static Filtered.Types Types<TType1, TType2, TType3>()
		=> new([typeof(TType1), typeof(TType2), typeof(TType3),],
			$"in types {Formatter.Format(typeof(TType1))}, {Formatter.Format(typeof(TType2))} and {Formatter.Format(typeof(TType3))}");

	/// <summary>
	///     Defines expectations on the types <paramref name="types" />
	/// </summary>
	public static Filtered.Types Types(params IEnumerable<Type> types)
		=> new(types, $"in types {Formatter.Format(types)}");

	/// <summary>
	///     Defines expectations on the given <paramref name="constructors" />.
	/// </summary>
	public static Filtered.Constructors Constructors(params IEnumerable<ConstructorInfo> constructors)
		=> new(constructors, $"in the constructors {Formatter.Format(constructors)}");

	/// <summary>
	///     Defines expectations on the given <paramref name="events" />.
	/// </summary>
	public static Filtered.Events Events(params IEnumerable<EventInfo> events)
		=> new(events, $"in the events {Formatter.Format(events)}");

	/// <summary>
	///     Defines expectations on the given <paramref name="fields" />.
	/// </summary>
	public static Filtered.Fields Fields(params IEnumerable<FieldInfo> fields)
		=> new(fields, $"in the fields {Formatter.Format(fields)}");

	/// <summary>
	///     Defines expectations on the given <paramref name="methods" />.
	/// </summary>
	public static Filtered.Methods Methods(params IEnumerable<MethodInfo> methods)
		=> new(methods, $"in the methods {Formatter.Format(methods)}");

	/// <summary>
	///     Defines expectations on the given <paramref name="properties" />.
	/// </summary>
	public static Filtered.Properties Properties(params IEnumerable<PropertyInfo> properties)
		=> new(properties, $"in the properties {Formatter.Format(properties)}");
}
