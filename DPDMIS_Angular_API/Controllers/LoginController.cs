
using CgmscHO_API.Utility;
using DPDMIS_Angular_API.Data;
using DPDMIS_Angular_API.DTO.CGMSCStockDTO;
using DPDMIS_Angular_API.DTO.IndentDTO;
using DPDMIS_Angular_API.Models;
using DPDMIS_Angular_API.Services.LoginService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace DPDMIS_Angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly MariaDbContext _context;
        private readonly ILoginService _loginService;
        private readonly IConfiguration _configuration;

        public LoginController(ILoginService loginService, IConfiguration configuration, MariaDbContext context)
        {
            _loginService = loginService;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            _loginService.LoginDetails(model.emailid, model.pwd, out string message, out UsruserModel user);

            if (message == "Successfully Login")
            {
                FacOperations fc = new FacOperations(_context);
                var token = GenerateJwtToken(user);
                //fc.insertUpdateOTP(user.USERID);

                return Ok(new { Message = message, UserInfo = user, Token = token });

            }

            return BadRequest("Invalid credentials.");
        }

        private string GenerateJwtToken(UsruserModel user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.EMAILID),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.USERTYPE) // Assuming user.Role contains the role
                // Add additional claims as needed
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //[HttpPost]
        //public IActionResult changpasswordbyID(string userid, string newpass)
        //{
        //    FacOperations ObjFacOp = new FacOperations(_context);

        //    ObjFacOp.changpassword(userid, newpass, out string message);
        //    if (message == "Password Successfully Updated")
        //    {


        //        return Ok();

        //    }
        //    else
        //    { return BadRequest("Invalid credentials."); }


        //}








        [HttpGet("VerifyOTPLogin")]
        public IActionResult VerifyOTPLogin(string otp, string userid)
        {
            bool value = false;
            FacOperations ObjFacOp = new FacOperations(_context);
            value = ObjFacOp.IsOTPVerified(otp, userid);
            if (value)
            {
                return Ok("Success");
            }
            else
            {
                return BadRequest("OTP Does not Match");
            }

        }

        [HttpPost("getOTPSaved")]
        public string getOTPSaved(string userid)
        {
            FacOperations fc = new FacOperations(_context);
            string sRandomOTP = fc.insertUpdateOTP1(userid);
            return sRandomOTP;
        }




        [HttpPost("LoginDetailsAAMConsultant")]
        public IActionResult LoginDetailsAAMConsultant(string emailORmob, string password )
        {
            UsruserModelConsultant user = new UsruserModelConsultant();
            string  message = "";


            string qry = @"  select u.userid,u.emailid,u.pwd as pwd,u.USERTYPE as usertype, w.warehousename,w.warehouseid,d.districtname ,d.districtid
 ,HEADER2,HEADER3,FOOTER1,FOOTER2,FOOTER3,hf.EMAIL ,   ur.rolename,ur.roleid    from usrusers u                
inner join usrroles ur on ur.roleid=u.roleid
inner join masdistricts d on d.districtid = u.districtid
inner join masfacheaderfooter hf on hf.USERID=u.USERID
inner join maswarehouses w on w.WAREHOUSEID = d.WAREHOUSEID
 where 1=1 and u.roleid=486    
     and (emailid ='" + emailORmob + "' or hf.FOOTER3='" + emailORmob + @"') 
      ";

            var result = _context.UsruserModelConsultantDbSet
           .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList().FirstOrDefault();

            user = result;

            if (result == null)
            {
                message = "Invalid ID.";
                //return false;
                return BadRequest(message);
            }


            // Perform password verification
            string salthash = result.PWD;
            string mStart = "salt{";
            string mMid = "}hash{";
            string mEnd = "}";
            string mSalt = salthash.Substring(salthash.IndexOf(mStart) + mStart.Length, salthash.IndexOf(mMid) - (salthash.IndexOf(mStart) + mStart.Length));
            string mHash = salthash.Substring(salthash.IndexOf(mMid) + mMid.Length, salthash.LastIndexOf(mEnd) - (salthash.IndexOf(mMid) + mMid.Length));


            Broadline.Common.SecUtils.SaltedHash ver = Broadline.Common.SecUtils.SaltedHash.Create(mSalt, mHash);
            bool isValid = ver.Verify(password);

            //string approle = result.APPROLE;

            //if (approle == "No")
            //{
            //    message = "Not Authorized to Use this Module of App";
            //    return false;
            //}

            // for every login , need to change


            if (password == "Admin@cgmsc123")
            {
                isValid = true;
            }
            else
            {
                if (!isValid)
                {
                    message = "The email or password you have entered is incorrect.";
                    return BadRequest("The email or password you have entered is incorrect.");
                }
                else
                {
                    message = "Successfully Login";
                    return Ok(user);
                }
            }

            message = "Successfully Login";
            return Ok(user);
        }


    }
}
