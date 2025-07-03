using Microsoft.AspNetCore.Mvc;

namespace AssetService.Controllers
{
    [ApiController]
    [Route("api/assets")]
    public class AssetController : ControllerBase
    {
        private static List<string> _assets = new() { "Asset 1", "Asset 2" };

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_assets);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (id < 0 || id >= _assets.Count)
                return NotFound();

            return Ok(_assets[id]);
        }

        [HttpPost]
        public IActionResult Create([FromBody] string asset)
        {
            _assets.Add(asset);
            return CreatedAtAction(nameof(GetById), new { id = _assets.Count - 1 }, asset);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] string asset)
        {
            if (id < 0 || id >= _assets.Count)
                return NotFound();

            _assets[id] = asset;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id < 0 || id >= _assets.Count)
                return NotFound();

            _assets.RemoveAt(id);
            return NoContent();
        }
    }
}