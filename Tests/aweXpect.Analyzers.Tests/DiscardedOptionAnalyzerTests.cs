using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<aweXpect.Analyzers.DiscardedOptionAnalyzer>;

namespace aweXpect.Analyzers.Tests;

public class DiscardedOptionAnalyzerTests
{
	private const string ExtensionOptions = """
	                                        namespace aweXpect.Json
	                                        {
	                                            [aweXpect.Core.ImmutableOptions]
	                                            public readonly struct JsonOptionsStruct
	                                            {
	                                                public JsonOptionsStruct IgnoringX() => this;
	                                            }
	                                        }

	                                        namespace aweXpect.Xml
	                                        {
	                                            [aweXpect.Core.ImmutableOptions]
	                                            public sealed class XmlOptions
	                                            {
	                                            }

	                                            public sealed class UnmarkedOptions
	                                            {
	                                            }

	                                            public static class XmlOptionsExtensions
	                                            {
	                                                public static XmlOptions IgnoringX(this XmlOptions options) => options;

	                                                public static UnmarkedOptions IgnoringX(this UnmarkedOptions options)
	                                                    => options;

	                                                public static T Log<T>(this T value) => value;
	                                            }
	                                        }

	                                        namespace aweXpect.Options
	                                        {
	                                            public readonly struct UnmarkedStruct
	                                            {
	                                                public UnmarkedStruct IgnoringX() => this;
	                                            }
	                                        }

	                                        namespace Other.Core
	                                        {
	                                            public sealed class ImmutableOptionsAttribute : System.Attribute
	                                            {
	                                            }
	                                        }

	                                        namespace Other
	                                        {
	                                            [aweXpect.Core.ImmutableOptions]
	                                            public readonly struct OtherOptions
	                                            {
	                                                public OtherOptions IgnoringX() => this;
	                                            }

	                                            public static class Outer
	                                            {
	                                                [aweXpect.Core.ImmutableOptions]
	                                                public readonly struct Nested
	                                                {
	                                                    public Nested IgnoringX() => this;
	                                                }
	                                            }

	                                            [Other.Core.ImmutableOptions]
	                                            public readonly struct OtherMarkedOptions
	                                            {
	                                                public OtherMarkedOptions IgnoringX() => this;
	                                            }

	                                            [aweXpect.Core.ImmutableOptions]
	                                            public class BaseOptions
	                                            {
	                                            }

	                                            public class DerivedOptions : BaseOptions
	                                            {
	                                                public DerivedOptions IgnoringX() => this;
	                                            }
	                                        }
	                                        """;

	[Test]
	[Arguments("json.{|#0:IgnoringX|}();", "IgnoringX", "JsonOptionsStruct")]
	[Arguments("nullableJson?.{|#0:IgnoringX|}();", "IgnoringX", "JsonOptionsStruct")]
	[Arguments("xml.{|#0:IgnoringX|}();", "IgnoringX", "XmlOptions")]
	[Arguments("XmlOptionsExtensions.{|#0:IgnoringX|}(xml);", "IgnoringX", "XmlOptions")]
	[Arguments("nullableXml?.{|#0:IgnoringX|}();", "IgnoringX", "XmlOptions")]
	[Arguments("Func<XmlOptions, XmlOptions> f = o => { o.{|#0:IgnoringX|}(); return o; };", "IgnoringX",
		"XmlOptions")]
	[Arguments("other.{|#0:IgnoringX|}();", "IgnoringX", "OtherOptions")]
	[Arguments("nested.{|#0:IgnoringX|}();", "IgnoringX", "Nested")]
	public async Task WhenTheReturnedExtensionOptionsAreDiscarded_ShouldBeFlagged(string statement, string method,
		string type) => await Verifier
		.VerifyAnalyzerAsync(
			$$"""
			  #nullable enable
			  using System;
			  using aweXpect.Json;
			  using aweXpect.Xml;
			  using Other;

			  public class MyClass
			  {
			      public void MyMethod(JsonOptionsStruct json, JsonOptionsStruct? nullableJson, XmlOptions xml,
			          XmlOptions? nullableXml, OtherOptions other, Outer.Nested nested)
			      {
			          {{statement}}
			      }
			  }

			  {{ExtensionOptions}}
			  """,
			Verifier.Diagnostic(Rules.DiscardedOptionRule)
				.WithLocation(0)
				.WithArguments(method, type)
		);

	[Test]
	[Arguments("xml = xml.IgnoringX();")]
	[Arguments("json = json.IgnoringX();")]
	[Arguments("Consume(xml.IgnoringX());")]
	[Arguments("Func<XmlOptions, XmlOptions> f = o => o.IgnoringX();")]
	[Arguments("Func<XmlOptions, XmlOptions> f = o => { return o.IgnoringX(); };")]
	public async Task WhenTheReturnedExtensionOptionsAreUsed_ShouldNotBeFlagged(string statement) => await Verifier
		.VerifyAnalyzerAsync(
			$$"""
			  using System;
			  using aweXpect.Json;
			  using aweXpect.Xml;

			  public class MyClass
			  {
			      public void MyMethod(JsonOptionsStruct json, XmlOptions xml)
			      {
			          {{statement}}
			      }

			      private static void Consume(XmlOptions value)
			      {
			      }
			  }

			  {{ExtensionOptions}}
			  """
		);

	[Test]
	[Arguments("Quantifier.Default.{|#0:AtLeast|}(2);", "AtLeast", "Quantifier")]
	[Arguments("quantifier.{|#0:AtLeast|}(2);", "AtLeast", "Quantifier")]
	[Arguments("holder.Value.{|#0:AtLeast|}(2);", "AtLeast", "Quantifier")]
	[Arguments("CollectionMatchOptions.Default.{|#0:InAnyOrder|}();", "InAnyOrder", "CollectionMatchOptions")]
	[Arguments("tolerance.{|#0:Within|}(0.1);", "Within", "NumberTolerance")]
	[Arguments("StringEqualityOptions.Default.{|#0:IgnoringCase|}();", "IgnoringCase", "StringEqualityOptions")]
	[Arguments("SignalerPredicateOptions<int>.Default.{|#0:AddPredicate|}(x => x > 0, \"x > 0\");", "AddPredicate",
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

	[Test]
	[Arguments("unmarked.IgnoringX();")]
	[Arguments("unmarkedStruct.IgnoringX();")]
	[Arguments("otherMarked.IgnoringX();")]
	[Arguments("derived.IgnoringX();")]
	[Arguments("xml.Log();")]
	public async Task WhenTheTypeIsNoImmutableOption_ShouldNotBeFlagged(string statement) => await Verifier
		.VerifyAnalyzerAsync(
			$$"""
			  using aweXpect.Options;
			  using aweXpect.Xml;
			  using Other;

			  public class MyClass
			  {
			      public void MyMethod(UnmarkedOptions unmarked, UnmarkedStruct unmarkedStruct,
			          OtherMarkedOptions otherMarked, DerivedOptions derived, XmlOptions xml)
			      {
			          {{statement}}
			      }
			  }

			  {{ExtensionOptions}}
			  """
		);
}
