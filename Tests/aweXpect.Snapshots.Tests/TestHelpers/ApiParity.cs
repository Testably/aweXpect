using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     Compares the rules for all snapshots with the rules of a single expectation.
/// </summary>
internal static class ApiParity
{
	/// <summary>
	///     Returns the public methods and implemented interfaces of the <paramref name="entry" /> type and of every
	///     selection type that its methods return, recursively, with the type that an action returns written as
	///     <c>TReturn</c>.
	/// </summary>
	public static string[] GetSurface(System.Type entry, System.Type returnType)
	{
		List<string> surface = [];
		HashSet<string> visited = [];
		Queue<System.Type> pending = new([entry,]);
		while (pending.Count > 0)
		{
			System.Type type = pending.Dequeue();
			string typeName = Name(type, returnType);
			if (!visited.Add(typeName))
			{
				continue;
			}

			surface.AddRange(type.GetInterfaces()
				.Select(interfaceType => $"{typeName} : {Name(interfaceType, returnType)}"));
			foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				         .Where(method => method.DeclaringType != typeof(object)))
			{
				surface.Add($"{typeName}.{method.Name}`{(method.IsGenericMethodDefinition ? method.GetGenericArguments().Length : 0)}" +
				            $"({string.Join(", ", method.GetParameters().Select(p => $"{Name(p.ParameterType, returnType)} {p.Name}"))})" +
				            $" : {Name(method.ReturnType, returnType)}");
				if (method.ReturnType != returnType && method.ReturnType.Namespace == typeof(SnapshotRule).Namespace)
				{
					pending.Enqueue(method.ReturnType);
				}
			}
		}

		return surface.OrderBy(signature => signature, StringComparer.Ordinal).ToArray();
	}

	private static string Name(System.Type type, System.Type returnType)
	{
		if (type == returnType)
		{
			return "TReturn";
		}

		if (type.IsGenericParameter)
		{
			return type.Name;
		}

		if (type.IsGenericType)
		{
			return $"{type.Namespace}.{type.Name}[{string.Join(",", type.GetGenericArguments().Select(argument => Name(argument, returnType)))}]";
		}

		return type.FullName ?? type.Name;
	}
}
