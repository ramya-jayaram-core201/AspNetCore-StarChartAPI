using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            //check if id exists
            var data = _context.CelestialObjects.Find(id);
            if (data == null)
                return NotFound();

            //assign Satellite to selected id
            data.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();
            return Ok(data);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var result = _context.CelestialObjects.Where(x => x.Name == name);
            if (!result.Any())
                return NotFound();
            //assign Satellite to selected id
            foreach (var iterateObject in result)
            {
                iterateObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == iterateObject.Id).ToList();
            }
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _context.CelestialObjects;

            //assign Satellite to selected id
            foreach (var iterateObject in result)
            {
                iterateObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == iterateObject.Id).ToList();
            }
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestailObject)
        {
            var data = _context.CelestialObjects.Find(id);
            if (data == null)
                return NotFound();

            data.Name = celestailObject.Name;
            data.OrbitalPeriod = celestailObject.OrbitalPeriod;
            data.OrbitedObjectId = celestailObject.OrbitedObjectId;
            _context.CelestialObjects.Update(data);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var selectedCelestialObject = _context.CelestialObjects.Find(id);
            if (selectedCelestialObject == null)
                return NotFound();

            selectedCelestialObject.Name = name;
            _context.Update(selectedCelestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var selectedCelestialObject = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id);
            if (!selectedCelestialObject.Any())
                return NotFound();

            _context.CelestialObjects.RemoveRange(selectedCelestialObject);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
