#if NET10_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using aweXpect.Customization;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

/// <summary>
///     The C# extension block syntax emits hidden <c>&lt;G&gt;$…</c> grouping types (and skeleton members) that are not
///     marked with <c>[CompilerGenerated]</c>. They must be excluded from discovery so they do not pollute the results.
/// </summary>
public sealed class ExtensionGroupingTypeFilteringTests
{
	private const string ExtensionMarkerAttributeName = "ExtensionMarkerAttribute";

	[Fact]
	public async Task Types_ShouldExcludeExtensionGroupingTypesByDefault()
	{
		IReadOnlyList<Type> types = await GetTypes();

		await That(types).None().Satisfy(t => t.DeclaringType == typeof(StaticClassWithNewExtensionMethods) &&
		                                      t.Name.StartsWith('<'));
		await That(types).Contains(t => t == typeof(StaticClassWithNewExtensionMethods));
	}

	[Fact]
	public async Task Types_WhenIncludingCompilerGenerated_ShouldContainExtensionGroupingTypes()
	{
		using (Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers()
			       .Set(CompilerGeneratedMembers.Types))
		{
			IReadOnlyList<Type> types = await GetTypes();

			await That(types).Contains(t => t.DeclaringType == typeof(StaticClassWithNewExtensionMethods) &&
			                                t.Name.StartsWith('<'));
		}
	}

	[Fact]
	public async Task Methods_ShouldExcludeExtensionGroupingSkeletonsByDefault()
	{
		IReadOnlyList<MethodInfo> methods = await GetMethods();

		await That(methods).None().Satisfy(m => m.GetCustomAttributesData()
			.Any(a => a.AttributeType.Name == ExtensionMarkerAttributeName));
		await That(methods).Contains(m => m.DeclaringType == typeof(StaticClassWithNewExtensionMethods) &&
		                                  m.Name == nameof(StaticClassWithNewExtensionMethods.Create) &&
		                                  m.GetParameters().Length == 0);
	}

	private static async Task<IReadOnlyList<Type>> GetTypes()
		=> await ToList(In.AssemblyContaining<ExtensionGroupingTypeFilteringTests>().Types());

	private static async Task<IReadOnlyList<MethodInfo>> GetMethods()
		=> await ToList(In.AssemblyContaining<ExtensionGroupingTypeFilteringTests>().Types().Methods());

	private static async Task<List<T>> ToList<T>(IAsyncEnumerable<T> source)
	{
		List<T> result = new();
		await foreach (T item in source)
		{
			result.Add(item);
		}

		return result;
	}
}
#endif
