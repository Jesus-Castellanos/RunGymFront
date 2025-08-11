using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RunGymFront.Services
{
    
    public static class Microservices
    {
        private static string apiURL = ConfigurationManager.AppSettings["Api"].ToString();

        public static async Task<T> PostWithoutToken<T>(object modelRequest, string endpoint)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Clear();

                    string json = JsonConvert.SerializeObject(modelRequest);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage Res = await client.PostAsync(endpoint, content);
                    string responseBody = await Res.Content.ReadAsStringAsync(); // <- Aquí se lee el contenido del error

                    if (!Res.IsSuccessStatusCode)
                    {
                        // Mostrar el contenido del error detalladamente
                        throw new Exception($"Error: {Res.StatusCode}\nContenido del error:\n{responseBody}");
                    }

                    // Validar si la respuesta parece un JSON
                    if (!responseBody.TrimStart().StartsWith("{") && !responseBody.TrimStart().StartsWith("["))
                    {
                        if (typeof(T) == typeof(string))
                        {
                            return (T)(object)responseBody;
                        }
                        else
                        {
                            throw new Exception($"Respuesta inválida JSON recibida: {responseBody}");
                        }
                    }

                    // Reemplazar valores problemáticos
                    responseBody = responseBody.Replace("Infinity", "null")
                                               .Replace("-Infinity", "null")
                                               .Replace("NaN", "null");

                    try
                    {
                        T result = JsonConvert.DeserializeObject<T>(responseBody);
                        return result;
                    }
                    catch (JsonException jsonEx)
                    {
                        throw new Exception($"Error al deserializar la respuesta JSON: {jsonEx.Message}\nContenido: {responseBody}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error haciendo la petición al Endpoint: " + endpoint + ", " + ex.ToString());
            }
        }


        public static async Task<T> PostWithToken<T>(object modelRequest, string endpoint)
        {
            try
            {
                if (!string.IsNullOrEmpty(SessionHelper.BearerToken))
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(apiURL);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionHelper.BearerToken);
                        string json = JsonConvert.SerializeObject(modelRequest);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        HttpResponseMessage Res = await client.PostAsync(endpoint, content);

                        if (Res.IsSuccessStatusCode)
                        {
                            var res = Res.Content.ReadAsStringAsync().Result;
                            T response = JsonConvert.DeserializeObject<T>(res);
                            return response;
                        }
                        else
                        {
                            throw new Exception("Error: " + Res.StatusCode);
                        }
                    }
                }
                else
                {
                    throw new Exception("Error: Token no existe o esta expierado");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error haciendo la petición post al Endpoint: " + endpoint);
            }
        }

        public static async Task<T>PutWithToken<T>(object modelRequest, string endpoint)
        {
            try
            {
                Console.WriteLine("Token actual: " + SessionHelper.BearerToken);

                if (string.IsNullOrWhiteSpace(SessionHelper.BearerToken))
                    throw new Exception("Token no disponible.");

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionHelper.BearerToken);

                    string json = JsonConvert.SerializeObject(modelRequest);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(endpoint, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string resultJson = await response.Content.ReadAsStringAsync();
                        T result = JsonConvert.DeserializeObject<T>(resultJson);
                        return result;
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Error {response.StatusCode}: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error haciendo POST con token: " + ex.Message, ex);
            }
        }
    

        public static async Task<T> GetWithoutToken<T>(Dictionary<string, string>? parameters, string endpoint)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Clear();

                    // Construir query string si hay parámetros
                    if (parameters != null && parameters.Any())
                    {
                        var query = string.Join("&", parameters.Select(kvp =>
                            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

                        // Agregar query a endpoint
                        endpoint = endpoint.Contains("?") ? $"{endpoint}&{query}" : $"{endpoint}?{query}";
                    }

                    HttpResponseMessage Res = await client.GetAsync(endpoint);

                    if (Res.IsSuccessStatusCode)
                    {
                        var res = await Res.Content.ReadAsStringAsync();
                        T response = JsonConvert.DeserializeObject<T>(res);
                        return response;
                    }
                    else
                    {
                        throw new Exception("Error: " + Res.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error haciendo la petición GET al Endpoint: " + endpoint, ex);
            }
        }

        public static async Task<T> GetWithToken<T>(Dictionary<string, string>? parameters, string endpoint)
        {
            try
            {
                if (string.IsNullOrEmpty(SessionHelper.BearerToken))
                {
                    HttpContext.Current.Response.Redirect("/Login/Login", true);
                    return default(T);
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionHelper.BearerToken);

                    if (parameters != null && parameters.Any())
                    {
                        var query = string.Join("&", parameters.Select(kvp =>
                            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

                        endpoint = endpoint.Contains("?") ? $"{endpoint}&{query}" : $"{endpoint}?{query}";
                    }

                    HttpResponseMessage Res = await client.GetAsync(endpoint);

                    if (Res.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        HttpContext.Current.Response.Redirect("/Login/Login", true);
                        return default(T);
                    }

                    if (Res.IsSuccessStatusCode)
                    {
                        var res = await Res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(res);
                    }
                    else
                    {
                        throw new Exception($"Error: {Res.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error haciendo la petición GET al Endpoint: " + endpoint, ex);
            }
        }

        public static async Task<bool> DeleteWithToken(string endpoint)
        {
            try
            {
                if (!string.IsNullOrEmpty(SessionHelper.BearerToken))
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(apiURL);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionHelper.BearerToken);

                        HttpResponseMessage response = await client.DeleteAsync(endpoint);

                        return response.IsSuccessStatusCode;
                    }
                }
                else
                {
                    throw new Exception("Error: Token no existe o está expirado");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error haciendo la petición DELETE al Endpoint: " + endpoint, ex);
            }
        }

        internal static Task<T2> PostWithToken<T1, T2>(T1 ejercicio, string v)
        {
            throw new NotImplementedException();
        }

        internal static Task<T> DeleteWithToken<T>(string v)
        {
            throw new NotImplementedException();
        }
    }
}