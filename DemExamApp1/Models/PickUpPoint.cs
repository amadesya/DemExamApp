using System;
using System.Collections.Generic;

namespace DemExamApp1.Models;

public partial class PickUpPoint
{
    public int PickUpPoint1 { get; set; }

    public string Address { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
