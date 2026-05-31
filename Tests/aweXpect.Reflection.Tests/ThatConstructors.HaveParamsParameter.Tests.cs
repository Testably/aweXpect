using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveParamsParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(AnotherClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have a params parameter,
					             but it contained constructors without a params parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task ByType_WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByType_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int[] with params modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByTypeAndName_WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]), "values");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByTypeExactly_WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByTypeExactlyAndName_WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]), "values");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]), "values");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenNotAllHaveParamsParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]), "values");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactly_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeExactly_WhenNotAllHaveParamsParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactlyAndName_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]), "values");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WithType_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithParamsParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveParamsParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]));
				}

				await That(Act).Throws<XunitException>();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(AnotherClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParamsParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have a params parameter,
					             but it only contained constructors with a params parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParamsParameter());
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
		private class ClassWithParamsParameter
		{
			public ClassWithParamsParameter(params int[] values)
			{
			}
		}

		private class AnotherClassWithParamsParameter
		{
			public AnotherClassWithParamsParameter(params string[] texts)
			{
			}
		}

		private class ClassWithoutModifiers
		{
			public ClassWithoutModifiers(int[] values)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
