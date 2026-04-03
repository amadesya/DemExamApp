using System;
using System.Collections.Generic;

namespace DemExamApp1.Models;

public partial class Role
{
    public string? Name { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
