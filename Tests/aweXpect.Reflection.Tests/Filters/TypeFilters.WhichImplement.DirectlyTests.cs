namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed partial class WhichImplement
	{
		public sealed class DirectlyTests
		{
			public sealed class Tests
			{
				[Test]
				public async Task ShouldApplyFilterForInterface()
				{
					async Task Act()
					{
						await That(In.AssemblyContaining<DirectlyTests>().Types()
								.WhichImplement<IFoo>().Directly())
							.Implement<IFoo>().Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ShouldIncludeImplementDirectlyInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AssemblyContaining<DirectlyTests>().Types()
								.WhichImplement<IFoo>().Directly())
							.AreAbstract();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that types which directly implement TypeFilters.WhichImplement.DirectlyTests.Tests.IFoo in assembly containing type TypeFilters.WhichImplement.DirectlyTests
						             all are abstract,
						             but none of 1 were

						             Not matching items:
						             [
						               TypeFilters.WhichImplement.DirectlyTests.Tests.FooImpl
						             ]

						             Collection:
						             [
						               TypeFilters.WhichImplement.DirectlyTests.Tests.FooImpl
						             ]
						             """);
				}

				[Test]
				public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
				{
					async Task Act()
					{
						await That(In.AssemblyContaining<DirectlyTests>().Types()
								.WhichImplement<FooImpl>().Directly())
							.AreNotAbstract();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check implementation of must be an interface, but it was TypeFilters.WhichImplement.DirectlyTests.Tests.FooImpl. Use 'InheritsFrom' to check for base-class inheritance.");
				}

				private interface IFoo;

				private class FooImpl : IFoo;

				private class FooImplDerived : FooImpl;
			}
		}

	}

	public sealed partial class WhichDoNotImplement
	{
		public sealed class DirectlyTests
		{
			public sealed class Tests
			{
				[Test]
				public async Task ShouldApplyFilterForInterface()
				{
					async Task Act()
					{
						await That(In.AssemblyContaining<DirectlyTests>().Types()
								.WhichDoNotImplement<IFoo>().Directly())
							.DoNotImplement<IFoo>().Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ShouldIncludeDoNotImplementDirectlyInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AssemblyContaining<DirectlyTests>().Types()
								.WhichDoNotImplement<IFoo>().Directly())
							.DoNotImplement<IFoo>();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that types which do not directly implement TypeFilters.WhichDoNotImplement.DirectlyTests.Tests.IFoo in assembly containing type TypeFilters.WhichDoNotImplement.DirectlyTests
						             none implement TypeFilters.WhichDoNotImplement.DirectlyTests.Tests.IFoo,
						             but 1 of * did

						             Matching items:
						             [
						               TypeFilters.WhichDoNotImplement.DirectlyTests.Tests.FooImplDerived
						             ]

						             Collection:
						             [
						               *
						             ]
						             """).AsWildcard();
				}

				[Test]
				public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
				{
					async Task Act()
					{
						await That(In.AssemblyContaining<DirectlyTests>().Types()
								.WhichDoNotImplement<FooImpl>().Directly())
							.AreNotAbstract();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check implementation of must be an interface, but it was TypeFilters.WhichDoNotImplement.DirectlyTests.Tests.FooImpl. Use 'InheritsFrom' to check for base-class inheritance.");
				}

				private interface IFoo;

				private class FooImpl : IFoo;

				private class FooImplDerived : FooImpl;
			}
		}
	}
}
