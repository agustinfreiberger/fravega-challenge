using PromocionesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromocionesAPI.MongoService
{
    public interface IMongoService
    {
        string CrearPromocion(Promocion p);

        string ModificarPromocion(Promocion p);

        string EliminarPromocion(Guid promId);

        string ModificarVigenciaPromocion(Guid Id, DateTime fechaInicio, DateTime fechaFin);
        Promocion Get(Guid promocionId);
        List<Promocion> Gets();
    }
}
