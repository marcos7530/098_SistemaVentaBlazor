using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentaBlazor.Server.Repositorio.Contrato;
using SistemaVentaBlazor.Shared;

namespace SistemaVentaBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDashBoardRepositorio _dashboardRepositorio;
        public DashBoardController(IDashBoardRepositorio dashboardRepositorio, IMapper mapper)
        {
            _mapper = mapper;
            _dashboardRepositorio = dashboardRepositorio;
        }

        [HttpGet]
        [Route("Resumen")]
        public async Task<IActionResult> Resumen()
        {
            ResponseDTO<DashBoardDTO> _response = new ResponseDTO <DashBoardDTO>();

            try
            {

                DashBoardDTO vmDashboard = new DashBoardDTO();

                vmDashboard.TotalVentas = await _dashboardRepositorio.TotalVentasUltimaSemana();
                vmDashboard.TotalIngresos = await _dashboardRepositorio.TotalIngresosUltimaSemana();
                vmDashboard.TotalProductos = await _dashboardRepositorio.TotalProductos();
                vmDashboard.ValorTotalStock = await _dashboardRepositorio.ValorTotalStock();

                List<VentaSemanaDTO> listaVentasSemana = new List<VentaSemanaDTO>();

                foreach (KeyValuePair<string, int> item in await _dashboardRepositorio.VentasUltimaSemana())
                {
                    listaVentasSemana.Add(new VentaSemanaDTO()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                }
                vmDashboard.VentasUltimaSemana = listaVentasSemana;

                // Obtener productos por categoría
                List<ProductoCategoriaDTO> listaProductosCategoria = new List<ProductoCategoriaDTO>();
                foreach (KeyValuePair<string, int> item in await _dashboardRepositorio.ProductosPorCategoria())
                {
                    listaProductosCategoria.Add(new ProductoCategoriaDTO()
                    {
                        Categoria = item.Key,
                        Cantidad = item.Value
                    });
                }
                vmDashboard.ProductosPorCategoria = listaProductosCategoria;

                // Obtener montos por categoría
                List<MontoCategoriaDTO> listaMontosCategoria = new List<MontoCategoriaDTO>();
                foreach (KeyValuePair<string, decimal> item in await _dashboardRepositorio.MontosPorCategoria())
                {
                    listaMontosCategoria.Add(new MontoCategoriaDTO()
                    {
                        Categoria = item.Key,
                        Monto = item.Value
                    });
                }
                vmDashboard.MontosPorCategoria = listaMontosCategoria;

                // Obtener valor del stock por categoría
                List<StockCategoriaDTO> listaStockCategoria = new List<StockCategoriaDTO>();
                foreach (KeyValuePair<string, decimal> item in await _dashboardRepositorio.StockPorCategoria())
                {
                    listaStockCategoria.Add(new StockCategoriaDTO()
                    {
                        Categoria = item.Key,
                        ValorTotal = item.Value
                    });
                }
                vmDashboard.StockPorCategoria = listaStockCategoria;

                _response = new ResponseDTO<DashBoardDTO>() { status = true, msg = "ok", value = vmDashboard };
                return StatusCode(StatusCodes.Status200OK, _response);

            }
            catch (Exception ex)
            {
                _response = new ResponseDTO<DashBoardDTO>() { status = false, msg = ex.Message, value = null };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

        }
    }
}
