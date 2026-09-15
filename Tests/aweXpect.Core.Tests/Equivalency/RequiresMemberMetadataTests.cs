using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Core.Metadata;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

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
			.IsEqualTo(["Compare",])
			.Because("the reflection lookup must not silently degrade into an empty test set");
	}

	/// <remarks>
	///     The equality members of the options types and property accessors take an <see cref="EquivalencyOptions" />
	///     as well, but only carry it around, so they are not entry points.
	/// </remarks>
	private static IEnumerable<MethodInfo> EquivalencyEntryPoints()
		=> typeof(EquivalencyComparison).Assembly.GetTypes()
			.Where(type => type.IsPublic || type.IsNestedPublic)
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static |
			                                    BindingFlags.DeclaredOnly))
			.Where(method => !method.IsSpecialName &&
			                 !typeof(EquivalencyOptions).IsAssignableFrom(method.DeclaringType) &&
			                 method.GetParameters().Any(parameter => parameter.ParameterType == typeof(EquivalencyOptions)));

	private static bool IsMarked(MethodInfo method)
		=> method.GetParameters().Any(parameter => parameter.IsDefined(typeof(RequiresMemberMetadataAttribute), false)) ||
		   (method.IsGenericMethodDefinition &&
		    method.GetGenericArguments().Any(type => type.IsDefined(typeof(RequiresMemberMetadataAttribute), false)));
}
