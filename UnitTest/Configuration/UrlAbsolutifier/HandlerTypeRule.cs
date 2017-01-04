using System;
using System.Web;
using System.Web.UI;
using NUnit.Framework;
using SUT = Paraesthesia.Web.Configuration.UrlAbsolutifier;

namespace Paraesthesia.Test.Unit.Web.Configuration.UrlAbsolutifier
{
	[TestFixture]
	public class HandlerTypeRule
	{
		SUT.HandlerTypeRule _rule = null;

		[SetUp]
		public void SetUp()
		{
			_rule = new SUT.HandlerTypeRule(SUT.RuleProcess.Include, typeof(Page).AssemblyQualifiedName);
		}

		[Test(Description = "Runs the rule against a request handler that doesn't match the specified type.")]
		public void ContextMatchesRule_HandlerNoMatch()
		{
			NonPageHandler handler = new NonPageHandler();
			HttpContext context = this.CreateContext(handler);
			Assert.IsFalse(this._rule.ContextMatchesRule(context), "A handler that doesn't match the configured type should not match the rule.");
		}

		[Test(Description = "Runs the rule against a request handler that matches the specified type.")]
		public void ContextMatchesRule_HandlerMatch()
		{
			PageHandler handler = new PageHandler();
			HttpContext context = this.CreateContext(handler);
			Assert.IsTrue(this._rule.ContextMatchesRule(context), "A handler that matches the configured type should match the rule.");
		}

		[Test(Description = "Ensures a null context doesn't match the rule.")]
		public void ContextMatchesRule_NullContext()
		{
			Assert.IsFalse(this._rule.ContextMatchesRule(null), "A null context should not match the rule.");
		}

		[Test(Description = "Ensures a null request handler doesn't match the rule.")]
		public void ContextMatchesRule_NullHandler()
		{
			HttpContext context = this.CreateContext(null);
			Assert.IsFalse(this._rule.ContextMatchesRule(context), "A null context should not match the rule.");
		}

		[Test(Description = "Verifies the value of the rule is converted during construction.")]
		public void Ctor_ConvertValue()
		{
			Assert.AreEqual(typeof(Page), this._rule.Value, "The value should be the type System.Web.UI.Page.");
		}

		[Test(Description = "Ensures empty values for the rule are not accepted.")]
		[ExpectedException(typeof(System.ArgumentException))]
		public void Ctor_EmptyValue()
		{
			SUT.HandlerTypeRule testSpecificRule = new SUT.HandlerTypeRule(SUT.RuleProcess.Include, "");
		}

		[Test(Description = "Ensures null values for the rule are not accepted.")]
		[ExpectedException(typeof(System.ArgumentException))]
		public void Ctor_NullValue()
		{
			SUT.HandlerTypeRule testSpecificRule = new SUT.HandlerTypeRule(SUT.RuleProcess.Include, null);
		}

		[Test(Description = "Checks the type of converter returned from the ValueConverter property.")]
		public void ValueConverter_TypeNameConverter()
		{
			Assert.IsInstanceOfType(typeof(System.Configuration.TypeNameConverter), this._rule.ValueConverter, "The converter should be a TypeNameConverter.");
		}

		private HttpContext CreateContext(IHttpHandler handler)
		{
			HttpRequest request = new HttpRequest("foo.aspx", "http://localhost/foo.aspx", "");
			HttpResponse response = new HttpResponse(new System.IO.StreamWriter(new System.IO.MemoryStream()));
			HttpContext context = new HttpContext(request, response);
			context.Handler = handler;
			return context;
		}

		/// <summary>
		/// Handler class used in testing.  Does not derive from <see cref="System.Web.UI.Page"/>.
		/// </summary>
		private class NonPageHandler : IHttpHandler
		{
			public bool IsReusable
			{
				get { throw new Exception("The method or operation is not implemented."); }
			}

			public void ProcessRequest(HttpContext context)
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Handler class used in testing.  Derives from <see cref="System.Web.UI.Page"/>.
		/// </summary>
		private class PageHandler : Page
		{
		}
	}
}
