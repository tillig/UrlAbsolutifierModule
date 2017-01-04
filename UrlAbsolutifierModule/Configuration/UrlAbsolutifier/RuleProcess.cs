using System;

namespace Paraesthesia.Web.Configuration.UrlAbsolutifier
{
	/// <summary>
	/// Indicates how a rule match should cause a request to be handled.
	/// </summary>
	public enum RuleProcess
	{
		/// <summary>
		/// Include items matching this rule.
		/// </summary>
		Include,

		/// <summary>
		/// Exclude items matching this rule.
		/// </summary>
		Exclude
	}
}
