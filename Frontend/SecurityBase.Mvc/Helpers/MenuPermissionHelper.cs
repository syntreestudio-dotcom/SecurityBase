using Microsoft.AspNetCore.Http;

namespace SecurityBase.Mvc.Helpers;

public static class MenuPermissionHelper
{
    public static dynamic GetMenuPermissions(HttpContext context, string path)
    {
        // Mocked for now - always returns full permissions
        return new {
            CanCreate = true,
            CanEdit = true,
            CanDelete = true
        };
    }
}
