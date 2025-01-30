using DPDMIS_Angular_API.Data;
using DPDMIS_Angular_API.MariadbDTO;
using DPDMIS_Angular_API.Services.IntegrationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace DPDMIS_Angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataTransferController : ControllerBase
    {
        private readonly DataTransferService _dataTransferService;

        public DataTransferController(DataTransferService dataTransferService)
        {
            _dataTransferService = dataTransferService;
        }

        [HttpPost("TransferData_masitems_O2M")]
        public async Task<IActionResult> TransferData_masitems_O2M()
        {
            await _dataTransferService.TransferDataAsync_masitems();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masdistricts_O2M")]
        public async Task<IActionResult> TransferData_masdistricts_O2M()
        {
            await _dataTransferService.TransferDataAsync_masdistricts();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masItemTypes_O2M")]
        public async Task<IActionResult> TransferData_masItemTypes_O2M()
        {
            await _dataTransferService.TransferDataAsync_masitemtypes();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masItemCategories_O2M")]
        public async Task<IActionResult> TransferData_masItemCategories_O2M()
        {
            await _dataTransferService.TransferDataAsync_masitemcategories();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masFacilityTypes_O2M")]
        public async Task<IActionResult> TransferData_masFacilityTypes_O2M()
        {
            await _dataTransferService.TransferDataAsync_masfacilitytypes();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masFacilities_O2M")]
        public async Task<IActionResult> TransferData_masFacilities_O2M()
        {
            await _dataTransferService.TransferDataAsync_masfacilities();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_usrUsers_O2M")]
        public async Task<IActionResult> TransferData_usrUsers_O2M()
        {
            await _dataTransferService.TransferDataAsync_usrusers();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masfacilitywh_O2M")]
        public async Task<IActionResult> TransferData_masfacilitywh_O2M()
        {
            await _dataTransferService.TransferDataAsync_masfacilitywh();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_maswarehouses_O2M")]
        public async Task<IActionResult> TransferData_maswarehouses_O2M()
        {
            await _dataTransferService.TransferDataAsync_maswarehouses();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masitemmaincategory_O2M")]
        public async Task<IActionResult> TransferData_masitemmaincategory_O2M()
        {
            await _dataTransferService.TransferDataAsync_masitemmaincategory();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masitemgroups_O2M")]
        public async Task<IActionResult> TransferData_masitemgroups_O2M()
        {
            await _dataTransferService.TransferDataAsync_masitemgroups();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masfacilitywards_O2M")]
        public async Task<IActionResult> TransferData_masfacilitywards_O2M()
        {
            await _dataTransferService.TransferDataAsync_masfacilitywards();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masprogram_O2M")]
        public async Task<IActionResult> TransferData_masprogram_O2M()
        {
            await _dataTransferService.TransferDataAsync_masprogram();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masaccyearsettings_O2M")]
        public async Task<IActionResult> TransferData_masaccyearsettings_O2M()
        {
            await _dataTransferService.TransferDataAsync_masaccyearsettings();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_maslocations_O2M")]
        public async Task<IActionResult> TransferData_maslocations_O2M()
        {
            await _dataTransferService.TransferDataAsync_maslocations();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_usrroles_O2M")]
        public async Task<IActionResult> TransferData_usrroles_O2M()
        {
            await _dataTransferService.TransferDataAsync_usrroles();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masedl_O2M")]
        public async Task<IActionResult> TransferData_masedl_O2M()
        {
            await _dataTransferService.TransferDataAsync_masedl();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferData_masfacheaderfooter_O2M")]
        public async Task<IActionResult> TransferData_masfacheaderfooter_O2M()
        {
            await _dataTransferService.TransferDataAsync_masfacheaderfooter();
            return Ok("Data transfer completed successfully.");
        }

        [HttpPost("TransferDataAsync_Freeze_MASCGMSCNOC_M2O")]
        public async Task<IActionResult> TransferDataAsync_Freeze_MASCGMSCNOC_M2O(Int64 nocid)
        {
            await _dataTransferService.TransferDataAsync_Freeze_MASCGMSCNOC(nocid);
            return Ok("Freeze and Data Update in MariaDB and Oracle Successfully.");
        }
    }
}
