using Microsoft.AspNetCore.Mvc;
using MvcCubosSegundaPracticaNr.Filters;
using MvcCubosSegundaPracticaNr.Models;
using MvcCubosSegundaPracticaNr.Services;

namespace MvcCubosSegundaPracticaNr.Controllers
{
    public class CubosController : Controller
    {
        private ServiceCubos service;
        private ServiceStorageBlobs serviceBlobs;
        public CubosController(ServiceCubos service, ServiceStorageBlobs serviceBlobs)
        {
            this.service = service;
            this.serviceBlobs = serviceBlobs;
        }
        //get all cubos
        public async Task<IActionResult> Index()
        {
            List<Cubo> cubos = new();

            cubos = await this.service.GetCubosAsync();
            foreach (var cub in cubos)
            {
                if (cub.Imagen != null)
                {
                    cub.Imagen = await this.serviceBlobs.GetBlobUriAsync("publicfotos", cub.Imagen);
                }
            }

            return View(cubos);

        }

        //get cubos by marca
        public IActionResult CubosByMarca()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CubosByMarca(string marca)
        {
            List<Cubo> cubos = await this.service.FindCubosByMarca(marca);
            return View(cubos);
        }

        public IActionResult InsertNewUser()
        {
            return View();
        }
        //insert new User
        [HttpPost]
        public async Task<IActionResult> InsertNewUser(Usuario user, IFormFile file)
        {
            string blobName = file.FileName;

            using (Stream stream = file.OpenReadStream())
            {
                await this.serviceBlobs.UploadBlobAsync("privatefotos", blobName, stream);
            }
            await this.service.InsertUserAsync(user.Nombre, user.Email, user.Pass, blobName);
            return RedirectToAction("Index");
        }

        //insert new Cubo
        public IActionResult InsertNewCubo()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> InsertNewCubo(Cubo cubo, IFormFile file)
        {
           
            string blobName = file.FileName;

            using (Stream stream = file.OpenReadStream())
            {
                await this.serviceBlobs.UploadBlobAsync("publicfotos", blobName, stream);
            }

            await this.service.InsertCuboAsync(cubo.Nombre, cubo.Marca, blobName, cubo.Precio);
            return RedirectToAction("Index");
        }
       

        //perfil
        [AuthorizeCubos]
        public async Task<IActionResult> PerfilUser()
        {
            string token =
                HttpContext.Session.GetString("TOKEN");
            Usuario usuario = await
                this.service.GetPerfilUserAsync(token);
          ViewData["private"] = await this.serviceBlobs.GetBlobUriAsync("privatefotos", usuario.Imagen);
            return View(usuario);
        }

        //pedidos
        [AuthorizeCubos]
        public async Task<IActionResult> PedidosUser()
        {
            string token = HttpContext.Session.GetString("TOKEN");
            List<CompraCubo> compra = await this.service.GetPedidosAsync(token);
            return View(compra);
        }

        //insert new Cubo    
        public async Task<IActionResult> ComprarCubo(int id)
        {
            string token = HttpContext.Session.GetString("TOKEN");
            Usuario user = await this.service.GetPerfilUserAsync(token);
            await this.service.InsertPedidoAsync(id, user.Id_Usuario, DateTime.Now, token);
            return RedirectToAction("Index");
        }
    }
}
