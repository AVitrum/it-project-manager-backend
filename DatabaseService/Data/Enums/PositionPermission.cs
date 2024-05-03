namespace DatabaseService.Data.Enums;

[Flags]
public enum PositionPermissions
{
    None = 0,
    CreateProject = 1 << 0,
    UpdateProject = 1 << 1,
    DeleteProject = 1 << 2,
    AddUser = 1 << 3,
    UpdateUser = 1 << 4,
    DeleteUser = 1 << 5,
    AddBudget = 1 << 6,
    UpdateBudget = 1 << 7,
    CreatePosition = 1 << 8,
    UpdatePosition = 1 << 9
}