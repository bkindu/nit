using System.Web;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Keeps the workflow already built for the result screen so PDF export
    /// does not recalculate astronomy or open a preview page.
    /// </summary>
    public static class KundliWorkflowCache
    {
        public const string WorkflowKey = "NirmalaAstrology.Workflow";
        public const string PdfKey = "NirmalaAstrology.Pdf";

        public static void Store(HttpSessionStateBase session, KundliWorkflowResult workflow)
        {
            if (session == null) return;
            session[WorkflowKey] = workflow;
            session.Remove(PdfKey);
        }

        public static KundliWorkflowResult GetWorkflow(HttpSessionStateBase session)
        {
            return session == null ? null : session[WorkflowKey] as KundliWorkflowResult;
        }

        public static void StorePdf(HttpSessionStateBase session, byte[] bytes, string fileName)
        {
            if (session == null || bytes == null) return;
            session[PdfKey] = new CachedPdf { Bytes = bytes, FileName = fileName };
        }

        public static CachedPdf GetPdf(HttpSessionStateBase session)
        {
            return session == null ? null : session[PdfKey] as CachedPdf;
        }

        public sealed class CachedPdf
        {
            public byte[] Bytes { get; set; }
            public string FileName { get; set; }
        }
    }
}
