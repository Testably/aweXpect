using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

// The System.Type overloads are deliberately exercised here alongside the generic ones.
#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatConstructor
{
	public sealed class HasParameterExactly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasParameterExactly<Stream>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type Stream,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenParameterIsExactType_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterExactly<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenParameterIsExactTypeWithName_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterExactly<Stream>("stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenParameterIsSubtype_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterExactly<IDisposable>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type IDisposable,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenParameterIsSubtypeWithMatchingName_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterExactly<IDisposable>("stream");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type IDisposable with name "stream",
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenParameterNameDoesNotMatch_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterExactly<Stream>("other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type Stream with name "other",
					             but it did not
					             """);
			}

			[Fact]
			public async Task WithType_WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasParameterExactly(typeof(Stream));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type Stream,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithType_WhenParameterIsExactType_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterExactly(typeof(Stream));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithType_WhenParameterIsExactTypeWithName_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterExactly(typeof(Stream), "stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithType_WhenParameterIsSubtype_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterExactly(typeof(IDisposable));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type IDisposable,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WithType_WhenParameterIsSubtypeWithMatchingName_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterExactly(typeof(IDisposable), "stream");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type IDisposable with name "stream",
					             but it did not
					             """);
			}

			[Fact]
			public async Task WithType_WhenParameterNameDoesNotMatch_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterExactly(typeof(Stream), "other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type Stream with name "other",
					             but it did not
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenParameterIsExactType_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameterExactly<Stream>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have parameter of exact type Stream,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenParameterIsSubtype_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameterExactly<IDisposable>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithType_WhenParameterIsExactType_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameterExactly(typeof(Stream)));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have parameter of exact type Stream,
					             but it did
					             """);
			}

			[Fact]
			public async Task WithType_WhenParameterIsSubtype_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameterExactly(typeof(IDisposable)));
				}

				await That(Act).DoesNotThrow();
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public TestClass(Stream stream) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
