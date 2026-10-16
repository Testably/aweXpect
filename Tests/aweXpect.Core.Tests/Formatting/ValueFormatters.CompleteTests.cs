using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class CompleteTests
	{
		[Test]
		public async Task WhenCollectionHasMoreItemsThanTheMaximum_ShouldWriteAllItems()
		{
			int[] value = Enumerable.Range(1, 25).ToArray();
			string expectedResult = $"[{string.Join(", ", value)}]";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.Complete);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.Complete);
			Formatter.Format(sb, value, FormattingOptions.Complete);

			await That(result).IsEqualTo(expectedResult)
				.Because("a snapshot must contain every item, not only the first ones");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCollectionHasMoreItemsThanTheMaximum_WithoutCompleteOption_ShouldStillShortenIt()
		{
			int[] value = Enumerable.Range(1, 12).ToArray();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 2 more)]")
				.Because("failure messages keep shortening long collections");
		}

		[Test]
		public async Task WhenCollectionIsLazy_ShouldWriteAllItems()
		{
			IEnumerable<int> value = Enumerable.Range(1, 12).Select(x => x);

			string result = Formatter.Format(value, FormattingOptions.Complete);

			await That(result).IsEqualTo("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]");
		}

		[Test]
		public async Task WhenCollectionIsNested_ShouldWriteAllItemsOnEveryLevel()
		{
			int[][] value = Enumerable.Range(0, 11).Select(x => Enumerable.Range(x * 11, 11).ToArray()).ToArray();
			string expectedResult =
				$"[{string.Join(", ", value.Select(inner => $"[{string.Join(", ", inner)}]"))}]";

			string result = Formatter.Format(value, FormattingOptions.Complete);

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCollectionIsRecursive_ShouldWriteTheRecursion()
		{
			List<object> value = [1,];
			value.Add(value);

			string result = Formatter.Format(value, FormattingOptions.Complete);

			await That(result).IsEqualTo("[1, [ *recursive* ]]");
		}

		[Test]
		public async Task WhenCustomFormatterIsRegistered_ShouldReceiveTheOptions()
		{
			using IDisposable _ = ValueFormatter.Register(new TaggedFormatter());
			Tagged value = new(Enumerable.Range(1, 12).ToArray());

			string result = Formatter.Format(value, FormattingOptions.Complete);

			await That(result).IsEqualTo("Tagged(IsComplete=True, [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12])")
				.Because("a custom formatter receives the options and passes them on to the nested values");
		}

		[Test]
		public async Task WhenDictionaryHasMoreEntriesThanTheMaximum_ShouldWriteAllEntries()
		{
			Dictionary<int, string> value = Enumerable.Range(1, 12).ToDictionary(x => x, x => $"v{x}");
			string expectedResult = $"{{{string.Join(", ", value.Select(x => $"[{x.Key}] = \"{x.Value}\""))}}}";

			string result = Formatter.Format(value, FormattingOptions.Complete);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.Complete);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenExceptionMessageIsLongerThanTheMaximumLength_ShouldWriteTheWholeMessage()
		{
			string message = new('x', 150);
			InvalidOperationException value = new(message);

			string result = Formatter.Format(value, FormattingOptions.Complete);

			await That(result).IsEqualTo($"InvalidOperationException: {message}");
		}

		[Test]
		public async Task WhenGraphIsDeeperThanTheMaximumDepth_ShouldWriteEveryLevel()
		{
			LinkedNode value = LinkedNode.Chain(100);
			string expectedResult =
				string.Concat(Enumerable.Repeat("ValueFormatters.CompleteTests.LinkedNode { Next = ", 100)) +
				"<null>" +
				string.Concat(Enumerable.Repeat(" }", 100));

			string result = Formatter.Format(value, FormattingOptions.Complete);

			await That(result).IsEqualTo(expectedResult)
				.Because("only the remaining stack limits the depth of complete output");
		}

		[Test]
		public async Task WhenGraphIsRecursive_ShouldWriteTheRecursion()
		{
			SelfNode value = new()
			{
				Value = 1,
			};
			value.Next = value;

			string result = Formatter.Format(value, FormattingOptions.Complete);

			await That(result).IsEqualTo(
				"ValueFormatters.CompleteTests.SelfNode { Next = ValueFormatters.CompleteTests.SelfNode { *recursive* }, Value = 1 }");
		}

		[Test]
		public async Task WhenGraphIsTooDeepForTheStack_ShouldThrowInsteadOfShortening()
		{
			LinkedNode value = LinkedNode.Chain(1_000_000);

			void Act() => Formatter.Format(value, FormattingOptions.Complete);

			await That(Act).Throws<InsufficientExecutionStackException>()
				.WithMessage("The value is nested too deeply to be formatted completely on the remaining stack.")
				.Because("complete output must neither overflow the stack nor leave out the deepest levels");
		}

		[Test]
		public async Task WhenGraphSharesNodesOnEveryLevel_ShouldWriteEveryNode()
		{
			SharingNode value = new();
			for (int i = 1; i < 12; i++)
			{
				value = new SharingNode
				{
					Left = value,
					Right = value,
				};
			}

			string result = Formatter.Format(value, FormattingOptions.Complete);

			await That(Count(result, "SharingNode { Left = ")).IsEqualTo(4095)
				.Because("complete output writes more than the 1000 nested objects of a failure message");
			await That(result).DoesNotContain("…");

			static int Count(string text, string part)
				=> (text.Length - text.Replace(part, "").Length) / part.Length;
		}

		[Test]
		public async Task WhenMultiDimensionalArrayHasMoreItemsThanTheMaximum_ShouldWriteAllItems()
		{
			int[,] value = new int[4, 4];
			value[3, 3] = 1;

			string result = Formatter.Format(value, FormattingOptions.Complete);

			await That(result).IsEqualTo("[[0, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, 1]]");
		}

		[Test]
		public async Task WhenObjectHasALongCollectionMember_ShouldWriteAllItems()
		{
			Wrapper value = new(Enumerable.Range(1, 12).ToArray());

			string result = Formatter.Format(value, FormattingOptions.Complete);

			await That(result).IsEqualTo(
				"ValueFormatters.CompleteTests.Wrapper { Inner = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12] }");
		}

		[Test]
		public async Task WhenStringIsLongerThanTheMaximumLength_ShouldWriteTheWholeString()
		{
			string value = new('a', 150);
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.Complete);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.Complete);
			Formatter.Format(sb, value, FormattingOptions.Complete);

			await That(result).IsEqualTo($"\"{value}\"");
			await That(objectResult).IsEqualTo($"\"{value}\"");
			await That(sb.ToString()).IsEqualTo($"\"{value}\"");
		}

		[Test]
		public async Task WithIndentation_ShouldWriteAllItemsIndented()
		{
			int[] value = Enumerable.Range(1, 12).ToArray();
			string expectedResult = """
			                        [
			                            1,
			                            2,
			                            3,
			                            4,
			                            5,
			                            6,
			                            7,
			                            8,
			                            9,
			                            10,
			                            11,
			                            12
			                          ]
			                        """;

			string result = Formatter.Format(value, FormattingOptions.Indented() with
			{
				IsComplete = true,
			});

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithMultipleLines_WhenItemIsAnObject_ShouldWriteAllItemsOnTheirOwnLines()
		{
			Wrapper[] value = Enumerable.Range(1, 11).Select(x => new Wrapper(x)).ToArray();
			string expectedResult =
				"[" + Environment.NewLine +
				string.Join("," + Environment.NewLine, value.Select(x =>
					$"  ValueFormatters.CompleteTests.Wrapper {{{Environment.NewLine}    Inner = {x.Inner}{Environment.NewLine}  }}")) +
				Environment.NewLine + "]";

			string result = Formatter.Format(value, FormattingOptions.MultipleLines with
			{
				IsComplete = true,
			});

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithType_ShouldPrefixTheTypeAndWriteAllItems()
		{
			int[] value = Enumerable.Range(1, 12).ToArray();

			string result = Formatter.Format(value, FormattingOptions.WithType with
			{
				IsComplete = true,
			});

			await That(result).IsEqualTo("int[] [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]");
		}

		private sealed class LinkedNode
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public LinkedNode? Next { get; set; }

			public static LinkedNode Chain(int length)
			{
				LinkedNode root = new();
				LinkedNode current = root;
				for (int i = 1; i < length; i++)
				{
					current.Next = new LinkedNode();
					current = current.Next;
				}

				return root;
			}
		}

		private sealed class SelfNode
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public SelfNode? Next { get; set; }

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Value { get; set; }
		}

		private sealed class SharingNode
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public SharingNode? Left { get; set; }

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public SharingNode? Right { get; set; }
		}

		private sealed class Tagged(int[] items)
		{
			public int[] Items { get; } = items;
		}

		private sealed class TaggedFormatter : IValueFormatter
		{
			public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
			{
				if (value is not Tagged tagged)
				{
					return false;
				}

				stringBuilder.Append("Tagged(IsComplete=").Append(options?.IsComplete).Append(", ");
				Formatter.Format(stringBuilder, tagged.Items, options);
				stringBuilder.Append(')');
				return true;
			}
		}

		private sealed class Wrapper(object inner)
		{
			public object Inner { get; } = inner;
		}
	}
}
