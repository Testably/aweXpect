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
}
