using internal_search.Domain.DTOs.buscador.persona;

public interface IHistorialService
{
    Task GuardarAsync(int codUsuario, byte[] contenido, string nombreArchivo,
                      IEnumerable<string> secciones, int totalDnis);

    Task<List<HistorialDescargaDto>> ListarAsync(int codUsuario);

    Task<(byte[] Contenido, string Nombre)?> DescargarAsync(int codHistorial, int codUsuario);
}