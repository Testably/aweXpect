using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<aweXpect.Analyzers.DiscardedOptionAnalyzer>;

namespace aweXpect.Analyzers.Tests;

public class DiscardedOptionAnalyzerTests
{
	[Test]
	[Arguments("Quantifier.Default.{|#0:AtLeast|}(2);", "AtLeast", "Quantifier")]
	[Arguments("quantifier.{|#0:AtLeast|}(2);", "AtLeast", "Quantifier")]
	[Arguments("holder.Value.{|#0:AtLeast|}(2);", "AtLeast", "Quantifier")]
	[Arguments("CollectionMatchOptions.Default.{|#0:InAnyOrder|}();", "InAnyOrder", "CollectionMatchOptions")]
	[Arguments("tolerance.{|#0:Within|}(0.1);", "Within", "NumberTolerance")]
	[Arguments("StringEqualityOptions.Default.{|#0:IgnoringCase|}();", "IgnoringCase", "StringEqualityOptions")]
	[Arguments("SignalerPredicateOptions<int>.Default.{|#0:WithPredicate|}(x => x > 0, \"x > 0\");", "WithPredicate",
		"SignalerPredicateOptions")]
	[Arguments("matchType.{|#0:Within|}(1);", "Within", "ObjectToleranceMatchType")]
	[Arguments("TriggerEventFilter.Default.{|#0:AddPredicate|}(_ => true, \"any\");", "AddPredicate",
		"TriggerEventFilter")]
	[Arguments("nullableQuantifier?.{|#0:AtLeast|}(2);", "AtLeast", "Quantifier")]
	[Arguments("nullableHolder?.Value.{|#0:AtLeast|}(2);", "AtLeast", "Quantifier")]
	public async Task WhenTheReturnedOptionsAreDiscarded_ShouldBeFlagged(string statement, string method,
		string type) => await Verifier
		.VerifyAnalyzerAsync(
			$$"""
			  #nullable enable
			  using aweXpect.Core;
			  using aweXpect.Options;

			  public class MyClass
			  {
			      public void MyMethod(Quantifier quantifier, OptionsHolder<Quantifier> holder,
			          NumberTolerance<double> tolerance, ObjectToleranceMatchType<int, int> matchType,
			          Quantifier? nullableQuantifier, OptionsHolder<Quantifier>? nullableHolder)
			      {
			          {{statement}}
			      }
			  }
			  """,
			Verifier.Diagnostic(Rules.DiscardedOptionRule)
				.WithLocation(0)
				.WithArguments(method, type)
		);

	[Test]
	[Arguments("quantifier = quantifier.AtLeast(2);")]
	[Arguments("holder.Value = holder.Value.AtLeast(2);")]
	[Arguments("_ = quantifier.AtLeast(2);")]
	[Arguments("holder.AtLeast(2);")]
	[Arguments("holder?.AtLeast(2);")]
	[Arguments("_ = holder?.Value.AtLeast(2);")]
	[Arguments("Consume(quantifier.AtLeast(2));")]
	[Arguments("quantifier.ToString();")]
	[Arguments("new MyOptions().Twice();")]
	[Arguments("void Act() => quantifier.AtLeast(-1);")]
	[Arguments("System.Action act = () => quantifier.AtLeast(-1);")]
	public async Task WhenTheReturnedOptionsAreUsed_ShouldNotBeFlagged(string statement) => await Verifier
		.VerifyAnalyzerAsync(
			$$"""
			  using aweXpect.Core;
			  using aweXpect.Options;

			  public readonly struct MyOptions
			  {
			      public MyOptions Twice() => this;
			  }

			  public class MyClass
			  {
			      public void MyMethod(Quantifier quantifier, OptionsHolder<Quantifier> holder)
			      {
			          {{statement}}
			      }

			      private static void Consume(Quantifier value)
			      {
			      }
			  }
			  """
		);
}
