using System;
using System.Web;
using NUnit.Framework;
using SUT = Paraesthesia.Web.Configuration.UrlAbsolutifier;

namespace Paraesthesia.Test.Unit.Web.Configuration.UrlAbsolutifier
{
	[TestFixture]
	public class UrlRegexRule
	{
		private const string DefaultRegexValue = @".*\.aspx$";
		private SUT.UrlRegexRule _rule = null;

		[SetUp]
		public void SetUp()
		{
			_rule = new SUT.UrlRegexRule(SUT.RuleProcess.Include, DefaultRegexValue);
		}

		[Test(Description = "Ensures a null context doesn't match the rule.")]
		public void ContextMatchesRule_NullContext()
		{
			Assert.IsFalse(this._rule.ContextMatchesRule(null), "A null context should not match the rule.");
		}

		[Test(Description = "Runs the rule against a request URL that matches the specified regex, having no querystring.")]
		public void ContextMatchesRule_UrlMatchNoQueryString()
		{
			HttpContext context = this.CreateContext("foo.aspx", "http://localhost/directory/foo.aspx", "");
			Assert.IsTrue(this._rule.ContextMatchesRule(context), "A URL should match the regex regardless of querystring.");
		}

		[Test(Description = "Runs the rule against a request URL that matches the specified regex, despite having a querystring.")]
		public void ContextMatchesRule_UrlMatchQueryString()
		{
			HttpContext context = this.CreateContext("foo.aspx", "http://localhost/directory/foo.aspx", "this=that&a=b");
			Assert.IsTrue(this._rule.ContextMatchesRule(context), "A URL should match the regex regardless of querystring.");
		}

		[Test(Description = "Runs the rule against a request URL that doesn't match the specified regex.")]
		public void ContextMatchesRule_UrlNoMatch()
		{
			HttpContext context = this.CreateContext("foo.html", "http://localhost/directory/foo.html", "this=that&a=b");
			Assert.IsFalse(this._rule.ContextMatchesRule(context), "A URL that doesn't match the configured regex should not match the rule.");
		}

		[Test(Description = "Verifies the value of the rule is converted during construction.")]
		public void Ctor_ConvertValue()
		{
			Assert.AreEqual(DefaultRegexValue, this._rule.Value.ToString(), "The value should be the specified regular expression.");
		}

		[Test(Description = "Ensures empty values for the rule are not accepted.")]
		[ExpectedException(typeof(System.ArgumentException))]
		public void Ctor_EmptyValue()
		{
			SUT.UrlRegexRule testSpecificRule = new SUT.UrlRegexRule(SUT.RuleProcess.Include, "");
		}

		[Test(Description = "Ensures null values for the rule are not accepted.")]
		[ExpectedException(typeof(System.ArgumentException))]
		public void Ctor_NullValue()
		{
			SUT.UrlRegexRule testSpecificRule = new SUT.UrlRegexRule(SUT.RuleProcess.Include, null);
		}

		[Test(Description = "Checks the type of converter returned from the ValueConverter property.")]
		public void ValueConverter_RegexConverter()
		{
			Assert.IsInstanceOfType(typeof(Paraesthesia.Web.Configuration.RegexConverter), this._rule.ValueConverter, "The converter should be a RegexConverter.");
		}

		private HttpContext CreateContext(string filename, string url, string queryString)
		{
			HttpRequest request = new HttpRequest(filename, url, queryString);
			HttpResponse response = new HttpResponse(new System.IO.StreamWriter(new System.IO.MemoryStream()));
			HttpContext context = new HttpContext(request, response);
			return context;
		}
	}
}
