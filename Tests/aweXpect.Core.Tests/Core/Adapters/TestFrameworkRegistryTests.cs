using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Adapters;

namespace aweXpect.Core.Tests.Core.Adapters;

public sealed class TestFrameworkRegistryTests
{
	[Fact]
	public async Task Add_WhenAdapterIsNotAvailable_ShouldBeIgnored()
	{
		TestFrameworkRegistry.Registration registration = new();

		registration.Add(new UnavailableAdapter(), true);

		await That(registration.TestFrameworkAdapter).IsNull()
			.Because("an unavailable adapter cannot report test results");
	}

	[Fact]
	public async Task Add_WhenNotOverwriting_ShouldKeepTheFirstAdapter()
	{
		TestFrameworkRegistry.Registration registration = new();
		AvailableAdapter first = new();
		AvailableAdapter second = new();

		registration.Add(first, false);
		registration.Add(second, false);

		await That(registration.TestFrameworkAdapter).IsSameAs(first)
			.Because("two generated registrations must resolve deterministically");
	}

	[Fact]
	public async Task Add_WhenNotOverwriting_ShouldNotReplaceAnExplicitlyRegisteredAdapter()
	{
		TestFrameworkRegistry.Registration registration = new();
		AvailableAdapter explicitlyRegistered = new();

		registration.Add(explicitlyRegistered, true);
		registration.Add(new AvailableAdapter(), false);

		await That(registration.TestFrameworkAdapter).IsSameAs(explicitlyRegistered)
			.Because("a generated registration must never win over an explicit one");
	}

	[Fact]
	public async Task Add_WhenOverwriting_ShouldReplaceAGeneratedAdapter()
	{
		TestFrameworkRegistry.Registration registration = new();
		AvailableAdapter explicitlyRegistered = new();

		registration.Add(new AvailableAdapter(), false);
		registration.Add(explicitlyRegistered, true);

		await That(registration.TestFrameworkAdapter).IsSameAs(explicitlyRegistered)
			.Because("an explicit registration must win, even though the module initializer ran first");
	}

	[Fact]
	public async Task Add_WhenUnavailableAdapterWasRegisteredBefore_ShouldUseTheAvailableAdapter()
	{
		TestFrameworkRegistry.Registration registration = new();
		AvailableAdapter available = new();

		registration.Add(new UnavailableAdapter(), false);
		registration.Add(available, false);

		await That(registration.TestFrameworkAdapter).IsSameAs(available);
	}

	[Fact]
	public async Task Add_WithNullAdapter_ShouldThrowArgumentNullException()
	{
		TestFrameworkRegistry.Registration registration = new();

		void Act() => registration.Add(null!, true);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("testFrameworkAdapter");
	}

	[Fact]
	public async Task TestFrameworkAdapter_WhenNothingIsRegistered_ShouldBeNull()
	{
		TestFrameworkRegistry.Registration registration = new();

		await That(registration.TestFrameworkAdapter).IsNull();
	}

	private sealed class AvailableAdapter : ITestFrameworkAdapter
	{
		public bool IsAvailable => true;

#pragma warning disable CS0436
		[DoesNotReturn]
		public void Fail(string message) => throw new NotSupportedException(message);

		[DoesNotReturn]
		public void Fail(string message, Exception innerException) => throw new NotSupportedException(message);

		[DoesNotReturn]
		public void Inconclusive(string message) => throw new NotSupportedException(message);

		[DoesNotReturn]
		public void Skip(string message) => throw new NotSupportedException(message);
#pragma warning restore CS0436
	}

	private sealed class UnavailableAdapter : ITestFrameworkAdapter
	{
		public bool IsAvailable => false;

#pragma warning disable CS0436
		[DoesNotReturn]
		public void Fail(string message) => throw new NotSupportedException(message);

		[DoesNotReturn]
		public void Fail(string message, Exception innerException) => throw new NotSupportedException(message);

		[DoesNotReturn]
		public void Inconclusive(string message) => throw new NotSupportedException(message);

		[DoesNotReturn]
		public void Skip(string message) => throw new NotSupportedException(message);
#pragma warning restore CS0436
	}
}
