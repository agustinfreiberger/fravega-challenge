using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PromocionesAPI.Models
{
    public class Promocion
    {
        public Guid Id { get; set; }
        public IEnumerable<string> MediosDePago { get; set; }
        public IEnumerable<string> Bancos { get;  set; }
        public IEnumerable<string> CategoriasProductos { get;  set; }
        public int? MaximaCantidadDeCuotas { get;  set; }
        public decimal? ValorInteresCuotas { get;  set; }
        public decimal? PorcentajeDeDescuento { get;  set; }
        public DateTime? FechaInicio { get;  set; }
        public DateTime? FechaFin { get;  set; }

        public bool Activo { get;  set; }
        public DateTime FechaCreacion { get;  set; }
        public DateTime? FechaModificacion { get;  set; }

    }
}
