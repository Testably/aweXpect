using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

// The System.Type overloads are deliberately exercised here alongside the generic ones.
#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatMethod
{
	public sealed class HasParameterExactly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task Type_WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? methodInfo = null;

				async Task Act()
				{
					await That(methodInfo).HasParameterExactly(typeof(Stream));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type Stream,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task Type_WhenParameterIsExactType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).HasParameterExactly(typeof(Stream));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_WhenParameterIsExactTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).HasParameterExactly(typeof(Stream), "stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_WhenParameterIsSubtype_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).HasParameterExactly(typeof(IDisposable));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type IDisposable,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Type_WhenParameterNameDoesNotMatch_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).HasParameterExactly(typeof(Stream), "other");
				}

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
				{
					await That(methodInfo).HasParameterExactly<Stream>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type Stream,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenParameterIsExactType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).HasParameterExactly<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenParameterIsExactTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).HasParameterExactly<Stream>("stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenParameterIsSubtype_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).HasParameterExactly<IDisposable>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type IDisposable,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenParameterNameDoesNotMatch_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).HasParameterExactly<Stream>("other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type Stream with name "other",
					             but it did not
					             """);
			}

			[Fact]
			public async Task WithName_WhenParameterIsSubtype_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).HasParameterExactly<IDisposable>("stream");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type IDisposable with name "stream",
					             but it did not
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task Type_WhenParameterIsExactType_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasParameterExactly(typeof(Stream)));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have parameter of exact type Stream,
					             but it did
					             """);
			}

			[Fact]
			public async Task Type_WhenParameterIsSubtype_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasParameterExactly(typeof(IDisposable)));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenParameterIsExactType_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasParameterExactly<Stream>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have parameter of exact type Stream,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenParameterIsSubtype_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStream))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasParameterExactly<IDisposable>());
				}

				await That(Act).DoesNotThrow();
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
