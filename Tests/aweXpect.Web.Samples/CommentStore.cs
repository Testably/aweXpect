using System.Collections.Generic;

namespace aweXpect.Web.Samples;

internal class CommentStore
{
	private readonly Dictionary<int, Comment> _comments = new();

	public CommentStore()
	{
		_comments.Add(1, new Comment(1, "Valentin", "This is my first example comment"));
		_comments.Add(2, new Comment(2, "Breuß", "Another comment (my second)"));
	}

	public IEnumerable<Comment> GetComments() => _comments.Values;
	public Comment GetComment(int id) => _comments[id];
}
