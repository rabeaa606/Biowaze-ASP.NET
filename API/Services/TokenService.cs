namespace API.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<AppUser> _userManager;
    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {
        _userManager = userManager;

        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
    }


    public async Task<string> CreateToken(AppUser user)
    {
        var claims = new List<Claim>
            {
                 new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),  //add user id to token claims
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName), //add user username to token claims

            };

        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role))); // add role to our token

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);  //signature or encription alg

        var tokenDescriptor = new SecurityTokenDescriptor   //data
        {
            Subject = new ClaimsIdentity(claims), // add the claims
            Expires = DateTime.Now.AddDays(7),  //expires
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
