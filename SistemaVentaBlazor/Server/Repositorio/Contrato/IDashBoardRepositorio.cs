namespace SistemaVentaBlazor.Server.Repositorio.Contrato
{
    public interface IDashBoardRepositorio
    {
        Task<int> TotalVentasUltimaSemana();
        Task<string> TotalIngresosUltimaSemana();
        Task<int> TotalProductos();
        Task<Dictionary<string, int>> VentasUltimaSemana();
        Task<Dictionary<string, int>> ProductosPorCategoria();
        Task<Dictionary<string, decimal>> MontosPorCategoria();
        Task<Dictionary<string, decimal>> StockPorCategoria();
        Task<string> ValorTotalStock();
    }
}
