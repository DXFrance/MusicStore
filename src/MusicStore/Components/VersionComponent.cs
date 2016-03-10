using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Components
{
    [ViewComponent(Name = "Version")]
    public class VersionComponent : ViewComponent
    {
        private string applicationVersion;

        public VersionComponent(IApplicationEnvironment applicationEnvironment)
        {
            this.applicationVersion = applicationEnvironment.ApplicationVersion;
        }

        public IViewComponentResult Invoke()
        {
            return View("Default", this.applicationVersion);
        }
    }
}
