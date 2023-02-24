using BookShop.Models;

namespace BookShop.Utils
{
    public class Authentication
    {
        private static Authentication _instance;
        private const string KeyId = "Id";
        public int userId { get; private set; }

        private const string KeyEmail = "Email";
        public string email { get; private set; }

        private const string KeyRole = "Role";
        public string role { get; private set; }

        public static Authentication Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Authentication();
                }
                return _instance;
            }
        }
        private Authentication() { }

        //get infor in session
        public bool BindingSession(HttpContext httpContext)
        {
            if (!string.IsNullOrEmpty(httpContext.Session.GetString(KeyEmail)))
            {
                this.userId = (int)httpContext.Session.GetInt32(KeyId);
                this.email = httpContext.Session.GetString(KeyEmail);
                this.role = httpContext.Session.GetString(KeyRole);
                return true;
            }
            return false;
        }

        //authentication user
        public bool Authorization(HttpContext httpContext, string zones)
        {
            if (BindingSession(httpContext))
            {
                List<string> zoneList = zones.Split(',').ToList();
                foreach (string zone in zoneList)
                {
                    if (this.role == zone)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        //create user session  for authentication
        public void SetSession(int userId, string email, string role, HttpContext httpContext)
        {
            httpContext.Session.SetInt32(KeyId, userId);
            httpContext.Session.SetString(KeyEmail, email);
            httpContext.Session.SetString(KeyRole, role);
        }
        
    }
}
