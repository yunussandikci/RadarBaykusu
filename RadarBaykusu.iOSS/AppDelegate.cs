using Foundation;
using RadarBaykusu.Core;
using System;
using System.Threading;
using UIKit;
using Google.Maps;

namespace RadarBaykusu.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        private UIWindow window;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var enterTime = DateTime.Now;
            UIApplication.SharedApplication.IdleTimerDisabled = true;
            string dbLocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library"); // Documents folder
            DBOperations dbOperations = new DBOperations(dbLocation);
            MapServices.ProvideAPIKey("AIzaSyDYxUpL02VClQcWYgsjFBhyAJUousAJqIs");

            //DB Klasörü Yoksa Olustur
            if (!System.IO.Directory.Exists(dbLocation))
                System.IO.Directory.CreateDirectory(dbLocation);

            //Connection Objesini
            //DB'yi check et boşsa oluştur ve doldur
            CommonMethods.DBCheck(dbOperations.DatabaseLocation);

            //result agreement kabulunu gösterir
            var result = dbOperations.GetConfiguration("isTermsAccepted");
            window = new UIWindow(UIScreen.MainScreen.Bounds);
            if (result != null && result.Result)
            {
                if (result.ReturnObject.ParamValue == "true")
                {
                    window.RootViewController = new UINavigationController(new MasterController());
                }
                else
                {
                    window.RootViewController = new AgreementController(window);
                }
            }
            else
            {
                window.RootViewController = new AgreementController(window);
            }
            window.MakeKeyAndVisible();
            var endTimeResult = DateTime.Now;
            var timeResult = endTimeResult - enterTime;
            if (timeResult.Seconds < 5)
            {
                Thread.Sleep((5 - timeResult.Seconds) * 1000);
            }
            return true;
        }
    }
}