using CgmscHO_API.Utility;
using DPDMIS_Angular_API.Data;
using DPDMIS_Angular_API.DTO.IndentDTO;
using DPDMIS_Angular_API.DTO.SourceDTO;
using DPDMIS_Angular_API.MariadbDTO.DestinationDTO;
using DPDMIS_Angular_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace DPDMIS_Angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OraController : ControllerBase
    {
        private readonly OraDbContext _context;
       // private readonly MariaDbContext _context;

        public OraController(OraDbContext context)
        {
            _context = context;
        }

        [HttpPost("postOtherFacIndentitems")]
        public IActionResult postOtherFacIndentitems(OraMasFacDemandItemsDTO objMasFacDemandItems)
        {
            Int32? indentId = null;
            string qry = @"  select indentid from MASFACTRANSFERS where mtransferid = "+ objMasFacDemandItems.INDENTID + " ";
            //(23413)
            var myList = _context.ORAMASFACTRANSFERSINDENT_DbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            if (myList.Any())
            {
                indentId = Convert.ToInt32(myList[0].INDENTID) ;
              
            }
            else
            {              
                return BadRequest("myList is empty.");
            }


            objMasFacDemandItems.STATUS = "T";
            objMasFacDemandItems.APPROVEDQTY = objMasFacDemandItems.REQUESTEDQTY;
            objMasFacDemandItems.INDENTID = indentId;

            try
            {
                if (objMasFacDemandItems.INDENTITEMID == 0)
                {
                    _context.OraMasFacDemandItemsDbSet.Add(objMasFacDemandItems);
                }
                else
                {
                    // Convert SR to long for comparison
                    // If SR is not "0", update the existing entry

                    //var existingItem = _context.OraMasFacDemandItemsDbSet.FirstOrDefault(x => x.INDENTITEMID == objMasFacDemandItems.INDENTITEMID);
                    var existingItem = _context.OraMasFacDemandItemsDbSet.FirstOrDefault(x => x.INDENTID == objMasFacDemandItems.INDENTID && x.ITEMID == objMasFacDemandItems.ITEMID);


                    if (existingItem != null)
                    //if(true)
                    {
                        existingItem.INDENTID = objMasFacDemandItems.INDENTID;
                        existingItem.ITEMID = objMasFacDemandItems.ITEMID;
                        existingItem.REQUESTEDQTY = objMasFacDemandItems.REQUESTEDQTY;
                        existingItem.FACSTOCK = objMasFacDemandItems.FACSTOCK;
                        existingItem.APPROVEDQTY = objMasFacDemandItems.APPROVEDQTY;
                        existingItem.STATUS = objMasFacDemandItems.STATUS;
                        existingItem.STOCKINHAND = objMasFacDemandItems.STOCKINHAND;
                        //existingItem.MTRANSFERID = objMasFacDemandItems.MTRANSFERID;

                    }
                    else
                    {
                        return NotFound("Item with the specified SR not found.");
                    }

                }

                // Save changes to the database
                _context.SaveChanges();
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("postOtherFacilityIndent1")]
        public async Task<ActionResult<IEnumerable<IncompleteWardIssueDTO>>> postOtherFacilityIndent1(Int64 facid, string indentDt, Int64 programid, Int64 Target_facid, Int64 mTranferId) //programid = 1
        {
            //try
            //{
            OraFacOperations ob = new OraFacOperations(_context);
            MASFACTRANSFERS objSaveIndent = new MASFACTRANSFERS();
            string indentDate = ob.FormatDate(indentDt);

            string faccode = ob.getFacCodeForSHCIndent(facid);
            string yearcode = ob.getSHAccYear();
            string yearId = ob.getACCYRSETID();
            string autono = facid.ToString() + "/SH" + faccode + "/" + yearcode;
            //objeIssue.ISSUENO = issueno;
            objSaveIndent.INDENTDATE = Convert.ToDateTime(indentDate);
            objSaveIndent.INDENTNO = autono;
            objSaveIndent.FACILITYID = facid;
            objSaveIndent.FROMFACILITYID = Target_facid;
            objSaveIndent.ACCYRSETID = Convert.ToInt64(yearId);
            objSaveIndent.PROGRAMID = programid;
            objSaveIndent.STATUS = "I";
            objSaveIndent.AUTO_CODE = faccode;
            objSaveIndent.MTRANSFERID = mTranferId;



            _context.MASFACTRANSFERSDbSet.Add(objSaveIndent);
            _context.SaveChanges();


            var myObj = GetOraFacDetailSHC(facid, autono);
            return Ok(myObj);
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(ex.Message.ToString());
            //}
        }

        [HttpGet("GetOraFacDetailSHC")]
        public async Task<ActionResult<IEnumerable<ORAMASFACTRANSFERS_DTO>>> GetOraFacDetailSHC(Int64 facid, string indentNo)
        {
            string qry = @"  select INDENTID, INDENTNO, INDENTDATE, FACILITYID, FROMFACILITYID, DISPATCHNO, DISPATCHDATE, REMARKS, PROGRAMID, AUTO_CODE, ENTRYDATE, ACCYRSETID, STATUS,MTRANSFERID from MASFACTRANSFERS
        where INDENTNO = '" + indentNo + "' and FACILITYID = " + facid + " ";
            //(23413)
            var myList = _context.ORAMASFACTRANSFERS_DbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
            //return Ok(JsonConvert.SerializeObject(myList));
        }

        [HttpPut("completeOtherFacIndent")]
        public IActionResult completeOtherFacIndent(Int64 indentId)
        {

            string qry = "  update MASFACTRANSFERS set STATUS = 'C',ENTRYDATE=sysdate where INDENTID = " + indentId;
            _context.Database.ExecuteSqlRaw(qry);


            return Ok("Successfully Update MASFACTRANSFERS status C");

        }

        [HttpDelete("deleteOtherFaceIdent")]
        public IActionResult deleteOtherFaceIdent(Int64 indentId)
        {
           // select* from masfacdemanditems where INDENTID in (select INDENTID from masfactransfers where  mtransferid = 47)

            string qry = " delete from masfacdemanditems where INDENTID in (select INDENTID from masfactransfers where  mtransferid = "+ indentId + ")  "  ;
            _context.Database.ExecuteSqlRaw(qry);

            string qry1 = "   delete from MASFACTRANSFERS where mtransferid =  " + indentId;
            _context.Database.ExecuteSqlRaw(qry1);

            return Ok("Successfully Deleted other facility Indent & Indented items");

        }
    }
}
