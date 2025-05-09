﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentaBlazor.Shared
{
    public class DashBoardDTO
    {
        public int TotalVentas { get; set; }
        public string? TotalIngresos { get; set; }
        public int TotalProductos { get; set; }
        public string? ValorTotalStock { get; set; }
        public List<VentaSemanaDTO>? VentasUltimaSemana { get; set; }
        public List<ProductoCategoriaDTO>? ProductosPorCategoria { get; set; }
        public List<MontoCategoriaDTO>? MontosPorCategoria { get; set; }
        public List<StockCategoriaDTO>? StockPorCategoria { get; set; }
    }
}
