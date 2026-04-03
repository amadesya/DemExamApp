using System;
using System.Collections.Generic;

namespace DemExamApp1.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime DeliveryDate { get; set; }

    public int PickUpPointId { get; set; }

    public int UserId { get; set; }

    public int Code { get; set; }

    public string Status { get; set; } = null!;

    public virtual PickUpPoint PickUpPoint { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
