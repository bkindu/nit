using System.Web;
using System.Web.SessionState;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    public static class BirthDataSessionStore
    {
        public const string Key = "NirmalaAstrology.BirthData";

        public static BirthDataSessionModel Get(HttpSessionStateBase session)
        {
            return session == null ? null : session[Key] as BirthDataSessionModel;
        }

        public static void Set(HttpSessionStateBase session, BirthDataSessionModel data)
        {
            if (session != null) session[Key] = data;
        }

        public static void Clear(HttpSessionStateBase session)
        {
            if (session != null) session.Remove(Key);
        }
    }
}
