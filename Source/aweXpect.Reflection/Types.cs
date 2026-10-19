using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

/// <summary>
///     Static entry point for selecting types by criteria.
/// </summary>
/// <remarks>
///     The <c>In*</c> methods mirror the assembly sources on <see cref="In" /> (each is the corresponding
///     <c>In.*(…).Types()</c>) and are mirrored again as source clarifiers on
///     <see cref="Filtered.Types.InNamespaceResult" />. When adding a new assembly source, keep all three
///     surfaces in sync.
/// </remarks>
public static class Types
{
	/// <summary>
	///     Defines expectations on all types in all loaded assemblies from the current
	///     <see cref="System.AppDomain.CurrentDomain" />.
	/// </summary>
	/// <remarks>
	///     Assemblies whose name matches one of the <c>ExcludedAssemblyPrefixes</c> at a name-segment boundary
	///     are skipped — the same matching the dependency assertions use, so one customization value has one
	///     meaning everywhere.
	/// </remarks>
	public static Filtered.Types InAllLoadedAssemblies()
		=> In.AllLoadedAssemblies().Types();

	/// <summary>
	///     Defines expectations on all types in the given <paramref name="assemblies" />.
	/// </summary>
	public static Filtered.Types InAssemblies(params IEnumerable<Assembly?> assemblies)
		=> In.Assemblies(assemblies).Types();

	/// <summary>
	///     Defines expectations on all types in the assembly that contains the <typeparamref name="TType" />.
	/// </summary>
	public static Filtered.Types InAssemblyContaining<TType>()
		=> In.AssemblyContaining<TType>().Types();

	/// <summary>
	///     Defines expectations on all types in the assembly that contains the <paramref name="type" />.
	/// </summary>
	public static Filtered.Types InAssemblyContaining(Type type)
		=> In.AssemblyContaining(type).Types();

	/// <summary>
	///     Defines expectations on all types within the <paramref name="namespace" /> (including sub-namespaces).
	/// </summary>
	/// <remarks>
	///     By default, the types from all loaded assemblies of the current
	///     <see cref="System.AppDomain.CurrentDomain" /> are considered; use one of the <c>In*</c> methods on the
	///     returned collection to clarify a different assembly source.
	///     <para />
	///     This matches the <paramref name="namespace" /> and its sub-namespaces, but not namespaces that merely start
	///     with the same text (e.g. <c>Foo.Bar</c> does not include <c>Foo.BarBaz</c>). The comparison is exact and
	///     case-sensitive.
	/// </remarks>
	public static Filtered.Types.InNamespaceResult InNamespace(string @namespace)
		=> new(@namespace);
}
