# UrlAbsolutifierModule
An ASP.NET 2.0 HttpModule for filtering response output and converting URL references from relative to absolute.

In certain cases, like syndicated content, it's necessary for URL references to be converted from relative (`/foo/bar.html`) to absolute (`http://myserver/foo/bar.html`).  This module allows an application to be configured such that selected response output will be processed to convert relative URLs to absolute URLs.

# Installation and Usage

Detailed usage is included in the API documentation and an implementation can be seen in the included demo site.  On a high level, you need to:

1. Add the `Paraesthesia.Web.UrlAbsolutifierModule` to the `httpModules` section of your `web.config` file.
2. Add the custom `urlabsolutifier` configuration section _definition_ to the `configSections` section of your `web.config` file.
3. Add the custom `urlabsolutifier` configuration section to your `web.config` file along with the rule configuration defining which responses should be filtered.

That's it - once it's configured, you're good to go.

# Configuring for Subtext

Using this with my SubText blog RSS feeds was the primary reason for making it. If you pass your RSS through FeedBurner, relative URLs end up looking like they come from the FeedBurner server, not from your blog.  Add the following to your SubText `web.config` to get it working.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section
      name="urlabsolutifier"
      type="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlAbsolutifierSection, Paraesthesia.Web.UrlAbsolutifierModule"/>
  </configSections>
  <urlabsolutifier>
    <add
      process="Include"
      type="Paraesthesia.Web.Configuration.UrlAbsolutifier.HandlerTypeRule, Paraesthesia.Web.UrlAbsolutifierModule"
      value="Subtext.Framework.Syndication.RssHandler, Subtext.Framework" />
    <add
      process="Include"
      type="Paraesthesia.Web.Configuration.UrlAbsolutifier.HandlerTypeRule, Paraesthesia.Web.UrlAbsolutifierModule"
      value="Subtext.Framework.Syndication.AtomHandler, Subtext.Framework" />
  </urlabsolutifier>
</configuration>
```

# Demo

A demo web site is included with the source.  Open the solution in Visual Studio and browse to the various pages in the demo site to see the filter in action.

# Known Issues

- The content type of the response is not checked before the filter is applied. Be careful not to configure the filter such that it incorrectly modifies binary output.
- The filter is aggressive - it doesn't check to see if the HTML attribute and URL value it's filtering is in a code snippet, or is encoded, or is just a little fragment in the middle of the content body.  You may come across a place where the filter incorrectly converts a relative URL to absolute.