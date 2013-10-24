using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Razor
{
    public class HtmlHelper
    {
        SimpleTemplateBase ParentView;

        public string Partial(string viewPath)
        {
            Type partialViewType = new RazorViews().GetViewType(viewPath);
            var instance = (SimpleTemplateBase)Activator.CreateInstance(partialViewType);

            // Make the partial view inherit the model, the handler and the ViewBag
            instance.SetHandler(ParentView.Handler);
            instance.SetModel(ParentView.Model);
            instance.SetViewBag(ParentView.ViewBag);
            instance.Render();

            // Return the output
            return instance.Output;
        }

        public HtmlHelper(SimpleTemplateBase view)
        {
            ParentView = view;
        }
    }
}
