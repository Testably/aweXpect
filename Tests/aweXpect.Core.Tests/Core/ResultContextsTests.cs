using System.Collections.Generic;
using System.Linq;

namespace aweXpect.Core.Tests.Core;

public sealed class ResultContextsTests
{
	[Fact]
	public async Task Add_ShouldAddContext()
	{
		ResultContexts sut = new();

		sut.Add(new ResultContext.Fixed("foo", "bar"));

		await That(sut).HasSingle().Which.IsEquivalentTo(new
		{
			Title = "foo",
		});
	}

	[Fact]
	public async Task AddMultiple_ShouldAddContext()
	{
		ResultContexts sut = new();

		sut.Add(new ResultContext.Fixed("foo", "1"));
		sut.Add(new ResultContext.Fixed("foo", "2"));
		sut.Add(new ResultContext.Fixed("foo", "2"));
		sut.Add(new ResultContext.Fixed("bar", "3"));
		sut.Add(new ResultContext.Fixed("foo", "4"));

		await That(sut).HasCount().EqualTo(5);
	}

	[Fact]
	public async Task AddSameContextMultipleTimes_ShouldStillWork()
	{
		var sharedContext = new ResultContext.Fixed("foo", "1");
		ResultContexts sut = new();
		sut.Add(sharedContext);
		sut.Add(sharedContext);
		sut.Add(sharedContext);
		
		await That(sut).All().ComplyWith(x => x.IsSameAs(sharedContext));
	}

	[Fact]
	public async Task Clear_ShouldRemoveAllContexts()
	{
		ResultContexts sut =
		[
			new ResultContext.Fixed("foo", "bar"),
			new ResultContext.Fixed("foo", "bar"),
		];

		sut.Clear();

		await That(sut).IsEmpty();
	}

	[Fact]
	public async Task Close_ShouldRestrictAdd()
	{
		ResultContexts sut = new();
		sut.Add(new ResultContext.Fixed("foo", "1"));

		sut.Close();

		sut.Add(new ResultContext.Fixed("bar", "2"));
		await That(sut).HasSingle().Which.IsEquivalentTo(new
		{
			Title = "foo",
		});
	}

	[Fact]
	public async Task Close_ShouldRestrictClear()
	{
		ResultContexts sut = new();
		sut.Add(new ResultContext.Fixed("foo", "1"));

		sut.Close();

		sut.Clear();
		await That(sut).HasSingle().Which.IsEquivalentTo(new
		{
			Title = "foo",
		});
	}

	[Fact]
	public async Task Close_ShouldRestrictRemoveWithPredicate()
	{
		ResultContexts sut = new();
		sut.Add(new ResultContext.Fixed("foo", "1"));

		sut.Close();

		sut.Remove(_ => true);
		await That(sut).HasSingle().Which.IsEquivalentTo(new
		{
			Title = "foo",
		});
	}

	[Fact]
	public async Task Close_ShouldRestrictRemoveWithTitle()
	{
		ResultContexts sut = new();
		sut.Add(new ResultContext.Fixed("foo", "1"));

		sut.Close();

		sut.Remove("foo");
		await That(sut).HasSingle().Which.IsEquivalentTo(new
		{
			Title = "foo",
		});
	}

	[Fact]
	public async Task Open_ShouldRestrictAdd()
	{
		ResultContexts sut = new();
		sut.Add(new ResultContext.Fixed("foo", "1"));
		sut.Close();

		sut.Open();

		sut.Add(new ResultContext.Fixed("bar", "2"));
		await That(sut).HasCount().EqualTo(2);
	}

	[Fact]
	public async Task Open_ShouldRestrictClear()
	{
		ResultContexts sut = new();
		sut.Add(new ResultContext.Fixed("foo", "1"));
		sut.Close();

		sut.Open();

		sut.Clear();
		await That(sut).IsEmpty();
	}

	[Fact]
	public async Task Open_ShouldRestrictRemoveWithPredicate()
	{
		ResultContexts sut = new();
		sut.Add(new ResultContext.Fixed("foo", "1"));
		sut.Close();

		sut.Open();

		sut.Remove(_ => true);
		await That(sut).IsEmpty();
	}

	[Fact]
	public async Task Open_ShouldRestrictRemoveWithTitle()
	{
		ResultContexts sut = new();
		sut.Add(new ResultContext.Fixed("foo", "1"));
		sut.Close();

		sut.Open();

		sut.Remove("foo");
		await That(sut).IsEmpty();
	}

	[Fact]
	public async Task Remove_WithPredicate_ShouldRemoveAllMatchingContexts()
	{
		ResultContexts sut =
		[
			new ResultContext.Fixed("foo", "bar"),
			new ResultContext.Fixed("bar", "baz"),
			new ResultContext.Fixed("foo", "bar"),
		];

		sut.Remove(c => c.Title == "foo");

		await That(sut).HasSingle().Which.IsEquivalentTo(new
		{
			Title = "bar",
		});
	}

	[Fact]
	public async Task Remove_WithTitle_ShouldRemoveAllMatchingContexts()
	{
		ResultContexts sut =
		[
			new ResultContext.Fixed("foo", "bar"),
			new ResultContext.Fixed("bar", "baz"),
			new ResultContext.Fixed("foo", "bar"),
		];

		sut.Remove("foo");

		await That(sut).HasSingle().Which.IsEquivalentTo(new
		{
			Title = "bar",
		});
	}

	[Fact]
	public async Task ShouldInitializeEmpty()
	{
		ResultContexts sut = new();

		await That(sut).IsEmpty();
	}
}
