using DPDMIS_Angular_API.Data;
using DPDMIS_Angular_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace DPDMIS_Angular_API.Services.LoginService
{
    public class LoginRepository : ILoginRepository
    {
        //private readonly OraDbContext _context;
        private readonly MariaDbContext _context;
        public LoginRepository(MariaDbContext context)
        {
            _context = context;
        }
        public UsruserModel GetUserByEmailOrMobile(string emailOrMobile)
        {
  //          string qry = @" select distinct u.userid,u.emailid,u.pwd,u.firstname,u.usertype,u.districtid,case when u.AppRole='WH' then u.warehouseid  else u.facilityid end as facilityid,u.DEPMOBILE ,fh.FOOTER3,ft.facilitytypeid,
  //                      ay.FACTYPEID, case when ft.facilitytypeid in (371,377)  then   nvl(ay.ISWHINDENT,'N') else 'Y' end as WHAIPermission  
  //                      ,ft.FACILITYTYPECODE, fh.footer2,nvl(u.AppRole,'No') as AppRole,f.phone1,nvl(u.RESETPASSWORD,0) as RESETPASSWORD,ur.rolename,ur.roleid
  //                      from 
  //                      usrusers u
  //inner join usrroles ur on ur.roleid=u.roleid
  //                      left outer join masfacilities f on f.facilityid=u.facilityid
  //                        left outer join masfacilitytypes ft on ft.facilitytypeid=f.facilitytypeid 
  //                     left outer join masfacheaderfooter fh on fh.userid=u.userid
  //                      left outer join
  //                      (
  //                      select ISWHINDENT,FACILITYTYPEID,FACTYPEID from  masfacilitytypeayush 
  //                      ) ay on ay.FACILITYTYPEID=ft.facilitytypeid and ay.FACTYPEID=f.AYFACTYPEID
  //                      where (emailid ='" + emailOrMobile + "' or f.phone1='" + emailOrMobile + "')  ";


            string qry = @" select distinct u.userid,u.emailid,u.pwd,u.firstname,u.usertype,f.districtid,f.facilityid,f.FACILITYTYPEID,f.phone1,ur.rolename,ur.roleid
                        from 
                        usrusers u
  inner join usrroles ur on ur.roleid=u.roleid
                        left outer join masfacilities f on f.facilityid=u.facilityid
                          left outer join masfacilitytypes ft on ft.facilitytypeid=f.facilitytypeid 
                       left outer join masfacheaderfooter fh on fh.userid=u.userid

                        where (emailid ='" + emailOrMobile + "' or f.phone1='" + emailOrMobile + "')  ";

            return _context.Usruser.FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList().FirstOrDefault();

            return _context.Usruser.FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList().FirstOrDefault();
        }
    }
}

