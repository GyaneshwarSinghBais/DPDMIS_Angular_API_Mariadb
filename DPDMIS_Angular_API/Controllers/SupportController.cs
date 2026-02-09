using DPDMIS_Angular_API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DPDMIS_Angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportController : ControllerBase
    {
        private readonly MariaDbContext _context;
        private readonly OraDbContext _contextoracle;
        public SupportController(MariaDbContext context, OraDbContext contextoracle)
        {
            _context = context;
            _contextoracle = contextoracle;
        }


        [HttpPut("upateIncompleteBasedOnReceiptIdAndId")]
        public IActionResult upateIncompleteBasedOnReceiptIdAndId(string facReceiptType, Int64 facilityId)
        {
            string strQuery = @"
        UPDATE tbfacilityreceipts
        SET STATUS = 'I'
        WHERE facreceipttype = '" + facReceiptType + @"'
        AND facilityid = " + facilityId;

            Int32 result = _context.Database.ExecuteSqlRaw(strQuery);

            if (result > 0)
            {
                return Ok("Facility receipts successfully invalidated");
            }
            else
            {
                return BadRequest("No records updated");
            }
        }

        [HttpPut("upateIncompleteBasedOnReceiptIdAndFacilityId")]
        public IActionResult upateIncompleteBasedOnReceiptIdAndFacilityId(Int64 facReceiptId, Int64 facilityId)
        {
            string strQuery = @"
        UPDATE tbfacilityreceipts
        SET STATUS = 'I'
        WHERE facreceiptid = " + facReceiptId + @"
        AND facilityid = " + facilityId;

            Int32 result = _context.Database.ExecuteSqlRaw(strQuery);

            if (result > 0)
            {
                return Ok("Facility receipt successfully invalidated");
            }
            else
            {
                return BadRequest("No records updated");
            }
        }

        [HttpPut("updateIncompleteBasedOnIssueIdAndFacilityId")]
        public IActionResult updateIncompleteBasedOnIssueIdAndFacilityId(Int64 issueId, Int64 facilityId)
        {
            string strQuery = @"
        UPDATE tbfacilityissues
        SET STATUS = 'IN'
        WHERE issueid = " + issueId + @"
        AND facilityid = " + facilityId;

            Int32 result = _context.Database.ExecuteSqlRaw(strQuery);

            if (result > 0)
            {
                return Ok("Facility issue successfully invalidated");
            }
            else
            {
                return BadRequest("No records updated");
            }
        }


        [HttpPut("resetPasswordByFacilityId")]
        public IActionResult resetPasswordByFacilityId(Int64 facilityId)
        {
            string strQuery = @"
        UPDATE usrusers
        SET pwd = 'salt{E+LZpyYr}hash{xZriCkh9iCIH11qR2HAxk+A+kU4=}'
        WHERE facilityid = " + facilityId;

            Int32 result = _context.Database.ExecuteSqlRaw(strQuery);

            if (result > 0)
            {
                return Ok("Password updated successfully based on facilityId");
            }
            else
            {
                return BadRequest("No records updated");
            }
        }


        [HttpPut("resetPasswordByUserId")]
        public IActionResult resetPasswordByUserId(Int64 userId)
        {
            string strQuery = @"
        UPDATE usrusers
        SET pwd = 'salt{E+LZpyYr}hash{xZriCkh9iCIH11qR2HAxk+A+kU4=}'
        WHERE userid = " + userId;

            Int32 result = _context.Database.ExecuteSqlRaw(strQuery);

            if (result > 0)
            {
                return Ok("Password updated successfully based on userId");
            }
            else
            {
                return BadRequest("No records updated");
            }
        }

        [HttpPut("resetPasswordByMobile")]
        public IActionResult resetPasswordByMobile(Int64 depMobile)
        {
            string strQuery = @"
        UPDATE usrusers
        SET pwd = 'salt{E+LZpyYr}hash{xZriCkh9iCIH11qR2HAxk+A+kU4=}'
        WHERE depmobile = " + depMobile;

            Int32 result = _context.Database.ExecuteSqlRaw(strQuery);

            if (result > 0)
            {
                return Ok("Password updated successfully based on mobile number");
            }
            else
            {
                return BadRequest("No records updated");
            }
        }

        [HttpPut("resetPasswordByEmail")]
        public IActionResult resetPasswordByEmail(string emailId)
        {
            string strQuery = @"
        UPDATE usrusers
        SET pwd = 'salt{E+LZpyYr}hash{xZriCkh9iCIH11qR2HAxk+A+kU4=}'
        WHERE emailid = '" + emailId + "'";

            Int32 result = _context.Database.ExecuteSqlRaw(strQuery);

            if (result > 0)
            {
                return Ok("Password updated successfully based on emailId");
            }
            else
            {
                return BadRequest("No records updated");
            }
        }

    }
}
