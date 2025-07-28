using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;

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
	public static Filtered.Assemblies AllLoadedAssemblies()
	{
		IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
		string[] prefixes = Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get();
		assemblies = assemblies.Where(assembly =>
			prefixes.All(prefix => !assembly.FullName?.StartsWith(prefix) == true));

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
	public static Filtered.Types Types(params Type[] types)
		=> new(types, $"in types {Formatter.Format(types)}");
}
