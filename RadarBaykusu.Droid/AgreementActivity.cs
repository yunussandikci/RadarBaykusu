using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLitePCL;
using RadarBaykusu.Core.Model;
using RadarBaykusu.Core;

namespace RadarBaykusu.Droid
{
    [Activity(Label = "AgreementActivity", NoHistory = true)]
    public class AgreementActivity : Activity
    {
        #region Local Declerations

        private Button Agree;
        private Button Disagree;
        private string dbPath;
        private DBOperations dbOperations;

        #endregion

        #region Activity Methods

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Agreement);

            //sqlite connection icin baglanti cumlersini olustur
           dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            //platform bilgisini al ve connection objesini olustur
            dbOperations = new DBOperations(dbPath);

            FindViews();
        }

        #endregion

        #region Other Methods

        private void FindViews()
        {
            Agree = FindViewById<Button>(Resource.Id.agree);
            Disagree = FindViewById<Button>(Resource.Id.disagree);
            Agree.Click += Agree_Click;
            Disagree.Click += Disagree_Click;
        }

        private void Disagree_Click(object sender, EventArgs e)
        {
            this.FinishAffinity();
        }

        private void Agree_Click(object sender, EventArgs e)
        {
            var agreement = dbOperations.GetConfiguration("isTermsAccepted").ReturnObject;
            agreement.ParamValue = "true";
            var updateResult = dbOperations.Update(agreement, typeof(Configuration));

            var intent = new Intent();
            intent.SetClass(this, typeof(MainActivity));
            StartActivity(intent);
        }

        #endregion
    }
}