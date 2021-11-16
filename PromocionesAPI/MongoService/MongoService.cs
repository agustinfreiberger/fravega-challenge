using MongoDB.Driver;
using PromocionesAPI.Models;
using PromocionesAPI.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromocionesAPI.MongoService
{
    public class MongoService : IMongoService
    {
        private MongoClient _mongoClient = null;
        private IMongoDatabase _database = null;
        private IMongoCollection<Promocion> _promocionTable = null;
        private List<string> MediosDePago = new()
        {
            "TARJETA_CREDITO",
            "TARJETA_DEBITO",
            "EFECTIVO",
            "GIFT_CARD"
        };
        private List<string> Bancos = new()
        {
            "Galicia", 
            "Santander Rio", 
            "Ciudad", 
            "Nacion", 
            "ICBC",
            "BBVA",
            "Macro"
        };
        private List<string> CategoriaProductos = new() {
            "Hogar",
            "Jardin",
            "ElectroCocina",
            "GrandesElectro",
            "Colchones",
            "Celulares",
            "Tecnologia",
            "Audio"
        };
        public MongoService(MongoSettings mongoSettings)
        {
            var settings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
            _mongoClient = new MongoClient(settings);
            _database = _mongoClient.GetDatabase(mongoSettings.DbName);
            _promocionTable = _database.GetCollection<Promocion>(mongoSettings.CollectionName);
        }

        public string CrearPromocion(Promocion p)
        {
            {
                if (ValidacionesPromocion(p))
                {
                    if (ValidacionSolapamiento(p))
                    {
                        if (ChequeoValidos(p)) {
                            Promocion prom = new Promocion() {
                                Id = Guid.NewGuid(),
                                MediosDePago = p.MediosDePago,
                                Bancos = p.Bancos,
                                CategoriasProductos = p.CategoriasProductos,
                                MaximaCantidadDeCuotas = p.MaximaCantidadDeCuotas,
                                PorcentajeDeDescuento = p.PorcentajeDeDescuento,
                                ValorInteresCuotas = p.ValorInteresCuotas,
                                FechaInicio = p.FechaInicio,
                                FechaFin = p.FechaFin
                            };
                            _promocionTable.InsertOneAsync(prom);
                            return prom.Id.ToString();
                        }
                        else {
                            return "Los valores ingresados no son válidos";
                        }
                    }else{
                        return "Se solapa con otra promoción";
                    }
                }else{
                    return "No cumple validaciones";
                }
            }
        }

        public string ModificarPromocion(Promocion p)
        {

            UpdateDefinition<Promocion> update = Builders<Promocion>.Update.Set(x => x.MediosDePago, p.MediosDePago)
                                                .Set(x => x.CategoriasProductos, p.CategoriasProductos)
                                                .Set(x => x.PorcentajeDeDescuento, p.PorcentajeDeDescuento)
                                                .Set(x => x.MaximaCantidadDeCuotas, p.MaximaCantidadDeCuotas)
                                                .Set(x => x.ValorInteresCuotas, p.ValorInteresCuotas)
                                                .Set(x => x.FechaInicio, p.FechaInicio)
                                                .Set(x => x.FechaFin, p.FechaFin);

            if (ValidacionSolapamiento(p))
            {
                var result = _promocionTable.UpdateOneAsync(x => x.Id == p.Id,update);
                return p.Id.ToString();

            }

            return "Error de solapamiento"; 
        }

        public string ModificarVigenciaPromocion(Guid Id, DateTime fechaInicio, DateTime fechaFin)
        {
            var builder = Builders<Promocion>.Filter;
            var filter = builder.Or(builder.And(builder.Lte("FechaInicio", fechaInicio), builder.Gt("FechaInicio", fechaFin)), builder.And(builder.Lt("FechaFin", fechaInicio), builder.Gte("FechaFin", fechaFin)), builder.And(builder.Gte("FechaInicio", fechaInicio), builder.Lte("FechaFin", fechaFin)));

            var result = _promocionTable.Find(filter).ToList();

            if (result.Count == 0)
            {
                var update = Builders<Promocion>.Update.Set(x => x.FechaInicio, fechaInicio)
                                                    .Set(x => x.FechaFin, fechaFin);

                 _promocionTable.UpdateOneAsync(x => x.Id == Id, update);
                return Id.ToString();
            }
            return "Error de solapamiento";
        }

        public string EliminarPromocion(Guid promId)
        {
            if (_promocionTable.DeleteOne(x => x.Id == promId).DeletedCount > 0)
            {
                return "Borrado con exito";
            }
            return "No existe el documento";
        }

        public Promocion Get(Guid promocionId)
        {
            return _promocionTable.Find(x => x.Id == promocionId).FirstOrDefault();
        }

        public List<Promocion> Gets()
        {
            return _promocionTable.Find(FilterDefinition<Promocion>.Empty).ToList();
        }

        public List<Promocion> GetPromocionsVigentes()
        {
            var filter = Builders<Promocion>.Filter.Where(x => x.FechaFin > DateTime.UtcNow && DateTime.UtcNow > x.FechaInicio);
            return _promocionTable.Find(filter).ToList();
        }

        public List<Promocion> GetPromocionsVigentesParaFecha(DateTime fecha)
        {
            var filter = Builders<Promocion>.Filter.Where(x => x.FechaFin > fecha && fecha > x.FechaInicio);
            return _promocionTable.Find(filter).ToList();
        }

        public List<Promocion> GetPromocionsVigentesParaVenta(string mediodePago, string banco, IEnumerable<string> categorias)
        {
            //para la comparacion de las categorias asumo las secuencias tienen que ser iguales
            var filter = Builders<Promocion>.Filter.Where(x => x.MediosDePago.Contains(mediodePago) && x.Bancos.Contains(banco) && x.CategoriasProductos.SequenceEqual(categorias));
            return _promocionTable.Find(filter).ToList();
        }

        private bool ValidacionesPromocion(Promocion prom) {
            if (prom.FechaInicio <= prom.FechaFin) {   //el challenge decia "Fecha fin no puede ser mayor que fecha inicio", entiendo que esto es al revés
                if (prom.PorcentajeDeDescuento != null) {
                    if (prom.PorcentajeDeDescuento > 5 && prom.PorcentajeDeDescuento < 80){
                        return true;
                    } else { 
                        return false;
                    }
                } else if (prom.MaximaCantidadDeCuotas == null) {
                    return false;
                } else {
                    return true;
                }
            }
            return false;
        }

        private bool ValidacionSolapamiento(Promocion p) {
            var builder = Builders<Promocion>.Filter;
            var filter = builder.And(builder.Or(builder.And(builder.Lte("FechaInicio", p.FechaInicio), builder.Gt("FechaInicio", p.FechaFin)), builder.And(builder.Lt("FechaFin", p.FechaInicio), builder.Gte("FechaFin", p.FechaFin)), builder.And(builder.Gte("FechaInicio", p.FechaInicio), builder.Lte("FechaFin", p.FechaFin))), builder.Or(builder.AnyIn("MediosDePago", p.MediosDePago), builder.AnyIn("Bancos", p.Bancos), builder.AnyIn("CategoriasProductos", p.CategoriasProductos)));

            var result = _promocionTable.Find(filter).ToList();

            if (result.Count == 0) {
                return true;
            }
            return false;
        }

        private bool ChequeoValidos(Promocion p) {
            if (p.Bancos != null) {
                foreach (var banco in p.Bancos)
                {
                    if (!Bancos.Contains(banco)) {
                        return false;
                    }
                }
            }
            if (p.MediosDePago != null)
            {
                foreach (var mediodePago in p.MediosDePago)
                {
                    if (!MediosDePago.Contains(mediodePago))
                    {
                        return false;
                    }
                }
            }
            if (p.CategoriasProductos != null)
            {
                foreach (var categoria in p.CategoriasProductos)
                {
                    if (!CategoriaProductos.Contains(categoria))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
