using Microsoft.AspNetCore.Mvc;
using PromocionesAPI.Models;
using PromocionesAPI.MongoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromocionesAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PromocionesController : Controller
    {
        private IMongoService _promRepo = null;

        public PromocionesController(IMongoService promRepo) {
            _promRepo = promRepo;
        }

        [HttpGet]
        public ActionResult<List<Promocion>> VerPromociones() => _promRepo.Gets();

        [HttpGet]
        public ActionResult<Promocion> VerPromocion(Guid promId) => _promRepo.Get(promId);

        [HttpPost]
        public ActionResult<string> CrearPromocion(Promocion promocion)
        {
            //hago chequeo de valores validos
            return _promRepo.CrearPromocion(promocion);
        }

        [HttpPut]
        public ActionResult<string> ModificarPromocion(Promocion promocion)
        {
            return _promRepo.ModificarPromocion(promocion);
        }

        [HttpPut]
        public ActionResult<string> ModificarVigenciaPromocion(Guid id, DateTime fechaInicio, DateTime fechaFin)
        {
            return _promRepo.ModificarVigenciaPromocion(id, fechaInicio, fechaFin);
        }

        [HttpDelete]
        public ActionResult<string> EliminarPromocion(Guid id) {
            return _promRepo.EliminarPromocion(id);
        }

    }
}
