using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Bumptech.Glide;
using Iamnearby.Adapters;
using Iamnearby.Methods;
using Java.Lang;
using Newtonsoft.Json;
using PCL.Database;
using PCL.Models;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddServiceActivity : Activity, ITextWatcher
    {
        RecyclerView recyclerView, portfolio_recyclerView;
        RecyclerView.LayoutManager layoutManager, portfolio_layoutManager;
        RelativeLayout backRelativeLayout, ifEmptyServicesRL, ifEmptyPortfolioRL, addPhotoRL, addServiceRL, changeDescriptionRL, acceptPhotoRL, tintDelLL;

        ImageButton back_button;
        Button removeBn;
        EditText skill_descET;
        TextView your_city_valueTV, portfolioTV, servicesAndPricesTV, specialization_valueTV, changeDescriptionTV, acceptPhotoTV, cancelPhotoTV, noTV, yesTV;
        LinearLayout tintLL, gridLL, linearLut3111123;
        ProgressBar activityIndicatorDesc, activityIndicatorDel;
        ImageView chosenImageIV;
        static int service_cat_id_for_del;
        PictureMethods pictureMethods = new PictureMethods();
        PCL.HttpMethods.ProfileAndExpertMethods profileAndExpertMethodsPCL = new PCL.HttpMethods.ProfileAndExpertMethods();
        Methods.ProfileAndExpertMethods profileAndExpertMethods = new Methods.ProfileAndExpertMethods();

        UserMethods userMethods = new UserMethods();
        ISharedPreferences specialization_for_edit_pref = Application.Context.GetSharedPreferences("specialization_for_edit_pref", FileCreationMode.Private);
        static ISharedPreferencesEditor edit;
        bool went_for_image_to_another_app;
        protected override void OnResume()
        {
            base.OnResume();

            if (!went_for_image_to_another_app)
                try
                {
                    SetContentView(Resource.Layout.addService);
                    edit = specialization_for_edit_pref.Edit();
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                    removeBn = FindViewById<Button>(Resource.Id.removeBn);
                    backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                    ifEmptyServicesRL = FindViewById<RelativeLayout>(Resource.Id.ifEmptyServicesRL);
                    ifEmptyPortfolioRL = FindViewById<RelativeLayout>(Resource.Id.ifEmptyPortfolioRL);
                    addPhotoRL = FindViewById<RelativeLayout>(Resource.Id.addPhotoRL);
                    addServiceRL = FindViewById<RelativeLayout>(Resource.Id.addServiceRL);
                    acceptPhotoRL = FindViewById<RelativeLayout>(Resource.Id.acceptPhotoRL);
                    back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                    skill_descET = FindViewById<EditText>(Resource.Id.skill_descET);
                    portfolio_recyclerView = (RecyclerView)FindViewById(Resource.Id.portfolio_recyclerView);
                    servicesAndPricesTV = FindViewById<TextView>(Resource.Id.servicesAndPricesTV);
                    changeDescriptionTV = FindViewById<TextView>(Resource.Id.changeDescriptionTV);
                    tintLL = FindViewById<LinearLayout>(Resource.Id.tintLL);
                    gridLL = FindViewById<LinearLayout>(Resource.Id.gridLL);
                    tintDelLL = FindViewById<RelativeLayout>(Resource.Id.tintDelLL);
                    your_city_valueTV = FindViewById<TextView>(Resource.Id.your_city_valueTV);
                    specialization_valueTV = FindViewById<TextView>(Resource.Id.specialization_valueTV);
                    changeDescriptionRL = FindViewById<RelativeLayout>(Resource.Id.changeDescriptionRL);
                    chosenImageIV = FindViewById<ImageView>(Resource.Id.chosenImageIV);
                    acceptPhotoTV = FindViewById<TextView>(Resource.Id.acceptPhotoTV);
                    cancelPhotoTV = FindViewById<TextView>(Resource.Id.cancelPhotoTV);
                    portfolioTV = FindViewById<TextView>(Resource.Id.portfolioTV);
                    linearLut3111123 = FindViewById<LinearLayout>(Resource.Id.linearLut3111123);

                    noTV = FindViewById<TextView>(Resource.Id.noTV);
                    yesTV = FindViewById<TextView>(Resource.Id.yesTV);
                    activityIndicatorDesc = FindViewById<ProgressBar>(Resource.Id.activityIndicatorDesc);
                    activityIndicatorDel = FindViewById<ProgressBar>(Resource.Id.activityIndicatorDel);
                    activityIndicatorDesc.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                    activityIndicatorDel.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                    specialization_valueTV.Text = specialization_for_edit_pref.GetString("name", System.String.Empty);
                    skill_descET.Text = specialization_for_edit_pref.GetString("description", System.String.Empty);
                    skill_descET.AddTextChangedListener(this);
                    servicesAndPricesTV.Visibility = ViewStates.Gone;
                    Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                    FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                    removeBn.SetTypeface(tf, TypefaceStyle.Normal);
                    FindViewById<TextView>(Resource.Id.textView2).SetTypeface(tf, TypefaceStyle.Normal);
                    specialization_valueTV.SetTypeface(tf, TypefaceStyle.Normal);
                    skill_descET.SetTypeface(tf, TypefaceStyle.Normal);
                    changeDescriptionTV.SetTypeface(tf, TypefaceStyle.Normal);
                    FindViewById<TextView>(Resource.Id.textView2).SetTypeface(tf, TypefaceStyle.Normal);
                    FindViewById<TextView>(Resource.Id.textView3).SetTypeface(tf, TypefaceStyle.Normal);
                    servicesAndPricesTV.SetTypeface(tf, TypefaceStyle.Normal);
                    FindViewById<TextView>(Resource.Id.reviewCountTV).SetTypeface(tf, TypefaceStyle.Normal);
                    FindViewById<TextView>(Resource.Id.portfolio_titleTV).SetTypeface(tf, TypefaceStyle.Bold);
                    FindViewById<TextView>(Resource.Id.textView333).SetTypeface(tf, TypefaceStyle.Normal);
                    FindViewById<TextView>(Resource.Id.textView3).SetTypeface(tf, TypefaceStyle.Normal);
                    portfolioTV.SetTypeface(tf, TypefaceStyle.Normal);
                    FindViewById<TextView>(Resource.Id.reviewCountTV).SetTypeface(tf, TypefaceStyle.Normal);
                    acceptPhotoTV.SetTypeface(tf, TypefaceStyle.Normal);
                    cancelPhotoTV.SetTypeface(tf, TypefaceStyle.Normal);
                    FindViewById<TextView>(Resource.Id.textView1).SetTypeface(tf, TypefaceStyle.Normal);
                    FindViewById<TextView>(Resource.Id.textView5).SetTypeface(tf, TypefaceStyle.Normal);

                    FindViewById<TextView>(Resource.Id.textView22).SetTypeface(tf, TypefaceStyle.Bold);
                    your_city_valueTV.SetTypeface(tf, TypefaceStyle.Normal);

                    yesTV.SetTypeface(tf, TypefaceStyle.Normal);
                    noTV.SetTypeface(tf, TypefaceStyle.Normal);

                    portfolioTV.Visibility = ViewStates.Gone;

                    int categoryIndex = specialization_for_edit_pref.GetInt("categoryIndex", 0);
                    string user_data = specialization_for_edit_pref.GetString("user_data", System.String.Empty);
                    var deserialized_user_data = JsonConvert.DeserializeObject<UserProfile>(user_data);
                    var serv_temp_list = new List<ServiceData>();
                    var image_list = new List<string>();
                    if (deserialized_user_data.mainCategories != null)
                    {
                        if (deserialized_user_data.mainCategories[categoryIndex].subcategories != null)
                        {
                            foreach (var d in deserialized_user_data.mainCategories[categoryIndex].subcategories)
                            {
                                if (Convert.ToInt32(specialization_for_edit_pref.GetString("categoryId", "")) == Convert.ToInt32(d.categoryId))
                                {
                                    if (d.services != null)
                                        foreach (var serv in d.services)
                                        {
                                            serv_temp_list.Add(new ServiceData() { service_id = serv.service_id, name = serv.name, price = serv.price });
                                            servicesAndPricesTV.Visibility = ViewStates.Visible;
                                            ifEmptyServicesRL.Visibility = ViewStates.Gone;
                                        }
                                }
                            }
                        }
                    }
                    else
                        recyclerView.Visibility = ViewStates.Gone;

                    try
                    {
                        if (deserialized_user_data.mainCategories != null)
                        {
                            if (deserialized_user_data.mainCategories[categoryIndex].subcategories != null)
                            {
                                foreach (var d in deserialized_user_data.mainCategories[categoryIndex].subcategories)
                                {
                                    if (Convert.ToInt32(specialization_for_edit_pref.GetString("categoryId", "")) == Convert.ToInt32(d.categoryId))
                                    {
                                        //int i = 0;
                                        if (d.photos != null)
                                            foreach (var photo in d.photos)
                                            {
                                                //serv_temp_list.Add(new ServiceData() { service_id = serv.service_id, name = serv.name, price = serv.price });
                                                //servicesAndPricesTV.Visibility = ViewStates.Visible;
                                                //ifEmptyServicesRL.Visibility = ViewStates.Gone;
                                                var current_img = JsonConvert.DeserializeObject<CategoryImage>(photo.ToString());
                                                image_list.Add(current_img.imageUrl);
                                                //i++;
                                                //if (i == d.photos.Count)
                                                //    break;
                                            }
                                    }
                                }
                            }
                        }
                        else
                            recyclerView.Visibility = ViewStates.Gone;
                    }
                    catch { }
                    layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                    recyclerView.SetLayoutManager(layoutManager);
                    var services_and_pricesAdapter = new Services_and_pricesAdapter(serv_temp_list, this, tf);
                    recyclerView.SetAdapter(services_and_pricesAdapter);
                    var folder = Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "DCIM/Camera";
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    //var filesList 
                    List<string> filesList = Directory.GetFiles(folder).ToList<string>();
                    var reverseList = filesList.AsEnumerable().Reverse().ToList<string>();
                    reverseList.Insert(0, Resource.Drawable.open_camera.ToString());
                    reverseList.Insert(1, Resource.Drawable.open_gallery.ToString());

                    var gridview = FindViewById<GridView>(Resource.Id.gridview);
                    gridview.Adapter = new ImageAdapter(this, reverseList, Resources.DisplayMetrics.WidthPixels);

                    gridview.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
                    {
                        if (args.Position > 1)
                        {
                            Glide.With(Application.Context)
                                       .Load(reverseList[args.Position])
                                       //.Placeholder(Resource.Drawable.specialization_imageIV)
                                       .Into(chosenImageIV);
                            //new UserProfileAdapter().LoadImage(reverseList[args.Position], null, null);
                            //userProfileAdapter.NotifyDataSetChanged();
                            tintLL.Visibility = ViewStates.Gone;
                            gridLL.Visibility = ViewStates.Gone;
                            portfolioTV.Visibility = ViewStates.Gone;
                            portfolio_recyclerView.Visibility = ViewStates.Gone;
                            addPhotoRL.Visibility = ViewStates.Gone;
                            acceptPhotoRL.Visibility = ViewStates.Visible;
                        }
                        else if (args.Position == 0)
                        {
                            if (pictureMethods.IsThereAnAppToTakePictures(this))
                            {
                                PictureMethods.cameraOrGalleryIndicator = "camera";
                                pictureMethods.CreateDirectoryForPictures();
                                pictureMethods.TakeAPicture(this);
                            }
                        }
                        else if (args.Position == 1)
                        {
                            PictureMethods.cameraOrGalleryIndicator = "gallery";
                            var imageIntent = new Intent();
                            imageIntent.SetType("image/*");
                            imageIntent.SetAction(Intent.ActionGetContent);
                            StartActivityForResult(
                                Intent.CreateChooser(imageIntent, "Select photo"), 0);
                        }
                    };
                    addPhotoRL.Click += (s, e) =>
                    {
                        if (tintLL.Visibility == ViewStates.Visible)
                        {
                            tintLL.Visibility = ViewStates.Gone;
                            gridLL.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            tintLL.Visibility = ViewStates.Visible;
                            gridLL.Visibility = ViewStates.Visible;
                        }
                    };
                    tintLL.Click += (s, e) =>
                    {
                        tintLL.Visibility = ViewStates.Gone;
                        gridLL.Visibility = ViewStates.Gone;
                    };

                    portfolio_layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                    portfolio_recyclerView.SetLayoutManager(portfolio_layoutManager);
                    var portfolioAdapter = new PortfolioAdapter(image_list, this);
                    portfolio_recyclerView.SetAdapter(portfolioAdapter);
                    if (image_list.Count > 0)
                    {
                        ifEmptyPortfolioRL.Visibility = ViewStates.Gone;
                        linearLut3111123.Visibility = ViewStates.Visible;
                        portfolioTV.Visibility = ViewStates.Visible;
                        portfolio_recyclerView.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        linearLut3111123.Visibility = ViewStates.Gone;
                        ifEmptyPortfolioRL.Visibility = ViewStates.Visible;
                    }

                    backRelativeLayout.Click += (s, e) =>
                    {
                        OnBackPressed();
                    };
                    back_button.Click += (s, e) =>
                    {
                        OnBackPressed();
                    };

                    skill_descET.EditorAction += (object sender, EditText.EditorActionEventArgs e) =>
                    {
                        imm.HideSoftInputFromWindow(skill_descET.WindowToken, 0);
                    };
                    addServiceRL.Click += (s, e) =>
                      {
                          StartActivity(typeof(ServiceActivity));
                      };
                    skill_descET.TextChanged += (s, e) =>
                      {
                          changeDescriptionRL.Visibility = ViewStates.Visible;
                          changeDescriptionTV.Visibility = ViewStates.Visible;
                          activityIndicatorDesc.Visibility = ViewStates.Gone;
                      };
                    changeDescriptionRL.Click += async (s, e) =>
                      {
                          changeDescriptionTV.Visibility = ViewStates.Visible;
                          activityIndicatorDesc.Visibility = ViewStates.Gone;
                          var res = await profileAndExpertMethods.EditSpecializationDescription(userMethods.GetUsersAuthToken(), specialization_for_edit_pref.GetString("categoryId", System.String.Empty), specialization_for_edit_pref.GetInt("categoryIndex", 0), skill_descET.Text);
                          edit.PutString("description", skill_descET.Text);
                          edit.Apply();
                          var res_reload = await ReloadData();
                          changeDescriptionTV.Visibility = ViewStates.Gone;
                          activityIndicatorDesc.Visibility = ViewStates.Visible;
                          changeDescriptionRL.Visibility = ViewStates.Gone;

                          var intent = new Intent(this, typeof(AddServiceActivity));
                          intent.SetFlags(ActivityFlags.NewTask);
                          StartActivity(intent);
                          Finish();
                      };
                    cancelPhotoTV.Click += (s, e) =>
                     {
                         portfolioTV.Visibility = ViewStates.Visible;
                         portfolio_recyclerView.Visibility = ViewStates.Visible;
                         addPhotoRL.Visibility = ViewStates.Visible;
                         acceptPhotoRL.Visibility = ViewStates.Gone;
                     };
                    acceptPhotoTV.Click += async (s, e) =>
                      {
                          Bitmap bitmap = ((BitmapDrawable)chosenImageIV.Drawable).Bitmap;
                          MemoryStream memStream = new MemoryStream();
                          bitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, memStream);
                          byte[] ba = memStream.ToArray();
                          string base64 = Base64.EncodeToString(ba, Base64.Default);
                          activityIndicatorDesc.Visibility = ViewStates.Visible;
                          acceptPhotoTV.Visibility = ViewStates.Gone;
                          cancelPhotoTV.Visibility = ViewStates.Gone;
                          chosenImageIV.Visibility = ViewStates.Gone;
                          var temp_photoList = new List<string>();
                          temp_photoList.Add(base64);
                          var res = await profileAndExpertMethods.AddSpecializationPhotos(userMethods.GetUsersAuthToken(), specialization_for_edit_pref.GetString("categoryId", System.String.Empty), specialization_for_edit_pref.GetInt("categoryIndex", 0), temp_photoList);
                          activityIndicatorDesc.Visibility = ViewStates.Visible;
                          acceptPhotoTV.Visibility = ViewStates.Gone;
                          cancelPhotoTV.Visibility = ViewStates.Gone;
                          chosenImageIV.Visibility = ViewStates.Gone;
                          var res_reload = await ReloadData();
                          var intent = new Intent(this, typeof(AddServiceActivity));
                          intent.SetFlags(ActivityFlags.NewTask);
                          StartActivity(intent);
                          Finish();
                      };

                    removeBn.Click += (s, e) =>
                      {
                          tintDelLL.Visibility = ViewStates.Visible;
                          your_city_valueTV.Text = specialization_for_edit_pref.GetString("name", System.String.Empty);

                      };
                    tintDelLL.Click += (s, e) =>
                      {
                          tintDelLL.Visibility = ViewStates.Gone;
                      };

                    yesTV.Click += (s, e) =>
                    {
                        activityIndicatorDel.Visibility = ViewStates.Visible;
                        var res = profileAndExpertMethods.DeleteSpecialization(userMethods.GetUsersAuthToken(), specialization_for_edit_pref.GetString("categoryId", System.String.Empty));
                        System.Threading.Thread.Sleep(100);
                        tintDelLL.Visibility = ViewStates.Gone;
                        StartActivity(typeof(UserProfileActivity));
                    };

                    noTV.Click += (s, e) =>
                    {
                        tintDelLL.Visibility = ViewStates.Gone;
                    };
                }
                catch (System.Exception ex)
                {
                    StartActivity(typeof(MainActivity));
                }
        }

        public async Task<string> ReloadData()
        {
            var user_data = await profileAndExpertMethodsPCL.UserProfileData(userMethods.GetUsersAuthToken());
            if (user_data == "401")
            {
                Toast.MakeText(this, Resource.String.you_not_logined, ToastLength.Long).Show();
                userMethods.ClearTable();
                userMethods.ClearUsersDataTable();
                userMethods.ClearTableNotif();
                StartActivity(typeof(MainActivity));
            }
            userMethods.InsertProfileData(user_data);
            edit.PutString("user_data", user_data);
            edit.Apply();

            return user_data;
        }

        public override void OnBackPressed()
        {
            try
            {
                if (tintLL.Visibility == ViewStates.Visible)
                {
                    tintLL.Visibility = ViewStates.Gone;
                    gridLL.Visibility = ViewStates.Gone;
                }
                else
                    base.OnBackPressed();
            }
            catch { base.OnBackPressed(); }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            went_for_image_to_another_app = true;
            tintLL.Visibility = ViewStates.Gone;
            gridLL.Visibility = ViewStates.Gone;
            if (PictureMethods.cameraOrGalleryIndicator == "gallery")
            {
                if (resultCode == Result.Ok)
                {
                    chosenImageIV.SetImageURI(data.Data);
                    tintLL.Visibility = ViewStates.Gone;
                    gridLL.Visibility = ViewStates.Gone;
                    portfolioTV.Visibility = ViewStates.Gone;
                    portfolio_recyclerView.Visibility = ViewStates.Gone;
                    addPhotoRL.Visibility = ViewStates.Gone;
                    acceptPhotoRL.Visibility = ViewStates.Visible;
                }
            }
            else if (PictureMethods.cameraOrGalleryIndicator == "camera")
            {
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);



                // Display in ImageView. We will resize the bitmap to fit the display.
                // Loading the full sized image will consume to much memory
                // and cause the application to crash.
                //var imageView =
                //        FindViewById<ImageView>(RecyclerViewSample.Resource.Id.myImageView);
                int height = Resources.DisplayMetrics.HeightPixels;
                int width = Resources.DisplayMetrics.WidthPixels;//profile_image.Height;
                App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
                if (App.bitmap != null)
                {
                    tintLL.Visibility = ViewStates.Gone;
                    gridLL.Visibility = ViewStates.Gone;
                    portfolioTV.Visibility = ViewStates.Gone;
                    portfolio_recyclerView.Visibility = ViewStates.Gone;
                    addPhotoRL.Visibility = ViewStates.Gone;
                    acceptPhotoRL.Visibility = ViewStates.Visible;
                    chosenImageIV.SetImageBitmap(App.bitmap);
                    App.bitmap = null;
                }

                // Dispose of the Java side bitmap.
                GC.Collect();
            }
        }

        public void AfterTextChanged(IEditable s)
        {
            //throw new NotImplementedException();
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
            //throw new NotImplementedException();
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            if (skill_descET.LineCount == 3)
            {
                skill_descET.Text = skill_descET.Text.Remove(skill_descET.Text.Length - 1);
                skill_descET.SetSelection(skill_descET.Text.Length);
                Toast toast = Toast.MakeText(this, GetString(Resource.String.description_cannot_occupy_more_than_two_lines), ToastLength.Short);
                toast.SetGravity(Gravity.GetAbsoluteGravity(GravityFlags.Top, GravityFlags.Center), 0, 150);
                toast.Show();
            }
        }
    }
}