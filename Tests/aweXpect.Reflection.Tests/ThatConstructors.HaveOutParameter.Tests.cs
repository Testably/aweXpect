using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveOutParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveOutParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
					typeof(AnotherClassWithOutParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveOutParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have an out parameter,
					             but it contained constructors without an out parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task ByType_WhenAllHaveOutParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByType_WhenNotAllHaveOutParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with out modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task ByTypeAndName_WhenAllHaveOutParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByTypeExactly_WhenAllHaveOutParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByTypeExactlyAndName_WhenAllHaveOutParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenAllHaveOutParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenNotAllHaveOutParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactly_WhenAllHaveOutParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeExactly_WhenNotAllHaveOutParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactlyAndName_WhenAllHaveOutParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOutParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WithType_WhenAllHaveOutParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithOutParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOutParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveOutParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithOutParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOutParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveOutParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
					typeof(AnotherClassWithOutParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveOutParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have an out parameter,
					             but it only contained constructors with an out parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllHaveOutParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOutParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveOutParameter());
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
		private class ClassWithOutParameter
		{
			public ClassWithOutParameter(out int value)
			{
				value = 0;
			}
		}

		private class AnotherClassWithOutParameter
		{
			public AnotherClassWithOutParameter(out string text)
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
