using System;
using System.Collections.Generic;

namespace HelpdeskDAL;

public partial class Role:HelpdeskEntity
{

    public string? RoleName { get; set; }


    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
}
