using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

// The System.Type overloads are deliberately exercised here alongside the generic ones.
#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatMethods
{
	public sealed class HaveParameterExactly
	{
#if NET8_0_OR_GREATER
		private static async IAsyncEnumerable<MethodInfo> ToAsyncEnumerable(params MethodInfo[] items)
		{
			foreach (MethodInfo item in items)
			{
				yield return item;
			}

			await Task.CompletedTask;
		}
#endif
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveExactType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameterExactly<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllHaveExactTypeWithName_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameterExactly<Stream>("stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveExactType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMemoryStream))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameterExactly<Stream>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task WithName_WhenExactTypeButNameDoesNotMatch_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameterExactly<Stream>("other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream with name "other",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task WithName_WhenSubtypeParameterMatchesName_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMemoryStream))!, // MemoryStream is not exactly Stream
				};

				async Task Act()
				{
					await That(methods).HaveParameterExactly<Stream>("stream");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream with name "stream",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task WithType_WhenAllHaveExactType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameterExactly(typeof(Stream));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithType_WhenAllHaveExactTypeWithName_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameterExactly(typeof(Stream), "stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithType_WhenExactTypeButNameDoesNotMatch_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameterExactly(typeof(Stream), "other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream with name "other",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task WithType_WhenNotAllHaveExactType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMemoryStream))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameterExactly(typeof(Stream));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task WithType_WhenSubtypeParameterMatchesName_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMemoryStream))!, // MemoryStream is not exactly Stream
				};

				async Task Act()
				{
					await That(methods).HaveParameterExactly(typeof(Stream), "stream");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream with name "stream",
					             but at least one did not
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WhenAllHaveExactType_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!,
					typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!);

				async Task Act()
				{
					await That(methods).HaveParameterExactly<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WhenAllHaveExactTypeWithName_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!,
					typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!);

				async Task Act()
				{
					await That(methods).HaveParameterExactly<Stream>("stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WhenNotAllHaveExactType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMemoryStream))!);

				async Task Act()
				{
					await That(methods).HaveParameterExactly<Stream>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_WithName_WhenSubtypeParameterMatchesName_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMemoryStream))!);

				async Task Act()
				{
					await That(methods).HaveParameterExactly<Stream>("stream");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream with name "stream",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_WithName_WhenExactTypeButNameDoesNotMatch_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!,
					typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!);

				async Task Act()
				{
					await That(methods).HaveParameterExactly<Stream>("other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream with name "other",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenAllHaveExactType_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!,
					typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!);

				async Task Act()
				{
					await That(methods).HaveParameterExactly(typeof(Stream));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenAllHaveExactTypeWithName_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!,
					typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!);

				async Task Act()
				{
					await That(methods).HaveParameterExactly(typeof(Stream), "stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveExactType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMemoryStream))!);

				async Task Act()
				{
					await That(methods).HaveParameterExactly(typeof(Stream));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_WithTypeAndName_WhenSubtypeParameterMatchesName_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMemoryStream))!);

				async Task Act()
				{
					await That(methods).HaveParameterExactly(typeof(Stream), "stream");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream with name "stream",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_WithTypeAndName_WhenExactTypeButNameDoesNotMatch_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!,
					typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!);

				async Task Act()
				{
					await That(methods).HaveParameterExactly(typeof(Stream), "other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream with name "other",
					             but at least one did not
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveExactType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveParameterExactly<Stream>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             not all have parameter of exact type Stream,
					             but all did
					             """);
			}

			[Fact]
			public async Task WhenNotAllHaveExactType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMemoryStream))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveParameterExactly<Stream>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithType_WhenAllHaveExactType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveParameterExactly(typeof(Stream)));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             not all have parameter of exact type Stream,
					             but all did
					             """);
			}

			[Fact]
			public async Task WithType_WhenNotAllHaveExactType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMemoryStream))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveParameterExactly(typeof(Stream)));
				}

				await That(Act).DoesNotThrow();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void FirstMethodWithStream(Stream stream) { }
			public void SecondMethodWithStream(Stream stream) { }
			public void MethodWithMemoryStream(MemoryStream stream) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
