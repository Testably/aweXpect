using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using aweXpect.Core.Adapters;
using aweXpect.Core.Initialization;
using aweXpect.Customization;

namespace aweXpect.Core.Tests.Core.Initialization;

public sealed class AweXpectInitializationTests
{
#if !NET8_0_OR_GREATER
	[Test]
	public async Task DetectFramework_WhenAllFrameworksAreNotAvailable_ShouldReturnNull()
	{
		ITestFrameworkAdapter? result = AweXpectInitialization.DetectFramework([typeof(UnavailableFrameworkAdapter),]);

		await That(result).IsNull();
	}

	[Test]
	public async Task DetectFramework_WhenFrameworkAdapterThrows_ShouldThrowInvalidOperationException()
	{
		void Act() => AweXpectInitialization.DetectFramework([typeof(IncorrectFrameworkAdapter),]);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				$"Could not instantiate test framework AweXpectInitializationTests.{nameof(IncorrectFrameworkAdapter)}.");
	}

	[Test]
	[Arguments("System", "exact framework assembly name")]
	[Arguments("System.Net.Http", "sub-name of an excluded prefix")]
	[Arguments("Microsoft.Extensions.Logging")]
	[Arguments("netstandard")]
	[Arguments("WindowsBase")]
	[Arguments("xunit.core")]
	[Arguments("TUnit.Core")]
	[Arguments("TUnit.Engine")]
	[Arguments("DynamicProxyGenAssembly2")]
	public async Task IsAssemblyNameIncluded_WhenExcludedByDefault_ShouldReturnFalse(string assemblyName, string? because = null)
	{
		bool included = AweXpectInitialization.IsAssemblyNameIncluded(assemblyName);

		await That(included).IsEqualTo(false).Because(because);
	}

	[Test]
	[Arguments("Systemics", "shares the \"System\" prefix, but not at a name boundary")]
	[Arguments("Microsoftish", "shares the \"Microsoft\" prefix, but not at a name boundary")]
	[Arguments("WindowsBaseExtensions", "shares the \"WindowsBase\" prefix, but not at a name boundary")]
	[Arguments("MyCompany.Product")]
	public async Task IsAssemblyNameIncluded_WhenNotExcluded_ShouldReturnTrue(string assemblyName, string? because = null)
	{
		bool included = AweXpectInitialization.IsAssemblyNameIncluded(assemblyName);

		await That(included).IsEqualTo(true).Because(because);
	}

	[Test]
	[Arguments("MyCompany.", "MyCompany.Product", false, "a prefix ending with a dot excludes every name starting with it")]
	[Arguments("MyCompany.", "MyCompany", true, "a prefix ending with a dot requires the dot")]
	[Arguments("", "MyCompany.Product", true, "an empty prefix excludes nothing")]
	[Arguments("MyCompany", "MyCompany", false, "a prefix excludes the assembly with exactly its name")]
	[Arguments("MyCompany", "MyCompany.Product", false, "a prefix excludes the names continuing with a dot after it")]
	[Arguments("MyCompany", "MyCompanyProduct", true, "a prefix only matches at a name-segment boundary")]
	public async Task IsAssemblyNameIncluded_WithCustomizedPrefix_ShouldMatchLikeAweXpectReflection(
		string prefix, string assemblyName, bool expected, string because)
	{
		bool included;
		using (Customize.aweXpect.Settings().ExcludedAssemblyPrefixes.Set([prefix,]))
		{
			included = AweXpectInitialization.IsAssemblyNameIncluded(assemblyName);
		}

		await That(included).IsEqualTo(expected).Because(because);
	}

	[Test]
	[Arguments(null)]
	[Arguments("")]
	public async Task IsAssemblyNameIncluded_WithoutName_ShouldReturnFalse(string? assemblyName)
	{
		bool included = AweXpectInitialization.IsAssemblyNameIncluded(assemblyName);

		await That(included).IsEqualTo(false);
	}
#endif

	[Test]
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
	[Test]
	public async Task DetectTestFramework_WhenNothingIsRegistered_ShouldReturnTheFallback()
	{
		TestFrameworkRegistry.Registration registration = new();

		ITestFrameworkAdapter result = AweXpectInitialization.DetectTestFramework(registration);

		await That(result.IsAvailable).IsFalse()
			.Because("the generated adapter registers itself, so no adapter is available without a registration");
	}
#else
	[Test]
	public async Task DetectTestFramework_WhenNothingIsRegistered_ShouldScanTheLoadedAssemblies()
	{
		TestFrameworkRegistry.Registration registration = new();

		ITestFrameworkAdapter result = AweXpectInitialization.DetectTestFramework(registration);

		await That(result.IsAvailable).IsTrue()
			.Because("the adapter cannot register itself without `ModuleInitializerAttribute`");
	}
#endif

#if NET8_0_OR_GREATER
	[Test]
	public async Task Fallback_Fail_ShouldThrowFailException()
	{
		ITestFrameworkAdapter fallback = AweXpectInitialization.DetectTestFramework(new TestFrameworkRegistry.Registration());

		void Act() => fallback.Fail("my failure");

		await That(Act).Throws<FailException>().WithMessage("my failure");
	}

	[Test]
	public async Task Fallback_FailWithInnerException_ShouldThrowFailExceptionWithTheInnerException()
	{
		ITestFrameworkAdapter fallback = AweXpectInitialization.DetectTestFramework(new TestFrameworkRegistry.Registration());
		InvalidOperationException innerException = new("the cause");

		void Act() => fallback.Fail("my failure", innerException);

		await That(Act).Throws<FailException>().WithMessage("my failure").And
			.WithInner<InvalidOperationException>(inner => inner.IsSameAs(innerException));
	}

	[Test]
	public async Task Fallback_Inconclusive_ShouldThrowInconclusiveException()
	{
		ITestFrameworkAdapter fallback = AweXpectInitialization.DetectTestFramework(new TestFrameworkRegistry.Registration());

		void Act() => fallback.Inconclusive("my reason");

		await That(Act).Throws<InconclusiveException>().WithMessage("my reason");
	}

	[Test]
	public async Task Fallback_Skip_ShouldThrowSkipException()
	{
		ITestFrameworkAdapter fallback = AweXpectInitialization.DetectTestFramework(new TestFrameworkRegistry.Registration());

		void Act() => fallback.Skip("my reason");

		await That(Act).Throws<SkipException>().WithMessage("my reason");
	}

	[Test]
	public async Task Fallback_TryAddAttachment_ShouldReturnFalse()
	{
		ITestFrameworkAdapter fallback = AweXpectInitialization.DetectTestFramework(new TestFrameworkRegistry.Registration());

		bool isAttached = fallback.TryAddAttachment("snapshot.txt", null);

		await That(isAttached).IsFalse();
	}

	[Test]
	public async Task Fallback_TryGetCancellationToken_ShouldReturnFalse()
	{
		ITestFrameworkAdapter fallback = AweXpectInitialization.DetectTestFramework(new TestFrameworkRegistry.Registration());

		bool hasToken = fallback.TryGetCancellationToken(out CancellationToken cancellationToken);

		await That(hasToken).IsFalse();
		await That(cancellationToken).IsEqualTo(CancellationToken.None);
	}

	[Test]
	public async Task Fallback_TryGetCurrentTest_ShouldReturnFalse()
	{
		ITestFrameworkAdapter fallback = AweXpectInitialization.DetectTestFramework(new TestFrameworkRegistry.Registration());

		bool isRunning = fallback.TryGetCurrentTest(out RunningTest? currentTest);

		await That(isRunning).IsFalse();
		await That(currentTest).IsNull();
	}
#endif

	[Test]
	public async Task TryAddAttachment_WhenFrameworkDoesNotSupportAttachments_ShouldReturnFalse()
	{
		AweXpectInitialization.InitializationState state = new(new RegisteredFrameworkAdapter());

		bool isAttached = state.TryAddAttachment("snapshot.txt", "the received snapshot");

		await That(isAttached).IsFalse();
	}

	[Test]
	public async Task TryAddAttachment_WhenFrameworkSupportsAttachments_ShouldAttachTheFile()
	{
		TestContextFrameworkAdapter adapter = new(
			new RunningTest(new TestIdentity("MyTests.AlbumTests", "Album", []), null, null, 0),
			CancellationToken.None);
		AweXpectInitialization.InitializationState state = new(adapter);

		bool isAttached = state.TryAddAttachment("snapshot.txt", "the received snapshot");

		await That(isAttached).IsTrue();
		await That(adapter.Attachments).IsEqualTo([("snapshot.txt", "the received snapshot"),]);
	}

	[Test]
	public async Task TryGetCancellationToken_WhenFrameworkDoesNotExposeIt_ShouldReturnFalse()
	{
		AweXpectInitialization.InitializationState state = new(new RegisteredFrameworkAdapter());

		bool hasToken = state.TryGetCancellationToken(out CancellationToken cancellationToken);

		await That(hasToken).IsFalse();
		await That(cancellationToken).IsEqualTo(CancellationToken.None);
	}

	[Test]
	public async Task TryGetCancellationToken_WhenFrameworkExposesIt_ShouldReturnTheToken()
	{
		using CancellationTokenSource cts = new();
		AweXpectInitialization.InitializationState state = new(
			new TestContextFrameworkAdapter(
				new RunningTest(new TestIdentity("MyTests.AlbumTests", "Album", []), null, null, 0), cts.Token));

		bool hasToken = state.TryGetCancellationToken(out CancellationToken cancellationToken);

		await That(hasToken).IsTrue();
		await That(cancellationToken).IsEqualTo(cts.Token);
	}

	[Test]
	public async Task TryGetCurrentTest_WhenFrameworkDoesNotExposeTheRunningTest_ShouldReturnFalse()
	{
		AweXpectInitialization.InitializationState state = new(new RegisteredFrameworkAdapter());

		bool isRunning = state.TryGetCurrentTest(out RunningTest? currentTest);

		await That(isRunning).IsFalse()
			.Because("an adapter that only reports the results does not know the running test");
		await That(currentTest).IsNull();
	}

	[Test]
	public async Task TryGetCurrentTest_WhenFrameworkExposesTheRunningTest_ShouldReturnItsTest()
	{
		RunningTest runningTest = new(
			new TestIdentity("MyTests.AlbumTests", "Album_MatchesSnapshot", ["Abbey Road",]), null, null, 0);
		AweXpectInitialization.InitializationState state = new(
			new TestContextFrameworkAdapter(runningTest, CancellationToken.None));

		bool isRunning = state.TryGetCurrentTest(out RunningTest? currentTest);

		await That(isRunning).IsTrue();
		await That(currentTest).IsSameAs(runningTest);
	}

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

		public bool TryGetCurrentTest([NotNullWhen(true)] out RunningTest? currentTest)
		{
			currentTest = null;
			return false;
		}

		public bool TryGetCancellationToken(out CancellationToken cancellationToken)
		{
			cancellationToken = CancellationToken.None;
			return false;
		}

		public bool TryAddAttachment(string filePath, string? description) => false;
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

		public bool TryGetCurrentTest([NotNullWhen(true)] out RunningTest? currentTest)
		{
			currentTest = null;
			return false;
		}

		public bool TryGetCancellationToken(out CancellationToken cancellationToken)
		{
			cancellationToken = CancellationToken.None;
			return false;
		}

		public bool TryAddAttachment(string filePath, string? description) => false;
#pragma warning restore CS0436
	}

	private sealed class TestContextFrameworkAdapter(RunningTest runningTest, CancellationToken testToken)
		: ITestFrameworkAdapter
	{
		public List<(string FilePath, string? Description)> Attachments { get; } = [];

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

		public bool TryGetCurrentTest([NotNullWhen(true)] out RunningTest? currentTest)
		{
			currentTest = runningTest;
			return true;
		}

		public bool TryGetCancellationToken(out CancellationToken cancellationToken)
		{
			cancellationToken = testToken;
			return true;
		}

		public bool TryAddAttachment(string filePath, string? description)
		{
			Attachments.Add((filePath, description));
			return true;
		}
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

		public bool TryGetCurrentTest([NotNullWhen(true)] out RunningTest? currentTest)
		{
			currentTest = null;
			return false;
		}

		public bool TryGetCancellationToken(out CancellationToken cancellationToken)
		{
			cancellationToken = CancellationToken.None;
			return false;
		}

		public bool TryAddAttachment(string filePath, string? description) => false;
#pragma warning restore CS0436
	}
#endif
}
