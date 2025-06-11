using Microsoft.AspNetCore.Mvc;
using SurvivalCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace SurvivalOnWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProficiencyController : ControllerBase
    {
        private static readonly string ProficiencyFilePath = Path.Combine("StaticData", "proficiencies.json");

        // GET: api/proficiency
        [HttpGet]
        public ActionResult<List<Proficiency>> GetAll()
        {
            var proficiencies = LoadAllProficiencies();
            return Ok(proficiencies);
        }

        // GET: api/proficiency/basic
        [HttpGet("basic")]
        public ActionResult<List<Proficiency>> GetAllBasic()
        {
            var proficiencies = Proficiency.GetAllBasicProficiencies(ProficiencyFilePath);
            return Ok(proficiencies);
        }

        // GET: api/proficiency/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<Proficiency> GetById(Guid id)
        {
            var proficiencies = LoadAllProficiencies();
            var proficiency = proficiencies.FirstOrDefault(p => p.UniqueId == id);
            if (proficiency == null)
                return NotFound("Proficiency not found.");
            return Ok(proficiency);
        }

        // Helper to load all proficiencies from file
        private List<Proficiency> LoadAllProficiencies()
        {
            if (!System.IO.File.Exists(ProficiencyFilePath))
                return new List<Proficiency>();
            var json = System.IO.File.ReadAllText(ProficiencyFilePath);
            return System.Text.Json.JsonSerializer.Deserialize<List<Proficiency>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            }) ?? new List<Proficiency>();
        }
    }
}