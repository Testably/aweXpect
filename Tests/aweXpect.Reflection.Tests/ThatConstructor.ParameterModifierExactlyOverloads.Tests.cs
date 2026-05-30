using System.IO;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class ParameterModifierExactlyOverloads
	{
		private static ConstructorInfo Constructor<T>()
			=> typeof(T).GetConstructors().Single();

		public sealed class Tests
		{
			[Fact]
			public async Task HasInParameter_OfBaseType_ShouldMatchDerivedParameterType()
			{
				ConstructorInfo constructorInfo = Constructor<InMemoryStreamCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasInParameterExactly_OfBaseType_ShouldNotMatchDerivedParameterType()
			{
				ConstructorInfo constructorInfo = Constructor<InMemoryStreamCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasInParameterExactly<Stream>();
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasInParameterExactly_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<InIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasInParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasInParameterExactlyAndName_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<InIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasInParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasInParameterExactlyAndName_WhenWrongName_ShouldFail()
			{
				ConstructorInfo constructorInfo = Constructor<InIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasInParameterExactly<int>("other");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasOptionalParameterExactly_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<OptionalIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOptionalParameterExactlyAndName_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<OptionalIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOptionalParameterExactlyAndName_WhenWrongName_ShouldFail()
			{
				ConstructorInfo constructorInfo = Constructor<OptionalIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameterExactly<int>("other");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasOutParameter_OfBaseType_ShouldMatchDerivedParameterType()
			{
				ConstructorInfo constructorInfo = Constructor<OutMemoryStreamCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameter<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOutParameterExactly_OfBaseType_ShouldNotMatchDerivedParameterType()
			{
				ConstructorInfo constructorInfo = Constructor<OutMemoryStreamCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameterExactly<Stream>();
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasOutParameterExactly_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<OutIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOutParameterExactlyAndName_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<OutIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOutParameterExactlyAndName_WhenWrongName_ShouldFail()
			{
				ConstructorInfo constructorInfo = Constructor<OutIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameterExactly<int>("other");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasParamsParameterExactly_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<ParamsIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameterExactly<int[]>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParamsParameterExactlyAndName_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<ParamsIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameterExactly<int[]>("values");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParamsParameterExactlyAndName_WhenWrongName_ShouldFail()
			{
				ConstructorInfo constructorInfo = Constructor<ParamsIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameterExactly<int[]>("other");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasRefParameter_OfBaseType_ShouldMatchDerivedParameterType()
			{
				ConstructorInfo constructorInfo = Constructor<RefMemoryStreamCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasRefParameterExactly_OfBaseType_ShouldNotMatchDerivedParameterType()
			{
				ConstructorInfo constructorInfo = Constructor<RefMemoryStreamCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameterExactly<Stream>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type Stream with ref modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasRefParameterExactly_OfExactType_ShouldMatchParameterType()
			{
				ConstructorInfo constructorInfo = Constructor<RefMemoryStreamCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameterExactly<MemoryStream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasRefParameterExactly_WhenByRefParameterUnwrapsToExactType_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<RefIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasRefParameterExactlyAndName_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<RefIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasRefParameterExactlyAndName_WhenWrongName_ShouldFail()
			{
				ConstructorInfo constructorInfo = Constructor<RefIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameterExactly<int>("other");
				}

				await That(Act).Throws<XunitException>();
			}
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

		private class RefMemoryStreamCtor
		{
			public RefMemoryStreamCtor(ref MemoryStream value)
			{
				value = null!;
			}
		}

		private class OutMemoryStreamCtor
		{
			public OutMemoryStreamCtor(out MemoryStream value)
			{
				value = null!;
			}
		}

		private class InMemoryStreamCtor
		{
			public InMemoryStreamCtor(in MemoryStream value)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
