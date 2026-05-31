using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

// The System.Type overloads are deliberately exercised here alongside the generic ones.
#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatConstructors
{
	public sealed class HaveParameterExactly
	{
#if NET8_0_OR_GREATER
		private static async IAsyncEnumerable<ConstructorInfo> ToAsyncEnumerable(params ConstructorInfo[] items)
		{
			foreach (ConstructorInfo item in items)
			{
				yield return item;
			}

			await Task.CompletedTask;
		}
#endif
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveParameterExactly_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!, typeof(OtherClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveParameterExactly<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllHaveParameterExactlyWithName_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveParameterExactly<Stream>("stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAnyParameterIsSubtype_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveParameterExactly<IDisposable>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type IDisposable,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task WhenAllHaveParameterExactlyByType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!, typeof(OtherClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveParameterExactly(typeof(Stream));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllHaveParameterExactlyByTypeWithName_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveParameterExactly(typeof(Stream), "stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAnyParameterIsSubtypeByType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveParameterExactly(typeof(IDisposable));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type IDisposable,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task WhenAnyParameterIsSubtypeWithName_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveParameterExactly<IDisposable>("stream");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type IDisposable with name "stream",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task WhenAnyParameterIsSubtypeByTypeWithName_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveParameterExactly(typeof(IDisposable), "stream");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type IDisposable with name "stream",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task WhenExactTypeMatchesButNameDiffers_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!, // Stream parameter named "stream"
				};

				async Task Act()
				{
					await That(constructors).HaveParameterExactly<Stream>("other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type Stream with name "other",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task WhenExactTypeMatchesButNameDiffersByType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!, // Stream parameter named "stream"
				};

				async Task Act()
				{
					await That(constructors).HaveParameterExactly(typeof(Stream), "other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type Stream with name "other",
					             but at least one did not
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WhenAllHaveParameterExactly_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
					typeof(OtherClass).GetConstructor([typeof(Stream),])!);

				async Task Act()
				{
					await That(constructors).HaveParameterExactly<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WhenAllHaveParameterExactlyWithName_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(TestClass).GetConstructor([typeof(Stream),])!);

				async Task Act()
				{
					await That(constructors).HaveParameterExactly<Stream>("stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WhenAnyParameterIsSubtype_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(TestClass).GetConstructor([typeof(Stream),])!);

				async Task Act()
				{
					await That(constructors).HaveParameterExactly<IDisposable>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type IDisposable,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_WhenAllHaveParameterExactlyByType_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
					typeof(OtherClass).GetConstructor([typeof(Stream),])!);

				async Task Act()
				{
					await That(constructors).HaveParameterExactly(typeof(Stream));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WhenAllHaveParameterExactlyByTypeWithName_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(TestClass).GetConstructor([typeof(Stream),])!);

				async Task Act()
				{
					await That(constructors).HaveParameterExactly(typeof(Stream), "stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WhenAnyParameterIsSubtypeByType_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(TestClass).GetConstructor([typeof(Stream),])!);

				async Task Act()
				{
					await That(constructors).HaveParameterExactly(typeof(IDisposable));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type IDisposable,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_WhenAnyParameterIsSubtypeWithName_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(TestClass).GetConstructor([typeof(Stream),])!);

				async Task Act()
				{
					await That(constructors).HaveParameterExactly<IDisposable>("stream");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type IDisposable with name "stream",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_WhenAnyParameterIsSubtypeByTypeWithName_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(TestClass).GetConstructor([typeof(Stream),])!);

				async Task Act()
				{
					await That(constructors).HaveParameterExactly(typeof(IDisposable), "stream");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type IDisposable with name "stream",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_WhenExactTypeMatchesButNameDiffers_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(TestClass).GetConstructor([typeof(Stream),])!); // Stream parameter named "stream"

				async Task Act()
				{
					await That(constructors).HaveParameterExactly<Stream>("other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type Stream with name "other",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_WhenExactTypeMatchesButNameDiffersByType_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(TestClass).GetConstructor([typeof(Stream),])!); // Stream parameter named "stream"

				async Task Act()
				{
					await That(constructors).HaveParameterExactly(typeof(Stream), "other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type Stream with name "other",
					             but at least one did not
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveParameterExactly_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!, typeof(OtherClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParameterExactly<Stream>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have parameter of exact type Stream,
					             but all did
					             """);
			}

			[Fact]
			public async Task WhenAnyParameterIsSubtype_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParameterExactly<IDisposable>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllHaveParameterExactlyByType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!, typeof(OtherClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParameterExactly(typeof(Stream)));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have parameter of exact type Stream,
					             but all did
					             """);
			}

			[Fact]
			public async Task WhenAnyParameterIsSubtypeByType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParameterExactly(typeof(IDisposable)));
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

		private class OtherClass
		{
			public OtherClass(Stream stream) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
