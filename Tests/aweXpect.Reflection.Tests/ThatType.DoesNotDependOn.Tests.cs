using System.Collections.Generic;
using aweXpect.Customization;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Synthetic;

// These tests intentionally exercise the non-generic DoesNotDependOn(Type) overload (closed generic
// constructions and array targets), so the "prefer generic overload" hint does not apply here.
#pragma warning disable CA2263

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DoesNotDependOn
	{
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
		private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";

		public sealed class Tests
		{
			[Test]
			public async Task WhenArrayTargetIsUsed_ShouldMatchElementType()
			{
				// The array wrapper is unwrapped on both sides: WithArrayField references TargetA[],
				// so it depends on typeof(TargetA[]) just like on typeof(TargetA).
				Type subject = typeof(WithArrayField);

				async Task Act()
				{
					await That(subject).DoesNotDependOn(typeof(TargetA[]));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not depend on type TargetA[],
					             but it did

					             Matching dependencies:
					             [
					               TargetA
					             ]
					             """);
			}

			[Test]
			public async Task WhenAttributeTypeIsExcludedViaCustomization_ShouldNotCountAsDependency()
			{
				// The built-in compiler-emitted attribute set can be extended via customization, e.g. for a
				// marker attribute of a future compiler version; outside the scope the attribute counts again.
				Type subject = typeof(ViaAttribute);

				using (Customize.aweXpect.Reflection().ExcludedAttributeTypes
					       .Set([typeof(TargetAttribute).FullName!,]))
				{
					async Task Act()
					{
						await That(subject).DoesNotDependOn(Layer1Namespace);
					}

					await That(Act).DoesNotThrow();
				}

				async Task ActOutsideScope()
				{
					await That(subject).DependsOn(Layer1Namespace);
				}

				await That(ActOutsideScope).DoesNotThrow();
			}

			[Test]
			public async Task WhenCompilerEmitsAttributesForAsyncMethod_ShouldNotCountAsDependency()
			{
				// An async method makes the compiler emit [AsyncStateMachine(typeof(<M>d__0))] and
				// [DebuggerStepThrough]; neither attribute is authored. (The authored `void` return type
				// still counts as a reference to "System", so the attribute namespaces are asserted.)
				Type subject = typeof(WithAsyncMethod);

				async Task Act()
				{
					await That(subject).DoesNotDependOn("System.Runtime.CompilerServices");
					await That(subject).DoesNotDependOn("System.Diagnostics");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCompilerEmitsAttributesForIteratorMethod_ShouldNotCountAsDependency()
			{
				// An iterator method makes the compiler emit [IteratorStateMachine(typeof(<M>d__0))];
				// the authored IEnumerable<int> return type lives in System.Collections.Generic, not here.
				Type subject = typeof(WithIteratorMethod);

				async Task Act()
				{
					await That(subject).DoesNotDependOn("System.Runtime.CompilerServices");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCompilerEmitsAttributesForRequiredMember_ShouldNotCountAsDependency()
			{
				// `required` makes the compiler emit [RequiredMember] on the type and property and
				// [Obsolete] + [CompilerFeatureRequired] on the constructor; none of these are authored.
				Type subject = typeof(WithRequiredProperty);

				async Task Act()
				{
					await That(subject).DoesNotDependOn("System");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCompilerEmitsParamArrayForParamsParameter_ShouldNotCountAsDependency()
			{
				// The `params` keyword compiles into [ParamArray] on the parameter, which the author can
				// never write directly (CS0674).
				Type subject = typeof(WithParamsArrayOfOwnType);

				async Task Act()
				{
					await That(subject).DoesNotDependOn<ParamArrayAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenCompilerEmitsPreserveBaseOverridesForCovariantReturn_ShouldNotCountAsDependency()
			{
				// A covariant-return override makes the compiler emit [PreserveBaseOverrides]; the
				// attribute is not authored.
				Type subject = typeof(CovariantReturnDerived);

				async Task Act()
				{
					await That(subject).DoesNotDependOn("System.Runtime.CompilerServices");
				}

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			public async Task WhenCompilerEmitsStateMachineForAsyncMethod_ShouldNotCountAsDependency()
			{
				// The nested <MethodAsync>d__0 state machine lives in the type's own namespace; the
				// typeof(...) argument of [AsyncStateMachine] must not surface it as a dependency.
				Type subject = typeof(WithAsyncMethod);

				async Task Act()
				{
					await That(subject)
						.DoesNotDependOn("aweXpect.Reflection.Tests.TestHelpers.Dependencies.Synthetic");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateInfrastructureIsRuntimeSupplied_ShouldNotCountAsDependency()
			{
				// The MulticastDelegate base, the (object, IntPtr) constructor and BeginInvoke/EndInvoke
				// are runtime-supplied; only the Invoke signature of a delegate is authored.
				Type subject = typeof(TargetProviderDelegate);

				async Task Act()
				{
					await That(subject).DoesNotDependOn("System");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenInterfaceIsImplementedOnlyByBaseType_ShouldNotCountAsDependency()
			{
				// GetInterfaces() returns the transitive closure: DerivedWithoutOwnReferences inherits
				// ITargetInterface from its base type without writing the reference itself.
				Type subject = typeof(DerivedWithoutOwnReferences);

				async Task Act()
				{
					await That(subject).DoesNotDependOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNamingFrameworkNamespaceThatIsReferenced_ShouldFail()
			{
				Type subject = typeof(FrameworkConsumer);

				async Task Act()
				{
					await That(subject).DoesNotDependOn("System.Collections.Generic");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not depend on namespace "System.Collections.Generic",
					             but it did

					             Matching dependencies:
					             [
					               "System.Collections.Generic"
					             ]
					             """);
			}

			[Test]
			public async Task WhenNoNamespaceIsSpecified_ShouldThrowArgumentException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DoesNotDependOn();
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("namespaces").And
					.WithMessage("The 'namespaces' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenOnlyOtherConstructionOfGenericIsReferenced_ShouldSucceed()
			{
				// ViaGenericArgument references List<TargetA>; List<TargetB> is a different construction
				// and must not be reported as a dependency.
				Type subject = typeof(ViaGenericArgument);

				async Task Act()
				{
					await That(subject).DoesNotDependOn(typeof(List<TargetB>));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOnlySystemReferenceIsImplicitBaseAndNullableAttributes_ShouldSucceed()
			{
				// OnlyLayer1's only authored dependency is Layer1.TargetA; the implicit object base type and the
				// compiler-emitted nullable attributes must not count as a dependency on "System".
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DoesNotDependOn("System");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(typeof(WithOutParameter))]
			[Arguments(typeof(WithInParameter))]
			[Arguments(typeof(WithRefReadonlyParameter))]
			[Arguments(typeof(WithOptionalParameter))]
			public async Task WhenParameterModifiersSurfaceAsPseudoAttributes_ShouldNotCountAsDependency(Type subject)
			{
				// `out`, `in`, `ref readonly` and optional parameters compile into metadata flags which
				// reflection surfaces as the [Out]/[In]/[Optional] pseudo-attributes; `ref readonly`
				// additionally emits [RequiresLocation]. None of them are authored.
				async Task Act()
				{
					await That(subject).DoesNotDependOn("System.Runtime.InteropServices");
					await That(subject).DoesNotDependOn("System.Runtime.CompilerServices");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenRecordSynthesizesEquatableInterface_ShouldNotCountAsDependency()
			{
				// The compiler synthesizes IEquatable<T> for records; the author never wrote that reference.
				Type subject = typeof(RecordWithLayer1Target);

				async Task Act()
				{
					await That(subject).DoesNotDependOn("System");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(typeof(WithStructConstraint<>))]
			[Arguments(typeof(WithUnmanagedConstraint<>))]
			public async Task WhenStructConstraintCompilesIntoValueType_ShouldNotCountAsDependency(Type subject)
			{
				// `where T : struct` / `where T : unmanaged` compile into a System.ValueType constraint in
				// metadata, which the author can never write directly (CS0702).
				async Task Act()
				{
					await That(subject).DoesNotDependOn("System");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeDependsOnNamespace_ShouldFail()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DoesNotDependOn(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not depend on namespace "{Layer1Namespace}",
					              but it did

					              Matching dependencies:
					              [
					                "{Layer1Namespace}"
					              ]
					              """);
			}

			[Test]
			public async Task WhenTypeDoesNotDependOnNamespace_ShouldSucceed()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DoesNotDependOn(Layer2Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeDoesNotReferenceSpecificType_ShouldSucceed()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DoesNotDependOn<TargetB>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeReferencesSpecificType_ShouldFail()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DoesNotDependOn<TargetA>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not depend on type TargetA,
					             but it did

					             Matching dependencies:
					             [
					               TargetA
					             ]
					             """);
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldFailIfAnyMatches()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).DoesNotDependOn(Layer2Namespace).OrOn(Layer1Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not depend on namespace "{Layer2Namespace}" or "{Layer1Namespace}",
					              but it did

					              Matching dependencies:
					              [
					                "{Layer1Namespace}",
					                "{Layer2Namespace}"
					              ]
					              """);
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Test]
			public async Task WhenTargetCollectionIsEmpty_ShouldSucceedTrivially()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DoesNotDependOn(Types.InNamespace("Non.Existent.Namespace"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeDependsOnTypeInTargetCollection_ShouldFail()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DoesNotDependOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not depend on types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it did

					              Matching dependencies:
					              [
					                TargetA
					              ]
					              """);
			}

			[Test]
			public async Task WhenTypeDoesNotDependOnAnyTypeInTargetCollection_ShouldSucceed()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DoesNotDependOn(Types.InNamespace(Layer2Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldFailIfAnyMatches()
			{
				Type subject = typeof(Layer1AndLayer2);

				async Task Act()
				{
					await That(subject).DoesNotDependOn(Types.InNamespace(Layer2Namespace))
						.OrOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not depend on types within namespace "{Layer2Namespace}" in all loaded assemblies or types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it did

					              Matching dependencies:
					              [
					                TargetA,
					                TargetB
					              ]
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeDependsOnNamespace_ShouldSucceed()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotDependOn(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
