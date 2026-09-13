using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Adapters;
using aweXpect.Core.Initialization;

namespace aweXpect.Core.Tests.Core.Initialization;

public sealed class AweXpectInitializationTests
{
#if !NET8_0_OR_GREATER
	[Fact]
	public async Task DetectFramework_WhenAllFrameworksAreNotAvailable_ShouldReturnNull()
	{
		ITestFrameworkAdapter? result = AweXpectInitialization.DetectFramework([typeof(UnavailableFrameworkAdapter),]);

		await That(result).IsNull();
	}

	[Fact]
	public async Task DetectFramework_WhenFrameworkAdapterThrows_ShouldThrowInvalidOperationException()
	{
		void Act() => AweXpectInitialization.DetectFramework([typeof(IncorrectFrameworkAdapter),]);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				$"Could not instantiate test framework 'AweXpectInitializationTests.{nameof(IncorrectFrameworkAdapter)}'!");
	}

	[Theory]
	[InlineData("System", "exact framework assembly name")]
	[InlineData("System.Net.Http", "sub-name of an excluded prefix")]
	[InlineData("Microsoft.Extensions.Logging")]
	[InlineData("netstandard")]
	[InlineData("WindowsBase")]
	[InlineData("xunit.core")]
	[InlineData("DynamicProxyGenAssembly2")]
	public async Task IsAssemblyNameIncluded_WhenExcludedByDefault_ShouldReturnFalse(string assemblyName, string? because = null)
	{
		bool included = AweXpectInitialization.IsAssemblyNameIncluded(assemblyName);

		await That(included).IsEqualTo(false).Because(because);
	}

	[Theory]
	[InlineData("Systemics", "shares the \"System\" prefix, but not at a name boundary")]
	[InlineData("Microsoftish", "shares the \"Microsoft\" prefix, but not at a name boundary")]
	[InlineData("WindowsBaseExtensions", "shares the \"WindowsBase\" prefix, but not at a name boundary")]
	[InlineData("MyCompany.Product")]
	public async Task IsAssemblyNameIncluded_WhenNotExcluded_ShouldReturnTrue(string assemblyName, string? because = null)
	{
		bool included = AweXpectInitialization.IsAssemblyNameIncluded(assemblyName);

		await That(included).IsEqualTo(true).Because(because);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public async Task IsAssemblyNameIncluded_WithoutName_ShouldReturnFalse(string? assemblyName)
	{
		bool included = AweXpectInitialization.IsAssemblyNameIncluded(assemblyName);

		await That(included).IsEqualTo(false);
	}
#endif

	[Fact]
	public async Task DetectTestFramework_WhenAdapterIsRegistered_ShouldReturnRegisteredAdapter()
	{
		TestFrameworkRegistry.Registration registration = new();
		RegisteredFrameworkAdapter registered = new();
		registration.Add(registered, true);

		ITestFrameworkAdapter result = AweXpectInitialization.DetectTestFramework(registration);

		await That(result).IsSameAs(registered)
			.Because("a registered adapter makes scanning the loaded assemblies unnecessary");
	}

#if NET8_0_OR_GREATER
	[Fact]
	public async Task DetectTestFramework_WhenNothingIsRegistered_ShouldReturnTheFallback()
	{
		TestFrameworkRegistry.Registration registration = new();

		ITestFrameworkAdapter result = AweXpectInitialization.DetectTestFramework(registration);

		await That(result.IsAvailable).IsFalse()
			.Because("the generated adapter registers itself, so no adapter is available without a registration");
	}
#else
	[Fact]
	public async Task DetectTestFramework_WhenNothingIsRegistered_ShouldScanTheLoadedAssemblies()
	{
		TestFrameworkRegistry.Registration registration = new();

		ITestFrameworkAdapter result = AweXpectInitialization.DetectTestFramework(registration);

		await That(result.IsAvailable).IsTrue()
			.Because("the adapter cannot register itself without `ModuleInitializerAttribute`");
	}
#endif

#if !NET8_0_OR_GREATER
	private sealed class UnavailableFrameworkAdapter : ITestFrameworkAdapter
	{
		public bool IsAvailable => false;

#pragma warning disable CS0436
		[DoesNotReturn]
		public void Fail(string message) => throw new NotSupportedException();

		[DoesNotReturn]
		public void Fail(string message, Exception innerException) => throw new NotSupportedException();

		[DoesNotReturn]
		public void Inconclusive(string message) => throw new NotSupportedException();

		[DoesNotReturn]
		public void Skip(string message) => throw new NotSupportedException();
#pragma warning restore CS0436
	}
#endif

	private sealed class RegisteredFrameworkAdapter : ITestFrameworkAdapter
	{
		public bool IsAvailable => true;

#pragma warning disable CS0436
		[DoesNotReturn]
		public void Fail(string message) => throw new NotSupportedException();

		[DoesNotReturn]
		public void Fail(string message, Exception innerException) => throw new NotSupportedException();

		[DoesNotReturn]
		public void Inconclusive(string message) => throw new NotSupportedException();

		[DoesNotReturn]
		public void Skip(string message) => throw new NotSupportedException();
#pragma warning restore CS0436
	}

#if !NET8_0_OR_GREATER
	private sealed class IncorrectFrameworkAdapter : ITestFrameworkAdapter
	{
		public bool IsAvailable => throw new NotSupportedException("Could not load the IncorrectFrameworkAdapter");

#pragma warning disable CS0436
		[DoesNotReturn]
		public void Fail(string message) => throw new NotSupportedException();

		[DoesNotReturn]
		public void Fail(string message, Exception innerException) => throw new NotSupportedException();

		[DoesNotReturn]
		public void Inconclusive(string message) => throw new NotSupportedException();

		[DoesNotReturn]
		public void Skip(string message) => throw new NotSupportedException();
#pragma warning restore CS0436
	}
#endif
}
