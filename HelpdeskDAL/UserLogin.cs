using System;
using System.Collections.Generic;

namespace HelpdeskDAL;

public partial class UserLogin :HelpdeskEntity
{

    public string UserPassword { get; set; } = null!;


    public string Email { get; set; } = null!;

    public int? EmployeeId { get; set; }

    public int? CustomerId { get; set; }

    public int? RoleId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Role? Role { get; set; }

    public virtual UserEmail? UserEmail { get; set; }
}
