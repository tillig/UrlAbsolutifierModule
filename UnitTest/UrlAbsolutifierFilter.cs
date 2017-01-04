using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using SUT = Paraesthesia.Web;

namespace Paraesthesia.Test.Unit.Web
{
	[TestFixture]
	public class UrlAbsolutifierFilter
	{
		#region Variables and Constants

		/// <summary>
		/// A filter pre-configured and ready for testing.
		/// </summary>
		private SUT.UrlAbsolutifierFilter _filter = null;

		#endregion


		#region SetUp/TearDown

		[SetUp]
		public void SetUp()
		{
			this._filter = new SUT.UrlAbsolutifierFilter(null, new Uri("http://localhost/subfolder/file.aspx"));
		}

		[TearDown]
		public void TearDown()
		{
			this._filter.Dispose();
		}

		#endregion


		#region Tests

		[Test(Description = "Checks behavior against a document that contains all of the supported attributes.")]
		public void Close_AllSupportedAttributes()
		{
			// This data tests:
			// * Every supported attribute.
			// * Relative URLs in the form "../foo/bar.html" and "foo/bar.html".
			// * Rooted URLs in the form "/foo/bar.html".
			// * Multiple supported attributes on a single tag.
			// * Multiple tags with supported attributes on a single line.
			// * Multiple supported attributes split across lines.
			string data =
				"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\n\n" +
				"<html>\n" +
				"<head profile=\"/profile/file.html\">\n" +
				"<title>Page Title</title>\n" +
				"<script src=\"/script/rooted-javascript.js\" type=\"text/javascript\" for=\"/for/file.html\"></script>\n" +
				"<script src=\"script/relative-javascript.js\" type=\"text/javascript\"></script>\n" +
				"</head>\n" +
				"<body background=\"/images/image.gif\">\n" +
				"<form action=\"/action/file.html\">\n" +
				"<object classid=\"/classid/file.html\" codebase=\"/codebase/file.html\" data=\"/data/file.html\" usemap=\"/usemap/file.html\"/>\n" +
				"<blockquote cite=\"/cite/file.html\"></blockquote>\n" +
				"<table datasrc=\"/datasrc/file.html\"></table>\n" +
				"<p><img src=\"/images/rooted-image.gif\" /></p><p><img src = \"../images/relative-image.gif\" /></p>\n" +
				"<p><a href =\"/folder/rooted-file.html\">Link 1</a></p>\n" +
				"<p><a href= \"../folder/relative-file.html\">Link 2</a></p>\n" +
				"<iframe longdesc=\"/longdesc/file.html\"\n" +
				"src=\"/framesrc/file.html\"/>\n" +
				"</form>\n" +
				"</body>\n" +
				"</html>";

			string expected =
				"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\n\n" +
				"<html>\n" +
				"<head profile=\"http://localhost/profile/file.html\">\n" +
				"<title>Page Title</title>\n" +
				"<script src=\"http://localhost/script/rooted-javascript.js\" type=\"text/javascript\" for=\"http://localhost/for/file.html\"></script>\n" +
				"<script src=\"http://localhost/subfolder/script/relative-javascript.js\" type=\"text/javascript\"></script>\n" +
				"</head>\n" +
				"<body background=\"http://localhost/images/image.gif\">\n" +
				"<form action=\"http://localhost/action/file.html\">\n" +
				"<object classid=\"http://localhost/classid/file.html\" codebase=\"http://localhost/codebase/file.html\" data=\"http://localhost/data/file.html\" usemap=\"http://localhost/usemap/file.html\"/>\n" +
				"<blockquote cite=\"http://localhost/cite/file.html\"></blockquote>\n" +
				"<table datasrc=\"http://localhost/datasrc/file.html\"></table>\n" +
				"<p><img src=\"http://localhost/images/rooted-image.gif\" /></p><p><img src = \"http://localhost/images/relative-image.gif\" /></p>\n" +
				"<p><a href =\"http://localhost/folder/rooted-file.html\">Link 1</a></p>\n" +
				"<p><a href= \"http://localhost/folder/relative-file.html\">Link 2</a></p>\n" +
				"<iframe longdesc=\"http://localhost/longdesc/file.html\"\n" +
				"src=\"http://localhost/framesrc/file.html\"/>\n" +
				"</form>\n" +
				"</body>\n" +
				"</html>";

			string actual = PassStringThroughFilter(data);
			Assert.AreEqual(expected, actual, "All supported attributes should be processed.");
		}

		[Test(Description = "Tests absolutification of a small simple content block with a malformed URL.")]
		public void Close_BadUrl()
		{
			string expected = "<p><img src=\"badly:formed/URL&here\" /></p>";
			Uri requestUrl = new Uri("http://localhost/otherpath/file.html");
			string actual = PassStringThroughFilter(expected);
			Assert.AreEqual(expected, actual, "A malformed URL in the document should be ignored.");
		}

		[Test(Description = "Tests absolutification of a content block that contains a base href.")]
		public void Close_BaseHref()
		{
			string data = "<base href=\"/base\" /><p><img src=\"../somepath/someimage.gif\" /></p>";
			string expected = "<base href=\"http://localhost/base\" /><p><img src=\"../somepath/someimage.gif\" /></p>";
			Uri requestUrl = new Uri("http://localhost/otherpath/file.html");
			string actual = PassStringThroughFilter(data);
			Assert.AreEqual(expected, actual, "If there is a base href, it should be absolutified but nothing else should be touched.");
		}

		[Test(Description = "Checks behavior when there are no URLs to manipulate.")]
		public void Close_NoUrls()
		{
			string expected = "This content has no URLs to process.";
			string actual = PassStringThroughFilter(expected);
			Assert.AreEqual(expected, actual, "If there are no URLs, no change should occur.");
		}

		[Test(Description = "Tests absolutification of a small simple content block with multiple relative URLs in different attributes on the same tag.")]
		public void Close_MultipleAttributesOneTag()
		{
			string data = "<p><img src=\"../somepath/someimage.gif\" alt=\"Some text\" datasrc=\"../anotherpath/data.html\" /></p>";
			string expected = "<p><img src=\"http://localhost/somepath/someimage.gif\" alt=\"Some text\" datasrc=\"http://localhost/anotherpath/data.html\" /></p>";
			Uri requestUrl = new Uri("http://localhost/otherpath/file.html");
			string actual = PassStringThroughFilter(data);
			Assert.AreEqual(expected, actual, "Both attributes' URLs should be absolutified.");
		}

		[Test(Description = "Tests the chaining of filters.")]
		public void Close_Passthrough()
		{
			string data = "<p><img src=\"/somepath/someimage.gif\" /></p>";
			string expected = "<p><img src=\"http://localhost/somepath/someimage.gif\" /></p>" + TestStream.StreamWriteInsert;
			string actual;
			using (MemoryStream memStream = new MemoryStream())
			using (TestStream baseFilter = new TestStream(memStream))
			using (SUT.UrlAbsolutifierFilter testFilter = new SUT.UrlAbsolutifierFilter(baseFilter, new Uri("http://localhost/folder/file.html")))
            {
				byte[] content = StringToByteArray(data);
				testFilter.Write(content, 0, content.Length);
				testFilter.Close();
				actual = StreamToString(memStream);
            }
			Assert.AreEqual(expected, actual, "The URL should be absolutified and pass through the original filter.");
		}

		[Test(Description = "Tests processing of a non-HTML content source.")]
		public void Close_Rss()
		{
			string data =
				@"<?xml version=""1.0"" encoding=""UTF-8""?>\n" +
				@"<rss xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns:trackback=""http://madskills.com/public/xml/rss/module/trackback/"" xmlns:wfw=""http://wellformedweb.org/CommentAPI/"" xmlns:slash=""http://purl.org/rss/1.0/modules/slash/"" xmlns:copyright=""http://blogs.law.harvard.edu/tech/rss"" xmlns:image=""http://purl.org/rss/1.0/modules/image/"" xmlns:creativeCommons=""http://backend.userland.com/creativeCommonsRssModule"" xmlns:feedburner=""http://rssnamespace.org/feedburner/ext/1.0"" version=""2.0"">\n" +
				@"    <channel>\n" +
				@"        <title>Paraesthesia</title>\n" +
				@"        <link>http://paraesthesia.com/Default.aspx</link>\n" +
				@"        <item>\n" +
				@"            <title>Demo RSS Item</title>\n" +
				@"            <category>Post Category</category>\n" +
				@"            <description>&lt;p&gt;This is a link &lt;a href=""http://www.paraesthesia.com/Default.aspx""&gt;right here&lt;/a&gt;.&lt;/p&gt;\n" +
				@"&lt;p&gt;&lt;img height=""300"" alt=""Alternate Text"" width=""400"" src=""/images/paraesthesia_com/someimage.gif"" /&gt;&lt;/p&gt;\n" +
				@"</description>\n" +
				@"            <dc:creator>Travis Illig</dc:creator>\n" +
				@"            <guid isPermaLink=""false"">http://paraesthesia.com/abc.aspx</guid>\n" +
				@"            <pubDate>Thu, 01 Nov 2007 12:00:00 GMT</pubDate>\n" +
				@"            <wfw:comment>http://paraesthesia.com/def.aspx</wfw:comment>\n" +
				@"            <comments>http://paraesthesia.com/ghi.aspx</comments>\n" +
				@"            <slash:comments>1</slash:comments>\n" +
				@"            <wfw:commentRss>http://paraesthesia.com/jkl.aspx</wfw:commentRss>\n" +
				@"        <feedburner:origLink>http://paraesthesia.com/mno.aspx</feedburner:origLink></item>\n" +
				@"    </channel>\n" +
				@"</rss>";
			string expected =
				@"<?xml version=""1.0"" encoding=""UTF-8""?>\n" +
				@"<rss xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns:trackback=""http://madskills.com/public/xml/rss/module/trackback/"" xmlns:wfw=""http://wellformedweb.org/CommentAPI/"" xmlns:slash=""http://purl.org/rss/1.0/modules/slash/"" xmlns:copyright=""http://blogs.law.harvard.edu/tech/rss"" xmlns:image=""http://purl.org/rss/1.0/modules/image/"" xmlns:creativeCommons=""http://backend.userland.com/creativeCommonsRssModule"" xmlns:feedburner=""http://rssnamespace.org/feedburner/ext/1.0"" version=""2.0"">\n" +
				@"    <channel>\n" +
				@"        <title>Paraesthesia</title>\n" +
				@"        <link>http://paraesthesia.com/Default.aspx</link>\n" +
				@"        <item>\n" +
				@"            <title>Demo RSS Item</title>\n" +
				@"            <category>Post Category</category>\n" +
				@"            <description>&lt;p&gt;This is a link &lt;a href=""http://www.paraesthesia.com/Default.aspx""&gt;right here&lt;/a&gt;.&lt;/p&gt;\n" +
				@"&lt;p&gt;&lt;img height=""300"" alt=""Alternate Text"" width=""400"" src=""http://localhost/images/paraesthesia_com/someimage.gif"" /&gt;&lt;/p&gt;\n" +
				@"</description>\n" +
				@"            <dc:creator>Travis Illig</dc:creator>\n" +
				@"            <guid isPermaLink=""false"">http://paraesthesia.com/abc.aspx</guid>\n" +
				@"            <pubDate>Thu, 01 Nov 2007 12:00:00 GMT</pubDate>\n" +
				@"            <wfw:comment>http://paraesthesia.com/def.aspx</wfw:comment>\n" +
				@"            <comments>http://paraesthesia.com/ghi.aspx</comments>\n" +
				@"            <slash:comments>1</slash:comments>\n" +
				@"            <wfw:commentRss>http://paraesthesia.com/jkl.aspx</wfw:commentRss>\n" +
				@"        <feedburner:origLink>http://paraesthesia.com/mno.aspx</feedburner:origLink></item>\n" +
				@"    </channel>\n" +
				@"</rss>";

			string actual = PassStringThroughFilter(data);
			Assert.AreEqual(expected, actual, "Encoded data in RSS should be processed.");

		}

		[Test(Description = "Tests absolutification of a simple content block that only contains absolute URLs.")]
		public void Close_SingleAbsoluteUrl()
		{
			string expected = "<p><img src=\"http://localhost/somepath/someimage.gif\" /></p>";
			string actual = PassStringThroughFilter(expected);
			Assert.AreEqual(expected, actual, "If there are no URLs, no change should occur.");
		}

		[Test(Description = "Tests absolutification of a small simple content block with a single relative URL.")]
		public void Close_SingleRelativeUrl()
		{
			string data = "<p><img src=\"../somepath/someimage.gif\" /></p>";
			string expected = "<p><img src=\"http://localhost/somepath/someimage.gif\" /></p>";
			string actual = PassStringThroughFilter(data);
			Assert.AreEqual(expected, actual, "The URL should be absolutified.");
		}

		[Test(Description = "Tests absolutification of a small simple content block with a single rooted URL.")]
		public void Close_SingleRootedUrl()
		{
			string data = "<p><img src=\"/somepath/someimage.gif\" /></p>";
			string expected = "<p><img src=\"http://localhost/somepath/someimage.gif\" /></p>";
			string actual = PassStringThroughFilter(data);
			Assert.AreEqual(expected, actual, "The URL should be absolutified.");
		}

		[Test(Description = "Ensures you can't create a stream with a null request URL.")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void Ctor_NullRequestUrl()
		{
			using (MemoryStream mem = new MemoryStream())
			{
				SUT.UrlAbsolutifierFilter actual = new SUT.UrlAbsolutifierFilter(mem, null);
			}
		}

		// TODO: Round out UrlAbsolutifierFilter unit tests.

		#endregion


		#region Helpers

		[Test(Description = "Checks the functionality of the TestStream class used in testing UrlAbsolutifierFilter.")]
		public void TestStream_FunctionalityCheck()
		{
			string baseValue = "value";
			string writtenValue = "";
			using (MemoryStream mem = new MemoryStream())
           	using (TestStream test = new TestStream(mem))
			{
				test.Write(StringToByteArray(baseValue), 0, baseValue.Length);
				writtenValue = StreamToString(mem);
            }
			Assert.AreEqual(baseValue + TestStream.StreamWriteInsert, writtenValue, "The TestStream class should append a constant value to the end of the stream so we know it passed through.");
		}

		private class TestStream : MemoryStream
		{
			public const string StreamWriteInsert = "TESTED!";
			private Stream _incoming;

			public TestStream(Stream incoming)
			{
				if (incoming == null)
				{
					throw new ArgumentNullException("incoming");
				}
				this._incoming = incoming;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				this._incoming.Write(buffer, offset, count);
				this._incoming.Write(new System.Text.UTF8Encoding().GetBytes(StreamWriteInsert), 0, StreamWriteInsert.Length);
			}

			public override void Close()
			{
				this._incoming.Close();
				base.Close();
			}
		}

		private string PassStringThroughFilter(string input)
		{
			byte[] content = StringToByteArray(input);
			this._filter.Write(content, 0, content.Length);
			this._filter.Close();
			string output = StreamToString(this._filter);
			return output;
		}

		private static string StreamToString(MemoryStream stream)
		{
			return System.Text.Encoding.UTF8.GetString(stream.ToArray());
		}

		private static byte[] StringToByteArray(string value)
		{
			return new System.Text.UTF8Encoding().GetBytes(value);
		}

		#endregion
	}
}
