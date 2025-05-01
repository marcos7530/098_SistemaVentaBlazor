using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SistemaVentaBlazor.Shared
{
    public class DetalleVentaDTO
    {
        public int IdProducto { get; set; }
        public string? DescripcionProducto { get; set; }
        public string? CodigoBarras { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Cantidad { get; set; } = 1;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Precio { get; set; } = 0;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Total { get; set; } = 0;
    }
}
