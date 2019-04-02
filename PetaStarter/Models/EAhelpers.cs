﻿using Speedbird;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Speedbird
{
    public static class EAHTMLHelpers
    {
        public static MvcHtmlString EAIdFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool HasId)
        {            
            return (HasId)? html.HiddenFor(expression):MvcHtmlString.Empty ;
        }
        

        /// <summary>
        /// Fetch the std html for a form control
        /// </summary>
        /// <param name="labelFor">Label of your form</param>
        /// <param name="inputBoxFor">The main input control</param>
        /// <param name="validationMessageFor">When used with a bound model sets the validation from Metadata</param>
        /// <param name="editorClass">Additional css class for main inputbox. Usually mb-3</param>
        /// <returns></returns>
        private static string FetchStdFormWrappers(MvcHtmlString inputBoxFor, MvcHtmlString validationMessageFor, string label = "", string editorClass="mb-3")
        {                       
            var innerSpanWrapper = new TagBuilder("span");
            innerSpanWrapper.AddCssClass("input-group-text");
            innerSpanWrapper.InnerHtml = label;

            var innerDivWrapper = new TagBuilder("div");
            innerDivWrapper.AddCssClass("input-group-prepend");
            innerDivWrapper.InnerHtml = innerSpanWrapper.ToString();

            var wrapperTagBuilder = new TagBuilder("div");
            wrapperTagBuilder.AddCssClass("input-group col " + editorClass);
            wrapperTagBuilder.InnerHtml = innerDivWrapper + inputBoxFor.ToHtmlString() + validationMessageFor?.ToHtmlString();
            return wrapperTagBuilder.ToString();
        }

        //Concatenate MVCHtmlstrings
        public static MvcHtmlString Concat(this MvcHtmlString first, params MvcHtmlString[] strings)
        {
            return MvcHtmlString.Create(first.ToString() + string.Concat(strings.Select(s => s.ToString())));
        }

        private static MvcHtmlString TextBound<TModel, TValue>(HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, ref string label)
        {
            if (label.Length == 0)
                label = MyExtensions.CamelToSpaceString(html.DisplayNameFor(expression).ToString());
            var validationMessageFor = html.ValidationMessageFor(expression);
            return validationMessageFor;
        }

        private static string TextUnBound(string id, string label)
        {
            if (label.Length == 0)
                label = MyExtensions.CamelToSpaceString(id);
            return label;
        }

        //Text box
        public static MvcHtmlString EATextFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string label="",  string editorClass="")
        {
            MvcHtmlString validationMessageFor = TextBound(html, expression, ref label);
            var textBoxFor = html.TextBoxFor(expression, new { @type = "text", @class = "form-control" });

            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, validationMessageFor, label, editorClass));
        }

        public static MvcHtmlString EAText(this HtmlHelper html, string id, string editorClass="", string label = "")
        {
            var textBoxFor = html.TextBox(id, "", new { @type = "text", @class = "form-control" });
            label = TextUnBound(id, label);
            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, MvcHtmlString.Empty, label, editorClass));
        }


        //Text Area
        public static MvcHtmlString EATextAreaFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string label = "", string editorClass = "")
        {
            MvcHtmlString validationMessageFor = TextBound(html, expression, ref label);            
            var textBoxFor = html.TextAreaFor(expression, new { @type = "text", @class = "form-control" });

            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, validationMessageFor, label, editorClass));
        }

        public static MvcHtmlString EATextArea(this HtmlHelper html, string id, string editorClass = "", string label = "")
        {
            var textBoxFor = html.TextArea(id, "", new { @type = "text", @class = "form-control" });
            label = TextUnBound(id, label);
            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, MvcHtmlString.Empty, label, editorClass));
        }

        //Text Numeric
        public static MvcHtmlString EANumberFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string label = "", string editorClass = "")
        {
            MvcHtmlString validationMessageFor = TextBound(html, expression, ref label);
            var textBoxFor = html.TextBoxFor(expression, new { @type = "number", @class = "form-control" });

            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, validationMessageFor, label, editorClass));
        }

        public static MvcHtmlString EANumber(this HtmlHelper html, string id, string editorClass = "", string label = "")
        {
            var textBoxFor = html.TextBox(id, "", new { @type = "number", @class = "form-control" });
            label = TextUnBound(id, label);
            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, MvcHtmlString.Empty, label, editorClass));
        }

        //Text Date
        public static MvcHtmlString EADateFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string label = "", string editorClass = "")
        {
            MvcHtmlString validationMessageFor = TextBound(html, expression, ref label);
            var textBoxFor = html.TextBoxFor(expression, new { @type = "text", @class = "form-control eadate" });

            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, validationMessageFor, label, editorClass));
        }

        public static MvcHtmlString EADate(this HtmlHelper html, string id, string editorClass = "", string label = "")
        {
            var textBoxFor = html.TextBox(id, "", new { @type = "text", @class = "form-control eadate" });
            label = TextUnBound(id, label);
            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, MvcHtmlString.Empty, label, editorClass));
        }

        //Select List
        public static MvcHtmlString EASelectListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string label = "", string editorClass = "")
        {
            MvcHtmlString validationMessageFor = TextBound(html, expression, ref label);
            var textBoxFor = html.DropDownListFor(expression,null, new {  @class = "form-control" });

            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, validationMessageFor, label, editorClass));
        }

        public static MvcHtmlString EASelectList(this HtmlHelper html, string id,IEnumerable<SelectListItem> List, string editorClass = "", string label = "")
        {
            var textBoxFor = html.DropDownList(id, List, new { @type = "text", @class = "form-control eadate" });
            label = TextUnBound(id, label);
            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, MvcHtmlString.Empty, label, editorClass));
        }

        //Check Box
        public static MvcHtmlString EAChkBoxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, bool>> expression, string label = "", string editorClass = "")
        {
            MvcHtmlString validationMessageFor = TextBound(html, expression, ref label);
            var textBoxFor = html.CheckBoxFor(expression, new { @class = "form-control" });

            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, validationMessageFor, label, editorClass));
        }

        public static MvcHtmlString EAChkBox(this HtmlHelper html, string id, string editorClass = "", string label = "")
        {
            var textBoxFor = html.CheckBox(id, new {  @class = "form-control" });
            label = TextUnBound(id, label);
            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, MvcHtmlString.Empty, label, editorClass));
        }

        //public static MvcHtmlString EASelectListFromSQL(this HtmlHelper html, string id,Repository db, string sql, string editorClass = "", string label = "")
        //{
        //    var eList = db.Query<EASelectListData>(sql).ToList().Select(u => new SelectListItem
        //    {
        //       Text = u.id,
        //       Value = u.value
        //    });
        //    var textBoxFor = html.DropDownList(id, eList, new { @type = "text", @class = "form-control eadate" });
        //    label = TextUnBound(id, label);
        //    return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, MvcHtmlString.Empty, label, editorClass));
        //}

        //Read at https://blogs.msdn.microsoft.com/stuartleeks/2012/04/23/asp-net-mvc-jquery-ui-autocomplete/
        //@Html.LabelFor(m=>m.SomeValue) 
        //@Html.AutocompleteFor(m=>m.SomeValue, “Autocomplete”, “Home”)
        public static MvcHtmlString EAAutocompleteFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string controllerName, string actionName, string label = "", string value="", string editorClass = "")
        {
            string autocompleteUrl = UrlHelper.GenerateUrl(null, actionName, controllerName,null,html.RouteCollection,html.ViewContext.RequestContext,includeImplicitMvcValues: true);
            string boxName = html.DisplayNameFor(expression).ToString();

            MvcHtmlString validationMessageFor = TextBound(html, expression, ref label);
            validationMessageFor= validationMessageFor.Concat(html.HiddenFor(expression));

            var textBoxFor= html.TextBox(boxName + "Txt", value ?? "", new { @type = "text", @class = "form-control" , data_autocombo_url = autocompleteUrl, @data_autocomplete_idholder = boxName  });

            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, validationMessageFor, label, editorClass));
        }

        public static MvcHtmlString EASpacer(this HtmlHelper html, string editorClass = "", string label = "")
        {
            var textBoxFor = html.Label( "", new { @type = "text", @class = "form-control" });
            label = TextUnBound("", label);
            return new MvcHtmlString(FetchStdFormWrappers(textBoxFor, MvcHtmlString.Empty, label, editorClass));
        }

        public static IHtmlString File(this HtmlHelper helper, string id)
        {
            TagBuilder tb = new TagBuilder("input");
            tb.Attributes.Add("type", "file");
            tb.Attributes.Add("id", id);
            return new MvcHtmlString(tb.ToString());
        }
    }
    
}