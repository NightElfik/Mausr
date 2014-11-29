using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Mausr.Web {
	public static class HtmlHelperExtensions {

		public static MvcHtmlString ModelValueHiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
				Expression<Func<TModel, TProperty>> expression) {

			var value = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;
			return htmlHelper.HiddenFor(expression, new { value = value.ToString() });
		}
	}
}