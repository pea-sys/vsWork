using Bunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vsWork.Shared;
using Xunit;

namespace vsWork.Test.Shared
{
    public class ClockTest
    {
        string fontSize = "10";
        string stringColor = "#0000ff";

        [Fact(DisplayName = "日時がリアルタイム表示されること")]
        public void RealTimeDisp()
        {
            using var ctx = new TestContext();
            var component = ctx.RenderComponent<Clock>();

            var oldFontElmText = DateTime.Parse(component.Find("font").TextContent);
            System.Threading.Thread.Sleep(2000);
            var newFontElmText = DateTime.Parse(component.Find("font").TextContent);
            TimeSpan ts = newFontElmText - oldFontElmText;
            // 正確な時差は取得できる保証がないので時間が進んでいたらOK
            Assert.True(ts.TotalSeconds > 0);
        }

        [Fact(DisplayName = "FontSizeパラメータが有効であること")]
        public void ParameterFontSize()
        {
            using var ctx = new TestContext();
            var component = ctx.RenderComponent<Clock>();
            fontSize = "1";
            component.SetParametersAndRender(parameters => parameters.Add(p => p.FontSize, fontSize));
            Assert.True(component.Find("font").GetAttribute("size") == $"{fontSize}px");

            fontSize = "99";
            component.SetParametersAndRender(parameters => parameters.Add(p => p.FontSize, fontSize));
            Assert.True(component.Find("font").GetAttribute("size") == $"{fontSize}px");
        }
        [Fact(DisplayName = "StringColorパラメータが有効であること")]
        public void ParameterStringColor()
        {
            using var ctx = new TestContext();
            var component = ctx.RenderComponent<Clock>();
            stringColor = "Red";
            component.SetParametersAndRender(parameters => parameters.Add(p => p.StringColor, stringColor));
            Assert.True(component.Find("font").GetAttribute("color") == $"{stringColor}");

            stringColor = "#0000ff";
            component.SetParametersAndRender(parameters => parameters.Add(p => p.StringColor, stringColor));
            Assert.True(component.Find("font").GetAttribute("color") == $"{stringColor}");
        }
        [Fact(DisplayName ="Disposeが有効であること")]
        public void DisposedTimer()
        {
            using var ctx = new TestContext();
            var component = ctx.RenderComponent<Clock>();
            component.Dispose();
            Assert.True(component.IsDisposed);
        }
    }
}
