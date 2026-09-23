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

            // --- Calificaciones: solo si hay algún campo que le aplica ---
            if (tieneDocumento || tieneApePat || tieneApeMat || tienePrenombres)
            {
                var qCalificaciones = _context.Calificaciones.AsQueryable();

                if (tieneDocumento)
                    qCalificaciones = qCalificaciones.Where(x => x.Documento.Contains(request.Documento!));
                if (tieneApePat)
                    qCalificaciones = qCalificaciones.Where(x => x.ApePat != null && x.ApePat.Contains(request.ApePat!));
                if (tieneApeMat)
                    qCalificaciones = qCalificaciones.Where(x => x.ApeMat != null && x.ApeMat.Contains(request.ApeMat!));
                if (tienePrenombres)
                    qCalificaciones = qCalificaciones.Where(x =>
                        (x.PriNombre != null && x.PriNombre.Contains(request.Prenombres!)) ||
                        (x.SegNombre != null && x.SegNombre.Contains(request.Prenombres!)));

                calificaciones = await qCalificaciones.ToListAsync();
            }

            // --- Deudas: solo si hay Documento o algún campo de nombre ---
            if (tieneDocumento || tieneApePat || tieneApeMat || tienePrenombres)
            {
                var qDeudas = _context.Deudas.AsQueryable();

                if (tieneDocumento)
                    qDeudas = qDeudas.Where(x => x.Documento.Contains(request.Documento!));
                if (tieneApePat)
                    qDeudas = qDeudas.Where(x => x.RazonSocial != null && x.RazonSocial.Contains(request.ApePat!));
                if (tieneApeMat)
                    qDeudas = qDeudas.Where(x => x.RazonSocial != null && x.RazonSocial.Contains(request.ApeMat!));
                if (tienePrenombres)
                    qDeudas = qDeudas.Where(x => x.RazonSocial != null && x.RazonSocial.Contains(request.Prenombres!));

                deudas = await qDeudas.ToListAsync();
            }

            // --- LineaCreditos: mismo patrón ---
            if (tieneDocumento || tieneApePat || tieneApeMat || tienePrenombres)
            {
                var qLineas = _context.LineaCreditos.AsQueryable();

                if (tieneDocumento)
                    qLineas = qLineas.Where(x => x.Documento.Contains(request.Documento!));
                if (tieneApePat)
                    qLineas = qLineas.Where(x => x.RazonSocial != null && x.RazonSocial.Contains(request.ApePat!));
                if (tieneApeMat)
                    qLineas = qLineas.Where(x => x.RazonSocial != null && x.RazonSocial.Contains(request.ApeMat!));
                if (tienePrenombres)
                    qLineas = qLineas.Where(x => x.RazonSocial != null && x.RazonSocial.Contains(request.Prenombres!));

                lineasCredito = await qLineas.ToListAsync();
            }

            // --- Movil: incluye Telefono, además de los demás ---
            if (tieneDocumento || tieneApePat || tieneApeMat || tienePrenombres || tieneTelefono)
            {
                var qMoviles = _context.Movil.AsQueryable();

                if (tieneDocumento)
                    qMoviles = qMoviles.Where(x => x.Documento.Contains(request.Documento!));
                if (tieneApePat)
                    qMoviles = qMoviles.Where(x => x.ApePat != null && x.ApePat.Contains(request.ApePat!));
                if (tieneApeMat)
                    qMoviles = qMoviles.Where(x => x.ApeMat != null && x.ApeMat.Contains(request.ApeMat!));
                if (tienePrenombres)
                    qMoviles = qMoviles.Where(x => x.Prenombres != null && x.Prenombres.Contains(request.Prenombres!));
                if (tieneTelefono)
                    qMoviles = qMoviles.Where(x => x.Telefono != null && x.Telefono.Contains(request.Telefono!));

                moviles = await qMoviles.ToListAsync();
            }

            // --- Filtro adicional por tipo de documento (exacto), si aplica ---
            if (tieneDocumento && EsLongitudValidaParaTipo(request.Documento!, tipoDocumento))
            {
                calificaciones = calificaciones.Where(x => x.Documento == request.Documento).ToList();
                deudas = deudas.Where(x => x.Documento == request.Documento).ToList();
                lineasCredito = lineasCredito.Where(x => x.Documento == request.Documento).ToList();
                moviles = moviles.Where(x => x.Documento == request.Documento).ToList();
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

