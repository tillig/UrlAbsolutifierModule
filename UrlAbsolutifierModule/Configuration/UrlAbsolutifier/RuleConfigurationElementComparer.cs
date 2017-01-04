using System;
using System.Collections;

namespace Paraesthesia.Web.Configuration.UrlAbsolutifier
{
	/// <summary>
	/// Compares two <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElement"/>
	/// objects to determine if they are equal.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class is used in conjunction with <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationCollection"/>
	/// to determine when rule configuration settings are equal - this allows elements
	/// to properly be added and removed.
	/// </para>
	/// </remarks>
	/// <seealso cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationCollection"/>
	public class RuleConfigurationElementComparer : IComparer
	{
		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <description>Condition</description>
		/// </listheader>
		/// <item>
		/// <term>Less than zero</term>
		/// <description><paramref name="x" /> is less than <paramref name="y" /></description>
		/// </item>
		/// <item>
		/// <term>Zero</term>
		/// <description><paramref name="x" /> equals <paramref name="y" /></description>
		/// </item>
		/// <item>
		/// <term>Greater than zero</term>
		/// <description><paramref name="x" /> is greater than <paramref name="y" /></description>
		/// </item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// <para>
		/// Objects are compared in the following property order:
		/// </para>
		/// <list type="number">
		/// <item>
		/// <term><see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElement.Type"/></term>
		/// </item>
		/// <item>
		/// <term><see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElement.Value"/></term>
		/// </item>
		/// <item>
		/// <term><see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElement.Process"/></term>
		/// </item>
		/// </list>
		/// </remarks>
		/// <exception cref="System.ArgumentException">
		/// Neither <paramref name="x" /> nor <paramref name="y" /> implements the
		/// <see cref="System.IComparable"/> interface -or- <paramref name="x" />
		/// and <paramref name="y" /> are of different types and neither one can
		/// handle comparisons with the other.
		/// </exception>
		public int Compare(object x, object y)
		{
			RuleConfigurationElement xConverted = x as RuleConfigurationElement;
			RuleConfigurationElement yConverted = y as RuleConfigurationElement;

			if (x != null && xConverted == null)
			{
				throw new ArgumentException("x must be a RuleConfigurationElement", "x");
			}
			if (y != null && yConverted == null)
			{
				throw new ArgumentException("y must be a RuleConfigurationElement", "y");
			}

			if (xConverted == yConverted)
			{
				return 0;
			}
			if (xConverted == null)
			{
				return -1;
			}
			if (yConverted == null)
			{
				return 1;
			}

			if (xConverted.Type != yConverted.Type)
			{
				return xConverted.Type.FullName.CompareTo(yConverted.Type.FullName);
			}
			
			int result = xConverted.Value.CompareTo(yConverted.Value);
			if (result != 0)
			{
				return result;
			}
			
			result = xConverted.Process.CompareTo(yConverted.Process);
			return result;
		}
	}
}
