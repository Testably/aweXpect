using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Core.Metadata;
using aweXpect.Equivalency;

namespace aweXpect.Tests;

public sealed class RequiresMemberMetadataTests
{
	[Fact]
	public async Task EveryEquivalencyEntryPoint_ShouldCarryTheMarker()
	{
		List<string> unmarked = EquivalencyEntryPoints()
			.Where(method => !IsMarked(method))
			.Select(method => $"{method.DeclaringType!.Name}.{method.Name}")
			.Distinct().OrderBy(identifier => identifier, StringComparer.Ordinal)
			.ToList();

		await That(unmarked).IsEmpty()
			.Because(
				"the generator only registers the types passed to a marked parameter or type parameter, so an unmarked entry point silently falls back to reflection under trimming");
	}

	[Fact]
	public async Task ShouldFindTheEquivalencyEntryPoints()
	{
		await That(EquivalencyEntryPoints().Select(method => method.Name).Distinct())
			.IsEqualTo(["AreEquivalentTo", "Equivalent", "IsEquivalentTo", "IsNotEquivalentTo",]).InAnyOrder()
			.Because("the reflection lookup must not silently degrade into an empty test set");
	}

	/// <remarks>
	///     The Core assembly is checked by its own test project, because this one builds against the released Core
	///     outside Debug, where a marker added on the branch is not visible yet.
	/// </remarks>
	private static IEnumerable<MethodInfo> EquivalencyEntryPoints()
		=> typeof(EquivalencyExtensions).Assembly.GetTypes()
			.Where(type => type.IsPublic || type.IsNestedPublic)
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static |
			                                    BindingFlags.DeclaredOnly))
			.Where(method => !method.IsSpecialName &&
			                 !typeof(EquivalencyOptions).IsAssignableFrom(method.DeclaringType) &&
			                 method.GetParameters().Any(TakesEquivalencyOptions));

	private static bool TakesEquivalencyOptions(ParameterInfo parameter)
		=> parameter.ParameterType == typeof(EquivalencyOptions) ||
		   (parameter.ParameterType is { IsGenericType: true, } type &&
		    type.GetGenericTypeDefinition() == typeof(Func<,>) &&
		    type.GetGenericArguments()[1] == typeof(EquivalencyOptions));

	private static bool IsMarked(MethodInfo method)
		=> method.GetParameters().Any(parameter => parameter.IsDefined(typeof(RequiresMemberMetadataAttribute), false)) ||
		   (method.IsGenericMethodDefinition &&
		    method.GetGenericArguments().Any(type => type.IsDefined(typeof(RequiresMemberMetadataAttribute), false)));
}
