﻿using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace Passcore
{
    [Activity(Label = "@string/app_name")]
    public class MainActivity : AppCompatActivity
    {
        int passLength = 16;
        int[,] LengthPool = new int[,] { { 16, 32, 64, 128, 256 }, { 4, 6, 8, 10, 12 } };
        bool ShortMode = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            string pass;

            this.Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);

            #region Initialize Activity
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            #endregion

            #region Initialize Widget
            EditText pKey_0 = FindViewById<EditText>(Resource.Id.pKey_0);
            EditText pKey_1 = FindViewById<EditText>(Resource.Id.pKey_1);
            EditText pKey_2 = FindViewById<EditText>(Resource.Id.pKey_2);
            SeekBar seekBar = FindViewById<SeekBar>(Resource.Id.seekBar);
            CheckBox isHard = FindViewById<CheckBox>(Resource.Id.isHard);
            CheckBox isShort = FindViewById<CheckBox>(Resource.Id.isShort);
            Button generate = FindViewById<Button>(Resource.Id.Generate);
            Button clean = FindViewById<Button>(Resource.Id.Clean);
            #endregion

            RefreshLengthDisplay(FindViewById<TextView>(Resource.Id.current_length));

            seekBar.Max = 4;
            seekBar.Min = 0;

            seekBar.ProgressChanged += (sender, e) =>
            {
                Seekbar2Length(seekBar);
            };

            generate.Click += (sender, e) =>
            {
                Core.Core core = new Core.Core();
                pass = core.EncryptMyPass(pKey_0.Text, pKey_1.Text, pKey_2.Text, isHard.Checked, passLength);
                if (pass != null && pass != string.Empty && pass != "")
                {
                    var alertDialog = new Android.App.AlertDialog.Builder(this).Create();
                    alertDialog.SetTitle(Resources.GetString(Resource.String.alert_title));
                    alertDialog.SetMessage(Resources.GetString(Resource.String.pass_front) + " " + pass);
                    alertDialog.SetButton(Resources.GetString(Resource.String.ok), (s, a) => { });
                    alertDialog.SetButton2(Resources.GetString(Resource.String.copy_to_clipboard), async (s, a) =>
                    {
                        await Clipboard.SetTextAsync(pass);
                        var alertDialog2 = new Android.App.AlertDialog.Builder(this).Create();
                        alertDialog2.SetTitle(Resources.GetString(Resource.String.alert_title));
                        alertDialog2.SetMessage(Resources.GetString(Resource.String.copy_success));
                        alertDialog2.SetButton(Resources.GetString(Resource.String.ok), (n, m) => { });
                        alertDialog2.Show();

                    });
                    alertDialog.Show();
                }
                else
                {
                    var alertDialog = new Android.App.AlertDialog.Builder(this).Create();
                    alertDialog.SetTitle(Resources.GetString(Resource.String.alert_title));
                    alertDialog.SetMessage(Resources.GetString(Resource.String.null_return));
                    alertDialog.SetButton(Resources.GetString(Resource.String.ok), (s, a) => { });
                    alertDialog.SetButton2(Resources.GetString(Resource.String.detail), (s, a) =>
                    {
                        var alertDialog2 = new Android.App.AlertDialog.Builder(this).Create();
                        alertDialog2.SetTitle(Resources.GetString(Resource.String.error_detail));
                        alertDialog2.SetMessage(Resources.GetString(Resource.String.null_return_code) + "\n" + Resources.GetString(Resource.String.null_return_disc));
                        alertDialog2.SetButton(Resources.GetString(Resource.String.ok), (n, m) => { });
                        alertDialog2.Show();

                    });
                    alertDialog.Show();
                }
            };

            clean.Click += (sender, e) => pKey_0.Text = pKey_1.Text = pKey_2.Text = string.Empty;

            isShort.CheckedChange += (sender, e) =>
            {
                if (isShort.Checked)
                {
                    var alertDialog = new Android.App.AlertDialog.Builder(this).Create();
                    alertDialog.SetTitle(Resources.GetString(Resource.String.warning));
                    alertDialog.SetMessage(Resources.GetString(Resource.String.lowlength_warning));
                    alertDialog.SetButton(Resources.GetString(Resource.String.confirm), (s, a) => 
                    {
                        ShortMode = true;

                    });
                    alertDialog.SetButton2(Resources.GetString(Resource.String.cancle), (s, a) =>
                    {
                        ShortMode = false;
                        isShort.Checked = false;
                    });
                    alertDialog.Show();
                    Seekbar2Length(seekBar);
                }
                else
                {
                    ShortMode = false;
                    Seekbar2Length(seekBar);
                }
            };

        }

        private void Seekbar2Length(SeekBar seekBar)
        {
            switch (seekBar.Progress)
            {
                case 0:
                    passLength = LengthPool[Convert.ToInt32(ShortMode), 0];
                    break;
                case 1:
                    passLength = LengthPool[Convert.ToInt32(ShortMode), 1];
                    break;
                case 2:
                    passLength = LengthPool[Convert.ToInt32(ShortMode), 2];
                    break;
                case 3:
                    passLength = LengthPool[Convert.ToInt32(ShortMode), 3];
                    break;
                case 4:
                    passLength = LengthPool[Convert.ToInt32(ShortMode), 4];
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Failed to build longer password.");
            }
            RefreshLengthDisplay(FindViewById<TextView>(Resource.Id.current_length));
        }

        private void RefreshLengthDisplay(TextView lengthView)
        {
            lengthView.Text = (Resources.GetString(Resource.String.length_display)) + passLength.ToString();
        }
    }
}