using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveOptionalParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(AnotherClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have an optional parameter,
					             but it contained constructors without an optional parameter [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSomeButNotAllParametersAreOptional_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithMixedOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByGenericType_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByGenericTypeAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter<int>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with name "value" with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter with name "value" with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByTypeAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with name "value" with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByGenericTypeExactly_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByTypeExactly_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByGenericTypeExactlyAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly<int>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with name "value" with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByTypeExactlyAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with name "value" with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByType_WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByType_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByTypeAndName_WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByTypeExactly_WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByTypeExactlyAndName_WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactly_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeExactly_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactlyAndName_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WithType_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithOptionalParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task AsyncEnumerable_WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(AnotherClassWithOptionalParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WhenSomeButNotAllParametersAreOptional_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithMixedOptionalParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have an optional parameter,
					             but it contained constructors without an optional parameter [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_ByGenericType_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_ByGenericTypeAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter<int>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with name "value" with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_ByName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter with name "value" with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_ByTypeAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with name "value" with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_ByGenericTypeExactly_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_ByTypeExactly_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_ByGenericTypeExactlyAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly<int>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with name "value" with optional modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AsyncEnumerable_ByTypeExactlyAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with name "value" with optional modifier,
					             but at least one did not
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(AnotherClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveOptionalParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have an optional parameter,
					             but it only contained constructors with an optional parameter [
					               ThatConstructors.HaveOptionalParameter.ClassWithOptionalParameter(int value = 0),
					               ThatConstructors.HaveOptionalParameter.AnotherClassWithOptionalParameter(string text = "")
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNotAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveOptionalParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

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

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ClassWithOptionalParameter
		{
			public ClassWithOptionalParameter(int value = 0)
			{
			}
		}

		private class AnotherClassWithOptionalParameter
		{
			public AnotherClassWithOptionalParameter(string text = "")
			{
			}
		}

		private class ClassWithMixedOptionalParameter
		{
			public ClassWithMixedOptionalParameter(int required, int value = 0)
			{
			}
		}

		private class ClassWithoutModifiers
		{
			public ClassWithoutModifiers(int value)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
