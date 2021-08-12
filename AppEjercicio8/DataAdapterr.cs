using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Graphics.Drawable;
using System.Collections.Generic;

namespace AppEjercicio8
{
    public class DataAdapterr : BaseAdapter<ElementosdelaTabla>
    {
        List<ElementosdelaTabla> items;

        Activity context;

        public DataAdapterr(Activity context, List<ElementosdelaTabla> items) : base()
        {
            this.context = context;
            this.items = items;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override ElementosdelaTabla this[int position]
        {
            get { return items[position]; }
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            view = context.LayoutInflater.Inflate(Resource.Layout.DataRow, null);
            view.FindViewById<TextView>(Resource.Id.txtcorreo).Text = item.Correo;
            var txtNombre = view.FindViewById<TextView>(Resource.Id.txtnombre);
            txtNombre.Text = item.Nombre;
            var typeface = Typeface.CreateFromAsset(context.Assets, "fonts/pacifico-regular.ttf");
            txtNombre.SetTypeface(typeface, TypefaceStyle.Italic);
            var path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), item.Imagen);
            var pathback = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), item.ImagenFondo);
            var Image = BitmapFactory.DecodeFile(path);
            var ImageBack = BitmapFactory.DecodeFile(pathback);
            Image = ResizeBitmap(Image, 100, 100);
            view.FindViewById<ImageView>(Resource.Id.imagen).SetImageDrawable(redondeo(Image, 5));
            view.FindViewById<ImageView>(Resource.Id.imagenfondo).SetImageDrawable(redondeo(ImageBack, 5));
            return view;
        }

        public RoundedBitmapDrawable redondeo(Bitmap image, int cornerRadius)
        {
            var corner = RoundedBitmapDrawableFactory.Create(null, image);
            corner.CornerRadius = cornerRadius;
            return corner;

        }

        private Bitmap ResizeBitmap(Bitmap imagenoriginal, int widthimagenoriginal, int heightimagenoriginal)
        {
            Bitmap resizeImage = Bitmap.CreateBitmap(widthimagenoriginal, heightimagenoriginal, Bitmap.Config.Argb8888);
            float Widht = imagenoriginal.Width;
            float Height = imagenoriginal.Height;
            var canvas = new Canvas(resizeImage);
            var scala = widthimagenoriginal / Widht;
            var xTranslation = 0.0f;
            var yTranslation = (heightimagenoriginal - Height * scala) / 2.0f;
            var Transformacion = new Matrix();
            Transformacion.PostTranslate(xTranslation, yTranslation);
            Transformacion.PreScale(scala, scala);
            var paint = new Paint();
            paint.FilterBitmap = true;
            canvas.DrawBitmap(imagenoriginal, Transformacion, paint);
            return resizeImage;
        }

    }
 }

