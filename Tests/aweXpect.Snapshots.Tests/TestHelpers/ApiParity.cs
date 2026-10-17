using System.Linq;
using System.Reflection;

namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     Compares the factories for all snapshots with the methods for a single expectation.
/// </summary>
internal static class ApiParity
{
	public static string[] GetFactories(System.Type type)
		=> GetSignatures(type.GetMethods(BindingFlags.Public | BindingFlags.Static));

	public static string[] GetMethods(System.Type type)
		=> GetSignatures(type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));

	private static string[] GetSignatures(MethodInfo[] methods)
		=> methods
			.Select(method => $"{method.Name}`{(method.IsGenericMethodDefinition ? method.GetGenericArguments().Length : 0)}" +
			                  $"({string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.FullName} {p.Name}"))})")
			.OrderBy(signature => signature, StringComparer.Ordinal)
			.ToArray();
}
