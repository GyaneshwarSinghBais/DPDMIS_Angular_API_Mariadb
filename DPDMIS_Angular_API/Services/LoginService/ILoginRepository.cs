using DPDMIS_Angular_API.Models;

namespace DPDMIS_Angular_API.Services.LoginService
{
    public interface ILoginRepository
    {
        UsruserModel GetUserByEmailOrMobile(string emailOrMobile);
    }
}
