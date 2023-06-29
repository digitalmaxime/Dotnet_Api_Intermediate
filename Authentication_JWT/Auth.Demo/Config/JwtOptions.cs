using System.ComponentModel.DataAnnotations;

namespace Auth.Demo.Config;

public class JwtOptions
{
    [Required]
    public string PrivateKey { get; set; }

}