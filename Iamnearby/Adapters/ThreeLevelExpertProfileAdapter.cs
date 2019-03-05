using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Iamnearby.Activities;
using Iamnearby.ExpandableRecyclerClasses;
using Iamnearby.Interfaces;
using Newtonsoft.Json;
using PCL.Models;

namespace Iamnearby.Adapters
{
    public class ThreeLevelExpertProfileAdapter : RecyclerView.Adapter
    {
        public const string TAG = "InvoiceItemAdapterPS";
        ISharedPreferences dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_dialog;
        public static List<IExpertProfileDataType> listItems;
        private Context context;
        private Activity _context;
        Typeface tf;
        private LayoutInflater inflater;
        public event EventHandler<int> GroupClick;
        public event EventHandler<int> GroupClick2;
        public event EventHandler<int> BttGroupClick;
        public event EventHandler<int> BttEntryClick;
        //RecyclerView.LayoutManager layoutManager;
        RecyclerView.LayoutManager portfolio_layoutManager;
        public static EntryHolder holderEntry;

        public override long GetItemId(int position)
        {
            return position;
        }

        public ThreeLevelExpertProfileAdapter(Context context, Activity _context, Typeface tf, List<IExpertProfileDataType> listItems)
        {
            this.context = context;
            this._context = _context;
            this.tf = tf;
            ThreeLevelExpertProfileAdapter.listItems = listItems;
            this.inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
        }

        void GroupOnClick(int position)
        {
            if (GroupClick != null)
                GroupClick(this, position);
        }
        void BackClick(int position)
        {
            _context.OnBackPressed();
        }
        void AboutExpertInfoClick(int position)
        {
            _context.StartActivity(typeof(AboutExpertActivity));
        }
        void FeedbackClick(int position)
        {
            _context.StartActivity(typeof(FeedbackActivity));
        }
        void FeedbackListClick(int position)
        {
            _context.StartActivity(typeof(ReviewsListActivity));
        }
        void CallClick(int position)
        {
            IExpertProfileDataType itemNode = listItems[0];

            ExpertGroupData group = itemNode as ExpertGroupData;
            if (!String.IsNullOrEmpty(group.expert_phone))
            {
                var uri = Android.Net.Uri.Parse("tel:" + group.expert_phone);
                var intent = new Intent(Intent.ActionDial, uri);
                _context.StartActivity(intent);
            }
            else
                Toast.MakeText(_context, Resource.String.number_not_specified, ToastLength.Short).Show();
        }
        void WriteClick(int position)
        {
            IExpertProfileDataType itemNode = listItems[0];

            ExpertGroupData group = itemNode as ExpertGroupData;
            edit_dialog = dialog_data.Edit();
            edit_dialog.PutString("expert_id", group.expertId);
            edit_dialog.PutString("expert_name", group.expertName);
            edit_dialog.PutString("expert_avatar", group.profile_image);
            edit_dialog.Apply();
            _context.StartActivity(typeof(DialogActivity));
        }
        void GroupOnClick2(int position)
        {
            if (GroupClick2 != null)
                GroupClick2(this, position);
        }

        void BttGroupOnClick(int position)
        {
            if (BttGroupClick != null)
                BttGroupClick(this, position);
        }

        void BttEntryOnClick(int position)
        {
            if (BttEntryClick != null)
                BttEntryClick(this, position);
        }

        public override int ItemCount
        {
            get
            {
                return listItems.Count;
            }
        }

        public override int GetItemViewType(int position)
        {
            return listItems[position].GetItemType();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            IExpertProfileDataType itemNode = listItems[position];

            if (itemNode.GetItemType() == 0)
            {
                ExpertGroupData group = itemNode as ExpertGroupData;
                GroupExpertThreeLevelHolder groupHolder = holder as GroupExpertThreeLevelHolder;


                if (position == 0)
                {
                    AboutExpertActivity.InfoText = group.aboutExpertInfo;
                    groupHolder.headerRL.Visibility = ViewStates.Visible;
                    groupHolder.itemRL.Visibility = ViewStates.Gone;

                    groupHolder.expertNameTV.SetTypeface(tf, TypefaceStyle.Bold);
                    groupHolder.expertSurnameTV.SetTypeface(tf, TypefaceStyle.Bold);
                    groupHolder.expert_phoneTV.SetTypeface(tf, TypefaceStyle.Normal);
                    groupHolder.aboutExpertTitle.SetTypeface(tf, TypefaceStyle.Bold);
                    groupHolder.aboutExpertInfo.SetTypeface(tf, TypefaceStyle.Normal);
                    groupHolder.writeTV.SetTypeface(tf, TypefaceStyle.Bold);
                    groupHolder.callTV.SetTypeface(tf, TypefaceStyle.Bold);
                    groupHolder.cityTV.SetTypeface(tf, TypefaceStyle.Normal);
                    groupHolder.nowTitle.SetTypeface(tf, TypefaceStyle.Bold);
                    groupHolder.distanceTV.SetTypeface(tf, TypefaceStyle.Normal);
                    groupHolder.rating_valueTV.SetTypeface(tf, TypefaceStyle.Bold);
                    groupHolder.expert_phoneTV.Text = group.expert_phone;

                    char[] chars = { ' ' };
                    string[] split = group.expertName.Split(chars);
                    try
                    {
                        string name = split[0];
                        string middlename = split[1];
                        groupHolder.expertNameTV.Text = name + " " + middlename;
                        groupHolder.expertSurnameTV.Text = split[2];
                    }
                    catch
                    {
                        groupHolder.expertNameTV.Text = group.expertName;
                        groupHolder.expertNameTV.Visibility = ViewStates.Visible;
                    }
                    Thread backgroundThread = new Thread(new ThreadStart(() =>
                    {
                        Glide.Get(Application.Context).ClearDiskCache();
                    }));
                    backgroundThread.IsBackground = true;
                    backgroundThread.Start();
                    Glide.Get(context).ClearMemory();
                    Glide.With(Application.Context)
                        .Load("https://api.iamnearby.net/" + group.profile_image)
                        .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                        .SkipMemoryCache(true))
                        //.Placeholder(Resource.Drawable.specialization_imageIV)
                        .Into(groupHolder.profile_image);

                    groupHolder.cityTV.Text = group.city;
                    if (group.online)
                        groupHolder.onlineIV.Visibility = ViewStates.Visible;
                    else
                        groupHolder.onlineIV.Visibility = ViewStates.Gone;
                    double distance_in_km;
                    bool distance_in_km_parsed = Double.TryParse(group.distance, out distance_in_km);
                    if (distance_in_km > 999)
                    {
                        distance_in_km = distance_in_km / 1000;
                        var final_dist = Math.Round(distance_in_km, 2).ToString().Replace(',', '.');
                        groupHolder.distanceTV.Text = final_dist + " км";
                    }
                    else
                        groupHolder.distanceTV.Text = group.distance + " м";
                    if (!String.IsNullOrEmpty(group.aboutExpertInfo))
                    {
                        groupHolder.aboutExpertRL.Visibility = ViewStates.Visible;
                        groupHolder.aboutExpertInfo.Text = group.aboutExpertInfo;
                    }
                    else
                        groupHolder.aboutExpertRL.Visibility = ViewStates.Gone;

                    groupHolder.rating_valueTV.Text = "(" + group.rating_value.ToString() + ")";
                    double rating_value = 0;
                    try
                    {
                        rating_value = Convert.ToDouble(group.rating_value, (CultureInfo.InvariantCulture));
                        if (rating_value >= 1)
                        {
                            groupHolder.star1IV.SetBackgroundResource(Resource.Drawable.active_star);
                            if (rating_value >= 2)
                            {
                                groupHolder.star2IV.SetBackgroundResource(Resource.Drawable.active_star);
                                if (rating_value >= 3)
                                {
                                    groupHolder.star3IV.SetBackgroundResource(Resource.Drawable.active_star);
                                    if (rating_value >= 4)
                                    {
                                        groupHolder.star4IV.SetBackgroundResource(Resource.Drawable.active_star);
                                        if (rating_value >= 5)
                                        {
                                            groupHolder.star5IV.SetBackgroundResource(Resource.Drawable.active_star);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    groupHolder.headerRL.Visibility = ViewStates.Gone;
                    groupHolder.itemRL.Visibility = ViewStates.Visible;
                    groupHolder.skillNameTV.Text = group.upper_categ;
                    groupHolder.skillNameTV.SetTypeface(tf, TypefaceStyle.Bold);
                }
            }
            else if (itemNode.GetItemType() == 1)
            {
                ExpertEntryData entry = itemNode as ExpertEntryData;
                holderEntry = holder as EntryHolder;

                holderEntry.descriptionTitleTV.SetTypeface(tf, TypefaceStyle.Bold);
                holderEntry.description_text_dataTV.SetTypeface(tf, TypefaceStyle.Normal);
                holderEntry.services_and_pricesTV.SetTypeface(tf, TypefaceStyle.Bold);
                holderEntry.portfolioTitleTV.SetTypeface(tf, TypefaceStyle.Bold);
                if (!String.IsNullOrEmpty(entry.description))
                    holderEntry.description_text_dataTV.Text = entry.description;
                else
                    holderEntry.description_text_dataTV.Text = _context.GetString(Resource.String.no_data);

                holderEntry.layoutManager_services = new LinearLayoutManager(this.context, LinearLayoutManager.Vertical, false);
                holderEntry.recyclerViewServices.SetLayoutManager(holderEntry.layoutManager_services);
                var serv_temp_list = new List<ServiceData>();

                if (entry.services != null)
                {
                    holderEntry.recyclerViewServices.Visibility = ViewStates.Visible;
                    holderEntry.services_and_pricesTV.Visibility = ViewStates.Visible;
                    foreach (var d in entry.services)
                    {
                        serv_temp_list.Add(new ServiceData() { name = d.name, price = d.price });
                    }
                }
                else
                {
                    holderEntry.recyclerViewServices.Visibility = ViewStates.Gone;
                    holderEntry.services_and_pricesTV.Visibility = ViewStates.Gone;
                }
                var services_and_pricesAdapter = new ServicesAndPricesForExpertAdapter(serv_temp_list, _context, tf);
                holderEntry.recyclerViewServices.SetAdapter(services_and_pricesAdapter);

                holderEntry.portfolio_layoutManager = new LinearLayoutManager(this.context, LinearLayoutManager.Horizontal, false);
                holderEntry.portfolio_recyclerView.SetLayoutManager(holderEntry.portfolio_layoutManager);
                try
                {
                    var image_list = new List<string>();
                    int count_ = 0;
                    if (entry.photos == null)
                        count_ = 0;
                    else
                    {
                        count_ = entry.photos.Count;
                        int i = 0;
                        foreach (var img in entry.photos)
                        {
                            var current_img = JsonConvert.DeserializeObject<CategoryImage>(entry.photos[i].ToString());
                            image_list.Add(current_img.imageUrl);
                            i++;
                            if (i == count_)
                                break;
                        }
                    }
                    var portfolioAdapter = new PortfolioExpertAdapter(image_list, _context);
                    holderEntry.portfolio_recyclerView.SetAdapter(portfolioAdapter);
                    if (count_ < 1)
                    {
                        holderEntry.portfolioTitleTV.Visibility = ViewStates.Gone;
                        holderEntry.portfolio_recyclerView.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        holderEntry.portfolioTitleTV.Visibility = ViewStates.Visible;
                        holderEntry.portfolio_recyclerView.Visibility = ViewStates.Visible;
                    }
                }
                catch
                {
                    holderEntry.portfolioTitleTV.Visibility = ViewStates.Gone;
                    holderEntry.portfolio_recyclerView.Visibility = ViewStates.Gone;
                }
            }
            else if (itemNode.GetItemType() == 2)
            {
                ExpertGroupData2 group2 = itemNode as ExpertGroupData2;
                GroupExpertThreeLevelHolder2 groupHolder2 = holder as GroupExpertThreeLevelHolder2;
                groupHolder2.itemRL.Visibility = ViewStates.Visible;
                groupHolder2.skillNameTV.Text = group2.subcategory;
                groupHolder2.skillNameTV.SetTypeface(tf, TypefaceStyle.Normal);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == 0)
            {
                View entryView = inflater.Inflate(Resource.Layout.expert_profile_group, parent, false);
                return new GroupExpertThreeLevelHolder(entryView,
                                                        BackClick,
                                                        AboutExpertInfoClick,
                                                        FeedbackClick,
                                                        FeedbackListClick,
                                                        CallClick,
                                                        WriteClick,
                                                        GroupOnClick,
                                                        BttGroupOnClick);
            }
            else if (viewType == 1)
            {
                View sectionView = inflater.Inflate(Resource.Layout.expert_profile_entry, parent, false);
                return new EntryHolder(sectionView, BttEntryOnClick);
            }
            else if (viewType == 2)
            {
                View entryView = inflater.Inflate(Resource.Layout.expert_profile_group2, parent, false);
                return new GroupExpertThreeLevelHolder2(entryView, GroupOnClick2, BttGroupOnClick);
            }
            else
            {
                return null;
            }
        }
    }

    public class GroupExpertThreeLevelHolder : RecyclerView.ViewHolder
    {
        public TextView txtIdNo { set; get; }
        public ImageView bttGroup { set; get; }

        public TextView expertNameTV { set; get; }
        public TextView expertSurnameTV { set; get; }
        public TextView expert_phoneTV { set; get; }
        public TextView aboutExpertTitle { set; get; }
        public TextView aboutExpertInfo { set; get; }
        public TextView rating_valueTV { set; get; }
        public ImageView star1IV { set; get; }
        public ImageView star2IV { set; get; }
        public ImageView star3IV { set; get; }
        public ImageView star4IV { set; get; }
        public ImageView star5IV { set; get; }

        public TextView writeTV { set; get; }
        public TextView callTV { set; get; }
        public TextView nowTitle { set; get; }
        public TextView distanceTV { set; get; }
        public TextView cityTV { set; get; }
        public RelativeLayout writeRL { set; get; }
        public RelativeLayout callRL { set; get; }
        public RelativeLayout aboutExpertRL { set; get; }
        public RelativeLayout backRelativeLayout { set; get; }
        public ImageView onlineIV;
        public ImageView profile_image { get; set; }
        public LinearLayout headerRL { get; set; }
        public RelativeLayout itemRL { get; set; }
        public TextView skillNameTV { get; set; }
        public RecyclerView portfolio_recyclerView { set; get; }
        public RecyclerView.LayoutManager portfolio_layoutManager { set; get; }
        public Button leaveFeedbackBn { set; get; }

        public GroupExpertThreeLevelHolder(View view,
                                            Action<int> backClickListener,
                                            Action<int> aboutExpertClickListener,
                                            Action<int> feedbackExpertClickListener,
                                            Action<int> feedbackListClickListener,
                                            Action<int> callClickListener,
                                            Action<int> writeClickListener,
                                            Action<int> groupClickListener,
                                            Action<int> bttClickListener) : base(view)
        {
            aboutExpertRL = (RelativeLayout)view.FindViewById(Resource.Id.aboutExpertRL);
            this.headerRL = (LinearLayout)view.FindViewById(Resource.Id.headerRL);
            this.itemRL = (RelativeLayout)view.FindViewById(Resource.Id.itemRL);
            this.txtIdNo = (TextView)view.FindViewById(Resource.Id.group_txtId);
            this.bttGroup = (ImageView)view.FindViewById(Resource.Id.group_bttClick);
            expertNameTV = (TextView)view.FindViewById(Resource.Id.expertNameTV);
            expertSurnameTV = (TextView)view.FindViewById(Resource.Id.expertSurnameTV);
            expert_phoneTV = (TextView)view.FindViewById(Resource.Id.expert_phoneTV);
            aboutExpertTitle = (TextView)view.FindViewById(Resource.Id.aboutExpertTitle);
            aboutExpertInfo = (TextView)view.FindViewById(Resource.Id.aboutExpertInfo);
            rating_valueTV = (TextView)view.FindViewById(Resource.Id.rating_valueTV);
            star1IV = (ImageView)view.FindViewById(Resource.Id.star1IV);
            star2IV = (ImageView)view.FindViewById(Resource.Id.star2IV);
            star3IV = (ImageView)view.FindViewById(Resource.Id.star3IV);
            star4IV = (ImageView)view.FindViewById(Resource.Id.star4IV);
            star5IV = (ImageView)view.FindViewById(Resource.Id.star5IV);
            writeTV = (TextView)view.FindViewById(Resource.Id.writeTV);
            callTV = (TextView)view.FindViewById(Resource.Id.callTV);
            nowTitle = (TextView)view.FindViewById(Resource.Id.nowTitle);
            distanceTV = (TextView)view.FindViewById(Resource.Id.distanceTV);
            cityTV = (TextView)view.FindViewById(Resource.Id.cityTV);
            writeRL = (RelativeLayout)view.FindViewById(Resource.Id.writeRL);
            callRL = (RelativeLayout)view.FindViewById(Resource.Id.callRL);
            backRelativeLayout = (RelativeLayout)view.FindViewById(Resource.Id.backRelativeLayout);
            onlineIV = (ImageView)view.FindViewById(Resource.Id.onlineIV);
            profile_image = (ImageView)view.FindViewById(Resource.Id.profile_image);
            skillNameTV = (TextView)view.FindViewById(Resource.Id.skillNameTV);
            leaveFeedbackBn = (Button)view.FindViewById(Resource.Id.leaveFeedbackBn);
            view.Click += (sender, e) => groupClickListener(base.AdapterPosition);
            view.Click += view_Click;
            backRelativeLayout.Click += (s, e) => backClickListener(base.AdapterPosition);
            aboutExpertRL.Click += (s, e) => aboutExpertClickListener(base.AdapterPosition);
            leaveFeedbackBn.Click += (s, e) => feedbackExpertClickListener(base.AdapterPosition);

            star1IV.Click += (s, e) => feedbackListClickListener(base.AdapterPosition);
            star2IV.Click += (s, e) => feedbackListClickListener(base.AdapterPosition);
            star3IV.Click += (s, e) => feedbackListClickListener(base.AdapterPosition);
            star4IV.Click += (s, e) => feedbackListClickListener(base.AdapterPosition);
            star5IV.Click += (s, e) => feedbackListClickListener(base.AdapterPosition);
            rating_valueTV.Click += (s, e) => feedbackListClickListener(base.AdapterPosition);
            callRL.Click += (s, e) => callClickListener(base.AdapterPosition);
            writeRL.Click += (s, e) => writeClickListener(base.AdapterPosition);
        }
        private void view_Click(object sender, EventArgs e)
        {
            if (bttGroup.Rotation != 180)
            {
                bttGroup.Rotation = 180;
            }
            else
            {
                bttGroup.Rotation = 0;
            }
        }
    }

    public class GroupExpertThreeLevelHolder2 : RecyclerView.ViewHolder
    {
        public TextView txtIdNo { set; get; }
        public ImageView bttGroup { set; get; }
        public TextView skillNameTV { get; set; }
        public RelativeLayout itemRL { get; set; }

        public GroupExpertThreeLevelHolder2(View view, Action<int> groupClickListener2, Action<int> bttClickListener) : base(view)
        {
            this.txtIdNo = (TextView)view.FindViewById(Resource.Id.group_txtId);
            this.skillNameTV = (TextView)view.FindViewById(Resource.Id.skillNameTV);
            this.itemRL = (RelativeLayout)view.FindViewById(Resource.Id.itemRL);
            this.bttGroup = (ImageView)view.FindViewById(Resource.Id.group_bttClick);
            view.Click += (sender, e) => groupClickListener2(base.AdapterPosition);
            view.Click += view_Click;
        }
        private void view_Click(object sender, EventArgs e)
        {
            if (bttGroup.Rotation != 180)
            {
                bttGroup.Rotation = 180;
            }
            else
            {
                bttGroup.Rotation = 0;
            }
        }
    }

    public class EntryHolder : RecyclerView.ViewHolder
    {
        public TextView descriptionTitleTV { set; get; }
        public TextView description_text_dataTV { set; get; }
        public TextView services_and_pricesTV { set; get; }
        public TextView portfolioTitleTV { set; get; }
        public RecyclerView recyclerViewServices { set; get; }
        public RecyclerView portfolio_recyclerView { set; get; }
        public RecyclerView.LayoutManager portfolio_layoutManager { set; get; }
        public RecyclerView.LayoutManager layoutManager_services { set; get; }

        public EntryHolder(View view, Action<int> bttEntryClickListener) : base(view)
        {
            portfolio_recyclerView = (RecyclerView)view.FindViewById(Resource.Id.portfolio_recyclerView);
            recyclerViewServices = (RecyclerView)view.FindViewById(Resource.Id.recyclerView_Services);
            descriptionTitleTV = (TextView)view.FindViewById(Resource.Id.descriptionTitleTV);
            description_text_dataTV = (TextView)view.FindViewById(Resource.Id.description_text_dataTV);
            services_and_pricesTV = (TextView)view.FindViewById(Resource.Id.services_and_pricesTV);
            portfolioTitleTV = (TextView)view.FindViewById(Resource.Id.portfolioTitleTV);
        }
    }
}