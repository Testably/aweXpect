using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class HasExactParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenParameterIsExactType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
					=> await That(methodInfo).HasExactParameter<Stream>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenParameterIsSubtype_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
					=> await That(methodInfo).HasExactParameter<IDisposable>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type IDisposable,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenParameterIsExactTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
					=> await That(methodInfo).HasExactParameter<Stream>("stream");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenParameterNameDoesNotMatch_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
					=> await That(methodInfo).HasExactParameter<Stream>("other");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type Stream with name "other",
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? methodInfo = null;

				async Task Act()
					=> await That(methodInfo).HasExactParameter<Stream>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type Stream,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenParameterIsSubtype_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
					=> await That(methodInfo).DoesNotComplyWith(it => it.HasExactParameter<IDisposable>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenParameterIsExactType_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
					=> await That(methodInfo).DoesNotComplyWith(it => it.HasExactParameter<Stream>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have parameter of exact type Stream,
					             but it did
					             """);
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithStream(Stream stream) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
