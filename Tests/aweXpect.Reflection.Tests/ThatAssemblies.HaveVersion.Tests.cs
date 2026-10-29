using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed partial class HaveVersion
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllVersionsMatch_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion(version => version.Major >= 0);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenVersionDoesNotMatch_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion(version => version.Major < 0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have version matching version => version.Major < 0,
					             but none of 1 did*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have version matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               (… and maybe more)
					             ]
					             """).AsWildcard()
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.HaveVersion(_ => throw new InvalidOperationException("boom")));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have version matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               (… and maybe more)
					             ]
					             """).AsWildcard()
					.Because("a predicate that throws answers neither the expectation nor its negation");
			}

			[Test]
			public async Task WhenAllVersionsMatch_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion(version => version.Major >= 0));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have version matching version => version.Major >= 0,
					             but all 1 did*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenVersionDoesNotMatch_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion(version => version.Major < 0));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ComponentTests
		{
			[Test]
			public async Task ShouldSupportAllComponents()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();
				Version version = typeof(PublicAbstractClass).Assembly.GetName().Version!;

				async Task Act()
				{
					await That(subject).HaveVersion()
						.WithMajor.EqualTo(version.Major)
						.WithMinor.EqualTo(version.Minor)
						.WithBuild.EqualTo(version.Build)
						.WithRevision.EqualTo(version.Revision);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllComparisonsAreSatisfied_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();
				int major = typeof(PublicAbstractClass).Assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(subject).HaveVersion()
						.WithMajor.GreaterThan(major - 1)
						.WithMajor.GreaterThanOrEqualTo(major)
						.WithMajor.LessThan(major + 1)
						.WithMajor.LessThanOrEqualTo(major)
						.WithMajor.EqualTo(major)
						.WithMajor.NotEqualTo(major + 1);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllComponentsMatch_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion().WithMajor.GreaterThanOrEqualTo(0);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBetweenIsNotSatisfied_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();
				int major = typeof(PublicAbstractClass).Assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(subject).HaveVersion().WithMajor.Between(major + 1).And(major + 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that in assembly containing type PublicAbstractClass
					              all have major version between {major + 1} and {major + 2},
					              but none of 1 did

					              Not matching items:
					              [
					                aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					              ]

					              Collection:
					              [
					                aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					              ]
					              """).AsWildcard();
			}

			[Test]
			[Arguments("GreaterThan", 0, "greater than")]
			[Arguments("GreaterThanOrEqualTo", 1, "greater than or equal to")]
			[Arguments("LessThan", 0, "less than")]
			[Arguments("LessThanOrEqualTo", -1, "less than or equal to")]
			[Arguments("EqualTo", 1, "equal to")]
			[Arguments("NotEqualTo", 0, "not equal to")]
			public async Task WhenComparisonFails_MessageShouldDescribeComparison(
				string comparison, int offset, string wording)
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();
				int major = typeof(PublicAbstractClass).Assembly.GetName().Version!.Major;
				int expected = major + offset;

				async Task Act()
				{
					await (comparison switch
					{
						"GreaterThan" => That(subject).HaveVersion().WithMajor.GreaterThan(expected),
						"GreaterThanOrEqualTo" => That(subject).HaveVersion().WithMajor.GreaterThanOrEqualTo(expected),
						"LessThan" => That(subject).HaveVersion().WithMajor.LessThan(expected),
						"LessThanOrEqualTo" => That(subject).HaveVersion().WithMajor.LessThanOrEqualTo(expected),
						"EqualTo" => That(subject).HaveVersion().WithMajor.EqualTo(expected),
						_ => That(subject).HaveVersion().WithMajor.NotEqualTo(expected),
					});
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that in assembly containing type PublicAbstractClass
					              all have major version {wording} {expected},
					              but none of 1 did*
					              """).AsWildcard();
			}

			[Test]
			[Arguments("GreaterThan", 0, "greater than")]
			[Arguments("GreaterThanOrEqualTo", 1, "greater than or equal to")]
			[Arguments("LessThan", 0, "less than")]
			[Arguments("LessThanOrEqualTo", -1, "less than or equal to")]
			[Arguments("EqualTo", 1, "equal to")]
			[Arguments("NotEqualTo", 0, "not equal to")]
			public async Task WhenComparisonIsNotSatisfied_ShouldFail(string comparison, int offset, string wording)
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int expected = assembly.GetName().Version!.Major + offset;

				async Task Act()
				{
					await (comparison switch
					{
						"GreaterThan" => That(subject).HaveVersion().WithMajor.GreaterThan(expected),
						"GreaterThanOrEqualTo" => That(subject).HaveVersion().WithMajor.GreaterThanOrEqualTo(expected),
						"LessThan" => That(subject).HaveVersion().WithMajor.LessThan(expected),
						"LessThanOrEqualTo" => That(subject).HaveVersion().WithMajor.LessThanOrEqualTo(expected),
						"EqualTo" => That(subject).HaveVersion().WithMajor.EqualTo(expected),
						_ => That(subject).HaveVersion().WithMajor.NotEqualTo(expected),
					});
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that in assembly containing type PublicAbstractClass
					              all have major version {wording} {expected},
					              but none of 1 did

					              Not matching items:
					              [
					                {assembly.FullName}
					              ]

					              Collection:
					              [
					                {assembly.FullName}
					              ]
					              """);
			}

			[Test]
			[Arguments("major")]
			[Arguments("minor")]
			[Arguments("build")]
			[Arguments("revision")]
			public async Task WhenComponentDoesNotMatch_MessageShouldNameComponent(string name)
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();
				Version version = typeof(PublicAbstractClass).Assembly.GetName().Version!;
				int actual = name switch
				{
					"major" => version.Major,
					"minor" => version.Minor,
					"build" => version.Build,
					_ => version.Revision,
				};

				async Task Act()
				{
					await (name switch
					{
						"major" => That(subject).HaveVersion().WithMajor.GreaterThan(actual),
						"minor" => That(subject).HaveVersion().WithMinor.GreaterThan(actual),
						"build" => That(subject).HaveVersion().WithBuild.GreaterThan(actual),
						_ => That(subject).HaveVersion().WithRevision.GreaterThan(actual),
					});
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that in assembly containing type PublicAbstractClass
					              all have {name} version greater than {actual},
					              but none of 1 did*
					              """).AsWildcard();
			}

			[Test]
			public async Task WhenComponentDoesNotMatch_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion().WithMajor.LessThan(0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have major version less than 0,
					             but none of 1 did*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenComponentDoesNotMatch_ShouldListNonMatchingAssembly()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion().WithMajor.LessThan(0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have major version less than 0,
					             but none of 1 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMultipleComponentsAreCombined_ShouldCombineExpectations()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();
				Version version = typeof(PublicAbstractClass).Assembly.GetName().Version!;

				async Task Act()
				{
					await That(subject).HaveVersion()
						.WithMajor.EqualTo(version.Major)
						.WithMinor.GreaterThan(version.Minor);
				}

				await That(Act).Throws<FailException>()
					.WithMessage(
						$"*all have major version equal to {version.Major} and minor version greater than {version.Minor}*but none of 1 did*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenNegatedAndAllComponentsMatch_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion().WithMajor.GreaterThanOrEqualTo(0));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have major version greater than or equal to 0,
					             but all 1 did*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNegatedAndAllVersionsExist_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*not all have a version*").AsWildcard();
			}

			[Test]
			public async Task WhenNegatedAndComponentDoesNotMatch_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion().WithMajor.LessThan(0));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegatedAndComponentMatches_ShouldListMatchingAssembly()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion().WithMajor.GreaterThanOrEqualTo(0));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have major version greater than or equal to 0,
					             but all 1 did

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNegatedAndOrderedAgainstNull_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion().WithMajor.GreaterThan(null));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have major version greater than <null>,
					             but for the item at index 0, it had major version *

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               (… and maybe more)
					             ]
					             """).AsWildcard()
					.Because("an ordering against null answers neither the expectation nor its negation");
			}

			[Test]
			public async Task WhenNotGreaterThanIsNotSatisfied_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();
				int major = typeof(PublicAbstractClass).Assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(subject).HaveVersion().WithMajor.NotGreaterThan(major - 1);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that in assembly containing type PublicAbstractClass
					              all have major version not greater than {major - 1},
					              but none of 1 did

					              Not matching items:
					              [
					                aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					              ]

					              Collection:
					              [
					                aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					              ]
					              """).AsWildcard();
			}

			[Test]
			public async Task WithoutComponent_ShouldVerifyThatAllVersionsExist()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithoutVersionTests
		{
			[Test]
			public async Task WhenAssemblyHasNoVersion_ShouldFail()
			{
				Filtered.Assemblies subject = In.Assemblies(new VersionlessAssembly());

				async Task Act()
				{
					await That(subject).HaveVersion();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in the assemblies [VersionlessAssembly]
					             all have a version,
					             but none of 1 did

					             Not matching items:
					             [
					               VersionlessAssembly
					             ]

					             Collection:
					             [
					               VersionlessAssembly
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAssemblyHasNoVersion_WithComponent_ShouldFail()
			{
				Filtered.Assemblies subject = In.Assemblies(new VersionlessAssembly());

				async Task Act()
				{
					await That(subject).HaveVersion().WithMajor.GreaterThanOrEqualTo(0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in the assemblies [VersionlessAssembly]
					             all have major version greater than or equal to 0,
					             but none of 1 did

					             Not matching items:
					             [
					               VersionlessAssembly
					             ]

					             Collection:
					             [
					               VersionlessAssembly
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNegatedAndAssemblyHasNoVersion_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.Assemblies(new VersionlessAssembly());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion());
				}

				await That(Act).DoesNotThrow();
			}

			private sealed class VersionlessAssembly : Assembly
			{
				public override string FullName => nameof(VersionlessAssembly);

				public override AssemblyName GetName() => new("VersionlessAssembly");
			}
		}
	}
}
