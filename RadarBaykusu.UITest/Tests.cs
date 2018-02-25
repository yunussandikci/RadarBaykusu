using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using System.Diagnostics;
using System.Threading;

namespace RadarBaykusu.UITest
{
    [TestFixture(Platform.Android)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp.Android
        .ApkFile(@"..\..\..\RadarBaykusu.Droid\bin\Release\com.pergamon.radarbaykusu.apk")
        .PreferIdeSettings()
        .EnableLocalScreenshots()
        .StartApp();
        }

        [Test]
        public void RadarbaykusuTest()
        {
            Func<AppQuery, AppQuery> AgreeButtonQuery = e => e.Id("agree");
            Func<AppQuery, AppQuery> BinekButtonQuery = e => e.Id("BinekButton");
            Func<AppQuery, AppQuery> CustomPanelQuery = e => e.Id("customPanel");
            app.WaitForElement(AgreeButtonQuery, "Timed out oldu");
            app.Tap(AgreeButtonQuery);

            app.WaitForElement(BinekButtonQuery, "Timed out oldu");
            app.Tap(BinekButtonQuery);

            app.WaitForElement(CustomPanelQuery, "Timed out oldu", new TimeSpan(0, 0, 0, 90, 0));
            app.Device.SetLocation(38.272664, 27.217156);
            app.Device.SetLocation(38.262033, 27.221531);
            app.Device.SetLocation(38.181769, 27.311198);
            app.Device.SetLocation(38.194008, 27.337692);



        }

    }
}

