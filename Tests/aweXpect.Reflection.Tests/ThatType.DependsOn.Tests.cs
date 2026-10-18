using System.Collections.Generic;
using System.Text;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Synthetic;

// These tests intentionally exercise the non-generic DependsOn(Type) overload (open/closed generic
// constructions and array targets), so the "prefer generic overload" hint does not apply here.
#pragma warning disable CA2263

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DependsOn
	{
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
		private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";

		public sealed class Tests
		{
			[Test]
			public async Task WhenAnyOfMultipleNamespacesMatches_ShouldSucceed()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Layer2Namespace, Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenArrayTargetIsUsed_ShouldMatchElementType()
			{
				// Dependencies are collected with array wrappers stripped, so an array target is
				// unwrapped symmetrically: typeof(TargetA[]) matches like typeof(TargetA).
				Type subject = typeof(WithArrayField);

				async Task Act()
				{
					await That(subject).DependsOn(typeof(TargetA[]));
					await That(subject).DependsOn(typeof(TargetA));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDebuggerStepThroughIsAuthoredOnAsyncMethod_ShouldCountAsDependency()
			{
				// In Debug builds the authored [DebuggerStepThrough] appears next to the compiler-emitted
				// one (two entries); in Release builds it is the only occurrence in an optimized assembly —
				// in both cases it is recognized as authored and counts.
				Type subject = typeof(WithAuthoredDebuggerStepThroughAsyncMethod);

				async Task Act()
				{
					await That(subject).DependsOn("System.Diagnostics");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateSignatureReferencesNamespace_ShouldSucceed()
			{
				// The authored Invoke signature (return type Layer1.TargetA, parameter Layer2.TargetB)
				// still counts as a dependency of the delegate type.
				Type subject = typeof(TargetProviderDelegate);

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace);
					await That(subject).DependsOn(Layer2Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumConstraintIsAuthored_ShouldCountAsDependency()
			{
				// Unlike the ValueType constraint `struct` compiles into, `where T : Enum` is authored.
				Type subject = typeof(WithEnumConstraint<>);

				async Task Act()
				{
					await That(subject).DependsOn("System");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExactGenericConstructionIsReferenced_ShouldSucceed()
			{
				Type subject = typeof(ViaGenericArgument);

				async Task Act()
				{
					await That(subject).DependsOn(typeof(List<TargetA>));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExcludingSubNamespaces_ShouldNotMatchSubNamespace()
			{
				Type subject = typeof(ViaSubNamespace);

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace).ExcludingSubNamespaces();
				}

				await That(Act).Throws<FailException>();
			}

			[Test]
			public async Task WhenExcludingSubNamespacesAfterBecause_ShouldNotMatchSubNamespace()
			{
				Type subject = typeof(ViaSubNamespace);

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace).Because("we need it").ExcludingSubNamespaces();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*, because we need it,*").AsWildcard()
					.Because("Because(…) must return the result, so that its options can still follow it");
			}

			[Test]
			public async Task WhenGenericTypeDefinitionIsTargeted_ShouldMatchAnyConstruction()
			{
				Type subject = typeof(ViaGenericArgument);

				async Task Act()
				{
					await That(subject).DependsOn(typeof(List<>));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(".Layer2")]
			[Arguments("Layer1..Layer2")]
			[Arguments("Layer2 ")]
			[Arguments(" Layer2")]
			[Arguments("Layer2\tSub")]
			public async Task WhenNamespaceCouldNeverMatch_ShouldThrowArgumentException(string @namespace)
			{
				// Leading dots, empty segments and whitespace cannot occur in a real namespace, so such an
				// input would make negative assertions pass vacuously, just like the trailing dot above.
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(@namespace);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The namespaces must not contain empty segments or whitespace (such a namespace could never match).");
			}

			[Test]
			public async Task WhenNamespaceEndsWithDot_ShouldThrowArgumentException()
			{
				// A trailing-dot namespace could never match and would make negative assertions pass
				// vacuously; only the excluded assembly prefixes use the trailing-dot convention.
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn($"{Layer1Namespace}.");
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The namespaces must not end with a dot (sub-namespaces are matched automatically).");
			}

			[Test]
			public async Task WhenNamespaceIsNull_ShouldThrowArgumentException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace, null!);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("namespaces").And
					.WithMessage("The 'namespaces' collection cannot contain <null>.").AsPrefix();
			}

			[Test]
			public async Task WhenNamespacesEnumerableIsNull_ShouldThrowArgumentNullException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn((IEnumerable<string>)null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("namespaces").And
					.WithMessage("The 'namespaces' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNamingFrameworkNamespace_ShouldSucceed()
			{
				Type subject = typeof(FrameworkConsumer);

				async Task Act()
				{
					await That(subject).DependsOn("System.Collections.Generic");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoNamespaceIsSpecified_ShouldThrowArgumentException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn();
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("namespaces").And
					.WithMessage("The 'namespaces' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenObsoleteIsAuthoredOnRequiredMembersConstructor_ShouldCountAsDependency()
			{
				// Only the compiler-emitted [Obsolete(marker, error: true)] paired with required members is
				// skipped; an authored [Obsolete] on the same constructor still counts.
				Type subject = typeof(WithRequiredPropertyAndAuthoredObsoleteConstructor);

				async Task Act()
				{
					await That(subject).DependsOn("System");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOnlyANestedTypeReferencesNamespace_ShouldFail()
			{
				// Nested types are separate types with their own dependency surface; the declaring type's
				// signature does not include what its nested types reference. The collection-based
				// assertions enumerate nested types as their own items and cover them there.
				Type subject = typeof(WithLayer1OnlyInNestedType);

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace);
				}

				await That(Act).Throws<FailException>();
			}

			[Test]
			public async Task WhenOnlyOtherConstructionOfGenericIsReferenced_ShouldFail()
			{
				// ViaGenericArgument references List<TargetA>; List<TargetB> is a different construction.
				Type subject = typeof(ViaGenericArgument);

				async Task Act()
				{
					await That(subject).DependsOn(typeof(List<TargetB>));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             depends on type List<TargetB>,
					             but it did not
					             """);
			}

			[Test]
			public async Task WhenSubNamespaceMatchesViaSubtree_ShouldSucceed()
			{
				Type subject = typeof(ViaSubNamespace);

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTargetingGlobalNamespaceWithEmptyString_ShouldSucceed()
			{
				// An empty string targets exactly the global namespace.
				Type subject = typeof(ReferencesGlobal);

				async Task Act()
				{
					await That(subject).DependsOn("");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeDoesNotDependOnNamespace_ShouldFail()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Layer2Namespace);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              depends on namespace "{Layer2Namespace}",
					              but it did not

					              Dependencies:
					              [
					                "{Layer1Namespace}"
					              ]
					              """);
			}

			[Test]
			public async Task WhenTypeDoesNotReferenceSpecificType_ShouldFail()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DependsOn<TargetB>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             depends on type TargetB,
					             but it did not
					             """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              depends on namespace "{Layer1Namespace}",
					              but it was <null>
					              """);
			}

			[Test]
			public async Task WhenTypeReferencesEnumOnlyViaAttributeArgument_ShouldSucceed()
			{
				// Layer2's TargetSeverity is referenced ONLY through the attribute's enum argument; the
				// attribute type itself lives in Layer1.
				Type subject = typeof(ViaEnumAttributeArgument);

				async Task Act()
				{
					await That(subject).DependsOn(Layer2Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(typeof(ViaBaseType))]
			[Arguments(typeof(ViaInterface))]
			[Arguments(typeof(ViaField))]
			[Arguments(typeof(ViaProperty))]
			[Arguments(typeof(ViaIndexer))]
			[Arguments(typeof(ViaEvent))]
			[Arguments(typeof(ViaMethodParameter))]
			[Arguments(typeof(ViaMethodReturn))]
			[Arguments(typeof(ViaGenericArgument))]
			[Arguments(typeof(ViaAttribute))]
			[Arguments(typeof(ViaGenericConstraint<>))]
			public async Task WhenTypeReferencesNamespaceInSignature_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeReferencesNamespaceOnlyViaParameterAttribute_ShouldSucceed()
			{
				// Layer1's TargetAttribute is referenced ONLY through the attribute on a method parameter.
				Type subject = typeof(ViaParameterAttribute);

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeReferencesNamespaceOnlyViaReturnValueAttribute_ShouldSucceed()
			{
				// Layer1's TargetAttribute is referenced ONLY through the [return: ...] attribute.
				Type subject = typeof(ViaReturnValueAttribute);

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeReferencesSpecificType_ShouldSucceed()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DependsOn<TargetA>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeReferencesTypeOnlyViaAttributeArgument_ShouldSucceed()
			{
				Type subject = typeof(ViaAttributeArgument);

				async Task Act()
				{
					await That(subject).DependsOn<TargetB>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenWidenedWithNamespaceEndingWithDot_ShouldThrowArgumentException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace).OrOn($"{Layer2Namespace}.");
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The namespaces must not end with a dot (sub-namespaces are matched automatically).");
			}

			[Test]
			public async Task WhenWidenedWithNamespaceThatCouldNeverMatch_ShouldThrowArgumentException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace).OrOn($".{Layer2Namespace}");
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The namespaces must not contain empty segments or whitespace (such a namespace could never match).");
			}

			[Test]
			public async Task WhenWidenedWithNullNamespace_ShouldThrowArgumentException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace).OrOn(Layer2Namespace, null!);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("namespaces").And
					.WithMessage("The 'namespaces' collection cannot contain <null>.").AsPrefix();
			}

			[Test]
			public async Task WhenWidenedWithNullNamespacesEnumerable_ShouldThrowArgumentNullException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Layer1Namespace).OrOn(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("namespaces").And
					.WithMessage("The 'namespaces' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldSucceed()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Layer2Namespace).OrOn(Layer1Namespace);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenWidenedWithOrOnType_ShouldSucceed()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DependsOn<TargetB>().OrOn<TargetA>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Test]
			public async Task WhenAdditionalTargetIsNull_ShouldThrowArgumentException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject)
						.DependsOn(Types.InNamespace(Layer1Namespace), null!, null!);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("additional").And
					.WithMessage("The 'additional' collection cannot contain <null>.").AsPrefix();
			}

			[Test]
			public async Task WhenAdditionalTargetsArrayIsNull_ShouldThrowArgumentNullException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Types.InNamespace(Layer1Namespace), null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("additional").And
					.WithMessage("The 'additional' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenAnyTargetCollectionMatches_ShouldSucceed()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Types.InNamespace(Layer2Namespace), Types.InNamespace(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenGenericTypeDefinitionIsInTargetCollection_ShouldMatchConstruction()
			{
				// ViaLayer1GenericConstruction references the constructed TargetGeneric<TargetB>; the scanned
				// target collection contains the open definition TargetGeneric<>.
				Type subject = typeof(ViaLayer1GenericConstruction);

				async Task Act()
				{
					await That(subject).DependsOn(In.Type(typeof(TargetGeneric<>)));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegated_ShouldListMatchingDependencies()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DependsOn(Types.InNamespace(Layer1Namespace)));
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
			public async Task WhenTargetCollectionContainsArrayType_ShouldMatchElementTypeDependency()
			{
				// Dependencies are stored element-stripped, so an array target matches like its element
				// type, mirroring the specific-type overload DependsOn(typeof(TargetA[])).
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DependsOn(In.Type(typeof(TargetA[])));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTargetCollectionContainsFrameworkTypes_ShouldMatchThem()
			{
				// A filtered collection is an explicit target, so framework types in it are checked normally.
				Type subject = typeof(FrameworkConsumer);

				async Task Act()
				{
					await That(subject).DependsOn(In.Type<StringBuilder>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTargetCollectionIsEmpty_ShouldFail()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DependsOn(Types.InNamespace("Non.Existent.Namespace"));
				}

				await That(Act).Throws<FailException>();
			}

			[Test]
			public async Task WhenTargetIsNull_ShouldThrowArgumentNullException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn((Filtered.Types)null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("target").And
					.WithMessage("The 'target' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenTypeDependsOnTypeInTargetCollection_ShouldSucceed()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DependsOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeDoesNotDependOnAnyTypeInTargetCollection_ShouldFail()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Types.InNamespace(Layer2Namespace));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              depends on types within namespace "{Layer2Namespace}" in all loaded assemblies,
					              but it did not
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).DependsOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              depends on types within namespace "{Layer1Namespace}" in all loaded assemblies,
					              but it was <null>
					              """);
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldSucceed()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Types.InNamespace(Layer2Namespace)).OrOn(Types.InNamespace(Layer1Namespace));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenWidenedWithoutTargets_ShouldThrowArgumentException()
			{
				Type subject = typeof(OnlyLayer1);

				async Task Act()
				{
					await That(subject).DependsOn(Types.InNamespace(Layer1Namespace)).OrOn();
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("targets").And
					.WithMessage("The 'targets' collection cannot be empty.").AsPrefix();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeDependsOnNamespace_ShouldFail()
			{
				Type subject = typeof(ViaField);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DependsOn(Layer1Namespace));
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
		}
	}
}
