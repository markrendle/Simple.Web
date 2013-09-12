namespace Sandbox
{
    using System;
    using Models;
    using Simple.Web;
    using Simple.Web.Authentication;
    using Simple.Web.Behaviors;
    using Simple.Web.Helpers;

    [UriTemplate("/login")]
    public class LoginPost : IPost<Login>, ILogin
    {
        private static readonly Guid MarkGuid = new Guid("B3EDB5DEFECD42779FBE0D7771D13AD2");
        public Status Post(Login login)
        {
            string redirectLocation;
            if (login.UserName == "mark" && login.Password == "password")
            {
                redirectLocation = string.IsNullOrWhiteSpace(login.ReturnUrl) ? "/" : login.ReturnUrl;
                this.LoggedInUser = new User(MarkGuid, "Mark");
            }
            else
            {
                redirectLocation = UriFromType.Get(() => new LoginForm { ReturnUrl = login.ReturnUrl }).ToString();
            }

            return Status.SeeOther(redirectLocation);
        }

        public IUser LoggedInUser { get; private set; }
    }
}