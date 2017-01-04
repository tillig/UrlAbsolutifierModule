using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Paraesthesia.Web.Configuration.UrlAbsolutifier;

namespace Paraesthesia.Web
{
	/// <summary>
	/// Rewrites HTML output to convert relative URL references to absolute URLs.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This module checks each request against a configured set of rules and, if
	/// the request matches the rules, appends a filter to the response output
	/// to convert all URLs in the output to absolute format.
	/// </para>
	/// <para>
	/// To use this module, you need to register it in <c>web.config</c> in the
	/// <c>httpModules</c> section as well as configuring rules.
	/// </para>
	/// <para>
	/// The rules that determine which responses to update get configured in
	/// <c>web.config</c> in a section called <c>urlabsolutifier</c> that gets
	/// parsed as a <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlAbsolutifierSection"/>.
	/// An example of the configuration can be seen below.
	/// </para>
	/// <para>
	/// If a request matches the configured rules such that it is set to be
	/// processed, the <see cref="System.Web.HttpResponse.Filter"/> for the current
	/// response is set to be a <see cref="Paraesthesia.Web.UrlAbsolutifierFilter"/>.
	/// The <see cref="Paraesthesia.Web.UrlAbsolutifierFilter"/> is responsible
	/// for parsing through the response stream and converting relative URLs to
	/// absolute.
	/// </para>
	/// <note type="caution">
	/// No consideration is given to the content type of the response being filtered.
	/// It doesn't look to see if it's an HTML or text response; it very well
	/// could attempt to filter binary output if the rules match the request.
	/// Given that, it is important that the configuration is set up correctly
	/// so the module will only affect the proper responses.
	/// </note>
	/// </remarks>
	/// <example>
	/// <para>
	/// The following shows an example abbreviated <c>web.config</c>.
	/// </para>
	/// <code>
	/// &lt;?xml version="1.0"?&gt;
	/// &lt;configuration&gt;
	///   &lt;configSections&gt; 
	///     &lt;section name="urlabsolutifier" type="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlAbsolutifierSection, Paraesthesia.Web.UrlAbsolutifierModule" /&gt; 
	///   &lt;/configSections&gt; 
	///   &lt;urlabsolutifier&gt;
	///     &lt;add process="Include" type="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlRegexRule, Paraesthesia.Web.UrlAbsolutifierModule" value="[a-z]{6}\.aspx" /&gt;
	///     &lt;add process="Exclude" type="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlRegexRule, Paraesthesia.Web.UrlAbsolutifierModule" value="abcdef.aspx" /&gt;
	///     &lt;add process="Include" type="Paraesthesia.Web.Configuration.UrlAbsolutifier.HandlerTypeRule, Paraesthesia.Web.UrlAbsolutifierModule" value="System.Web.UI.Page, System.Web" /&gt;
	///     &lt;add process="Exclude" type="Paraesthesia.Web.Configuration.UrlAbsolutifier.HandlerTypeRule, Paraesthesia.Web.UrlAbsolutifierModule" value="MyCustomHandler, MyAssembly" /&gt;
	///   &lt;/urlabsolutifier&gt;
	///   &lt;system.web&gt;
	///     &lt;httpModules&gt;
	///       &lt;add name="urlabsolutifier" type="Paraesthesia.Web.UrlAbsolutifierModule, Paraesthesia.Web.UrlAbsolutifierModule"/&gt;
	///     &lt;/httpModules&gt;
	///   &lt;/system.web&gt;
	/// &lt;/configuration&gt;
	/// </code>
	/// </example>
	/// <seealso cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlAbsolutifierSection"/>
	/// <seealso cref="Paraesthesia.Web.UrlAbsolutifierFilter"/>
	public sealed class UrlAbsolutifierModule : IHttpModule
	{
		/*
		 * Idea for future development: Rather than have a module that is single-
		 * purposed to add just the one filter type, create a more general purpose
		 * module that not only processes rules to determine matches but also have
		 * each rule determine which filter to apply.  It'd be more of a generic
		 * output filter module than it would be specifically for URL fixup.
		 */

		/// <summary>
		/// Name of the configuration section used to retrieve settings for this module.
		/// Expected to be of type <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlAbsolutifierSection"/>
		/// </summary>
		public const string ConfigSectionId = "urlabsolutifier";

		/// <summary>
		/// Object used for threadsafety/locking of configuration.
		/// </summary>
		private static readonly object ConfigurationSyncRoot = new object();

		/// <summary>
		/// The complete set of configured rules that should be matched to determine
		/// if a response gets processed.
		/// </summary>
		private static List<Rule> ConfiguredRules = null;

		/// <summary>
		/// Handles the <see cref="System.Web.HttpApplication.PostRequestHandlerExecute"/> event
		/// and appends the <see cref="Paraesthesia.Web.UrlAbsolutifierFilter"/>
		/// to the response.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An <see cref="System.EventArgs" /> that contains the event data.</param>
		/// <remarks>
		/// <para>
		/// If the current context (<see cref="System.Web.HttpContext.Current"/>)
		/// needs to be filtered (<see cref="Paraesthesia.Web.UrlAbsolutifierModule.ResponseNeedsFilter"/>)
		/// then a new <see cref="Paraesthesia.Web.UrlAbsolutifierFilter"/> is
		/// appended to the current response.
		/// </para>
		/// </remarks>
		private void AppendResponseFilter(object sender, EventArgs e)
		{
			if (ResponseNeedsFilter(HttpContext.Current))
			{
				HttpContext.Current.Response.Filter = new UrlAbsolutifierFilter(HttpContext.Current.Response.Filter, HttpContext.Current.Request.Url);
			}
		}

		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that
		/// implements <see cref="System.Web.IHttpModule"/>.
		/// </summary>
		public void Dispose()
		{
			// Nothing to do - only implemented for the interface.
		}

		/// <summary>
		/// Retrieves the rule configuration information.
		/// </summary>
		/// <returns>
		/// A <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlAbsolutifierSection"/>
		/// with the rule configuration.
		/// </returns>
		/// <exception cref="System.Configuration.ConfigurationErrorsException">
		/// Thrown if the configuration information isn't found.
		/// </exception>
		private static UrlAbsolutifierSection GetConfiguration()
		{
			UrlAbsolutifierSection configuration = ConfigurationManager.GetSection(ConfigSectionId) as UrlAbsolutifierSection;
			if (configuration == null)
			{
				throw new ConfigurationErrorsException("In order to use the UrlAbsolutifierModule, you neeed a configuration UrlAbsolutifierSection called '" + ConfigSectionId + "'.");
			}
			return configuration;
		}

		/// <summary>
		/// Initializes a module and prepares it to handle requests.
		/// </summary>
		/// <param name="context">
		/// An <see cref="System.Web.HttpApplication"/> that provides access to
		/// the methods, properties, and events common to all application objects
		/// within an ASP.NET application
		/// </param>
		/// <remarks>
		/// <para>
		/// Subscribes to the <see cref="System.Web.HttpApplication.PostRequestHandlerExecute"/>
		/// event to append the processing filter if necessary.
		/// </para>
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="context" /> is <see langword="null" />.
		/// </exception>
		public void Init(HttpApplication context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			InitializeRules();
			context.PostRequestHandlerExecute += new EventHandler(AppendResponseFilter);
		}

		/// <summary>
		/// Initializes the set of rules that will be used to determine which
		/// responses get processed.
		/// </summary>
		private static void InitializeRules()
		{
			if (ConfiguredRules != null)
			{
				return;
			}
			lock (ConfigurationSyncRoot)
			{
				if (ConfiguredRules != null)
				{
					return;
				}
				UrlAbsolutifierSection configuration = GetConfiguration();
				ConfiguredRules = ParseConfiguration(configuration);
			}
		}

		/// <summary>
		/// Parses rule configuration information into a collection of rules.
		/// </summary>
		/// <param name="configuration">The <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlAbsolutifierSection"/> to parse.</param>
		/// <returns>
		/// A <see cref="System.Collections.Generic.List{T}"/> of <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule"/>
		/// objects that determine if a response should be processed.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="configuration" /> is <see langword="null" />.
		/// </exception>
		private static List<Rule> ParseConfiguration(UrlAbsolutifierSection configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			List<Rule> rules = new List<Rule>();
			foreach (RuleConfigurationElement configuredRule in configuration.Rules)
			{
				Rule rule = (Rule)Activator.CreateInstance(configuredRule.Type, configuredRule.Process, configuredRule.Value);
				rules.Add(rule);
			}
			return rules;
		}

		/// <summary>
		/// Determines if the set of configured rules require that the current
		/// response should be processed.
		/// </summary>
		/// <param name="context">The context to check against the rules.</param>
		/// <returns>
		/// <see langword="true" /> if the current response should be processed
		/// for URL absolutification; <see langword="false" /> if not.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="context" /> is <see langword="null" />.
		/// </exception>
		private static bool ResponseNeedsFilter(HttpContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			bool include = false;
			foreach (Rule rule in ConfiguredRules)
			{
				if (!rule.ContextMatchesRule(context))
				{
					continue;
				}
				include = rule.Process == RuleProcess.Include;
			}
			return include;
		}
	}
}
