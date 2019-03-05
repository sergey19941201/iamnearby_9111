using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;

namespace Iamnearby.Adapters
{
    public class ImageAdapter : BaseAdapter
    {
        Context context;
        //string[] filelist;
        List<string> filelist;
        int width_screen;

        public ImageAdapter(Context c, List<string> filelist/*string[] f*/, int width_screen)
        {
            context = c;
            this.filelist = filelist;
            this.width_screen = width_screen;
        }

        public override int Count
        {
            get { try { return filelist.Count; } catch { return 0; } }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        // create a new ImageView for each item referenced by the Adapter
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView imageView;

            if (convertView == null)
            {  // if it's not recycled, initialize some attributes
                imageView = new ImageView(context);
                imageView.LayoutParameters = new GridView.LayoutParams(width_screen / 3, width_screen / 3);
                imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
                //imageView.SetPadding(8, 8, 8, 8);
            }
            else
            {
                imageView = (ImageView)convertView;
            }
            if (position == 0 || position == 1)
                imageView.SetImageResource(Convert.ToInt32(filelist[position]));
            else
                Glide.With(Application.Context)
                             .Load(filelist[position])
                             //.Placeholder(Resource.Drawable.specialization_imageIV)
                             .Into(imageView);
            //imageView.SetImageResource(thumbIds[position]);
            return imageView;
        }
    };
}
