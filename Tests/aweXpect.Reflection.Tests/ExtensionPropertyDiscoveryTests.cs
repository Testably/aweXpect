#if NET10_0_OR_GREATER
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using aweXpect.Customization;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

/// <summary>
///     The C# extension block syntax surfaces extension properties only on the hidden <c>&lt;G&gt;$…</c> grouping
///     types, while the public static class exposes their (non-special-name) accessor methods. Discovery must surface
///     the properties and must not let the accessor methods pollute the method results.
/// </summary>
public sealed class ExtensionPropertyDiscoveryTests
{
	[Fact]
	public async Task Properties_ShouldSurfaceExtensionProperties()
	{
		IReadOnlyList<PropertyInfo> properties = await GetProperties();

		await That(properties).Contains(p => p.DeclaringType?.DeclaringType ==
		                                     typeof(StaticClassWithNewExtensionProperties) &&
		                                     p.Name == "IsBlankText");
		await That(properties).Contains(p => p.DeclaringType?.DeclaringType ==
		                                     typeof(StaticClassWithNewExtensionProperties) &&
		                                     p.Name == "DefaultValue");
		await That(properties).Contains(p => p.DeclaringType?.DeclaringType ==
		                                     typeof(GenericClassWithNewExtensionProperties) &&
		                                     p.Name == "Capacity");
	}

	[Fact]
	public async Task Methods_ShouldExcludeExtensionPropertyAccessorsByDefault()
	{
		IReadOnlyList<MethodInfo> methods = await GetMethods();

		await That(methods).None().Satisfy(m =>
			m.DeclaringType == typeof(StaticClassWithNewExtensionProperties) &&
			(m.Name == "get_IsBlankText" || m.Name == "get_DefaultValue"));
		await That(methods).Contains(m => m.DeclaringType == typeof(StaticClassWithNewExtensionProperties) &&
		                                  m.Name == nameof(StaticClassWithNewExtensionProperties.IsLongText));
	}

	[Fact]
	public async Task Methods_WhenIncludingAccessors_ShouldContainExtensionPropertyAccessors()
	{
		using (Customize.aweXpect.Reflection().IncludedSpecialNameMembers().Set(SpecialNameMembers.Accessors))
		{
			IReadOnlyList<MethodInfo> methods = await GetMethods();

			await That(methods).Contains(m => m.DeclaringType == typeof(StaticClassWithNewExtensionProperties) &&
			                                  m.Name == "get_IsBlankText");
		}
	}

	[Fact]
	public async Task Methods_ShouldExcludeSettableExtensionPropertyAccessorsByDefault()
	{
		IReadOnlyList<MethodInfo> methods = await GetMethods();

		await That(methods).None().Satisfy(m =>
			m.DeclaringType == typeof(StaticClassWithNewExtensionProperties) &&
			(m.Name == "get_MutableDefault" || m.Name == "set_MutableDefault"));
	}

	[Fact]
	public async Task Methods_WhenIncludingAccessors_ShouldContainSettableExtensionPropertyAccessors()
	{
		using (Customize.aweXpect.Reflection().IncludedSpecialNameMembers().Set(SpecialNameMembers.Accessors))
		{
			IReadOnlyList<MethodInfo> methods = await GetMethods();

			await That(methods).Contains(m => m.DeclaringType == typeof(StaticClassWithNewExtensionProperties) &&
			                                  m.Name == "get_MutableDefault");
			await That(methods).Contains(m => m.DeclaringType == typeof(StaticClassWithNewExtensionProperties) &&
			                                  m.Name == "set_MutableDefault");
		}
	}

	[Fact]
	public async Task Properties_ShouldSurfaceSettableExtensionProperty()
	{
		IReadOnlyList<PropertyInfo> properties = await GetProperties();

		await That(properties).Contains(p => p.DeclaringType?.DeclaringType ==
		                                     typeof(StaticClassWithNewExtensionProperties) &&
		                                     p.Name == "MutableDefault" &&
		                                     p.CanRead && p.CanWrite);
	}

	private static async Task<IReadOnlyList<PropertyInfo>> GetProperties()
		=> await ToList(In.AssemblyContaining<ExtensionPropertyDiscoveryTests>().Types().Properties());

	private static async Task<IReadOnlyList<MethodInfo>> GetMethods()
		=> await ToList(In.AssemblyContaining<ExtensionPropertyDiscoveryTests>().Types().Methods());

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
