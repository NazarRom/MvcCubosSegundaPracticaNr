using Microsoft.AspNetCore.Mvc;
using MvcCubosSegundaPracticaNr.Filters;
using MvcCubosSegundaPracticaNr.Models;
using MvcCubosSegundaPracticaNr.Services;

namespace MvcCubosSegundaPracticaNr.Controllers
{
    public class CubosController : Controller
    {
        private ServiceCubos service;
        public CubosController(ServiceCubos service)
        {
            this.service = service;
        }
        //get all cubos
        public async Task<IActionResult> Index()
        {
            List<Cubo> cubos = await this.service.GetCubosAsync();
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
        public async Task<IActionResult> InsertNewUser(Usuario user)
        {
            await this.service.InsertUserAsync(user.Nombre, user.Email, user.Pass, user.Imagen);
            return RedirectToAction("Index");
        }

        //insert new Cubo
        public IActionResult InsertNewCubo()
        {          
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> InsertNewCubo(Cubo cubo)
        {
            await this.service.InsertCuboAsync(cubo.Nombre, cubo.Marca, cubo.Imagen, cubo.Precio);
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
