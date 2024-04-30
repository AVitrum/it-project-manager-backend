namespace DatabaseService.Data.Enums;

[Flags]
public enum PositionPermissions
{
    None = 0,
    CreateProject = 1 << 0,
    UpdateProject = 1 << 1,
    DeleteProject = 1 << 2,
    AddUser = 1 << 3,
    DeleteUser = 1 << 4
}

public static class PositionPermissionsHelper
{
    public static void AddAllPermissions(ref PositionPermissions permissions)
    {
        permissions = Enum.GetValues(typeof(PositionPermissions)).Cast<PositionPermissions>()
            .Where(perm => perm != PositionPermissions.None)
            .Aggregate(permissions, (current, perm) => current | perm);
    }
}