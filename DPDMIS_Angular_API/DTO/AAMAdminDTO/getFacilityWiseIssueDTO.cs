namespace DPDMIS_Angular_API.DTO.AAMAdminDTO
{
    public class getFacilityWiseIssueDTO
    {
        public Int64 Id { get; set; }
        public Int64? ITEMID { get; set; }
        public string? ITEMCODE { get; set; }
        public string? ITEMNAME { get; set; }
        public string? STRENGTH1 { get; set; }
        public Int64? ISSUEQTY { get; set; }
        public DateTime? ISSUEDATE { get; set; }
        public Int64? WARDID { get; set; }
        public string? WARDNAME { get; set; }
        public string? INWNO { get; set; }
        public string? BATCHNO { get; set; }
        public string? MFGDATE { get; set; }
        public string? EXPDATE { get; set; }
        public string? FACILITYNAME { get; set; }
        public Int64? FACILITYID { get; set; }
        public string? DISTRICTNAME { get; set; }
        public string? LOCATIONNAME { get; set; }
        public Int64? DISTRICTID { get; set; }
        public Int64? LOCATIONID { get; set; }
    }
}
