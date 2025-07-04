﻿using System;
using System.Collections.Generic;

namespace HelpdeskDAL;

public partial class Employee:HelpdeskEntity
{

    public string? Title { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNo { get; set; }

    public string? Email { get; set; }

    public int DepartmentId { get; set; }


    public virtual ICollection<Call> Calls { get; set; } = new List<Call>();

    public virtual Department Department { get; set; } = null!;

    public virtual UserEmail? EmailNavigation { get; set; }

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
}
