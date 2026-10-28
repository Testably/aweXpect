namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed partial class WhichInheritFrom
	{
		public sealed class DirectlyTests
		{
			public sealed class Tests
			{
				[Test]
				public async Task ShouldApplyFilterForBaseType()
				{
					async Task Act()
					{
						await That(In.AssemblyContaining<DirectlyTests>().Types()
								.WhichInheritFrom<FooBase>().Directly())
							.InheritFrom<FooBase>().Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ShouldIncludeInheritDirectlyInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AssemblyContaining<DirectlyTests>().Types()
								.WhichInheritFrom<FooBase>().Directly())
							.AreAbstract();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that types which inherit directly from TypeFilters.WhichInheritFrom.DirectlyTests.Tests.FooBase in assembly containing type TypeFilters.WhichInheritFrom.DirectlyTests
						             all are abstract,
						             but none of 1 were

						             Not matching items:
						             [
						               TypeFilters.WhichInheritFrom.DirectlyTests.Tests.FooDerived
						             ]

						             Collection:
						             [
						               TypeFilters.WhichInheritFrom.DirectlyTests.Tests.FooDerived
						             ]
						             """);
				}

				[Test]
				public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
				{
					async Task Act()
					{
						await That(In.AssemblyContaining<DirectlyTests>().Types()
								.WhichInheritFrom<IFoo>().Directly())
							.AreNotAbstract();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check inheritance from must be a class, but it was the interface TypeFilters.WhichInheritFrom.DirectlyTests.Tests.IFoo. Use 'Implements' to check for interface implementations.");
				}

				private interface IFoo;

				private class FooBase;

				private class FooDerived : FooBase;

				private class FooGrandChild : FooDerived;
			}
		}

	}

	public sealed partial class WhichDoNotInheritFrom
	{
		public sealed class DirectlyTests
		{
			public sealed class Tests
			{
				[Test]
				public async Task ShouldApplyFilterForBaseType()
				{
					async Task Act()
					{
						await That(In.AssemblyContaining<DirectlyTests>().Types()
								.WhichDoNotInheritFrom<FooBase>().Directly())
							.DoNotInheritFrom<FooBase>().Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ShouldIncludeDoNotInheritDirectlyInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AssemblyContaining<DirectlyTests>().Types()
								.WhichDoNotInheritFrom<FooBase>().Directly())
							.DoNotInheritFrom<FooBase>();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that types which do not inherit directly from TypeFilters.WhichDoNotInheritFrom.DirectlyTests.Tests.FooBase in assembly containing type TypeFilters.WhichDoNotInheritFrom.DirectlyTests
						             none inherit from TypeFilters.WhichDoNotInheritFrom.DirectlyTests.Tests.FooBase,
						             but 1 of * did

						             Matching items:
						             [
						               TypeFilters.WhichDoNotInheritFrom.DirectlyTests.Tests.FooGrandChild
						             ]

						             Collection:
						             [
						               *
						             ]
						             """).AsWildcard();
				}

				[Test]
				public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
				{
					async Task Act()
					{
						await That(In.AssemblyContaining<DirectlyTests>().Types()
								.WhichDoNotInheritFrom<IFoo>().Directly())
							.AreNotAbstract();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check inheritance from must be a class, but it was the interface TypeFilters.WhichDoNotInheritFrom.DirectlyTests.Tests.IFoo. Use 'Implements' to check for interface implementations.");
				}

				private interface IFoo;

				private class FooBase;

				private class FooDerived : FooBase;

				private class FooGrandChild : FooDerived;
			}
		}
	}
}
