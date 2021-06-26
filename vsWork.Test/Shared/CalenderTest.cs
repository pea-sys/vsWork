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
    public class CalenderTest
    {
        [Fact(DisplayName = "初期選択が現在年月日であること")]
        public void InitialSelectedDate()
        {
            using var ctx = new TestContext();
            var component = ctx.RenderComponent<Calender>();
            var ElmText = DateTime.Parse(component.Find("caption").TextContent.Replace("◀","").Replace("▶", ""));
            DateTime now = DateTime.Now;
            Assert.True((now.Year == ElmText.Year) && (now.Month == ElmText.Month) && (now.Day == ElmText.Day));
        }
        [Fact(DisplayName ="月戻りと月送りボタンが有効であること")]
        public void NextMonthAndPreviousMonth()
        {
            using var ctx = new TestContext();
            var component = ctx.RenderComponent<Calender>();
            var ElmText = DateTime.Parse(component.Find("caption").TextContent.Replace("◀", "").Replace("▶", ""));
            var buttons = component.FindAll("button");
            foreach (var btn in buttons)
            {
                if (btn.Id == "previous_month")
                {
                    btn.Click();
                    var NextElmText = DateTime.Parse(component.Find("caption").TextContent.Replace("◀", "").Replace("▶", ""));
                    ElmText = ElmText.AddMonths(-1);

                    Assert.True((NextElmText.Year == ElmText.Year) && (NextElmText.Month == ElmText.Month) && (NextElmText.Day == ElmText.Day));
                }
                else if (btn.Id == "next_month")
                {
                    btn.Click();
                    var NextElmText = DateTime.Parse(component.Find("caption").TextContent.Replace("◀", "").Replace("▶", ""));
                    ElmText = ElmText.AddMonths(1);

                    Assert.True((NextElmText.Year == ElmText.Year) && (NextElmText.Month == ElmText.Month) && (NextElmText.Day == ElmText.Day));
                }
            }
        }
        [Fact(DisplayName = "日付クリックで選択日付が更新されること")]
        public void DayClickAndView()
        {
            using var ctx = new TestContext();
            var component = ctx.RenderComponent<Calender>();
            DateTime dateTime = new DateTime(2020, 1, 1);

            var clickButton = component.FindAll("button").Where(p => p.ClassList.Contains("saturday")).FirstOrDefault();
            clickButton.Click();
            var NextElmText = component.Instance.SelectedDay.Day.ToString();
            Assert.True(NextElmText == clickButton.TextContent);
        }
    }
}
