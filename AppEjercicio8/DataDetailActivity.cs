using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.Core.Graphics.Drawable;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;

namespace AppEjercicio8
{
    [Activity(Label = "DataDetailActivity")]
    public class DataDetailActivity : Activity, IOnMapReadyCallback
    {
        TextView txtNombre, txtDomicilio, txtCorreo, txtSaldo, txtEdad;
        ImageView Imagen, ImagenFondo;
        GoogleMap googleMap;
        string correo, imagen, nombre, domicilio, imagenfondo;
        double saldo, lat, lon;
        int edad;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DataDetail);
            try
            {
                correo = Intent.GetStringExtra("correo");
                imagen = Intent.GetStringExtra("imagen");
                imagenfondo = Intent.GetStringExtra("imagenfondo");
                nombre = Intent.GetStringExtra("nombre");
                edad = int.Parse(Intent.GetStringExtra("edad"));
                domicilio = Intent.GetStringExtra("domicilio");
                saldo = double.Parse(Intent.GetStringExtra("saldo"));
                lat = double.Parse(Intent.GetStringExtra("latitud"));
                lon = double.Parse(Intent.GetStringExtra("longitud"));

                txtNombre = FindViewById<TextView>(Resource.Id.txtname);
                txtDomicilio = FindViewById<TextView>(Resource.Id.txtaddress);
                txtCorreo = FindViewById<TextView>(Resource.Id.txtmail);
                txtEdad = FindViewById<TextView>(Resource.Id.txtage);
                txtSaldo = FindViewById<TextView>(Resource.Id.txtrevenues);
                Imagen = FindViewById<ImageView>(Resource.Id.image);
                ImagenFondo = FindViewById<ImageView>(Resource.Id.backimage);

                txtNombre.Text = nombre;
                txtDomicilio.Text = domicilio;
                txtCorreo.Text = correo;
                txtEdad.Text = edad.ToString();
                txtSaldo.Text = saldo.ToString();
                var typeface = Typeface.CreateFromAsset(this.Assets, "fonts/pacifico-regular.ttf");
                txtNombre.SetTypeface(typeface, TypefaceStyle.Normal);
                var RutaImagen = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), imagen);
                var RutaImagenFondo = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), imagenfondo);
                var rutauriimagen = Android.Net.Uri.Parse(RutaImagen);
                var rutauriimagenfondo = Android.Net.Uri.Parse(RutaImagenFondo);

                Imagen.SetImageURI(rutauriimagen);
                ImagenFondo.SetImageURI(rutauriimagenfondo);
                var opciones = new BitmapFactory.Options();
                opciones.InPreferredConfig = Bitmap.Config.Argb8888;
                var bitmap = BitmapFactory.DecodeFile(RutaImagen, opciones);
                Imagen.SetImageDrawable(redondeo(bitmap, 20));

                var mapView = FindViewById<MapView>(Resource.Id.map);
                mapView.OnCreate(savedInstanceState);
                mapView.GetMapAsync(this);
                MapsInitializer.Initialize(this);
            }
            catch(System.Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }

        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this.googleMap = googleMap;
            var builder = CameraPosition.InvokeBuilder();
            builder.Target(new LatLng(lat, lon));
            builder.Zoom(17);
            var cameraPosition = builder.Build();
            var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            this.googleMap.AnimateCamera(cameraUpdate);
        }

        public RoundedBitmapDrawable redondeo(Bitmap image, int cornerRadius)
        {
            var corner = RoundedBitmapDrawableFactory.Create(null, image);
            corner.CornerRadius = cornerRadius;
            return corner;

        }
    }
}