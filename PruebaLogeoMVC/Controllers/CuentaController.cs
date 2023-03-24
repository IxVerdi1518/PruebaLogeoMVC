using Microsoft.AspNetCore.Mvc;
using PruebaLogeoMVC.Data;
using PruebaLogeoMVC.Models;
using System.Data;
using System.Data.SqlClient;

namespace PruebaLogeoMVC.Controllers
{
    public class CuentaController : Controller
    {
        private readonly DbContext _dbContext;
        public CuentaController(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Registrar()
        {
            return View("Registrar");
        }

        [HttpPost]
        public ActionResult Registrar(UsuarioModel usuarioModel)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    using(SqlConnection con = new(_dbContext.Valor))
                    {
                        using(SqlCommand cmd = new ("sp_registrar",con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = usuarioModel.Nombre;
                            cmd.Parameters.Add("@Correo", SqlDbType.VarChar).Value = usuarioModel.Correo;
                            cmd.Parameters.Add("@Edad", SqlDbType.Int).Value = usuarioModel.Edad;
                            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = usuarioModel.Usuario;
                            cmd.Parameters.Add("@Clave", SqlDbType.VarChar).Value = usuarioModel.Clave;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    return RedirectToAction("Index","Home");
                }
            }catch (Exception)
            {
                return View("Registrar");
            }
            ViewData["error"] = "Error de credenciales";
            return View("Registrar");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginmodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (SqlConnection con = new(_dbContext.Valor))
                    {
                        using (SqlCommand cmd = new("sp_login", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = loginmodel.Usuario;
                            cmd.Parameters.Add("@Clave", SqlDbType.VarChar).Value = loginmodel.Clave;
                            con.Open();
                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                Response.Cookies.Append("user", "Bienvenido "+ loginmodel.Usuario);
                                return RedirectToAction("Index","Home");
                            }
                            else
                            {
                                ViewData["error"] = "Error de credenciales";
                            }
                            con.Close();
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return View("Login");
            }
            return View("Login");
        }

        public ActionResult Logout()
        {
            Response.Cookies.Delete("user");
            return RedirectToAction("Index", "Home");
        }
            
    }
}
