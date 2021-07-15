using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects
                .Select(co => new CelestialObject
                {
                    Id = co.Id,
                    Name = co.Name,
                    OrbitalPeriod = co.OrbitalPeriod,
                    OrbitedObjectId = co.OrbitedObjectId,
                    Satellites = _context.CelestialObjects.Where(co2 => co2.OrbitedObjectId == co.Id).ToList()
                })
                .SingleOrDefault(obj => obj.Id == id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(obj => obj.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects
                .Where(obj => obj.Name == name)
                .Select(co => new CelestialObject
                {
                    Id = co.Id,
                    Name = co.Name,
                    OrbitalPeriod = co.OrbitalPeriod,
                    OrbitedObjectId = co.OrbitedObjectId,
                    Satellites = _context.CelestialObjects.Where(co2 => co2.OrbitedObjectId == co.Id).ToList()
                })
                .ToList();

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects
                .Select(co => new CelestialObject
                {
                    Id = co.Id,
                    Name = co.Name,
                    OrbitalPeriod = co.OrbitalPeriod,
                    OrbitedObjectId = co.OrbitedObjectId,
                    Satellites = _context.CelestialObjects.Where(co2 => co2.OrbitedObjectId == co.Id).ToList()
                })
                .ToList();

            return Ok(celestialObjects);
        }
    }
}
