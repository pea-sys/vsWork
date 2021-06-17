using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsWork.Test.Mock
{
    public class FakeNavigationManager : NavigationManager
    {
        private readonly ITestRenderer renderer;

        public FakeNavigationManager(ITestRenderer renderer)
        {
            Initialize("http://localhost/", "http://localhost/");
            this.renderer = renderer;
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            Uri = ToAbsoluteUri(uri).ToString();

            renderer.Dispatcher.InvokeAsync(
                () => NotifyLocationChanged(isInterceptedLink: false));
        }
    }
}
