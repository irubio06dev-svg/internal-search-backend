using internal_search.Domain.DTOs.Buscador;
using internal_search.Domain.Entities;
using internal_search.Domain.Interfaces.Buscador;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search_backend.Infraestructure.Repositories.Buscador
{



    public class BuscadorRepository : IBuscadorRepository
    {
        private readonly AppDbContext _context;

        public BuscadorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BuscadorResponse> BuscarAsync(BuscadorEntrada request)
        {
            var tipoDocumento = request.TipoDocumento?.Trim().ToUpper() ?? "";

            var calificaciones = new List<Calificacion>();
            var deudas = new List<Deuda>();
            var lineasCredito = new List<LineaCredito>();
            var moviles = new List<Movil>();

            bool tieneDocumento = !string.IsNullOrWhiteSpace(request.Documento);
            bool tieneApePat = !string.IsNullOrWhiteSpace(request.ApePat);
            bool tieneApeMat = !string.IsNullOrWhiteSpace(request.ApeMat);
            bool tienePrenombres = !string.IsNullOrWhiteSpace(request.Prenombres);
            bool tieneTelefono = !string.IsNullOrWhiteSpace(request.Telefono);

            bool esRuc = tipoDocumento == "RUC";

            // --- Calificaciones ---
            if (tieneDocumento || tieneApePat || (!esRuc && (tieneApeMat || tienePrenombres)))
            {
                var q = _context.Calificaciones.AsQueryable();

                if (tieneDocumento)
                    q = q.Where(x => x.Documento != null && x.Documento.Contains(request.Documento!));

                if (tieneApePat)
                    q = q.Where(x => x.ApePat != null && x.ApePat.Contains(request.ApePat!));

                if (!esRuc && tieneApeMat)
                    q = q.Where(x => x.ApeMat != null && x.ApeMat.Contains(request.ApeMat!));

                if (!esRuc && tienePrenombres)
                    q = q.Where(x =>
                        (x.PriNombre != null && x.PriNombre.Contains(request.Prenombres!)) ||
                        (x.SegNombre != null && x.SegNombre.Contains(request.Prenombres!)));

                calificaciones = await q.ToListAsync();
            }

            // --- Deudas (razón social directa) ---
            if (tieneDocumento || tieneApePat || (!esRuc && (tieneApeMat || tienePrenombres)))
            {
                var q = _context.Deudas.AsQueryable();

                if (tieneDocumento)
                    q = q.Where(x => x.Documento != null && x.Documento.Contains(request.Documento!));

                // Para RUC busca razón social con ApePat; para DNI también sirve como "apellido/nombre libre"
                if (tieneApePat)
                    q = q.Where(x => x.RazonSocial != null && x.RazonSocial.Contains(request.ApePat!));

                if (!esRuc && tieneApeMat)
                    q = q.Where(x => x.RazonSocial != null && x.RazonSocial.Contains(request.ApeMat!));

                if (!esRuc && tienePrenombres)
                    q = q.Where(x => x.RazonSocial != null && x.RazonSocial.Contains(request.Prenombres!));

                deudas = await q.ToListAsync();
            }

            // --- LineaCreditos (mismo patrón que Deudas) ---
            if (tieneDocumento || tieneApePat || (!esRuc && (tieneApeMat || tienePrenombres)))
            {
                var q = _context.LineaCreditos.AsQueryable();

                if (tieneDocumento)
                    q = q.Where(x => x.Documento != null && x.Documento.Contains(request.Documento!));

                if (tieneApePat)
                    q = q.Where(x => x.RazonSocial != null && x.RazonSocial.Contains(request.ApePat!));

                if (!esRuc && tieneApeMat)
                    q = q.Where(x => x.RazonSocial != null && x.RazonSocial.Contains(request.ApeMat!));

                if (!esRuc && tienePrenombres)
                    q = q.Where(x => x.RazonSocial != null && x.RazonSocial.Contains(request.Prenombres!));

                lineasCredito = await q.ToListAsync();
            }

            // --- Movil: NO aplica para RUC (empresas no tienen línea móvil personal) ---
            if (!esRuc && (tieneDocumento || tieneApePat || tieneApeMat || tienePrenombres || tieneTelefono))
            {
                var q = _context.Movil.AsQueryable();

                if (tieneDocumento)
                    q = q.Where(x => x.Documento != null && x.Documento.Contains(request.Documento!));
                if (tieneApePat)
                    q = q.Where(x => x.ApePat != null && x.ApePat.Contains(request.ApePat!));
                if (tieneApeMat)
                    q = q.Where(x => x.ApeMat != null && x.ApeMat.Contains(request.ApeMat!));
                if (tienePrenombres)
                    q = q.Where(x => x.Prenombres != null && x.Prenombres.Contains(request.Prenombres!));
                if (tieneTelefono)
                    q = q.Where(x => x.Telefono != null && x.Telefono.Contains(request.Telefono!));

                moviles = await q.ToListAsync();
            }

            return new BuscadorResponse
            {
                Calificaciones = calificaciones,
                Deudas = deudas,
                LineasCredito = lineasCredito,
                Moviles = moviles
            };
        }
        private static bool EsLongitudValidaParaTipo(string documento, string tipoDocumento)
        {
            return tipoDocumento switch
            {
                "DNI" => documento.Length == 8,
                "RUC" => documento.Length == 11,
                "CE" => documento.Length >= 9 && documento.Length <= 12,
                "PASAPORTE" => documento.Length >= 6 && documento.Length <= 12,
                _ => false
            };
        }
    }
}

