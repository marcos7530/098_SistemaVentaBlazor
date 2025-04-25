using SistemaVentaBlazor.Server.Models;
using SistemaVentaBlazor.Server.Repositorio.Contrato;
using System.Globalization;

namespace SistemaVentaBlazor.Server.Repositorio.Implementacion
{
    public class DashBoardRepositorio : IDashBoardRepositorio
    {
        private readonly DbventaBlazorContext _dbcontext;
        public DashBoardRepositorio(DbventaBlazorContext context)
        {
            _dbcontext = context;
        }

        public async Task<int> TotalVentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> _ventaQuery = _dbcontext.Venta.AsQueryable();
                DateTime fechaHoy = DateTime.Today;
                
                return _ventaQuery.Count(v => v.FechaRegistro.Value.Date == fechaHoy);
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> TotalIngresosUltimaSemana()
        {
            try
            {
                IQueryable<Venta> _ventaQuery = _dbcontext.Venta.AsQueryable();
                DateTime fechaHoy = DateTime.Today;
                
                decimal resultado = _ventaQuery
                    .Where(v => v.FechaRegistro.Value.Date == fechaHoy)
                    .Sum(v => v.Total ?? 0);

                return string.Format("$ {0:N2}", resultado);
            }
            catch
            {
                throw;
            }
        }
        public async Task<int> TotalProductos()
        {
            try
            {
                IQueryable<Categoria> query = _dbcontext.Categoria;
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();
            try
            {
                IQueryable<Venta> _ventaQuery = _dbcontext.Venta.AsQueryable();
                if (_ventaQuery.Count() > 0)
                {
                    DateTime? ultimaFecha = _dbcontext.Venta.OrderByDescending(v => v.FechaRegistro).Select(v => v.FechaRegistro).First();
                    ultimaFecha = ultimaFecha.Value.AddDays(-7);

                    IQueryable<Venta> query = _dbcontext.Venta.Where(v => v.FechaRegistro.Value.Date >= ultimaFecha.Value.Date);

                    resultado = query
                        .GroupBy(v => v.FechaRegistro.Value.Date).OrderBy(g => g.Key)
                        .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                        .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);
                }


                return resultado;

            }
            catch
            {
                throw;
            }

        }

        public async Task<Dictionary<string, int>> ProductosPorCategoria()
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();
            try
            {
                var query = from p in _dbcontext.Productos
                           join c in _dbcontext.Categoria on p.IdCategoria equals c.IdCategoria
                           group p by c.Descripcion into g
                           select new { Categoria = g.Key, Cantidad = g.Count() };

                resultado = query.ToDictionary(keySelector: r => r.Categoria, elementSelector: r => r.Cantidad);

                return resultado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, decimal>> MontosPorCategoria()
        {
            Dictionary<string, decimal> resultado = new Dictionary<string, decimal>();
            try
            {
                var query = from dv in _dbcontext.DetalleVenta
                           join p in _dbcontext.Productos on dv.IdProducto equals p.IdProducto
                           join c in _dbcontext.Categoria on p.IdCategoria equals c.IdCategoria
                           group dv by c.Descripcion into g
                           select new { Categoria = g.Key, Monto = Math.Round(g.Sum(x => x.Total ?? 0), 2) };

                resultado = query.ToDictionary(keySelector: r => r.Categoria, elementSelector: r => r.Monto);

                return resultado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, decimal>> StockPorCategoria()
        {
            Dictionary<string, decimal> resultado = new Dictionary<string, decimal>();
            try
            {
                var query = from p in _dbcontext.Productos
                           join c in _dbcontext.Categoria on p.IdCategoria equals c.IdCategoria
                           group p by c.Descripcion into g
                           select new { Categoria = g.Key, ValorTotal = Math.Round(g.Sum(x => (x.Stock * x.Precio) ?? 0), 2) };

                resultado = query.ToDictionary(keySelector: r => r.Categoria, elementSelector: r => r.ValorTotal);

                return resultado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> ValorTotalStock()
        {
            decimal resultado = 0;
            try
            {
                resultado = _dbcontext.Productos
                    .Sum(p => (p.Stock * p.Precio) ?? 0);

                return string.Format("$ {0:N2}", resultado);
            }
            catch
            {
                throw;
            }
        }
    }
}
