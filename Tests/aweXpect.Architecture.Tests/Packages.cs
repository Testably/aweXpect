using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Snapshots.DiffEngine;
using aweXpect.Snapshots.PublicApi;

namespace aweXpect.Architecture.Tests;

/// <summary>
///     The assemblies of all packages and the selections the rules share.
/// </summary>
internal static class Packages
{
	public static Assembly AweXpect { get; } = typeof(ThatBool).Assembly;

	public static Assembly[] Extensions { get; } =
	[
		typeof(JsonExtensions).Assembly,
		typeof(ThatHttpRequestMessage).Assembly,
		typeof(ThatXElement).Assembly,
		typeof(In).Assembly,
		typeof(ThatSnapshot).Assembly,
		typeof(DiffEngineReporter).Assembly,
		typeof(PublicApiDeclarations).Assembly,
	];

	public static Assembly[] All { get; } = [AweXpect, typeof(Expect).Assembly, .. Extensions,];

	public static IEnumerable<string> ExtensionNames()
		=> Extensions.Select(assembly => assembly.GetName().Name!);

	public static Assembly Extension(string name)
		=> Extensions.Single(assembly => assembly.GetName().Name == name);

	public static bool IsResult(Type type)
		=> typeof(Expectation).IsAssignableFrom(type);

	public static Filtered.Types PublicTypes(params Assembly[] assemblies)
		=> In.Assemblies(assemblies).Types().Which(type => type.IsVisible);
}
