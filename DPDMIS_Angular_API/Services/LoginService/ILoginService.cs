using DPDMIS_Angular_API.Models;

namespace DPDMIS_Angular_API.Services.LoginService
{
    public interface ILoginService
    {      
        bool LoginDetails(string emailOrMob, string password, out string message, out UsruserModel user);
    }
}
