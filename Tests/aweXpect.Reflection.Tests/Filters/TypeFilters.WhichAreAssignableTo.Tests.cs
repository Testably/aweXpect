using System;
using System.Collections.Generic;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreAssignableTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldApplyFilterForAssignableTo()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichAreAssignableTo>().Types().WhichAreAssignableTo<AssignableBase>())
						.AreAssignableTo<AssignableBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldApplyFilterForNotAssignableTo()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichAreAssignableTo>().Types()
							.WhichAreNotAssignableTo<AssignableBase>())
						.AreNotAssignableTo<AssignableBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldApplyFilterForAssignableFrom()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichAreAssignableTo>().Types()
							.WhichAreAssignableFrom<AssignableDerived>())
						.AreAssignableFrom<AssignableDerived>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldApplyFilterForNotAssignableFrom()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichAreAssignableTo>().Types()
							.WhichAreNotAssignableFrom<AssignableDerived>())
						.AreNotAssignableFrom<AssignableDerived>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldIncludeAssignableInformationInErrorMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichAreAssignableTo>().Types().WhichAreAssignableTo<AssignableBase>())
						.AreAbstract();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types which are assignable to TypeFilters.WhichAreAssignableTo.Tests.AssignableBase in assembly containing type TypeFilters.WhichAreAssignableTo
					             are all abstract,
					             but it contained non-abstract types [
					               TypeFilters.WhichAreAssignableTo.Tests.AssignableDerived
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeIsAnOpenGeneric_ShouldThrowArgumentException()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichAreAssignableTo>().Types()
							.WhichAreAssignableTo(typeof(IEnumerable<>)))
						.AreNotAbstract();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check assignability against must not be an open generic type definition, but it was IEnumerable<>. Use 'Implements' or 'InheritsFrom' for open generic type definitions.");
			}

			private abstract class AssignableBase;

			// ReSharper disable once UnusedType.Local
			private class AssignableDerived : AssignableBase;
		}
	}
}
