using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SistemaVentaBlazor.Shared
{
    public class VentaDTO
    {
        public int IdVenta { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? TipoPago { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? FechaRegistro { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Total { get; set; }

        [JsonIgnore]
        public string? TotalTexto
        {
            get
            {
                if (DetalleVenta == null || !DetalleVenta.Any())
                    return "0";

                decimal? sum = DetalleVenta.Sum(p => p.Total);
                return sum?.ToString() ?? "0";
            }
        }

        public List<DetalleVentaDTO> DetalleVenta { get; set; } = new List<DetalleVentaDTO>();
    }
}
