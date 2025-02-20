using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

using PRUEBAS_LOGIN.Models;

using System.Data.SqlClient;
using System.Data;

using Newtonsoft.Json;


namespace PRUEBAS_LOGIN.Controllers
{
    public class AccesoController : Controller
    {
        //Conexion a la base de datos
        static string cadena = "Data Source=DESKTOP-HF98NSE\\SQLEXPRESS;Initial Catalog=DB_FACTURA;Integrated Security=true";
        

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [Obsolete]
        public IActionResult Registrar(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;
                
            if (oUsuario.Clave == oUsuario.ConfirmarClave) {

                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            }else {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }
            using (SqlConnection cn = new SqlConnection(cadena)) {
                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.Parameters.AddWithValue("Nombre", oUsuario.Nombre);

                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                cmd.ExecuteNonQuery();
                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                #pragma warning disable CS8600
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();
            }
            ViewData["Mensaje"] = mensaje;

            if(registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }else {
                return View();
            }
        }
        [HttpPost]
        [Obsolete]
        public IActionResult Login(Usuario oUsuario)
        {
            try
            {
                // Asegúrate de que la contraseña sea segura, al convertirla a SHA256
                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);

                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Correo", oUsuario.Correo);
                    cmd.Parameters.AddWithValue("@Clave", oUsuario.Clave);

                    cn.Open();

                    // Ejecutamos el procedimiento almacenado y leemos el resultado
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // Si hay datos
                        {
                            int resultado = reader.GetInt32(0); // IdUsuario o código de error
                            string nombre = reader.IsDBNull(1) ? null : reader.GetString(1); // Nombre

                            if (resultado > 0)
                            {
                                // Si el resultado es mayor que 0, el usuario es válido
                                oUsuario.IdUsuario = resultado;
                                oUsuario.Nombre = nombre; // Asignar el nombre recuperado

                                // Aquí se serializa el objeto 'Usuario' y se guarda en la sesión
                                HttpContext.Session.SetString("usuario", JsonConvert.SerializeObject(oUsuario));

                                // Asegúrate de que la sesión esté activa antes de redirigir
                                if (HttpContext.Session.GetString("usuario") != null)
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    // Si por alguna razón la sesión no se guardó correctamente
                                    ViewData["Mensaje"] = "Hubo un problema al iniciar sesión. Intenta nuevamente.";
                                    return View();
                                }
                            }
                            else if (resultado == -1)
                            {
                                // Si el procedimiento almacenado devuelve -1, la clave es incorrecta
                                ViewData["Mensaje"] = "La clave es incorrecta. Intenta nuevamente.";
                            }
                            else if (resultado == -2)
                            {
                                // Si el procedimiento almacenado devuelve -2, el correo es incorrecto
                                ViewData["Mensaje"] = "Correo electrónico incorrecto. Por favor, revisa tu correo.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Captura la excepción y muestra un mensaje de error
                ViewData["Mensaje"] = "Ocurrió un error al iniciar sesión: " + ex.Message;
                return View();
            }

            // Asegúrate de que ViewData["Mensaje"] está asignado antes de retornar la vista
            return View();
        }

        public static string ConvertirSha256(string texto)
        {
            // Crea una instancia de SHA256 usando el método Create()
            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                StringBuilder sb = new StringBuilder();
                foreach (byte b in result)
                {
                    sb.Append(b.ToString("x2"));  // Convierte cada byte en su representación hexadecimal
                }
                return sb.ToString();
            }
        }

    }
}
