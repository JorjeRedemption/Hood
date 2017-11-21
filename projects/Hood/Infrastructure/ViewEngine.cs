using Hood.Extensions;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Hood.Services
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        private IEnumerable<string> locs;
        private ISettingsRepository _settings;

        public ViewLocationExpander()
        {
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            _settings = (ISettingsRepository)context.ActionContext.HttpContext.RequestServices.GetService(typeof(ISettingsRepository));
            string theme = _settings != null ? _settings["Hood.Settings.Theme"] : null;
            var temp = new List<string>();
            if (theme.IsSet())
                temp.AddRange(GetLocations("/Themes/" + theme + "/Areas/{2}"));
            temp.AddRange(GetLocations("/Areas/{2}"));
            temp.AddRange(GetLocations("/Core/Areas/{2}"));
            if (theme.IsSet())
                temp.AddRange(GetLocations("/Themes/" + theme));
            temp.AddRange(GetLocations(""));
            temp.AddRange(GetLocations("/Core"));
            locs = temp.AsEnumerable();
            context.Values["theme"] = theme;
        }

        protected string[] GetLocations(string baseLocation)
        {
            return new[]
            {
                baseLocation + "/Views/{0}.cshtml",
                baseLocation + "/Views/{1}/{0}.cshtml",
                baseLocation + "/Views/{1}/Partials/{0}.cshtml",
                baseLocation + "/Views/Layouts/{1}/{0}.cshtml",
                baseLocation + "/Views/Layouts/{0}.cshtml",
                baseLocation + "/Views/Shared/{1}/{0}.cshtml",
                baseLocation + "/Views/Shared/{0}.cshtml",
                baseLocation + "/Views/Templates/{0}.cshtml",
                baseLocation + "/Views/Components/{1}/{0}.cshtml",
                baseLocation + "/Views/Components/{0}.cshtml"
            };
        }

        public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return locs;
        }
    }
}
