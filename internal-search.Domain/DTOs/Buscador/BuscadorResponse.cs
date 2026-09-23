using internal_search.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search.Domain.DTOs.Buscador
{
    public class BuscadorResponse
    {
        public List<Calificacion> Calificaciones { get; set; } = new();

        public List<Deuda> Deudas { get; set; } = new();

        public List<LineaCredito> LineasCredito { get; set; } = new();

        public List<Movil> Moviles { get; set; } = new();
    }
}
