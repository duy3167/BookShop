using BookShop.Models;

namespace BookShop.Utils
{
    public class Authentication
    {
        private static Authentication _instance;
        private const string KeyId = "Id";
        public int userId { get; set; }

        private const string KeyEmail = "Email";
        public string email { get; set; }

        private const string KeyRole = "Role";
        public string role { get; set; }

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
        public void BindingSession(HttpContext httpContext)
        {
            if (!string.IsNullOrEmpty(httpContext.Session.GetString(KeyEmail)))
            {
                this.userId = (int)httpContext.Session.GetInt32(KeyId);
                this.email = httpContext.Session.GetString(KeyEmail);
                this.role = httpContext.Session.GetString(KeyRole);
            }
        }

        public bool Authorization(string zones)
        {
            List<string> zone = zones.Split(',').ToList();
            bool result = false;
            foreach (string z in zone)
            {
                if(this.role == z)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public void SetSession(int userId, string email, string role, HttpContext httpContext)
        {
            httpContext.Session.SetInt32(KeyId, userId);
            httpContext.Session.SetString(KeyEmail, email);
            httpContext.Session.SetString(KeyRole, role);
        }
        
    }
}
