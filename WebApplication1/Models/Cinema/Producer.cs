using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Cinema;

public partial class Producer
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Nationality { get; set; } = null!;
    public string Email { get; set; } = null!;

    public virtual ICollection<Movie>? Movies { get; set; }  // ← ajoutez le ?
}