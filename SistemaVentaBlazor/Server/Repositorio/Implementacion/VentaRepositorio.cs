using Microsoft.EntityFrameworkCore;
using SistemaVentaBlazor.Server.Models;
using SistemaVentaBlazor.Server.Repositorio.Contrato;
using System.Globalization;

namespace SistemaVentaBlazor.Server.Repositorio.Implementacion
{
    public class VentaRepositorio : IVentaRepositorio
    {
        private readonly DbventaBlazorContext _dbcontext;
        public VentaRepositorio(DbventaBlazorContext context)
        {
            _dbcontext = context;
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            var strategy = _dbcontext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                Venta VentaGenerada = new Venta();
                using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Validar que haya productos en la venta
                        if (entidad.DetalleVenta == null || !entidad.DetalleVenta.Any())
                        {
                            throw new Exception("No hay productos en la venta");
                        }

                        // Verificar y actualizar stock de productos
                        foreach (DetalleVenta dv in entidad.DetalleVenta)
                        {
                            var producto_encontrado = await _dbcontext.Productos
                                .FirstOrDefaultAsync(p => p.IdProducto == dv.IdProducto);

                            if (producto_encontrado == null)
                            {
                                throw new Exception($"No se encontró el producto con ID {dv.IdProducto}");
                            }

                            if (producto_encontrado.Stock < dv.Cantidad)
                            {
                                throw new Exception($"Stock insuficiente para el producto {producto_encontrado.Nombre}. Stock actual: {producto_encontrado.Stock}");
                            }

                            producto_encontrado.Stock = producto_encontrado.Stock - dv.Cantidad;
                            _dbcontext.Productos.Update(producto_encontrado);
                        }
                        await _dbcontext.SaveChangesAsync();

                        // Obtener y actualizar número de documento
                        var correlativo = await _dbcontext.NumeroDocumentos.FirstOrDefaultAsync();
                        if (correlativo == null)
                        {
                            throw new Exception("No se encontró la configuración de números de documento");
                        }

                        correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                        correlativo.FechaRegistro = DateTime.Now;

                        _dbcontext.NumeroDocumentos.Update(correlativo);
                        await _dbcontext.SaveChangesAsync();

                        // Generar número de venta
                        string ceros = string.Concat(Enumerable.Repeat("0", 4));
                        string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                        numeroVenta = numeroVenta.Substring(numeroVenta.Length - 4, 4);

                        // Registrar la venta
                        entidad.NumeroDocumento = numeroVenta;
                        entidad.FechaRegistro = DateTime.Now;

                        await _dbcontext.Venta.AddAsync(entidad);
                        await _dbcontext.SaveChangesAsync();

                        VentaGenerada = entidad;

                        await transaction.CommitAsync();
                        Console.WriteLine($"Venta registrada exitosamente. Número de documento: {numeroVenta}");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Error al registrar venta: {ex.Message}");
                        if (ex.InnerException != null)
                            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                        throw;
                    }
                }
                return VentaGenerada;
            });
        }

        public async Task<List<Venta>> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = _dbcontext.Venta;

            if (buscarPor == "fecha")
            {
                DateTime fech_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-PE"));
                DateTime fech_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-PE"));

                return await query.Where(v =>
                    v.FechaRegistro.Value.Date >= fech_Inicio.Date &&
                    v.FechaRegistro.Value.Date <= fech_Fin.Date
                )
                .Include(dv => dv.DetalleVenta)
                .ThenInclude(p => p.IdProductoNavigation)
                .ToListAsync();
            }
            else
            {
                return await query.Where(v => v.NumeroDocumento == numeroVenta)
                  .Include(dv => dv.DetalleVenta)
                  .ThenInclude(p => p.IdProductoNavigation)
                  .ToListAsync();
            }
        }

        public async Task<List<DetalleVenta>> Reporte(string FechaInicio, string FechaFin)
        {
            DateTime fech_Inicio = DateTime.ParseExact(FechaInicio, "dd/MM/yyyy", new CultureInfo("es-PE"));
            DateTime fech_Fin = DateTime.ParseExact(FechaFin, "dd/MM/yyyy", new CultureInfo("es-PE"));

            List<DetalleVenta> listaResumen = await _dbcontext.DetalleVenta
                .Include(p => p.IdProductoNavigation)
                .Include(v => v.IdVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fech_Inicio.Date && dv.IdVentaNavigation.FechaRegistro.Value.Date <= fech_Fin.Date)
                .ToListAsync();

            return listaResumen;
        }
    }
}
