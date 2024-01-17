namespace Web.Extensions
{
    public static class HttpContextExtensions
    {
        public static bool RuleAccessRequest(
          this HttpContext context,
          string area,
          string controller,
          string action = "Index"
        )
        {
            return true;
        }

        public static bool RuleAccessRequest(
          this HttpContext context
        )
        {
            RouteValueDictionary routDataValues = context.GetRouteData().Values;
            string area = routDataValues["area"]?.ToString();
            string controller = routDataValues["controller"]?.ToString();
            string action = routDataValues["action"]?.ToString();

            return RuleAccessRequest(context, area, controller, action);
        }
    }
}
