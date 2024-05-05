using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DatabaseService.Data.DTOs;
using DatabaseService.Data.Enums;

namespace DatabaseService.Data.Models;

public class PositionInCompany
{
    [Key] 
    public long Id { get; init; }
    public required long CompanyId { get; init; }
    public Company Company { get; init; } = null!;
    public IEnumerable<Employee> Employees { get; } = new List<Employee>();
    
    [MaxLength(30)]
    public required string Name { get; init; }
    
    public required long Priority { get; set; }

    [Column(TypeName = "bigint")]
    public PositionPermissions Permissions { get; set; } = PositionPermissions.None;

    public void AddAllPermissions()
    {
        Permissions = Enum.GetValues(typeof(PositionPermissions)).Cast<PositionPermissions>()
            .Aggregate(PositionPermissions.None, (current, perm) => current | perm);
    }

    public bool HasPermissions(params PositionPermissions[] requiredPermissions)
    {
        return requiredPermissions.All(HasPermission);
    }

    public void SetPermissions(PositionInCompanyDto positionInCompanyDto)
    {
        var actions = new Dictionary<Func<PositionInCompanyDto, bool>, PositionPermissions>
        {
            { dto => dto.CreateProject, PositionPermissions.CreateProject },
            { dto => dto.UpdateProject, PositionPermissions.UpdateProject },
            { dto => dto.DeleteProject, PositionPermissions.DeleteProject },
            { dto => dto.AddUser, PositionPermissions.AddUser },
            { dto => dto.UpdateUser, PositionPermissions.UpdateUser },
            { dto => dto.DeleteUser, PositionPermissions.DeleteUser },
            { dto => dto.AddBudget, PositionPermissions.AddBudget },
            { dto => dto.UpdateBudget, PositionPermissions.UpdateBudget },
            { dto => dto.CreatePosition, PositionPermissions.CreatePosition },
            { dto => dto.UpdatePosition, PositionPermissions.UpdatePosition },
            { dto => dto.CreateTask, PositionPermissions.CreateTask },
            { dto => dto.UpdateTask, PositionPermissions.UpdateTask },
            { dto => dto.DeleteTask, PositionPermissions.DeleteTask },
        };

        var newPermissions = actions
            .Where(kv => kv.Key(positionInCompanyDto))
            .Select(kv => kv.Value)
            .Aggregate(PositionPermissions.None, (current, permission) => current | permission);

        newPermissions |= Permissions & ~newPermissions;

        newPermissions = actions.Where(action => 
            !action.Key(positionInCompanyDto)).Aggregate(newPermissions, (current, action) => 
            current & ~action.Value);
        Permissions = newPermissions;
    }

    private bool HasPermission(PositionPermissions requiredPermission)
    {
        return (Permissions & requiredPermission) == requiredPermission;
    }
}