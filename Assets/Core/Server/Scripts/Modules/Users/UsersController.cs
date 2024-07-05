namespace MMC.Server
{
    [Controller]
    public class UsersController : Controller
    {
        public override string routeName => "/users";

        [UseModule] public UsersService service;
    }
}