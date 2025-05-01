using System.Net.Http.Json;
using System.Text.Json;

namespace SistemaVentaBlazor.Client.Servicios.Implementacion
{
    public class ProductoService : IProductoService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductoService(HttpClient http)
        {
            _http = http;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ResponseDTO<ProductoDTO>> Crear(ProductoDTO entidad)
        {
            try
            {
                var result = await _http.PostAsJsonAsync("api/producto/Guardar", entidad);
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadFromJsonAsync<ResponseDTO<ProductoDTO>>(_jsonOptions);
                    return response!;
                }
                return new ResponseDTO<ProductoDTO> { status = false, msg = "Error al crear el producto" };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<ProductoDTO> { status = false, msg = ex.Message };
            }
        }

        public async Task<bool> Editar(ProductoDTO entidad)
        {
            try
            {
                var result = await _http.PutAsJsonAsync("api/producto/Editar", entidad);
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadFromJsonAsync<ResponseDTO<bool>>(_jsonOptions);
                    return response!.status;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var result = await _http.DeleteAsync($"api/producto/Eliminar/{id}");
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadFromJsonAsync<ResponseDTO<string>>(_jsonOptions);
                    return response!.status;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ResponseDTO<List<ProductoDTO>>> Lista()
        {
            try
            {
                Console.WriteLine("Iniciando solicitud de lista de productos...");
                var response = await _http.GetFromJsonAsync<ResponseDTO<List<ProductoDTO>>>("api/producto/Lista", _jsonOptions);
                
                if (response == null)
                {
                    Console.WriteLine("La respuesta es nula");
                    return new ResponseDTO<List<ProductoDTO>> 
                    { 
                        status = false, 
                        msg = "La respuesta del servidor fue nula",
                        value = new List<ProductoDTO>()
                    };
                }

                Console.WriteLine($"Respuesta recibida - Status: {response.status}, Mensaje: {response.msg}, Cantidad de productos: {response.value?.Count ?? 0}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Lista(): {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                
                return new ResponseDTO<List<ProductoDTO>> 
                { 
                    status = false, 
                    msg = "Error al obtener la lista de productos: " + ex.Message,
                    value = new List<ProductoDTO>()
                };
            }
        }
    }
}
