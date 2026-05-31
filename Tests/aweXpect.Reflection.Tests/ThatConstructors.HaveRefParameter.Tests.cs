using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveRefParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveRefParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
					typeof(AnotherClassWithRefParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveRefParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have a ref parameter,
					             but it contained constructors without a ref parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task ByType_WhenAllHaveRefParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByType_WhenNotAllHaveRefParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with ref modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByTypeAndName_WhenAllHaveRefParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByTypeExactly_WhenAllHaveRefParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByTypeExactlyAndName_WhenAllHaveRefParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenNotAllHaveRefParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactly_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeExactly_WhenNotAllHaveRefParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactlyAndName_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WithType_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithRefParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveRefParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveRefParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithRefParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveRefParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveRefParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
					typeof(AnotherClassWithRefParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveRefParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have a ref parameter,
					             but it only contained constructors with a ref parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllHaveRefParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithRefParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveRefParameter());
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
		private class ClassWithRefParameter
		{
			public ClassWithRefParameter(ref int value)
			{
				value = 0;
			}
		}

		private class AnotherClassWithRefParameter
		{
			public AnotherClassWithRefParameter(ref string text)
			{
				text = string.Empty;
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
