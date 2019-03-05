using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Iamnearby.Activities;
using PCL.Models;
using Iamnearby.ViewHolders;
using System.Collections.Generic;

namespace Iamnearby.Adapters
{
    public class Services_and_pricesAdapter : RecyclerView.Adapter
    {
        private Activity _context;
        private List<ServiceData> service_data;
        RecyclerView.LayoutManager layoutManager;
        Typeface tf;

        public Services_and_pricesAdapter(List<ServiceData> service_data, Activity context, Typeface tf)
        {
            this.service_data = service_data;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var addServiceViewHolder = (Services_and_prices_view_holder)holder;

            addServiceViewHolder.serviceTV.Text = service_data[position].name;
            addServiceViewHolder.priceTV.Text = service_data[position].price;
            addServiceViewHolder.serviceTV.SetTypeface(tf, TypefaceStyle.Bold);
            addServiceViewHolder.priceTV.SetTypeface(tf, TypefaceStyle.Normal);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ServicesAndPricesRow, parent, false);
            return new Services_and_prices_view_holder(layout, OnItemClick/*, OnServiceClick, OnPhotoClick*/);
        }
        public override int ItemCount
        {
            get { try { return service_data.Count; } catch { return 0; } }
        }

        void OnItemClick(int position)
        {
            EditServiceActivity.service_id = service_data[position].service_id;
            EditServiceActivity.service_name = service_data[position].name;
            EditServiceActivity.service_price = service_data[position].price;
            _context.StartActivity(typeof(EditServiceActivity));
        }
        void OnServiceClick(int position)
        {

        }
        void OnPhotoClick(int position)
        {

        }
    }
}