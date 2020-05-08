using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WidgetWebAPI.Models;

namespace WidgetWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ODataRoutePrefix("Widget")]
    public class WidgetController : ODataController // ControllerBase
    {
        private readonly WidgetDBContext _context;

        public WidgetController(WidgetDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<Widget> GetWidget() => _context.Widget.AsQueryable();

        //[HttpGet("{id}")]
        //[EnableQuery]
        //public Widget GetSingleWidget([FromODataUri] int id) => _context.Widget.Find(id);


        [EnableQuery]
        [ODataRoute("({id})", RouteName = nameof(GetSingleWidget))]
        public async Task<IActionResult> GetSingleWidget([FromODataUri] int id)
        {
            var widget = await _context.Widget.FindAsync(id);
            return Ok(widget);
        }


        // PUT: api/Widget/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWidget(int id, Widget widget)
        {
            if (id != widget.WidgetId)
            {
                return BadRequest();
            }

            _context.Entry(widget).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WidgetExists(id))
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

        // POST: api/Widget
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Widget>> PostWidget(Widget widget)
        {
            _context.Widget.Add(widget);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWidget", new { id = widget.WidgetId }, widget);
        }

        // DELETE: api/Widget/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Widget>> DeleteWidget(int id)
        {
            var widget = await _context.Widget.FindAsync(id);
            if (widget == null)
            {
                return NotFound();
            }

            _context.Widget.Remove(widget);
            await _context.SaveChangesAsync();

            return widget;
        }

        private bool WidgetExists(int id)
        {
            return _context.Widget.Any(e => e.WidgetId == id);
        }
    }
}
