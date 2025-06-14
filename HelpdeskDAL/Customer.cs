using System;
using System.Collections.Generic;

namespace HelpdeskDAL;

public partial class Customer : HelpdeskEntity
{

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? PhoneNo { get; set; }

    public string Email { get; set; } = null!;


    public string? Title { get; set; }

    public virtual ICollection<Call> Calls { get; set; } = new List<Call>();

    public virtual UserEmail EmailNavigation { get; set; } = null!;

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
}
