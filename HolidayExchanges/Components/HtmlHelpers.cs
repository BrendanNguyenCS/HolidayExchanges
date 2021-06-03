using HolidayExchanges.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

/// <summary>
/// Custom HTML helper extension methods class
/// </summary>
public static class HtmlHelpers
{
    /// <summary>
    /// A custom image creation helper
    /// </summary>
    /// <param name="helper">The current <see cref="HtmlHelper"/> instance.</param>
    /// <param name="src">The image source url/path.</param>
    /// <param name="id">The image identifier.</param>
    /// <param name="class">The image CSS/Bootstrap classes.</param>
    /// <param name="altText">The image alternate text.</param>
    /// <param name="imageWidth">The width of the image, in pixels.</param>
    /// <param name="imageHeight">The height of the image, in pixels.</param>
    /// <param name="htmlAttributes">An additional html attributes for the image.</param>
    /// <returns>An image</returns>
    public static MvcHtmlString Image(this HtmlHelper helper, string src, string id, string @class, string altText, int? imageWidth, int? imageHeight, object htmlAttributes = null)
    {
        var builder = new TagBuilder("img");
        builder.MergeAttribute("src", src);
        builder.MergeAttribute("alt", altText);
        builder.AddCssClass(@class);
        builder.MergeAttribute("id", id);
        if (imageHeight != null)
            builder.MergeAttribute("width", imageWidth.ToString());
        if (imageHeight != null)
            builder.MergeAttribute("height", imageHeight.ToString());
        builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
    }

    /// <summary>
    /// A custom AJAX action link that can display html.
    /// </summary>
    /// <param name="ajaxHelper">The current <see cref="AjaxHelper"/> instance.</param>
    /// <param name="linkText">The link text.</param>
    /// <param name="actionName">The target action name for the link.</param>
    /// <param name="controllerName">The target controller name for the link.</param>
    /// <param name="routeValues">The target route values for the link.</param>
    /// <param name="ajaxOptions">Additional AJAX options for the link.</param>
    /// <param name="htmlAttributes">Additional HTML attributes for the link.</param>
    /// <returns>An actionlink</returns>
    public static MvcHtmlString ActionLinkAjax(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes = null)
    {
        var repID = Guid.NewGuid().ToString();
        var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, ajaxOptions, htmlAttributes);
        return MvcHtmlString.Create(lnk.ToString().Replace(repID, linkText));
    }

    /// <summary>
    /// A custom helper that outputs a nicely-formatted address element.
    /// </summary>
    /// <typeparam name="TModel">Generic model class.</typeparam>
    /// <typeparam name="TProperty">Generic model property.</typeparam>
    /// <param name="helper">The current <see cref="HtmlHelper"/> instance.</param>
    /// <param name="expression">LINQ expression that selects the viewmodel properties.</param>
    /// <param name="isEditable">Boolean to indicate if a textbox or display area is needed.</param>
    /// <param name="htmlAttributes">Additional HTML attributes for the address element.</param>
    /// <returns>An address</returns>
    public static MvcHtmlString DisplayAddressFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, bool isEditable = false, object htmlAttributes = null)
    {
        var valueGetter = expression.Compile();
        var model = valueGetter(helper.ViewData.Model) as User;
        var sb = new List<string>();

        if (model != null)
        {
            if (!string.IsNullOrEmpty(model.Address1))
                sb.Add(model.Address1);
            if (!string.IsNullOrEmpty(model.Address2))
                sb.Add(model.Address2);
            if (!string.IsNullOrEmpty(model.City) || !string.IsNullOrEmpty(model.State) || !string.IsNullOrEmpty(model.Zip))
                sb.Add(string.Format("{0}, {1} {2}", model.City, model.State, model.Zip));
            if (!string.IsNullOrEmpty(model.Country))
                sb.Add(model.Country);
        }

        var delimiter = (isEditable) ? Environment.NewLine : "<br />";
        var addr = (isEditable) ? new TagBuilder("textarea") : new TagBuilder("address");
        addr.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        addr.InnerHtml = string.Join(delimiter, sb.ToArray());
        return MvcHtmlString.Create(addr.ToString());
    }

    /// <summary>
    /// A custom HTML action link that can be clicked to sort.
    /// </summary>
    /// <param name="helper">The current <see cref="HtmlHelper"/> instance.</param>
    /// <param name="linkText">The link text.</param>
    /// <param name="actionName">The target action name for the link.</param>
    /// <param name="sortField">The target controller name for the link.</param>
    /// <param name="currentSort">The current sorting type.</param>
    /// <param name="currentDesc">The current sorting direction.</param>
    /// <param name="htmlAttributes">Additional HTML attributes for the element.</param>
    /// <returns>An actionlink with an icon within</returns>
    public static MvcHtmlString ActionLinkSortable(this HtmlHelper helper, string linkText, string actionName, string sortField, string currentSort, object currentDesc, object htmlAttributes = null)
    {
        bool desc = (currentDesc == null) ? false : Convert.ToBoolean(currentDesc);
        // get link route values
        var routeValues = new RouteValueDictionary();
        routeValues.Add("id", sortField);
        routeValues.Add("desc", (currentSort == sortField) && !desc);
        // build the tag
        if (currentSort == sortField)
            linkText = string.Format(" {0} <span class='badge'><i class='fas fa-sort-amount-down-alt'></i></span>", linkText, (desc) ? "-alt" : "");
        TagBuilder tagBuilder = new TagBuilder("a");
        tagBuilder.InnerHtml = linkText;
        // add url to the link
        var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
        var url = urlHelper.Action(actionName, routeValues);
        tagBuilder.MergeAttribute("href", url);
        tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        // put it all together
        return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    /// <summary>
    /// A custom file selector helper.
    /// </summary>
    /// <param name="helper">The current <see cref="HtmlHelper"/> instance.</param>
    /// <param name="id">The file input identifier.</param>
    /// <param name="class">The file selector CSS/Bootstrap classes.</param>
    /// <param name="htmlAttributes">Additional HTML attributes for the file input.</param>
    /// <returns>A file input area.</returns>
    public static MvcHtmlString File(this HtmlHelper helper, string id, string @class, object htmlAttributes = null)
    {
        TagBuilder builder = new TagBuilder("input");
        builder.MergeAttribute("type", "file");
        builder.AddCssClass("form-control");
        builder.AddCssClass(@class);
        builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
    }

    /// <summary>
    /// A custom date picker input field.
    /// </summary>
    /// <param name="helper">The current <see cref="HtmlHelper"/> instance.</param>
    /// <param name="name">The name of the date picker.</param>
    /// <param name="value">Optional value of the date picker.</param>
    /// <param name="htmlAttributes">Additional HTML attributes for the element.</param>
    /// <returns>An input field (date picker with UI)</returns>
    public static MvcHtmlString DatePicker(this HtmlHelper helper, string name, DateTime value, object htmlAttributes = null)
    {
        TagBuilder builder = new TagBuilder("input");
        builder.MergeAttribute("name", name);
        builder.AddCssClass("form-control");
        builder.GenerateId(name);
        if (value != null)
            builder.MergeAttribute("value", value.ToString("yyyy-MM-dd"));
        else
            builder.MergeAttribute("value", String.Empty);

        builder.MergeAttribute("type", "date");
        builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
    }

    /// <summary>
    /// A custom time picker input field.
    /// </summary>
    /// <param name="helper">The current <see cref="HtmlHelper"/> instance.</param>
    /// <param name="name">The name of the time picker.</param>
    /// <param name="value">Optional value of the time picker.</param>
    /// <param name="htmlAttributes">Additional HTML attributes for the element.</param>
    /// <returns>An input field (time picker with UI)</returns>
    public static MvcHtmlString TimePicker(this HtmlHelper helper, string name, DateTime value, object htmlAttributes = null)
    {
        TagBuilder builder = new TagBuilder("input");
        builder.MergeAttribute("name", name);
        builder.AddCssClass("form-control");
        builder.GenerateId(name);
        if (value != null)
            builder.MergeAttribute("value", value.ToString("h:mmttTK"));
        else
            builder.MergeAttribute("value", String.Empty);

        builder.MergeAttribute("type", "time");
        builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
    }

    /// <summary>
    /// A custom helper for redirect links in forms (mainly for the forgot password page).
    /// </summary>
    /// <param name="helper">The current <see cref="HtmlHelper"/> instance.</param>
    /// <param name="text">The link text.</param>
    /// <param name="actionName">The target action name for the link.</param>
    /// <param name="controllerName">The target controller name for the link.</param>
    /// <param name="htmlAttributes">Additional HTML attributes for the element.</param>
    /// <returns>A redirect link within a paragraph element.</returns>
    public static MvcHtmlString FormHelpTextLink(this HtmlHelper helper, string text, string actionName, string controllerName, object htmlAttributes = null)
    {
        TagBuilder outer = new TagBuilder("p");
        outer.AddCssClass("form-text");
        outer.AddCssClass("text-muted");

        TagBuilder inner = new TagBuilder("a");
        inner.MergeAttribute("style", "text-decoration: none;");
        var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
        var url = urlHelper.Action(actionName, controllerName);
        inner.MergeAttribute("href", url);
        inner.InnerHtml = text;
        outer.InnerHtml = inner.ToString(TagRenderMode.Normal);
        outer.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        return MvcHtmlString.Create(outer.ToString(TagRenderMode.Normal));
    }

    /// <summary>
    /// A custom helper for redirect links in forms (mainly for the forgot password page).
    /// </summary>
    /// <param name="helper">The current <see cref="HtmlHelper"/> instance.</param>
    /// <param name="linktext">The link text.</param>
    /// <param name="text">The other text text.</param>
    /// <param name="url">The redirect url.</param>
    /// <param name="innerhtmlAttributes">Additional HTML attributes for the inner link element.</param>
    /// <param name="outerhtmlAttributes">Additional HTML attributes for the outer paragraph element.</param>
    /// <returns>A redirect link within a paragraph element.</returns>
    public static MvcHtmlString FormHelpTextLink(this HtmlHelper helper, string text, string linktext, string url, object innerhtmlAttributes = null, object outerhtmlAttributes = null)
    {
        TagBuilder outer = new TagBuilder("p");
        outer.AddCssClass("form-text");
        outer.AddCssClass("text-muted");

        TagBuilder inner = new TagBuilder("a");
        inner.MergeAttribute("style", "text-decoration: none;");
        inner.MergeAttribute("href", url);
        inner.SetInnerText(linktext);
        inner.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(innerhtmlAttributes));
        outer.InnerHtml = inner.ToString(TagRenderMode.Normal);
        outer.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(outerhtmlAttributes));
        return MvcHtmlString.Create(outer.ToString(TagRenderMode.Normal));
    }

    /// <summary>
    /// A custom helper for a link.
    /// </summary>
    /// <param name="helper">The current <see cref="HtmlHelper"/> instance.</param>
    /// <param name="text">The link text.</param>
    /// <param name="url">The redirect url.</param>
    /// <param name="htmlAttributes">Additional HTML attributes for the element.</param>
    /// <returns>A link.</returns>
    public static MvcHtmlString Link(this HtmlHelper helper, string text, string url, object htmlAttributes = null)
    {
        TagBuilder builder = new TagBuilder("a");
        builder.AddCssClass("text-decoration-none");
        builder.SetInnerText(text);
        builder.MergeAttribute("href", url);
        builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
    }

    /// <summary>
    /// A custom helper for a link.
    /// </summary>
    /// <param name="helper">The current <see cref="HtmlHelper"/> instance.</param>
    /// <param name="text">The link text.</param>
    /// <param name="controllerName">The targeted controller.</param>
    /// <param name="actionName">The targeted action within the targeted controller.</param>
    /// <param name="htmlAttributes">Additional HTML attributes for the element.</param>
    /// <returns>A link.</returns>
    public static MvcHtmlString Link(this HtmlHelper helper, string text, string controllerName, string actionName, object htmlAttributes = null)
    {
        TagBuilder builder = new TagBuilder("a");
        builder.AddCssClass("text-decoration-none");
        builder.SetInnerText(text);
        var urlHelper = new UrlHelper();
        var url = urlHelper.Action(actionName, controllerName);
        builder.MergeAttribute("href", url);
        builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
    }

    /// <summary>
    /// A custom helper for a email link.
    /// </summary>
    /// <param name="helper">The current <see cref="HtmlHelper"/> instance.</param>
    /// <param name="email">The receiving email address</param>
    /// <param name="text">The link inner text.</param>
    /// <param name="cssClasses">Any CSS/Bootstrap classes.</param>
    /// <param name="htmlAttributes">Additional HTML attributes for the link element.</param>
    /// <returns></returns>
    public static MvcHtmlString EmailLink(this HtmlHelper helper, string email, string text = null, string cssClasses = null, object htmlAttributes = null)
    {
        TagBuilder builder = new TagBuilder("a");
        if (!string.IsNullOrEmpty(cssClasses))
            builder.AddCssClass(cssClasses);
        builder.SetInnerText(text);
        builder.MergeAttribute("href", "mailto:" + email);
        builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
    }
}