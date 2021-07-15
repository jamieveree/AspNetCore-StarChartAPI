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

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new {id = celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject celestialObject)
        {
            var target = _context.CelestialObjects.SingleOrDefault(co => co.Id == id);

            if (target == null)
            {
                return NotFound();
            }

            _context.Attach(target);

            target.Name = celestialObject.Name;
            target.OrbitalPeriod = celestialObject.OrbitalPeriod;
            target.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var target = _context.CelestialObjects.SingleOrDefault(co => co.Id == id);

            if (target == null)
            {
                return NotFound();
            }

            _context.Attach(target);

            target.Name = name;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var targets = _context.CelestialObjects.Where(co => co.Id == id || co.OrbitedObjectId == id).ToList();

            if (targets.Count == 0)
            {
                return NotFound();
            }

            _context.RemoveRange(targets);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
