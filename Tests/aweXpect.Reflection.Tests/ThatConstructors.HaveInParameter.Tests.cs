using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveInParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveInParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
					typeof(AnotherClassWithInParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveInParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have an in parameter,
					             but it contained constructors without an in parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task ByType_WhenAllHaveInParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByType_WhenNotAllHaveInParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with in modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByTypeAndName_WhenAllHaveInParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByTypeExactly_WhenAllHaveInParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByTypeExactlyAndName_WhenAllHaveInParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenAllHaveInParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenNotAllHaveInParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactly_WhenAllHaveInParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeExactly_WhenNotAllHaveInParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactlyAndName_WhenAllHaveInParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveInParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WithType_WhenAllHaveInParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithInParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveInParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveInParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithInParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveInParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveInParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
					typeof(AnotherClassWithInParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveInParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have an in parameter,
					             but it only contained constructors with an in parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllHaveInParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithInParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveInParameter());
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
		private class ClassWithInParameter
		{
			public ClassWithInParameter(in int value)
			{
			}
		}

		private class AnotherClassWithInParameter
		{
			public AnotherClassWithInParameter(in string text)
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
