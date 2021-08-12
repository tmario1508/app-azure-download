using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using System.Collections.Generic;

namespace AppEjercicio8
{
    [Activity(Icon = "@mipmap/azure", Label = "8-Azure Download", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ProgressDialog Progreso;
        ListView listado;
        List<Clientes> ListadeClientes = new List<Clientes>();
        List<ElementosdelaTabla> ElementosTabla = new List<ElementosdelaTabla>();
        string elementoimagen, elementoimagenfondo;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Ocultar barra superior
            SupportActionBar.Hide();

            listado = FindViewById<ListView>(Resource.Id.Lista);
            Progreso = new ProgressDialog(this);
            Progreso.Indeterminate = true;
            Progreso.SetProgressStyle(ProgressDialogStyle.Spinner);
            Progreso.SetMessage("Descargando informacion de azure");
            Progreso.SetCancelable(false);
            Progreso.Show();
            await CargarDatosAzure();
            Progreso.Hide();

        }
        public async Task CargarDatosAzure()
        {
            try
            {
                var typeface = Typeface.CreateFromAsset(this.Assets, "fonts/pacifico-regular.ttf");
                var titulo = FindViewById<TextView>(Resource.Id.title);
                titulo.SetTypeface(typeface, TypefaceStyle.Italic);
                var CuentadeAlmacenamiento = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=mariotorresestorage;AccountKey=r3NpKfwEjGua/bu/F2vRcixJwUbWT8WpiUGL+zKKleq66bi/5AfBf4dyW8o13fr+SCbq06zxPjijktLaQEGWWA==;EndpointSuffix=core.windows.net"); 
                var ClienteBlob = CuentadeAlmacenamiento.CreateCloudBlobClient();
                var Contenedor = ClienteBlob.GetContainerReference("devs");
                var TablaNoSQL = CuentadeAlmacenamiento.CreateCloudTableClient();
                var Tabla = TablaNoSQL.GetTableReference("devs");
                var Consulta = new TableQuery<Clientes>();
                TableContinuationToken token = null;
                var Datos = await Tabla.ExecuteQuerySegmentedAsync<Clientes>(Consulta, token, null, null);
                ListadeClientes.AddRange(Datos.Results);
                int iCorreo = 0;
                int iNombre = 0;
                int iImage = 0;
                int iDomicilio = 0;
                int iEdad = 0;
                int iSaldo = 0;
                int iLatitud = 0;
                int iLongitud = 0;
                int iImagenFondo = 0;
                ElementosTabla = ListadeClientes.Select(r => new ElementosdelaTabla()
                {
                    Correo = ListadeClientes.ElementAt(iCorreo++).Correo,
                    Nombre = ListadeClientes.ElementAt(iNombre++).RowKey,
                    Imagen = ListadeClientes.ElementAt(iImage++).Imagen,
                    Domicilio = ListadeClientes.ElementAt(iDomicilio++).Domicilio,
                    Edad = ListadeClientes.ElementAt(iEdad++).Edad,
                    Saldo = ListadeClientes.ElementAt(iSaldo++).Saldo,
                    Longitud = ListadeClientes.ElementAt(iLongitud++).Longitud,
                    Latitud = ListadeClientes.ElementAt(iLatitud++).Latitud,
                    ImagenFondo = ListadeClientes.ElementAt(iImagenFondo++).ImagenFondo,

                }).ToList();
                int contadorimagen = 0;
                while (contadorimagen < ListadeClientes.Count)
                {
                    elementoimagen = ListadeClientes.ElementAt(contadorimagen).Imagen;
                    elementoimagenfondo = ListadeClientes.ElementAt(contadorimagen).ImagenFondo;
                    var ImagenBlob = Contenedor.GetBlockBlobReference(elementoimagen);
                    var ImagenFondoBlob = Contenedor.GetBlockBlobReference(elementoimagenfondo);
                    var rutaimagen = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    var ArchivoImagen = System.IO.Path.Combine(rutaimagen, elementoimagen);
                    var ArchivoImagenFondo = System.IO.Path.Combine(rutaimagen, elementoimagenfondo);
                    var StreamImagen = File.OpenWrite(ArchivoImagen);
                    var StreamImagenFondo = File.OpenWrite(ArchivoImagenFondo);
                    await ImagenBlob.DownloadToStreamAsync(StreamImagen);
                    await ImagenFondoBlob.DownloadToStreamAsync(StreamImagenFondo);
                    contadorimagen++;
                }
                Toast.MakeText(this, "Imagenes Descargadas", ToastLength.Long).Show();
                listado.Adapter = new DataAdapterr(this, ElementosTabla);
                listado.ItemClick += OnListItemClick;
            }
            catch(System.Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        public void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var DataSend = ElementosTabla[e.Position];
            var DataIntent = new Intent(this, typeof(DataDetailActivity));
            DataIntent.PutExtra("correo", DataSend.Correo);
            DataIntent.PutExtra("imagen", DataSend.Imagen);
            DataIntent.PutExtra("imagenfondo", DataSend.ImagenFondo);
            DataIntent.PutExtra("nombre", DataSend.Nombre);
            DataIntent.PutExtra("edad", DataSend.Edad.ToString());
            DataIntent.PutExtra("saldo", DataSend.Saldo.ToString());
            DataIntent.PutExtra("domicilio", DataSend.Domicilio);
            DataIntent.PutExtra("latitud", DataSend.Latitud.ToString());
            DataIntent.PutExtra("longitud", DataSend.Longitud.ToString());
            StartActivity(DataIntent);
        }

        public class Clientes : TableEntity
        {

            public Clientes(string Categoria, string Nombre) {
                PartitionKey = Categoria;
                RowKey = Nombre;
            }

            public Clientes() { }

            public string Domicilio { get; set; }

            public int Edad { get; set; }
            public string ImagenFondo { get; set; }
            public string Imagen { get; set; }
            public string Latitud { get; set; }
            public string Longitud { get; set; }
            public string Correo { get; set; }
            public double Saldo { get; set; }
        }

        
    }

    public class ElementosdelaTabla
    {
        public string Nombre { get; set; }

        public string Domicilio { get; set; }
        public int Edad { get; set; }
        public string ImagenFondo { get; set; }
        public string Imagen { get; set; }

        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Correo { get; set; }
        public double Saldo { get; set; }
    }
}