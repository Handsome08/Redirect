using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Text.Encodings.Web.Utf8;
using Microsoft.EntityFrameworkCore;
using Redirect.Models;
using UrlEncoder = System.Text.Encodings.Web.UrlEncoder;

namespace Redirect.Controllers
{
    [Produces("application/json")]
    [Route("api/UriModels")]
    public class UriModelsController : Controller
    {
        private readonly RedirectContext _context;

        public UriModelsController(RedirectContext context)
        {
            _context = context;
        }

        // GET: api/UriModels
        [HttpGet]
        public IEnumerable<UriModel> GetUriModel()
        {
            return _context.UriModel;
        }

        // GET: api/UriModels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUriModel([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var uriModel = await _context.UriModel.SingleOrDefaultAsync(m => m.Id == id);

            if (uriModel == null)
            {
                return NotFound();
            }

            return Ok(uriModel);
        }

        [Route("builduri")]
        [HttpGet("{uri}")]
        public string GetBuildUri(string uri)
        {
            Regex regx = new Regex(@"\b(?<=(http|https)://)[\w- ./?%&=]*$");
            string encode = regx.Replace(uri, new MatchEvaluator(ReplaceURl));

            string ReplaceURl(Match m)
            {
                string x = m.ToString();
                x = Uri.EscapeUriString(x);
                return x;
            }
            var uriModel = new UriModel(){Id = Guid.NewGuid().ToString(),Uri = encode };
            _context.UriModel.Add(uriModel);
            _context.SaveChanges();
            var request = this.HttpContext.Request;
            var path = $"{request.Host.ToUriComponent()}/api/UriModels/redirect?id={uriModel.Id}";
            return $"{request.Scheme}://{path}";
        }


        [Route("redirect")]
        [HttpGet("{id}")]
        public IActionResult GetRedirect(string id)
        {
            var uriModel = _context.UriModel.Find(id);
            var redirectResult = Redirect(uriModel.Uri);
            return redirectResult;
        }

        // PUT: api/UriModels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUriModel([FromRoute] string id, [FromBody] UriModel uriModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != uriModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(uriModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UriModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UriModels
        [HttpPost]
        public async Task<IActionResult> PostUriModel([FromBody] UriModel uriModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UriModel.Add(uriModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUriModel", new { id = uriModel.Id }, uriModel);
        }

        // DELETE: api/UriModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUriModel([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var uriModel = await _context.UriModel.SingleOrDefaultAsync(m => m.Id == id);
            if (uriModel == null)
            {
                return NotFound();
            }

            _context.UriModel.Remove(uriModel);
            await _context.SaveChangesAsync();

            return Ok(uriModel);
        }

        private bool UriModelExists(string id)
        {
            return _context.UriModel.Any(e => e.Id == id);
        }
    }
}