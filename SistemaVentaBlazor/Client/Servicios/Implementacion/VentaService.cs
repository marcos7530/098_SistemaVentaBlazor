using System.Net.Http.Json;
using System.Text.Json;

namespace SistemaVentaBlazor.Client.Servicios.Implementacion
{
    public class VentaService : IVentaService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        public VentaService(HttpClient http)
        {
            _http = http;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<ResponseDTO<List<VentaDTO>>> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<ResponseDTO<List<VentaDTO>>>($"api/venta/Historial?buscarPor={buscarPor}&numeroVenta={numeroVenta}&fechaInicio={fechaInicio}&fechaFin={fechaFin}", _jsonOptions);
                return result!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Historial: {ex.Message}");
                return new ResponseDTO<List<VentaDTO>> { status = false, msg = ex.Message };
            }
        }

        public async Task<ResponseDTO<VentaDTO>> Registrar(VentaDTO entidad)
        {
            try
            {
                // Asegurarse de que la lista DetalleVenta no sea null
                entidad.DetalleVenta ??= new List<DetalleVentaDTO>();
                
                // Serializar manualmente para verificar
                var jsonContent = JsonSerializer.Serialize(entidad, _jsonOptions);
                Console.WriteLine($"Enviando datos: {jsonContent}");

                var result = await _http.PostAsJsonAsync("api/venta/Registrar", entidad, _jsonOptions);
                
                if (!result.IsSuccessStatusCode)
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error en la respuesta: {errorContent}");
                    return new ResponseDTO<VentaDTO> { status = false, msg = $"Error del servidor: {result.StatusCode}" };
                }

                var response = await result.Content.ReadFromJsonAsync<ResponseDTO<VentaDTO>>(_jsonOptions);
                return response!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Registrar: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                return new ResponseDTO<VentaDTO> { status = false, msg = ex.Message };
            }
        }

        public async Task<ResponseDTO<List<ReporteDTO>>> Reporte(string fechaInicio, string fechaFin)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<ResponseDTO<List<ReporteDTO>>>($"api/venta/Reporte?fechaInicio={fechaInicio}&fechaFin={fechaFin}", _jsonOptions);
                return result!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Reporte: {ex.Message}");
                return new ResponseDTO<List<ReporteDTO>> { status = false, msg = ex.Message };
            }
        }
    }
}
