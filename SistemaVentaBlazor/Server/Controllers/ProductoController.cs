using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVentaBlazor.Server.Models;
using SistemaVentaBlazor.Server.Repositorio.Contrato;
using SistemaVentaBlazor.Shared;
using System.Text.Json;

namespace SistemaVentaBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductoRepositorio _productoRepositorio;
        public ProductoController(IProductoRepositorio productoRepositorio, IMapper mapper)
        {
            _mapper = mapper;
            _productoRepositorio = productoRepositorio;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            try
            {
                Console.WriteLine("Iniciando consulta de productos...");
                var query = await _productoRepositorio.Consultar();
                
                Console.WriteLine("Incluyendo navegación de categorías...");
                query = query.Include(r => r.IdCategoriaNavigation);
                
                Console.WriteLine("Ejecutando consulta...");
                var productos = await query.ToListAsync();
                
                Console.WriteLine($"Productos encontrados: {productos.Count}");
                var listaProductos = _mapper.Map<List<ProductoDTO>>(productos);
                Console.WriteLine($"Productos mapeados: {listaProductos.Count}");

                var response = new ResponseDTO<List<ProductoDTO>>
                {
                    status = listaProductos.Any(),
                    msg = listaProductos.Any() ? "ok" : "No se encontraron productos",
                    value = listaProductos
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Lista: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                
                var response = new ResponseDTO<List<ProductoDTO>>
                {
                    status = false,
                    msg = $"Error al obtener productos: {ex.Message}",
                    value = null
                };
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar([FromBody] ProductoDTO request)
        {
            ResponseDTO<ProductoDTO> _ResponseDTO = new ResponseDTO<ProductoDTO>();
            try
            {
                Producto _producto = _mapper.Map<Producto>(request);
                _producto.FechaRegistro = DateTime.Now;
                _producto.EsActivo = true;

                Producto _productoCreado = await _productoRepositorio.Crear(_producto);

                if (_productoCreado.IdProducto != 0)
                    _ResponseDTO = new ResponseDTO<ProductoDTO>() { status = true, msg = "ok", value = _mapper.Map<ProductoDTO>(_productoCreado) };
                else
                    _ResponseDTO = new ResponseDTO<ProductoDTO>() { status = false, msg = "No se pudo crear el producto" };

                return Ok(_ResponseDTO);
            }
            catch (Exception ex)
            {
                _ResponseDTO = new ResponseDTO<ProductoDTO>() { status = false, msg = ex.Message };
                return StatusCode(500, _ResponseDTO);
            }
        }

        [HttpPut]
        [Route("Editar")]
        public async Task<IActionResult> Editar([FromBody] ProductoDTO request)
        {
            ResponseDTO<bool> _ResponseDTO = new ResponseDTO<bool>();
            try
            {
                Producto _producto = _mapper.Map<Producto>(request);
                Producto _productoParaEditar = await _productoRepositorio.Obtener(u => u.IdProducto == _producto.IdProducto);

                if (_productoParaEditar != null)
                {
                    _productoParaEditar.Nombre = _producto.Nombre;
                    _productoParaEditar.IdCategoria = _producto.IdCategoria;
                    _productoParaEditar.Stock = _producto.Stock;
                    _productoParaEditar.Precio = _producto.Precio;
                    _productoParaEditar.CodigoBarras = _producto.CodigoBarras;

                    bool respuesta = await _productoRepositorio.Editar(_productoParaEditar);

                    if (respuesta)
                        _ResponseDTO = new ResponseDTO<bool>() { status = true, msg = "ok", value = true };
                    else
                        _ResponseDTO = new ResponseDTO<bool>() { status = false, msg = "No se pudo editar el producto" };
                }
                else
                {
                    _ResponseDTO = new ResponseDTO<bool>() { status = false, msg = "No se encontró el producto" };
                }

                return Ok(_ResponseDTO);
            }
            catch (Exception ex)
            {
                _ResponseDTO = new ResponseDTO<bool>() { status = false, msg = ex.Message };
                return StatusCode(500, _ResponseDTO);
            }
        }

        [HttpDelete]
        [Route("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            ResponseDTO<string> _ResponseDTO = new ResponseDTO<string>();
            try
            {
                Producto _productoEliminar = await _productoRepositorio.Obtener(u => u.IdProducto == id);

                if (_productoEliminar != null)
                {
                    bool respuesta = await _productoRepositorio.Eliminar(_productoEliminar);

                    if (respuesta)
                        _ResponseDTO = new ResponseDTO<string>() { status = true, msg = "ok", value = "" };
                    else
                        _ResponseDTO = new ResponseDTO<string>() { status = false, msg = "No se pudo eliminar el producto", value = "" };
                }

                return Ok(_ResponseDTO);
            }
            catch (Exception ex)
            {
                _ResponseDTO = new ResponseDTO<string>() { status = false, msg = ex.Message };
                return StatusCode(500, _ResponseDTO);
            }
        }
    }
}
