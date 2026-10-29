using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class ParameterModifierExactlyOverloads
	{
		private static ConstructorInfo Constructor<T>()
			=> typeof(T).GetConstructors().Single();

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
			[Test]
			public async Task HaveInParameterExactly_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<InIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveInParameterExactlyAndName_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<InIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveOptionalParameterExactly_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<OptionalIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveOptionalParameterExactlyAndName_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<OptionalIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveOutParameterExactly_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<OutIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveOutParameterExactlyAndName_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<OutIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveParamsParameterExactly_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<ParamsIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly<int[]>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveParamsParameterExactlyAndName_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<ParamsIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly<int[]>("values");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveRefParameter_OfBaseType_ShouldMatchDerivedParameterType()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<RefMemoryStreamCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameter<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveRefParameterExactly_OfBaseType_ShouldNotMatchDerivedParameterType()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<RefMemoryStreamCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameterExactly<Stream>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type Stream with ref modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.ParameterModifierExactlyOverloads.RefMemoryStreamCtor(ref MemoryStream value)
					             ]

					             Collection:
					             [
					               ThatConstructors.ParameterModifierExactlyOverloads.RefMemoryStreamCtor(ref MemoryStream value)
					             ]
					             """);
			}

			[Test]
			public async Task HaveRefParameterExactly_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<RefIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveRefParameterExactly_WhenNotAllMatch_ShouldFailWithExactTypeWording()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<RefIntCtor>(), Constructor<PlainIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameterExactly<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.ParameterModifierExactlyOverloads.PlainIntCtor(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.ParameterModifierExactlyOverloads.RefIntCtor(ref int value),
					               ThatConstructors.ParameterModifierExactlyOverloads.PlainIntCtor(int value)
					             ]
					             """);
			}

			[Test]
			public async Task HaveRefParameterExactlyAndName_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<RefIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task HaveRefParameterExactly_AsyncEnumerable_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					Constructor<RefIntCtor>());

				async Task Act()
				{
					await That(constructors).HaveRefParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveRefParameterExactly_AsyncEnumerable_WhenNotAllMatch_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					Constructor<RefIntCtor>(),
					Constructor<PlainIntCtor>());

				async Task Act()
				{
					await That(constructors).HaveRefParameterExactly<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.ParameterModifierExactlyOverloads.PlainIntCtor(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.ParameterModifierExactlyOverloads.RefIntCtor(ref int value),
					               ThatConstructors.ParameterModifierExactlyOverloads.PlainIntCtor(int value)
					             ]
					             """);
			}

			[Test]
			public async Task HaveRefParameterExactly_AsyncEnumerableWithName_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					Constructor<RefIntCtor>());

				async Task Act()
				{
					await That(constructors).HaveRefParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveOutParameterExactly_AsyncEnumerable_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					Constructor<OutIntCtor>());

				async Task Act()
				{
					await That(constructors).HaveOutParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveOutParameterExactly_AsyncEnumerableWithName_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					Constructor<OutIntCtor>());

				async Task Act()
				{
					await That(constructors).HaveOutParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveInParameterExactly_AsyncEnumerable_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					Constructor<InIntCtor>());

				async Task Act()
				{
					await That(constructors).HaveInParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveInParameterExactly_AsyncEnumerableWithName_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					Constructor<InIntCtor>());

				async Task Act()
				{
					await That(constructors).HaveInParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveOptionalParameterExactly_AsyncEnumerable_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					Constructor<OptionalIntCtor>());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveOptionalParameterExactly_AsyncEnumerableWithName_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					Constructor<OptionalIntCtor>());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveParamsParameterExactly_AsyncEnumerable_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					Constructor<ParamsIntCtor>());

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly<int[]>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveParamsParameterExactly_AsyncEnumerableWithName_WhenAllMatch_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					Constructor<ParamsIntCtor>());

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly<int[]>("values");
				}

				await That(Act).DoesNotThrow();
			}
#endif
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class RefIntCtor
		{
			public RefIntCtor(ref int value)
			{
				value = 0;
			}
		}

		private class OutIntCtor
		{
			public OutIntCtor(out int value)
			{
				value = 0;
			}
		}

		private class InIntCtor
		{
			public InIntCtor(in int value)
			{
			}
		}

		private class OptionalIntCtor
		{
			public OptionalIntCtor(int value = 0)
			{
			}
		}

		private class ParamsIntCtor
		{
			public ParamsIntCtor(params int[] values)
			{
			}
		}

		private class PlainIntCtor
		{
			public PlainIntCtor(int value)
			{
			}
		}

		private class RefMemoryStreamCtor
		{
			public RefMemoryStreamCtor(ref MemoryStream value)
			{
				value = null!;
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
