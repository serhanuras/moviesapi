
using Microsoft.AspNetCore.Identity;

namespace MoviesAPi.PostgreSqlProvider
{

    public class UserLogin : IdentityUserLogin<long> { }
    public class UserRole : IdentityUserRole<long> { }
    public class UserClaim : IdentityUserClaim<long> { }
    public class Role : IdentityRole<long> { }
    public class RoleClaim : IdentityRoleClaim<long> { }
    public class UserToken : IdentityUserToken<long> { }

}
