using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class ParameterModifierExactlyOverloads
	{
		private static MethodInfo Method(string name)
			=> typeof(ModifierMethods).GetMethod(name)!;

		public sealed class Tests
		{
			[Fact]
			public async Task HasInParameter_OfBaseType_ShouldMatchDerivedParameterType()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.InMemoryStream));

				async Task Act()
				{
					await That(methodInfo).HasInParameter<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasInParameterExactly_OfBaseType_ShouldNotMatchDerivedParameterType()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.InMemoryStream));

				async Task Act()
				{
					await That(methodInfo).HasInParameterExactly<Stream>();
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasInParameterExactly_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.InInt));

				async Task Act()
				{
					await That(methodInfo).HasInParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasInParameterExactlyAndName_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.InInt));

				async Task Act()
				{
					await That(methodInfo).HasInParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasInParameterExactlyAndName_WhenWrongName_ShouldFail()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.InInt));

				async Task Act()
				{
					await That(methodInfo).HasInParameterExactly<int>("other");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasOptionalParameterExactly_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.OptionalInt));

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOptionalParameterExactlyAndName_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.OptionalInt));

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOptionalParameterExactlyAndName_WhenWrongName_ShouldFail()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.OptionalInt));

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameterExactly<int>("other");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasOutParameter_OfBaseType_ShouldMatchDerivedParameterType()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.OutMemoryStream));

				async Task Act()
				{
					await That(methodInfo).HasOutParameter<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOutParameterExactly_OfBaseType_ShouldNotMatchDerivedParameterType()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.OutMemoryStream));

				async Task Act()
				{
					await That(methodInfo).HasOutParameterExactly<Stream>();
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasOutParameterExactly_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.OutInt));

				async Task Act()
				{
					await That(methodInfo).HasOutParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOutParameterExactlyAndName_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.OutInt));

				async Task Act()
				{
					await That(methodInfo).HasOutParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOutParameterExactlyAndName_WhenWrongName_ShouldFail()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.OutInt));

				async Task Act()
				{
					await That(methodInfo).HasOutParameterExactly<int>("other");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasParamsParameterExactly_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.ParamsInt));

				async Task Act()
				{
					await That(methodInfo).HasParamsParameterExactly<int[]>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParamsParameterExactlyAndName_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.ParamsInt));

				async Task Act()
				{
					await That(methodInfo).HasParamsParameterExactly<int[]>("values");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParamsParameterExactlyAndName_WhenWrongName_ShouldFail()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.ParamsInt));

				async Task Act()
				{
					await That(methodInfo).HasParamsParameterExactly<int[]>("other");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasRefParameter_OfBaseType_ShouldMatchDerivedParameterType()
			{
				// Contrast: the non-exact overload matches assignable (derived) parameter types.
				MethodInfo methodInfo = Method(nameof(ModifierMethods.RefMemoryStream));

				async Task Act()
				{
					await That(methodInfo).HasRefParameter<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasRefParameterExactly_OfBaseType_ShouldNotMatchDerivedParameterType()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.RefMemoryStream));

				async Task Act()
				{
					await That(methodInfo).HasRefParameterExactly<Stream>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type Stream with ref modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasRefParameterExactly_OfExactType_ShouldMatchParameterType()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.RefMemoryStream));

				async Task Act()
				{
					await That(methodInfo).HasRefParameterExactly<MemoryStream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasRefParameterExactly_WhenByRefParameterUnwrapsToExactType_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.RefInt));

				async Task Act()
				{
					await That(methodInfo).HasRefParameterExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasRefParameterExactly_WhenParameterIsNotRef_ShouldFailWithExactTypeWording()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.PlainInt));

				async Task Act()
				{
					await That(methodInfo).HasRefParameterExactly<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type int with ref modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasRefParameterExactlyAndName_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.RefInt));

				async Task Act()
				{
					await That(methodInfo).HasRefParameterExactly<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasRefParameterExactlyAndName_WhenWrongName_ShouldFail()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.RefInt));

				async Task Act()
				{
					await That(methodInfo).HasRefParameterExactly<int>("other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type int with name "other" with ref modifier,
					             but it did not
					             """);
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ModifierMethods
		{
			public void RefInt(ref int value) => value = 0;

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

			public void OutMemoryStream(out MemoryStream value) => value = null!;

			public void InMemoryStream(in MemoryStream value)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
