using MvcCubosSegundaPracticaNr.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace MvcCubosSegundaPracticaNr.Services
{
    public class ServiceCubos
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApi;

        public ServiceCubos(IConfiguration configuration)
        {
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this.UrlApi = configuration.GetValue<string>("ApiUrl:ApiCubos");
        }

        public async Task<string> GetTokenAsync
        (string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };
                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data =
                        await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token =
                        jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }
        //call async con el token
        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        //call async sin el token
        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default;
                }
            }
        }
        //get all cubos
        public async Task<List<Cubo>> GetCubosAsync()
        {
            string request = "api/Cubos";
            List<Cubo> cubos = await this.CallApiAsync<List<Cubo>>(request);
            return cubos;
        }

        //buscar cubos por marca
        public async Task<List<Cubo>> FindCubosByMarca(string marca)
        {
            string request = "api/Cubos/" + marca;
            List<Cubo> cubos = await this.CallApiAsync<List<Cubo>>(request);
            return cubos;
        }

        //crear un nuevo usuario
        public async Task InsertUserAsync(string nombre, string email, string pass, string imagen)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Cubos/InsertUser";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                //tenemos que enviar un objeto JSON
                //nos creamos un objeto de la clase Hospital
                Usuario usuario = new Usuario
                {
                    Nombre = nombre,
                    Email = email,
                    Pass = pass,
                    Imagen = imagen
                };
                //convertimos el objeto a json
                string json = JsonConvert.SerializeObject(usuario);
                //para enviar datos al servicio se utiliza 
                //la clase SytringContent, donde debemos indicar
                //los datos, de ending  y su tipo
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        //crear un nuevo CUBO
        public async Task InsertCuboAsync(string nombre, string marca, string imagen, int precio)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Cubos/InsertCubo";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                //tenemos que enviar un objeto JSON
                //nos creamos un objeto de la clase Hospital
                Cubo cubo = new Cubo
                {
                  Nombre = nombre,
                  Marca = marca,
                  Imagen = imagen,
                  Precio = precio
                };
                //convertimos el objeto a json
                string json = JsonConvert.SerializeObject(cubo);
                //para enviar datos al servicio se utiliza 
                //la clase SytringContent, donde debemos indicar
                //los datos, de ending  y su tipo
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }


        //METODO PROTEGIDO PARA RECUPERAR EL PERFIL
        public async Task<Usuario> GetPerfilUserAsync
            (string token)
        {
            string request = "api/Cubos/perfiluser";
            Usuario empleado = await
                this.CallApiAsync<Usuario>(request, token);
            return empleado;
        }

        //METODO PROTEGIDO PARA RECUPERAR LOS PEDIDIOS
        public async Task<List<CompraCubo>> GetPedidosAsync
            (string token)
        {
            string request = "api/Cubos/GetPedidosByUser";
            List<CompraCubo> compras = await this.CallApiAsync<List<CompraCubo>>(request, token);
            return compras;
        }

        //crear un nuevo pedido
        public async Task InsertPedidoAsync(int idcubo, int iduser, DateTime fecha, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Cubos/InsertPedido";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                     ("Authorization", "bearer " + token);
                //tenemos que enviar un objeto JSON
                //nos creamos un objeto de la clase Cubo
                CompraCubo compra = new CompraCubo
                {
                    Id_Pedido = 0,
                    Id_Cubo = idcubo,
                    Id_Usario = iduser,
                    FechaPedidio = fecha
                };
                //convertimos el objeto a json
                string json = JsonConvert.SerializeObject(compra);
                //para enviar datos al servicio se utiliza 
                //la clase SytringContent, donde debemos indicar
                //los datos, de ending  y su tipo
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }



    }
}