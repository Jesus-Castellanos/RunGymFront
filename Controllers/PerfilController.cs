using Newtonsoft.Json;
using RunGymFront.Models;
using RunGymFront.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace RunGymFront.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private static string apiURL = ConfigurationManager.AppSettings["Api"].ToString();

        public async Task<ActionResult> Perfil()
        {
            try
            {
                List<Usuarios> usuarios = await Microservices.GetWithToken<List<Usuarios>>(null, "Usuarios/GetUsuarios");

                int usuarioId = 0;
                if (SessionHelper.Usuario != null)
                    usuarioId = SessionHelper.Usuario.Id;
                else
                {
                    TempData["Error"] = "Debe iniciar sesión.";
                    return RedirectToAction("Login", "Login");
                }

                usuarios = usuarios.FindAll(m => m.Id == usuarioId);

                TempData["Usuarios"] = JsonConvert.SerializeObject(usuarios);
                return View(usuarios);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al obtener el perfil: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Editar()
        {
            try
            {
                if (SessionHelper.Usuario == null)
                {
                    TempData["Error"] = "Debe iniciar sesión.";
                    return RedirectToAction("Login", "Login");
                }

                int usuarioId = SessionHelper.Usuario.Id;

                List<Usuarios> usuarios = await Microservices.GetWithToken<List<Usuarios>>(null, "Usuarios/GetUsuarios");
                Usuarios usuario = usuarios.Find(u => u.Id == usuarioId);

                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado.";
                    return RedirectToAction("Perfil");
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el perfil para edición: {ex.Message}";
                return RedirectToAction("Perfil");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(Usuarios model)
        {
            ModelState.Remove("Contraseña");
            ModelState.Remove("ConfirmarContraseña");
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (SessionHelper.Usuario == null || string.IsNullOrEmpty(SessionHelper.BearerToken))
                {
                    TempData["Error"] = "Debe iniciar sesión.";
                    return RedirectToAction("Login", "Login");
                }

                using (var client = new HttpClient())
                {
                    // Asegura que el ID venga de la sesión, no del form manipulable
                    model.Id = SessionHelper.Usuario.Id;

                    // Solo incluye campos permitidos para la actualización
                    var usuarioModificado = new
                    {
                        model.Id,
                        model.Nombre,
                        model.Apellido,
                        model.TipoDocumento,
                        model.Documento,
                        model.Correo,
                        model.Celular,
                        model.Genero,
                        model.FechaNacimiento,
                        model.Peso,
                        model.Altura,
                        model.HorasSueno,
                        model.ConsumoAgua,
                        RolId = SessionHelper.Usuario.RolId,
                        Contraseña = SessionHelper.Usuario.Contraseña

                        // Se omiten: Contraseña, ConfirmarContraseña, RolId
                    };

                    var json = JsonConvert.SerializeObject(usuarioModificado);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await Microservices.PutWithToken<HttpResponseMessage>(usuarioModificado, "Usuarios/PutUsuario/");

                    if (response.IsSuccessStatusCode)
                    {
                        // Actualiza solo los campos que se enviaron en la sesión
                        SessionHelper.Usuario.Nombre = model.Nombre;
                        SessionHelper.Usuario.Apellido = model.Apellido;
                        SessionHelper.Usuario.TipoDocumento = model.TipoDocumento;
                        SessionHelper.Usuario.Documento = model.Documento;
                        SessionHelper.Usuario.Correo = model.Correo;
                        SessionHelper.Usuario.Celular = model.Celular;
                        SessionHelper.Usuario.Genero = model.Genero;
                        SessionHelper.Usuario.FechaNacimiento = model.FechaNacimiento;
                        SessionHelper.Usuario.Peso = model.Peso;
                        SessionHelper.Usuario.Altura = model.Altura;
                        SessionHelper.Usuario.HorasSueno = model.HorasSueno;
                        SessionHelper.Usuario.ConsumoAgua = model.ConsumoAgua;

                        TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
                        return RedirectToAction("Perfil");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = "Error al actualizar el perfil: " + errorContent;
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar el perfil: {ex.Message}";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Eliminar()
        {
            try
            {
                if (SessionHelper.Usuario == null)
                {
                    TempData["Error"] = "Debe iniciar sesión para ver su perfil.";
                    return RedirectToAction("Login", "Login");
                }

                int usuarioId = SessionHelper.Usuario.Id;

                // Idealmente, deberías tener un endpoint que recupere el usuario por ID directamente,
                // en lugar de traer toda la lista y luego filtrar.
                // Por ahora, mantendremos la lógica de filtrar de la lista si no tienes otro endpoint.
                List<Usuarios> usuarios = await Microservices.GetWithToken<List<Usuarios>>(null, "Usuarios/GetUsuarios");
                Usuarios usuarioActual = usuarios.Find(u => u.Id == usuarioId);

                if (usuarioActual == null)
                {
                    TempData["Error"] = "No se encontró la información de su perfil. Por favor, inicie sesión nuevamente.";
                    // Limpiar sesión si el usuario no se encuentra, por si hay inconsistencia
                    SessionHelper.Usuario = null;
                    SessionHelper.BearerToken = null;
                    return RedirectToAction("Login", "Login");
                }

                // Devuelve una lista que contiene solo el usuario actual, ya que tu vista @model List<Usuarios>
                return View(new List<Usuarios> { usuarioActual });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el perfil: {ex.Message}";
                // Considera redirigir a una página de error o al login si el error es grave
                return RedirectToAction("Login", "Login");
            }
        }

        // ---

        // Este es el método [HttpPost] que se encarga de la lógica de eliminación.
        // Se llamará cuando el usuario confirme la eliminación desde el modal en la vista de Perfil.
        [HttpPost]
        [ValidateAntiForgeryToken] // Fundamental para proteger contra ataques CSRF
        public async Task<ActionResult> EliminarConfirmado()
        {
            try
            {
                // 1. Validar que el usuario esté autenticado
                if (SessionHelper.Usuario == null || string.IsNullOrEmpty(SessionHelper.BearerToken))
                {
                    TempData["Error"] = "Debe iniciar sesión para eliminar su cuenta.";
                    return RedirectToAction("Login", "Login");
                }

                // 2. Obtener el ID del usuario de la sesión (el usuario actualmente logueado)
                int userId = SessionHelper.Usuario.Id;

                // 3. Realizar la llamada al microservicio para eliminar el usuario por su ID
                var result = await Microservices.DeleteWithToken($"Usuarios/DeleteUsuario/{userId}");

                // 4. Manejar la respuesta del microservicio
                if (result)
                {
                    // Si la eliminación fue exitosa, cerrar la sesión del usuario
                    SessionHelper.Usuario = null;
                    SessionHelper.BearerToken = null;
                    TempData["SuccessMessage"] = "Su cuenta ha sido eliminada correctamente.";
                    // Redirigir a la página de inicio de sesión o a una página de confirmación de eliminación
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    // Si el microservicio no pudo eliminar la cuenta
                    TempData["Error"] = "No se pudo eliminar la cuenta en este momento. Intente de nuevo más tarde.";
                    // Redirigir de vuelta al perfil con un mensaje de error
                    return RedirectToAction("Perfil");
                }
            }
            catch (Exception ex)
            {
                // Capturar y manejar cualquier excepción durante el proceso de eliminación
                TempData["Error"] = $"Ocurrió un error inesperado al intentar eliminar la cuenta: {ex.Message}";
                return RedirectToAction("Perfil");
            }
        }
    }
}

