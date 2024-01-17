using Newtonsoft.Json;
using Web.Areas.Admin.Models;

namespace Web.Extensions
{
    public static class SessionExtensions
    {
        public const string SessionKeyToken = "Token";
        public const string SessionKeyAccountProfile = "AccountProfile"; 
        public const string SessionKeyAccountPermissions = "AccountPermissions";

        public static void SetObjectAsJson(this ISession session, string key, object value)
          => session.SetString(key, JsonConvert.SerializeObject(value));

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }

        public static void SetToken(this ISession session, string value)
          => session.SetString(SessionKeyToken, value);

        public static string GetToken(this ISession session)
          => session.GetString(SessionKeyToken);

        public static void SetAccountProfile(this ISession session, AccountProfileModel model)
          => session.SetObjectAsJson(SessionKeyAccountProfile, model);

        public static AccountProfileModel GetAccountProfile(
          this ISession session
        )
          => session.GetObjectFromJson<AccountProfileModel>(SessionKeyAccountProfile);
          
        public static void SetAccountPermissions(
          this ISession session,
          List<string> model
        )
          => session.SetObjectAsJson(SessionKeyAccountPermissions, model);

        public static List<string> GetAccountPermissions(
          this ISession session
        )
          => session.GetObjectFromJson<List<string>>(SessionKeyAccountPermissions);

        public static T GetComplexData<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            if (data == null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(data);
        }

        public static void SetComplexData(this ISession session, string key, object value)
           => session.SetString(key, JsonConvert.SerializeObject(value));

    }
}
