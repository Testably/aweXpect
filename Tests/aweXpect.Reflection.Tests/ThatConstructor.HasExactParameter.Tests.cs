using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class HasExactParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasExactParameter<Stream>();
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
					await That(constructorInfo).HasExactParameter<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenParameterIsExactTypeWithName_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasExactParameter<Stream>("stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenParameterIsSubtype_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasExactParameter<IDisposable>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type IDisposable,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenParameterNameDoesNotMatch_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasExactParameter<Stream>("other");
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
					await That(constructorInfo).DoesNotComplyWith(it => it.HasExactParameter<Stream>());
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
					await That(constructorInfo).DoesNotComplyWith(it => it.HasExactParameter<IDisposable>());
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
