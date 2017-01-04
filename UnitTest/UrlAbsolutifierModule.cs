using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using NUnit.Framework;
using Paraesthesia.Web.Configuration.UrlAbsolutifier;
using TypeMock;
using SUT = Paraesthesia.Web;

namespace Paraesthesia.Test.Unit.Web
{
	[TestFixture]
	[VerifyMocks]
	public class UrlAbsolutifierModule
	{
		MockObject<SUT.UrlAbsolutifierModule> _mockModule = null;
		SUT.UrlAbsolutifierModule _module = null;
		HttpContext _context = null;

		[SetUp]
		public void SetUp()
		{
			// Set up the module for mock expectations
			this._mockModule = MockManager.MockObject<SUT.UrlAbsolutifierModule>(Constructor.NotMocked);
			this._module = this._mockModule.Object;
			this.ResetModuleConfiguration();

			// Mock the HttpContext.Current
			this._context = CreateContext();
			using (RecordExpectations recorder = RecorderManager.StartRecording())
			{
				HttpContext dummy = HttpContext.Current;
				recorder.Return(this._context);
				recorder.RepeatAlways();
			}
		}

		[TearDown]
		public void TearDown()
		{
			this.ResetModuleConfiguration();
		}

		[Test(Description = "Ensures the response for a context that is included from the rules will be filtered.")]
		public void AppendResponseFilter_Match()
		{
			Assert.IsNull(this._context.Response.Filter, "There should be no filter before the module runs.");
			this._mockModule.ExpectAndReturn("ResponseNeedsFilter", true);
			using (RecordExpectations recorder = RecorderManager.StartRecording())
			{
				this._context.Response.Filter = null;
				recorder.CheckArguments(Check.IsTypeOf<SUT.UrlAbsolutifierFilter>("The filter that gets set should be a UrlAbsolutifierFilter."));
			}
			this.InvokeNonPublicInstanceMethod("AppendResponseFilter", new object[] { null, null });
		}

		[Test(Description = "Ensures the response for a context that is excluded from the rules will not be filtered.")]
		public void AppendResponseFilter_NoMatch()
		{
			Assert.IsNull(this._context.Response.Filter, "There should be no filter before the module runs.");
			this._mockModule.ExpectAndReturn("ResponseNeedsFilter", false);
			using (RecordExpectations recorder = RecorderManager.StartRecording())
			{
				recorder.VerifyMode = VerifyMode.PassIfNotCalled;
				this._context.Response.Filter = null;
				recorder.FailWhenCalled();
			}
			this.InvokeNonPublicInstanceMethod("AppendResponseFilter", new object[] { null, null });
		}

		[Test(Description = "Verifies that Dispose does not throw an exception.")]
		public void Dispose()
		{
			this._module.Dispose();
		}

		[Test(Description = "Ensures there has to be some configuration returned.")]
		[ExpectedException(typeof(System.Configuration.ConfigurationErrorsException))]
		public void GetConfiguration_NullConfiguration()
		{
			using (RecordExpectations recorder = RecorderManager.StartRecording())
			{
				object dummy = System.Configuration.ConfigurationManager.GetSection(null);
				recorder.WhenArgumentsMatch(SUT.UrlAbsolutifierModule.ConfigSectionId);
				recorder.Return(null);
			}
			this.InvokeNonPublicStaticMethod("GetConfiguration", null);
		}

		[Test(Description = "Gets configuration from the configuration manager.")]
		public void GetConfiguration_ValidConfiguration()
		{
			UrlAbsolutifierSection config = this.MockConfiguration();
			object actual = this.InvokeNonPublicStaticMethod("GetConfiguration", null);
			Assert.AreSame(config, actual, "The configuration returned was not correct.");
		}

		[Test(Description = "Verifies the set of required things occurs during initialization.")]
		public void Init_InitializationOccurs()
		{
			using (RecordExpectations recorder = RecorderManager.StartRecording())
			{
				HttpApplication dummy = new HttpApplication();

				// Ensure the PostRequestHandlerExecute event gets subscribed to.
				dummy.PostRequestHandlerExecute += null;
			}
			HttpApplication application = new HttpApplication();

			// Ensure the rules get initialized.
			this._mockModule.CallStatic.ExpectCall("InitializeRules");

			this._module.Init(application);
		}

		[Test(Description = "Ensures the module can't initialize against a null application.")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void Init_NullApplication()
		{
			this._module.Init(null);
		}

		[Test(Description = "Ensures rules get initialized on the first call.")]
		public void InitializeRules_Initialization()
		{
			this.MockConfiguration();
			object rules = this.GetNonPublicStaticField("ConfiguredRules");
			Assert.IsNull(rules, "The rules should start out not initialized.");
			this.InvokeNonPublicStaticMethod("InitializeRules", null);
			rules = this.GetNonPublicStaticField("ConfiguredRules");
			Assert.IsNotNull(rules, "The rules should be initialized after the InitializeRules call.");
		}

		[Test(Description = "Ensures rules won't get double-initialized.")]
		public void InitializeRules_ReInitialization()
		{
			this.MockConfiguration();
			this.InvokeNonPublicStaticMethod("InitializeRules", null);
			object firstInit = this.GetNonPublicStaticField("ConfiguredRules");
			this.InvokeNonPublicStaticMethod("InitializeRules", null);
			object secondInit = this.GetNonPublicStaticField("ConfiguredRules");
			Assert.AreSame(firstInit, secondInit, "Initialization should only happen once, regardless of multiple calls to InitializeRules.");
		}

		[Test(Description = "Tests the yield of an empty configuration.")]
		public void ParseConfiguration_EmptyConfiguration()
		{
			UrlAbsolutifierSection config = new UrlAbsolutifierSection();
			List<Rule> rules = this.InvokeNonPublicStaticMethod("ParseConfiguration", new object[] { config }) as List<Rule>;
			Assert.IsNotNull(rules, "The configuration should be parsed into a list of rules.");
			Assert.IsEmpty(rules, "Empty configuration should yield an empty list of rules.");
		}

		[Test(Description = "Tests the yield of a configuration section with multiple rules.")]
		public void ParseConfiguration_MultipleRules()
		{
			UrlAbsolutifierSection config = new UrlAbsolutifierSection();
			int ruleCount = 5;
			for (int i = 0; i < ruleCount; i++)
			{
				RuleProcess process = (i % 2 == 0 ? RuleProcess.Include : RuleProcess.Exclude);
				config.Rules.Add(new RuleConfigurationElement(process, typeof(TestFalseRule), i.ToString()));
			}
			List<Rule> rules = this.InvokeNonPublicStaticMethod("ParseConfiguration", new object[] { config }) as List<Rule>;
			Assert.IsNotNull(rules, "The configuration should be parsed into a list of rules.");
			Assert.AreEqual(ruleCount, rules.Count, "The wrong number of rules was parsed.");
			for (int i = 0; i < ruleCount; i++)
			{
				Rule rule = rules[i];
				RuleProcess process = (i % 2 == 0 ? RuleProcess.Include : RuleProcess.Exclude);
				Assert.AreEqual(i.ToString(), rule.Value.ToString(), "The rule value wasn't parsed correctly.");
				Assert.AreEqual(process, rule.Process, "The rule process wasn't parsed correctly.");
				Assert.IsInstanceOfType(typeof(TestFalseRule), rule, "The rule type wasn't parsed correctly.");
			}
		}

		[Test(Description = "Ensures you can't parse a null configuration.")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void ParseConfiguration_NullConfiguration()
		{
			this.InvokeNonPublicStaticMethod("ParseConfiguration", new object[] { null });
		}

		[Test(Description = "Tests the yield of a configuration section with a single rule.")]
		public void ParseConfiguration_SingleRule()
		{
			UrlAbsolutifierSection config = new UrlAbsolutifierSection();
			config.Rules.Add(new RuleConfigurationElement(RuleProcess.Include, typeof(TestFalseRule), "value"));
			List<Rule> rules = this.InvokeNonPublicStaticMethod("ParseConfiguration", new object[] { config }) as List<Rule>;
			Assert.IsNotNull(rules, "The configuration should be parsed into a list of rules.");
			Assert.AreEqual(1, rules.Count, "The wrong number of rules was parsed.");
			Rule rule = rules[0];
			Assert.AreEqual("value", rule.Value.ToString(), "The rule value wasn't parsed correctly.");
			Assert.AreEqual(RuleProcess.Include, rule.Process, "The rule process wasn't parsed correctly.");
			Assert.IsInstanceOfType(typeof(TestFalseRule), rule, "The rule type wasn't parsed correctly.");
		}

		[Test(Description = "Checks the behavior of the system when all configured rules match.")]
		public void ResponseNeedsFilter_AllMatchingRules()
		{
			UrlAbsolutifierSection config = this.MockConfiguration();
			config.Rules.Add(new RuleConfigurationElement(RuleProcess.Include, typeof(TestTrueRule), "value"));
			config.Rules.Add(new RuleConfigurationElement(RuleProcess.Exclude, typeof(TestTrueRule), "value"));
			config.Rules.Add(new RuleConfigurationElement(RuleProcess.Include, typeof(TestTrueRule), "value"));
			this.InvokeNonPublicStaticMethod("InitializeRules", null);

			HttpContext context = this.CreateContext();
			bool filter = (bool)this.InvokeNonPublicStaticMethod("ResponseNeedsFilter", new object[] { context });
			Assert.IsTrue(filter, "If all rules match, the last matching rule should take precedence.");
		}

		[Test(Description = "Checks the behavior of the system when several rules match and contradict each other.")]
		public void ResponseNeedsFilter_ContradictoryRules()
		{
			UrlAbsolutifierSection config = this.MockConfiguration();
			config.Rules.Add(new RuleConfigurationElement(RuleProcess.Include, typeof(TestTrueRule), "value"));
			config.Rules.Add(new RuleConfigurationElement(RuleProcess.Exclude, typeof(TestFalseRule), "value"));
			config.Rules.Add(new RuleConfigurationElement(RuleProcess.Exclude, typeof(TestTrueRule), "value"));
			this.InvokeNonPublicStaticMethod("InitializeRules", null);

			HttpContext context = this.CreateContext();
			bool filter = (bool)this.InvokeNonPublicStaticMethod("ResponseNeedsFilter", new object[] { context });
			Assert.IsFalse(filter, "If several rules match, the last matching rule should take precedence.");
		}

		[Test(Description = "Checks the behavior of the system when an early rule in the chain matches and later rules don't.")]
		public void ResponseNeedsFilter_EarlyMatchingRule()
		{
			UrlAbsolutifierSection config = this.MockConfiguration();
			config.Rules.Add(new RuleConfigurationElement(RuleProcess.Include, typeof(TestTrueRule), "value"));
			config.Rules.Add(new RuleConfigurationElement(RuleProcess.Exclude, typeof(TestFalseRule), "value"));
			this.InvokeNonPublicStaticMethod("InitializeRules", null);

			HttpContext context = this.CreateContext();
			bool filter = (bool)this.InvokeNonPublicStaticMethod("ResponseNeedsFilter", new object[] { context });
			Assert.IsTrue(filter, "If only some of the rules match, the last matching rule should take precedence.");
		}

		[Test(Description = "Checks the behavior of the system when no configured rules match.")]
		public void ResponseNeedsFilter_NoMatchingRules()
		{
			UrlAbsolutifierSection config = this.MockConfiguration();
			config.Rules.Add(new RuleConfigurationElement(RuleProcess.Include, typeof(TestFalseRule), "value"));
			config.Rules.Add(new RuleConfigurationElement(RuleProcess.Exclude, typeof(TestFalseRule), "value"));
			config.Rules.Add(new RuleConfigurationElement(RuleProcess.Include, typeof(TestFalseRule), "value"));
			this.InvokeNonPublicStaticMethod("InitializeRules", null);

			HttpContext context = this.CreateContext();
			bool filter = (bool)this.InvokeNonPublicStaticMethod("ResponseNeedsFilter", new object[] { context });
			Assert.IsFalse(filter, "If there are no matching rules, the response should not be filtered.");
		}

		[Test(Description = "Checks the default behavior of the system when no rules are configured.")]
		public void ResponseNeedsFilter_NoRules()
		{
			UrlAbsolutifierSection config = this.MockConfiguration();
			this.InvokeNonPublicStaticMethod("InitializeRules", null);

			HttpContext context = this.CreateContext();
			bool filter = (bool)this.InvokeNonPublicStaticMethod("ResponseNeedsFilter", new object[] { context });
			Assert.IsFalse(filter, "The default behavior should be to not filter a response.");
		}

		[Test(Description = "Ensures you can't test a null context.")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void ResponseNeedsFilter_NullContext()
		{
			this.InvokeNonPublicStaticMethod("ResponseNeedsFilter", new object[] { null });
		}

		/// <summary>
		/// Creates a web context that can be used in testing.
		/// </summary>
		/// <returns>A slim, but valid, web context for testing.</returns>
		private HttpContext CreateContext()
		{
			HttpRequest request = new HttpRequest("foo.aspx", "http://localhost/foo.aspx", "");
			HttpResponse response = new HttpResponse(new System.IO.StreamWriter(new System.IO.MemoryStream()));
			HttpContext context = new HttpContext(request, response);
			context.Handler = new System.Web.UI.Page();
			return context;
		}

		/// <summary>
		/// Gets a non public static field from the module type.
		/// </summary>
		/// <param name="fieldName">Name of the field to get the value of.</param>
		/// <returns>The value of the field.</returns>
		private object GetNonPublicStaticField(string fieldName)
		{
			return typeof(SUT.UrlAbsolutifierModule).GetField(fieldName, BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic).GetValue(null);
		}

		/// <summary>
		/// Invokes a non public instance method on the test module instance.
		/// </summary>
		/// <param name="methodName">Name of the method to invoke.</param>
		/// <param name="args">Method arguments.</param>
		/// <returns>The output of the method call.</returns>
		private object InvokeNonPublicInstanceMethod(string methodName, object[] args)
		{
			try
			{
				return typeof(SUT.UrlAbsolutifierModule).InvokeMember(methodName, BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, this._module, args);
			}
			catch (TargetInvocationException err)
			{
				if (err.InnerException != null)
				{
					throw err.InnerException;
				}
				throw;
			}
		}

		/// <summary>
		/// Invokes a non public static method on the module type.
		/// </summary>
		/// <param name="methodName">Name of the method to invoke.</param>
		/// <param name="args">Method arguments.</param>
		/// <returns>The output of the method call.</returns>
		private object InvokeNonPublicStaticMethod(string methodName, object[] args)
		{
			try
			{
				return typeof(SUT.UrlAbsolutifierModule).InvokeMember(methodName, BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, this._module, args);
			}
			catch (TargetInvocationException err)
			{
				if (err.InnerException != null)
				{
					throw err.InnerException;
				}
				throw;
			}
		}

		/// <summary>
		/// Mocks the configuration.
		/// </summary>
		/// <returns>The mocked configuration section (so it can be further manipulated in tests).</returns>
		private UrlAbsolutifierSection MockConfiguration()
		{
			UrlAbsolutifierSection config = new UrlAbsolutifierSection();
			using (RecordExpectations recorder = RecorderManager.StartRecording())
			{
				object dummy = System.Configuration.ConfigurationManager.GetSection(null);
				recorder.WhenArgumentsMatch(SUT.UrlAbsolutifierModule.ConfigSectionId);
				recorder.Return(config);
				recorder.RepeatAlways();
			}
			return config;
		}

		/// <summary>
		/// Resets the module configuration.
		/// </summary>
		private void ResetModuleConfiguration()
		{
			typeof(SUT.UrlAbsolutifierModule).GetField("ConfiguredRules", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, null);
		}

		/// <summary>
		/// Simple test rule. Never matches a context.
		/// </summary>
		public class TestFalseRule : Rule
		{
			public TestFalseRule(RuleProcess process, string ruleValue) : base(process, ruleValue) { }

			public override System.ComponentModel.TypeConverter ValueConverter
			{
				get { return new System.ComponentModel.StringConverter(); }
			}

			public override bool ContextMatchesRule(HttpContext context)
			{
				return false;
			}
		}

		/// <summary>
		/// Simple test rule. Always matches a context.
		/// </summary>
		public class TestTrueRule : Rule
		{
			public TestTrueRule(RuleProcess process, string ruleValue) : base(process, ruleValue) { }

			public override System.ComponentModel.TypeConverter ValueConverter
			{
				get { return new System.ComponentModel.StringConverter(); }
			}

			public override bool ContextMatchesRule(HttpContext context)
			{
				return true;
			}
		}
	}
}
