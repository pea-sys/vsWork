using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsWork.Test.Mock
{
    internal class FakeNavigationManager : NavigationManager
    {
        public FakeNavigationManager()
        {
            Initialize("https://unit-test.example/", "https://unit-test.example/");
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            NotifyLocationChanged(false);
        }
    }
}
