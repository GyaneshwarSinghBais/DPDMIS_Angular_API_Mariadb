using DPDMIS_Angular_API.Data;
using DPDMIS_Angular_API.MariadbDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace DPDMIS_Angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MariaDbController : ControllerBase
    {
        private readonly MariaDbContext _context;

        public MariaDbController(MariaDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllFacilityTypes")]
        public async Task<ActionResult<IEnumerable<GetAllFacilityTypesDTO>>> GetAllFacilityTypes()
        {
            string qry = @" SELECT * FROM masfacilitytypes ";

            var myList = _context.GetAllFacilityTypesDbSet.FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }
    }
}
