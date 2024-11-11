using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Owner
{
    public int OwnerId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}
