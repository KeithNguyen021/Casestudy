using System;
using System.Collections.Generic;

namespace HelpdeskDAL;

public partial class UserEmail : HelpdeskEntity
{

    public string Email { get; set; } = null!;

    public virtual Customer? Customer { get; set; }

    public virtual UserLogin EmailNavigation { get; set; } = null!;

    public virtual Employee? Employee { get; set; }
}
