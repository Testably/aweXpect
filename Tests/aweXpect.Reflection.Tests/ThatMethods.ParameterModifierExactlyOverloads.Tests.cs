using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class ParameterModifierExactlyOverloads
	{
		private static MethodInfo Method(string name)
			=> typeof(ModifierMethods).GetMethod(name)!;

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
			public async Task HaveInParameterExactly_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.InInt)),
				};

				async Task Act()
				{
					await That(methods).HaveInParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveInParameterExactlyAndName_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.InInt)),
				};

				async Task Act()
				{
					await That(methods).HaveInParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveOptionalParameterExactly_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.OptionalInt)),
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveOptionalParameterExactlyAndName_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.OptionalInt)),
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveOutParameterExactly_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.OutInt)),
				};

				async Task Act()
				{
					await That(methods).HaveOutParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveOutParameterExactlyAndName_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.OutInt)),
				};

				async Task Act()
				{
					await That(methods).HaveOutParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParamsParameterExactly_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.ParamsInt)),
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameterExactly<int[]>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParamsParameterExactlyAndName_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.ParamsInt)),
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameterExactly<int[]>("values");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveRefParameter_OfBaseType_ShouldMatchDerivedParameterType()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.RefMemoryStream)),
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveRefParameterExactly_OfBaseType_ShouldNotMatchDerivedParameterType()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.RefMemoryStream)),
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly<Stream>();
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HaveRefParameterExactly_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.RefInt)), Method(nameof(ModifierMethods.AnotherRefInt)),
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveRefParameterExactly_WhenNotAllMatch_ShouldFailWithExactTypeWording()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.RefInt)), Method(nameof(ModifierMethods.PlainInt)),
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type int with ref modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task HaveRefParameterExactlyAndName_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.RefInt)),
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task HaveRefParameterExactly_AsyncEnumerable_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					Method(nameof(ModifierMethods.RefInt)),
					Method(nameof(ModifierMethods.AnotherRefInt)));

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveRefParameterExactly_AsyncEnumerable_WhenNotAllMatch_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					Method(nameof(ModifierMethods.RefInt)),
					Method(nameof(ModifierMethods.PlainInt)));

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly<int>();
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HaveRefParameterExactly_AsyncEnumerableWithName_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					Method(nameof(ModifierMethods.RefInt)));

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveOutParameterExactly_AsyncEnumerable_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					Method(nameof(ModifierMethods.OutInt)));

				async Task Act()
				{
					await That(methods).HaveOutParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveOutParameterExactly_AsyncEnumerableWithName_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					Method(nameof(ModifierMethods.OutInt)));

				async Task Act()
				{
					await That(methods).HaveOutParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveInParameterExactly_AsyncEnumerable_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					Method(nameof(ModifierMethods.InInt)));

				async Task Act()
				{
					await That(methods).HaveInParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveInParameterExactly_AsyncEnumerableWithName_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					Method(nameof(ModifierMethods.InInt)));

				async Task Act()
				{
					await That(methods).HaveInParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveOptionalParameterExactly_AsyncEnumerable_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					Method(nameof(ModifierMethods.OptionalInt)));

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveOptionalParameterExactly_AsyncEnumerableWithName_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					Method(nameof(ModifierMethods.OptionalInt)));

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParamsParameterExactly_AsyncEnumerable_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					Method(nameof(ModifierMethods.ParamsInt)));

				async Task Act()
				{
					await That(methods).HaveParamsParameterExactly<int[]>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParamsParameterExactly_AsyncEnumerableWithName_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					Method(nameof(ModifierMethods.ParamsInt)));

				async Task Act()
				{
					await That(methods).HaveParamsParameterExactly<int[]>("values");
				}

				await That(Act).DoesNotThrow();
			}
#endif
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ModifierMethods
		{
			public void RefInt(ref int value) => value = 0;

			public void AnotherRefInt(ref int value) => value = 0;

			public void OutInt(out int value) => value = 0;

			public void InInt(in int value)
			{
			}

			public void OptionalInt(int value = 0)
			{
			}

			public void ParamsInt(params int[] values)
			{
			}

			public void PlainInt(int value)
			{
			}

			public void RefMemoryStream(ref MemoryStream value) => value = null!;
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
