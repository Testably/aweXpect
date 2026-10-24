using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class HasVersion
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenVersionDoesNotMatch_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				Version actualVersion = assembly.GetName().Version!;

				async Task Act()
				{
					await That(assembly).HasVersion(version => version.Major < 0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that assembly
					              has version matching version => version.Major < 0,
					              but it had version {actualVersion}
					              """);
			}

			[Test]
			public async Task WhenVersionMatches_ShouldSucceed()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).HasVersion(version => version.Major >= 0);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).HasVersion(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				InvalidOperationException exception = new("boom");

				async Task Act()
				{
					await That(assembly).HasVersion(_ => throw exception);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that assembly
					             has version matching _ => throw exception,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """).And
					.Whose(e => e.InnerException, inner => inner.IsSameAs(exception))
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).DoesNotComplyWith(it
						=> it.HasVersion(_ => throw new InvalidOperationException("boom")));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that assembly
					             does not have version matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("a predicate that throws answers neither the expectation nor its negation");
			}

			[Test]
			public async Task WhenVersionDoesNotMatch_ShouldSucceed()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).DoesNotComplyWith(it => it.HasVersion(version => version.Major < 0));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenVersionMatches_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				Version actualVersion = assembly.GetName().Version!;

				async Task Act()
				{
					await That(assembly).DoesNotComplyWith(it => it.HasVersion(version => version.Major >= 0));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that assembly
					              does not have version matching version => version.Major >= 0,
					              but it had version {actualVersion}
					              """);
			}
		}

		public sealed class WithoutVersionTests
		{
			[Test]
			public async Task WithComponent_WhenVersionIsNull_ShouldFail()
			{
				Assembly assembly = new VersionlessAssembly();

				async Task Act()
				{
					await That(assembly).HasVersion().WithMajor.GreaterThanOrEqualTo(0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that assembly
					             has major version greater than or equal to 0,
					             but it had no version
					             """);
			}

			[Test]
			public async Task WithoutComponent_WhenVersionIsNull_ShouldFail()
			{
				Assembly assembly = new VersionlessAssembly();

				async Task Act()
				{
					await That(assembly).HasVersion();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that assembly
					             has a version,
					             but it had no version
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenVersionIsNull_ShouldFail()
			{
				Assembly assembly = new VersionlessAssembly();

				async Task Act()
				{
					await That(assembly).HasVersion(version => version.Major >= 0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that assembly
					             has version matching version => version.Major >= 0,
					             but it had version <null>
					             """);
			}

			private sealed class VersionlessAssembly : Assembly
			{
				public override string FullName => nameof(VersionlessAssembly);

				public override AssemblyName GetName() => new("VersionlessAssembly");
			}
		}

		public sealed class ComponentTests
		{
			[Test]
			public async Task ShouldSupportAllComponents()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				Version version = assembly.GetName().Version!;

				async Task Act()
				{
					await That(assembly).HasVersion()
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
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(assembly).HasVersion()
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
			public async Task WhenBetweenIsNotSatisfied_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(assembly).HasVersion().WithMajor.Between(major + 1).And(major + 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that assembly
					              has major version between {major + 1} and {major + 2},
					              but it had major version {major}
					              """);
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
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;
				int expected = major + offset;

				async Task Act()
				{
					await (comparison switch
					{
						"GreaterThan" => That(assembly).HasVersion().WithMajor.GreaterThan(expected),
						"GreaterThanOrEqualTo" => That(assembly).HasVersion().WithMajor.GreaterThanOrEqualTo(expected),
						"LessThan" => That(assembly).HasVersion().WithMajor.LessThan(expected),
						"LessThanOrEqualTo" => That(assembly).HasVersion().WithMajor.LessThanOrEqualTo(expected),
						"EqualTo" => That(assembly).HasVersion().WithMajor.EqualTo(expected),
						_ => That(assembly).HasVersion().WithMajor.NotEqualTo(expected),
					});
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that assembly
					              has major version {wording} {expected},
					              but it had major version {major}
					              """);
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
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;
				int expected = major + offset;

				async Task Act()
				{
					await (comparison switch
					{
						"GreaterThan" => That(assembly).HasVersion().WithMajor.GreaterThan(expected),
						"GreaterThanOrEqualTo" => That(assembly).HasVersion().WithMajor.GreaterThanOrEqualTo(expected),
						"LessThan" => That(assembly).HasVersion().WithMajor.LessThan(expected),
						"LessThanOrEqualTo" => That(assembly).HasVersion().WithMajor.LessThanOrEqualTo(expected),
						"EqualTo" => That(assembly).HasVersion().WithMajor.EqualTo(expected),
						_ => That(assembly).HasVersion().WithMajor.NotEqualTo(expected),
					});
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that assembly
					              has major version {wording} {expected},
					              but it had major version {major}
					              """);
			}

			[Test]
			[Arguments("GreaterThan", -1)]
			[Arguments("GreaterThanOrEqualTo", 0)]
			[Arguments("LessThan", 1)]
			[Arguments("LessThanOrEqualTo", 0)]
			[Arguments("EqualTo", 0)]
			[Arguments("NotEqualTo", 1)]
			public async Task WhenComparisonIsSatisfied_ShouldSucceed(string comparison, int offset)
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					int expected = assembly.GetName().Version!.Major + offset;
					await (comparison switch
					{
						"GreaterThan" => That(assembly).HasVersion().WithMajor.GreaterThan(expected),
						"GreaterThanOrEqualTo" => That(assembly).HasVersion().WithMajor.GreaterThanOrEqualTo(expected),
						"LessThan" => That(assembly).HasVersion().WithMajor.LessThan(expected),
						"LessThanOrEqualTo" => That(assembly).HasVersion().WithMajor.LessThanOrEqualTo(expected),
						"EqualTo" => That(assembly).HasVersion().WithMajor.EqualTo(expected),
						_ => That(assembly).HasVersion().WithMajor.NotEqualTo(expected),
					});
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("major")]
			[Arguments("minor")]
			[Arguments("build")]
			[Arguments("revision")]
			public async Task WhenComponentDoesNotMatch_MessageShouldNameComponent(string name)
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				Version version = assembly.GetName().Version!;
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
						"major" => That(assembly).HasVersion().WithMajor.GreaterThan(actual),
						"minor" => That(assembly).HasVersion().WithMinor.GreaterThan(actual),
						"build" => That(assembly).HasVersion().WithBuild.GreaterThan(actual),
						_ => That(assembly).HasVersion().WithRevision.GreaterThan(actual),
					});
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that assembly
					              has {name} version greater than {actual},
					              but it had {name} version {actual}
					              """);
			}

			[Test]
			public async Task WhenComponentDoesNotMatch_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(assembly).HasVersion().WithMajor.LessThan(0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that assembly
					              has major version less than 0,
					              but it had major version {major}
					              """);
			}

			[Test]
			public async Task WhenComponentMatches_ShouldSucceed()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).HasVersion().WithMajor.GreaterThanOrEqualTo(0);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).HasVersion().WithMajor.NotBetween(2).And(1);
				}

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Test]
			public async Task WhenMultipleComponentsAreCombined_ShouldReportFailingComponent()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				Version version = assembly.GetName().Version!;

				async Task Act()
				{
					await That(assembly).HasVersion()
						.WithMajor.EqualTo(version.Major)
						.WithMinor.GreaterThan(version.Minor);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that assembly
					              has major version equal to {version.Major} and minor version greater than {version.Minor},
					              but it had minor version {version.Minor}
					              """);
			}

			[Test]
			public async Task WhenNotGreaterThanIsNotSatisfied_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(assembly).HasVersion().WithMajor.NotGreaterThan(major - 1);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that assembly
					              has major version not greater than {major - 1},
					              but it had major version {major}
					              """);
			}

			[Test]
			public async Task WithoutComponent_ShouldVerifyThatVersionExists()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).HasVersion();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedComponentTests
		{
			[Test]
			public async Task WhenComponentDoesNotMatch_ShouldSucceed()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).DoesNotComplyWith(it => it.HasVersion().WithMajor.LessThan(0));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenComponentMatches_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				Version version = assembly.GetName().Version!;

				async Task Act()
				{
					await That(assembly).DoesNotComplyWith(it => it.HasVersion().WithMajor.GreaterThanOrEqualTo(0));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that assembly
					              does not have major version greater than or equal to 0,
					              but it had version {version}
					              """);
			}

			[Test]
			public async Task WhenOrderedAgainstNull_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(assembly).DoesNotComplyWith(it => it.HasVersion().WithMajor.GreaterThan(null));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that assembly
					              does not have major version greater than <null>,
					              but it had major version {major}
					              """)
					.Because("an ordering against null answers neither the expectation nor its negation");
			}

			[Test]
			public async Task WhenVersionExists_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				Version version = assembly.GetName().Version!;

				async Task Act()
				{
					await That(assembly).DoesNotComplyWith(it => it.HasVersion());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that assembly
					              does not have a version,
					              but it had version {version}
					              """);
			}
		}
	}
}
