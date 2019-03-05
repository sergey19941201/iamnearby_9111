using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Iamnearby.Activities;
using Iamnearby.ViewHolders;
using System.Collections.Generic;

namespace Iamnearby.Adapters
{
    class PortfolioExpertAdapter : RecyclerView.Adapter
    {
        private Activity _context;
        private List<string> photos;
        public PortfolioExpertAdapter(List<string> photos, Activity context)
        {
            this.photos = photos;
            _context = context;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var portfolioViewHolder = (PortfolioViewHolder)holder;
            if (!System.String.IsNullOrEmpty(photos[position]))
            {
                Glide.With(_context)
               .Load(photos[position])
               .Into(portfolioViewHolder.portfolioIV);
            }
            else
            {
                Glide.With(_context)
                .Load(null)
                .Into(portfolioViewHolder.portfolioIV);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PortfolioRow, parent, false);
            return new PortfolioViewHolder(layout, OnItemClick);
        }

        public override int ItemCount
        {
            get { try { return photos.Count; } catch { return 0; } }
        }

        void OnItemClick(int position)
        {
            DisplayPortfolioPhotoActivity.from_expert = true;
            DisplayPortfolioPhotoActivity.image_url = photos[position];
            _context.StartActivity(typeof(DisplayPortfolioPhotoActivity));
        }
    }
}