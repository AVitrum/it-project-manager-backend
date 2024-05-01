namespace DatabaseService.Data.Enums;

[Flags]
public enum PositionPermissions
{
    None = 0,
    CreateProject = 1 << 0,
    UpdateProject = 1 << 1,
    DeleteProject = 1 << 2,
    AddUser = 1 << 3,
    DeleteUser = 1 << 4,
    AddBudget = 1 << 5,
    UpdateBudget = 1 << 6,
}

public static class PositionPermissionsHelper
{
    public static void AddAllPermissions(ref PositionPermissions permissions)
    {
        permissions = Enum.GetValues(typeof(PositionPermissions)).Cast<PositionPermissions>()
            .Where(perm => perm != PositionPermissions.None)
            .Aggregate(permissions, (current, perm) => current | perm);
    }

    public static bool HasPermissions(PositionPermissions permissions, params PositionPermissions[] requiredPermissions)
    {
        return requiredPermissions.All(
            requiredPermission => HasPermission(permissions, requiredPermission));
    }

    private static bool HasPermission(PositionPermissions permissions, PositionPermissions requiredPermission)
    {
        return (permissions & requiredPermission) == requiredPermission;
    }
}